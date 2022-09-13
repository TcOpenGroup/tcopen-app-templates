using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using TcOpen.Inxton.Data;
using x_template_xPlc;
using x_template_xProductionPlaner.Generic;
using x_template_xProductionPlaner.Data.Handler;

namespace x_template_xProductionPlaner.Planer
{
    public class ProductionPlanController : INotifyPropertyChanged
    {
        public ProductionPlanController(RepositorySetDataHandler<ProductionItem> productionSetData, string configName, IRepository<PlainProcessData> repositorySource)
        {
            DataHandler = productionSetData;
            ConfigName = configName;
            RepositorySource = repositorySource;
            var items = RepositorySource.Queryable.Select(p => p._EntityId).ToList();
            RecipeCollection = new ObservableCollection<string>(items);
        }

        private ProductionItem currentItem;

        /// <summary>
        /// Gets or sets current item.
        /// </summary>
        public ProductionItem CurrentItem
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

        /// <summary>
        /// Gets current Production set.
        /// </summary>
        public EntitySet<ProductionItem> CurrentProductionSet { get; set; } = new EntitySet<ProductionItem>();

        /// <summary>
        /// Gets production of this 
        /// </summary>
        protected RepositorySetDataHandler<ProductionItem> DataHandler { get; }
        public string ConfigName { get; set; }
        public IRepository<PlainProcessData> RepositorySource { get; private set; }

        public bool ProductonPlanCompleted
        {
            get => productonPlanCompleted;
            set
            {
                if (productonPlanCompleted == value)
                {
                    return;
                }

                productonPlanCompleted = value;
                OnPropertyChanged(nameof(ProductonPlanCompleted));
            }
        }


        private readonly ProductionItem EmptyItem = new ProductionItem() { Status = EnumItemStatus.Empty };
        private bool productonPlanCompleted;

        /// <summary>
        /// When overriden performs update of <see cref="CurrentItem"/>.
        /// </summary>
        public void RefreshItems(out ProductionItem itm)
        {
            var item = CurrentProductionSet.Items.Where(p => p.Status == EnumItemStatus.Required || p.Status == EnumItemStatus.Active).FirstOrDefault();

            if (item != null)
            {
                CurrentItem = item;
                CurrentItem.ActualCount++;

                if (CurrentItem.ActualCount < CurrentItem.RequiredCount)
                    CurrentItem.Status = EnumItemStatus.Active;
                else if (CurrentItem.ActualCount >= CurrentItem.RequiredCount)
                    CurrentItem.Status = EnumItemStatus.Done;
            }
            else
            {
                CurrentItem = EmptyItem;

            }
            itm = CurrentItem;
            ProductonPlanCompleted = CurrentItem.Status == EnumItemStatus.Empty;
            SaveItemsSet(ConfigName);


        }

        public void Reinit()
        {
            CurrentProductionSet.Items.ToList().ForEach(p => p.ActualCount = 0);

        }

        /// <summary>
        /// Loads items set from the repository to this controller.
        /// </summary>
        /// <param name="itemsId">Instrucion set id.</param>
        public void LoadItemsSet(string itemsId)
        {
            var result = DataHandler.Repository.Queryable.FirstOrDefault(p => p._EntityId == itemsId);

            if (result == null)
            {
                DataHandler.Create(itemsId, CurrentProductionSet);
            }

            CurrentProductionSet = DataHandler.Read(itemsId);
        }

        /// <summary>
        /// Saves items set from this controller to the repository.
        /// </summary>
        /// <param name="itemsId">Instrucion set id.</param>
        public void SaveItemsSet(string itemsId)
        {
            var result = DataHandler.Repository.Queryable.FirstOrDefault(p => p._EntityId == itemsId);

            if (result == null)
            {
                DataHandler.Create(itemsId, CurrentProductionSet);
            }
            DataHandler.Update(itemsId, CurrentProductionSet);


        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ObservableCollection<string> RecipeCollection { get; }

    }
}
