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
using System.IO;

namespace MyExpenseManager
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            XDocument file = XDocument.Load(@"ExpensesFile.xml");
            XElement BGcolor = file.Root.Element("background").Element("color");

            BGcolor.Attribute("a").Value = ColorPicker.Color.A.ToString();
            BGcolor.Attribute("r").Value = ColorPicker.Color.R.ToString();
            BGcolor.Attribute("b").Value = ColorPicker.Color.B.ToString();
            BGcolor.Attribute("g").Value = ColorPicker.Color.G.ToString();
            BGcolor.Attribute("hex").Value = ColorPicker.Color.ToString();
            using (FileStream stream = File.Open(@"ExpensesFile.xml", FileMode.Truncate, FileAccess.ReadWrite))
            {
                file.Save(stream);
            }
            NavigationService.GoBack();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}