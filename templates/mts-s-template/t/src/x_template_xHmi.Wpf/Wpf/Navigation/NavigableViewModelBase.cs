namespace x_template_xHmi.Wpf
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
  
    using Vortex.Presentation.Wpf;

    public class NavigableViewModelBase : BindableBase, INavigable
    {
        public NavigableViewModelBase()
        {
            Renderer.Get.AttachAssemblyToLookUpList(Assembly.GetAssembly(this.GetType()).GetName().Name);
            ToggleMenuCommand = new TcOpen.Inxton.Input.RelayCommand(o => ToggleMenu());
        }

        FrameworkElement _CurrentView;

        public FrameworkElement CurrentView
        {
            get { return _CurrentView; }
            set { SetProperty(ref _CurrentView, value); NavigableViewModelBase.Current = this; }
        }

        private readonly ObservableCollection<NavCommand> _NavCommands = new ObservableCollection<NavCommand>();
        public ObservableCollection<NavCommand> NavCommands { get { return this._NavCommands; } }

        protected void ToggleMenu()
        {
            if (MenuVisibility == Visibility.Collapsed)
                MenuVisibility = Visibility.Visible;
            else
                MenuVisibility = Visibility.Collapsed;
        }

        public NavCommand AddCommand(Type type, object buttonContent = null, object dataContext = null)
        {
            var cmd = new NavCommand(type, this, buttonContent, dataContext);
            NavCommands.Add(cmd);
            return cmd;
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

        public void OpenView(FrameworkElement view)
        {            
             CurrentView = CreateElement(view);
        }

        private FrameworkElement CreateElement(FrameworkElement view)
        {
            var element = view;
            return element;
        }

        public static NavigableViewModelBase Current
        {
            get;
            internal set;
        }

        public Window ShowInWindow(FrameworkElement content)
        {
            var window = new Window();
            window.Topmost = true;
            window.Owner = Application.Current.MainWindow;           
            window.Content = content;
            window.Show();
            return window;
        }


        /// <summary>
        /// Shows content in separate dialogue window.
        /// </summary>
        public void ShowDialogueInWindow(FrameworkElement content)
        {
            var window = new Window();
            window.Topmost = true;
            window.Owner = Application.Current.MainWindow;            
            window.Content = content;
            window.ShowDialog();            
        }

        public void OpenDefault()
        {
            this.NavCommands.FirstOrDefault(p => true)?.Execute(null);
        }
    }
}
