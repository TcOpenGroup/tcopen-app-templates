using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TcOpen.Inxton.Input;
using TcOpen.Inxton.Instructor;
using x_template_xInstructor.Configurator;

namespace x_template_xInstructor
{
    public class InstructorViewModel
    {
        public InstructorViewModel()
        {

            ConfigCommand = new RelayCommand(a => OpenConfigurationView());
            UpdateCommand = new RelayCommand(a => UpdateInstructionsList());
            SaveCommand = new RelayCommand(a => Save());
            RemoveCommand = new RelayCommand(a => RemoveDeletedInstruction());
        }


        public InstructorViewModel(InstructorController instructorController)
        {
            ConfigCommand = new RelayCommand(a => OpenConfigurationView());
            UpdateCommand = new RelayCommand(a => UpdateInstructionsList());
            SaveCommand = new RelayCommand(a => Save());
            RemoveCommand = new RelayCommand(a => RemoveDeletedInstruction());
            this.Controller = instructorController;
            this.Load();

        }


        private void OpenConfigurationView()
        {
            var win = new Window() { Title = $"CONFIGURATION - {this.Controller.InstructionControlProvider.ProviderId }" };
            var configUi = new ConfigurationView() { DataContext = this };
            win.Content = configUi;
            win.ShowDialog();
        }

        private void Save()
        {
            Controller.SaveInstructionSet(Controller.InstructionControlProvider.ProviderId);
        }

        private void Load()
        {
            Controller.LoadInstructionSet(Controller.InstructionControlProvider.ProviderId);
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
                        case enumInstructionItemStatus.Deleted:
                            Controller.CurrentInstructionSet.RemoveRecord(SelectedInstructionItem);
                            break;
                        case enumInstructionItemStatus.Active:
                            MessageBox.Show("Unable to remove Item with active status!", "WARNING!");
                            break;
                    }
                }


            }
        }

        public InstructionItem SelectedInstructionItem { get; set; }


        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        public InstructorController Controller { get; set; }
        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand ConfigCommand { get; private set; }

    }
}

