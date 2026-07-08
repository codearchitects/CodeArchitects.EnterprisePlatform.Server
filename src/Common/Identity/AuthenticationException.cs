using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Common.Identity;

/// <summary>
/// Exception thrown when an unauthenticated user attempts to perform actions that requires authentication.
/// </summary>
[Serializable]
public sealed class AuthenticationException : Exception
{
	public AuthenticationException()
		: base("The user is not authenticated")
	{
	}

	private AuthenticationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{ 
	}
}
