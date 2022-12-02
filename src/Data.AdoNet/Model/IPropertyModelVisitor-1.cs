using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IPropertyModelVisitor<out TResult>
{
  TResult VisitForeignKey(IForeignKeyPropertyModel property);
  TResult VisitOrdinary(IOrdinaryPropertyModel property);
  TResult VisitPrimaryKey(IPrimaryKeyPropertyModel property);
  TResult VisitPrimaryAndForeignKey(IPrimaryAndForeignKeyPropertyModel property);
}
