using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using TcOpen.Inxton.Data;
using System.Collections.Generic;

namespace x_template_xProductionPlaner.Generic
{
    public class EntitySet<T> : IBrowsableDataObject, INotifyPropertyChanged
    {
        private IList<T> items = new List<T>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets instructions of this set.
        /// </summary>
        public IList<T> Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChange(nameof(Items));
            }
        }
        public void AddRecord(T item)
        {
            Items.Add(item);
        }

        public void RemoveRecord(T item)
        {
            Items.Remove(item);

        }


        public dynamic _recordId { get; set; }
        public DateTime _Created { get; set; }
        public string _EntityId { get; set; }
        public DateTime _Modified { get; set; }

    }
}
