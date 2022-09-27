
using System;
using System.Collections.ObjectModel;
using TcOpen.Inxton.Input;

namespace x_template_xProductionPlaner.Planer.View
{
    public class ProductionPlanViewModel 
    {
        private ProductionItem _selectedItem;

        public ProductionPlanViewModel(ProductionPlanController controller)
        {
            Controller = controller;

            UpdateCommand = new RelayCommand(a => UpdateList());
            SaveCommand = new RelayCommand(a => Controller.SaveDataSet(Controller.ConfigName));
            ReinitCommand = new RelayCommand(a => ReinitValues());
            DeleteAllCommand = new RelayCommand(a => DeleteAll());
            DeleteCommand = new RelayCommand(a => Delete());
            RefreshRecipeSourceCommand = new RelayCommand(a => Controller.RefreshSourceRecipeList());
            UpCommand = new RelayCommand(a => Up());
            DownCommand = new RelayCommand(a => Down());

            Load();
        }

        private void DeleteAll()
        {
            Controller.CurrentProductionSet.Items.Clear();
        }

        private void Delete()
        {
            if (SelectedItem != null)
            {

                Controller.CurrentProductionSet.RemoveRecord(SelectedItem);

            }
        }

        private void UpdateList()
        {
            ProductionItem result = null;
            Controller.RefreshItems(out result);
            Console.WriteLine(result);
        }


        private void Load()
        {
            Controller.LoadDataSet(Controller.ConfigName);
        }

        /// <summary>
        /// Provide Collection of recipe
        /// </summary>
        public ObservableCollection<string> RecipeCollection => Controller.RecipeCollection;


        /// <summary>
        /// Set all countes values in collection to zero 
        /// </summary>
        private void ReinitValues()
        {
            Controller.Reinit();
        }
        /// <summary>
        /// Actual selectected item
        /// </summary>
        public ProductionItem SelectedItem{
            get
            {
                if (_selectedItem == null)
                {
                    _selectedItem = new ProductionItem() { Status = EnumItemStatus.Required};
                }
                return _selectedItem;
            }
            set => _selectedItem = value; }

        private void Up()
        {
            var currentIndex = Controller.CurrentProductionSet.Items.IndexOf(SelectedItem);

            if (currentIndex > 0)
            {
                int upIndex = currentIndex - 1;
                //move the items
                var oc = new ObservableCollection<ProductionItem>(Controller.CurrentProductionSet.Items);

                oc.Move(upIndex, currentIndex);
                Controller.CurrentProductionSet.Items = oc;
            }
        }
        private void Down()
        {
            var currentIndex = Controller.CurrentProductionSet.Items.IndexOf(SelectedItem);

            if (currentIndex + 1 < Controller.CurrentProductionSet.Items.Count)
            {
                int downIndex = currentIndex + 1;
                //move the items
                var oc = new ObservableCollection<ProductionItem>(Controller.CurrentProductionSet.Items);

                oc.Move(downIndex, currentIndex);
                Controller.CurrentProductionSet.Items = oc;
            }
        }


        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        public ProductionPlanController Controller { get; set; }
        /// <summary>
        /// Update command
        /// </summary>
        public RelayCommand UpdateCommand { get; private set; }
        /// <summary>
        /// Savae command
        /// </summary>
        public RelayCommand SaveCommand { get; private set; }
        /// <summary>
        /// Reinit command
        /// </summary>
        public RelayCommand ReinitCommand { get; private set; }
        /// <summary>
        /// Delete all  items in collectio command
        /// </summary>
        public RelayCommand DeleteAllCommand { get; private set; }
        /// <summary>
        /// delete single item in collection command
        /// </summary>
        public RelayCommand DeleteCommand { get; private set; }
        /// <summary>
        /// update collection of recipes 
        /// </summary>
        public RelayCommand RefreshRecipeSourceCommand { get; private set; }

        /// <summary>
        /// Change position(priority) of item in collection - direction up
        /// </summary>
        public RelayCommand UpCommand { get; private set; }
        /// <summary>
        /// Change position(priority) of item in collection - direction down
        /// </summary>
        public RelayCommand DownCommand { get; private set; }

    }
}

