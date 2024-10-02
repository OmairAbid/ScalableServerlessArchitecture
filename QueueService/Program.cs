using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        configurator.ConfigureEndpoints(context);
    });
});

var app = builder.Build();


// if you want to explicitly send a message to a specific queue instead of publishing
// it (which can be more useful in point-to-point communication, as opposed to pub/sub),
// you would use ISendEndpoint instead of IPublishEndpoint.

app.MapGet("hello/", async (ISendEndpointProvider sendEndpointProvider) =>
{
    var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("rabbitmq://localhost/my-queue"));
    var message = new MyMessage { Text = "Hello from Microservice 1" };
    await endpoint.Send(message);  // Send the message to the specified queue
    return "Message Sent to my-queue!";
});

app.Run();


public record MyMessage
{
    public string Text { get; init; }
}
