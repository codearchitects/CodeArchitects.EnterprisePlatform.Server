using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IPropertyModelVisitor<TResult, TState>
{
  TResult VisitPrimaryKey(IPrimaryKeyPropertyModel property, in TState state);
  TResult VisitForeignKey(IForeignKeyPropertyModel property, in TState state);
  TResult VisitOrdinary(IOrdinaryPropertyModel property, in TState state);
  TResult VisitPrimaryAndForeignKey(IPrimaryAndForeignKeyPropertyModel property, in TState state);
}
