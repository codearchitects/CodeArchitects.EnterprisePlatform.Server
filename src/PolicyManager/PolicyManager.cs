using CodeArchitects.Platform.PolicyManager.Models;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.RegularExpressions;
using CodeArchitects.Platform.PolicyManager.PredicateBuilder;

namespace CodeArchitects.Platform.PolicyManager;

internal class PolicyManager : IPolicyManager
{
	private Dictionary<string, Dictionary<string, Func<ClaimsPrincipal, bool>>> _policies;
	private static readonly Expression<Func<ClaimsPrincipal, bool>> s_allowAccessForDefaultPagePolicy = principal => true;

	public static PolicyManager Create()
	{
		return new PolicyManager([]);
	}

	public static IEnumerable<PolicyCollection> CreatePolicyCollectionFromJson(string policyName, string json)
	{
		return [
			new PolicyCollection()
			{
				PolicyName = policyName,
				Policies = JsonConvert.DeserializeObject<IEnumerable<BasePolicy>>(json)!
			}
		];
	}

	public PolicyManager(Dictionary<string, Dictionary<string, Func<ClaimsPrincipal, bool>>> policies)
	{
		_policies = policies;
	}

	public bool CheckAccess(string policy, string resource, ClaimsPrincipal principal)
	{
		bool isPolicyFound = _policies.TryGetValue(policy, out Dictionary<string, Func<ClaimsPrincipal, bool>> policyCollection);
		if (isPolicyFound)
		{
			bool isResourceFound = policyCollection.TryGetValue(resource, out Func<ClaimsPrincipal, bool> policyDelegate);
			if (isResourceFound)
			{
				return policyDelegate(principal);
			}

			foreach (var kvp in policyCollection)
			{
				if (Regex.IsMatch(resource, kvp.Key))
				{
					return kvp.Value(principal);
				}
			}

			return isResourceFound;
		}

		return isPolicyFound;
	}

	public void LoadConfiguration(IEnumerable<PolicyCollection> policyCollections)
	{
		foreach (PolicyCollection policyCollection in policyCollections)
		{
			Dictionary<string, Func<ClaimsPrincipal, bool>> policies = [];
			foreach (var policy in policyCollection.Policies)
			{
				policies.Add(
					policy.Resource,
					ReadPolicy(policy)
				);
			}
			_policies.Add(
				policyCollection.PolicyName,
				policies
			);
		}
	}

	private Func<ClaimsPrincipal, bool> ReadPolicy(BasePolicy policy)
	{
		Expression<Func<ClaimsPrincipal, bool>> conditionExpr = ReadPolicy((PolicyCondition)policy);

		return conditionExpr.Compile();
	}

	private Expression<Func<ClaimsPrincipal, bool>> ReadPolicy(PolicyCondition policy)
	{
		return policy.GetPolicyConditionType() switch
		{
			PolicyConditionType.Claim => GetExpressionFromClaim(policy.Claim!),
			PolicyConditionType.And => GetExpressionFromConditionList(policy.And!, 0, PolicyConditionType.And),
			PolicyConditionType.Or => GetExpressionFromConditionList(policy.Or!, 0, PolicyConditionType.Or),
			PolicyConditionType.Empty => s_allowAccessForDefaultPagePolicy,
			_ => throw new InvalidOperationException("Unexpected policy condition.")
		};
	}

	private Expression<Func<ClaimsPrincipal, bool>> GetExpressionFromClaim(PolicyClaim claim)
	{
		return principal => principal.Claims.Any(c =>
			c.Type == claim.ClaimType &&
			c.ValueType == ClaimValueTypes.String &&
			c.Value == claim.ClaimValue);
	}

	private Expression<Func<ClaimsPrincipal, bool>> GetExpressionFromConditionList(IEnumerable<PolicyCondition> conditions, int index, PolicyConditionType exprOp)
	{
		PolicyCondition left = conditions.ElementAtOrDefault(index);
		if (left == null)
		{
			return exprOp switch
			{
				PolicyConditionType.And => principal => true,
				PolicyConditionType.Or => principal => false,
				_ => throw new InvalidOperationException("Unexpected policy condition type.")
			};
		}

		Expression<Func<ClaimsPrincipal, bool>> leftExpr = ReadPolicy(left);

		Expression<Func<ClaimsPrincipal, bool>> rightExpr = GetExpressionFromConditionList(conditions, ++index, exprOp);

		return exprOp switch
		{
			PolicyConditionType.And => ExpressionBuilder.AndAlso(leftExpr, rightExpr),
			PolicyConditionType.Or => ExpressionBuilder.OrElse(leftExpr, rightExpr),
			_ => throw new InvalidOperationException("Unexpected policy condition type.")
		};
	}
}
