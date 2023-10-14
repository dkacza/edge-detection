using EdgeDetection.Implementations;
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

namespace EdgeDetection.View.Options.ImplementationSelection
{
    /// <summary>
    /// Interaction logic for ImplementationSelection.xaml
    /// </summary>
    public partial class ImplementationSelection : UserControl
    {
        public ImplementationSelection()
        {
            InitializeComponent();
        }

        private void csharpImplementation_Checked(object sender, RoutedEventArgs e)
        {
            CSharpImplementation cSharpImplementation = new CSharpImplementation();
            App.SetImplementation(cSharpImplementation);
        }

        private void cpp_implementation_Checked(object sender, RoutedEventArgs e)
        {
            CppImplementation cppImplementation = new CppImplementation();
            App.SetImplementation(cppImplementation);
        }

        private void asm_implementation_Checked(object sender, RoutedEventArgs e)
        {
            AsmImplementation asmImplementation = new AsmImplementation();
            App.SetImplementation(asmImplementation);
        }
    }
}
