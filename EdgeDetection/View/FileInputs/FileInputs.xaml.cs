using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;

namespace EdgeDetection.View.FileInputs
{
    /// <summary>
    /// Interaction logic for FileInputs.xaml
    /// </summary>
    public partial class FileInputs : UserControl
    {
        public FileInputs()
        {
            InitializeComponent();
        }

        private void inputSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "JPG image | *.jpg";
            bool? success = fileDialog.ShowDialog();
            if (success == true)
            {
                string path = fileDialog.FileName;
                inputPath.Text = path;
                App.SetInputPath(path);
            }
        }

        private void outputSelect_Click(object sender, RoutedEventArgs e)
        {
            WinForms.FolderBrowserDialog dialog = new WinForms.FolderBrowserDialog();
            WinForms.DialogResult result = dialog.ShowDialog();
            if (result == WinForms.DialogResult.OK)
            {
                string folder = dialog.SelectedPath;
                outputPath.Text = folder;
                App.SetOutputPath(folder);
            }
        }
    }
}
