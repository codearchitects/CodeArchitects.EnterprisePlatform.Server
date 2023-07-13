using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Model;
using StrawberryShake;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class VariableExtractorProvider : IVariableExtractorProvider
{
  private static readonly Type s_dictionaryTupleType = typeof((IReadOnlyDictionary<string, object?>, IReadOnlyDictionary<string, Upload?>));

  private static readonly Type s_objectDictionaryType = typeof(Dictionary<string, object?>);

  private static readonly Type s_uploadDictionaryType = typeof(Dictionary<string, Upload?>);

  private static readonly ConstructorInfo s_dictionaryTupleConstructor = s_dictionaryTupleType.GetRequiredConstructor(
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: new[] { typeof(IReadOnlyDictionary<string, object?>), typeof(IReadOnlyDictionary<string, Upload?>) });

  private static readonly ConstructorInfo s_objectDictionaryConstructor = s_objectDictionaryType.GetRequiredConstructor(
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: Type.EmptyTypes);

  private static readonly ConstructorInfo s_uploadDictionaryConstructor = s_uploadDictionaryType.GetRequiredConstructor(
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: Type.EmptyTypes);

  private static readonly MethodInfo s_addObjectMethod = s_objectDictionaryType.GetRequiredMethod(
    name: nameof(Dictionary<string, object?>.Add),
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: new[] { typeof(string), typeof(object) });

  private static readonly MethodInfo s_addUploadMethod = s_uploadDictionaryType.GetRequiredMethod(
    name: nameof(Dictionary<string, Upload?>.Add),
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: new[] { typeof(string), typeof(Upload?) });

  private static readonly ConstructorInfo s_nullableUploadConstructor = typeof(Upload?).GetRequiredConstructor(
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: new[] { typeof(Upload) });

  private readonly Synchronizer _synchronizer;
  private readonly IModel _model;
  private readonly ConcurrentDictionary<Type, Delegate> _extractors;

  public VariableExtractorProvider(Synchronizer synchronizer, IModel model)
  {
    _synchronizer = synchronizer;
    _model = model;
    _extractors = new();
  }

  public VariableExtractor<TVariables> GetExtractor<TVariables>()
    where TVariables : notnull
  {
    VariableExtractor<TVariables>? extractor;
    Type variablesType = typeof(TVariables);
    Debug.Assert(variablesType != typeof(EmptyVariables));

    if (TryGetExtractor(variablesType, out extractor))
      return extractor;

    using (_synchronizer.Sync(variablesType))
    {
      if (TryGetExtractor(variablesType, out extractor))
        return extractor;

      extractor = CompileExtractor<TVariables>(variablesType);
      _extractors[variablesType] = extractor;
    }

    return extractor;
  }

  private VariableExtractor<TVariables> CompileExtractor<TVariables>(Type variablesType)
    where TVariables : notnull
  {
    string variableTypeFullName = variablesType.FullName;
    Debug.Assert(variableTypeFullName is not null);

    (List<IVariable> objectVariables, List<IVariable> uploadVariables) = GetVariables(variablesType);

    DynamicMethod method = new($"<{variableTypeFullName.Replace('.', '_')}>Extract", s_dictionaryTupleType, new[] { typeof(TVariables) });
    ILGenerator il = method.GetILGenerator();

    il.Emit(OpCodes.Newobj, s_objectDictionaryConstructor);            // Create $objectVars := new Dictionary<string, object?>() | Stack: $objectVars
    foreach (IVariable objectVariable in objectVariables)              //                                                         | 
    {                                                                  //                                                         | 
      il.Emit(OpCodes.Dup);                                            // Duplicate $objectVars                                   | Stack: $objectVars, $objectVars
      il.Emit(OpCodes.Ldstr, objectVariable.Name.Camelize());          // Push the camelized name := $name                        | Stack: $objectVars, $objectVars, $name
      il.Emit(OpCodes.Ldarg_0);                                        // Push $variables                                         | Stack: $objectVars, $objectVars, $name, $variables
      il.Emit(OpCodes.Callvirt, objectVariable.ClrProperty.GetMethod); // Load the variable's property := $prop                   | Stack: $objectVars, $objectVars, $name, $prop
      il.Emit(OpCodes.Callvirt, s_addObjectMethod);                    // Call $objectVars.Add($name, $prop)                      | Stack: $objectVars
    }

    il.Emit(OpCodes.Newobj, s_uploadDictionaryConstructor);            // Create $uploadVars := new Dictionary<string, object?>() | Stack: $objectVars, $uploadVars
    foreach (IVariable uploadVariable in uploadVariables)              //                                                         | 
    {                                                                  //                                                         | 
      il.Emit(OpCodes.Dup);                                            // Duplicate $uploadVars                                   | Stack: $objectVars, $uploadVars, $uploadVars
      il.Emit(OpCodes.Ldstr, uploadVariable.Name.Camelize());          // Push the camelized name := $name                        | Stack: $objectVars, $uploadVars, $uploadVars, $name
      il.Emit(OpCodes.Ldarg_0);                                        // Push $variables                                         | Stack: $objectVars, $uploadVars, $uploadVars, $name, $variables
      il.Emit(OpCodes.Callvirt, uploadVariable.ClrProperty.GetMethod); // Load the variable's property := $prop                   | Stack: $objectVars, $uploadVars, $uploadVars, $name, $prop
      if (uploadVariable.ClrProperty.PropertyType == typeof(Upload))   //                                                         |
      {                                                                //                                                         |
        il.Emit(OpCodes.Newobj, s_nullableUploadConstructor);          // Create a nullable $prop := $prop                        | Stack: $objectVars, $uploadVars, $uploadVars, $name, $prop
      }                                                                //                                                         |
      il.Emit(OpCodes.Callvirt, s_addObjectMethod);                    // Call $uploadVars.Add($name, $prop)                      | Stack: $objectVars, $uploadVars
    }

    il.Emit(OpCodes.Newobj, s_dictionaryTupleConstructor);             // Create $tuple = ($objectVars, $uploadVars)              | Stack: $tuple
    il.Emit(OpCodes.Ret);                                              // Return                                                  | Stack: $tuple

    return (VariableExtractor<TVariables>)method.CreateDelegate(typeof(VariableExtractor<TVariables>));
  }

  private (List<IVariable> ObjectVariables, List<IVariable> UploadVariables) GetVariables(Type variablesType)
  {
    IReadOnlyCollection<IVariable> variables = _model.GetVariables(variablesType);

    List<IVariable> objectVariables = new(variables.Count);
    List<IVariable> uploadVariables = new();

    foreach (IVariable variable in variables)
    {
      if (variable.Type.ClrType == typeof(Upload) || variable.Type.ClrType == typeof(Upload?))
      {
        uploadVariables.Add(variable);
      }
      else
      {
        objectVariables.Add(variable);
      }
    }

    return (objectVariables, uploadVariables);
  }

  private bool TryGetExtractor<TVariables>(Type variablesType, [NotNullWhen(true)] out VariableExtractor<TVariables>? extractor)
    where TVariables : notnull
  {
    if (_extractors.TryGetValue(variablesType, out Delegate untypedExtractor))
    {
      extractor = (VariableExtractor<TVariables>)untypedExtractor;
      return true;
    }

    extractor = null;
    return false;
  }
}
