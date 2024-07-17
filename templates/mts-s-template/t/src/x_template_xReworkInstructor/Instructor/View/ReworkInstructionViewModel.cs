using System;
using System.Windows;
using TcOpen.Inxton.Input;

namespace x_template_xReworkInstructor.Instructor.View
{
    public class ReworkInstructionViewModel
        
    {
        public ReworkInstructionViewModel()
        {

            RefreshReworkRecipeCommand = new RelayCommand(a => RefreshListOfReworkRecipes());
            ConfigCommand = new RelayCommand(a => OpenConfigurationView());
            UpdateCommand = new RelayCommand(a => UpdateInstructionsList());
            SaveCommand = new RelayCommand(a => Save());
            RemoveCommand = new RelayCommand(a => RemoveDeletedInstruction());
            Load();
        }



        public ReworkInstructionViewModel(ReworkInstructorController instructorController)
        {
            RefreshReworkRecipeCommand = new RelayCommand(a => RefreshListOfReworkRecipes());
            ConfigCommand = new RelayCommand(a => OpenConfigurationView());
            UpdateCommand = new RelayCommand(a => UpdateInstructionsList());
            SaveCommand = new RelayCommand(a => Save());
            RemoveCommand = new RelayCommand(a => RemoveDeletedInstruction());
            Controller = instructorController;
            Load();

        }

        private void RefreshListOfReworkRecipes()
        {
            Controller.RefreshReworkSet();
        }


        private void OpenConfigurationView()
        {
            var win = new Window() { Title = $"CONFIGURATION" };
            var configUi = new InstructionConfiguratorView() { DataContext = this };
            win.Content = configUi;
            win.ShowDialog();
        }

        


        private void Save()
        {
             if (!string.IsNullOrEmpty(Controller.OnlineData.HumanReadable))
            {
                Controller.SaveDataSet(Controller.OnlineData.HumanReadable);
            }
            
        }

        private void Load()
        {
         if (!string.IsNullOrEmpty(Controller.OnlineData.HumanReadable))
            {
                Controller.LoadDataSet(Controller.OnlineData.HumanReadable);
            }
        }


        private void UpdateInstructionsList()
        {
            Controller.UpdateFromTemplate();
        }

        /// <summary>
        /// Removes selected instruction item with deleted status
        /// </summary>
        private void RemoveDeletedInstruction()
        {
            {
                if (SelectedInstructionItem != null)
                {
                    switch (SelectedInstructionItem.Status)
                    {
                        case ReworkInstructionItemStatus.Deleted:
                            Controller.CurrentInstructionSet.RemoveRecord(SelectedInstructionItem);
                            break;
                        case ReworkInstructionItemStatus.Active:
                            MessageBox.Show("Unable to remove Item with active status!", "WARNING!");
                            break;
                    }
                }


            }
        }

        public ReworkInstructionItem SelectedInstructionItem { get; set; }


        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        public ReworkInstructorController Controller { get; set; }
        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand RefreshReworkRecipeCommand { get; private set; }
        public RelayCommand ConfigCommand { get; private set; }

    }
}

