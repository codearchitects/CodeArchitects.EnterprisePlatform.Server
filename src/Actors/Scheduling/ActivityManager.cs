using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Common.Exceptions;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Scheduling;

internal class ActivityManager : IActivityManager
{
  private delegate ActivityPayload PayloadFactory(IReadOnlyList<object?> arguments);

  private static readonly MethodInfo s_listAccessMethod = typeof(IReadOnlyList<object>).GetRequiredMethod(
    name: "get_Item",
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: new[] { typeof(int) });

  private readonly IReadOnlyDictionary<string, ActivityDescriptorBatch> _activityBatches;
  private readonly ConcurrentDictionary<IActivityDescriptor, PayloadFactory> _payloadFactories;

  private ActivityManager(IReadOnlyDictionary<string, ActivityDescriptorBatch> activityBatches)
  {
    _activityBatches = activityBatches;
    _payloadFactories = new(ActivityDescriptorEqualityComparer.Instance);
  }

  public ActivityPayload CreatePayload(MethodInfo method, IReadOnlyList<object?> arguments)
  {
    IActivityDescriptor activity = _activityBatches[method.Name].ResolveActivity(method);

    return _payloadFactories.GetOrAdd(activity, GetPayloadFactory).Invoke(arguments);
  }

  public ActivityPayload CreatePayload(string activityName, IReadOnlyList<object?> arguments)
  {
    if (!_activityBatches.TryGetValue(activityName, out ActivityDescriptorBatch batch) || !batch.TryResolveActivity(arguments, out IActivityDescriptor? activity))
      throw new InvalidOperationException($"Could not find a method with name '{activityName}' and provided parameter types.");

    return _payloadFactories.GetOrAdd(activity, GetPayloadFactory).Invoke(arguments);
  }

  private static PayloadFactory GetPayloadFactory(IActivityDescriptor activity)
  {
    ParameterExpression argumentsParam = Expression.Parameter(typeof(IReadOnlyList<object>), "arguments");

    Expression<PayloadFactory> expression = Expression.Lambda<PayloadFactory>(
      body: Expression.MemberInit(
        newExpression: Expression.New(activity.PayloadType),
        bindings: activity.PayloadFields.Select((field, index) => Expression.Bind(
          member: field,
          expression: Expression.Convert(
            expression: Expression.Call(
              instance: argumentsParam,
              method: s_listAccessMethod,
              arguments: Expression.Constant(index)),
            type: field.FieldType)))),
      parameters: argumentsParam);

    return expression.Compile();
  }

  public static ActivityManager Create(IActorDescriptor actor)
  {
    Dictionary<string, List<IActivityDescriptor>> activityBatches = new();

    foreach (IActivityDescriptor activity in actor.Activities)
    {
      string activityName = activity.ImplementationMethod.Name;
      if (!activityBatches.TryGetValue(activityName, out List<IActivityDescriptor>? batch))
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
    private readonly IReadOnlyList<IActivityDescriptor> _activities;

    public ActivityDescriptorBatch(IReadOnlyList<IActivityDescriptor> activities)
    {
      _activities = activities;
    }

    public IActivityDescriptor ResolveActivity(MethodInfo method)
    {
      for (int i = 0; i < _activities.Count; i++)
      {
        IActivityDescriptor activity = _activities[i];
        if (activity.ImplementationMethod == method)
          return activity;
      }

      Debug.Fail($"Invalid method supplied: '{method.Name}'.");
      throw Errors.Unreachable;
    }

    public bool TryResolveActivity(IReadOnlyList<object?> arguments, [NotNullWhen(true)] out IActivityDescriptor? activity)
    {
      bool found = false;
      activity = null;

      for (int i = 0; i < _activities.Count; i++)
      {
        IActivityDescriptor currentActivity = _activities[i];

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

  private class ActivityDescriptorEqualityComparer : EqualityComparer<IActivityDescriptor>
  {
    public static readonly ActivityDescriptorEqualityComparer Instance = new();

    private ActivityDescriptorEqualityComparer() { }

    public override bool Equals(IActivityDescriptor x, IActivityDescriptor y)
    {
      return x.Id == y.Id;
    }

    public override int GetHashCode(IActivityDescriptor obj)
    {
      return obj.Id.GetHashCode();
    }
  }
}
