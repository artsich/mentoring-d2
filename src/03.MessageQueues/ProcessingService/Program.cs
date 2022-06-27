using Azure.Messaging.ServiceBus;
using Messaging;
using Microsoft.Extensions.Configuration;
using ProcessingService;

// generate big file:
// fsutil file createnew large3.txt 1000000000 ~ 100mb

var config = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.Build();

await using var client = new ServiceBusClient(config["ServiceBus:ConnectionString"]);
var queueName = config["ServiceBus:QueueName"];
var outputFolder = config["OutputFolder"];

var processingService = 
	new MainProcessingService(
		new ChunkReceiver(
			client.CreateReceiver(queueName)),
		outputFolder);

while (true)
{
	try
	{
		Console.WriteLine("Start proccessing.");

		await processingService.Process();

		Console.WriteLine("Finish processing.");
	}
	catch (Exception e)
	{
		Console.WriteLine(e);
	}
}
