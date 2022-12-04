using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IColumnModelVisitor<out TResult, TState>
{
  TResult VisitPrimaryKey(IPrimaryKeyColumnModel column, in TState state);
  TResult VisitForeignKey(IForeignKeyColumnModel column, in TState state);
  TResult VisitOrdinary(IOrdinaryColumnModel column, in TState state);
  TResult VisitPrimaryAndForeignKey(IPrimaryAndForeignKeyColumnModel column, in TState state);
}
