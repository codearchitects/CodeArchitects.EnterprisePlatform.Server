using CodeArchitects.Platform.Common;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class VirtualStateDescriptor : StateDescriptor
{
  private readonly List<object?> _defaultValues;

  public VirtualStateDescriptor(Type actorType, Type stateType)
    : base(actorType, stateType)
  {
    _defaultValues = new();
  }

  public override bool IsVirtual => true;

  public override IReadOnlyList<object?>? DefaultValues => _defaultValues;

  public void AddField(FieldInfo field, Optional<object?> defaultValue)
  {
    AddField(field);

    _defaultValues.Add(GetDefaultValue(field, defaultValue));
  }

  private object? GetDefaultValue(FieldInfo field, Optional<object?> defaultValue)
  {
    if (defaultValue.HasValue)
    {
      if (!field.FieldType.IsInstanceOfType(defaultValue.Value))
        throw InvalidActorException.InvalidDefaultValue(_actorType, field);
      
      return defaultValue.Value;
    }

    try
    {
      return Activator.CreateInstance(field.FieldType);
    }
    catch
    {
      throw InvalidActorException.ActorCannotBeVirtual(_actorType);
    }
  }
}
