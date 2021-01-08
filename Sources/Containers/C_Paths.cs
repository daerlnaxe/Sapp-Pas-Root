using DxPaths.Windows;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SPR.Containers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Utilisé pour la liste des dossiers de plateforme dans GamesModel
    /// </remarks>
    public class C_Paths : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Chemin passé en paramètre
        /// </summary>
        public string OldPath { get; private set; }

        /// <summary>
        /// Type de chemin
        /// </summary>
        public string Type { get; set; }

        private string _HardPath;
        /// <summary>
        /// Hard Path before modification
        /// </summary>
        public string HardPath
        {
            get { return _HardPath; }
            set
            {
                _HardPath = value;
                OnPropertyChanged();
            }
        }

        private string _RelatPath;
        /// <summary>
        /// Relat Path before modification
        /// </summary>
        public string RelatPath
        {
            get { return _RelatPath; }
            set
            {
                _RelatPath = value;
                OnPropertyChanged();
            }
        }


        public C_Paths()
        {

        }


        public C_Paths(string applicationPath, string type)
        {
            Type = type;

            if (string.IsNullOrEmpty(applicationPath))
                return;

            OldPath = applicationPath;

            FormatPath(applicationPath, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationPath"></param>
        /// <param name="type"></param>
        internal void FormatPath(string applicationPath, string type)
        {
            // Pour l'instant le chemin relatif aura une allure de chemin absolu si les lecteurs sont différents
            if (OldPath.StartsWith('.'))
            {
                HardPath = Path.GetFullPath(OldPath, Global.LaunchBoxRoot);
                RelatPath = OldPath;
            }
            // Hardlink 
            else if (OldPath.Contains(':'))
            {
                HardPath = OldPath;
                RelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, OldPath);
                // OldRelatPath = Path.GetRelativePath(Global.LaunchBoxPath, OldPath);
            }
            // Cas spécial quand ça commence par un nom de folder (Games ne l'utiliserait pas)
            // Si c'est vide ça le transforme en .\
            else
            {
                /*OldHardPath = OldPath;
                OldRelatPath = DxPath.ToRelative(Global.LaunchBoxRoot, OldPath);*/
                // Cas de figure ou c'est à la racine de Launchbox
                HardPath = Path.Combine(Global.LaunchBoxRoot, OldPath);
                //OldRelatPath = @$".\{OldPath}";
                RelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, HardPath);
            }
        }


    }
}
