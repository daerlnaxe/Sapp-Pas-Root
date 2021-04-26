using DxTBoxCore.BoxChoose;
using DxTBoxCore.Languages;
using SPR.Models;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace SPR.Graph
{
    /// <summary>
    /// Logique d'interaction pour P_LoadFolder.xaml
    /// </summary>
    public partial class P_LoadFolder : Page
    { 

        internal AChooseFolder Model { get; set; }

        public P_LoadFolder()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DataContext = Model;
        }

        private void Browse_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Browse_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TreeChoose cf = new TreeChoose()
            {
                SaveButtonName = DxTBLang.Select,
                CancelButtonName = DxTBLang.Cancel,
                Model = new M_ChooseFolder()
                {
                    HideWindowsFolder = true,
                    PathCompareason = System.StringComparison.CurrentCultureIgnoreCase,
                    StartingFolder = Model.StartingFolder
                }
            };

            cf.ShowDialog();

            if (cf.DialogResult == true)
            {
                Model.Browse_Executed(cf.Model.LinkResult);
            }

        }

   
    }
}
