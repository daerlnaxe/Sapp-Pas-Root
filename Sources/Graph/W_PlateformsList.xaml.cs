using DxTBoxCore.Common;
using DxTBoxCore.Languages;
using DxTBoxCore.MBox;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class W_PlateformsList : Window
    {


        #region commandes
        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedCommand EditPlatRC = new RoutedCommand("Edit_Platform", typeof(W_PlateformsList));
        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedCommand EditGames = new RoutedCommand("Edit_Games", typeof(W_PlateformsList));

        /// <summary>
        /// 
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show(e.OriginalSource.ToString());
            this.Hide();
            W_PlatformPaths wpp = new W_PlatformPaths();
            wpp.ShowDialog();
            this.Show();

        }


        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

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

        }

        private void ModifyPlatform_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Edit_Platforme(_Model.SelectedPlatform);
        }

        /// <summary>
        /// Show form to edit a platform
        /// </summary>
        /// <param name="platform"></param>
        private void Edit_Platforme(IPlatform platform)
        {
            if (_Model.SelectedPlatform == null)
                return;

            this.Hide();

            W_PlatformPaths wp = new W_PlatformPaths();
            wp.ShowDialog();

            this.Show();
        }


        #endregion

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

            try
            {
                //this.Hide();

                string oldPath = _Model.SelectedPlatform.Folder;

                // Lancement de la modification des paths
                W_PlatformPaths wPP = new W_PlatformPaths(_Model.SelectedPlatform)
                {
                };

                //wp.Model.InitializeEdition(SelectedPlatform) ;


                wPP.ShowDialog();

                // Rafraichissement
                if (!Global.DebugMode)// && !wp.Model.PlatformObject.Folder.Equals(oldPath))
                {
                    _Model.Initialize();
                }

                // Si la plateforme n'a pas le même dossier on va proposer une reconstruction pour les jeux
                if (!oldPath.Equals(_Model.SelectedPlatform.Folder) &&
                    DxMBox.ShowDial(SPRLang.QChange_GamesPaths, SPRLang.Question, DxButtons.YesNo) == true)
                {
                    W_GamePaths wGP = new W_GamePaths(_Model.SelectedPlatform);
                    wGP.ShowDialog();

                }



            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                HeTrace.WriteLine(exc.Message);
                HeTrace.WriteLine(exc.StackTrace);
            }


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
            W_GamePaths wp = new W_GamePaths(_Model.SelectedPlatform);
            wp.ShowDialog();
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
