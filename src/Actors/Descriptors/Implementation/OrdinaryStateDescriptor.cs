using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class OrdinaryStateDescriptor : StateDescriptor
{
  public OrdinaryStateDescriptor(Type actorType, Type stateType)
    : base(actorType, stateType)
  {
  }

  public override bool IsVirtual => false;

  public override IReadOnlyList<object?>? DefaultValues => null;

  public new void AddField(FieldInfo field)
  {
    base.AddField(field);
  }
}
