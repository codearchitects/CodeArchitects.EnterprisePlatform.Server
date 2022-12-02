using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IPropertyModel : IPropertyModelBase
{
  bool IsPrimaryKey { get; }

  string ColumnName { get; }
  
  short Index { get; }

  TResult Accept<TVisitor, TResult>(in TVisitor visitor)
    where TVisitor : IPropertyModelVisitor<TResult>;

  TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
    where TVisitor : IPropertyModelVisitor<TResult, TState>;
}
