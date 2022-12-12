using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

public interface IMTMAssociationBuilder<TFrom, TTo>
{
  IMTMAssociationBuilder<TFrom, TTo> Navigation(Expression<Func<TFrom, IEnumerable<TTo>?>> expression);
  
  IMTMAssociationBuilder<TFrom, TTo> Navigation(string navigationName);
  
  IMTMAssociationBuilder<TFrom, TTo> InverseNavigation(Expression<Func<TTo, IEnumerable<TFrom>?>> expression);
  
  IMTMAssociationBuilder<TFrom, TTo> InverseNavigation(string navigationName);
  
  IMTMAssociationBuilder<TFrom, TTo> UsingJoinTable(string tableName);
  
  IMTMAssociationBuilder<TFrom, TTo> UsingJoinColumnNames(params string[] keyNames);
}
