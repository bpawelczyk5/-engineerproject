using CerberPass.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using System.ComponentModel;


namespace CerberPass
{
    public partial class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // Ustawienie początkowego UserControl do wyświetlenia
            MainContent.Content = new Home();
            
        }

        private void NavigationViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is NavigationViewItem navigationViewItem)
            {
                string selectedTag = navigationViewItem.Tag as string;
                switch (selectedTag)
                {
                    case "Home":
                        MainContent.Content = new Home();
                        break;
                    case "Pass":
                        MainContent.Content = new Passwords();
                        break;
                    case "Vault":
                        MainContent.Content = new Vault();
                        break;
                    case "PassGen":
                        MainContent.Content = new PassGen();
                        break;
                    default:
                        break;
                }

                // Mark the event as handled so that it doesn't propagate further
                e.Handled = true;
            }
        }

    }
}
