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
        /// Path for Video
        /// </summary>
        public C_PathsDouble ThemeVideoPath
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
        public string PlatformName { get; private set; }

        /*  public C_Platform()
          {
          }
        */
        /// <summary>
        /// construit un objet C_Platform à l'aide d'un objet d'interface IPlatform
        /// </summary>
        /// <param name="platform"></param>
        /*public C_Platform(IPlatform platform)
        {
        }*/

        #region Initialisation

        /// <summary>
        /// Initialise le folder Games
        /// </summary>
        /// <param name="platform"></param>
        public static C_Platform Platform_Maker(IPlatform platform, IPlatformFolder[] platformFolders)
        {
            // Attribution en fonction de l'allure du chemin
            string oldPath = string.Empty;
            if (string.IsNullOrEmpty(platform.Folder))
                oldPath = Path.Combine(Global.LaunchBoxRoot, "Games");
            else
                oldPath = platform.Folder;


            C_Platform visP = new C_Platform()
            {
                PlatformName = platform.Name,
                ApplicationPath = new C_PathsDouble(oldPath, "Games"),
            };

            visP.BuildCategsFolders(platformFolders);

            return visP;
        }


        /// <summary>
        /// Initialise les folders en fonction dun tableau des platformFolders
        /// </summary>
        /// <param name="platformFolders"></param>
        private void BuildCategsFolders(IPlatformFolder[] platformFolders)
        {

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

                    case "Theme Video":
                        this.ThemeVideoPath = Get_PlatformFolder(p, "Theme Video");
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

        internal IEnumerable<C_PathsDouble> GetPaths()
        {
            // On ne renvoie pas AppPath
            yield return ManuelPath;
            yield return MusicPath;
            yield return VideoPath;
            yield return ThemeVideoPath;

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
            yield return ThemeVideoPath;

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
