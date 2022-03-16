using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using System.Collections.Generic;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

internal interface ITopicDelegateFactory
{
  TopicDelegate CreateDelegate(IEnumerable<MessageHandlerIdentity> identities);
}