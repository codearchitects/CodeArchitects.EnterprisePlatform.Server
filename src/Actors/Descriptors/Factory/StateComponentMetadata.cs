using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

internal abstract class StateComponentMetadata<TActor> : IStateComponentMetadata
  where TActor : class
{
  protected StateComponentMetadata(int index, MemberInfo member, Type type)
  {
    Index = index;
    Member = member;
    Type = type;

    CheckStateType();
  }

  public int Index { get; }

  public MemberInfo Member { get; }

  public Type Type { get; }

  public abstract bool HasDefaultValue(out object? defaultComponentValue);

  public abstract bool IsActorIdSource(out PropertyInfo? actorIdProperty);

  private void CheckStateType()
  {
    Type nestedType = GetNestedType(Type, 0);
    if (nestedType.IsInterface || nestedType.IsAbstract)
      throw InvalidActorException.InvalidStateType(typeof(TActor), Member);
  }

  private Type GetNestedType(Type currentType, int genericNesting)
  {
    if (currentType.IsGenericType)
    {
      Type parameterTypeDefinition = currentType.GetGenericTypeDefinition();
      Type firstTypeArgument = currentType.GetGenericArguments()[0];

      if (IsCollectionLikeType(parameterTypeDefinition))
        return GetNestedType(firstTypeArgument, genericNesting + 1);

      if (IsDictionaryLikeType(parameterTypeDefinition))
      {
        Type secondTypeArgument = currentType.GetGenericArguments()[1];
        if (IsSupportedKeyType(firstTypeArgument))
          return GetNestedType(secondTypeArgument, genericNesting + 1);

        throw InvalidActorException.InvalidStateType(typeof(TActor), Member);
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
