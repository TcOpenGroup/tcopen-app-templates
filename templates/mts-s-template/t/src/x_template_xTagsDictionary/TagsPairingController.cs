using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TcOpen.Inxton.Data;
using TcOpen.Inxton.RepositoryDataSet;
using x_template_xPlc;

namespace x_template_xTagsDictionary
{
    public class TagsPairingController : INotifyPropertyChanged
    {
        public TagsPairingController(RepositoryDataSetHandler<TagItem> tagsDictionarySetData, string configName)
        {
            DataHandler = tagsDictionarySetData;
            ConfigName = configName;
       
           
        }

      

        private TagItem currentItem;

        /// <summary>
        /// Gets or sets current item.
        /// </summary>
        public TagItem CurrentItem
        {
            get => currentItem;
            set
            {
                if (currentItem == value)
                {
                    return;
                }

                currentItem = value;

                OnPropertyChanged(nameof(CurrentItem));
            }
        }

        internal void DeleteAll()
        {
            CurrentSet.Items.Clear();
            OnPropertyChanged("Items");
            OnPropertyChanged("CurrentSet");

        }

        /// <summary>
        /// Gets current set.
        /// </summary>
        public EntitySet<TagItem> CurrentSet { get; set; } = new EntitySet<TagItem>();

        /// <summary>
        /// Gets production of this 
        /// </summary>
        protected RepositoryDataSetHandler<TagItem> DataHandler { get; }
        public string ConfigName { get; set; }
        public EnumResultsStatus ResultsStatus { get; private set; }

        private readonly TagItem UnknowItem = new TagItem() { Status = EnumItemStatus.Unknown };

        /// <summary>
        /// Provide tag ingo from config collection. Result is stored in itm. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itm"></param>
        /// <param name="status"></param>
        public void GetTag( string key ,out TagItem itm, out EnumResultsStatus status )
        {
            LoadDataSet(ConfigName);
            status = EnumResultsStatus.TagNotFound;

            var item = CurrentSet.Items.Where(p => p.Key == key).FirstOrDefault();

            if (item != null)
            {
                if (item.Status == EnumItemStatus.Active && item.AssignedValue != string.Empty)
                {
                    CurrentItem = item;
                    CurrentItem.NumberOfUse++;
                    currentItem.LastUse = DateTime.Now;
                    status = EnumResultsStatus.TagFound;
                }
                else if (item.Status == EnumItemStatus.Active && item.AssignedValue == string.Empty)
                {
                    CurrentItem = item;
                    CurrentItem.NumberOfUse++;
                    currentItem.LastUse = DateTime.Now;
                    status = EnumResultsStatus.TagFoundAssignedValueEmpty;
                }
                else if (item.Status == EnumItemStatus.Inactive)
                {
                    CurrentItem = item;
                    status = EnumResultsStatus.TagFoundInactive;
                }
                else if (item.Status == EnumItemStatus.Unknown)
                {
                    CurrentItem = item;
                    status = EnumResultsStatus.TagFoundUnknown;
                }

            }
            else if (CurrentSet.Items.Count() == 0)
            {
                CurrentItem = UnknowItem;
                status = EnumResultsStatus.EmptyCollection;
            }
            else
            {
                CurrentItem = UnknowItem;
            }

            itm = CurrentItem;
            ResultsStatus = status;

            SaveDataSet(ConfigName);
            OnPropertyChanged(nameof(CurrentSet));
            OnPropertyChanged(nameof(ResultsStatus));



        }



        public void AddTag( TagItem itm, out EnumResultsStatus status)
        {
            LoadDataSet(ConfigName);

            var any =CurrentSet.Items.Any(p => p.Key == itm.Key);
            status = EnumResultsStatus.TagAddedNotSuccessfuly;

            if (any)
            {
                status = EnumResultsStatus.TagAlreadyExist;
            }
            else if (!string.IsNullOrEmpty(itm.Key))
            {
                status = EnumResultsStatus.TagAddedSuccessfully;

                CurrentSet.Items.Add(new TagItem()
                {   Key = itm.Key, 
                    Description = itm.Description,
                    FirstUse = DateTime.Now,
                    LastUse = DateTime.Now,
                    AssignedValue = itm.AssignedValue,
                    NumberOfUse = 1, Status = EnumItemStatus.Active 
               });
            }

            ResultsStatus = status;

            SaveDataSet(ConfigName);
            OnPropertyChanged(nameof(CurrentSet));
            OnPropertyChanged(nameof(ResultsStatus));

        }



        public void AddTag(TagItem itm)
        {
            EnumResultsStatus enumResults;

            AddTag(itm,out enumResults);


        }


        /// <summary>
        /// Loads items set from the repository to this controller.
        /// </summary>
        /// <param name="setid">set id.</param>
        public void LoadDataSet(string setid)
        {
            var result = DataHandler.Repository.Queryable.FirstOrDefault(p => p._EntityId == setid);

            if (result == null)
            {
                DataHandler.Create(setid, CurrentSet);
            }

            CurrentSet = DataHandler.Read(setid);
        }

        /// <summary>
        /// Saves items set from this controller to the repository.
        /// </summary>
        /// <param name="setId">Instrucion set id.</param>
        public void SaveDataSet(string setId)
        {

            if (!DataHandler.Repository.Queryable.Where(p => p._EntityId == setId).Any())
            {
                DataHandler.Create(setId, CurrentSet);
            }
            DataHandler.Update(setId, CurrentSet);


        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


     

    }
}
