using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using POS.Services;
using POS.Views;

namespace POS.Shells
{
    public sealed partial class Shell_Owner : Window, INavigation
    {
        private bool _navigatedFlag = false;
        public Shell_Owner()
        {
            this.InitializeComponent();
            //this.SizeChanged += Shell_SizeChanged;
        }
        //Navigate from top to left for NavigationView
        //private void Shell_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        //    {
        //        if (e.Size.Width > 1100)
        //        {
        //            NavigationView.PaneDisplayMode = NavigationViewPaneDisplayMode.Top;
        //        }
        //        else
        //        {
        //            NavigationView.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;
        //        }
        //    }


        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            // Navigates, but does not update the Menu.
            // ContentFrame.Navigate(typeof(HomePage));

            SetCurrentNavigationViewItem(GetNavigationViewItems().FirstOrDefault());
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (_navigatedFlag == false)
            {
                SetCurrentNavigationViewItem(args.SelectedItemContainer as NavigationViewItem);
            }
        }

        public List<NavigationViewItem> GetNavigationViewItems()
        {
            var result = new List<NavigationViewItem>();
            var items = NavigationView.MenuItems.Select(i => (NavigationViewItem)i).ToList();
            items.AddRange(NavigationView.FooterMenuItems.Select(i => (NavigationViewItem)i));
            result.AddRange(items);

            foreach (NavigationViewItem mainItem in items)
            {
                result.AddRange(mainItem.MenuItems.Select(i => (NavigationViewItem)i));
            }

            return result;
        }

        public List<NavigationViewItem> GetNavigationViewItems(Type type)
        {
            return GetNavigationViewItems().Where(i => i.Tag.ToString() == type.FullName).ToList();
        }

        public List<NavigationViewItem> GetNavigationViewItems(Type type, string title)
        {
            return GetNavigationViewItems(type).Where(ni => ni.Content.ToString() == title).ToList();
        }

        public void SetCurrentNavigationViewItem(NavigationViewItem item)
        {
            if (item == null)
            {
                return;
            }

            if (item.Tag == null)
            {
                return;
            }

            ContentFrame.Navigate(Type.GetType(item.Tag.ToString()), item.Content);
            //NavigationView.Header = item.Content;
            NavigationView.SelectedItem = item;
        }

        public NavigationViewItem GetCurrentNavigationViewItem()
        {
            return NavigationView.SelectedItem as NavigationViewItem;
        }

        public void SetCurrentPage(Type type)
        {
            ContentFrame.Navigate(type);
        }

        public void SetCurrentNavigationViewItemForMenuWithArgument(object arg)
        {
            var menuItem = GetNavigationViewItems(typeof(Menu)).First();
            if (menuItem == null)
            {
                return;
            }

            if (menuItem.Tag == null)
            {
                return;
            }

            ContentFrame.Navigate(Type.GetType(menuItem.Tag.ToString()), arg);
            _navigatedFlag = true;
            //NavigationView.Header = item.Content;
            NavigationView.SelectedItem = menuItem;
            _navigatedFlag = false;
        }
    }
}
