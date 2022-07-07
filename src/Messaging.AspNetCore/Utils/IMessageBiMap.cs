namespace CodeArchitects.Platform.Messaging.AspNetCore.Utils;

/// <summary>
/// Bidirectional map between message types and names.
/// </summary>
internal interface IMessageBiMap : IReadOnlyDictionary<string, Type>, IReadOnlyDictionary<Type, string>
{
}
