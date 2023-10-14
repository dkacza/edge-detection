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

namespace EdgeDetection.View.Actions
{
    /// <summary>
    /// Interaction logic for Actions.xaml
    /// </summary>
    public partial class Actions : UserControl
    {
        public Actions()
        {
            InitializeComponent();
        }

        private void measureBtn_Click(object sender, RoutedEventArgs e)
        {
            progressBar.IsIndeterminate = true;
            App.runMeasurements();
            progressBar.IsIndeterminate = false;
        }

        private void convertBtn_Click(object sender, RoutedEventArgs e)
        {
            progressBar.IsIndeterminate = true;
            App.runConversion();
            progressBar.IsIndeterminate = false;
        }
    }
}
