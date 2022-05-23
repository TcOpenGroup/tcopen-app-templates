using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace TcOpen.Scaffold.UI
{
    public class MainWindowViewModel : Prism.Mvvm.BindableBase
    {
        private bool isScaffoling;

        public MainWindowViewModel()
        {
            // Checking for updates          
            ScaffoldCommand = new Prism.Commands.DelegateCommand(() => Exectute());
            SelectOutputFolderCommand = new Prism.Commands.DelegateCommand(() => SelectOutputFolder());
            IsNotScaffoling = true;
        }

        public string Version
        {
            get
            {
                return GitVersionInformation.SemVer;
            }
        }
        public bool IsNotScaffoling
        {
            get => isScaffoling;
            set { isScaffoling = value; this.RaisePropertyChanged(nameof(IsNotScaffoling)); }
        } 
        private async void Exectute()
        {
            IsNotScaffoling = false;
            var context = new Context(this.Options);
            await Task.Run(() => context.Execute());
            IsNotScaffoling = true;
        }
        private void SelectOutputFolder()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.Options.OutputDirectory = fbd.SelectedPath;
                }
            }
        }
        public Options Options { get; set; } = new Options() { Source = GitVersionInformation.SemVer, ProjectName = "MyProject", TemplateName = "mts-s-template" };
        public Prism.Commands.DelegateCommand ScaffoldCommand { get; }
        public Prism.Commands.DelegateCommand SelectOutputFolderCommand { get; }        
    }
}
