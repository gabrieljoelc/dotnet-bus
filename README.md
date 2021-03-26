# Dotnet bus

Use conventions to publish webhook requests to the bus.

Components:

1. Create topics, subscriptions on app startup - Charles

Prototype creating topics and subscriptions (hardcoded) in Console App using [ManagementClient](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.servicebus.management.managementclient?view=azure-dotnet) class.

- inputs: service bus namespace connection string (config / app settings), topic name string, subscription name string
- outputs: service bus resources are created if not existing in Azure

```
public interface ITopicAndSubscriptionBuilder
{
    Task Build(string topic, string subscription name);
}

internal class AzureServiceBusTopicAndSubscriptionBuilder
{
    public async Task Build(string topic, string subscription)
    {
        if (!ManagmentClient.TopicExists(topic)) ///...
        if (!ManagmentClient.SubscriptionExists(subscription)) ///...
    }
}
```

2. Infer topic, subscription names from strong-types - Gabe

Prototype an interface that can provide topic and subscription discovery at app startup.

- inputs: interface implementations in assembly
- outputs: topic and subscription name strings

```
public interface IHandler<TMessage>
{
    Task Handle(TMessage message);
}

var topicsAndSubscription = Assembly.GetTypes().Where(x => x.IsInterface(typeof(IHandler<>))).Select(x => new { x.Topic, x.Subscription);
foreach (var topicAndSub in topicsAndSubscription)
{
    TopicAndSubscriptionBuilder.Build(topicAndSub.Topic, topicAndSub.Subscription);
}
```

3. Process web request and publish to the bus in one class - todo
