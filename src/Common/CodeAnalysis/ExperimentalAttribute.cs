namespace CodeArchitects.Platform.Common.CodeAnalysis;

[AttributeUsage(
  AttributeTargets.Class       |
  AttributeTargets.Constructor |
  AttributeTargets.Delegate    |
  AttributeTargets.Enum        |
  AttributeTargets.Event       |
  AttributeTargets.Field       |
  AttributeTargets.Interface   |
  AttributeTargets.Method      |
  AttributeTargets.Property    |
  AttributeTargets.Struct)]
internal class ExperimentalAttribute : Attribute { }
