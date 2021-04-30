using DxTBoxCore.Box_MBox;
using DxTBoxCore.Common;
using DxTBoxCore.Languages;
using Hermes;
using SPR.Graph.Commands;
using SPR.Languages;
using SPR.Models;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Graph
{
    /*
     * Locked migrate and migrate sont différents
     *  - le premier se fait entre les dossiers d'une veille plateforme et d'une nouvelle, automatiquement.
     *  - le second se fait sur manipulations par l'utilisateur.
     */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class W_PlateformsList : Window
    {


        #region commandes
        /// <summary>
        /// Edition des chemins d'une plateforme
        /// </summary>
        public static readonly RoutedCommand EditPlatRC = new RoutedCommand("Edit_Platform", typeof(W_PlateformsList));
        /// <summary>
        /// Edition des chemins des jeux
        /// </summary>
        public static readonly RoutedCommand EditGames = new RoutedCommand("Edit_Games", typeof(W_PlateformsList));

        /// <summary>
        /// Migration de fichiers entre les anciens dossiers d'une plateforme et les nouveaux
        /// </summary>
        public static readonly RoutedCommand LockedMigrateCommand = new RoutedCommand("Migrate", typeof(W_PlateformsList));
        /// <summary>
        /// Migration de fichiers entre deux dossiers
        /// </summary>
        public static readonly RoutedCommand MigrateCommand = new RoutedCommand("Migrate", typeof(W_PlateformsList));
        #endregion

        public string Version
        {
            get;
        }


        PlateformsListModel _Model;


        public W_PlateformsList()
        {
            // fr-FR pour français
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;

            Assembly asmbly = Assembly.GetExecutingAssembly();


            _Model = new PlateformsListModel();
            InitializeComponent();

            this.DataContext = _Model;

            Version = $"Version: {asmbly.GetName().Version}";
            this.Title = asmbly.GetName().Name.ToString();

            _Model.Initialize();

        }

        private void ListPlatform()
        {

        }
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(e.OriginalSource.ToString());
            this.Hide();
            W_PlatformPaths wpp = new W_PlatformPaths()
            {
                Model = new PlatformModel(),
            };
            wpp.ShowDialog();
            this.Show();
        }*/

        /*
        #region Edit platform
        /// <summary>
        /// Left double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //e.source peut nous donner la listview
            // Le selected item fonctionne bien
            if (e.ChangedButton.Equals(MouseButton.Left))
            {
                Edit_Platforme(_Model.SelectedPlatform);
            }

        }*/
        /*
        private void ModifyPlatform_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Edit_Platforme(_Model.SelectedPlatform);
        }*/
        /*
        /// <summary>
        /// Show form to edit a platform
        /// </summary>
        /// <param name="platform"></param>
        private void Edit_Platforme(IPlatform platform)
        {
            if (_Model.SelectedPlatform == null)
                return;

            this.Hide();

            W_PlatformPaths wp = new W_PlatformPaths()
            {
                Model = new PlatformModel(),
            };
            wp.ShowDialog();

            this.Show();
        }
        #endregion
        */



        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_Model.SelectedPlatform == null)
                e.CanExecute = false;
        }

        /// <summary>
        /// Vérifie que l'item ne soit pas null avant de valider la posibilité de pouvoir exécuter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandDependItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_Model.SelectedPlatform != null)
                e.CanExecute = true;
        }

        /// <summary>
        /// Commande pour éditer une plateforme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandEditPlat_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //this.Hide();
            _Model.EditPlatform();

            //this.Show();
        }

        #region edit platform games
        private void CommandEditGames_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_Model.SelectedPlatform != null)
                e.CanExecute = true;
        }

        private void CommandEditGames_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _Model.EditGames();

        }

        #endregion

        #region Locked Migrate
        private void LockedMigrate_Executed(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_Model != null)
                e.CanExecute =
                        _Model.CBAckupPlatform != null
                        && _Model.CBAckupPlatform.PlatformName.Equals(_Model.SelectedPlatform.Name)
                        /*&& !_Model.SelectedPlatform.Folder.Equals(_Model.PreviousPlatformState.Folder)*/;
        }

        private void LockedMigrate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _Model.LockedMigrate();
        }
        #endregion

        #region Open in explorer
        private void OpenInExplorer_Command(object sender, ExecutedRoutedEventArgs e)
        {
            string path = _Model.SelectedPlatform.Folder;
            ComCommands.OpenInExplorer_Command(System.IO.Path.GetFullPath(path, Global.LaunchBoxRoot));
        }
        #endregion

        private void VerifLine_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_Model.SelectedPlatform != null)
                e.CanExecute = true;
        }




        #region Migration
        private void CommandMigrate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Avertissement

            DxMBox.ShowDial(SPRLang.Warning_Music, DxTBLang.Warning, optMessage: SPRLang.W_Music_Message);
            Main_Window mw = new Main_Window();
            mw.ShowDialog();
            //W_Migrate wMig = new W_Migrate(_Model.SelectedPlatform);
            //wMig.ShowDialog();
        }

        #endregion


    }
}
