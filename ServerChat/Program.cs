using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

class ServerChat
{
    // TCP listener to accept incoming connections
    private readonly TcpListener tcpListener;

    // Subject that acts as the message bus to broadcast messages to all clients
    private readonly Subject<(string SenderId, string Message)> messagesBus = new();

    // Thread-safe dictionary to store connected clients by their IDs
    private readonly ConcurrentDictionary<string, TcpClient> clients = new();

    // Counter to assign unique client IDs
    private int clientsCounter = 1;

    public ServerChat(int port)
    {
        // Bind the TCP listener to any IP on the specified port
        tcpListener = new TcpListener(IPAddress.Any, port);
    }

    // Disconnect and remove a client by ID
    private void DisconnectClient(string clientId)
    {
        if (clients.TryRemove(clientId, out var client))
        {
            client.Close(); // Close the client's connection
            Console.WriteLine(Environment.NewLine + clientId + " disconnected.");
        }
    }

    // Handles a new client connection
    private void HandleClient(TcpClient client, string clientId)
    {
        var stream = client.GetStream();

        // Writer for sending messages to this client
        var streamWriter = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

        // Reader for receiving messages from this client
        var streamReader = new StreamReader(stream, Encoding.UTF8);

        // Create an observable sequence for incoming messages from the client
        var observableSequence = Observable.Create<string>(async (observer, cancellationToken) =>
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Read one line (message) from the client
                    var input = await streamReader.ReadLineAsync();

                    if (input == null)
                    {
                        break; // Client disconnected
                    }

                    // Push message into the observable stream
                    observer.OnNext(input);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(Environment.NewLine + "Error from " + clientId + ": " + exception.Message);
            }

            // Notify the end of the sequence
            observer.OnCompleted();
        });

        // Subscribe to the incoming messages from this client
        var subscription = observableSequence.Subscribe
        (
            // On each message: push it to the central message bus
            message => messagesBus.OnNext((clientId, message)),

            // On error: log it
            exception => Console.WriteLine(Environment.NewLine + "Error in " + clientId + "'s subscription: " + exception.Message),

            // On completion: disconnect the client
            () => DisconnectClient(clientId)
        );

        // Subscribe to the central message bus to broadcast messages to this client
        var broadcastSubscription = messagesBus
            .Select(a =>
            {
                // Format message with sender's name or "Me" if it's the same client
                var prefix = a.SenderId == clientId ? "Me" : a.SenderId;
                return prefix + " [" + DateTime.Now + "]: " + a.Message + Environment.NewLine;
            })
            .Subscribe(async message =>
            {
                try
                {
                    // Send the message to the client
                    await streamWriter.WriteLineAsync(message);
                }
                catch
                {
                    // If sending fails, disconnect the client
                    DisconnectClient(clientId);
                }
            });

        // Send welcome message to the newly connected client
        streamWriter.WriteLine(clientId + " welcome! Please write here!" + Environment.NewLine);
    }

    // Starts the TCP server and listens for clients
    public async Task StartAsync()
    {
        tcpListener.Start(); // Start listening on the port
        Console.WriteLine("Server started on port 9000! Connect from client using the command \"telnet localhost 9000\"" + Environment.NewLine + "Waiting for connections...");

        // Accept clients in a loop
        while (true)
        {
            var client = await tcpListener.AcceptTcpClientAsync();

            // Assign a unique ID to the client
            string clientId = "Client" + clientsCounter++;

            // Store the client connection
            clients[clientId] = client;

            Console.WriteLine(Environment.NewLine + clientId + " connected.");

            // Start handling communication with the client
            HandleClient(client, clientId);
        }
    }

    // Main entry point: starts the server
    public static async Task Main(string[] args)
    {
        Console.Title = "Rx.NET multi-client TCP chat server";
        var serverChat = new ServerChat(9000);
        await serverChat.StartAsync(); // Launch the server
    }
}
