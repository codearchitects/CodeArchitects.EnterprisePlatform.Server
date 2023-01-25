using CodeArchitects.Platform.Actors.Descriptors;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace FluentAssertions
{
  using CodeArchitects.Platform.Actors.Fixtures;

  internal static class FluentAssertionsExtensions
  {
    public static ActorDescriptorAssertions Should(this IActorDescriptor descriptor)
    {
      return new ActorDescriptorAssertions(descriptor);
    }
  }
}

namespace CodeArchitects.Platform.Actors.Fixtures
{
  internal class ActorDescriptorAssertions : ReferenceTypeAssertions<IActorDescriptor, ActorDescriptorAssertions>
  {
    public ActorDescriptorAssertions(IActorDescriptor subject)
      : base(subject)
    {
    }

    protected override string Identifier => nameof(IActorDescriptor);

    public AndConstraint<ActorDescriptorAssertions> BeEquivalentTo(IActorDescriptor other, string because = "", params object[] becauseArgs)
    {
      Execute.Assertion
        .BecauseOf(because, becauseArgs)
        .ForCondition(DescriptorEqualityComparer.Instance.Equals(Subject, other))
        .FailWith("Descriptors are not equivalent");

      return new AndConstraint<ActorDescriptorAssertions>(this);
    }
  }
}
