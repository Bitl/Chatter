using ChatterShared;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ChatterServerService
{
    public class ChatterService : Chatter.ChatterBase
    {
        // Logger for the server.
        private readonly ILogger<ChatterService> _logger;

        // Empty class that contains nothing for use in the SendMessage Task.
        private Empty _empty = new Empty();

        public ChatterService(ILogger<ChatterService> logger)
        {
            _logger = logger;
        }

        // This task adds the sent message to the list of messages and logs it.
        public override Task<Empty> SendMessage(Message request, ServerCallContext context)
        {
            // Messages are composed of 2 strings that are formatted together and then added to the server's persistent message storage.
            string message = string.Format("[{0}]: {1}", request.Username, request.Text);

            // Add the message to the message storage.
            ServerVars.messageStorage.Add(message);

            // Log it.
            _logger.LogInformation("New Message: {0}", message);
            _logger.LogInformation("Storage contents: {0}", string.Join(", ", ServerVars.messageStorage));

            // Log an empty object (basically nothing).
            return Task.FromResult(_empty);
        }

        // This task sends our saved messages to the client. This allows all clients that are connected to get the same messages from the server.
        public override Task<MessageList> UpdateSubscriptions(Empty nothing, ServerCallContext context)
        {
            // Create a new list and add the stored messages into it.
            MessageList list = new MessageList();
            list.Messages.AddRange(ServerVars.messageStorage.AsEnumerable());

            // Log client information and storage.
            _logger.LogInformation("Client requested message subscription update.");
            _logger.LogInformation("Update contents: {0}", string.Join(", ", list.Messages));

            // Return the list.
            return Task.FromResult(list);
        }

        // Respond to client pings. Uses the message class which in this case stores the client's username and "ping" as the message.
        public override Task<Message> Ping(Message msg, ServerCallContext context)
        {
            _logger.LogInformation("Recieving ping from {0}", msg.Username);

            // Send back a ping message.
            return Task.FromResult(new Message
            {
                Username = msg.Username,
                Text = "Pong"
            });
        }
    }
}
