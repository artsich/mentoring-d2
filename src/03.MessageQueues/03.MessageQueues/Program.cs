using _03.MessageQueues;
using _03.MessageQueues.FileService;
using _03.MessageQueues.Messaging;
using _03.MessageQueues.Processing;
using Azure.Messaging.ServiceBus;

// generate big file:
// fsutil file createnew large3.txt 1000000000 ~ 100mb

var options1 = new ListenOptions()
{
	Folder = ".\\ListenFolder",
	Filter = "*.txt"
};

var options2 = new ListenOptions()
{
	Folder = ".\\ListenFolder",
	Filter = "*.mp3"
};

var receivedFolderPath = ".\\Received";

PrepareFolder(options1.Folder);
PrepareFolder(options2.Folder);
PrepareFolder(receivedFolderPath);

string connectionString = "Endpoint=sb://mentoringservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=dQxwwtBpKQFZaw3sFNYgKKzR8V8yrqCUls2r8wAp3OU=";
string queueName = "files-queue";

await using var client = new ServiceBusClient(connectionString);

GetCaptureService(options1).Run();
GetCaptureService(options2).Run();

var processingService = new MainProcessingService(
	new MessageReceiver(client.CreateReceiver(queueName)),
	receivedFolderPath);

while (true) 
{
	Console.WriteLine("Start proccessing.");

	await processingService.Process();

	Console.WriteLine("Finish processing.");
}

DataCaptureService GetCaptureService(ListenOptions options)
	=> new DataCaptureService(options, new FileService(new MessageSender(client.CreateSender(queueName))));

void PrepareFolder(string folder)
{
	Directory.CreateDirectory(folder);
}