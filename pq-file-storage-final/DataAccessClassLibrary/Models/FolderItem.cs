using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccessClassLibrary.Models
{
    public class FolderItem : INotifyPropertyChanged
    {
        // private variable declarations and initialization
        private string _name = string.Empty;
        private string _icon = string.Empty;
        private string _path = string.Empty;
        private ObservableCollection<FileItem> _files = new ObservableCollection<FileItem>();
        private ObservableCollection<object> _items = new ObservableCollection<object>();

        /* 
         * public properties with OnPropertyChanged event
         * notify the UI when the property value changes
         * using the INotifyPropertyChanged interface
         * and the OnPropertyChanged method
         * with optional propertyName parameter
         * to specify the property name that changed
        */
        // getters and setters for private variables
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FileItem> Files
        {
            get => _files;
            set
            {
                _files = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<object> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        // INotifyPropertyChanged event handler
        public event PropertyChangedEventHandler? PropertyChanged;

        // OnPropertyChanged method to raise the PropertyChanged event
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}