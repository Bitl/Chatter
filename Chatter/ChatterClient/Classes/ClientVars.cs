namespace ChatterClient
{
    // This class stores variables that are recalled by the client.
    public static class ClientVars
    {
        // The currently connected server.
        public static ServerDef connectedServer = new ServerDef();

        // The username of the user.
        public static string userName = "User";

        // The client associated with this instance of the application. Null unless called by the LoginPage.
        public static ClientHandler? clientHandler = null;
    }
}
