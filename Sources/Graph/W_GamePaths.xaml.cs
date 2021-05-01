using Hermes;
using SPR.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Graph
{
    /// <summary>
    /// Logique d'interaction pour W_GamePaths.xaml
    /// </summary>
    public partial class W_GamePaths : Window//, INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        #region Sans notif
        /// <summary>
        /// Element jeu agrandi
        /// </summary>
        private CC_Game _ChosenPanel;


        /*  private bool _activeCtrl = true;
          public bool ActiveCtrl
          {
              get { return _activeCtrl; }
              set
              {
                  _activeCtrl = value;
                  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ActiveCtrl"));
              }
          }*/
        #endregion

        #region Avec notif
        /*  public int ChosenMode
          private int _ChosenMode = 0;
          /// <summary>
          /// Chosen Mode
          /// </summary>
          {
              get { return _ChosenMode; }
              set
              {
                  if (_ChosenMode != value)
                  {
                      _ChosenMode = value;
                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChosenMode"));

                  }

              }
          }*/

        #endregion

        #region commandes
        public static readonly RoutedCommand RescanCommand = new RoutedCommand();
        #endregion


        internal GamePathsModel Model { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedPlatform"></param>
        public W_GamePaths()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            //  Model.InitializeEdition(selectedPlatform);

            Mouse.OverrideCursor = null;

            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = Model;

        }

        /// <summary>
        /// Action when a game element is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void C_Game_OnClickPanel(object sender, RoutedEventArgs e)
        {
            CC_Game cG = (CC_Game)sender;

            // if same => pass
            if (_ChosenPanel == null)
                _ChosenPanel = cG;
            else if (_ChosenPanel.Title.Equals(cG.Title))
                return;
            else
            {
                _ChosenPanel.PanelOpen = false;
                _ChosenPanel = cG;
            }
        }

        /// <summary>
        /// Action when a mode is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void ModesChecked(object sender, RoutedEventArgs e)
        {
            // Reinitialisation pour la simulation
            Model.ActiveSimulate = true;
            Model.ActiveApply = false;

            // Recherche du dossier pivot pour déterminer ce que l'on garde
            /*
            if (ChosenMode == 1)
            {
                _Model.ToReplace = string.Empty;
                //   ShowModeTB = false;
            }
            else if (ChosenMode == 2)
            {
                _Model.FindPivot();
                // ShowModeTB = true;
            }*/
        }


        #region Simulate

        private void Simulate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Model != null)
            {
                HeTrace.WriteLine($"Simulate_CanExecute: {Model.ActiveSimulate} | {Model.Core.HasErrors} ");
                e.CanExecute = Model.ActiveSimulate && !Model.Core.HasErrors;
            }
        }

        private void Simulate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // ---
            Model.PrepareSimulation();
        }
        #endregion Simulate

        #region Apply

        private void Apply_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Model != null)
                e.CanExecute = Model.ActiveApply;
        }

        private void Apply_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Model.PrepareApply();
        }
        #endregion Apply

        #region Rescan
        private bool _ActiveRescan;
        private void Rescan_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _ActiveRescan;
        }

        private void Rescan_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        #endregion

        /// <summary>
        /// Hidden Games Checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HidGames_Checked(object sender, RoutedEventArgs e)
        {
            _ActiveRescan = true;
            Model.ActiveSimulate = true;
            Model.ActiveApply = false;

        }


        #region Enclenche un test des jeux
        private void cbModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Model.PrepareCheckAllGames();
            Model.ActiveSimulate = true;
            Model.ActiveApply = false;

        }

        /// <summary>
        /// Additionnal Paths Checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AAP_Checked(object sender, RoutedEventArgs e)
        {
            // Revérification des games 
            Model.PrepareCheckAllGames();

            Model.ActiveSimulate = true;
            Model.ActiveApply = false;
        }

        private void AAP_UnChecked(object sender, RoutedEventArgs e)
        {
            // Revérification des games 
            Model.PrepareCheckAllGames();

            Model.ActiveSimulate = true;
            Model.ActiveApply = false;
        }

        #endregion



        private void PlatformToReplace_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Model.ActiveSimulate = true;
            Model.ActiveApply = false;

            if (Global.VerifString(e.Text, Common.StringFormat.Folder))
                e.Handled = true;


        }
        private void SubstringToRemove_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Model.ActiveSimulate = true;
            Model.ActiveApply = false;

            if (Global.VerifString(e.Text, Common.StringFormat.Path))
                e.Handled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Model.Dispose();
        }


    }
}
