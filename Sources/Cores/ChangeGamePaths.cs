
using AsyncProgress;
using AsyncProgress.Cont;
using DxPaths.Windows;
using DxTBoxCore.Box_MBox;
using Hermes;
using SPR.Containers;
using SPR.Enums;
using SPR.Languages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Data;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Cores
{
    internal class ChangeGamePaths : I_SigProgress, I_ASBase, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

        public CancellationToken CancelToken => TokenSource.Token;

        public bool IsPaused { get; set; }

        public bool IsInterrupted { get; }

        public bool CancelFlag { get; }

        public event ProgressHandler UpdateProgress;
        public event StateHandler UpdateStatus;

        // ---

        /// <summary>
        /// Liste originale des jeux
        /// </summary>
        IEnumerable<IGame> _IPlatformGames;

        // ---
        /// <summary>
        /// Liste des chemins de la plateforme
        /// </summary>
        public Dictionary<MediaType, C_Paths> DicSystemPaths { get; private set; } =
                                                            new Dictionary<MediaType, C_Paths>();

        public IPlatform SelectedPlatform { get; internal set; }

        /// <summary>
        /// Platform hard path
        /// </summary>
        public string PlatformHardPath { get; set; }

        /// <summary>
        /// Platform relative path
        /// </summary>
        public string PlatformRelatPath { get; set; }


        /// <summary>
        /// Boolean to include hidden apps;
        /// </summary>
        /// <remarks>
        /// Pas de notif
        /// </remarks>
        public bool IncludeHidden { get; set; }

        /// <summary>
        /// Boolean to include Additionnal App Paths
        /// </summary>
        /// <remarks>
        /// example for a game with several roms
        /// </remarks>
        public bool AddAppPaths { get; set; }

        public string SubStringToRemove { get; set; }

        // --- Avec notification

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string _PlatformName;
        /// <summary>
        /// Current Name of the plateform
        /// </summary>
        public string PlatformName
        {
            get { return _PlatformName; }
            set
            {
                _PlatformName = value;
                OnPropertyChanged();
            }
        }

        private string _PlatformToReplace;
        public string PlatformToReplace
        {
            get => _PlatformToReplace;
            set
            {
                _PlatformToReplace = value;

                OnPropertyChanged();
            }
        }

        private GamePathMode _ChosenMode;


        public GamePathMode ChosenMode
        {
            get { return _ChosenMode; }
            set
            {
                RemoveError();

                switch (value)
                {
                    case GamePathMode.None:
                        AddError(SPRLang.Err_SelectMode);
                        break;

                }

                _ChosenMode = value;

                OnPropertyChanged();
            }
        }

        // ---

        /// <summary>
        /// Lock la collection des jeux
        /// </summary>
        private object _itemsLock = new object();

        /// <summary>
        /// Tables des jeux de la plateforme
        /// </summary>
        /*private List<C_Game> _ExtPlatformGames;
        public List<C_Game> ExtPlatformGames
        {
            get => _ExtPlatformGames;
            set
            {
                _ExtPlatformGames = value;
                OnPropertyChanged();
            }
        }*/

        public ObservableCollection<C_Game> ExtPlatformGames { get; set; } = new ObservableCollection<C_Game>();
        private void ClearGames()
        {
            lock (_itemsLock)
            {
                ExtPlatformGames.Clear();
            }
        }

        private void AddGame(C_Game game)
        {
            lock (_itemsLock)
            {
                ExtPlatformGames.Add(game);
            }
        }

        // ---

        public ChangeGamePaths()
        {
            BindingOperations.EnableCollectionSynchronization(ExtPlatformGames, _itemsLock);
        }

        /// <summary>
        /// Initialization with a platform (edition)
        /// </summary>
        /// <param name="selectedPlatform"></param>
        /// <remarks>
        /// Appelé par GamePathsModel (synchrone)
        /// </remarks>
        internal void InitializePlatform()
        {
            // --- Remplissage des informations de la plateforme
            HeTrace.WriteLine(@"Filling information fields"); // NewLine

            // Dans le cas où l'on est à la racine
            string folder = SelectedPlatform.Folder;
            if (string.IsNullOrEmpty(folder))
            {
                folder = Path.Combine(Global.LaunchBoxRoot, "Games", SelectedPlatform.Name);
            }

            if (folder.StartsWith('.'))
                PlatformRelatPath = folder;
            else
                PlatformRelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, folder);

            HeTrace.WriteLine(@"Transformation RelatLink To HardLink "); // NewLine
            PlatformHardPath = Path.GetFullPath(PlatformRelatPath, Global.LaunchBoxRoot);

            // --- Récupération des dossiers de la plateforme
            InitializePlateformFolders();
        }

        /// <summary>
        /// Construit le dictionnaire pour avoir tous les paths de la plateforme
        /// </summary>
        /// <remarks>
        /// Appelé:
        ///  - Initialisation
        ///  - Simulation
        /// </remarks>
        private void InitializePlateformFolders()
        {
            // --- Init pathfolders
            IPlatformFolder[] platformFolders;
            DicSystemPaths.Clear();

            //
            if (Global.DebugMode)
            {
                // Utilisation de pseudos dossiers
                platformFolders = ((MvPlatform)SelectedPlatform).GetAllPlatformFolders();
            }
            else
            {
                platformFolders = SelectedPlatform.GetAllPlatformFolders();
            }

            DicSystemPaths = Global.Make_DicPlatformPaths(SelectedPlatform.Folder, platformFolders);
        }

        /// <summary>
        /// Initialize games
        /// </summary>
        /// <remarks>
        /// Ne doit pas checker l'état des jeux
        /// Appelé par:
        ///  - Simulation
        ///  - ApplyChange
        ///  - GamePathModel
        /// </remarks>
        internal bool InitializeGames()
        {
            try
            {
                HeTrace.WriteLine("--- Initialize Games ---"); // NewLine
                UpdateStatus?.Invoke(this, new StateArg("Initialize Games"));

                IGame[] games = null;
                if (Global.DebugMode == true)
                {
                    games = DebugResources.GamesPaths;
                }
                else
                    games = SelectedPlatform.GetAllGames(IncludeHidden, true);

                // Remise en ordre des jeux
                _IPlatformGames = games
                    .Where(x => x.Title != null)
                    .OrderBy(x => x.Title);

                // --- Transformation des jeux
                //List<C_Game> extPlatformGames = new List<C_Game>();
                ClearGames();
                foreach (IGame game in _IPlatformGames)
                {
                    UpdateStatus?.Invoke(this, new StateArg($"Work on {game.Title}"));

                    C_Game tmpGame = new C_Game(game);
                    tmpGame.BuildPaths(game);

                    AddGame(tmpGame);
                    //extPlatformGames.Add(tmpGame);
                }

                //                ExtPlatformGames = extPlatformGames;
                return true;
            }
            catch (Exception exc)
            {
                HeTrace.WriteLine(exc.Message);
                HeTrace.WriteLine(exc.StackTrace);
                return false;
            }
        }


        /// <summary>
        /// Version asynchrone
        /// </summary>
        /// <returns></returns>
        public bool CheckAllGames()
        {
            try
            {
                UpdateStatus?.Invoke(this, new StateArg("Check All Games"));

                foreach (C_Game game in ExtPlatformGames)
                {
                    HeTrace.WriteLine($"Test for: {game.Title} [{ChosenMode}]");
                    UpdateStatus?.Invoke(this, new StateArg($"Test for: {game.Title}"));
                    game.CheckValid = CheckGame(game);
                }
                return true;
            }
            catch (Exception exc)
            {
                HeTrace.WriteLine(exc.Message);
                HeTrace.WriteLine(exc.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mvGame"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>
        private bool CheckGame(C_Game mvGame)
        {
            if (mvGame.ApplicationPath == null)
                return false;

            bool isValide = false;

            List<CState> states = new List<CState>();

            // Check des paths principaux (ne gère pas les images)
            foreach (C_PathsDouble pathO in mvGame.EnumGetPaths)
            {
                //  if (String.IsNullOrEmpty(pathO.OldRelatPath))
                if (pathO == null)
                    continue;

                HeTrace.WriteLine($"\t[CheckGame] {pathO.Type}: {pathO.RelatPath}");
                switch (pathO.Type)
                {
                    // Le premier initialise 'valide'
                    case nameof(PathType.ApplicationPath):
                        //isValide = pathO.OldRelatPath.Contains(DicSystemPaths["Application"]);
                        bool resPath = Test_Path(DicSystemPaths[MediaType.Application].RelatPath, pathO.RelatPath);
                        states.Add(new CState($"Main Application\n{pathO.RelatPath}", resPath));
                        isValide = resPath;
                        break;

                    // Les autres font une opération de bit
                    case nameof(PathType.ManualPath):
                        //isValide &= pathO.OldRelatPath.Contains(DicSystemPaths["Manual"]);
                        bool resMan = Test_Path(DicSystemPaths[MediaType.Manual].RelatPath, pathO.RelatPath);
                        states.Add(new CState($"Manual\n{pathO.RelatPath}", resMan));
                        isValide &= resMan;
                        //HeTrace.EndLine($"{isValide} (Manual): {DicSystemPaths["Manual"]}", 10);
                        break;

                    case nameof(PathType.MusicPath):
                        //isValide &= pathO.OldRelatPath.Contains(DicSystemPaths["Music"]);
                        bool resMusic = Test_Path(DicSystemPaths[MediaType.Music].RelatPath, pathO.RelatPath);
                        states.Add(new CState($"Music\n{pathO.RelatPath}", resMusic));
                        isValide &= resMusic;
                        //HeTrace.EndLine($"{isValide} (Music): {DicSystemPaths["Music"]}", 10);
                        break;

                    case nameof(PathType.VideoPath):
                        bool resVideo = Test_Path(DicSystemPaths[MediaType.Video].RelatPath, pathO.RelatPath);
                        states.Add(new CState($"Video\n{pathO.RelatPath}", resVideo));
                        isValide &= resVideo;
                        break;
                }


                //                valide &= pathO.Original_RLink.Contains(tboxROldPath.Text);
            }

            // Check des chemins additionnels, si les chemins additionnels sont pris en compte
            if (AddAppPaths)
                foreach (AAppPath pathO in mvGame.AddiRomPaths)
                {
                    bool test = Test_Path(DicSystemPaths[MediaType.Application].RelatPath, pathO.RelatPath);
                    //states.Add(new CState($"Clone id: {pathO.Id}", test));
                    states.Add(new CState($"Clone id: {pathO.Id}\n{pathO.RelatPath}", test));
                    isValide &= test;
                }

            HeTrace.WriteLine($"\tValidity of the game: {isValide}");

            mvGame.States = states;

            return isValide;
        }



        /// <summary>
        /// Teste la validité d'un chemin en fonction d'un référent
        /// </summary>
        /// <param name="referent">La racine</param>
        /// <param name="toTest">Le chemin vers le fichier</param>
        /// <returns></returns>
        bool Test_Path(string referent, string toTest)
        {
            HeTrace.WriteLine($"\t\treferent: '{referent}'", 10);
            HeTrace.WriteLine($"\t\tto test: '{toTest}'", 10);

            if (!string.IsNullOrEmpty(toTest) && string.IsNullOrEmpty(referent))
            {
                HeTrace.WriteLine($"\t\tResult: false - referent is null string", 10);
                return false;
            }

            #region 05/03/2021
            // dans le cas forcé on n'accepte pas les sous dossiers
            if (ChosenMode == GamePathMode.Forced)
            {
                string predictedP = $@"{referent}\{Path.GetFileName(toTest)}";
                if (toTest.Equals(predictedP))
                {
                    HeTrace.WriteLine($"\t\tResult: true", 10);
                    return true;
                }

            }
            // Dans le cas des keepsubfolders on accepte les mix
            else if (ChosenMode == GamePathMode.KeepSubFolders)
            {
                if (toTest.Contains(referent))
                {
                    HeTrace.WriteLine($"\t\tResult: true", 10);
                    return true;
                }
            }
            else
            {
                HeTrace.WriteLine($"\t\tResult: false - ChosenMode is set on None", 10);
                return false;

            }
            #endregion 05/03/2021

            HeTrace.WriteLine($"\t\tResult: false", 10);
            return false;
        }

        // ---

        /// <summary>
        /// Simulation
        /// </summary>
        internal bool Simulation(/*GamePathMode mode*/)
        {
            try
            {
                HeTrace.WriteLine("Simulation"); // NewLine
                UpdateStatus?.Invoke(this, new StateArg($"Simulation for {ChosenMode} ..."));

                // On verifie que ToReplace n'est pas null (peut arriver)
                /*         if (ChosenMode == GamePathMode.KeepSubFolders && string.IsNullOrEmpty(ToReplace))
                         {
                             AddError("Is Null", nameof(ToReplace));
                             return;
                         }*/

                // say mode
                HeTrace.WriteLine($"Mode: {ChosenMode}");

                // Refresh les chemins de la plateforme
                this.InitializePlateformFolders();
                HeTrace.WriteLine("The platform paths have been refreshed"); // NewLine

                // Refresh des jeux pour avoir la dernière version
                this.InitializeGames();
                HeTrace.WriteLine("Video game paths have been refreshed"); // NewLine

                HeTrace.WriteLine("Simulation begin...");
                // Modification des chemins
                foreach (C_Game game in ExtPlatformGames)
                {
                    UpdateStatus?.Invoke(this, new  StateArg($"Simulation: {game.Title}"));
                    // Trace.WriteLine($@"{game.Title}:");
                    HeTrace.WriteLine($"-- {game.Title}"); // NewLine

                    #region 04/03/2021
                    // On va profiter à chaque fois pour vérifier s'il y a validité
                    bool gameCheck = true;

                    bool gameToKeep = true;
                    #endregion

                    List<CState> states = new List<CState>();
                    // On examine tous les chemins, et en fonction du type on modifie les destinations                
                    foreach (C_PathsDouble pathO in game.EnumGetPaths)
                    {
                        string rootPath = "";


                        // Cas où pathO est null
                        if (pathO == null)
                        {
                            HeTrace.WriteLine($"\tPath O == null");
                            continue;
                        }

                        HeTrace.Write($"\t-- {pathO.Type}: ");


                        // On passe si le chemin d'origine est vide (balise vide)
                        if (string.IsNullOrEmpty(pathO.RelatPath))
                        {
                            HeTrace.WriteLine("null");
                            continue;
                        }

                        HeTrace.WriteLine("Existing");

                        // -------------------------- Vérification achevées

                        pathO.NewRelatPath = pathO.RelatPath;
                        pathO.NewHardPath = pathO.HardPath;

                        // Assignation du root path pour reconstruire

                        // On va commencer par chercher le chemin auquel rataché les informations
                        switch (pathO.Type)
                        {
                            // On peut utiliser le keep ou le forcer grâce à AppPathsBuilder
                            case nameof(PathType.ApplicationPath):
                                rootPath = DicSystemPaths[MediaType.Application].RelatPath;
                                PathsBuilder(pathO, rootPath);
                                break;

                            #region Forced mode
                            case nameof(PathType.ManualPath):
                                rootPath = DicSystemPaths[MediaType.Manual].RelatPath;
                                PathsBuilder(pathO, rootPath);
                                break;

                            case nameof(PathType.MusicPath):
                                rootPath = DicSystemPaths[MediaType.Music].RelatPath;
                                PathsBuilder(pathO, rootPath);
                                break;

                            case nameof(PathType.VideoPath):
                                rootPath = DicSystemPaths[MediaType.Video].RelatPath;
                                KeepSubFolderMode(pathO, rootPath);
                                break;
                            #endregion Forced mode


                            #region

                            /*
                            case "CartBackImagePath":
                                rootPath = dicSystemPaths["Cart - Back"];
                                boxLog.Text.Insert(0, $"CartBackImagePath: {rootPath}" + Environment.NewLine);
                                break;

                            case "CartFrontImagePath":
                                rootPath = dicSystemPaths["Cart - Front"];
                                boxLog.Text.Insert(0, $"CartFrontImagePath: {rootPath}" + Environment.NewLine);
                                break;

                            case "Cart3DImagePath":
                                break;
                            case "BackgroundImagePath":
                                break;
                            case "Box3DImagePath":
                                break;
                            case "BackImagePath":
                                break;


                            case "MarqueeImagePath":
                            case "FrontImagePath":
                                rootPath = dicSystemPaths["Box - Front"];
                                break;

                            case "ScreenshotImagePath":
                                rootPath = dicSystemPaths["Screenshot - Gameplay"];
                                break;
                            case "ClearLogoImagePath":
                                rootPath = dicSystemPaths["Clear Logo"];
                                break;*/

                            #endregion
                            default:
                                HeTrace.WriteLine("\t!!! Not managed");
                                continue;

                        }


                        if (!pathO.ToModify)
                        {
                            pathO.NewHardPath = SPRLang.No_Modif;
                            pathO.NewRelatPath = SPRLang.No_Modif;
                            HeTrace.WriteLine($"\tAppPathsBuilder no modification: {pathO.RelatPath} ");
                        }

                        bool test = pathO.RelatPath.Contains(rootPath) ||
                                        pathO.NewRelatPath.Contains(rootPath);
                        states.Add(new CState($"{pathO.Type}\n{pathO.RelatPath}\n{pathO.NewRelatPath}", test));
                        gameCheck &= test;

                        gameToKeep &= !pathO.ToModify;
                    }


                    if (AddAppPaths == true && game.AddiRomPaths.Count > 0)
                    {
                        HeTrace.WriteLine($"2020: Additionnal App Path Managing  -----"); // NewLine

                        // Modifications sur les chemins des roms mixed
                        foreach (AAppPath addiApp in game.AddiRomPaths)
                        {
                            addiApp.NewRelatPath = addiApp.RelatPath;
                            addiApp.NewHardPath = addiApp.HardPath;

                            string rootPath = DicSystemPaths[MediaType.Application].RelatPath;
                            HeTrace.WriteLine($"\t-- AdditionnalApplication: {addiApp.RelatPath}");

                            // Conversions des / en \
                            addiApp.RelatPath = addiApp.RelatPath.Replace('/', '\\');
                            addiApp.HardPath = addiApp.HardPath.Replace('/', '\\');

                            // Assignation du root
                            PathsBuilder(addiApp, rootPath);


                            if (!addiApp.ToModify)
                            {
                                addiApp.NewHardPath = SPRLang.No_Modif;
                                addiApp.NewRelatPath = SPRLang.No_Modif;
                                HeTrace.WriteLine($"\tAppPathsBuilder no modification: {addiApp.RelatPath} ");
                            }


                            bool test = addiApp.RelatPath.Contains(rootPath) ||
                                    addiApp.NewRelatPath.Contains(rootPath);
                            states.Add(new CState($"{addiApp.Id}\n{addiApp.RelatPath}\n{addiApp.NewHardPath}", test));
                            gameCheck &= test;
                            gameToKeep &= !addiApp.ToModify;

                        }
                    } // End additional app

                    game.CheckValid = gameCheck;
                    game.States = states;
                    HeTrace.WriteLine($"Check test (Simulation) {game.Title}:  {gameCheck}");

                    game.ToModify = !gameToKeep;
                    HeTrace.WriteLine($"ToModify test {game.Title}:  {game.ToModify}");
                }       // Fin du parcours des jeux;


                HeTrace.WriteLine("End of Simulation"); // NewLine
                return true;
            }
            catch (Exception exc)
            {
                HeTrace.WriteLine($"[Simulation] {exc.Message}");
                HeTrace.WriteLine(exc.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Créer les chemins en fonction du mode
        /// </summary>
        /// <param name="pathO"></param>
        /// <param name="rootPath">Chemin de root avec lequel travailler</param>
        /// <param name="mode">Mode des chemins (normal/forcé)</param>
        /// <remarks>
        /// Pour le moment uniquement utilisé pour les applications:
        ///     - Cas forcé.
        ///     - Cas du keep, il doit avoir le pivot inclus dans son chemin
        ///     - Cas du keep qui n'a pas le pivot inclus, on passe pour laisser l'utilisateur faire d'autres modifs
        ///     
        /// </remarks>
        private void PathsBuilder(C_PathsDouble pathO, string rootPath)
        {
            HeTrace.WriteLine($"\tUtilisation du rootpath {rootPath}", 10); // NewLine


            // modes
            if (ChosenMode == GamePathMode.Forced)
            {
                ForcedMode(pathO, rootPath);
            }
            // Mode conservant les sous-dossiers - Doit fonctionner normalement avec les fichiers à la racine aussi
            else if (ChosenMode == GamePathMode.KeepSubFolders)
            {
                KeepSubFolderMode(pathO, rootPath);
                RemoveSubString(pathO, rootPath);
            }

        }


        /// <summary>
        /// Mode forcé on ne va récupérer que le nom du fichier
        /// </summary>
        /// <param name="pathO"></param>
        /// <param name="categPath"></param>
        private void ForcedMode(C_PathsDouble pathO, string categPath)
        {
            string tmp = Path.Combine(categPath, Path.GetFileName(pathO.NewRelatPath));

            #region 26/04/2021
            if (pathO.NewRelatPath.Equals(tmp))
            {
                HeTrace.WriteLine($"\tPath use already ForcedMode: '{pathO.NewRelatPath}'");
                pathO.ToModify = false;
                return;

            }
            #endregion

            pathO.ToModify = true;

            // Recherche du lien réel + remise en forme en passant
            pathO.NewHardPath = Path.GetFullPath(tmp, Global.LaunchBoxRoot);
            pathO.NewRelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, pathO.NewHardPath);

            #region 24/12/2020
            // 05/03/2021 pathO.ToModify = true;
            #endregion

            HeTrace.WriteLine($"\tForcedMode: {pathO.NewRelatPath}");
        }

        /// <summary>
        /// Mode Keep, on conserve la structure
        /// </summary>
        /// <param name="pathO"></param>
        /// <param name="categPath"></param>
        private void KeepSubFolderMode(C_PathsDouble pathO, string categPath)
        {


            //string toReplace = ToReplace.Trim();
            HeTrace.WriteLine($"\tCheck {pathO.NewRelatPath}");


            // Si le chemin contient déjà l'ancien rootpath ou qu'il ne contient pas ce qu'on doit remplacer on passe
            bool cond1 = pathO.NewRelatPath.Contains(categPath);
            bool cond2 = pathO.NewRelatPath.Contains(PlatformToReplace);


            // Le chemin a déjà la bonne racine
            if (cond1)
            {
                HeTrace.WriteLine($"\tPath is Good");

                pathO.ToModify = false;

                return;
            }

            // Le chemin ne contient pas l'ancienne plateforme
            else if (!cond2)
            {
                HeTrace.WriteLine($"\tPath is bad, but doesn't contain '{PlatformToReplace}'");

                pathO.ToModify = false;
                return;
            }

            if (cond2)
            {
                HeTrace.WriteLine($"\tPath is bad and contains '{PlatformToReplace}'");

                // On récupère la mauvaise partie
                string badPart = pathO.NewRelatPath.Substring(0, pathO.NewRelatPath.IndexOf(PlatformToReplace) + PlatformToReplace.Length);
                HeTrace.WriteLine($"\tFound bad part: {badPart}");

                string tmp = pathO.NewRelatPath.Replace(badPart, categPath);

                pathO.NewHardPath = Path.GetFullPath(tmp, Global.LaunchBoxRoot);

            }

            pathO.ToModify = true;
            pathO.NewRelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, pathO.NewHardPath);
            HeTrace.WriteLine($"\tKeepSubFolderMode: {pathO.NewHardPath}");
            HeTrace.WriteLine($"\tKeepSubFolderMode: {pathO.NewRelatPath}");
        }

        private void RemoveSubString(C_PathsDouble pathO, string categPath)
        {
            if (string.IsNullOrEmpty(SubStringToRemove))
                return;

            string tail = pathO.NewRelatPath.Replace(categPath, string.Empty);

            string substringToRemove = SubStringToRemove.Replace('/', '\\');

            if (!tail.Contains(substringToRemove, StringComparison.OrdinalIgnoreCase))
                return;

            // Le chemin nécessite une modification sur la tail
            HeTrace.WriteLine($"\tPath need to remove '{substringToRemove}'");

            string target = tail.Replace(substringToRemove, string.Empty, StringComparison.OrdinalIgnoreCase)
                                    .Replace(@"\\", @"\");

            pathO.NewHardPath = Path.GetFullPath(Path.Combine(categPath, target.TrimStart('\\')),
                                        Global.LaunchBoxPath);
            pathO.NewRelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, pathO.NewHardPath);
            pathO.ToModify = true;

        }

        // ---

        /// <summary>
        /// Apply changes
        /// </summary>
        internal bool ApplyChanges()
        {
            try
            {
                HeTrace.WriteLine(@"[ApplyChanges] Process start ..."); // NewLine
                UpdateStatus?.Invoke(this, new StateArg("Apply"));

                foreach (var game in ExtPlatformGames)
                {
                    UpdateStatus?.Invoke(this, new StateArg($"Application for: {game.Title}"));

                    //
                    if (game.ToModify == false)
                    {
                        HeTrace.WriteLine($"\tGame already valid -> pass: {game.Title}");
                        continue;
                    }

                    DematrioChka(game);
                }

                if (!Global.DebugMode)
                {
                    HeTrace.WriteLine("Save");
                    PluginHelper.DataManager.Save();


                    //MessageBox.Show(Lang.Save_Ok, Lang.Save_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Régénération
                InitializeGames();
                // Check des jeux
                CheckAllGames();

                HeTrace.WriteLine(@"[ApplyChanges] Process finish."); // NewLine
                return true;
            }
            catch (Exception exc)
            {
                HeTrace.WriteLine($"[ApplyChanges] {exc.Message}");
                HeTrace.WriteLine("{exc.StackTrace}");
                return false;
            }

        }


        /// <summary>
        /// Utilisé par Apply
        /// </summary>
        /// <param name="game"></param>
        private void DematrioChka(C_Game game)
        {
            HeTrace.WriteLine($"\t[DematrioChka] {game.Title}");
            // Trace.Indent();

            // Cherchez dans tous les IGames
            /*IGame originalGame = null;
            foreach (IGame igame in _IPlatformGames)
            {
                if (igame.Id.Equals(game.Id))
                {
                    originalGame = igame;
                    break;
                }
            }*/
            IGame originalGame = _IPlatformGames.FirstOrDefault(x => game.Id.Equals(x.Id));
            if (originalGame == null)
            {
                HeTrace.WriteLine("\t\tGame not found");
                return;
            }

            if (originalGame.ApplicationPath == null)
            {
                HeTrace.WriteLine("\t\tGame's application path is null !");
                return;
            }

            #region 04/03/2021
            /*
            //  A totalement modifier car ne gère pas le "tomodify"
            if (!game.ApplicationPath.ToModify)
            {
               HeTrace.WriteLine("\t\tGame not modified (because of simulation method)");
                return;
            }*/

            #endregion 04/03/2021

            // Jeux                
            //originalGame.ApplicationPath = game.ApplicationPath == null ? null : game.ApplicationPath.NewRelatPath;
            // Manual
            // originalGame.ManualPath = game.ManualPath == null ? null : game.ManualPath.NewRelatPath;
            // Music
            //originalGame.MusicPath = game.MusicPath == null ? null : game.MusicPath.NewRelatPath;
            // Video
            //originalGame.VideoPath = game.VideoPath == null ? null : game.VideoPath.NewRelatPath;
            // Theme Video
            // originalGame.ThemeVideoPath = game.ThemeVideoPath == null ? null : game.ThemeVideoPath.NewRelatPath;

            // Passer tous les dossiers cibles des mvGames dans IPGames
            foreach (C_PathsDouble collecOPaths in game.EnumGetPaths)
            {
                // Si la balise est vide
                if (collecOPaths == null)
                {
                    HeTrace.WriteLine($"Path object is null");
                    continue;
                }

                // S'il n'y a pas d'utilité de cette partie modifier
                if (!collecOPaths.ToModify)
                {
                    HeTrace.WriteLine($"\t\t{collecOPaths.Type} Path not modified (Same as original)");
                    continue;
                }


                switch (collecOPaths.Type)
                {
                    // On passe sur les principaux car ils sont déjà traités
                    case nameof(PathType.ApplicationPath):
                        originalGame.ApplicationPath = game.ApplicationPath.NewRelatPath;
                        break;

                    case nameof(PathType.ManualPath):
                        originalGame.ManualPath = game.ManualPath.NewRelatPath;
                        break;

                    case nameof(PathType.MusicPath):
                        originalGame.MusicPath = game.MusicPath.NewRelatPath;
                        break;

                    case nameof(PathType.VideoPath):
                        originalGame.VideoPath = game.VideoPath.NewRelatPath;
                        break;
                }

                HeTrace.WriteLine($"\t\tModifications for {collecOPaths.Type}", 10);

                //
                collecOPaths.RelatPath = collecOPaths.NewRelatPath;
                collecOPaths.HardPath = collecOPaths.NewHardPath;
                collecOPaths.Raz_NewPaths();
            }

            #region 2020 Prise en charge des mixed roms

            // Uniquement si la box est cochée
            if (AddAppPaths == true)
            {
                HeTrace.WriteLine("\t\tModifications for the Additionnal Apps");
                foreach (IAdditionalApplication oMixedRoms in originalGame.GetAllAdditionalApplications())
                {
                    // Récupération des informations des chemins pour la mixes rom
                    AAppPath appPaths = game.AddiRomPaths.FirstOrDefault(x => x.Id.Equals(oMixedRoms.Id));

                    if (appPaths == null)
                    {
                        HeTrace.WriteLine("\t\tAdditionnal application not found");
                        continue;
                    }

                    // S'il n'y a pas d'utilité de cette partie modifier
                    if (!appPaths.ToModify)
                    {
                        HeTrace.WriteLine($"\t\t{appPaths.RelatPath} Path not modified (Same as Original)");
                        continue;
                    }


                    //MessageBox.Show($"{paths.ID} {paths.Destination_HLink} - {paths.Destination_RLink}");

                    //
                    oMixedRoms.ApplicationPath = appPaths.NewRelatPath;
                    //
                    appPaths.RelatPath = appPaths.NewRelatPath;
                    appPaths.HardPath = appPaths.NewHardPath;
                    appPaths.Raz_NewPaths();
                }
            }
            #endregion
        }


        #region errors

        /// <summary>
        /// All Errors
        /// </summary>
        private readonly Dictionary<string, List<string>> _erreurs = new Dictionary<string, List<string>>();


        public bool HasErrors => _erreurs.Any();// throw new NotImplementedException();



        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
        {

            if (_erreurs.ContainsKey(propertyName))
            {
                string tmp = null;

                // Récupération des erreurs de la propriété concernée
                foreach (string errMess in _erreurs[propertyName])
                    if (tmp == null)
                        tmp = errMess;
                    else
                        tmp += Environment.NewLine + errMess;

                yield return tmp;

            }
        }


        /// <summary>
        /// Ajoute une erreur au tableau des erreurs
        /// </summary>
        /// <param name="propertyName">Name of property</param>
        /// <param name="erreur">error description</param>
        private void AddError(string erreur, [CallerMemberName] string propertyName = null)
        {
            // On ajoute au cas où ça serait manquant
            if (!_erreurs.ContainsKey(propertyName))
            {
                _erreurs[propertyName] = new List<string>();
            }

            _erreurs[propertyName].Add($"{propertyName}: {erreur}");
        }

        /// <summary>
        /// Enlève une erreur en fonction du nom de la propriété
        /// </summary>
        /// <param name="propertyName"></param>

        private void RemoveError([CallerMemberName] string propertyName = null)
        {
            if (_erreurs.ContainsKey(propertyName))
                // On enlève de la file
                _erreurs.Remove(propertyName);
        }



        #endregion

        public void Pause(int timeSleep)
        {
            throw new NotImplementedException();
        }

        public void StopTask()
        {
            throw new NotImplementedException();
        }
    }
}
