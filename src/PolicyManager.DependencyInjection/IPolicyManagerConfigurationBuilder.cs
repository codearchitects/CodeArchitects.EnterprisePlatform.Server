using CodeArchitects.Platform.PolicyManager.Models;
using System.Collections.Generic;

namespace CodeArchitects.Platform.PolicyManager.DependencyInjection
{
	/// <summary>
	/// An object used to configure the policy manager context and services.
	/// </summary>
	public interface IPolicyManagerConfigurationBuilder
	{
		/// <summary>
		/// Specify the policy configuration to initialize the policy manager.
		/// </summary>
		/// <param name="policyCollections">Object array containing the policies.</param>
		/// <returns>An <see cref="IPolicyManagerConfigurationBuilder"/>.</returns>
		IPolicyManagerConfigurationBuilder AddPolicyConfiguration(IEnumerable<PolicyCollection>? policyCollections);

		/// <summary>
		/// Specify the policy configurations to initialize the policy manager.
		/// </summary>
		/// <param name="servicePolicies">Object array containing the service policies.</param>
		/// <param name="adaptersPolicies">Object array containing the adapters policies.</param>
		/// <returns>An <see cref="IPolicyManagerConfigurationBuilder"/>.</returns>
		IPolicyManagerConfigurationBuilder AddPolicyConfiguration(IEnumerable<PolicyCollection>? servicePolicies, IEnumerable<PolicyCollection>? adaptersPolicies);
	}
}
