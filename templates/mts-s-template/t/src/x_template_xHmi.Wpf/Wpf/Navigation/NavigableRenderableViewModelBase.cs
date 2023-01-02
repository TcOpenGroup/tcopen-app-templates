using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vortex.Presentation.Wpf;

namespace x_template_xHmi.Wpf
{
    public abstract class NavigableRenderableViewModelBase : RenderableViewModel, INavigable
    {
        public NavigableRenderableViewModelBase() : base()
        {
            ToggleMenuCommand = new TcOpen.Inxton.Input.RelayCommand(o => ToggleMenu());            
        }

        FrameworkElement _CurrentView;

        public FrameworkElement CurrentView
        {
            get { return _CurrentView; }
            set { SetProperty(ref _CurrentView, value); }
        }

        private readonly ObservableCollection<NavCommand> _NavCommands = new ObservableCollection<NavCommand>();
        public ObservableCollection<NavCommand> NavCommands { get { return this._NavCommands; } }

        public NavCommand AddCommand(Type type, object buttonContent = null, object dataContext = null)
        {
            var cmd = new NavCommand(type, this, buttonContent, dataContext);
            NavCommands.Add(cmd);
            return cmd;
        }

        protected void ToggleMenu()
        {
            if (MenuVisibility == Visibility.Collapsed)
                MenuVisibility = Visibility.Visible;
            else
                MenuVisibility = Visibility.Collapsed;
        }

        public void OpenCommand(NavCommand navCommand)
        {
            navCommand.Execute(null);
        }

        string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                SetProperty(ref title, value);
            }
        }

        Visibility menuVisibility;
        public Visibility MenuVisibility
        {
            get
            {
                return menuVisibility;
            }
            set
            {
                if (menuVisibility == value)
                {
                    return;
                }

                SetProperty(ref menuVisibility, value);
            }
        }

        public TcOpen.Inxton.Input.RelayCommand ToggleMenuCommand
        {
            get;
            private set;
        }

        ContentOpeningMode openInWindow;
        public ContentOpeningMode ContentPresentationMode
        {
            get
            {
                return openInWindow;
            }
            set
            {
                SetProperty(ref openInWindow, value);
            }
        }

        public void OpenDefault()
        {
            this.NavCommands.FirstOrDefault(p => true)?.Execute(null);
        }
        
}
}
