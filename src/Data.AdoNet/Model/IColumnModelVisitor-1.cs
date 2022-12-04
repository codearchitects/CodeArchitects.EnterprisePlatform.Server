using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IColumnModelVisitor<out TResult>
{
  TResult VisitForeignKey(IForeignKeyColumnModel column);
  TResult VisitOrdinary(IOrdinaryColumnModel column);
  TResult VisitPrimaryKey(IPrimaryKeyColumnModel column);
  TResult VisitPrimaryAndForeignKey(IPrimaryAndForeignKeyColumnModel column);
}
