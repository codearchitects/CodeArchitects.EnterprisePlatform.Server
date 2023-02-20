using System.Collections.Concurrent;

namespace ActorApp.Domain;

public class ActorOutput
{
  private readonly ConcurrentDictionary<Guid, string> _outputs;

	public ActorOutput()
	{
		_outputs = new();
	}

	public string? GetOutput(Guid id)
	{
		return _outputs.GetValueOrDefault(id);
	}

	public void SetOutput(Guid id, string value)
	{
		_outputs[id] = value;
	}
}
