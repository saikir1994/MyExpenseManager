using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyExpenseManager.Resources;
using Windows.Storage;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Windows.Media;

namespace MyExpenseManager
{
    public partial class MainPage : PhoneApplicationPage
    {
        StorageFolder local = ApplicationData.Current.LocalFolder;
        //string filename = "ExpensesFile.xml";
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            XDocument file = XDocument.Load(@"ExpensesFile.xml");
            XElement BGcolor = file.Root.Element("background").Element("color");
            LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(Convert.ToByte(BGcolor.Attribute("a").Value), Convert.ToByte(BGcolor.Attribute("r").Value), Convert.ToByte(BGcolor.Attribute("g").Value), Convert.ToByte(BGcolor.Attribute("b").Value)));
        
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has opted in or out of Location
                return;
            }
            else
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        
    }
}