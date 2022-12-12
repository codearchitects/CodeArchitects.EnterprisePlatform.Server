using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

public interface IOTMAssociationBuilder<TFrom, TTo>
{
  IOTMAssociationBuilder<TFrom, TTo> Navigation(Expression<Func<TFrom, IEnumerable<TTo>?>> expression);
  
  IOTMAssociationBuilder<TFrom, TTo> Navigation(string navigationName);
  
  IOTMAssociationBuilder<TFrom, TTo> InverseNavigation(Expression<Func<TTo, TFrom?>> expression);
  
  IOTMAssociationBuilder<TFrom, TTo> InverseNavigation(string navigationName);
  
  IOTMAssociationBuilder<TFrom, TTo> UsingForeignKey<TForeignKey>(Expression<Func<TTo, TForeignKey>> expression);
  
  IOTMAssociationBuilder<TFrom, TTo> UsingForeignKey(params Name[] keyNames);
  
  IOTMAssociationBuilder<TFrom, TTo> UsingForeignKey(params string[] keyNames);
}
