using System;
using System.ComponentModel;
using System.Collections.Generic;
using TcOpen.Inxton.Data;

namespace TcoRepositorySetDataHandler
{
    public class EntitySet<T> : IBrowsableDataObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets instructions of this set.
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();

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
