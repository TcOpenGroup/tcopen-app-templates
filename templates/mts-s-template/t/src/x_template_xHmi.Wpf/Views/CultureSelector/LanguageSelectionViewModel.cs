
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using x_template_xHmi.Wpf.Properties;
using System.Globalization;
using System.Windows.Input;
using TcOpen.Inxton.Input;

namespace x_template_xHmi.Wpf
{
    public class LanguageSelectionViewModel
        
    {
        public LanguageSelectionViewModel()
        {
            ChangeCommand = new RelayCommand(a => ChangeCulture());
        }

        public List<ApplicationCulture> AvailableCultures { get; set; } = new List<ApplicationCulture>();

        public void AddCulture(string culture,string sourceImage)
        {
            AvailableCultures.Add(new ApplicationCulture() { Culture = culture, SourceImage = sourceImage });
        }
        
        public ApplicationCulture SelectedCulture { get; set; }
        public RelayCommand ChangeCommand { get; private set; }

        public void ChangeCulture()
        {

            if (SelectedCulture == null)
            {
                MessageBox.Show(strings.LanguageMessageBoxSelect, strings.LanguageMessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }  

            var result = MessageBox.Show(strings.LanguageMessageBoxText, strings.LanguageMessageBoxCaption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            Settings.Default.Culture = SelectedCulture.Culture;
            Settings.Default.Save();

            MainWindow.IsRestarting = true;
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }

    public class ApplicationCulture
    {
        public string Culture { get; set; }

        public string SourceImage { get; set; }



    }

}
