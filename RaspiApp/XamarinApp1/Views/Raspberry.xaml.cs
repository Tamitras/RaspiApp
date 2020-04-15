using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApp1.ViewModels;

namespace XamarinApp1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Raspberry : ContentPage
    {
        private const string NoConnection = "Keine Verbindung zum Server möglich";
        private static readonly HttpClient Client = new HttpClient();
        private RaspberryViewModel RaspberryViewModel { get; set; }
        private bool LedStatus { get; set; }
        public Raspberry()
        {
            InitializeComponent();
            this.BindingContext = this.RaspberryViewModel = new RaspberryViewModel();
            string uri = $"http://192.168.0.53:5000";
            Client.BaseAddress = new Uri(uri);
            Client.Timeout = new TimeSpan(0, 0, 5); // 5 Sekunden
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            new Task(() =>
            {
                while (true)
                {
                    this.GetDataFromServer();
                }

            }).Start();
        }


        private async void Button_Hello_Clicked(object sender, EventArgs e)
        {
            await SayHello();
        }

        private async Task SayHello()
        {
            try
            {
                HttpResponseMessage resp = Client.GetAsync($"Main/Register").Result;
                resp.EnsureSuccessStatusCode();
                this.RaspberryViewModel.LabelResponseViewModel.Text = await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                this.RaspberryViewModel.LabelResponseViewModel.Text = ex.Message;
            }
        }

        private async void GetDataFromServer()
        {
            try
            {
                Thread.Sleep(1000);

                HttpResponseMessage resp = Client.GetAsync($"Main/GetChannel?Channel=0").Result;
                resp.EnsureSuccessStatusCode();
                this.RaspberryViewModel.LabelLightSensorViewModel.Text = "Lichtsensor: " + await resp.Content.ReadAsStringAsync();

                if (!this.RaspberryViewModel.LabelResponseViewModel.Text.Contains("Willkommen"))
                {
                    await SayHello();
                }

                this.RaspberryViewModel.BindableToolbarItem.IsVisible = true;
            }
            catch (ArgumentNullException argNullEx)
            {
                this.RaspberryViewModel.LabelLightSensorViewModel.Text = argNullEx.Message;
                this.RaspberryViewModel.BindableToolbarItem.IsVisible = false;
                this.RaspberryViewModel.LabelResponseViewModel.Text = NoConnection;
            }
            catch (HttpRequestException httpEx)
            {
                this.RaspberryViewModel.LabelLightSensorViewModel.Text = httpEx.Message;
                this.RaspberryViewModel.BindableToolbarItem.IsVisible = false;
                this.RaspberryViewModel.LabelResponseViewModel.Text = NoConnection;
            }
            catch (Exception ex)
            {
                this.RaspberryViewModel.LabelLightSensorViewModel.Text = ex.Message;
                this.RaspberryViewModel.BindableToolbarItem.IsVisible = false;
                this.RaspberryViewModel.LabelResponseViewModel.Text = NoConnection;
            }
            finally
            {
            }
        }

        private async void Button_ToggleLed_Clicked(object sender, EventArgs e)
        {
            try
            {
                var pin = 23;

                var ledStatusAsInt = !this.LedStatus == true ? 1 : 0;

                HttpResponseMessage resp = Client.GetAsync($"Main/SetGPIO?pin={pin}&value={ledStatusAsInt}").Result;
                resp.EnsureSuccessStatusCode();

                var res = await resp.Content.ReadAsStringAsync();
                if (bool.TryParse(res, out bool value))
                {
                    this.LedStatus = value;
                    this.RaspberryViewModel.ButtonToggleLedViewModel.Text = value == true ? $"LED an" : "LED aus";

                    if (this.LedStatus) // LED an
                    {
                        this.RaspberryViewModel.ButtonToggleLedViewModel.BackgroundColor = Color.Green;
                    }
                    else
                    {
                        this.RaspberryViewModel.ButtonToggleLedViewModel.BackgroundColor = Color.Gray;
                    }
                }
            }
            catch (Exception ex)
            {
                this.RaspberryViewModel.ButtonToggleLedViewModel.Text = ex.Message;
            }
        }
    }
}