using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using System.Windows.Media;
using System.Globalization;
using System.IO;

namespace MyExpenseManager
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            XDocument file = XDocument.Load(@"ExpensesFile.xml");
            XElement BGcolor = file.Root.Element("background").Element("color");
            ColorBox.Text = BGcolor.Attribute("hex").Value;
            LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(Convert.ToByte(BGcolor.Attribute("a").Value),Convert.ToByte(BGcolor.Attribute("r").Value),Convert.ToByte(BGcolor.Attribute("g").Value),Convert.ToByte(BGcolor.Attribute("b").Value)));
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            XDocument file = XDocument.Load(@"ExpensesFile.xml");
            XElement BGcolor = file.Root.Element("background").Element("color");
            BGcolor.Attribute("hex").Value = "#FF000000";
            BGcolor.Attribute("a").Value = "0";
            BGcolor.Attribute("r").Value = "0";
            BGcolor.Attribute("g").Value = "0";
            BGcolor.Attribute("b").Value = "0";
            XElement Categories = file.Root.Element("categories");
            Categories.RemoveNodes();
            XElement Items = file.Root.Element("items");
            Items.RemoveNodes();
            XElement Wishes = file.Root.Element("wishlist");
            Wishes.RemoveNodes();
            file.Root.Element("balance").Element("bank").Attribute("balance").Value = "0";
            file.Root.Element("balance").Element("cash").Attribute("inhand").Value = "0";
            using (FileStream stream = File.Open(@"ExpensesFile.xml", FileMode.Truncate, FileAccess.ReadWrite))
            {
                file.Save(stream);
            }
        }
    }
}