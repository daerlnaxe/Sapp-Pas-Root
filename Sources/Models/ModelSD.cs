using SPR.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SPR.Models
{
    /*        
        Modèle source / Destination
     */
    public class ModelSD : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


        //   public PathType PathType { get; set; }

        public ModelSD(string title)
        {
            if (!string.IsNullOrEmpty(title))
                Title = title;
        }
        /*
        public ModelSD(PathType pathTtype, string title)
        {
            if (string.IsNullOrEmpty(title))
                Title = title;

            this.PathType = pathTtype;
        }*/

        private string _title;
        public string Title 
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();

            }
        }

        private string _source;
        public string Source
        {
            get { return _source; }
            set
            {
                Error_Source = string.Empty;
                _source = value;
                OnPropertyChanged();
            }
        }

        private string _destination;
        public string Destination
        {
            get { return _destination; }
            set
            {
                Error_Destination = string.Empty;
                _destination = value;
                OnPropertyChanged();
            }
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnErrorChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }


        private string _errSource;
        public string Error_Source
        {
            get { return _errSource; }
            set
            {
                _errSource = value;
                OnErrorChanged(nameof(Source));
            }
        }

        private string _errDestination;
        public string Error_Destination 
        {
            get { return _errDestination; }
            set
            {
                _errDestination = value;
                OnErrorChanged(nameof(Destination));
            }
        }

        public bool HasErrors => !(string.IsNullOrEmpty(Error_Source) && string.IsNullOrEmpty(Error_Destination));

        public IEnumerable GetErrors(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Source):
                    yield return Error_Source;
                    break;

                case nameof(Destination):
                    yield return Error_Destination;
                    break;

                default:
                    yield return string.Empty;
                    break;
            }
        }
    }
}
