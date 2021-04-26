using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SPR.Models
{
    internal abstract class AChooseFolder: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void StringValueChanged(string resultFolder);
        public event StringValueChanged ResultFolderChanged;




        /// <summary>
        /// Message à afficher en haut pour décrire l'action
        /// </summary>
        public abstract string Info { get; }

        /// <summary>
        /// Folder to start exploration
        /// </summary>
        public abstract string StartingFolder { get; set; }

        public abstract void Browse_Executed(string linkResult);


        private string _resultFolder;
        /// <summary>
        /// Dossier choisi
        /// </summary>
        public string ResultFolder
        {
            get { return _resultFolder; }
            set
            {
                _resultFolder = value;
                ResultFolderChanged?.Invoke(_resultFolder);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResultFolder"));
            }
        }
    }
}
