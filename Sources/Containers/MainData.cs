using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;
using System.Collections;
using System.Linq;

namespace SPR.Containers
{
    internal class MainDatas : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        // --- Games

        private string _games;
        public string Games
        {
            get { return _games; }
            set
            {
                _games = value;
                OnPropertyChanged();
            }
        }

        // --- Manuals
        private string _manuals;
        public string Manuals
        {
            get { return _manuals; }
            set
            {
                _manuals = value;
                OnPropertyChanged();
            }
        }


        // --- Images
        private string _images;
        public string Images
        {
            get { return _images; }
            set
            {
                _images = value;
                OnPropertyChanged();
            }
        }


        // --- Musics 
        private string _musics;
        public string Musics
        {
            get { return _musics; }
            set
            {
                _musics = value;
                OnPropertyChanged();
            }
        }

        // --- Videos
        private string _videos;
        public string Videos
        {
            get { return _videos; }
            set
            {
                _videos = value;
                OnPropertyChanged();
            }
        }


        public MainDatas()
        {

        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return Games;
            yield return Manuals;
            yield return Images;
            yield return Musics;
            yield return Videos;
        }

        public IEnumerable<KeyValuePair<string, string>> GetPairs()
        {
            yield return new KeyValuePair<string, string>(nameof(Games), Games);
            yield return new KeyValuePair<string, string>(nameof(Manuals), Manuals);
            yield return new KeyValuePair<string, string>(nameof(Images), Images);
            yield return new KeyValuePair<string, string>(nameof(Musics), Musics);
            yield return new KeyValuePair<string, string>(nameof(Videos), Videos);
        }

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
        internal void AddError(string erreur, [CallerMemberName] string propertyName = null)
        {
            // On ajoute au cas où ça serait manquant
            if (!_erreurs.ContainsKey(propertyName))
            {
                _erreurs[propertyName] = new List<string>();
            }

            _erreurs[propertyName].Add($"{propertyName}: {erreur}");
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Enlève une erreur en fonction du nom de la propriété
        /// </summary>
        /// <param name="propertyName"></param>
        internal void RemoveError([CallerMemberName] string propertyName = null)
        {
            if (_erreurs.ContainsKey(propertyName))
                // On enlève de la file
                _erreurs.Remove(propertyName);
        }

        #endregion
    }
}
