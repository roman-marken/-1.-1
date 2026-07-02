using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSocketTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            
            Console.WriteLine("Виберіть завдання для запуску:");
            Console.WriteLine("1 - Завдання 1 (Синхронний чат-сервер)");
            Console.WriteLine("2 - Завдання 1 (Синхронний чат-клієнт)");
            Console.WriteLine("3 - Завдання 2 (Синхронний сервер часу/дати)");
            Console.WriteLine("4 - Завдання 2 (Синхронний клієнт часу/дати)");
            Console.WriteLine("5 - Завдання 3 (Асинхронний чат-сервер)");
            Console.WriteLine("6 - Завдання 3 (Асинхронний чат-клієнт)");
            Console.WriteLine("7 - Завдання 4 (Асинхронний сервер часу/дати)");
            Console.WriteLine("8 - Завдання 4 (Асинхронний клієнт часу/дати)");
            Console.Write("Ваш вибір: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    SyncChatServer();
                    break;
                case "2":
                    SyncChatClient();
                    break;
                case "3":
                    SyncTimeServer();
                    break;
                case "4":
                    SyncTimeClient();
                    break;
                case "5":
                    AsyncChatServer().Wait();
                    break;
                case "6":
                    AsyncChatClient().Wait();
                    break;
                case "7":
                    AsyncTimeServer().Wait();
                    break;
                case "8":
                    AsyncTimeClient().Wait();
                    break;
                default:
                    Console.WriteLine("Невірний вибір.");
                    break;
            }
            
            Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }

        static void SyncChatServer()
        {
            int port = 8888;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Сервер запущено на порту {port}. Очікування клієнта...");
            
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            string clientEndpoint = client.Client.RemoteEndPoint.ToString();
            string time = DateTime.Now.ToString("HH:mm");
            Console.WriteLine($"О {time} від [{clientEndpoint}] отримано рядок: {receivedMessage}");
            
            string response = "Привіт, клієнт!";
            byte[] responseData = Encoding.UTF8.GetBytes(response);
            stream.Write(responseData, 0, responseData.Length);
            
            stream.Close();
            client.Close();
            listener.Stop();
        }

        static void SyncChatClient()
        {
            string serverIP = "127.0.0.1";
            int port = 8888;
            
            TcpClient client = new TcpClient();
            client.Connect(serverIP, port);
            NetworkStream stream = client.GetStream();
            
            string message = "Привіт, сервер!";
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            string serverEndpoint = client.Client.RemoteEndPoint.ToString();
            string time = DateTime.Now.ToString("HH:mm");
            Console.WriteLine($"О {time} від [{serverEndpoint}] отримано рядок: {response}");
            
            stream.Close();
            client.Close();
        }

        static void SyncTimeServer()
        {
            int port = 8889;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Сервер часу/дати запущено на порту {port}. Очікування клієнта...");
            
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim().ToLower();
            
            string response = "";
            if (request == "time" || request == "час")
            {
                response = DateTime.Now.ToString("HH:mm:ss");
            }
            else if (request == "date" || request == "дата")
            {
                response = DateTime.Now.ToString("dd.MM.yyyy");
            }
            else
            {
                response = "Невідомий запит. Використовуйте 'time' або 'date'.";
            }
            
            byte[] responseData = Encoding.UTF8.GetBytes(response);
            stream.Write(responseData, 0, responseData.Length);
            
            stream.Close();
            client.Close();
            listener.Stop();
        }

        static void SyncTimeClient()
        {
            string serverIP = "127.0.0.1";
            int port = 8889;
            
            Console.Write("Введіть запит (time/date або час/дата): ");
            string request = Console.ReadLine();
            
            TcpClient client = new TcpClient();
            client.Connect(serverIP, port);
            NetworkStream stream = client.GetStream();
            
            byte[] data = Encoding.UTF8.GetBytes(request);
            stream.Write(data, 0, data.Length);
            
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            Console.WriteLine($"Відповідь сервера: {response}");
            
            stream.Close();
            client.Close();
        }

        static async Task AsyncChatServer()
        {
            int port = 8890;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Асинхронний сервер запущено на порту {port}. Очікування клієнта...");
            
            TcpClient client = await listener.AcceptTcpClientAsync();
            NetworkStream stream = client.GetStream();
            
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            string clientEndpoint = client.Client.RemoteEndPoint.ToString();
            string time = DateTime.Now.ToString("HH:mm");
            Console.WriteLine($"О {time} від [{clientEndpoint}] отримано рядок: {receivedMessage}");
            
            string response = "Привіт, клієнт!";
            byte[] responseData = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(responseData, 0, responseData.Length);
            
            stream.Close();
            client.Close();
            listener.Stop();
        }

        static async Task AsyncChatClient()
        {
            string serverIP = "127.0.0.1";
            int port = 8890;
            
            TcpClient client = new TcpClient();
            await client.ConnectAsync(serverIP, port);
            NetworkStream stream = client.GetStream();
            
            string message = "Привіт, сервер!";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
            
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            string serverEndpoint = client.Client.RemoteEndPoint.ToString();
            string time = DateTime.Now.ToString("HH:mm");
            Console.WriteLine($"О {time} від [{serverEndpoint}] отримано рядок: {response}");
            
            stream.Close();
            client.Close();
        }

        static async Task AsyncTimeServer()
        {
            int port = 8891;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Асинхронний сервер часу/дати запущено на порту {port}. Очікування клієнта...");
            
            TcpClient client = await listener.AcceptTcpClientAsync();
            NetworkStream stream = client.GetStream();
            
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim().ToLower();
            
            string response = "";
            if (request == "time" || request == "час")
            {
                response = DateTime.Now.ToString("HH:mm:ss");
            }
            else if (request == "date" || request == "дата")
            {
                response = DateTime.Now.ToString("dd.MM.yyyy");
            }
            else
            {
                response = "Невідомий запит. Використовуйте 'time' або 'date'.";
            }
            
            byte[] responseData = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(responseData, 0, responseData.Length);
            
            stream.Close();
            client.Close();
            listener.Stop();
        }

        static async Task AsyncTimeClient()
        {
            string serverIP = "127.0.0.1";
            int port = 8891;
            
            Console.Write("Введіть запит (time/date або час/дата): ");
            string request = Console.ReadLine();
            
            TcpClient client = new TcpClient();
            await client.ConnectAsync(serverIP, port);
            NetworkStream stream = client.GetStream();
            
            byte[] data = Encoding.UTF8.GetBytes(request);
            await stream.WriteAsync(data, 0, data.Length);
            
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            Console.WriteLine($"Відповідь сервера: {response}");
            
            stream.Close();
            client.Close();
        }
    }
}