#!/usr/bin/env dotnet-script

Console.WriteLine("Hello World!");


var types = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(s => s.GetTypes());

types = GetAllTypesImplementingOpenGenericType(types, typeof(IBusHandler<>));

var typeNames = string.Join(", ", types.Select(x => x.Name));
Console.WriteLine($"handlers: {typeNames}");

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
}

private static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(IEnumerable<Type> types, Type openGenericType)
{
    return from x in types
            from z in x.GetInterfaces()
            let y = x.BaseType
            where
            (y != null && y.IsGenericType &&
            openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
            (z.IsGenericType &&
            openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
            select x;
}
