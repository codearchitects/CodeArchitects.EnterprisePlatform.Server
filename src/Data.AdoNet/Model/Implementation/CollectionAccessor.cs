extern alias CaPlatformCommon;
using CaPlatformCommon.System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class CollectionAccessor : ICollectionAccessor
{
  private readonly Action<object, object> _addAction;
  private readonly Action<object, object> _removeAction;
  private readonly Func<object, int>? _getNonEnumeratedCount;

  public CollectionAccessor(Action<object, object> addAction, Action<object, object> removeAction, Func<object, int>? getNonEnumeratedCount)
  {
    _addAction = addAction;
    _removeAction = removeAction;
    _getNonEnumeratedCount = getNonEnumeratedCount;
  }

  public void Add(object instance, object value)
  {
    _addAction(instance, value);
  }

  public void Remove(object instance, object value)
  {
    _removeAction(instance, value);
  }

  public bool TryGetNonEnumeratedCount(object instance, out int count)
  {
    if (_getNonEnumeratedCount is not null)
    {
      count = _getNonEnumeratedCount(instance);
      return true;
    }

    count = 0;
    return false;
  }

  public static CollectionAccessor Create<T>(AccessibleMemberComponent<T> memberComponent)
  {
    Debug.Assert(memberComponent.Type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(memberComponent.Type), "Expected a collection-like member.");
    
    Type elementType = memberComponent.Type.GetGenericArguments()[0];
    Type collectionType = typeof(ICollection<>).MakeGenericType(elementType);

    MethodInfo addMethod = collectionType.GetRequiredMethod(
      name: nameof(ICollection<object>.Add),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { elementType });

    MethodInfo removeMethod = collectionType.GetRequiredMethod(
      name: nameof(ICollection<object>.Remove),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { elementType });

    Action<object, object> addAction = CompileCollectionAction(addMethod);
    Action<object, object> removeAction = CompileCollectionAction(removeMethod);
    Func<object, int>? getNonEnumeratedCount = typeof(IReadOnlyCollection<>).MakeGenericType(elementType).IsAssignableFrom(memberComponent.Type)
      ? CompileGetNonEnumeratedCountAction()
      : null;

    return new CollectionAccessor(addAction, removeAction, getNonEnumeratedCount);

    Action<object, object> CompileCollectionAction(MethodInfo method)
    {
      ConstructorInfo exceptionConstructor = typeof(InvalidOperationException).GetRequiredConstructor(
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: new[] { typeof(string) });

      ParameterExpression instanceParam = Expression.Parameter(typeof(object), "instance");
      ParameterExpression valueParam = Expression.Parameter(typeof(object), "value");
      ParameterExpression[] parameters = new[] { instanceParam, valueParam };

      ParameterExpression memberVar = Expression.Variable(typeof(object), "member");
      ParameterExpression[] variables = new[] { memberVar };

      Expression<Action<object, object>> expression = Expression.Lambda<Action<object, object>>(
        Expression.Block(
          type: typeof(void),
          variables: variables,
          expressions: new Expression[]
          {
          Expression.Assign(
            left: memberVar,
            right: Expression.Invoke(
              expression: Expression.Constant(memberComponent.GetValue),
              arguments: instanceParam)),
          Expression.IfThen(
            test: Expression.Not(
              expression: Expression.TypeIs(
                expression: memberVar,
                type: collectionType)),
            ifTrue: Expression.Throw(
              value: Expression.New(
                constructor: exceptionConstructor,
                arguments: Expression.Constant($"The runtime type of member '{memberComponent.Name}' of type '{memberComponent.Member.DeclaringType!.Name}' does not implement 'ICollection<T>'.")))),
          Expression.Call(
            instance: Expression.Convert(memberVar, collectionType),
            method: method,
            arguments: Expression.Convert(
              expression: valueParam,
              type: elementType))
          }),
        parameters: parameters);

      return expression.Compile();
    }

    Func<object, int> CompileGetNonEnumeratedCountAction()
    {
      ParameterExpression instanceParam = Expression.Parameter(typeof(object), "instance");

      Expression<Func<object, int>> expression = Expression.Lambda<Func<object, int>>(
        body: Expression.Property(
          expression: Expression.Convert(
            expression: Expression.Invoke(
              expression: Expression.Constant(memberComponent.GetValue),
              arguments: instanceParam),
            type: typeof(IReadOnlyCollection<>).MakeGenericType(elementType)),
          propertyName: nameof(IReadOnlyCollection<object>.Count)),
        parameters: instanceParam);

      return expression.Compile();
    }
  }
}
