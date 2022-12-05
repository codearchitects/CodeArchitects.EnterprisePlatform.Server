using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

public interface IAssociationBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  IAssociationBuilderOTO<TFrom, TTo> OneToOne();
  IAssociationBuilderOTM<TFrom, TTo> OneToMany();
  IAssociationBuilderMTM<TFrom, TTo> ManyToMany();
}


// One to one

public interface IAssociationBuilderOTO<TFrom, TTo>
{
  IAssociationBuilderOTO<TFrom, TTo> Navigation(Expression<Func<TFrom, TTo?>> expression);
  IAssociationBuilderOTO<TFrom, TTo> Navigation(string navigationName);
  IAssociationBuilderOTO<TFrom, TTo> InverseNavigation(Expression<Func<TTo, TFrom?>> expression);
  IAssociationBuilderOTO<TFrom, TTo> InverseNavigation(string navigationName);
  IAssociationBuilderOTO<TFrom, TTo> UsingForeignKey<TForeignKey>(Expression<Func<TTo, TForeignKey>> expression);
  IAssociationBuilderOTO<TFrom, TTo> UsingForeignKey<TForeignKey>(params string[] keyNames);
}


// One to many

public interface IAssociationBuilderOTM<TFrom, TTo>
{
  IAssociationBuilderOTM<TFrom, TTo> Navigation(Expression<Func<TFrom, IEnumerable<TTo>?>> expression);
  IAssociationBuilderOTM<TFrom, TTo> Navigation(string navigationName);
  IAssociationBuilderOTM<TFrom, TTo> InverseNavigation(Expression<Func<TTo, TFrom?>> expression);
  IAssociationBuilderOTM<TFrom, TTo> InverseNavigation(string navigationName);
  IAssociationBuilderOTM<TFrom, TTo> UsingForeignKey<TForeignKey>(Expression<Func<TTo, TForeignKey>> expression);
  IAssociationBuilderOTM<TFrom, TTo> UsingForeignKey<TForeignKey>(params string[] keyNames);
}


// Many to many

public interface IAssociationBuilderMTM<TFrom, TTo>
{
  IAssociationBuilderMTM<TFrom, TTo> Navigation(Expression<Func<TFrom, IEnumerable<TTo>?>> expression);
  IAssociationBuilderMTM<TFrom, TTo> Navigation(string navigationName);
  IAssociationBuilderMTM<TFrom, TTo> InverseNavigation(Expression<Func<TTo, TFrom?>> expression);
  IAssociationBuilderMTM<TFrom, TTo> InverseNavigation(string navigationName);
  IAssociationBuilderMTM<TFrom, TTo> UsingJoinTable(string tableName);
  IAssociationBuilderMTM<TFrom, TTo> UsingForeignKey(params string[] keyNames);
}
