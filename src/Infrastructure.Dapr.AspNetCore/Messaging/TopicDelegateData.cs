using System;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

internal record TopicDelegateData(Type MessageType, Type? ResultType, Type HandlerType);
