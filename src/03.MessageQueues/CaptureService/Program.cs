using Messaging;
using CaptureService.FileService;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.ServiceBus;
using CaptureService;

var config = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.Build();

var listenOptions = config.GetSection("ListenOptions").Get<IList<ListenOptions>>();

await using var client = new ServiceBusClient(config["ServiceBus:ConnectionString"]);
var queueName = config["ServiceBus:QueueName"];

foreach (var option in listenOptions)
{
	GetCaptureService(option).Run();
}

DataCaptureService GetCaptureService(ListenOptions options)
	=> new(options, new FileService(
		new MessageSender(
				client.CreateSender(queueName))));

do
{
	Console.WriteLine("Press Q to stop!");
}
while (Console.ReadKey().Key != ConsoleKey.Q);

