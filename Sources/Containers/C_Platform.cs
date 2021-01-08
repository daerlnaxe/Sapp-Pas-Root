using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Containers
{
    public class C_Platform : IEnumerable<C_PathsDouble>
    {
        /// <summary>
        /// Path for application/path
        /// </summary>
        public C_PathsDouble ApplicationPath
        {
            get;
            set;
        }

        /// <summary>
        /// Path for manuel
        /// </summary>
        public C_PathsDouble ManuelPath
        {
            get;
            set;
        }

        /// <summary>
        /// Path for music
        /// </summary>
        public C_PathsDouble MusicPath
        {
            get;
            set;
        }

        /// <summary>
        /// Path for Video
        /// </summary>
        public C_PathsDouble VideoPath
        {
            get;
            internal set;
        }

        /// <summary>
        /// Paths for images
        /// </summary>
        public ObservableCollection<C_PathsDouble> ImagePaths { get; set; } = new ObservableCollection<C_PathsDouble>();

        /// <summary>
        /// 
        /// </summary>
        public int LongestCateg
        {
            get;
            private set;
        }


        /// <summary>
        /// Platform Name
        /// </summary>
        public string PlatformName { get; }

        public C_Platform()
        {
            /*ApplicationPath = new C_PathsCollec("apppath", "Appapath") ;
            ManuelPath = new C_PathsCollec("manuelpath", "");*/
        }

        /// <summary>
        /// construit un objet C_Platform à l'aide d'un objet d'interface IPlatform
        /// </summary>
        /// <param name="platform"></param>
        public C_Platform(IPlatform platform)
        {
            PlatformName = platform.Name;
        }

        #region Initialisation

        /// <summary>
        /// Initialise le folder Games
        /// </summary>
        /// <param name="platform"></param>
        public void BuildGamesFolder(IPlatform platform)
        {
            string oldPath = string.Empty;
            //  throw new Exception();
            // Attribution en fonction de l'allure du chemin
            if (string.IsNullOrEmpty(platform.Folder))
                oldPath = Path.Combine(Global.LaunchBoxRoot, "Games");
            else
                oldPath = platform.Folder;

            this.ApplicationPath = new C_PathsDouble(oldPath, "Games");

            /*
            //2020 levé pour unifier
            // Pour l'instant le chemin relatif aura une allure de chemin absolu si les lecteurs sont différents
            if (ApplicationPath.OldPath.StartsWith('.'))
            {
                ApplicationPath.HardPath = Path.GetFullPath(ApplicationPath.OldPath, Global.LaunchBoxRoot);
                ApplicationPath.RelatPath = ApplicationPath.OldPath;
            }
            else
            {
                ApplicationPath.HardPath = ApplicationPath.OldPath;
                ApplicationPath.RelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, ApplicationPath.OldPath);
                //ApplicationPath.OldRelatPath = Path.GetRelativePath(Global.LaunchBoxPath, ApplicationPath.OldPath);
            }

            ApplicationPath.Raz_NewPaths();*/
        }


        /// <summary>
        /// Initialise les folders en fonction dun tableau des platformFolders
        /// </summary>
        /// <param name="platformFolders"></param>
        public void BuildCategsFolders(IPlatformFolder[] platformFolders)
        {
            // CheatCodes non traité

            // Manuels
            /*IPlatformFolder iplManual = platformFolders.FirstOrDefault(x => x.MediaType.Equals("Manual"));
            if (iplManual != null)
                this.ManuelPath = new C_PathsCollec(iplManual.FolderPath, iplManual.MediaType);*/

            // Musics
            /*IPlatformFolder iplMusic = platformFolders.FirstOrDefault(x => x.MediaType.Equals("Music"));
            if (iplMusic != null)
                this.MusicPath = new C_PathsCollec(iplMusic.FolderPath, iplMusic.MediaType);*/

            // Videos
            /*IPlatformFolder iplVideo = platformFolders.FirstOrDefault(x => x.MediaType.Equals("Video"));
            if (iplVideo != null)
                this.VideoPath = new C_PathsCollec(iplVideo.FolderPath, iplVideo.MediaType);*/

            // On sait que c'est là que la catégorie aura le nom le plus long            
            // Images            
            //foreach (var p in _PlatformFolders)
            //for (int i = 3; i < platformFolders.Length; i++)

            // 
            for (int i = 0; i < platformFolders.Length; i++)

            {
                // 12/2020 il y avait une boucle inutile
                IPlatformFolder p = platformFolders[i];
                switch (p.MediaType)
                {
                    case "Manual":
                        this.ManuelPath = Get_PlatformFolder(p/*platformFolders*/, "Manual");
                        break;

                    case "Music":
                        this.MusicPath = Get_PlatformFolder(p/*latformFolders*/, "Music");
                        break;

                    case "Video":
                        this.VideoPath = Get_PlatformFolder(p/*latformFolders*/, "Video");
                        break;

                    default:
                        //C_PathsCollec tmp = new C_PathsCollec(p);
                        C_PathsDouble tmp = Get_PlatformFolder(p/*latformFolders*/, p.MediaType);
                        if (tmp.Type.Length > LongestCateg)
                            LongestCateg = tmp.Type.Length;
                        //            ExtPlatformPaths.Add(tmp);
                        this.AddImagePaths(tmp);

                        break;
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platformFolders"></param>
        /// <param name="categ"></param>
        /// <returns></returns>
        private C_PathsDouble? Get_PlatformFolder(IPlatformFolder ipl/*[] platformFolders*/, string categ)
        {
            //IPlatformFolder ipl = platformFolders.FirstOrDefault(x => x.MediaType.Equals(categ));

            if (ipl == null)
                return null;

            return new C_PathsDouble(ipl.FolderPath, ipl.MediaType);
        }
        #endregion Initialisation

        /// <summary>
        /// Add an image paths to observable collection
        /// </summary>
        /// <param name="tmp"></param>
        internal void AddImagePaths(C_PathsDouble tmp)
        {
            ImagePaths.Add(tmp);
        }


        #region iterators
        /*
        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return ApplicationPath;
            yield return ManuelPath;
            yield return MusicPath;
            yield return VideoPath;

            foreach (var imageP in ImagePaths)
            {
                yield return imageP;
            }
        }*/


        internal IEnumerable<C_PathsDouble> GetPaths()
        {
            // On ne renvoie pas AppPath
            yield return ManuelPath;
            yield return MusicPath;
            yield return VideoPath;

            foreach (var imageP in ImagePaths)
            {
                yield return imageP;
            }
        }

        public IEnumerator<C_PathsDouble> GetEnumerator()
        {
            yield return ApplicationPath;
            yield return ManuelPath;
            yield return MusicPath;
            yield return VideoPath;

            foreach (var imageP in ImagePaths)
            {
                yield return imageP;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    #endregion

}
