using System.IO;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Containers
{
    /*
     * Utilisé pour le débug
     */
    public class MvFolder : IPlatformFolder
    {
        public string Platform { get; set; }
        public string MediaType { get; set; }

        /// <summary>
        /// LaunchBox relative link - original
        /// </summary>
        public string FolderPath { get; set; }

        #region extensions
        /// <summary>
        /// New relative link -> destination
        /// </summary>
        public string NewFolderPath { get; set; }

        /// <summary>
        /// Old Hard folder path
        /// </summary>
        public string HFolderPath { get; set; }
        /// <summary>
        /// New Hard folder path
        /// </summary>
        public string HNewFolderPath { get; set; }
        #endregion


        public MvFolder() { }

        public MvFolder(IPlatformFolder src, string LaunchBoxRoot)
        {
            Platform = src.Platform;
            MediaType = src.MediaType;
            FolderPath = src.FolderPath;
            HFolderPath = Path.GetFullPath(Path.Combine(LaunchBoxRoot, FolderPath));

            //todo NewFolderPath = Languages.Lang.Waiting;
            // todo HNewFolderPath = Languages.Lang.Waiting;
        }

        public MvFolder(string CurrentFolder, string LaunchBoxRoot)
        {
            FolderPath = CurrentFolder;
            HFolderPath = Path.GetFullPath(Path.Combine(LaunchBoxRoot, CurrentFolder));
            //todo NewFolderPath = HNewFolderPath = Languages.Lang.Waiting;
        }

        /* public MvFolder(MvFolder src)
         {
             Platform = src.Platform;
             MediaType = src.MediaType;
             FolderPath = src.FolderPath;
             NewPath = src.NewPath;
             NewPath = "src.FolderPath";// bug put to src.NewPath after test
         }*/

        /// <summary>
        /// Convertit des interfaces de platform Folder en un objet local
        /// </summary>
        /// <remarks>
        /// Le hardlink est recréé
        /// </remarks>
        /// <param name="ArrPlatFolder"></param>
        /// <param name="PlatformFolder">Permet de récupérer le dossier de l'application</param>
        /// <param name="LaunchBoxRoot">La racine du programme</param>
        /// <returns></returns>
        public static MvFolder[] Convert(IPlatformFolder[] ArrPlatFolder, string PlatformFolder, string LaunchBoxRoot)
        {
            // On ajoute 1 pour introduire le dossier de jeu
            MvFolder[] retMVF = new MvFolder[ArrPlatFolder.Length + 1];

            retMVF[0] = new MvFolder(PlatformFolder, LaunchBoxRoot);
            retMVF[0].MediaType = "Game";
            retMVF[0].Platform = ArrPlatFolder[0].Platform;

            for (int i = 1; i < retMVF.Length; i++)
            {
                retMVF[i] = new MvFolder(ArrPlatFolder[i - 1], LaunchBoxRoot);
            }

            return retMVF;
        }
    }
}