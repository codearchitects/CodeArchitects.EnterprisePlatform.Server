using CodeArchitects.Platform.PolicyManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace CodeArchitects.Platform.PolicyManager.DependencyInjection
{
	internal class PolicyManagerConfigurationBuilder : IPolicyManagerConfigurationBuilder
	{
		private List<PolicyCollection>? _policyCollections;

		public PolicyManagerConfigurationBuilder()
		{
			_policyCollections = new List<PolicyCollection>();
		}

		public void AddServices(IServiceCollection services)
		{
			PolicyManager policyManager = PolicyManager.Create();
			policyManager.LoadConfiguration(_policyCollections!);
			services.AddSingleton<IPolicyManager>(policyManager);
		}

		public IPolicyManagerConfigurationBuilder AddPolicyConfiguration(IEnumerable<PolicyCollection>? policyCollections)
		{
			LoadPolicies(policyCollections);

			return this;
		}

		public IPolicyManagerConfigurationBuilder AddPolicyConfiguration(IEnumerable<PolicyCollection>? servicePolicies, IEnumerable<PolicyCollection>? adaptersPolicies)
		{
			LoadPolicies(servicePolicies);
			LoadPolicies(adaptersPolicies);

			return this;
		}

		private void LoadPolicies(IEnumerable<PolicyCollection>? policies)
		{
			if (policies != null)
				_policyCollections!.AddRange(policies);
		}
	}
}
