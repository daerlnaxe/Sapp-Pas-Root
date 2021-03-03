using DxTBoxCore.Box_Progress;
using DxTBoxCore.Languages;
using DxTBoxCore.MBox;
using SPR.Languages;
using SPR.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Graph
{
    /// <summary>
    /// Logique d'interaction pour P_Migrate.xaml
    /// </summary>
    public partial class P_Migrate : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MigrateModel _Model;

        #region Pages
        /// <summary>
        /// Page pour charger la source
        /// </summary>
        P_LoadFolder srcPageLoadFolder;

        /// <summary>
        /// Page pour charger le dossier de destination
        /// </summary>
        private readonly P_LoadFolder destPageLoadFolder;

        /// <summary>
        /// Page pour modifier les sous-dossiers pour la source
        /// </summary>
        private P_SubFolders srcSubFolders;

        /// <summary>
        /// Page pour modifier les sous-dossiers pour la destination
        /// </summary>
        private P_SubFolders destSubFolders;


        private Page _activePage;

        /// <summary>
        /// Page visible dans la frame 1
        /// </summary>
        public Page ActivePage
        {
            get { return _activePage; }
            set
            {
                if (value == null)
                    TopHeight = 400;
                else
                    TopHeight = 200;


                _activePage = value;
                OnPropertyChanged();
            }
        }
        #endregion Pages

        #region Commandes 
        public static readonly RoutedCommand Preview = new RoutedCommand("Preview", typeof(W_PlateformsList));
        public static readonly RoutedCommand Open_ConfSrcPath = new RoutedCommand("Open_ConfSrcPath", typeof(W_PlateformsList));
        public static readonly RoutedCommand Open_ConfDestPath = new RoutedCommand("Open_ConfDestPath", typeof(W_PlateformsList));
        public static readonly RoutedCommand Open_ConfSrcSub = new RoutedCommand("Open_ConfSrcSub", typeof(W_PlateformsList));
        public static readonly RoutedCommand Open_ConfDestSub = new RoutedCommand("Open_ConfDestSub", typeof(W_PlateformsList));


        #endregion Commandes 

        #region Panel Size
        private ushort _topHeight = 400;
        /// <summary>
        /// Utilisé pour la taille du panneau de la prévisualisation
        /// </summary>
        public ushort TopHeight
        {
            get { return _topHeight; }
            set
            {
                _topHeight = value;
                OnPropertyChanged();
            }
        }


        #endregion


        public P_Migrate()
        {

            // Configuration des pages

            _Model = new MigrateModel(this);
            _Model.Initialize();


            InitializeComponent();
            DataContext = _Model;



            // Chemin source
            srcPageLoadFolder = new P_LoadFolder()
            {
                Info = SPRLang.Choose_Source_Folder,
                StartingFolder = System.IO.Path.Combine(Global.LaunchBoxRoot, Properties.Settings.Default.AppsFolder)

            };
            srcPageLoadFolder.ResultFolderChanged += ((x) => _Model.Source = x);

            // Chemin destination
            destPageLoadFolder = new P_LoadFolder()
            {
                Info = SPRLang.Choose_Dest_Folder,
                StartingFolder = System.IO.Path.Combine(Global.LaunchBoxRoot, Properties.Settings.Default.AppsFolder)

            };
            destPageLoadFolder.ResultFolderChanged += ((x) => _Model.Destination = x);

            // Subfolders Src
            srcSubFolders = new P_SubFolders();
            srcSubFolders.Model.Info = SPRLang.Source_Sub;
            srcSubFolders.Model.ToolTipInfo = SPRLang.TT_Sub;
            _Model.SrcSub = srcSubFolders.Model;

            // Subfoldes Dest
            destSubFolders = new P_SubFolders();
            destSubFolders.Model.Info = SPRLang.Dest_Sub;
            destSubFolders.Model.ToolTipInfo = SPRLang.TT_Sub;
            _Model.DestSub = destSubFolders.Model;
        }

        /// <summary>
        /// Vide la frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetPage_Null(object sender, ExecutedRoutedEventArgs e)
        {
            ActivePage = null;
        }

        /// <summary>
        /// Charge la page pour configurer le chemin source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void Load_ConfigSrc(object sender, ExecutedRoutedEventArgs e)
        {
            ActivePage = srcPageLoadFolder;
        }

        /// <summary>
        /// Charge la page pour configurer le chemin source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void Load_ConfigDest(object sender, ExecutedRoutedEventArgs e)
        {
            ActivePage = destPageLoadFolder;
        }

        #region Config Source Sub Folders
        private void Can_ShowSrcSub(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(_Model.Source);
        }


        private void Load_ConfigSrcSub(object sender, ExecutedRoutedEventArgs e)
        {
            ActivePage = srcSubFolders;
        }

        #endregion

        #region Config Destination Sub Folders

        private void Can_ShowDestSub(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(_Model.Destination);
        }

        private void Load_ConfigDestSub(object sender, ExecutedRoutedEventArgs e)
        {

            ActivePage = destSubFolders;
        }

        #endregion

        #region simulate
        private void Can_Simulate(object sender, CanExecuteRoutedEventArgs e)
        {
            /* Pour que la simulation puisse débuter:
                - il ne doit pas y avoir d'erreurs dans les pages
                - Destination et source ne doivent pas être null
                

             */

            bool HasNoError = true;
            HasNoError &= !string.IsNullOrEmpty(_Model.Source);
            HasNoError &= !string.IsNullOrEmpty(_Model.Destination);

            if (srcSubFolders == null)
                HasNoError &= false;
            else
                HasNoError &= !srcSubFolders.Model.HasErrors;

            if (destSubFolders == null)
                HasNoError &= false;
            else
                HasNoError &= !destSubFolders.Model.HasErrors;

            e.CanExecute = HasNoError;
            /*
            e.CanExecute = !(string.IsNullOrEmpty(_Model.Source)
                                && srcSubFolders !=null && srcSubFolders.Model.HasErrors
                                && destPageSubFolders != null && destPageSubFolders.Model.HasErrors);*/
        }

        // Note sur cette portion on va surtout plus vérifier un peu tout, tel que l'existence des répertoires
        private void Exec_Simulate(object sender, ExecutedRoutedEventArgs e)
        {
            ActivePage = null;
            if (_Model.Verifications())
            {
                  DxMBox.ShowDial(SPRLang.SimuSuccess);
                ActiveApply = true;
            }
            else
            {
                DxTBoxCore.MBox.DxMBox.ShowDial(SPRLang.SimuFail, title: DxTBLang.Warning);
                //optMessage: chemin
                ActiveApply = false;
            }
        }
        #endregion

        private void Exec_Apply(object sender, ExecutedRoutedEventArgs e)
        {
            if (_Model.Apply() == true)
            {
                    DxMBox.ShowDial(SPRLang.Transfert_Success);
            }

            else
            {
                  DxMBox.ShowDial(SPRLang.Transfert_Fail);
            }

            // Todo afficher un relevé d'informations
            /*
                Relevé d'informations
                    - X fichiers passés car similaires
                    - X fichiers écrasés
                    - X fichiers transférés sans collision
                    - X fichiers problèmes
            */

        }

        private bool ActiveApply;
        private void Can_Apply(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ActiveApply;
        }
    }
}
