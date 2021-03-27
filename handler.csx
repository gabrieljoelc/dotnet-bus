#!/usr/bin/env dotnet-script

Console.WriteLine("Hello World!");


var handlers = GetHandlerInfos(AppDomain.CurrentDomain.GetAssemblies());

var handlerString = string.Join(", ", handlers.Select(x => x));
Console.WriteLine($"handlers: {handlerString}");

public interface IBusHandler<TMessage>
{
    Task Handle(TMessage message);
}

public class DemoBusHandler : IBusHandler<DemoCompleted>
{
    public async Task Handle(DemoCompleted message)
    {
        await Task.Delay(100);
        Console.WriteLine($"Was epic? {message.WasEpic}");
    }
}

public class DemoCompleted
{
    public bool WasEpic { get; set; }
}

public class HandlerInfo
{
    public string Topic { get; set; }
    public string Subscription { get; set; }

    public override string ToString()
    {
        return $"Topic: {Topic} => Subscription: {Subscription}";
    }
}

private static IEnumerable<HandlerInfo> GetHandlerInfos(IEnumerable<System.Reflection.Assembly> assembiles)
{
    var types = assembiles.SelectMany(s => s.GetTypes());
    var typeTuples = GetAllTypesImplementingOpenGenericType(types, typeof(IBusHandler<>));
    foreach (var type in typeTuples)
    {
        yield return new HandlerInfo { Subscription = type.Item1.Name, Topic = type.Item2.Name };
    }
}

private static IEnumerable<(Type, Type)> GetAllTypesImplementingOpenGenericType(IEnumerable<Type> types, Type openGenericType)
{
    return from x in types
            from z in x.GetInterfaces()
            let y = x.BaseType
            let itWasY = y != null && y.IsGenericType && openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())
            let itWasZ = !itWasY && z.IsGenericType && openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition())
            let chooseType = itWasY ? y : z
            where itWasY || itWasZ
            select (x, chooseType.GenericTypeArguments.Single());
}
