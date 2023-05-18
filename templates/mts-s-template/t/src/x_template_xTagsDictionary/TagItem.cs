using System;
using System.ComponentModel;
using TcOpen.Inxton.RepositoryDataSet;

namespace x_template_xTagsDictionary
{
    public class TagItem : INotifyPropertyChanged, IDataSetItems
    {
        private string key;

        /// <summary>
        /// Gets or sets the key of this instruction item (list of process set).
        /// </summary>
        public string Key
        {
            get => key;
            set
            {
                if (key == value)
                {
                    return;
                }

                key = value;
                NotifyPropertyChange(nameof(Key));
            }
        }
        /// <summary>
        /// This represents assigned paired value to unique key
        /// </summary>
        public string AssignedValue { get => assignedValue; set { assignedValue = value; NotifyPropertyChange(nameof(AssignedValue)); } }


        private string description;
        private EnumItemStatus _status;
        private string assignedValue;
        private DateTime firstUse;
        private DateTime lastUse;
        private uint numberOfUse;

        /// <summary>
        /// gets or sets additional information. 
        /// </summary>
        public string Description
        {
            get => description;
            set
            {
                if (description == value)
                {
                    return;
                }

                description = value;
                NotifyPropertyChange(nameof(Description));
            }
        }



        /// <summary>
        /// Status
        /// </summary>
        public EnumItemStatus Status { get => _status; set { _status = value; NotifyPropertyChange(nameof(Status)); } }

        /// <summary>
        /// NumberOfUse
        /// </summary>
        public uint NumberOfUse { get => numberOfUse; set { numberOfUse= value; NotifyPropertyChange(nameof(NumberOfUse)); } }

        /// <summary>
        /// First use
        /// </summary>
        public DateTime FirstUse { get => firstUse; set { firstUse = value; NotifyPropertyChange(nameof(FirstUse)); } }


        /// <summary>
        /// Last use
        /// </summary>
        public DateTime LastUse { get => lastUse; set { lastUse = value; NotifyPropertyChange(nameof(LastUse)); } }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




    }
}
