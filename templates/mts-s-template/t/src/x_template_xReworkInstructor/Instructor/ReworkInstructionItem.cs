using System;
using System.ComponentModel;
using TcOpen.Inxton.RepositoryDataSet;

namespace x_template_xReworkInstructor.Instructor
{
    public class ReworkInstructionItem
        : INotifyPropertyChanged, IEquatable<ReworkInstructionItem>, IDataSetItems
    {

        string key;
        /// <summary>
        /// Gets or sets the key of this instruction item.
        /// </summary>
        public string Key
        {
            get
            {
                return key;
            }
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
        /// Gets or sets the key description of this instruction item.
        /// </summary>
        public string KeyDescription
        {
            get
            {
                return keyDescription;
            }
            set
            {
                if (keyDescription == value)
                {
                    return;
                }

                keyDescription = value;
                NotifyPropertyChange(nameof(KeyDescription));
            }
        }
        string primaryDescription;

        /// <summary>
        /// Gets or sets arbitrary description of this instruction.
        /// </summary>
        public string PrimaryDescription
        {
            get
            {
                return primaryDescription;
            }
            set
            {
                if (primaryDescription == value)
                {
                    return;
                }

                primaryDescription = value;
                NotifyPropertyChange(nameof(PrimaryDescription));
            }
        }

        /// <summary>
        /// Gets or sets arbitrary description of this instruction.
        /// </summary>
        public string SecondaryDescription
        {
            get
            {
                return secondaryDescription;
            }
            set
            {
                if (secondaryDescription == value)
                {
                    return;
                }

                secondaryDescription = value;
                NotifyPropertyChange(nameof(SecondaryDescription));
            }
        }
        public string ReworkName
        {
            get
            {
                return reworkName;
            }
            set
            {
                if (reworkName == value)
                {
                    return;
                }

                reworkName = value;
                NotifyPropertyChange(nameof(ReworkName));
            }
        }

       

        string contentSource;
        private string secondaryDescription;
        private ReworkInstructionItemStatus status;
        private string keyDescription;
        private string reworkName;
        private ulong counter;
        private string description;

        /// <summary>
        /// Content source (path to image) of this instruction item.
        /// </summary>
        public string ContentSource
        {
            get
            {
                return contentSource;
            }
            set
            {
                if (contentSource == value)
                {
                    return;
                }

                contentSource = value;
                NotifyPropertyChange(nameof(ContentSource));
            }
        }


        public ReworkInstructionItemStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                if (status == value)
                {
                    return;
                }

                status = value;
                NotifyPropertyChange(nameof(Status));
            }
        }

        public string Description { get => description; set{ description = value;
                NotifyPropertyChange(nameof(Description)); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(ReworkInstructionItem other)
        {
            if (other == null)
                return false;

            if (this.Key == other.Key)
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            ReworkInstructionItem personObj = obj as ReworkInstructionItem;
            if (personObj == null)
                return false;
            else
                return Equals(personObj);
        }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }
    }
}
