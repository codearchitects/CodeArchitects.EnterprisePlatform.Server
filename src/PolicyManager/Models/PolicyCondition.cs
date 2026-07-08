namespace CodeArchitects.Platform.PolicyManager.Models;

public class PolicyCondition
{
	public PolicyClaim? Claim { get; set; }
	public IEnumerable<PolicyCondition>? And { get; set; }
	public IEnumerable<PolicyCondition>? Or { get; set; }

	public PolicyConditionType GetPolicyConditionType()
	{
		if (Claim is not null)
		{
			return PolicyConditionType.Claim;
		}

		if (And is not null)
		{
			return PolicyConditionType.And;
		}

		if (Or is not null)
		{
			return PolicyConditionType.Or;
		}

		return PolicyConditionType.Empty;
	}
}
