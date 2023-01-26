namespace System.Reflection;

[Flags]
internal enum BackingFieldNameConvention
{
  AutoGen = 1,
  CamelCase = 2,
  UnderscorePrefix = 4,
  MemberPrefix = 8,
  All = AutoGen | CamelCase | UnderscorePrefix | MemberPrefix
}
