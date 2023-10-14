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

namespace EdgeDetection.View.Options.CoresSelection
{
    /// <summary>
    /// Interaction logic for CoresSelection.xaml
    /// </summary>
    public partial class CoresSelection : UserControl
    {
        public CoresSelection()
        {
            InitializeComponent();
        }

        private void coresSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int newVal = (int)Math.Round(coresSlider.Value);
            App.SetCores(newVal);
        }
    }
}
