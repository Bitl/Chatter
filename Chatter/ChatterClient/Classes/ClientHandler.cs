using ChatterShared;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System.Windows;

namespace ChatterClient
{
    // Used to connect Chatter to the server, and do things while its connected.
    public class ClientHandler
    {
        // If this is able to get a response from the server, this stays true.
        // If this gets any error, it's set to false.
        public bool isServerActive = false;

        // Client object.
        private Chatter.ChatterClient? client;

        // The server's IP address. Sent over through the client's constructor.
        private string? serverIP = null;

        public ClientHandler(string ip)
        {
            serverIP = ip;
        }

        public MessageBoxResult ShowClientError(string text, string context)
        {
            return MessageBox.Show(
                    string.Format("{0}\nContext: {1}", text, context),
                    "Chatter Client Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }

        // Gets the current Chatter.ChatterClient object, or creates one if it doesn't exist.
        public Chatter.ChatterClient GetClientHandler()
        {
            if (serverIP == null)
            {
                // Throw an exception if the server IP doesn't exist.
                throw new Exception("Server IP not found.");
            }

            if (client == null)
            {
                // Create a client if one doesn't exist.
                GrpcChannel channel = GrpcChannel.ForAddress(serverIP);
                Chatter.ChatterClient newClient = new Chatter.ChatterClient(channel);
                client = newClient;
            }

            return client;
        }

        // Calls the "ping" message on the server.
        public async Task<bool> PingServer(string username)
        {
            try
            {
                // Get the client handler.
                Chatter.ChatterClient client = GetClientHandler();

                // Send a Ping request.
                var response = await client.PingAsync(new Message
                {
                    Username = username,
                    Text = "Pong"
                });

                if (response != null)
                {
                    isServerActive = true;
                }

                return (response != null);
            }
            catch (Exception)
            {
                isServerActive = false;
                return false;
            }
        }

        // Updates the messages by asking the server for the updated message list.
        // The chat window should wipe all messages before running this!!
        // Exception handling is done by the Update() method.
        public async Task<List<string>?> UpdateSubscriptions()
        {
            // Get the client handler.
            Chatter.ChatterClient client = GetClientHandler();

            // Update the messages.
            var response = await client.UpdateSubscriptionsAsync(new Empty());

            if (response != null)
            {
                // Return the messages sent from the server.
                isServerActive = true;

                // Go through each message and add it to the list in a parallel thread.
                List<string> messages = new List<string>();
                Parallel.ForEach(response.Messages, messages.Add);

                return messages;
            }

            isServerActive = false;
            return null;
        }

        // Sends a message, then adds it to the server. Afterwards, update the message list.
        public async void SendMessage(string textMessage)
        {
            try
            {
                // Get the client handler.
                Chatter.ChatterClient client = GetClientHandler();

                // Send the message.
                var response = await client.SendMessageAsync(new Message
                {
                    Username = ClientVars.userName,
                    Text = textMessage
                });
            }
            catch (Exception ex)
            {
                // Show a message error.
                ShowClientError("Unable to send the message.", string.Format("{0} ({1})", ex.Message, ex.Source));
            }
        }
    }
}
