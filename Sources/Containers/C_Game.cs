using Hermes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Containers
{
    /*
     * - Utilisé comme représentation d'un jeu pour l'affichage
     * - Si un path est null, pas la peine de lui faire un path collec
     */
    public class C_Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Properties of IGame
        /// </summary>
        static PropertyInfo[] _GameClassProperty;

        #region Sans notif
        public string Title
        {
            get;
            private set;
        }

        public string Id { get; private set; }
        #endregion

        #region Avec Notifs
        private bool _IsValide;
        public bool IsValide
        {
            get { return _IsValide; }
            set
            {
                if (_IsValide == value)
                    return;

                _IsValide = value;
                OnPropertyChanged();
            }
        }
        #endregion


        /// <summary>
        /// Path for the application
        /// </summary>
        public C_PathsDouble ApplicationPath { get; set; }

        /// <summary>
        /// Manual Path
        /// </summary>
        public C_PathsDouble ManualPath { get; set; }

        /// <summary>
        /// Path for Music
        /// </summary>
        public C_PathsDouble MusicPath { get; set; }

        /// <summary>
        /// Path for Video
        /// </summary>
        public C_PathsDouble VideoPath { get; set; }

        /// <summary>
        /// Path for VideoPath
        /// </summary>
       // public C_PathsCollec ThemeVideoPath { get; private set; }



        /// <summary>
        /// Paths for Images
        /// </summary>
        List<C_PathsDouble> ImgPaths { get; set; } = new List<C_PathsDouble>();

        /// <summary>
        /// Additionnals roms paths to manage mixed roms mode
        /// </summary>
        internal HashSet<AAppPath> AddiRomPaths { get; set; } = new HashSet<AAppPath>();


        private string _OldHardPath;
        public string OldHardPath
        {
            get { return _OldHardPath; }
            set
            {
                _OldHardPath = value;
                OnPropertyChanged();
            }
        }

        ///<summary>
        /// Enumerate Paths
        ///</summary>         
        /// <remarks>Ne gère pas les images</remarks>
        public IEnumerable<C_PathsDouble> EnumGetPaths
        {
            get
            {
                yield return ApplicationPath;
                yield return ManualPath;
                yield return MusicPath;
                yield return VideoPath;
                //   yield return ThemeVideoPath;
            }
        }





        public C_Game(IGame game)
        {
            if (_GameClassProperty == null)
                C_Game.InitializeProperties();

            this.Id = game.Id;
            this.Title = game.Title;

            HeTrace.WriteLine($"- {game.Title}");
        }


        /// <summary>
        /// Initialize properties
        /// </summary>
        private static void InitializeProperties()
        {
            string prefix = "C_Game Buildpaths>> ";

            _GameClassProperty = typeof(IGame).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            HeTrace.WriteLine($"{prefix}Build properties");

            HeTrace.WriteLine($"{prefix}Information about properties", 2);


            // Indication à l'utilisateur de ce qui n'est pas traité
            foreach (var prop in _GameClassProperty)
            {
                switch (prop.Name)
                {
                    // Cas déjà traités
                    case "ApplicationPath":
                    case "ManualPath":
                    case "MusicPath":
                    case "VideoPath":
                    case "ThemeVideoPath":
                        HeTrace.WriteLine($"\t{prop.Name} is managed", 2);
                        continue;

                    // Cas que l'on ne traite pas pour le moment
                    case "Box3DOrNormalImagePath":
                    case "Cart3DOrNormalImagePath":
                        HeTrace.WriteLine($"\t!--- 3DO case - Not managed: {prop.Name}", 2);
                        continue;
                }

                if (prop.Name.Contains("Image"))
                {
                    HeTrace.WriteLine($"\t?--- Image case - Will manage: {prop.Name}", 2);
                    continue;
                }
                // Cas de ce qui contient Path
                else if (prop.Name.Contains("Path"))
                {
                    HeTrace.WriteLine($"\t!--- Path case - Not managed: - {prop.Name}", 5);
                    //Debug.WriteLine($"---- => Don't manage Path property: - {prop.Name}");
                }
                // Cas du reste
                else
                {
                    //verb += $"C_Game >> Don't manage other cases property: - {prop.Name}" + Environment.NewLine;
                    HeTrace.WriteLine($"\t!--- Other cases - Not managed: - {prop.Name}", 5);
                }
            }
        }

        /// <summary>
        /// Construction des chemins
        /// </summary>
        /// <param name="game"></param>
        /// <returns>
        /// Retourne une chaine indiquant le déroulement
        /// </returns>
        public void BuildPaths(IGame game)
        {
            /*
             * Rajouter
             * Hidden ?
             * Broken ? 
             * LaunchBoxDbId ?
             * Il y a un problème qui ne vient pas de moi sur la propriété PlatformClearLogoImagePath
             */

            #region Présents dans platform.xml
            // Si on veut les passer en premier

            // Avant on mettait null, maintenant si c'est null on va le passe en chaine vide car c'est ce que fait Launchbox
            ApplicationPath = string.IsNullOrEmpty(game.ApplicationPath) ? null : new C_PathsDouble(game.ApplicationPath, nameof(game.ApplicationPath));
            ManualPath = string.IsNullOrEmpty(game.ManualPath) ? null : new C_PathsDouble(game.ManualPath, nameof(game.ManualPath));
            MusicPath = string.IsNullOrEmpty(game.MusicPath) ? null : new C_PathsDouble(game.MusicPath, nameof(game.MusicPath));
            VideoPath = string.IsNullOrEmpty(game.VideoPath) ? null : new C_PathsDouble(game.VideoPath, nameof(game.VideoPath));

            // ThemeVideoPath = game.VideoPath == null ? null : new C_PathsCollec(game.ThemeVideoPath, nameof(game.ThemeVideoPath));

            // theme video ?
            #endregion
            /*
            this.OldRelatPath = game.ApplicationPath;
            this.OldHardPath = */



            string valeur = string.Empty;

            //Tout le reste sauf additionnal apps            
            foreach (PropertyInfo prop in _GameClassProperty)
            {

                try
                {
                    valeur = string.Empty;

                    switch (prop.Name)
                    {
                        case "Id":
                        case "Title":
                            break;

                        case "ApplicationPath":
                        /*valeur = prop.GetValue(game) as string;
                        if (!string.IsNullOrEmpty(valeur))
                            ExtPaths.Insert(0, new C_PathsCollec(valeur, prop.Name));
                        break;*/

                        case "ManualPath":
                        case "MusicPath":
                        case "VideoPath":
                            /*valeur = prop.GetValue(game) as string;
                            if (string.IsNullOrEmpty(valeur))
                                ExtPaths.Add(new C_PathsCollec(valeur, prop.Name));*/
                            break;

                        // Engendre erreur
                        case "PlatformClearLogoImagePath":
                            break;

                        // Cas non managés pour le moment
                        case "Box3DOrNormalImagePath":
                        case "Cart3DOrNormalImagePath":
                            break;

                        default:

                            // Cas des images
                            if (prop.Name.Contains("Image"))
                            {
                                valeur = prop.GetValue(game, null) as string;

                                if (!string.IsNullOrEmpty(valeur))
                                    ImgPaths.Add(new C_PathsDouble(valeur, prop.Name));
                            }
                            // Pour le moment on ne traite pas le reste
                            break;
                    }

                }
                catch (Exception exc)
                {
                    // bug connu
                    if (prop.Name.Equals("PlatformClearLogoImagePath"))
                        HeTrace.WriteLine("\t[BuildPaths]C_Game >> Known bug handled on PlatformClearLogoImagePath");
                    else
                        HeTrace.WriteLine($"\t[BuildPaths]{prop.Name}/{prop.PropertyType}: exc.Message");
                }

            }

            //return verb;


            // Récupération des apps en plus
            var test = game.GetAllAdditionalApplications();
            foreach (IAdditionalApplication addiApp in game.GetAllAdditionalApplications())
            {
                /* Fitre sur les ids, si l'application a le même id que le jeu on conserve pour
                 * changer les paths. Choix pour le moment, pour ne garder que les jeux, en attendant de
                 * voir ce que ça donne au niveau de l'emploi des applications additionnelles dans Launchbox
                */

                // 2020 j'ai l'impresson que ça ne sert à rien le jeu s'occupe justement de ne pas récupérer ce qui n'a pas de rapport.
                // a voir
                /*if (addiApp.GameId != game.Id)
                    continue;*/

                AddiRomPaths.Add(new AAppPath(addiApp.Id, addiApp.ApplicationPath));

                //Add_AdditionnalApplication(new );
                // MessageBox.Show($"{srcGame.Id} subid:{addiApp.Id}: {addiApp.ApplicationPath}");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPath">Chemin de la propriété </param>
        /// <param name="pName">Nom de la propriété</param>
        /// <returns></returns>
        [Obsolete]
        private C_PathsDouble BuildPath(string pPath, string pName, string pDir, IGame game)
        {
            return null;
        }
    }
}
