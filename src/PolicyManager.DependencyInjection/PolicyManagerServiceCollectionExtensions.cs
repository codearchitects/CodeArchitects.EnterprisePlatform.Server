using Microsoft.Extensions.DependencyInjection;
using System;

namespace CodeArchitects.Platform.PolicyManager.DependencyInjection
{
	/// <summary>
	/// Extension methods for registering the policy manager in the dependency injection container.
	/// </summary>
	public static class PolicyManagerServiceCollectionExtensions
	{
		/// <summary>
		/// Registers the policy manager in the dependency injection container.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> instance to which the policy manager services will be added.</param>
		/// <param name="configure">A delegate used to configure the policy manager settings.</param>
		/// <returns>The <see cref="IServiceCollection"/> instance after the policy manager services have been added.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> parameter is null.</exception>
		public static IServiceCollection AddPolicyManager(this IServiceCollection services, Func<IPolicyManagerConfigurationBuilder, IPolicyManagerConfigurationBuilder> configure)
		{
			if (services is null)
				throw new ArgumentNullException(nameof(services));

			PolicyManagerConfigurationBuilder builder = new PolicyManagerConfigurationBuilder();
			configure(builder);

			builder.AddServices(services);

			return services;
		}
	}
}
