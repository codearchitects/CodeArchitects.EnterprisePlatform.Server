using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IPropertyModelVisitor<TResult>
{
  TResult VisitPrimaryKey(IPrimaryKeyPropertyModel property);
  TResult VisitForeignKey(IForeignKeyPropertyModel property);
  TResult VisitOrdinary(IOrdinaryPropertyModel property);
}
