using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace ChatterClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        // Is this program connected to the server?
        bool connecting = false;

        public LoginPage()
        {
            InitializeComponent();
        }

        // Shows a simple connection error.
        private void ShowGenericConnectionError(string context)
        {
            MessageBox.Show(
                    string.Format("Unable to connect to the server. Try using 'localhost' to connect to your own, or try a different server IP address and port.\nContext: {0}", context),
                    "Chatter Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }

        // Load the default values for the server identification field.
        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            ServerIPBox.Text = ClientVars.connectedServer.ip;
            ServerPortBox.Text = ClientVars.connectedServer.port.ToString();
            UsernameBox.Text = ClientVars.userName;
        }

        // Attempts to connect to the server.
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!connecting)
            {
                // Set the values for server, port and username.
                ClientVars.connectedServer.ip = ServerIPBox.Text;

                // Try to parse the text as a number. Else, set the port to defaults.
                int port = 0;
                if (!int.TryParse(ServerPortBox.Text, out port))
                {
                    port = ClientVars.connectedServer.defaultPort;
                }

                // Set the port and the username.
                ClientVars.connectedServer.port = port;
                ClientVars.userName = UsernameBox.Text;

                // Change the button text to "Connecting..."
                connecting = true;
                LoginButton.Content = "Connecting...";

                string ip = ClientVars.connectedServer.ToString();

                try
                {
                    // Create the client object.
                    ClientVars.clientHandler = new ClientHandler(ip);

                    // Ping the server to see if it's active.
                    bool result = await ClientVars.clientHandler.PingServer(ClientVars.userName);

                    if (result)
                    {
                        // Open the Chat dialog. When it opens, it'll automatically grab any messages stored on the server.
                        ChatWindow window = new ChatWindow();
                        window.Show();

                        // Close this window.
                        Close();
                    }
                    else
                    {
                        // Show an error related to the server being unaccessible as it is down.
                        ShowGenericConnectionError(string.Format("Server \"{0}\" is unavailable.", ip));
                        connecting = false;
                        LoginButton.Content = "Log In";
                    }
                }
                catch (Exception ex)
                {
                    // Show an error related to the server being unaccessible due to a critical error.
                    ShowGenericConnectionError(string.Format("Server \"{0}\" is unavailable. {1} ({2})", ip, ex.Message, ex.Source));
                    connecting = false;
                    LoginButton.Content = "Log In";
                }
            }
        }
    }
}