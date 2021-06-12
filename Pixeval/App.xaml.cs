using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Pixeval
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }
        public new int Run()
        {
            return Run(MainWindow);
        }
    }
}
