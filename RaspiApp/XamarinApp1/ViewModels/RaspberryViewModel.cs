using System.Drawing;
using XamarinApp1.ViewModels.ButtonViewModels;
using XamarinApp1.ViewModels.LabelViewModels;
using XamarinApp1.ViewModels.ToolbarViewModels;

namespace XamarinApp1.ViewModels
{
    public class RaspberryViewModel : BaseViewModel
    {
        public ButtonViewModel ButtonHelloViewModel { get; set; }
        public ButtonViewModel ButtonToggleLedViewModel { get; set; }
        public LabelViewModel LabelResponseViewModel { get; set; }
        public LabelViewModel LabelLightSensorViewModel { get; set; }

        public BindableToolbarItem BindableToolbarItem { get; set; }

        public RaspberryViewModel()
        {
            this.ButtonHelloViewModel = new ButtonViewModel(true, "Say Hello ", Color.IndianRed);
            this.ButtonToggleLedViewModel = new ButtonViewModel(true, "LED aus", Color.Gray);
            this.LabelResponseViewModel = new LabelViewModel(true, "Keine Daten empfangen", Color.BlueViolet);
            this.LabelLightSensorViewModel = new LabelViewModel(true, "Bisher keine Daten empfangen", Color.BlueViolet);

            this.Title = "Raspberry Server";

            this.BindableToolbarItem = new BindableToolbarItem()
            {
                IsVisible = false,
                IconImageSource = "raspian.png",
                Order = Xamarin.Forms.ToolbarItemOrder.Primary,
                Priority = 1,
                Text = "online"
            };

        }
    }
}