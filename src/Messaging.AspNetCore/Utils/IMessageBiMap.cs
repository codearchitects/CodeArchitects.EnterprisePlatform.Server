namespace CodeArchitects.Platform.Messaging.AspNetCore.Utils;

internal interface IMessageBiMap : IReadOnlyDictionary<string, Type>, IReadOnlyDictionary<Type, string>
{
}
