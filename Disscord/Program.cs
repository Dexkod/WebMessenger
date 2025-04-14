using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;


var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5237/chat")
    .Build();

connection.On<string, string>("ReceiveMessage", (user, message) =>
{
    Console.WriteLine($"{user}: {message}");
});

await connection.StartAsync();

Console.WriteLine("Connected to SignalR hub. Type a message and press Enter to send.");

while (true)
{
    var message = Console.ReadLine();
    await connection.InvokeAsync("Send", "ConsoleClient", message);
}

//string outputFilePath = "C:\\CSMy\\Disscord\\Disscord\\output.wav";
//int sampleRate = 44100;
//int channels = 1;

//using (var waveIn = new WaveInEvent())
//{
//    waveIn.DeviceNumber = 0; 
//    waveIn.WaveFormat = new WaveFormat(sampleRate, channels);
//    waveIn.DataAvailable += (sender, e) =>
//    {
//    };

//    using (var writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat))
//    {
//        waveIn.DataAvailable += (sender, e) =>
//        {
//            writer.Write(e.Buffer, 0, e.Buffer.Length);
//        };

//        waveIn.StartRecording();

//        Console.WriteLine("Запись началась. Нажмите Enter для остановки...");
//        Console.ReadLine();

//        waveIn.StopRecording();
//        Console.WriteLine("Запись остановлена.");
//    }
//}

