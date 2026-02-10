using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ChatterClient
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        // Should this program continue to update?
        bool isAllowedToUpdate = true;

        public ChatWindow()
        {
            InitializeComponent();
            // Used to scroll any new messages into view.
            ((INotifyCollectionChanged)messageListBox.Items).CollectionChanged += MessageListBox_CollectionChanged;
        }

        // Updates the chat client with information from the server.
        private async void Update()
        {
            try
            {
                if (ClientVars.clientHandler != null)
                {
                    // Update the message list.
                    List<string>? messages = await ClientVars.clientHandler.UpdateSubscriptions();

                    if (messages != null)
                    {
                        // Clear the chat window.
                        messageListBox.Items.Clear();

                        foreach (string message in messages)
                        {
                            // Creates an item and adds the message to the message panel.
                            ListBoxItem messageItem = new ListBoxItem();
                            messageItem.Content = message;

                            messageListBox.Items.Add(messageItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ClientVars.clientHandler != null)
                {
                    // Send a ping to make sure that the server is indeed down.
                    bool result = await ClientVars.clientHandler.PingServer(ClientVars.userName);

                    if (!result)
                    {
                        // Stop the updating NOW. This isn't not connected anymore.
                        if (isAllowedToUpdate)
                        {
                            isAllowedToUpdate = false;

                            MessageBoxResult res = ClientVars.clientHandler.ShowClientError(
                                "Unable to connect to the server. Going back to the login screen.",
                                string.Format("{0} ({1})", ex.Message, ex.Source));

                            if (res == MessageBoxResult.OK)
                            {
                                Close();
                            }
                        }
                    }
                }
            }
        }

        private void Shutdown()
        {
            // Stop the updating NOW. This isn't not connected anymore.
            if (isAllowedToUpdate)
            {
                isAllowedToUpdate = false;
            }

            // Tell the client handler that we are no longer connected to the server.
            if (ClientVars.clientHandler != null && ClientVars.clientHandler.isServerActive)
            {
                ClientVars.clientHandler.isServerActive = false;
            }

            // Send us back to the login screen.
            LoginPage window = new LoginPage();
            window.Show();
        }

        private async void SendMessage()
        {
            // Check if the message isn't empty and if the client handler exists.
            if (ClientVars.clientHandler != null && !string.IsNullOrWhiteSpace(messageBox.Text))
            {
                // Send the message.
                ClientVars.clientHandler.SendMessage(messageBox.Text);

                // Clear the text in the message box after sending the message.
                messageBox.Text = "";

                // Update the message panel.
                Update();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the username label to the username.
            UsernameLabel.Content = ClientVars.userName;

            // Create a task that updates the messages.
            Task.Run(async () => 
            {
                // If the client can update...
                while (isAllowedToUpdate)
                {
                    // Update every 0.2 seconds (or 200 ticks).
                    await Task.Delay(200);

                    // Run the Update function on the UI thread.
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        Update();
                    }));
                }
            });
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // When enter is pressed, send the message.
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        // Used to scroll any new messages into view.
        private void MessageListBox_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                messageListBox.ScrollIntoView(e.NewItems[0]);
            }
        }

        // Launch the login page when shut down.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Shutdown();
        }

        // Send a message when the button is pressed.
        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }
    }
}
