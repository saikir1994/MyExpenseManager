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
using System.IO;

namespace MyExpenseManager
{
    public partial class PastExpenses : PhoneApplicationPage
    {
        public PastExpenses()
        {
            InitializeComponent();
            From.Value = DateTime.Today.AddMonths(-1);
            To.Value = DateTime.Today;
            LoadHist();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            XDocument file = XDocument.Load(@"ExpensesFile.xml");
            XElement BGcolor = file.Root.Element("background").Element("color");
            LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(Convert.ToByte(BGcolor.Attribute("a").Value), Convert.ToByte(BGcolor.Attribute("r").Value), Convert.ToByte(BGcolor.Attribute("g").Value), Convert.ToByte(BGcolor.Attribute("b").Value)));
        }
        private void LoadHist()
        {
            HistList.Children.Clear();
            XDocument file = XDocument.Load(@"ExpensesFile.xml");
            List<XElement> Items = file.Root.Element("items").Elements().ToList<XElement>();
            var reqitems = from XElement item in Items
                           where DateTime.Compare(Convert.ToDateTime(item.Attribute("date").Value), (DateTime)From.Value) >= 0 && DateTime.Compare(Convert.ToDateTime(item.Attribute("date").Value), (DateTime)To.Value) <= 0
                           select item;
            foreach (var item in reqitems)
            {
                ExpanderView Item = new ExpanderView();
                Item.Hold += Delete_Hold;
                Item.Header = "Name: " + item.Attribute("name").Value + "  " + "Cost: " + item.Attribute("cost").Value;
                Item.FontSize = 24;
                Item.Items.Add(new TextBlock()
                {
                    Text = "Date: " + item.Attribute("date").Value
                        + "\nCategory: " + item.Attribute("category").Value
                        + "\nLocation: " + item.Attribute("place").Value
                });
                HistList.Children.Add(Item);
            }
        }

        void Delete_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show("Do you want to delete the current expense?", "Select OK or Cancel", MessageBoxButton.OKCancel);
            if(Result == MessageBoxResult.OK)
            {
                ExpanderView Selected = (ExpanderView)sender;
                XDocument file = XDocument.Load(@"ExpensesFile.xml");
                List<XElement> Items = file.Root.Element("items").Elements().ToList<XElement>();
                foreach (XElement item in Items)
                {
                    if("Name: " + item.Attribute("name").Value + "  " + "Cost: " + item.Attribute("cost").Value == Selected.Header.ToString())
                    {
                        var itemset = file.Root.Element("items").Elements();
                        var delitem = itemset.FirstOrDefault(x => "Name: " + x.Attribute("name").Value + "  " + "Cost: " + x.Attribute("cost").Value == Selected.Header.ToString());
                        delitem.Remove();
                        break;
                    }
                }
                using (FileStream stream = File.Open(@"ExpensesFile.xml", FileMode.Truncate, FileAccess.ReadWrite))
                {
                    file.Save(stream);
                }
                LoadHist();
            }
        }


        private void Time_SelectionChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            LoadHist();
        }
        

    }
}