using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.GraphQL.Model;
using StrawberryShake;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class VariableExtractorProvider : IVariableExtractorProvider
{
  private delegate void Populate<TVariables>(IDictionary<string, object?> variableDictionary, IDictionary<string, Upload?> fileDictionary, TVariables variables);

  private static readonly ImmutableDictionary<string, object?> s_emptyVariableDictionary = ImmutableDictionary.Create<string, object?>();

  private static readonly ImmutableDictionary<string, Upload?> s_emptyFileDictionary = ImmutableDictionary.Create<string, Upload?>();

  private static readonly Type s_objectDictionaryType = typeof(IDictionary<string, object?>);

  private static readonly Type s_uploadDictionaryType = typeof(IDictionary<string, Upload?>);

  private static readonly MethodInfo s_addObjectMethod = s_objectDictionaryType.GetRequiredMethod(
    name: nameof(IDictionary<string, object?>.Add),
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: new[] { typeof(string), typeof(object) });

  private static readonly MethodInfo s_addUploadMethod = s_uploadDictionaryType.GetRequiredMethod(
    name: nameof(IDictionary<string, Upload?>.Add),
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: new[] { typeof(string), typeof(Upload?) });

  private static readonly ConstructorInfo s_nullableUploadConstructor = typeof(Upload?).GetRequiredConstructor(
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: new[] { typeof(Upload) });

  private readonly IModel _model;
  private readonly IILGeneratorProvider _ilProvider;
  private readonly ConcurrentDictionary<Type, Delegate> _extractors;

  public VariableExtractorProvider(IModel model, IILGeneratorProvider ilProvider)
  {
    _model = model;
    _ilProvider = ilProvider;
    _extractors = new();
  }

  public VariableExtractor<TVariables> GetExtractor<TVariables>()
    where TVariables : notnull
  {
    return (VariableExtractor<TVariables>)_extractors.GetOrAdd(typeof(TVariables), BuildExtractor, this);

    static Delegate BuildExtractor(Type variablesType, VariableExtractorProvider self)
      => self.BuildExtractor<TVariables>(variablesType);
  }

  private VariableExtractor<TVariables> BuildExtractor<TVariables>(Type variablesType)
    where TVariables : notnull
  {
    string variableTypeFullName = variablesType.FullName;
    Debug.Assert(variableTypeFullName is not null);

    (List<IVariable> objectVariables, List<IVariable> uploadVariables) = GetVariables(variablesType);
    Populate<TVariables> populate = BuildPopulateMethod<TVariables>(variableTypeFullName, objectVariables, uploadVariables);

    return delegate (TVariables variables)
    {
      IDictionary<string, object?> variableDictionary = objectVariables.Count == 0
        ? s_emptyVariableDictionary
        : new Dictionary<string, object?>();

      IDictionary<string, Upload?> fileDictionary = uploadVariables.Count == 0
        ? s_emptyFileDictionary
        : new Dictionary<string, Upload?>();

      populate(variableDictionary, fileDictionary, variables);

      return ((IReadOnlyDictionary<string, object?>)variableDictionary, (IReadOnlyDictionary<string, Upload?>)fileDictionary);
    };
  }

  private Populate<TVariables> BuildPopulateMethod<TVariables>(string variableTypeFullName, List<IVariable> objectVariables, List<IVariable> uploadVariables)
    where TVariables : notnull
  {
    DynamicMethod method = new($"<{variableTypeFullName.Replace('.', '_')}>Populate", typeof(void), new[] { s_objectDictionaryType, s_uploadDictionaryType, typeof(TVariables) });
    IILGenerator il = _ilProvider.GetILGenerator(method);

    foreach (IVariable objectVariable in objectVariables)
    {
      PropertyInfo variableProperty = objectVariable.ClrProperty;

      il.Emit(OpCodes.Ldarg_0);                               // Load $variableDictionary                   | Stack: $variableDictionary
      il.Emit(OpCodes.Ldstr, objectVariable.Name.Camelize()); // Push the camelized name := $name           | Stack: $variableDictionary, $name
      il.Emit(OpCodes.Ldarg_2);                               // Load $variables                            | Stack: $variableDictionary, $name, $variables
      il.Emit(OpCodes.Callvirt, variableProperty.GetMethod);  // Load the variable's property := $prop      | Stack: $variableDictionary, $name, $prop
      if (variableProperty.PropertyType.IsValueType)          //                                            | 
      {                                                       //                                            | 
        il.Emit(OpCodes.Box, variableProperty.PropertyType);  // Box $prop                                  | Stack: $variableDictionary, $name, $prop
      }                                                       //                                            | 
      il.Emit(OpCodes.Callvirt, s_addObjectMethod);           // Call $variableDictionary.Add($name, $prop) | Stack: -
    }

    foreach (IVariable uploadVariable in uploadVariables)
    {
      PropertyInfo variableProperty = uploadVariable.ClrProperty;

      il.Emit(OpCodes.Ldarg_1);                               // Load $fileDictionary                   | Stack: $fileDictionary
      il.Emit(OpCodes.Ldstr, uploadVariable.Name.Camelize()); // Push the camelized name := $name       | Stack: $fileDictionary, $name
      il.Emit(OpCodes.Ldarg_2);                               // Load $variables                        | Stack: $fileDictionary, $name, $variables
      il.Emit(OpCodes.Callvirt, variableProperty.GetMethod);  // Load the variable's property := $prop  | Stack: $fileDictionary, $name, $prop
      if (variableProperty.PropertyType == typeof(Upload))    //                                        | 
      {                                                       //                                        | 
        il.Emit(OpCodes.Newobj, s_nullableUploadConstructor); // Wrap $prop in a Nullable instance      | Stack: $fileDictionary, $name, $prop
      }                                                       //                                        | 
      il.Emit(OpCodes.Callvirt, s_addUploadMethod);           // Call $fileDictionary.Add($name, $prop) | Stack: -
    }

    il.Emit(OpCodes.Ret);                                     // Return

    Populate<TVariables> populate = (Populate<TVariables>)method.CreateDelegate(typeof(Populate<TVariables>));
    return populate;
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
