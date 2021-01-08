using DxTBoxCore.BoxChoose;
using DxTBoxCore.MBox;
using SPR.Common;
using SPR.Languages;
using SPR.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Graph
{
    /*
     *  - Signale par le wpf quand un champ est null (rouge + point d'exclamation). C'est juste un style.
     *  
     *  validation d'un champ 3 Méthodes
     *  1) IDataErrorInfo sur le model:
     *      A chaque caractère vérifie via un regex que le caractère n'est pas interdit pour
     *      les folders. ( Ne gère pas le paste, il faut perdre le focus. Réglé en mettant updatesourcetrigger)

     *  2) Une classe qui implémente ValidationRule
            Sur le chemin vérifie qu'il n'est pas null et que le chemin existe, quand on tape ou colle via
            une rule "DirectoryRule" (methode 2). Gère le paste.
            intégration wpf (exemple avec textbox)
            <TextBox.Text>
                <Binding Path="ChosenFolder" UpdateSourceTrigger="PropertyChanged">
                    <Binding.ValidationRules>
                        <rule:DirectoryRule />
                    </Binding.ValidationRules>
                </Binding>
            </TextBox.Text>

        3) INotifyDataErrorInfo sur le model:
            Vérifie que ce n'est ni null, ni via un regex que ça contien des caractères interdits,
            gère plusieurs fautes si nécessaire (faut changer par contre la partie dans le wpf).
            Gère le paste.        
            


        Conclusion:
            Dans tous les cas pour éviter la redondance il est préférable d'unifier. En effet pour définir
        qu'on active telle ou telle commande il va falloir stocker quelque part les erreurs. Certains combinaisons
        sont possibles mais ça peut vite devenir compliqué.
            La solution 2 n'est pas retenue car ça ne semble que visuel, sinon il faut faire une donnée statique 
        qui peut amener des problèmes. On peut contourner en faisant un check en profondeur dès qu'on va lancer la simulation
        après tout elle peut aussi prévenir des erreurs, mais on ne pourra pas les afficher par contre.
        La seule manière est avec le datainfo. Avec explicit on peut avoir la part visuelle et de l'autre le traitement, mais
        en même temps ça amène à devoir faire deux traitements séparés identiques. 
            La solution 1 pourrait faire l'affaire avec un déclenchement en explicit pour le chosenfolder de là
        on va faire sauter ou pas simulation mais on aura en tout cas l'affichage. A l'arrivée dans tous les cas
        c'est mal compatible avec le fait de rendre actif le bouton simulation en raison du fait de devoir
        tester sans cesse le chemin. Il reste la solution de faire comme le systeme d'event du 3
        avec un tableau dans lequel rentrer les erreurs et ainsi de juste demander au simulate de voir
        s'il y a des entrées. Mais le problème de tester le chemin d'accès juste quand on valide avec un bouton
        n'est pas compatible avec ce que je veux faire, c'est mieux s'il n'y a pas de répertoires à tester.
            - !!!! Solution 3 retenue en raison de la possibilité de notifier quand on veut !!!!
            Du coup, notification via enter, via + retest derrière avec notification quand on va lancer la simul



        

        - Pour intercepter un paste:
             DataObject.Pasting="Foo" à mettre dans le wpf sur le control (textbox par exemple)

        - Pour utiliser ctrl+z ctrl+y : IsUndoEnabled="True" dans la textbox

        - Behavior: Pour bloquer certaines touches, ne peut rien mentionner et ne détecte pas le paste

    */


    /// <summary>
    /// Logique d'interaction pour W_EditPlatform.xaml
    /// </summary>
    public partial class W_PlatformPaths : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public static readonly RoutedCommand ResetFactory = new RoutedCommand("ResetFactory", typeof(W_PlatformPaths));



        public PlatformModel Model { get; private set; }


        //private bool _OutPath
        //private bool
        /// <summary>
        /// Definit si l'on active la possibilité de lancer la simulation
        /// </summary>

        public bool SimulationOk
        {
            get;
            set;
        }

        public bool ActiveApply
        {
            get;
            set;
        }

        private bool _activeBox = true;
        /// <summary>
        /// Locker des box (utilisé pour bloquer toute écriture pendant la simulation et l'application)
        /// </summary>
        public bool ActiveBox
        {
            get { return _activeBox; }
            set
            {
                _activeBox = value;
                OnPropertyChanged();
            }
        }

        private int _Separator = 1;
        /// <summary>
        /// Separateur pour les chemins
        /// </summary>
        public int Separator
        {
            get { return _Separator; }
            set
            {
                if (_Separator != value)
                {
                    _Separator = value;
                    OnPropertyChanged();
                }
            }
        }


        public W_PlatformPaths()
        {
            InitializeComponent();
            DataContext = new PlatformModel();
        }

        /// <summary>
        /// Constructeur pour l'édition
        /// </summary>
        /// <param name="platform"></param>
        public W_PlatformPaths(IPlatform platform)
        {
            Model = new PlatformModel()
            {
                //Platform = platform,
            };
            Model.InitializeEdition(platform);

            // Calcul de la largeur de

            InitializeComponent();

            this.DataContext = Model;

            // "tb" is a TextBox
            //  DataObject.AddPastingHandler(tb, OnPaste);

        }


        /*
        private void ASlashRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _Model.FormatMainPaths(@"\");
        }

        private void ShlashRadioButton_Checked(object sender, RoutedEventArgs e)
        {

            _Model.FormatMainPaths(@"/");
        }*/

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string separator;
            if (Separator == 1)
                separator = @"\";
            else
                separator = @"/";

            Model.FormatMainPaths(separator);
        }
        #region Browse


        private void CommandBrowse_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !SimulationOk;
        }

        private void CommandBrowse_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // ChooseFolder cf = new ChooseFolder()
            TreeChoose tf = new TreeChoose
            {
                Model = new M_ChooseFolder()
                {
                    HideWindowsFolder = true,
                    PathCompareason = StringComparison.CurrentCultureIgnoreCase,
                    //2021 StartingFolder = Properties.Settings.Default.LastKPath,
                    StartingFolder = Properties.Settings.Default.LastKPath,
                },
                SaveButtonName = "Select",
            };


            if (tf.ShowDialog() != true)
                return;

            /*
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = Lang.FB_Description;*/
            //fait fbd.SelectedPath = Properties.Settings.Default.LastKPath;

            /*
            // fait if (fbd.ShowDialog() != DialogResult.OK) return;*/

            // Memorisation lastKnowPath
            string tmpPath;
            Properties.Settings.Default.LastKPath = tmpPath = tf.Model.LinkResult;
            Properties.Settings.Default.Save();

            // Assignation 
            Model.ChosenFolder = tf.Model.LinkResult;
            /*
            this.tbMainPath.Text = _NewRoot = tmpPath;
*/
        }

        #endregion Browse

        #region Simulation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandSimulate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            //    Debug.WriteLine($"Can ex {this.Model.HasErrors}");
            e.CanExecute = !this.Model.HasErrors && !SimulationOk;


            // e.CanExecute = this.Model.HasErrors; ;
            /*
             if (ActiveSimulate == false)
            else
                e.CanExecute = false;*/
        }


        private void CommandSimulate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Verouillage des textbox
            ActiveBox = false;

            BindingExpression be = chosenBox.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();

            // Lancement de la simulation
            if (Model.Simulate())
            {

                // simulation terminée, on peut masquer
                SimulationOk = true;
                DxMBox.ShowDial(SPRLang.SimuSuccess);
            }
            else
            {
                // On réactive en cas d'erreur
                ActiveBox = true;
                //
                SimulationOk = false;
                DxMBox.ShowDial(SPRLang.SimuFail);

            }

            //string tmp = _Model.ChosenFolder;
        }


        /*
         * On doit afficher stop quand apply est affiché
         */



        #endregion


        #region 2nd partie
        private void CommandApply_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SimulationOk;
        }

        private void CommandApply_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Model.Apply())
            {
                // Refill panel information
                string separator;
                if (Separator == 1)
                    separator = @"\";
                else
                    separator = @"/";

                Model.FormatMainPaths(separator);

                // Opérations de masquage des boutons
                //ActiveApply = false;
            }

            else
            {
                DxMBox.ShowDial(SPRLang.Paths_Not_Modified, SPRLang.Error);
            }

            SimulationOk = false;
            ActiveBox = true;
        }


        private void CommandStop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SimulationOk;
        }

        private void CommandStop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Model.RAZ_DPaths();
            SimulationOk = false;
            ActiveBox = true;
        }
        #endregion 2nd partie


        #region ResetFactory
        private void CommandResetFac_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

        }

        private void CommandResetFac_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Model.ResetFactory();
            SimulationOk = false;

            //ActiveApply = false;

            // Refill panel information
            string separator;
            if (Separator == 1)
                separator = @"\";
            else
                separator = @"/";

            DxMBox.ShowDial(SPRLang.IReset_Factory);
            // Model.FormatMainPaths(separator);
        }
        #endregion




        /// <summary>
        /// Vérifie si ça ne contient pas des caractères interdit, si c'est le cas, annule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PastingVerifFolder(object sender, DataObjectPastingEventArgs e)
        {
            string oo = (string)e.DataObject.GetData(typeof(string));

            if (Global.VerifString(oo, StringFormat.Folder))
            {

                e.CancelCommand();
            }
        }

        /// <summary>
        /// Vérifie que le chemin existe en cas de copie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void MainRoot_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            string oo = (string)e.DataObject.GetData(typeof(string));
            if (Model.VerifPath(oo, nameof(Model.ChosenFolder)))
                return;

            TextBox tb = (TextBox)sender;

            e.CancelCommand();
        }


        /// <summary>
        /// Si l'on appuie sur une touche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>     
        /// <remarks>
        /// Enter permet de faire un test de validation
        /// </remarks>
        private void TBChosenFolder_KeyUp(object sender, KeyEventArgs e)
        {
            // On vérifie le chemin quand on appuie sur entrée
            if (e.Key == Key.Enter)
            {
                //ActiveSimulate = true;
                //ActiveApply = false;
                Model.VerifPath(((TextBox)sender).Text, nameof(Model.ChosenFolder));
            }
            else
            {
                //ActiveSimulate = false; 

            }

        }

        /// <summary>
        /// Contrôle quand la box du systeme folder est touchée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void SFN_KeyUp(object sender, KeyEventArgs e)
        {


            // Si c'est null on désactive les deux boutons
            if (string.IsNullOrEmpty(Model.SystemFolderName))
            {
                //ActiveApply = false;

            }

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Model.Dispose();
        }
    }
}
