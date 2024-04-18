namespace CodeArchitects.Platform.PolicyManager.Models;

public class PolicyCollection
{
	public required string PolicyName { get; set; }
	public required IEnumerable<BasePolicy> Policies { get; set; }
}
