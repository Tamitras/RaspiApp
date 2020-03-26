using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinApp1.ViewModels.ButtonViewModels;
using XamarinApp1.ViewModels.LabelViewModels;

namespace XamarinApp1.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public ButtonViewModel ButtonUdpViewModel { get; set; }
        public ButtonViewModel ButtonTcpViewModel { get; set; }
        public ButtonViewModel ButtonWcfViewModel { get; set; }  
        public ButtonViewModel ButtonStartStopViewModel { get; set; }

        public LabelViewModel LabelCurrentSongTitle { get; set; }


        public AboutViewModel()
        {
            ButtonUdpViewModel = new ButtonViewModel(true, "UDP ", Color.IndianRed);
            ButtonTcpViewModel = new ButtonViewModel(true, "TCP ", Color.IndianRed);
            ButtonWcfViewModel = new ButtonViewModel(true, "WCF ", Color.IndianRed);
            ButtonStartStopViewModel = new ButtonViewModel(true, "Play/Pause ", Color.BlueViolet);
            LabelCurrentSongTitle = new LabelViewModel(true, "Keine Daten empfangen", Color.BlueViolet);

            Title = "Am Server verbinden";
        }
    }
}