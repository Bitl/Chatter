namespace ChatterClient
{
    // This class defines a server and how it's saved in memory in the client.
    public class ServerDef
    {
        // The server's IP address.
        public string ip { get; set; } = "http://localhost";

        // The default server port.
        public int defaultPort { get; } = 80;

        // The server's port.
        public int port { get; set; } = 80;

        // The constructors for the server.
        public ServerDef() { }

        // Override the ToString() function to return the IP and the port.
        public override string ToString()
        {
            return string.Format("{0}:{1}", ip, port);
        }
    }
}
