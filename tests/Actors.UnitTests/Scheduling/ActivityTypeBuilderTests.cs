using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Fixtures;
using CodeArchitects.Platform.Actors.Fixtures.Examples;
using CodeArchitects.Platform.Emit.Testing;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Scheduling;

public class ActivityTypeBuilderTests
{
  private readonly ModuleBuilder _module;

  public ActivityTypeBuilderTests()
  {
    _module = DynamicAssembly.CreateModule();
  }

  [Fact]
  public void BuildBase_ShouldBuildBaseActivityType()
  {
    // Arrange
    Type actorType = typeof(StandardActor);

    FakeILGeneratorProvider ilProvider = new();
    ActivityTypeBuilder sut = new(_module, ilProvider);

    // Act
    Type activityBaseType = sut.BuildBase(actorType);

    // Assert
    activityBaseType.Namespace.Should().Be(actorType.Namespace);
    activityBaseType.Name.Should().Be($"<{actorType.Name}>{ActivityTypeBuilder.ComponentName}");
    activityBaseType.Should().BeAbstract();
  }

  [Fact]
  public void Build_ShouldBuildActivityType_WhenActivityIsTaskMethod() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKzAzNnJgGVg5GpXLFKAQQDGwAPbFUAb1SZsvGADZsmgLKlghOZQAUtYJnF0AlKrWYAvAD5Mh4nIDumGqS8A5OWAASQBbAAcIUhDSGmBSSgBRAA9pUjDgMDkaE2tOAF9UHksAIwBnYGJyWT5MGQyANzBgAE9lOxhecjKKqotzTCDKTCVMOgMGTAKuFDUOzH7QiKiY4QysweHR8cxS7ampoq7yyur4WtkwRpaAHnw6hRcQc4am1pnMD0JSMkw72QVME94G13nMjj1TtpkqRpABXOKSUrNGjSEx/eTESz/YgAGkwAGERKkIBByGsaPg5ABrGKYaRE0gksmZCnUmK5VAHNCdbonCxnIQiMQSe7EOqXV6A54Sm6C0TiKTYpwgrlzM5afSGYziq7NKVy4WKjE614gtRhYiXMmkeaxTAAfSs9qp9vtACEqlTaHQAGJgRmUTjtXhyerfS2UG0LIbOTAIIOg3j9STEOh2FTvNRjCyxx2p52uj3SL30P0Bzj2HYGRwuPN0Avuz3essQGOYerkCCw0gVyaFROYUPhsCRnSYaFwhFIlEmA0K0VYjF4wkoxmk8mUmk0OkMpkbtk0WzvDOVmAAdkXClwGoMRlMdYbRZLvv9rY5732QA=
  {
    // Arrange
    Type actorType = typeof(StandardActor);
    Type baseType = typeof(StandardActorActivity);
    IMethodDescriptor descriptor = StandardActorFixture.Descriptor.Activities[0];
    ParameterInfo[] parameters = descriptor.ImplementationMethod.GetParameters();

    string argPropertyName = parameters[0].Name!;
    string argFieldName = $"<{argPropertyName}>k__BackingField";
    Type argType = parameters[0].ParameterType;

    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator idGetterIL = ilProvider.AddGenerator();
    FakeILGenerator argGetterIL = ilProvider.AddGenerator();
    FakeILGenerator argSetterIL = ilProvider.AddGenerator();
    FakeILGenerator executeAsyncMethodIL = ilProvider.AddGenerator();
    ActivityTypeBuilder sut = new(_module, ilProvider);

    // Act
    Type activityType = sut.Build(descriptor, actorType, baseType);

    // Assert
    activityType.Namespace.Should().Be(actorType.Namespace);
    activityType.Name.Should().Be($"<{actorType.Name}>{ActivityTypeBuilder.ComponentName}1");
    activityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Should().HaveCount(1)
      .And.ContainSingle(property =>
        property.Name == argPropertyName &&
        property.PropertyType == argType &&
        property.CanRead &&
        property.CanWrite);

    idGetterIL.VerifyIL(_ => _
      .Ldc_I4_1()
      .Ret());

    argGetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld(argFieldName)
      .Ret());

    argSetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld(argFieldName)
      .Ret());

    executeAsyncMethodIL.VerifyNoLocals();

    executeAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Ldarg_0()
      .Ldfld(argFieldName)
      .Callvirt(descriptor.ImplementationMethod)
      .Ret());
  }

  [Fact]
  public void Build_ShouldBuildActivityType_WhenActivityIsTaskTMethod() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKzAzNnJgGVg5GpXLFKAQQDGwAPbFUAb1SZsvGADYAPLWAA+bJvwBZUsEJzKACj2ZxdADSYAwiOmkIEcsDBya+HIA1qQ0mNLunt6+/oEhNACUqmqYALyGFsRyAO6YNKS5AHJywACSALYADhCk5aHApJQAogAeHpUxNNYJnAC+qDz2AEYAzsDE5LJ8mDK+AG5gwACeyskwvOSj45PAmHallJhKmHTmDJj9XChq63s0uxXVtfU+fjQHRydnmCPfl5eDTZjCZTeAzWRgBbLbT4WYKQwgcHzRYra6YbKEUhkTCw2QKTCI+CrNG3IHbUGaTCtUjSACuDUkIyWNGk1lx8mI9jxxGcbhZUVesWCoXCkS8goCwsSfQGaA2WxBuzBQhEYgkcOIs0hKIJSO10JVonEUm5+mJAKutzBWlM5ksJuRy11hrVDoUWqhqJUaMqxEhPlId12AH0HMGgsHgwAhSZBWh0ABiYE8lE4a14cjmWL9lED+0O6UwcDTJN4dkkxDoyW9KS+u0Locr4cjMekcfoSZTnFrv3rhkbdGb0dj8c7EALmDm5AgtNI3YuspuGazxBzga0VJaNPppEZzNZLuNGq5HN5YuibziIoi/PFnSviWryRuAHYTwpcDazBYrNYB0PW3bRNk3HZwbw8O9LylHpkn+IA===
  {
    // Arrange
    Type actorType = typeof(StandardActor);
    Type baseType = typeof(StandardActorActivity);
    IMethodDescriptor descriptor = StandardActorFixture.Descriptor.Activities[1];
    ParameterInfo[] parameters = descriptor.ImplementationMethod.GetParameters();

    string argPropertyName = parameters[0].Name!;
    string argFieldName = $"<{argPropertyName}>k__BackingField";
    Type argType = parameters[0].ParameterType;

    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator idGetterIL = ilProvider.AddGenerator();
    FakeILGenerator argGetterIL = ilProvider.AddGenerator();
    FakeILGenerator argSetterIL = ilProvider.AddGenerator();
    FakeILGenerator executeAsyncMethodIL = ilProvider.AddGenerator();
    ActivityTypeBuilder sut = new(_module, ilProvider);

    // Act
    Type activityType = sut.Build(descriptor, actorType, baseType);

    // Assert
    activityType.Namespace.Should().Be(actorType.Namespace);
    activityType.Name.Should().Be($"<{actorType.Name}>{ActivityTypeBuilder.ComponentName}2");
    activityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Should().HaveCount(1)
      .And.ContainSingle(property =>
        property.Name == argPropertyName &&
        property.PropertyType == argType &&
        property.CanRead &&
        property.CanWrite);

    idGetterIL.VerifyIL(_ => _
      .Ldc_I4_2()
      .Ret());

    argGetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld(argFieldName)
      .Ret());

    argSetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld(argFieldName)
      .Ret());

    executeAsyncMethodIL.VerifyNoLocals();

    executeAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Ldarg_0()
      .Ldfld(argFieldName)
      .Ldarg_2()
      .Callvirt(descriptor.ImplementationMethod)
      .Ret());
  }

  [Fact]
  public void Build_ShouldBuildActivityType_WhenActivityIsValueTaskMethod() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKzAzNnJgGVg5GpXLFKAQQDGwAPbFUAb1SZsvAGrkIAV1IwAbJi279BgLKlghOZQAUAYRHTSECOWBg5NfHIDWpDSY0s6u7p7evgE0AJSqapgAvAB8mNbEcgDumDSk2QBycsAAkgC2AA4QpKWBwKSUAKIAHi7lETR2MZwAvqg8mOQARgDOwMTksnyYMp4AbmDAAJ7K8TC8Q6Pjk7TAmMWUmEqYdFYMmL1cKGprmDt7FVU1NMLt+4fHp5jDnxcX/RtjCa7eDTWRgeZLAA8+BmClSIFBcwWyyumEyhFIZEwMNkCkwCPgK1RNwBW2BRmapGkOjqkmGixo0jsOPkxAGuOIABpME5GWEPF4fP5AsFQm4BZFhbEen00OsRoDJiChCIxBJYcQZuDkfjEdqoSrROIpBzkkS/nKpiY9IZLNZbFqIYtdYa1SbWY7kUTrrw5LNMcQwJRSLdnnsDilMNxOKtff7iIHg9gKU0qTTSHSGUzXcaNezWdzeS5xe0oiKQnyS4Ky7F4ipUdcAOz5hS4a1mO02ewV4vhatSmK4OmGTqcNS/IA==
  {
    // Arrange
    Type actorType = typeof(StandardActor);
    Type baseType = typeof(StandardActorActivity);
    IMethodDescriptor descriptor = StandardActorFixture.Descriptor.Activities[2];
    ParameterInfo[] parameters = descriptor.ImplementationMethod.GetParameters();

    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator idGetterIL = ilProvider.AddGenerator();
    FakeILGenerator executeAsyncMethodIL = ilProvider.AddGenerator();
    ActivityTypeBuilder sut = new(_module, ilProvider);

    // Act
    Type activityType = sut.Build(descriptor, actorType, baseType);

    // Assert
    activityType.Namespace.Should().Be(actorType.Namespace);
    activityType.Name.Should().Be($"<{actorType.Name}>{ActivityTypeBuilder.ComponentName}3");
    activityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Should().BeEmpty();

    idGetterIL.VerifyIL(_ => _
      .Ldc_I4_3()
      .Ret());

    executeAsyncMethodIL.VerifyLocals(_ => _
      .OfType(descriptor.ReturnType));

    executeAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Ldarg_2()
      .Callvirt(descriptor.ImplementationMethod)
      .Stloc_0()
      .Ldloca_S(0)
      .Call(descriptor.ReturnType, nameof(ValueTask.AsTask), Type.EmptyTypes)
      .Ret());
  }

  [Fact]
  public void Build_ShouldBuildActivityType_WhenActivityIsValueTaskTMethod() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKzAzNnJgGVg5GpXLFKAQQDGwAPbFUAb1SZsvAGrkIAV1IwAbAB4c6AHyYtu/QfwBZUsEJzKACgCUqtZgC8Fp8RyAO6YNKQhAHJywACSALYADhCkcaQ0wKSUAKIAHtKkCcBgcjQenAC+qDyY5ABGAM7AxOSyfJgyRQBuYMAAnspeMLx1jc2ttMCYMZSYSph0jgyYlVwoakOYE1OJyanp5EUl07Pzi5j1Zysr1SNNLZPw7bJg3X1G+B0KFiBPXT39a0wQUIpDImA+sgUmB+8AGgI2tzGDwMmFypGkOgyknqvRo0lcEPkxBqkOIABpMABhET5CAQA7FGj4OQAazSmGkNNIdIZJWZbJo7gqVTQwwad1ajyEIjEEk+xA6L3+0N+Sre0tE4ikpLMcOuoraVj0hnsjmc2r+fRVGtlFoUiteAJU8N4ck6oOIYEopE26SmMz8mAALJxBq73cRPd7sCi0RisTi8a4bVr5SSiRTqXjufTDkzWezOdmeXn+WlPIDnd5sAB2dMKXBGmympwuDy4bGGMpeK5AA==
  {
    // Arrange
    Type actorType = typeof(StandardActor);
    Type baseType = typeof(StandardActorActivity);
    IMethodDescriptor descriptor = StandardActorFixture.Descriptor.Activities[3];

    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator idGetterIL = ilProvider.AddGenerator();
    FakeILGenerator executeAsyncMethodIL = ilProvider.AddGenerator();
    ActivityTypeBuilder sut = new(_module, ilProvider);

    // Act
    Type activityType = sut.Build(descriptor, actorType, baseType);

    // Assert
    activityType.Namespace.Should().Be(actorType.Namespace);
    activityType.Name.Should().Be($"<{actorType.Name}>{ActivityTypeBuilder.ComponentName}4");
    activityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Should().BeEmpty();

    idGetterIL.VerifyIL(_ => _
      .Ldc_I4_4()
      .Ret());

    executeAsyncMethodIL.VerifyLocals(_ => _
      .OfType(descriptor.ReturnType));

    executeAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Callvirt(descriptor.ImplementationMethod)
      .Stloc_0()
      .Ldloca_S(0)
      .Call(descriptor.ReturnType, nameof(ValueTask.AsTask), Type.EmptyTypes)
      .Ret());
  }

  [Fact]
  public void Build_ShouldBuildActivityType_WhenActivityIsVoidMethod() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKzAzNnJgGVg5GpXLFKAQQDGwAPbFUAb1SZsvGABZMANTlgpssADcwwAJ4AKAJSq1mALwA+TMBJyA7phqkvAOTlgAEkAWwAHCFIQ0hpgUkoAUQAPaVIw4DA5GhtOAF9UHkxyACMAZ2Biclk+TBkM0wtlOxheEvLK6tpgTCDKTCVMOlJgBkx8rhQ1FswunvDI6NjyDKze/sHh0dLNsdRxwraKqu74WqMG8wAefDqFFxAz+rNzOw9CUjJMG9kFTAf4JqTdRFMpHaowABsmGSpGkAFc4pJSuYaNJLN95MQij9iAAaTAAYREqQgEGWmRo+DkAGsYphpMTSKTyVkqbSaNY8gU0K1QR0TvwhCIxBJbsQ6iZnn9HpKLJchaJxIZMU5AfseTU9AYJRcdVKHgqRcqFHrGigVEDpnJjB9iAZSDNYj0+s5MBxuVNeNbbfbsFCYfDEcjUZZDUqxdjMfiiaimWSVpSaXSGbHmQm2TFbECLfZIwpcFrlbKrJy7FMAOx+3AEuTzYbxSGcNTjXJAA==
  {
    // Arrange
    Type actorType = typeof(StandardActor);
    Type baseType = typeof(StandardActorActivity);
    IMethodDescriptor descriptor = StandardActorFixture.Descriptor.Activities[4];

    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator idGetterIL = ilProvider.AddGenerator();
    FakeILGenerator executeAsyncMethodIL = ilProvider.AddGenerator();
    ActivityTypeBuilder sut = new(_module, ilProvider);

    // Act
    Type activityType = sut.Build(descriptor, actorType, baseType);

    // Assert
    activityType.Namespace.Should().Be(actorType.Namespace);
    activityType.Name.Should().Be($"<{actorType.Name}>{ActivityTypeBuilder.ComponentName}5");
    activityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Should().BeEmpty();

    idGetterIL.VerifyIL(_ => _
      .Ldc_I4_5()
      .Ret());

    executeAsyncMethodIL.VerifyNoLocals();

    executeAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Callvirt(descriptor.ImplementationMethod)
      .Call(typeof(Task), $"get_{nameof(ValueTask.CompletedTask)}", Type.EmptyTypes)
      .Ret());
  }

  [Fact]
  public void Build_ShouldBuildActivityType_WhenActivityBelongsToSpecificImplementation() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKzAzJuQEYBnYMXIBjYNjiYACgHsIATwC2c4gAdCYMQEEJa1AG9UAX1Q8pshSrWbte4GoCSy9RFLLSNYOWBg5NAiYIFZKqhpauvrERqiY2LwwAGyYDmAAbmDAigAUAJRx8ZgAvAB8mMAkcgDumDSktQBycsAubh5ewKSUAKIAHmKk6n4B+ZxmXGi8AsKiEpZpmdmxKPEw00Ii4pK0kk6UmIaYdKTADJgTheuYu5ht7p7evv40+4fHp+eCnxem5lN8TZzSTwVISDJZRQAHnwDjU5RCi0hhWqhFIZEwsOiwSkKzWG1m22wKX6pDEAFcujpBIoaGIcljHMQ+NEADSYADC5DppAgEGeAXwcgA1l5MGJuYM+QKaELRTQ8uN/tcZlt5qD5GFbJE4cQkdkcfroZqbBF7NFSitLgDQUajTiTeE7FEmUa8QlMHJ0ujiGBKKQbt47gcypgOP98Z7vcRff7iZhSRSqTS6TlHdrzUyWUz2VyedKRrKRWKJfn+YW5V4CqtDoU1gB2TA5NPWJ066L3DpPQsIPLbNR5XBGsaFCYmIA
  {
    // Arrange
    Type actorType = typeof(PolymorphicActor);
    Type baseType = typeof(PolymorphicActorActivity);
    IMethodDescriptor descriptor = PolymorphicActorFixture.Descriptor.Activities[3];

    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator idGetterIL = ilProvider.AddGenerator();
    FakeILGenerator executeAsyncMethodIL = ilProvider.AddGenerator();
    ActivityTypeBuilder sut = new(_module, ilProvider);

    // Act
    Type activityType = sut.Build(descriptor, actorType, baseType);

    // Assert
    activityType.Namespace.Should().Be(actorType.Namespace);
    activityType.Name.Should().Be($"<{actorType.Name}>{ActivityTypeBuilder.ComponentName}4");
    activityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Should().BeEmpty();

    idGetterIL.VerifyIL(_ => _
      .Ldc_I4_4()
      .Ret());

    executeAsyncMethodIL.VerifyNoLocals();

    executeAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .CastClass(typeof(PolymorphicActorImplementation1))
      .Callvirt(descriptor.ImplementationMethod)
      .Ret());
  }
}
