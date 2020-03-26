using System.Drawing;
using XamarinApp1.ViewModels.ButtonViewModels;
using XamarinApp1.ViewModels.LabelViewModels;

namespace XamarinApp1.ViewModels
{
    public class RaspberryViewModel : BaseViewModel
    {
        public ButtonViewModel ButtonHellpViewModel { get; set; }
        public LabelViewModel LabelResponseViewModel { get; set; }
        public LabelViewModel LabelLightSensorViewModel { get; set; }


        public RaspberryViewModel()
        {
            ButtonHellpViewModel = new ButtonViewModel(true, "Say Hello ", Color.IndianRed);

            LabelResponseViewModel = new LabelViewModel(true, "Keine Daten empfangen", Color.BlueViolet);

            LabelLightSensorViewModel = new LabelViewModel(true, "Bisher keine Daten empfangen", Color.BlueViolet);

            Title = "Raspberry Server";
        }
    }
}