﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Command;

namespace ETest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ExternalEvent ee = null;
        private ExternalCommand cmd = null;
        public MainWindow()
        {
            InitializeComponent();
            if (cmd == null)
            {
                cmd = new ExternalCommand();
            }
            if (ee == null)
            {
                ee = ExternalEvent.Create(cmd);
            }
        }

        private void Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ee.Raise();
        }
    }
}
