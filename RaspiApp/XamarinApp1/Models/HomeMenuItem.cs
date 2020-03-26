using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApp1.Models
{
    public enum MenuItemType
    {
        Browse,
        About,
        RaspberryPi
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
