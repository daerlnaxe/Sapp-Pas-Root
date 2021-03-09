using DxTBoxCore.BoxChoose;
using DxTBoxCore.Languages;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace SPR.Graph
{
    /// <summary>
    /// Logique d'interaction pour P_LoadFolder.xaml
    /// </summary>
    public partial class P_LoadFolder : Page, INotifyPropertyChanged
    {
        public delegate void StringValueChanged(string resultFolder);
        public event StringValueChanged ResultFolderChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Message à afficher en haut pour décrire l'action
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Folder to start exploration
        /// </summary>
        public string StartingFolder { get; set; }

        /*
        /// <summary>
        /// Return FolderChosen
        /// </summary>
        //public string ResultFolder { get; private set; }*/

        private string _resultFolder;
        /// <summary>
        /// Dossier choisi
        /// </summary>
        public string ResultFolder
        {
            get { return _resultFolder; }
            set
            {
                _resultFolder = value;
                ResultFolderChanged?.Invoke(_resultFolder);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResultFolder"));
            }
        }

        public P_LoadFolder()
        {
            InitializeComponent();
            DataContext = this;
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
                    StartingFolder = Properties.Settings.Default.LastKPath

                }
            };

            cf.ShowDialog();

            if (cf.DialogResult == true)
            {
                ResultFolder = cf.Model.LinkResult;
                Properties.Settings.Default.LastKPath = ResultFolder;
                Properties.Settings.Default.Save();
            }
            /*
            if (cf.DialogResult == true && e.Parameter.Equals("Source"))
            {
                _Model.Source = cf.LinkResult;
            }
            else if (cf.DialogResult == true && e.Parameter.Equals("Destination"))
            {
                _Model.Destination = cf.LinkResult;
            }*/
        }
    }
}
