using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Factory;

internal class MemberMetadata : IEquatable<MemberMetadata>
{
  public MemberMetadata(MemberInfo member, Type type)
  {
    Member = member;
    Type = type;
  }

  public MemberInfo Member { get; }

  public Type Type { get; }

  public bool Equals(MemberMetadata? other)
  {
    if (other is null) return false; // Handle null case explicitly
    return Member == other.Member && Type == other.Type;
  }

  public override bool Equals(object? obj)
  {
    return obj is MemberMetadata other && Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Member, Type);
  }
}
