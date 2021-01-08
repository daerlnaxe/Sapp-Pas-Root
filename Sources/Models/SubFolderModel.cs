using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SPR.Models
{
    public class SubFolderModel : INotifyDataErrorInfo
    {

        //public event PropertyChangedEventHandler PropertyChanged;

        public delegate void StringChanged(string value, string property);
        public event StringChanged ValueChanged;

        private void OnStringChanged(string value, string property)
        {
            ValueChanged?.Invoke(value, property);
        }

        /*public event StringChanged GamesChanged;
public event StringChanged ManualsChanged;
public event StringChanged ImagesChanged;
public event StringChanged Changed;*/

        /*
        private void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }*/

        /// <summary>
        /// Affiche un message en haut pour indiquer
        /// </summary>
        public string Info { get; set; }

        // Affiche un message au dessus des infos pour expliquer le fonctionnement
        public string ToolTipInfo { get; set; }

        private string _gamesFName = Properties.Settings.Default.AppsFolder;
        /// <summary>
        /// Nom du dossier de jeux
        /// </summary>
        public string GamesFName
        {
            get { return _gamesFName; }
            set
            {
                _gamesFName = value;
                Validate_FolderName(value);
                // OnPropertyChanged();
            }
        }

        private string _manualsFName = Properties.Settings.Default.ManualsFolder;
        /// <summary>
        /// Nom du dossier Manuals
        /// </summary>
        public string ManualsFName
        {
            get { return _manualsFName; }
            set
            {
                _manualsFName = value;
                Validate_FolderName(value);
            }
        }

        private string _imagesFName = Properties.Settings.Default.ImagesFolder;
        /// <summary>
        /// Nom du dossier Images
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

        private string _musicsFName = Properties.Settings.Default.MusicsFolder;
        /// <summary>
        /// Nom du dossier Musique
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


        private string _videosFName = Properties.Settings.Default.VideosFolder;
        /// <summary>
        /// Nom du dossier Video
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



        #region Validation

        /// <summary> 
        /// Vérifie qu'un nom de dossier est correctement formaté (modif)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>        
        private void Validate_FolderName(string value, [CallerMemberName] string propertyName = null)
        {
            bool noError = true;
            RemoveError(propertyName);

            // Tests
            if (string.IsNullOrEmpty(value))
            {
                AddError("Can't be a null or empty string", propertyName);
                noError &= false;
            }

            if (Global.VerifString(value, Common.StringFormat.Folder))
            {
                AddError(@"Folder name can't contain ?:/*\<|> charcacter", propertyName);
                noError &= false;
            }

            // Pas d'erreur, on peut signaler le changement
            if (noError)
                OnStringChanged(value, propertyName);

        }

        #endregion


        #region errors (copie)

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


        /// <summary>
        /// All Errors
        /// </summary>
        private readonly Dictionary<string, List<string>> _erreurs = new Dictionary<string, List<string>>();

        public bool HasErrors => _erreurs.Any();

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
    }
}
