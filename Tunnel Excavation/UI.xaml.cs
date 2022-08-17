using Microsoft.Win32;
using System;
using System.IO;
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
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace Tunnel_Excavation
{
    /// <summary>
    /// Interaction logic for UI.xaml
    /// </summary>
    public partial class UI : Window
    {
        public UIDocument Uidoc { get; }
        public Document Doc { get; }

        public string familyTemplateName = "";
        public string familyName = "";
        public UI(UIDocument UiDoc)
        {
            Uidoc = UiDoc;
            Doc = UiDoc.Document;
            InitializeComponent();
            Title = "Create Tunnel";
        }

        private void CreateTunnel_Click(object sender, RoutedEventArgs e)
        {
            // get all inputs from UI WPF
            // inpputs of tunnel geometry
            float span = Convert.ToSingle(String.Format("{0:0.00}", this.input_span.Text));
            float degree = Convert.ToSingle(String.Format("{0:0.00}", this.input_degree.Text));
            float ratio = Convert.ToSingle(String.Format("{0:0.00}", this.input_ratio.Text));

            // input of the fmaily type
            string familyTemplateType =((ComboBoxItem) this.input_familyType.SelectedItem).Content.ToString();
        }

        private void SaveFamily_Click(object sender, RoutedEventArgs e) 
        {
            SaveFileDialog saveFamilyDialog = new SaveFileDialog();
            saveFamilyDialog.InitialDirectory = @"c:\temp\";
            saveFamilyDialog.AddExtension = true;
            saveFamilyDialog.DefaultExt = "rfa";
            saveFamilyDialog.Filter = "Comma Separated(*.rfa)| *.* ";

            if (saveFamilyDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFamilyDialog.FileName, this.input_fileDirectory.Text);
            }
        }

        private void Input_familyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string familyType = ((ComboBoxItem)input_familyType.SelectedItem).Content.ToString();

            if (familyType == "Metric Template")
            {
                familyTemplateName = @"English\Metric Generic Model.rft";
            }
            else 
            {
                familyTemplateName = @"English-Imperial\Generic Model.rft";
            }
        }
        private void Input_fileDirectory_TextChanged(object sender, TextChangedEventArgs e)
        {
            familyName = input_fileDirectory.Text;
        }
        private void SelectRail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
