using Microsoft.AspNetCore.SignalR.Client;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
namespace SignalRClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection connection;
        public MainWindow()
        {
            InitializeComponent();
            connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7181/myhub")
            .WithAutomaticReconnect()
            .Build();

            connection.Reconnecting += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Attempting to reconnect...";
                    lstBoxMessages.Items.Add(newMessage);
                });

                return Task.CompletedTask;
            };

            connection.Reconnected += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Reconnected to the server";
                    lstBoxMessages.Items.Clear();
                    lstBoxMessages.Items.Add(newMessage);
                });

                return Task.CompletedTask;
            };

            connection.Closed += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Connection Closed";
                    lstBoxMessages.Items.Add(newMessage);
                    openConnection.IsEnabled = true;
                    sendMessage.IsEnabled = false;
                });

                return Task.CompletedTask;
            };

            // The name should same as in server
            connection.On<string, string>("ReceiveMessage", (userAction, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{userAction}: {message}";
                    lstBoxMessages.Items.Add(newMessage);

                    if (userAction == "Server:Token")
                    {
                        string signature = SignChallenge(messageInput.Text);
                        connection.InvokeAsync("SendMessage", "Client:Signature", signature);
                    }
                });
            });
        }

        private async void openConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.StartAsync();
                lstBoxMessages.Items.Add("Connection Started");
                openConnection.IsEnabled = false;
                sendMessage.IsEnabled = true;

                await connection.InvokeAsync("SendMessage",
                "Client:Start", "starts communication");

            }
            catch (Exception ex)
            {
                lstBoxMessages.Items.Add(ex.Message);
            }
        }

        private async void sendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = messageInput.Text;

                // below line will call to Server.MyHub.SendMessage()
                await connection.InvokeAsync("SendMessage",
                    "Client", message);
            }
            catch (Exception ex)
            {
                lstBoxMessages.Items.Add(ex.Message);
            }
        }

        /// <summary>
        // genarate Signature for generated random string received from Server using keyed hash algorithm HMACSHA1
        /// </summary>
        /// <param name="challenge"></param>
        /// <returns></returns>
        static string SignChallenge(string challenge)
        {
            using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes("vilas_secret_key")))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(challenge));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}