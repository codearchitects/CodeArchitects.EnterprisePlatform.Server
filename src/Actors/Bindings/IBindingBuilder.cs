namespace CodeArchitects.Platform.Actors.Bindings;

public interface IBindingBuilder<TActor>
  where TActor : class
{
  IBindingBuilder<TActor> WithPreCondition(Func<TActor, bool> preCondition);
  IBindingBuilder<TActor> WithPostCondition(Func<TActor, bool> postCondition);
  IBindingBuilder<TActor> IsEnabled(bool enabled = true);
  IBindingResult BindTo(Func<TActor, CancellationToken, Task> activity);
}
