using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace XamarinApp1.ViewModels.LabelViewModels
{
    public class LabelViewModel : BaseViewModel
    {
        public LabelViewModel(bool enabled, string text, Color backGroundColor)
        {
            this.Enabled = enabled;
            this.Text = text;
            this.BackgroundColor = backGroundColor;
        }
    }
}
