using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApp1.Interfaces;
using XamarinApp1.UDP;
using XamarinApp1.ViewModels;

namespace XamarinApp1.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        public IPAddress IPAddress { get; set; }

        /// <summary>
        /// Hostname des Clients
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gibt an ob der Client mit einem lokalen Server verbunden ist
        /// </summary>
        public Boolean IsLocalServer { get; set; }

        private static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// IpAdresse des HauptServers
        /// </summary>
        public String ServerIpAdress { get; set; }
        private bool CloseApplication = false;

        private AboutViewModel AboutViewModel { get; set; }

        public AboutPage()
        {
            InitializeComponent();

            this.BindingContext = this.AboutViewModel = new AboutViewModel();
        }

        private void CreateUDPConnection()
        {
            this.HostName = Dns.GetHostName();
            var hostEntry = Dns.GetHostEntry(this.HostName);

            var ipAdresses = hostEntry.AddressList.Where(c => c.AddressFamily == AddressFamily.InterNetwork).ToList();
            var ipAdress = ipAdresses.FirstOrDefault();
            IPAddress = ipAdress;

            new Thread(() =>
            {
                GetNetworkServerAdressOverUDP();
            }).Start();
        }

        /// <summary>
        /// Init-Methode
        /// </summary>
        private void GetNetworkServerAdressOverUDP()
        {
            UDPMessage message = new UDPMessage();
            UdpClient udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 5000;
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 1336);
            BroadCastSend(message, udpClient, remoteEndPoint);
        }

        /// <summary>
        /// Sendet einen Broadcast ins Netzwerk
        /// Wenn einer darauf antwortet, dann mit seiner IPAdresse
        /// </summary>
        /// <returns></returns>
        private void BroadCastSend(UDPMessage udpMessage, UdpClient udpClient, IPEndPoint remoteEndPoint)
        {
            IsLocalServer = true;
            try
            {
                //Broadcast wird versendet
                udpClient.Send(udpMessage.DataInBytes, udpMessage.Data.Length, udpMessage.TargetAddress, udpMessage.Port);
                try
                {
                    //Daten werden aus dem udpStream gelesen
                    udpMessage.DataInBytes = udpClient.Receive(ref remoteEndPoint);
                    String messageFromServer = udpMessage.ReadBytes(udpMessage.DataInBytes);

                    if (null != messageFromServer)
                    {
                        ServerIpAdress = messageFromServer;

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            this.AboutViewModel.ButtonUdpViewModel.BackgroundColor = Color.Green;
                            this.AboutViewModel.ButtonUdpViewModel.Text += " Geöffnet";
                            this.AboutViewModel.ButtonUdpViewModel.Enabled = false;
                        });


                        udpClient.Close();
                    }
                    else //Keine Antwort zurück bekommen
                    {
                        // Server im Internet wird verwendet
                        udpClient.Close();
                        udpClient.Client.Disconnect(true);
                        // Wenn man die Verbindung über das Internet laufen lassen möchte
                        //ConnectionString = Properties.Settings.Default.ConnectionString;
                        //IsLocalServer = false;
                    }
                }
                catch (Exception timeout)
                {
                    udpClient.Close();
                    // Wenn man die Verbindung über das Internet laufen lassen möchte
                    //ConnectionString = Properties.Settings.Default.ConnectionString;
                    IsLocalServer = false;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message); //TODO:
            }
        }



        private void ConnectToTCPSocketServer()
        {
            try
            {
                Int32 port = 1337;
                IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse(this.ServerIpAdress), port);
                TcpClient client = new TcpClient();
                client.Connect(serverAddress);

                NetworkStream stream = client.GetStream();
                ASCIIEncoding encoder = new ASCIIEncoding();

                Thread.Sleep(20);

                // Überlieferung des Hostnames
                Byte[] bufferToClient = encoder.GetBytes(this.HostName);
                client.GetStream().Write(bufferToClient, 0, bufferToClient.Length);
                client.GetStream().Flush();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    this.AboutViewModel.ButtonTcpViewModel.BackgroundColor = Color.Green;
                    this.AboutViewModel.ButtonTcpViewModel.Text += " Geöffnet";
                    this.AboutViewModel.ButtonTcpViewModel.Enabled = false;
                });

                Thread.Sleep(20);

                while (!CloseApplication)
                {
                    WaitForTCPSocketServerMessages(client, stream);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
        }

        private void WaitForTCPSocketServerMessages(TcpClient client, NetworkStream stream)
        {
            Thread.Sleep(2000);

            if (stream.DataAvailable
                && client.Connected)
            {
                byte[] message = new byte[4096]; // Dies ist unser Buffer
                int bytesRead;
                bytesRead = stream.Read(message, 0, 4096);

                if (bytesRead > 0)
                {
                    String receivedMessage = new ASCIIEncoding().GetString(message);
                    if (receivedMessage.Contains("Close"))
                    {
                        client.Close();
                        stream.Close();
                        CloseApplication = true;
                    }
                }
            }
        }

        private async void ConnectToWCFService()
        {
            try
            {
                string uri = $"http://{ServerIpAdress}:8082";
                Client.BaseAddress = new Uri(uri);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                this.AboutViewModel.ButtonWcfViewModel.BackgroundColor = Color.Green;
                this.AboutViewModel.ButtonWcfViewModel.Text = "HTTP Verbunden";
                this.AboutViewModel.ButtonWcfViewModel.Enabled = false;

                // SendTestMessage
                HttpResponseMessage resp = Client.GetAsync("api/Spotify/Register").Result;
                resp.EnsureSuccessStatusCode();

                Thread.Sleep(2000);

                this.AboutViewModel.ButtonWcfViewModel.Text = await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void ButtonUdp_Clicked(object sender, EventArgs e)
        {
            CreateUDPConnection();
        }

        private void ButtonTcp_Clicked(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                this.ConnectToTCPSocketServer();
            }).Start();
        }

        private void ButtonWcf_Clicked(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                this.ConnectToWCFService();
            }).Start();
        }

        private async void ButtonStartStop_Clicked(object sender, EventArgs e)
        {
            try
            {
                // SendTestMessage
                //HttpResponseMessage resp = Client.GetAsync("api/Spotify/PlayPause").Result;

                var hostname = Dns.GetHostName();
                var adresses = Dns.GetHostAddresses(hostname);
                hostname = "Eriks Samsung";
                var ipAdresses = adresses.Where(c => c.AddressFamily == AddressFamily.InterNetwork).ToList();
                var ipAdress = ipAdresses.FirstOrDefault();
                var ipAdressBytes = ipAdress.GetAddressBytes();

                HttpResponseMessage resp = Client.GetAsync($"api/Spotify/PlayPause?ipAdress={ipAdress}&hostname={hostname}").Result;
                resp.EnsureSuccessStatusCode();
                
                this.AboutViewModel.LabelCurrentSongTitle.Text = await resp.Content.ReadAsStringAsync();                
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}