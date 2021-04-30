using DxPaths.Windows;
using DxTBoxCore.Box_MBox;
using DxTBoxCore.BoxChoose;
using Hermes;
using Hermes.Messengers;
using SPR.Containers;
using SPR.Languages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;
using static SPR.Properties.Settings;

namespace SPR.Models
{
    /*
     * La validation du chemin se fait soit sur enter, 
     * soit au début de la fonction simulation
     */



    public partial class PlatformModel : INotifyPropertyChanged/*, IDataErrorInfo */ , INotifyDataErrorInfo, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        /// <summary>
        /// All Errors
        /// </summary>
        private readonly Dictionary<string, List<string>> _erreurs = new Dictionary<string, List<string>>();

        // ---

        #region Avec notification
        /*
        private String _Log;
        /// <summary>
        /// Log des informations
        /// </summary>
        public string Log
        {
            get { return _Log; }
            set
            {
                _Log = value;
                OnPropertyChanged();
            }
        }*/
        /// <summary>
        /// Utilisé pour le log à l'écran
        /// </summary>
        public MeVerbose Mev
        {
            get;
            set;
        }


        /*private string _PlatformName;
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
        }*/

        private string _CHardLink;
        /// <summary>
        /// Current MAIN Hard Path of the plateform
        /// </summary>
        public string CHardLink
        {
            get { return _CHardLink; }
            set
            {
                _CHardLink = value;
                OnPropertyChanged();
            }
        }

        private string _CRelatLink;
        /// <summary>
        /// Current MAIN Relative Path of the platform
        /// </summary>
        public string CRelatLink
        {
            get { return _CRelatLink; }
            set
            {
                _CRelatLink = value;
                OnPropertyChanged();
            }
        }

        private string _CPlatformPath;
        /// <summary>
        /// Lien brut donné par le iplatform
        /// </summary>
        /// <remarks>Voir si utile</remarks>        
        private string CPlatformPath
        {
            get { return _CPlatformPath; }
            set
            {
                _CPlatformPath = value;
                OnPropertyChanged();
            }
        }

        private string _chosenFolder;
        /// <summary>
        /// Dossier choisi (dans la textbox)
        /// </summary>
        public string ChosenFolder
        {
            get { return _chosenFolder; }
            set
            {
                _chosenFolder = value;
                OnPropertyChanged();
                //VerifPath(value, nameof(ChosenFolder));
            }
        }

        private int _SizeCateg;
        /// <summary>
        /// Largeur de la catégorie des bandeaux
        /// </summary>
        public int SizeCateg
        {
            get { return _SizeCateg; }
            set
            {
                _SizeCateg = value;
                OnPropertyChanged();
            }
        }

        private C_Platform _PlatformPaths;
        /// <summary>
        /// Tableau des chemins de la plateforme
        /// </summary>
        /// <remarks>
        /// With Notif
        /// </remarks>        
        public C_Platform PlatformPaths
        {
            get { return _PlatformPaths; }
            set
            {
                _PlatformPaths = value;
                OnPropertyChanged();
            }
        }

        private string _SystemFolderName;
        /// <summary>
        /// Détermine le nom du system que l'on veut employer
        /// </summary>
        /// <remarks>
        /// Calculé au début ainsi que via le reset factory
        /// </remarks>
        public string SystemFolderName
        {
            get { return _SystemFolderName; }
            set
            {
                _SystemFolderName = value;
                OnPropertyChanged();
                Validate_FolderName(value);
            }
        }
        #endregion Avec notification

        // ---

        #region Sans notification
        string _gamesFName;
        /// <summary>
        /// Name of games folder
        /// </summary>
        public string GamesFName
        {
            get { return _gamesFName; }
            set
            {
                _gamesFName = value;
                OnPropertyChanged();
                Validate_FolderName(value);
            }
        }

        private string _manualsFName;
        /// <summary>
        /// Name of Manuals folder
        /// </summary>
        public string ManualsFName
        {
            get { return _manualsFName; }
            set
            {
                //ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("ManualsFName"));
                _manualsFName = value;
                Validate_FolderName(value);

            }
        }

        private string _imagesFName;
        /// <summary>
        /// Name of Images folder
        /// </summary>
        public string ImagesFName
        {
            get { return _imagesFName; }
            set
            {
                _imagesFName = value;
                Validate_FolderName(value);
            }
        }

        private string _musicsFName;
        /// <summary>
        /// Name of Musics folder
        /// </summary>
        public string MusicsFName
        {
            get { return _musicsFName; }
            set
            {
                _musicsFName = value;
                Validate_FolderName(value);
            }
        }

        private string _videosFName;
        /// <summary>
        /// Name of Videos folder
        /// </summary>
        public string VideosFName
        {
            get { return _videosFName; }
            set
            {
                _videosFName = value;
                Validate_FolderName(value);
            }
        }

        /// <summary>
        /// Selected Plateform
        /// </summary>
        public IPlatform PlatformObject { get; private set; }

        /*
         * Sans event, ne fonctionne pas si la propriété n'est pas mise à jour directement (sur un contrôle après par ex)
         * NotifyOnTargetUpdated=True ne sert pas à l'event

         */
        internal void Test()
        {
            AddError(nameof(ChosenFolder), "bzzz");
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(ChosenFolder)));
        }

        private string previousFolderPath;

        /// <summary>
        /// Tableau contenant tous les dossiers de la plateforme
        /// </summary>
        /// <remarks>
        /// Ne pas bouger faudra refresh au plus près
        /// </remarks>
        IPlatformFolder[] _PlatformFolders { get; set; }

        // public C_Platform PlatformObject { get; private set; }
        #endregion Sans notification

        // ---

        /// <summary>
        /// Constructeur vide
        /// </summary>
        public PlatformModel()
        {
        }

        /// <summary>
        /// Constructeur pour l'édition
        /// </summary>
        /// <param name="platform"></param>
        public PlatformModel(string platformName)
        {
            InitializeEdition(platformName);
        }



        #region Initialization Functions

        /// <summary>
        /// Initialization with a platform (edition)
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public void InitializeEdition(string platformName)
        {
            if (string.IsNullOrEmpty(platformName))
                throw new NullReferenceException("Object Plateform is null !");


            if (Global.DebugMode)
            {
                HeTrace.WriteLine("Debug Mode Activé");
                PlatformObject = DebugPoint.FakePlatforms.First(x => x.Name.Equals(platformName));
            }
            else
            {
                PlatformObject = PluginHelper.DataManager.GetPlatformByName(platformName);
                HeTrace.WriteLine("Plugin Mode Activé");
            }


            // Messenger
            Mev = new MeVerbose()
            {
                ByPass = true,
                LogLevel = Global.Config.VerbLvl,
            };
            HeTrace.AddMessenger("Verbose", Mev);

            // on récupère le dernier dossier visité
            ChosenFolder = Properties.Settings.Default.LastKPath;


            HeTrace.WriteLine("Initialisation");
            HeTrace.WriteLine($@"LaunchBox main path: {Global.LaunchBoxPath}");
            HeTrace.WriteLine($"Identified System: {PlatformObject.Name}");

            // Initialization of paths
            this.InitializePaths();

            // Detection du nom du dossier system (vérifié le 29/04/2021)
            if (string.IsNullOrEmpty(PlatformObject.Folder))
            {
                SystemFolderName = PlatformObject.Name;
            }
            else
            {
                // On prend le dernier dossier
                SystemFolderName = PlatformObject.Folder.Substring(PlatformObject.Folder.LastIndexOf(@"\") + 1);
            }

            HeTrace.WriteLine($"Identified Folder System: {SystemFolderName}");

            // Initialisation des folders name            
            GamesFName = Default.AppsFolder;
            ImagesFName = Default.ImagesFolder;
            ManualsFName = Default.ManualsFolder;
            MusicsFName = Default.MusicsFolder;
            VideosFName = Default.VideosFolder;

        }

        /// <summary>
        /// (Re)Initialize les paths
        /// </summary>
        private void InitializePaths()
        {
            HeTrace.WriteLine("Paths initialization", 5);

            // Initialization according to the mode (debug/plugin)
            if (Global.DebugMode)
                // Utilisation de pseudos dossiers
                _PlatformFolders = ((MvPlatform)PlatformObject).GetAllPlatformFolders();
            else
                // Récupération de tous les dossiers + tri
                _PlatformFolders = PlatformObject.GetAllPlatformFolders()
                                        .OrderBy(x => x.MediaType).ToArray();

            //
            HeTrace.WriteLine($"Dossier de jeu: {PlatformObject.Folder}");
            C_Platform tmp = C_Platform.Platform_Maker(PlatformObject, _PlatformFolders);

            PlatformPaths = tmp;
            GC.Collect();
        }

        #endregion


        /// <summary>
        /// Format paths to seek the main and show him as hard and relative
        /// </summary>
        /// <param name="v"></param>
        internal void FormatMainPaths(string sep)
        {
            HeTrace.WriteLine("Format main paths", 5);
            // On détermine le path en selon l'emplacement de Launchbox
            if (String.IsNullOrEmpty(PlatformObject.Folder))
            {
                CHardLink = Global.LaunchBoxRoot; // On récupère avec le core car ça correspond pas sinon
                CRelatLink = @".\";
                return;
            }

            // Détermination du chemin réel si c'est un chemin relatif
            string platformPath = null;
            if (PlatformObject.Folder.StartsWith('.') || PlatformObject.Folder.Substring(1, 2) != @":\")
            {
                //platformPath = Path.GetFullPath(PlatformObject.Folder, Global.LaunchBoxPath);
                platformPath = Path.GetFullPath(PlatformObject.Folder, Global.LaunchBoxRoot);
            }
            else
            {
                platformPath = PlatformObject.Folder;
            }

            #region Remplace FillInformation de l'ancienne version

            HeTrace.WriteLine($@"PlatformFolder: '{PlatformObject.Folder}'"); // Envnew
            HeTrace.WriteLine(@"Search main paths"); // Envnew


            #region Recherche du dossier hard
            HeTrace.Write(@"--- Seek of HardLink: ");

            string[] foldersArr = platformPath.Split(sep);          // Sur windows le getfullpath a ramené le chemin avec des \

            // Ici une erreur peut se produire si on a un chemin de type "G:"
            CHardLink = String.Join(sep, foldersArr, 0, foldersArr.Length - 2);
            HeTrace.EndLine($"'{CHardLink}'"); // Envnew
            #endregion


            #region conversion en dur du lien vers le dossier actuel 

            // Vérification que le path est sur le même chemin que l'application
            DriveInfo driveApp = new DriveInfo(Global.LaunchBoxPath);
            DriveInfo driveFolder = new DriveInfo(platformPath);
            if (driveApp.Name.Equals(driveFolder.Name))
            {
                HeTrace.Write(@"--- Transformation HardLink RelatLink: ");
                CRelatLink = DxPath.To_Relative(Global.LaunchBoxRoot, CHardLink);
                //CRelatLink = Path.GetRelativePath(Global.LaunchBoxPath, CHardLink);
                HeTrace.WriteLine($"'{CRelatLink}'"); // Envnew

            }
            else
            {
                HeTrace.WriteLine(@"--- RelatLink impossible different drives letters "); // Envnew

            }


            //tmp = tmp.Replace($@"\{OldPlatformObject.Name}", "");

            //CRelatLink = tmp;
            //tmp = null;
            #endregion

            #endregion Remplace FillInformation de l'ancienne version

        }



        internal void Browse()
        {
            // ChooseFolder cf = new ChooseFolder()            
            TreeChoose tf = new TreeChoose
            {
                Model = new M_ChooseFolder()
                {
                    HideWindowsFolder = true,
                    PathCompareason = StringComparison.CurrentCultureIgnoreCase,
                    StartingFolder = Properties.Settings.Default.LastKPath,
                },
                SaveButtonName = "Select",
            };

            if (tf.ShowDialog() != true)
                return;

            string tmpPath;
            Default.LastKPath = tmpPath = tf.Model.LinkResult;
            Default.Save();

            VerifPath(tf.Model.LinkResult, nameof(ChosenFolder));

            // Assignation 
            ChosenFolder = tf.Model.LinkResult;
        }



        #region Simulation
        /// <summary>
        /// Simulate effect on paths
        /// </summary>
        /// <remarks>
        /// On ne modifie pas le chemin pour laisser le soin à l'utilisateur de le modifier manuellement
        /// </remarks>
        internal bool Simulate()
        {
            HeTrace.WriteLine(">>> Simulation");

            // Vérification du chemin
            if (!VerifPath(ChosenFolder, nameof(ChosenFolder)))
                return false;


            // --- Mettre à jour les chemins
            InitializePaths();

            // --- Lancer la simulation

            #region AlterPath

            HeTrace.WriteLine("Altération des dossiers", 5);

            // foreach (var ecp in ExtPlatformPaths)
            foreach (C_PathsDouble ecp in PlatformPaths)
            {

                HeTrace.WriteLine($"\t[AlterPath] {ecp.Type}", level: 10);

                // Choix de filtrer que videos / games / manuals / music et ... ?
                string hardPath;

                // Games
                if (ecp.Type.Equals("Games", StringComparison.OrdinalIgnoreCase))
                    hardPath = Path.Combine(_chosenFolder, GamesFName, SystemFolderName);
                // Manual
                else if (ecp.Type.Equals("Manual", StringComparison.OrdinalIgnoreCase))
                    hardPath = Path.Combine(_chosenFolder, ManualsFName, SystemFolderName);
                // Music
                else if (ecp.Type.Equals("Music", StringComparison.OrdinalIgnoreCase))
                    hardPath = Path.Combine(_chosenFolder, MusicsFName, SystemFolderName);
                // Video
                else if (ecp.Type.Equals("Video", StringComparison.OrdinalIgnoreCase))
                    hardPath = Path.Combine(_chosenFolder, VideosFName, SystemFolderName);
                // Other type of video
                else if (ecp.Type.Equals("Theme Video", StringComparison.OrdinalIgnoreCase))
                    hardPath = Path.Combine(_chosenFolder, VideosFName, SystemFolderName, "Theme" );
                // Images
                else
                    hardPath = Path.Combine(_chosenFolder, ImagesFName, SystemFolderName, ecp.Type);


                //string relatPath = Path.GetRelativePath(Global.LaunchBoxPath, hardPath);
                string relatPath = DxPath.To_Relative(Global.LaunchBoxRoot, hardPath);

                ecp.NewRelatPath = relatPath;
                ecp.NewHardPath = hardPath;

                HeTrace.WriteLine($"\tCombinaison give: {hardPath}", 10);
                HeTrace.WriteLine($"\tRelat find give: {relatPath}", 10);
                // Analyse props n'a plus d'utilité
                // Generate info path n'a apparemment plus d'utilité.
                // Style FLP change le fond mais pas utile ici
                // SetMainWindow Pour la taille de la fenêtre, pas utile ici non plus

            }

            #endregion AlterPath

            HeTrace.WriteLine(">>> End of Simulation"); // Envnew

            return true;

        }

        #endregion

        #region Apply 
        /// <summary>
        /// Appliquer les changements de la simulation dans les fichiers de configuration de launchbox.
        /// </summary>
        /// <returns></returns>
        internal bool Apply()
        {
            HeTrace.WriteLine(@"Process start ..."); // Envnew

            /*
             * Assignation des properties settings ? 
             * Todo réfléchir car je ne sauve pas au fur et à mesure mais en même temps ça m'évite
             * avec une sauvegarde systématique d'oublier éventuellement ce que je voulais faire
            */


            // Dematriochka
            Dematriochka();

            // Sauvegarde
            previousFolderPath = PlatformObject.Folder;
            PlatformObject.Folder = //[0].OldRelatPath;    // Assignation du chemin path des games à la plateforme
            PlatformObject.Folder = PlatformPaths.ApplicationPath.RelatPath;    // Assignation du chemin path des games à la plateforme
            if (!Global.DebugMode)
            {
                PluginHelper.DataManager.Save();
                DxMBox.ShowDial(SPRLang.Paths_Modified);

            }


            // Régénération panneau de gauche

            HeTrace.WriteLine(@"Process finish ..."); // Envnew

            return true;
        }

        /// <summary>
        /// On va parcourir les PathsCollec et assigner les paths de la simulation aux paths de la plateforme
        /// </summary>
        /// <remarks>
        /// Il n'y a pas de sauvegarde ici, afin de pouvoir continuer en mode debug
        /// </remarks>
        private void Dematriochka()
        {
            HeTrace.WriteLine("[PlatformPaths] [Dematriochka]");
            // Trace.Indent();

            // Modification of App
            //C_PathsCollec game = ExtPlatformPaths[0];
            C_PathsDouble game = PlatformPaths.ApplicationPath;

            // Traitement manuel car ce dossier n'est pas contenu dans les ipfolder
            HeTrace.WriteLine("\t[Dematriochka] Management of 'Game' property", 5);
            game.RelatPath = game.NewRelatPath;
            game.HardPath = game.NewHardPath;
            game.Raz_NewPaths();

            // Traitement des autres

            foreach (IPlatformFolder ipFolder in _PlatformFolders)
            // foreach (IPlatformFolder ipFolder in PlatformPaths)
            {
                HeTrace.WriteLine($"\t[PlatformPaths] [Dematriochka] Management of '{ipFolder.MediaType}' property", 5);

                //C_PathsCollec pFolder = ExtPlatformPaths.FirstOrDefault(x => x.Type == ipFolder.MediaType);
                C_PathsDouble pFolder = PlatformPaths.GetPaths().FirstOrDefault(x => x.Type == ipFolder.MediaType);

                //
                if (pFolder != null)
                {
                    // Assignation au platform Path de Launchbox
                    ipFolder.FolderPath = pFolder.NewRelatPath;

                    // Permutation new => old
                    pFolder.HardPath = pFolder.NewHardPath;
                    pFolder.RelatPath = pFolder.NewRelatPath;

                    // Raz new
                    pFolder.Raz_NewPaths();
                }
            }
            Trace.Unindent();
        }
        #endregion

        #region Resets
        /// <summary>
        /// Remet comme ça devrait être si Launchbox était le dossier root
        /// </summary>
        internal void ResetFactory()
        {
            HeTrace.WriteLine("Reset to factory parameters");

            // Remise à zéro des paramètres
            GamesFName = Default.AppsFolder;
            ImagesFName = Default.ImagesFolder;
            ManualsFName = Default.ManualsFolder;
            MusicsFName = Default.MusicsFolder;
            VideosFName = Default.VideosFolder;
            SystemFolderName = PlatformObject.Name;

            ChosenFolder = Global.LaunchBoxRoot;
            //PlatformObject.Folder = Path.Combine(Global.LaunchBoxRoot, "Games", PlatformObject.Name);

            //AlterPath();

        }

        internal void RAZ_DPaths()
        {
            HeTrace.WriteLine("Reset new Paths"); // Envnew
            foreach (C_PathsDouble c in PlatformPaths)
                c.Raz_NewPaths();
        }
        #endregion


        #region Validation


        /// <summary>
        /// Vérification du chemin entré
        /// </summary>        
        internal bool VerifPath(string pathD, string propertyName)
        {
            HeTrace.WriteLine(@"Folder verification"); // Envnew

            RemoveError(propertyName);

            bool result = true;
            string error = string.Empty;

            if (string.IsNullOrEmpty(pathD))
            {
                error = "Value can't be null";
                result = false;
            }
            else if (!Directory.Exists(pathD))
            {
                error = "Directory doesn't exist";
                result = false;
            }

            /*
             * todo
            Properties.Settings.Default.LastKPath = _NewRoot = this.tbMainPath.Text;
            Properties.Settings.Default.Save();
            */
            if (!result)
            {
                AddError(propertyName, error);
                // On notifie car ce n'est pas déclenché en cas d'update de la propriété
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            return result;
        }


        #endregion


        #region Errors

        public bool HasErrors => _erreurs.Any();

        /// <summary>
        /// Vérifie qu'un nom de dossier est correctement formaté
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        private void Validate_FolderName(string value, [CallerMemberName] string propertyName = null)
        {
            // bool needupdate = false;

            RemoveError(propertyName);

            // Tests
            if (string.IsNullOrEmpty(value))
            {
                AddError(propertyName, "Can't be a null or empty string");
                // needupdate = true;
            }

            if (Global.VerifString(value, Common.StringFormat.Folder))
            {
                AddError(propertyName, @"Folder name can't contain ?:/*\<|> charcacter");
                //   needupdate = true;
            }
        }


        /// <summary>
        /// Récupération de l'erreur concernant la propriété
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>        
        public IEnumerable GetErrors(string propertyName)
        {
            Debug.WriteLine(propertyName);

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

            // Surtout ne pas renvoyer null, ça sera pris comme une erreur
        }

        /// <summary>
        /// Ajoute une erreur au tableau des erreurs
        /// </summary>
        /// <param name="propertyName">Name of property</param>
        /// <param name="erreur">error description</param>
        private void AddError(string propertyName, string erreur)
        {
            // On ajoute au cas où ça serait manquant
            if (!_erreurs.ContainsKey(propertyName))
            {
                _erreurs[propertyName] = new List<string>();
            }

            _erreurs[propertyName].Add(propertyName + erreur);
        }

        /// <summary>
        /// Enlève une erreur en fonction du nom de la propriété
        /// </summary>
        /// <param name="propertyName"></param>

        private void RemoveError(string propertyName)
        {
            if (_erreurs.ContainsKey(propertyName))
            {
                // On enlève de la file
                _erreurs.Remove(propertyName);
                // On ne notifie plus que sur commande
                //ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            }
        }


        public void Dispose()
        {
            HeTrace.RemoveMessenger("Verbose");
        }
        #endregion
        /*
        */
    }
}
