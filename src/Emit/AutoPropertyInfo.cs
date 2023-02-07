using System.Reflection;

namespace CodeArchitects.Platform.Emit;

internal record AutoPropertyInfo(PropertyInfo Property, FieldInfo BackingField);
