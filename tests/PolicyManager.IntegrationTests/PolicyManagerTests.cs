using CodeArchitects.Platform.PolicyManager.DependencyInjection;
using CodeArchitects.Platform.PolicyManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace CodeArchitects.Platform.PolicyManager.IntegrationTests;

public class PolicyManagerTests
{
	private readonly ClaimsIdentity _identity;
	private readonly ClaimsPrincipal _principal;
	private readonly string _resource = "component://invoices/button/confirm";
	private readonly string _policyCollectionName = "TestPolicy";
	private IServiceProvider? _services;

	public PolicyManagerTests()
	{
		_identity = new();
		_principal = new();
	}

	public IPolicyManager CreatePolicyManager(string json)
	{
		_services = new ServiceCollection()
			.AddPolicyManager(options => options
				.AddPolicyConfiguration(PolicyManager.CreatePolicyCollectionFromJson("TestPolicy", json)))
			.BuildServiceProvider();

		return _services.GetRequiredService<IPolicyManager>();
	}

	public IPolicyManager CreatePolicyManager(string json, IEnumerable<PolicyCollection> policyCollection)
	{
		_services = new ServiceCollection()
			.AddPolicyManager(options => options
				.AddPolicyConfiguration(PolicyManager.CreatePolicyCollectionFromJson("TestPolicy", json), policyCollection))
			.BuildServiceProvider();

		return _services.GetRequiredService<IPolicyManager>();
	}

	[Fact]
	public void CheckAccess_ShouldVerifyPolicy_WithPolicyClaims()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show",	
					"claim":
					{
						"claimType": "type1",
						"claimValue": "value1"
					}
				},
				{
					"type": "authorization",
					"resource": "component://invoices/button/write",
					"selector": "type",	
					"claim":
					{
						"claimType": "type2",
						"claimValue": "value2"
					}
				},
				{
					"type": "authorization",
					"resource": "component://invoices/button/read",
					"selector": "view",	
					"claim":
					{
						"claimType": "type3",
						"claimValue": "value3"
					}
				}
			]
			""";
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		result.Should().BeTrue();
	}

	[Fact]
	public void CheckAccess_ShouldNotVerifyPolicy_WithPolicyClaims()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show",	
					"claim":
					{
						"claimType": "type1",
						"claimValue": "value1"
					}
				},
				{
					"type": "authorization",
					"resource": "component://invoices/button/write",
					"selector": "type",	
					"claim":
					{
						"claimType": "type2",
						"claimValue": "value2"
					}
				},
				{
					"type": "authorization",
					"resource": "component://invoices/button/read",
					"selector": "view",	
					"claim":
					{
						"claimType": "type3",
						"claimValue": "value3"
					}
				}
			]
			""";
		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		result.Should().BeFalse();
	}

	[Fact]
	public void CheckAccess_ShouldVerifyPolicy_WithPolicyRuleAndNodes()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show",
					"and": 
					[
						{
							"claim":
							{
								"claimType": "type1",
								"claimValue": "value1"
							}
						},
						{
							"claim":
							{
								"claimType": "type2",
								"claimValue": "value2"
							}
						},
						{
							"and":
							[
								{
									"claim":
									{
										"claimType": "type3",
										"claimValue": "value3"
									}
								},
								{
									"claim":
									{
										"claimType": "type4",
										"claimValue": "value4"
									}
								}
							]
						}
					]
				}
			]
			""";
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type2", "value2", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type3", "value3", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type4", "value4", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		result.Should().BeTrue();
	}

	[Fact]
	public void CheckAccess_ShouldNotVerifyPolicy_WithPolicyRuleAndNodes()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show",
					"and": 
					[
						{
							"claim":
							{
								"claimType": "type1",
								"claimValue": "value1"
							}
						},
						{
							"claim":
							{
								"claimType": "type2",
								"claimValue": "value2"
							}
						},
						{
							"and":
							[
								{
									"claim":
									{
										"claimType": "type3",
										"claimValue": "value3"
									}
								},
								{
									"claim":
									{
										"claimType": "type4",
										"claimValue": "value4"
									}
								}
							]
						}
					]
				}
			]
			""";
		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		result.Should().BeFalse();
	}

	[Fact]
	public void CheckAccess_ShouldVerifyPolicy_WithPolicyRuleOrNodes()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show",
					"or": 
					[
						{
							"claim":
							{
								"claimType": "type1",
								"claimValue": "value1"
							}
						},
						{
							"claim":
							{
								"claimType": "type2",
								"claimValue": "value2"
							}
						},
						{
							"or":
							[
								{
									"claim":
									{
										"claimType": "type3",
										"claimValue": "value3"
									}
								},
								{
									"claim":
									{
										"claimType": "type4",
										"claimValue": "value4"
									}
								}
							]
						}
					]
				}
			]
			""";
		_identity.AddClaim(new Claim("type3", "value3", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		result.Should().BeTrue();
	}

	[Fact]
	public void CheckAccess_ShouldNotVerifyPolicy_WithPolicyRuleOrNodes()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show",
					"or": 
					[
						{
							"claim":
							{
								"claimType": "type1",
								"claimValue": "value1"
							}
						},
						{
							"claim":
							{
								"claimType": "type2",
								"claimValue": "value2"
							}
						},
						{
							"or":
							[
								{
									"claim":
									{
										"claimType": "type3",
										"claimValue": "value3"
									}
								},
								{
									"claim":
									{
										"claimType": "type4",
										"claimValue": "value4"
									}
								}
							]
						}
					]
				}
			]
			""";
		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		result.Should().BeFalse();
	}

	[Fact]
	public void CheckAccess_ShouldVerifyPolicy_WithNestedPolicyRules()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show",
					"and": 
					[
						{
							"claim":
							{
								"claimType": "type1",
								"claimValue": "value1"
							}
						},
						{
							"claim":
							{
								"claimType": "type2",
								"claimValue": "value2"
							}
						},
						{
							"and":
							[
								{
									"claim":
									{
										"claimType": "type3",
										"claimValue": "value3"
									}
								},
								{
									"claim":
									{
										"claimType": "type4",
										"claimValue": "value4"
									}
								}
							]
						},
						{
							"or":
							[
								{
									"claim":
									{
										"claimType": "type5",
										"claimValue": "value5"
									}
								},
								{
									"and":
									[
										{
											"claim":
											{
												"claimType": "type6",
												"claimValue": "value6"
											}
										},
										{
											"claim":
											{
												"claimType": "type7",
												"claimValue": "value7"
											}
										}
									]
								},
								{
									"claim":
									{
										"claimType": "type8",
										"claimValue": "value8"
									}
								}
							]
						}
					]
				}
			]
			""";
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type3", "value3", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type2", "value2", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type4", "value4", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type6", "value6", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type7", "value7", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		result.Should().BeTrue();
	}

	[Fact]
	public void CheckAccess_ShouldNotVerifyPolicy_WithNestedPolicyRules()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show",
					"and": 
					[
						{
							"claim":
							{
								"claimType": "type1",
								"claimValue": "value1"
							}
						},
						{
							"claim":
							{
								"claimType": "type2",
								"claimValue": "value2"
							}
						},
						{
							"and":
							[
								{
									"claim":
									{
										"claimType": "type3",
										"claimValue": "value3"
									}
								},
								{
									"claim":
									{
										"claimType": "type4",
										"claimValue": "value4"
									}
								}
							]
						},
						{
							"or":
							[
								{
									"claim":
									{
										"claimType": "type5",
										"claimValue": "value5"
									}
								},
								{
									"and":
									[
										{
											"claim":
											{
												"claimType": "type6",
												"claimValue": "value6"
											}
										},
										{
											"claim":
											{
												"claimType": "type7",
												"claimValue": "value7"
											}
										}
									]
								},
								{
									"claim":
									{
										"claimType": "type8",
										"claimValue": "value8"
									}
								}
							]
						}
					]
				}
			]
			""";
		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		result.Should().BeFalse();
	}

	[Fact]
	public void CheckAccess_ShouldVerifyPolicy_WithEmptyPolicyCondition()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show"
				}
			]
			""";
		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		result.Should().BeTrue();
	}

	[Fact]
	public void CheckAccess_ShouldVerifyPolicy_WithAdditionalConfigurationLoaded()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show",	
					"claim":
					{
						"claimType": "type1",
						"claimValue": "value1"
					}
				}
			]
			""";

		string configurationName = "AdditionalTestPolicy";
		string resource = "otherPolicy";
		PolicyCollection[] configuration = [
			new PolicyCollection() {
				PolicyName = configurationName,
				Policies = [
					new BasePolicy() {
						Type = "authorization",
						Resource = resource,
						Selector = "view",
						And = [
							new PolicyCondition() {
								Claim = new PolicyClaim() {
									ClaimType = "type4",
									ClaimValue = "value4"
								}
							},
							new PolicyCondition() {
								Claim = new PolicyClaim() {
									ClaimType = "type5",
									ClaimValue = "value5"
								}
							}
						]
					}
				]
			}
		];

		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type4", "value4", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type5", "value5", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json, configuration);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);
		bool additionalResult = sut.CheckAccess(configurationName, resource, _principal);

		// Assert
		result.Should().BeTrue();
		additionalResult.Should().BeTrue();
	}

	[Fact]
	public void CheckAccess_ShouldNotVerifyPolicy_WithAdditionalConfigurationLoaded()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show",	
					"claim":
					{
						"claimType": "type1",
						"claimValue": "value1"
					}
				}
			]
			""";

		string configurationName = "AdditionalTestPolicy";
		string resource = "otherPolicy";
		PolicyCollection[] configuration = [
			new PolicyCollection() {
				PolicyName = configurationName,
				Policies = [
					new BasePolicy() {
						Type = "authorization",
						Resource = resource,
						Selector = "view",
						And = [
							new PolicyCondition() {
								Claim = new PolicyClaim() {
									ClaimType = "type4",
									ClaimValue = "value4"
								}
							},
							new PolicyCondition() {
								Claim = new PolicyClaim() {
									ClaimType = "type5",
									ClaimValue = "value5"
								}
							}
						]
					}
				]
			}
		];

		_identity.AddClaim(new Claim("type4", "value4", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json, configuration);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);
		bool additionalResult = sut.CheckAccess(configurationName, resource, _principal);

		// Assert
		result.Should().BeFalse();
		additionalResult.Should().BeFalse();
	}

	[Fact]
	public void CheckAccess_ShouldVerifyPolicy_WithEmptyPolicyCondition_WithAdditionalConfigurationLoaded()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/confirm",
					"selector": "show"
				}
			]
			""";

		string configurationName = "AdditionalTestPolicy";
		string resource = "otherPolicy";
		PolicyCollection[] configuration = [
			new PolicyCollection() {
				PolicyName = configurationName,
				Policies = [
					new BasePolicy() {
						Type = "authorization",
						Resource = resource,
						Selector = "view"
					}
				]
			}
		];

		_principal.AddIdentity(_identity);

		// Act
		IPolicyManager sut = CreatePolicyManager(json, configuration);
		bool result = sut.CheckAccess(_policyCollectionName, _resource, _principal);
		bool additionalResult = sut.CheckAccess(configurationName, resource, _principal);

		// Assert
		result.Should().BeTrue();
		additionalResult.Should().BeTrue();
	}
}
