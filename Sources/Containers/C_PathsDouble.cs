using SPR.Languages;
using System;

namespace SPR.Containers
{
    /*
     *  Utilisé pour afficher les paths
     *  Remplace le path collec
     * 
     *  Edité pour être plus générique
     */
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Utilisé dans PlatformModel pour avoir la liste des chemins en old et new
    /// AppPaths en hérite.
    /// </remarks>
    public class C_PathsDouble : C_Paths
    {

        private string _newHardPath;
        /// <summary>
        /// Hard Path after modification
        /// </summary>
        public string NewHardPath
        {
            get { return _newHardPath; }
            set
            {
                _newHardPath = value;
                OnPropertyChanged();
            }
        }

        private string _newRelatPath;
        /// <summary>
        /// Relative Path after modification
        /// </summary>
        public string NewRelatPath
        {
            get { return _newRelatPath; }
            set
            {
                _newRelatPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indique si l'on peut permuter entre old et new
        /// </summary>
        public bool ToModify { get; set; } = false;

        /*
        /// <summary>
        /// 
        /// </summary>
        public C_PathsDouble()
        {
        }*/

        public C_PathsDouble()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationPath">Path Link</param>
        /// <param name="type">Type of Path</param>
        public C_PathsDouble(string applicationPath, string type) : base(applicationPath, type)
        {
            Raz_NewPaths();

        }


        /// <summary>
        /// Mise à zéro des newpaths
        /// </summary>
        internal void Raz_NewPaths()
        {
            NewHardPath = NewRelatPath = SPRLang.Waiting;
        }

        /// <summary>
        /// Test if path are similars
        /// </summary>
        /// <returns></returns>
        internal bool Test_Validity()
        {
            return HardPath.Equals(NewHardPath) && RelatPath.Equals(NewRelatPath);
        }
    }
}
