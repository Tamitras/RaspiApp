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
        private static readonly HttpClient Client = new HttpClient();
        private RaspberryViewModel RaspberryViewModel { get; set; }
        private bool LedStatus { get; set;  }
        public Raspberry()
        {
            InitializeComponent();
            this.BindingContext = this.RaspberryViewModel = new RaspberryViewModel();
            string uri = $"http://192.168.0.53:5000";
            Client.BaseAddress = new Uri(uri);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            new Task(() =>
            {
                while (true)
                {
                    this.GetDataFromServer();
                    Thread.Sleep(1000);
                }

            }).Start();
        }


        private async void Button_Hello_Clicked(object sender, EventArgs e)
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
                HttpResponseMessage resp = Client.GetAsync($"Main/GetChannel?Channel=0").Result;
                resp.EnsureSuccessStatusCode();
                this.RaspberryViewModel.LabelLightSensorViewModel.Text = "Lichtsensor: " + await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                this.RaspberryViewModel.LabelLightSensorViewModel.Text = ex.Message;
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
                if(bool.TryParse(res, out bool value))
                {
                    this.LedStatus = value;
                    this.RaspberryViewModel.ButtonToggleLedViewModel.Text = value == true ? $"LED an" : "LED aus";

                    if(this.LedStatus) // LED an
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