using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

public interface IOTOAssociationBuilder<TFrom, TTo>
{
  IOTOAssociationBuilder<TFrom, TTo> Navigation(Expression<Func<TFrom, TTo?>> expression);
  
  IOTOAssociationBuilder<TFrom, TTo> Navigation(string navigationName);
  
  IOTOAssociationBuilder<TFrom, TTo> InverseNavigation(Expression<Func<TTo, TFrom?>> expression);
  
  IOTOAssociationBuilder<TFrom, TTo> InverseNavigation(string navigationName);
  
  IOTOAssociationBuilder<TFrom, TTo> UsingForeignKey<TForeignKey>(Expression<Func<TTo, TForeignKey>> expression);
  
  IOTOAssociationBuilder<TFrom, TTo> UsingForeignKey(params Name[] keyNames);
  
  IOTOAssociationBuilder<TFrom, TTo> UsingForeignKey(params string[] keyNames);
}
