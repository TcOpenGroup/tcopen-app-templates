using System.ComponentModel;
using x_template_xProductionPlaner.Generic.Handler;

namespace x_template_xProductionPlaner.Planer
{
    public class ProductionItem : IItemsCollection
    {
        private string key;

        /// <summary>
        /// Gets or sets the key of this instruction item (list of process set).
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
            
            }
        }

        private int reqCount;

        /// <summary>
        /// Gets or sets required Ccounter value 
        /// </summary>
        public int RequiredCount
        {
            get => reqCount;
            set
            {
                if (reqCount == value)
                {
                    return;
                }

                reqCount = value;
              
            }
        }

        private int actualCount;
        /// <summary>
        /// Gets or sets actual counter value.
        /// </summary>
        public int ActualCount
        {
            get => actualCount;
            set
            {
                if (actualCount == value)
                {
                    return;
                }

                actualCount = value;
            }
        }

        private string description;
        private EnumItemStatus _status;

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
                
            }
        }

        /// <summary>
        /// Step HMI message
        /// </summary>
        public EnumItemStatus Status { get => _status; set { _status = value;  } }








    }
}
