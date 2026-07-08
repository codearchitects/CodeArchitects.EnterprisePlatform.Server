# Messaging with Dapr

Pub/sub is one of the most important feature of a distributed application and Dapr offers an out-of-the-box implementation of this pattern.

We wrap its functionality in our messaging API and take care of most of the configuration stuff, letting the developer focus on the business logic.

You can read more about pub/sub with Dapr following the links below:

- <https://docs.dapr.io/developing-applications/building-blocks/pubsub/>
- <https://docs.microsoft.com/it-it/dotnet/architecture/dapr-for-net-developers/publish-subscribe>

## Configuration

> TODO: Link to Dapr configuration docs

The `Messaging` section of the configuration file will be used for configuring the messaging infrastructure.

Currently, this section only contains the `DefaultBus` property that can be used to configure the name of the default bus the application will use.

```json
{
    "Messaging": {
        "DefaultBus": "pubsub"
    }
}
```

If you specify a default bus, publishers and subscribers will use that name by default, if no bus name is specified.

## Dependency Injection

Normally, you will use Dependency Injection to access the message bus in your consumer classes. If a default bus is configured, you can request an `IMessageBus` (or `IMessageBus<DaprMetadata>`) instance into the class' constructor.

```c#
public class Publisher
{
    private readonly IMessageBus _bus;

    public Publisher(IMessageBus bus)
    {
        _bus = bus;
    }
}
```

However, Dapr lets you use multiple message brokers at the same time in your application, which are identified by a name. You can manually resolve a specific message bus by name, using the `IServiceResolver` generic interface.

```c#
public class Publisher
{
    private readonly IMessageBus _bus;

    public Publisher(IServiceResolver<IMessageBus> busResolver)
    {
        _bus = busResolver.Resolve("busName");
    }
}
```

You can resolve `IMessageBus<DaprMetadata>` as well.

## Topics

Dapr uses topics to route messages. A publisher will publish a message to a specific topic and subscribers that are subscribed to that topic will receive that message.

By default, the name of the topic is the name of the message class, but you can override this in both the publisher and the subscriber. For most use cases, you do not need to do this.

The default behavior ensures that both the publisher and the subscriber agree on the type of the message they exchange, but you can potentially send messages of different types onto the same topic. Currently, this is prevented and topics can only be used to restrict the scope of a message. This may be changed in the future for more flexibility.

To restrict a message handler to a given topic, use the `SubscribeToTopic` attribute.

```c#
// This handler will handle only MyMessage instances that are published on the topic 'myTopic'.
[SubscribeToTopic("myTopic")]
public class MyMessageHandler : IMessageHandler<MyMessage>
{
    public async Task HandleAsync(MyMessage message, CancellationToken cancellationToken)
    {
        // Message handling
    }
}
```

To publish a message on a particular topic, use the Topic property of the metadata, as described in the 'Metadata' section.

## Busses

Potentially, an application may have more than one message bus. If your application has such need, the message handler must be decorated with the `SubscribeToBus` attribute, which will specify the name of the bus that message handler will receive message from.

```c#
// This handler will handle only MyMessage instances that are published on the bus 'myBus'.
[SubscribeToBus("myBus")]
public class MyMessageHandler : IMessageHandler<MyMessage>
{
    public async Task HandleAsync(MyMessage message, CancellationToken cancellationToken)
    {
        // Message handling
    }
}
```

If the message handler is not decorated with the `SubscribeToBus`, the default bus name will be used, if configured. Otherwise, the handler will not be active.

## Metadata

Metadata are provided to Dapr in the form of key-value pairs of strings. However, we define the `DaprMetadata` class, which lets you define and access these values in a strongly-typed way.

The `DaprMetadata` class has 3 properties:

- TimeToLiveInSeconds, of type `int?`: the number of seconds for the message to expire as [described here](https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-message-ttl/).
- RawPayload, of type `bool?`: boolean to determine if Dapr should publish the event without wrapping it as CloudEvent as [described here](https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-raw/).
- Topic, of type `string?`: the name of the topic the message should be published to. Note that, from the Dapr perspective, this is not part of the metadata API.

```c#
public class Publisher
{
    private readonly IMessageBus<DaprMetadata> _bus;

    public async Task DoSomethingAndPublishAsync(string message)
    {
        // Some business logic
        MyMessage message = new MyMessage(Guid.NewGuid(), _sequenceNumber, message);
        DaprMetadata metadata = new DaprMetadata
        {
            RawPayload = true,
            TimeToLiveInSeconds = 30,
            Topic = "myTopic"
        };
        await _bus.SendAsync(message, metadata);
        // Other business logic
    }
}
```

### In Startup.cs

To start configuring the Dapr infrastructure, call the `AddDaprInfrastructure` extension method on your `IServiceCollection` instance. There are two overload of this method. The first one takes an optional `DaprConfiguration` argument, which contains the configuration instance to use; we recommend using that for prototyping and testing purposes only. For production, we recommend using the overload that takes a building action, which allows to fluently add configuration options from your `IConfiguration` instance and from other sources.

To empower your service with publishing capabilities, chain a `AddMessageBus` call to the infrastructure configuration method. This will inject an `IServiceResolver<IMessageBus>` and an `IServiceResolver<IMessageBus<DaprMetadata>>` as singleton services. Instances of `IMessageBus` (and `IMessageBus<DaprMetadata>`) are lazily created when needed and available as singletons. Furthermore, if a default bus is configured, a default `IMessageBus` (and `IMessageBus<DaprMetadata>`) will be added to the services: this is expecially useful when only a single message broker is used in the architecture.

To register handlers, use the `AddMessageHandlers` method, specifying the assemblies where your handlers are defined. If no assemblies are defined the caller assembly will be used.

An example of how you can configure services for both publishing and subscribing is shown below.

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddDaprInfrastructure(cfg => cfg.AddServiceOptions(Configuration))
        .AddMessageBus()
        .AddMessageHandlers();
}
```

The last step required to allow the microservice to receive messages is exposing the endpoints that the Dapr sidecar will call to deliver them. To do so, call the `MapMessageHandlers` method inside the endpoint configuration delegate.

```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapMessageHandlers();
    });
}
```

Note that `MapMessageHandlers` requires that handlers are registered as services, so it will throw if `AddMessageHandlers` was not called in the `ConfigureServices` method.
