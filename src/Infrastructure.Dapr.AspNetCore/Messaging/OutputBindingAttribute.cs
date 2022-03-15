using System;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

[AttributeUsage(AttributeTargets.ReturnValue)]
public abstract class OutputBindingAttribute : Attribute
{

}
