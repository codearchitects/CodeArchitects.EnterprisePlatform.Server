using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface INavigationModel : IPropertyModelBase
{
  int Id { get; }

  int Index { get; }

  bool IsAggregation { get; }

  bool IsOnDependent { get; }

  [MemberNotNullWhen(true, nameof(ElementType))]
  bool IsCollection { get; }
  
  Type? ElementType { get; }
  
  IEntityModel From { get; }
  
  IEntityModel To { get; }

  IForeignKeyModel ForeignKey { get; }
}
