using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal abstract class StateDescriptor : IStateDescriptor
{
  protected readonly Type _actorType;
  private readonly List<FieldInfo> _fields;

  public StateDescriptor(Type actorType, Type stateType)
  {
    _actorType = actorType;
    StateType = stateType;
    _fields = new();
  }

  public abstract bool IsVirtual { get; }

  public abstract IReadOnlyList<object?>? DefaultValues { get; }

  public bool IsStateless => false;

  public Type StateType { get; }

  public IReadOnlyList<FieldInfo> Fields => _fields;

  protected void AddField(FieldInfo field)
  {
    CheckFieldType(field);
    _fields.Add(field);
  }

  private void CheckFieldType(FieldInfo field)
  {
    Type nestedType = GetNestedType(field, field.FieldType, 0);
    if (nestedType.IsInterface || nestedType.IsAbstract)
      throw InvalidActorException.InvalidStateType(_actorType, field);
  }

  private Type GetNestedType(FieldInfo field, Type currentType, int genericNesting)
  {
    if (currentType.IsGenericType)
    {
      Type parameterTypeDefinition = currentType.GetGenericTypeDefinition();
      Type firstTypeArgument = currentType.GetGenericArguments()[0];

      if (IsCollectionLikeType(parameterTypeDefinition))
        return GetNestedType(field, firstTypeArgument, genericNesting + 1);

      if (IsDictionaryLikeType(parameterTypeDefinition))
      {
        Type secondTypeArgument = currentType.GetGenericArguments()[1];
        if (IsSupportedKeyType(firstTypeArgument))
          return GetNestedType(field, secondTypeArgument, genericNesting + 1);

        throw InvalidActorException.InvalidStateType(_actorType, field);
      }
    }

    return currentType;
  }

  private static bool IsCollectionLikeType(Type type)
  {
    return
      type == typeof(IEnumerable<>)         ||
      type == typeof(IReadOnlyCollection<>) ||
      type == typeof(ICollection<>)         ||
      type == typeof(IReadOnlyList<>)       ||
      type == typeof(IList<>)               ||
      type == typeof(List<>)                ||
      type == typeof(LinkedList<>)          ||
      type == typeof(ISet<>)                ||
      type == typeof(HashSet<>)             ||
      type == typeof(SortedSet<>)           ||
      type == typeof(Queue<>);
  }

  private static bool IsDictionaryLikeType(Type type)
  {
    return
      type == typeof(IReadOnlyDictionary<,>) ||
      type == typeof(IDictionary<,>)         ||
      type == typeof(Dictionary<,>)          ||
      type == typeof(SortedDictionary<,>)    ||
      type == typeof(KeyValuePair<,>);
  }

  private static bool IsSupportedKeyType(Type type)
  {
    return
      type == typeof(bool)           ||
      type == typeof(byte)           ||
      type == typeof(DateTime)       ||
      type == typeof(DateTimeOffset) ||
      type == typeof(decimal)        ||
      type == typeof(double)         ||
      type == typeof(Guid)           ||
      type == typeof(short)          ||
      type == typeof(int)            ||
      type == typeof(long)           ||
      type == typeof(object)         ||
      type == typeof(sbyte)          ||
      type == typeof(float)          ||
      type == typeof(string)         ||
      type == typeof(ushort)         ||
      type == typeof(uint)           ||
      type == typeof(ulong)          ||
      type.BaseType == typeof(Enum);
  }
}
