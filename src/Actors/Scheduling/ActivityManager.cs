using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Common.Exceptions;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Scheduling;

internal class ActivityManager<TActor> : IActivityManager<TActor>
  where TActor : class
{
  private delegate Activity ActivityFactory(int implementationId, IReadOnlyList<object?> arguments);

  private static readonly MethodInfo s_listAccessMethod = typeof(IReadOnlyList<object>).GetRequiredMethod(
    name: "get_Item",
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: new[] { typeof(int) });
  private static readonly PropertyInfo s_implementationIdProperty = typeof(Activity).GetRequiredProperty(
    name: nameof(Activity.ImplementationId),
    bindingAttr: BindingFlags.Instance | BindingFlags.Public);

  private readonly IReadOnlyDictionary<string, ActivityDescriptorBatch> _activityBatches;
  private readonly ConcurrentDictionary<IMethodDescriptor, ActivityFactory> _activityFactories;

  private ActivityManager(IReadOnlyDictionary<string, ActivityDescriptorBatch> activityBatches)
  {
    _activityBatches = activityBatches;
    _activityFactories = new(MethodDescriptorEqualityComparer.Instance);
  }

  public Activity<TActor> CreateActivity(int implementationId, MethodInfo method, IReadOnlyList<object?> arguments)
  {
    IMethodDescriptor activity = _activityBatches[method.Name].ResolveActivity(method);

    return (Activity<TActor>)_activityFactories.GetOrAdd(activity, CreateActivityFactory).Invoke(implementationId, arguments);
  }

  public Activity<TActor> CreateActivity(int implementationId, string activityName, IReadOnlyList<object?> arguments)
  {
    if (!_activityBatches.TryGetValue(activityName, out ActivityDescriptorBatch batch) || !batch.TryResolveActivity(arguments, out IMethodDescriptor? activity))
      throw new InvalidOperationException($"Could not find a method with name '{activityName}' and provided parameter types.");

    return (Activity<TActor>)_activityFactories.GetOrAdd(activity, CreateActivityFactory).Invoke(implementationId, arguments);
  }

  private static ActivityFactory CreateActivityFactory(IMethodDescriptor activity)
  {
    IReadOnlyList<FieldInfo> activityFields = activity.ActivityFields;

    ParameterExpression implementationIdParam = Expression.Parameter(typeof(int), "implementationId");
    ParameterExpression argumentsParam = Expression.Parameter(typeof(IReadOnlyList<object>), "arguments");

    List<MemberAssignment> bindings = new()
    {
      Expression.Bind(s_implementationIdProperty, implementationIdParam)
    };

    for (int i = 0; i < activityFields.Count; i++)
    {
      FieldInfo activityField = activityFields[i];

      bindings.Add(Expression.Bind(
        member: activityField,
        expression: Expression.Convert(
          expression: Expression.Call(
            instance: argumentsParam,
            method: s_listAccessMethod,
            arguments: Expression.Constant(i)),
          type: activityField.FieldType)));
    }

    Expression<ActivityFactory> expression = Expression.Lambda<ActivityFactory>(
      body: Expression.MemberInit(
        newExpression: Expression.New(activity.ActivityType),
        bindings: bindings),
      parameters: new[] { implementationIdParam, argumentsParam });

    return expression.Compile();
  }

  public static ActivityManager<TActor> Create(IActorDescriptor actor)
  {
    Dictionary<string, List<IMethodDescriptor>> activityBatches = new();

    foreach (IMethodDescriptor activity in actor.Activities)
    {
      string activityName = activity.Name;
      if (!activityBatches.TryGetValue(activityName, out List<IMethodDescriptor>? batch))
      {
        batch = new();
        activityBatches.Add(activityName, batch);
      }

      batch.Add(activity);
    }

    return new(activityBatches.ToDictionary(
      keySelector: kvp => kvp.Key,
      elementSelector: kvp => new ActivityDescriptorBatch(kvp.Value)));
  }

  private readonly struct ActivityDescriptorBatch
  {
    private readonly IReadOnlyList<IMethodDescriptor> _activities;

    public ActivityDescriptorBatch(IReadOnlyList<IMethodDescriptor> activities)
    {
      _activities = activities;
    }

    public IMethodDescriptor ResolveActivity(MethodInfo method)
    {
      for (int i = 0; i < _activities.Count; i++)
      {
        IMethodDescriptor activity = _activities[i];
        if (activity.ImplementationMethod == method)
          return activity;
      }

      Debug.Fail($"Invalid method supplied: '{method.Name}'.");
      throw Errors.Unreachable;
    }

    public bool TryResolveActivity(IReadOnlyList<object?> arguments, [NotNullWhen(true)] out IMethodDescriptor? activity)
    {
      bool found = false;
      activity = null;

      for (int i = 0; i < _activities.Count; i++)
      {
        IMethodDescriptor currentActivity = _activities[i];

        if (ArgumentsMatch(currentActivity.ParameterTypes, arguments))
        {
          if (found)
            throw new AmbiguousMatchException("More than one method can be matched with the supplied name and arguments.");

          found = true;
          activity = currentActivity;
        }
      }

      return found;
    }

    private static bool ArgumentsMatch(IReadOnlyList<Type> parameterTypes, IReadOnlyList<object?> arguments)
    {
      if (parameterTypes.Count != arguments.Count)
        return false;

      for (int i = 0; i < parameterTypes.Count; i++)
      {
        object? argument = arguments[i];
        Type parameterType = parameterTypes[i];

        if (argument is null)
        {
          if (parameterType.IsValueType)
            return false;
        }
        else
        {
          if (!parameterType.IsInstanceOfType(argument))
            return false;
        }
      }

      return true;
    }
  }

  private class MethodDescriptorEqualityComparer : EqualityComparer<IMethodDescriptor>
  {
    public static readonly MethodDescriptorEqualityComparer Instance = new();

    private MethodDescriptorEqualityComparer() { }

    public override bool Equals(IMethodDescriptor x, IMethodDescriptor y)
    {
      return x.Id == y.Id;
    }

    public override int GetHashCode(IMethodDescriptor obj)
    {
      return obj.Id.GetHashCode();
    }
  }
}
