﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using System.Xml;
using Windows.Storage;
using System.IO;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.Device;
using System.Device.Location;
using Microsoft.Phone.Maps.Services;
using System.Windows.Media;
namespace MyExpenseManager
{
    public partial class ManageExpenses : PhoneApplicationPage
    {
        public ManageExpenses()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {

            XDocument file = XDocument.Load(@"ExpensesFile.xml");
            List<XElement> Category = file.Root.Element("categories").Elements().ToList<XElement>();
            List<string> CategoryList = new List<string>();
            foreach (XElement category in Category)
            {
                CategoryList.Add(category.Attribute("name").Value);
            }
            CategoriesList.ItemsSource = CategoryList;

        }
        private async void GetLocation_Click(object sender, RoutedEventArgs ev)
        {

            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                return;
            }

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );
                string address = "";
                ReverseGeocodeQuery query = new ReverseGeocodeQuery();
                query.GeoCoordinate = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);
                query.QueryCompleted += (s, e) =>
                {
                    if (e.Error != null)
                        return;
                    if (e.Result.Count > 0)
                        address = e.Result[0].Information.Address.District + " ," + e.Result[0].Information.Address.City;
                    LocationBox.Text = address;
                };
                query.QueryAsync();
            }
            catch
            {
                LocationBox.Text = "Enter Loaction";
            }
        }
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            var tempCat = CategoriesList.Text;
            CategoriesList.Text = tempCat.Substring(0, 1).ToUpper() + tempCat.Substring(1).ToLower();
            if (!CategoriesList.ItemsSource.Cast<string>().Contains(CategoriesList.Text) && CategoriesList.Text != string.Empty)
            {
                XDocument file = XDocument.Load(@"ExpensesFile.xml");
                XElement Categories = file.Root.Element("categories");
                Categories.Add(new XElement("category", new XAttribute("name", CategoriesList.Text)));
                using (FileStream stream = File.Open(@"ExpensesFile.xml", FileMode.Truncate, FileAccess.ReadWrite))
                {
                    file.Save(stream);
                }
                LoadCategories();
            }
            if (CategoriesList.Text != string.Empty && ItemBox.Text != string.Empty)
            {
                XDocument file = XDocument.Load(@"ExpensesFile.xml");
                XElement Items = file.Root.Element("items");
                Items.Add(new XElement("item", new XAttribute("name", ItemBox.Text), new XAttribute("category", CategoriesList.Text), new XAttribute("date", DateSelector.ValueString), new XAttribute("cost", CostBox.Text), new XAttribute("place", LocationBox.Text)));
                using (FileStream stream = File.Open(@"ExpensesFile.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    file.Save(stream);
                }
            }
            MessageBox.Show("Item added successfully");
            ItemBox.Text = "";
            CategoriesList.Text = "";
            CostBox.Text = "";
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            XDocument file = XDocument.Load(@"ExpensesFile.xml");
            XElement BGcolor = file.Root.Element("background").Element("color");
            LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(Convert.ToByte(BGcolor.Attribute("a").Value), Convert.ToByte(BGcolor.Attribute("r").Value), Convert.ToByte(BGcolor.Attribute("g").Value), Convert.ToByte(BGcolor.Attribute("b").Value)));
        }
    }
}