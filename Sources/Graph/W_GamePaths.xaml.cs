using SPR.Models;
using System;
using System.ComponentModel;
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

        /// <summary>
        /// Apply mode activated
        /// </summary>
        private bool _ActiveApply { get; set; }
        private bool _ActiveSimulate { get; set; } = true;

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


        GamesModel _Model;

        public W_GamePaths()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedPlatform"></param>
        public W_GamePaths(IPlatform selectedPlatform)
        {
            _Model = new GamesModel()
            {

            };
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            _Model.InitializeEdition(selectedPlatform);

            Mouse.OverrideCursor = null;

            InitializeComponent();

            this.DataContext = _Model;
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
            _ActiveSimulate = true;
            _ActiveApply = false;

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

        /// <summary>
        /// Remet à zéro la visibilité des boutons
        /// </summary>
        private void ResetButtons()
        {

        }

        #region Simulate
        /// <summary>
        /// Simulation mode activated
        /// </summary>
        //private bool _ActiveSimulate { get; set; }

        private void Simulate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _ActiveSimulate && !_Model.HasErrors;
        }

        private void Simulate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //GamePathMode mode = GamePathMode.None;
            // ---
            /*if (ChosenMode == 1)
                mode = GamePathMode.Forced;
            else if (ChosenMode == 2)
                mode = GamePathMode.KeepSubFolders;
            */
            // ---
            _Model.Simulation();

            // ---
            _ActiveSimulate = false;
            _ActiveApply = true;
        }
        #endregion Simulate

        #region Apply

        private void Apply_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _ActiveApply;
        }

        private void Apply_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _Model.ApplyChanges();

            _ActiveSimulate = true;
            _ActiveApply = false;
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
            _ActiveSimulate = true;
            _ActiveApply = false;

        }

        /// <summary>
        /// Additionnal Paths Checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AAP_Checked(object sender, RoutedEventArgs e)
        {
            // Revérification des games 
            _Model.CheckAllGames();

            /*if (ChosenMode == 0)
                return;*/

            _ActiveSimulate = true;
            _ActiveApply = false;
        }

        private void AAP_UnChecked(object sender, RoutedEventArgs e)
        {
            // Revérification des games 
            _Model.CheckAllGames();

            /*  if (ChosenMode == 0)
                  return;*/

            _ActiveSimulate = true;
            _ActiveApply = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void tbReplace_KeyDown(object sender, KeyEventArgs e)
        {
            RemoveLastASlash((TextBox)sender);
            _ActiveSimulate = true;
            _ActiveApply = false;
        }

        /// <summary>
        /// Vérifie que le dernier caractère n'est pas un 
        /// </summary>
        [Obsolete]
        private void RemoveLastASlash(TextBox tb)
        {
            char c = tb.Text[tb.Text.Length - 1];
            if (c == '\\')
            {
                _Model.ToReplace = tb.Text.Substring(0, tb.Text.Length - 2);

                tb.Select(tb.Text.Length, 0);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _Model.Dispose();
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void cbModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _Model.CheckAllGames();
            _ActiveSimulate = true;
            _ActiveApply = false;
            
        }
    }
}
