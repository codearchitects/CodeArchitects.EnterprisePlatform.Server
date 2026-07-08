namespace CodeArchitects.Platform.PolicyManager.Models;

public class BasePolicy : PolicyCondition
{
	public required string Type { get; set; }
	public required string Resource { get; set; }
	public required string Selector { get; set; }
}
