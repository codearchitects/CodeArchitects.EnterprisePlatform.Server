using System.Security.Claims;

namespace CodeArchitects.Platform.PolicyManager;

public class PolicyManagerTests
{
	private readonly PolicyManager _sut;
	private readonly ClaimsIdentity _identity;
	private readonly ClaimsPrincipal _principal;
	private readonly Dictionary<string, Dictionary<string, Func<ClaimsPrincipal, bool>>> _policyCollections;
	private readonly string _resource = "component://invoices/button/write";
	private readonly string _policyCollectionName = "TestPolicy";

	public PolicyManagerTests()
	{
		_policyCollections = [];
		_sut = new(_policyCollections);
		_identity = new();
		_principal = new();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnTrue_WithPolicyClaim_WhenClaimPrincipalIsAuthorized()
	{
		// Arrange
		string json = """
				[
					{
						"type": "authorization",
						"resource": "component://invoices/button/write",
						"selector": "show",
						"claim":
						{
							"claimType": "type1",
							"claimValue":"value1"
						}
					}
				]
			""";
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeTrue();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnFalse_WithPolicyClaim_WhenClaimPrincipalIsUnauthorized()
	{
		// Arrange
		string json = """
				[
					{
						"type": "authorization",
						"resource": "component://invoices/button/write",
						"selector": "show",
						"claim":
						{
							"claimType": "type1",
							"claimValue":"value1"
						}
					}
				]
			""";
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeFalse();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnTrue_WithPolicyRuleAndNode_WhenClaimPrincipalIsAuthorized()
	{
		// Arrange
		string json = """
				[
					{
						"type": "authorization",
						"resource": "component://invoices/button/write",
						"selector": "show",
						"and": [
							{
								"claim":
								{
									"claimType": "type1",
									"claimValue":"value1"
								}
							},
							{
								"claim":
									{
										"claimType": "type2",
										"claimValue":"value2"
									}
							},
							{
								"claim":
									{
										"claimType": "type3",
										"claimValue":"value3"
									}
							},
						]
					}
				]
			""";
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type2", "value2", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type3", "value3", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeTrue();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnFalse_WithPolicyRuleAndNode_WhenClaimPrincipalUnauthorized()
	{
		// Arrange
		string json = """
				[
					{
						"type": "authorization",
						"resource": "component://invoices/button/write",
						"selector": "show",
						"and": [
							{
								"claim":
								{
									"claimType": "type1",
									"claimValue":"value1"
								}
							},
							{
								"claim":
									{
										"claimType": "type2",
										"claimValue":"value2"
									}
							},
							{
								"claim":
									{
										"claimType": "type3",
										"claimValue":"value3"
									}
							},
						]
					}
				]
			""";
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type3", "value3", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeFalse();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnTrue_WithNestedPolicyRuleAndNode_WhenClaimPrincipalIsAuthorized()
	{
		// Arrange
		string json = """
				[
					{
						"type": "authorization",
						"resource": "component://invoices/button/write",
						"selector": "show",
						"and": [
							{
								"claim":
								{
									"claimType": "type1",
									"claimValue":"value1"
								}
							},
							{
								"claim":
									{
										"claimType": "type2",
										"claimValue":"value2"
									}
							},
							{
								"claim":
									{
										"claimType": "type3",
										"claimValue":"value3"
									}
							},
							{
								"and": [
									{
										"claim":
										{
											"claimType": "type4",
											"claimValue":"value4"
										}
									},
									{
										"claim":
											{
												"claimType": "type5",
												"claimValue":"value5"
											}
									},
									{
										"claim":
											{
												"claimType": "type6",
												"claimValue":"value6"
											}
									}
								]
							},
							{
								"claim":
									{
										"claimType": "type7",
										"claimValue":"value7"
									}
							}
						]
					}
				]
			""";
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type2", "value2", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type3", "value3", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type4", "value4", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type5", "value5", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type6", "value6", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type7", "value7", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeTrue();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnFalse_WithNestedPolicyRuleAndNode_WhenClaimPrincipalIsUnauthorized()
	{
		// Arrange
		string json = """
				[
					{
						"type": "authorization",
						"resource": "component://invoices/button/write",
						"selector": "show",
						"and": [
							{
								"claim":
								{
									"claimType": "type1",
									"claimValue":"value1"
								}
							},
							{
								"claim":
									{
										"claimType": "type2",
										"claimValue":"value2"
									}
							},
							{
								"claim":
									{
										"claimType": "type3",
										"claimValue":"value3"
									}
							},
							{
								"and": [
									{
										"claim":
										{
											"claimType": "type4",
											"claimValue":"value4"
										}
									},
									{
										"claim":
											{
												"claimType": "type5",
												"claimValue":"value5"
											}
									},
									{
										"claim":
											{
												"claimType": "type6",
												"claimValue":"value6"
											}
									}
								]
							},
							{
								"claim":
									{
										"claimType": "type7",
										"claimValue":"value7"
									}
							}
						]
					}
				]
			""";
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type2", "value2", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type3", "value3", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type4", "value4", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type6", "value6", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type7", "value7", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeFalse();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnTrue_WithPolicyRuleOrNode_WhenClaimPrincipalIsAuthorized()
	{
		// Arrange
		string json = """
				[
					{
						"type": "authorization",
						"resource": "component://invoices/button/write",
						"selector": "show",
						"or": [
							{
								"claim":
								{
									"claimType": "type1",
									"claimValue":"value1"
								}
							},
							{
								"claim":
									{
										"claimType": "type2",
										"claimValue":"value2"
									}
							},
							{
								"claim":
									{
										"claimType": "type3",
										"claimValue":"value3"
									}
							},
						]
					}
				]
			""";
		_identity.AddClaim(new Claim("type2", "value2", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeTrue();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnFalse_WithPolicyRuleOrNode_WhenClaimPrincipalIsUnauthorized()
	{
		string json = """
				[
					{
						"type": "authorization",
						"resource": "component://invoices/button/write",
						"selector": "show",
						"or": [
							{
								"claim":
								{
									"claimType": "type1",
									"claimValue":"value1"
								}
							},
							{
								"claim":
									{
										"claimType": "type2",
										"claimValue":"value2"
									}
							},
							{
								"claim":
									{
										"claimType": "type3",
										"claimValue":"value3"
									}
							},
						]
					}
				]
			""";
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeFalse();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnTrue_WithNestedPolicyRuleOrNode_WhenClaimPrincipalIsAuthorized()
	{
		// Arrange
		string json = """
				[
					{
						"type": "authorization",
						"resource": "component://invoices/button/write",
						"selector": "show",
						"or": [
							{
								"claim":
								{
									"claimType": "type1",
									"claimValue":"value1"
								}
							},
							{
								"claim":
									{
										"claimType": "type2",
										"claimValue":"value2"
									}
							},
							{
								"claim":
									{
										"claimType": "type3",
										"claimValue":"value3"
									}
							},
							{
								"or": [
									{
										"claim":
										{
											"claimType": "type4",
											"claimValue":"value4"
										}
									},
									{
										"claim":
											{
												"claimType": "type5",
												"claimValue":"value5"
											}
									},
									{
										"claim":
											{
												"claimType": "type6",
												"claimValue":"value6"
											}
									}
								]
							},
							{
								"claim":
									{
										"claimType": "type7",
										"claimValue":"value7"
									}
							}
						]
					}
				]
			""";
		_identity.AddClaim(new Claim("type5", "value5", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeTrue();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnFalse_WithNestedPolicyRuleOrNode_WhenClaimPrincipalIsUnauthorized()
	{
		// Arrange
		string json = """
				[
					{
						"type": "authorization",
						"resource": "component://invoices/button/write",
						"selector": "show",
						"or": [
							{
								"claim":
								{
									"claimType": "type1",
									"claimValue":"value1"
								}
							},
							{
								"claim":
									{
										"claimType": "type2",
										"claimValue":"value2"
									}
							},
							{
								"claim":
									{
										"claimType": "type3",
										"claimValue":"value3"
									}
							},
							{
								"or": [
									{
										"claim":
										{
											"claimType": "type4",
											"claimValue":"value4"
										}
									},
									{
										"claim":
											{
												"claimType": "type5",
												"claimValue":"value5"
											}
									},
									{
										"claim":
											{
												"claimType": "type6",
												"claimValue":"value6"
											}
									}
								]
							},
							{
								"claim":
									{
										"claimType": "type7",
										"claimValue":"value7"
									}
							}
						]
					}
				]
			""";
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeFalse();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnTrue_WithNestedPolicies_WhenClaimPrincipalIsAuthorized()
	{
		// Arrange
		string json = """
				[
					{
						"type": "authorization",
						"resource": "component://invoices/button/write",
						"selector": "show",
						"and": [
							{
								"claim":
								{
									"claimType": "type1",
									"claimValue":"value1"
								}
							},
							{
								"or": [
									{
										"claim":
										{
											"claimType": "type2",
											"claimValue":"value2"
										}
									},
									{
										"claim":
											{
												"claimType": "type3",
												"claimValue":"value3"
											}
									}
								]
							},
							{
								"claim":
									{
										"claimType": "type4",
										"claimValue":"value4"
									}
							},
							{
								"and": [
									{
										"claim":
										{
											"claimType": "type5",
											"claimValue":"value5"
										}
									},
									{
										"claim":
											{
												"claimType": "type6",
												"claimValue":"value6"
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
		_identity.AddClaim(new Claim("type4", "value4", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type5", "value5", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type6", "value6", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeTrue();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnFalse_WithNestedPolicies_WhenClaimPrincipalIsUnauthorized()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/write",
					"selector": "show",
					"and": [
						{
							"claim":
							{
								"claimType": "type1",
								"claimValue":"value1"
							}
						},
						{
							"or": [
								{
									"claim":
									{
										"claimType": "type2",
										"claimValue":"value2"
									}
								},
								{
									"claim":
										{
											"claimType": "type3",
											"claimValue":"value3"
										}
								}
							]
						},
						{
							"claim":
								{
									"claimType": "type4",
									"claimValue":"value4"
								}
						},
						{
							"and": [
								{
									"claim":
									{
										"claimType": "type5",
										"claimValue":"value5"
									}
								},
								{
									"claim":
										{
											"claimType": "type6",
											"claimValue":"value6"
										}
								}
							]
						}
					]
				}
			]
		""";
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type4", "value4", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type5", "value5", "http://www.w3.org/2001/XMLSchema#string"));
		_identity.AddClaim(new Claim("type6", "value6", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeFalse();
	}

	[Fact]
	internal void ReadPolicy_ShouldReturnTrue_WithEmptyPolicyCondition()
	{
		// Arrange
		string json = """
			[
				{
					"type": "authorization",
					"resource": "component://invoices/button/write",
					"selector": "show",
				}
			]
		""";
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool collectionResult = _policyCollections.TryGetValue(_policyCollectionName, out Dictionary<string, Func<ClaimsPrincipal, bool>>? policies);
		bool result = policies!.TryGetValue(_resource, out Func<ClaimsPrincipal, bool>? policyDelegate);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		policies.Should().NotBeEmpty();
		collectionResult.Should().BeTrue();
		result.Should().BeTrue();
		policyDelegate.Should().NotBeNull();
		policyDelegate!(_principal).Should().BeTrue();
	}

	[Fact]
	internal void CheckAccess_ShouldReturnTrue_WhenPolicyIsValidated()
	{
		// Arrange
		Dictionary<string, Func<ClaimsPrincipal, bool>> policies = [];
		policies.Add(
			_resource,
			principal => true
		);
		_policyCollections.Add(
			_policyCollectionName,
			policies
		);
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		bool result = _sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		result.Should().BeTrue();
	}

	[Fact]
	internal void CheckAccess_ShouldReturnTrue_WhenPolicyRegexIsValidated()
	{
		// Arrange
		string resource = "/api/service/controller/method";
		string json = """
				[
					{
						"type": "authorization",
						"resource": "/api/.+/controller/.+",
						"selector": "show",
						"claim":
						{
							"claimType": "type1",
							"claimValue":"value1"
						}
					}
				]
			""";
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool result = _sut.CheckAccess(_policyCollectionName, resource, _principal);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		result.Should().BeTrue();
	}

	[Fact]
	internal void CheckAccess_ShouldReturnFalse_WhenPolicyIsNotValidated()
	{
		// Arrange
		Dictionary<string, Func<ClaimsPrincipal, bool>> policies = [];
		policies.Add(
			_resource,
			principal => false
		);
		_policyCollections.Add(
			_policyCollectionName,
			policies
		);
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		bool result = _sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		_policyCollectionName.Should().NotBeEmpty();
		result.Should().BeFalse();
	}

	[Fact]
	internal void CheckAccess_ShouldReturnFalse_WhenPolicyRegexIsNotValidated()
	{
		// Arrange
		string resource = "/api/service/controller2/method";
		string json = """
				[
					{
						"type": "authorization",
						"resource": "/api/.+/controller/.+",
						"selector": "show",
						"claim":
						{
							"claimType": "type1",
							"claimValue":"value1"
						}
					}
				]
			""";
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		_sut.LoadConfiguration(PolicyManager.CreatePolicyCollectionFromJson(_policyCollectionName, json));
		bool result = _sut.CheckAccess(_policyCollectionName, resource, _principal);

		// Assert
		_policyCollections.Should().NotBeEmpty();
		result.Should().BeFalse();
	}

	[Fact]
	internal void CheckAccess_ShouldReturnFalse_WhenPolicyDoesNotExist()
	{
		// Arrange
		_identity.AddClaim(new Claim("type1", "value1", "http://www.w3.org/2001/XMLSchema#string"));
		_principal.AddIdentity(_identity);

		// Act
		bool result = _sut.CheckAccess(_policyCollectionName, _resource, _principal);

		// Assert
		_policyCollections.Should().BeEmpty();
		result.Should().BeFalse();
	}
}
