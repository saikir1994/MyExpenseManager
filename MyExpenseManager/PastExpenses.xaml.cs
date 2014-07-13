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

namespace MyExpenseManager
{
    public partial class PastExpenses : PhoneApplicationPage
    {
        public PastExpenses()
        {
            InitializeComponent();
            From.Value = DateTime.Today;
            To.Value = DateTime.Today;
            LoadHist();
        }

        private void LoadHist()
        {
            HistList.Children.Clear();
            XDocument file = XDocument.Load(@"ExpensesFile.xml");
            List<XElement> Items = file.Root.Element("items").Elements().ToList<XElement>();
            foreach (var item in Items)
            {
                DateTime.Compare(Convert.ToDateTime(item.Attribute("date").Value), (DateTime)From.Value);
                DateTime.Compare(Convert.ToDateTime(item.Attribute("date").Value), (DateTime)To.Value);
            }
            var reqitems = from XElement item in Items
                           where DateTime.Compare(Convert.ToDateTime(item.Attribute("date").Value), (DateTime)From.Value) >= 0 && DateTime.Compare(Convert.ToDateTime(item.Attribute("date").Value), (DateTime)To.Value) <= 0
                           select item;
            foreach (var item in reqitems)
            {
                ExpanderView Item = new ExpanderView();
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

        private void Time_SelectionChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            LoadHist();
        }
        

    }
}