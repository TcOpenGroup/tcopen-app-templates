
using System;
using System.Collections.ObjectModel;
using TcOpen.Inxton.Input;

namespace x_template_xTagsDictionary.View
{
    public class TagsPairingViewModel 
    {
        private TagItem _selectedItem;
     

        public TagsPairingViewModel
            
            (TagsPairingController controller)
        {
            Controller = controller;

            SaveCommand = new RelayCommand(a => Controller.SaveDataSet(Controller.ConfigName));
            AddCommand = new RelayCommand(a => { EnumResultsStatus res = default; Controller.AddTag(TagItem, out res);  });
            GetCommand = new RelayCommand(a => { EnumResultsStatus res = default; TagItem tag; Controller.GetTag(ReqKey,out tag, out res ); TagItem = tag; });
          
            Load();
        }


        private void Load()
        {
            Controller.LoadDataSet(Controller.ConfigName);
        }

       
       
        /// <summary>
        /// Actual selectected item
        /// </summary>
        public TagItem SelectedItem
        {
            get
            {
                if (_selectedItem == null)
                {
                    _selectedItem = new TagItem() { Status = EnumItemStatus.Unknown};
                }
                return _selectedItem;
            }
            set => _selectedItem = value;
        }

       


        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        public TagsPairingController Controller { get; set; }
        /// <summary>
        /// Update command
        /// </summary>
        public RelayCommand UpdateCommand { get; private set; }
        /// <summary>
        /// Savae command
        /// </summary>
        public RelayCommand SaveCommand { get; private set; }
        /// <summary>
        /// Add new command
        /// </summary>
        public RelayCommand AddCommand { get; private set; }
        /// <summary>
        /// Get tag command
        /// </summary>
        public RelayCommand GetCommand { get; private set; }
        public TagItem TagItem { get; set; } = new TagItem();
        public string ReqKey { get;  set; }
    }
}

