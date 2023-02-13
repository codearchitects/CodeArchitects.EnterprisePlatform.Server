using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.Emit.Reflection;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Actors.Scheduling;

public partial class ActivityTests
{
  public class TaskActivityDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      int arg = 12;

      StandardActorActivity1 sut1 = new() { arg = arg };
      yield return new object?[] { sut1, arg };

      IActorDescriptor descriptor = StandardActorFixture.Descriptor;
      ActivityTypeBuilder typeBuilder = new(DynamicAssembly.NewModule(), new ReflectionILGeneratorProvider());
      Type baseType = typeBuilder.BuildBase(descriptor.ActorType);
      Type activityType = typeBuilder.Build(descriptor.Activities[0], descriptor.ActorType, baseType);
      PropertyInfo argProperty = activityType.GetRequiredProperty(
        name: "0",
        bindingAttr: BindingFlags.Instance | BindingFlags.Public);

      object sut2 = Activator.CreateInstance(activityType)!;
      argProperty.SetValue(sut2, arg);
      yield return new object?[] { sut2, arg };
    }
  }

  public class TaskTActivityDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      int arg = 12;

      StandardActorActivity2 sut1 = new() { arg = arg };
      yield return new object?[] { sut1, arg };

      IActorDescriptor descriptor = StandardActorFixture.Descriptor;
      ActivityTypeBuilder typeBuilder = new(DynamicAssembly.NewModule(), new ReflectionILGeneratorProvider());
      Type baseType = typeBuilder.BuildBase(descriptor.ActorType);
      Type activityType = typeBuilder.Build(descriptor.Activities[1], descriptor.ActorType, baseType);
      PropertyInfo argProperty = activityType.GetRequiredProperty(
        name: "0",
        bindingAttr: BindingFlags.Instance | BindingFlags.Public);

      object sut2 = Activator.CreateInstance(activityType)!;
      argProperty.SetValue(sut2, arg);
      yield return new object?[] { sut2, arg };
    }
  }

  public class ValueTaskActivityDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      StandardActorActivity3 sut1 = new();
      yield return new object?[] { sut1 };

      IActorDescriptor descriptor = StandardActorFixture.Descriptor;
      ActivityTypeBuilder typeBuilder = new(DynamicAssembly.NewModule(), new ReflectionILGeneratorProvider());
      Type baseType = typeBuilder.BuildBase(descriptor.ActorType);
      Type activityType = typeBuilder.Build(descriptor.Activities[2], descriptor.ActorType, baseType);

      object sut2 = Activator.CreateInstance(activityType)!;
      yield return new object?[] { sut2 };
    }
  }

  public class ValueTaskTActivityDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      StandardActorActivity4 sut1 = new();
      yield return new object?[] { sut1 };

      IActorDescriptor descriptor = StandardActorFixture.Descriptor;
      ActivityTypeBuilder typeBuilder = new(DynamicAssembly.NewModule(), new ReflectionILGeneratorProvider());
      Type baseType = typeBuilder.BuildBase(descriptor.ActorType);
      Type activityType = typeBuilder.Build(descriptor.Activities[3], descriptor.ActorType, baseType);

      object sut2 = Activator.CreateInstance(activityType)!;
      yield return new object?[] { sut2 };
    }
  }

  public class VoidActivityDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      StandardActorActivity5 sut1 = new();
      yield return new object?[] { sut1 };

      IActorDescriptor descriptor = StandardActorFixture.Descriptor;
      ActivityTypeBuilder typeBuilder = new(DynamicAssembly.NewModule(), new ReflectionILGeneratorProvider());
      Type baseType = typeBuilder.BuildBase(descriptor.ActorType);
      Type activityType = typeBuilder.Build(descriptor.Activities[4], descriptor.ActorType, baseType);

      object sut2 = Activator.CreateInstance(activityType)!;
      yield return new object?[] { sut2 };
    }
  }

  public class ActivityOverload1DataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      int arg = 12;

      StandardActorActivity6 sut1 = new() { arg = arg };
      yield return new object?[] { sut1, arg };

      IActorDescriptor descriptor = StandardActorFixture.Descriptor;
      ActivityTypeBuilder typeBuilder = new(DynamicAssembly.NewModule(), new ReflectionILGeneratorProvider());
      Type baseType = typeBuilder.BuildBase(descriptor.ActorType);
      Type activityType = typeBuilder.Build(descriptor.Activities[5], descriptor.ActorType, baseType);
      PropertyInfo argProperty = activityType.GetRequiredProperty(
        name: "0",
        bindingAttr: BindingFlags.Instance | BindingFlags.Public);

      object sut2 = Activator.CreateInstance(activityType)!;
      argProperty.SetValue(sut2, arg);
      yield return new object?[] { sut2, arg };
    }
  }

  public class ActivityOverload2DataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      string arg = "12";

      StandardActorActivity7 sut1 = new() { arg = arg };
      yield return new object?[] { sut1, arg };

      IActorDescriptor descriptor = StandardActorFixture.Descriptor;
      ActivityTypeBuilder typeBuilder = new(DynamicAssembly.NewModule(), new ReflectionILGeneratorProvider());
      Type baseType = typeBuilder.BuildBase(descriptor.ActorType);
      Type activityType = typeBuilder.Build(descriptor.Activities[6], descriptor.ActorType, baseType);
      PropertyInfo argProperty = activityType.GetRequiredProperty(
        name: "0",
        bindingAttr: BindingFlags.Instance | BindingFlags.Public);

      object sut2 = Activator.CreateInstance(activityType)!;
      argProperty.SetValue(sut2, arg);
      yield return new object?[] { sut2, arg };
    }
  }

  public class SpecificImplementationActivityDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      PolymorphicActorActivity4 sut1 = new();
      yield return new object?[] { sut1 };

      IActorDescriptor descriptor = PolymorphicActorFixture.Descriptor;
      ActivityTypeBuilder typeBuilder = new(DynamicAssembly.NewModule(), new ReflectionILGeneratorProvider());
      Type baseType = typeBuilder.BuildBase(descriptor.ActorType);
      Type activityType = typeBuilder.Build(descriptor.Activities[3], descriptor.ActorType, baseType);

      object sut2 = Activator.CreateInstance(activityType)!;
      yield return new object?[] { sut2 };
    }
  }
}
