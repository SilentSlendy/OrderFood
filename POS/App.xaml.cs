using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using POS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using POS.Services;
using POS.Shells;
using POS.Helpers;
using POS.Models;
using POS.Login;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using POS.Views;

namespace POS
{

    public partial class App : Application
    {
        public byte[] aesKey { get; set; } 
        public App()
        {
            this.InitializeComponent();
            aesKey = new byte[32]
        {
            0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
            0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
            0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
            0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF
        };  // 32 bytes
            //AccountCreator.GenarateBase64AccountData("tien", "tien");
        }
        public EmployeeDataForLogin CurrentEmployee { get; set; }
        public PaymentViewModel PaymentViewModel { get; set; } = new PaymentViewModel();
        public INavigation navigate { get; set; }
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {


            m_window2 = new ShellWindow();
            m_window2.Activate();
            // Set the title for the app
            m_window2.Title = "POS HCMUS";

            //m_window = new Shell();
            //navigate = m_window;
            //m_window.Activate();

        }

        //protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        //{
        //    m_window = new MainWindow();
        //    var root = new Frame();
        //    m_window.Content = root;
        //    var name = "POS.Views.Menu2";
        //    var type = Type.GetType(name);
        //    m_window.Activate();
        //    root.Navigate(type);

           

            //m_window = new LoginWindow();
            //Frame rootFrame = new Frame();
            //rootFrame.NavigationFailed += OnNavigationFailed;
            //// Navigate to the first page, configuring the new page
            //// by passing required information as a navigation parameter
            //rootFrame.Navigate(typeof(MainPage), args.Arguments);

            //// Place the frame in the current Window
            //m_window.Content = rootFrame;
            //m_window.Activate();
        //}

        //void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        //{
        //    throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        //}

        internal Shell m_window { get; set; }
        internal ShellWindow m_window2 { get; set; }
    }
}
