using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface INavigationModel : IPropertyBase
{
  int Id { get; }
  int Index { get; }
  bool IsOnDependent { get; }

  [MemberNotNullWhen(true, nameof(ElementType))]
  bool IsCollection { get; }
  IEntityModel From { get; }
  IEntityModel To { get; }
  IReadOnlyList<IKeyPair> Keys { get; }
  Type? ElementType { get; }
}
