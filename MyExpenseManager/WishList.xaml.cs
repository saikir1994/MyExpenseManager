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
using System.Windows.Media;

namespace MyExpenseManager
{
    public partial class WishList : PhoneApplicationPage
    {
        public WishList()
        {
            InitializeComponent();
            LoadWish();
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
        private void LoadWish()
        {
            WiList.Children.Clear();
            XDocument file = XDocument.Load(@"ExpensesFile.xml");
            List<XElement> Wishes = file.Root.Element("wishlist").Elements().ToList<XElement>();
            
            foreach (var wish in Wishes)
            {
                ExpanderView Item = new ExpanderView();
                Item.Header = "Name: " + wish.Attribute("name").Value;
                Item.FontSize = 30;
                Item.Hold += Item_Hold;
                Item.Items.Add(new TextBlock()
                {
                    Text = "Category: " + wish.Attribute("category").Value                        
                        + "\nCost: " + wish.Attribute("cost").Value
                });
                WiList.Children.Add(Item);
            }
        }

        void Item_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show("Do you want to delete the current Wish?", "Select OK or Cancel", MessageBoxButton.OKCancel);
            if (Result == MessageBoxResult.OK)
            {
                ExpanderView Selected = (ExpanderView)sender;
                XDocument file = XDocument.Load(@"ExpensesFile.xml");
                List<XElement> Wishes = file.Root.Element("wishlist").Elements().ToList<XElement>();
                foreach (XElement wish in Wishes)
                {
                    if ("Name: " + wish.Attribute("name").Value== Selected.Header.ToString())
                    {
                        var wishset = file.Root.Element("wishlist").Elements();
                        var delwish = wishset.FirstOrDefault(x => "Name: " + x.Attribute("name").Value == Selected.Header.ToString());
                        delwish.Remove();
                        break;
                    }
                }
                using (FileStream stream = File.Open(@"ExpensesFile.xml", FileMode.Truncate, FileAccess.ReadWrite))
                {
                    file.Save(stream);
                }
                LoadWish();
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
                using (FileStream stream = File.Open(@"ExpensesFile.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    file.Save(stream);
                }
                LoadCategories();
            }
            if (CategoriesList.Text != string.Empty && ItemBox.Text != string.Empty)
            {
                XDocument file = XDocument.Load(@"ExpensesFile.xml");
                XElement Wishes = file.Root.Element("wishlist");
                Wishes.Add(new XElement("wish", new XAttribute("name", ItemBox.Text), new XAttribute("category", CategoriesList.Text), new XAttribute("cost", CostBox.Text)));
                using (FileStream stream = File.Open(@"ExpensesFile.xml", FileMode.Truncate, FileAccess.ReadWrite))
                {
                    file.Save(stream);
                }
            }
            MessageBox.Show("Item added to WishList successfully");
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