using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit;

internal static class TypeBuilderExtensions
{
  public static MethodBuilder DefineMethodOverrideFromDeclaration(this TypeBuilder type, MethodInfo declaration, MethodAttributes attributes)
  {
    ParameterInfo[] parameters = declaration.GetParameters();

    MethodBuilder method = type.DefineMethod(
      name: declaration.Name,
      attributes: attributes,
      callingConvention: CallingConventions.HasThis,
      returnType: declaration.ReturnType,
      returnTypeRequiredCustomModifiers: declaration.ReturnParameter.GetRequiredCustomModifiers(),
      returnTypeOptionalCustomModifiers: declaration.ReturnParameter.GetOptionalCustomModifiers(),
      parameterTypes: parameters.Map(parameter => parameter.ParameterType),
      parameterTypeRequiredCustomModifiers: parameters.Map(parameter => parameter.GetRequiredCustomModifiers()),
      parameterTypeOptionalCustomModifiers: parameters.Map(parameter => parameter.GetOptionalCustomModifiers()));

    foreach (ParameterInfo parameter in parameters)
    {
      method.DefineParameter(
        position: parameter.Position + 1,
        attributes: parameter.Attributes,
        strParamName: parameter.Name);
    }

    type.DefineMethodOverride(method, declaration);

    return method;
  }

  public static string GetComponentTypeName(this Type type, string componentName)
  {
    if (type.FullName == null)
    {
      throw new InvalidOperationException("Type.FullName is null.");
    }

    int index = type.FullName.LastIndexOf(type.Name);
    Debug.Assert(index >= 0);

    return type.FullName.Remove(index, type.Name.Length) + $"<{type.Name}>{componentName}";
  }
}
