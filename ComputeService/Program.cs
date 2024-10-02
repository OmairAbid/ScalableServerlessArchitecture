using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MyMessageConsumer>();  // Register the consumer

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");  // Replace with actual RabbitMQ host
        cfg.ReceiveEndpoint("my-queue", e =>
        {
            e.ConfigureConsumer<MyMessageConsumer>(context);  // Consume from the queue
        });
    });
});

var app = builder.Build();

app.Run();


public class MyMessageConsumer : IConsumer<MyMessage>
{
    public Task Consume(ConsumeContext<MyMessage> context)
    {
        Console.WriteLine($"Received message: {context.Message.Text}");
        return Task.CompletedTask;
    }
}

public record MyMessage
{
    public string Text { get; init; }
}
