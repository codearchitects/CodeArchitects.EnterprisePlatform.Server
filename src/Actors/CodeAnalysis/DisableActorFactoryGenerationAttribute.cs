namespace CodeArchitects.Platform.Actors.CodeAnalysis;

/// <summary>
/// Disables the generation of actor factory interfaces.
/// </summary>
/// <remarks>
/// When disabling the generation of factories, the interfaces must be manually created and decorated with <see cref="ActorFactoryAttribute"/> or <see cref="ActorFactoryAttribute{TActor}"/>, or registered via the fluent configuration API.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly)]
public class DisableActorFactoryGenerationAttribute : Attribute
{
}
