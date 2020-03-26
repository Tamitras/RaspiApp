
using System.Drawing;

namespace XamarinApp1.ViewModels.ButtonViewModels
{
    public class ButtonViewModel : ButtonViewModelBase
    {
        public ButtonViewModel(bool enabled, string text, Color backGroundColor)
        {
            this.Enabled = enabled;
            this.Text = text;
            this.BackgroundColor = backGroundColor;
        }
    }
}
