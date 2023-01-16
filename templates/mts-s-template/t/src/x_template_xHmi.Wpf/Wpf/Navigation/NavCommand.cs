using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace x_template_xHmi.Wpf
{
    public class NavCommand : ICommand
    {
        public NavCommand(Type viewType,
                          INavigable presenter,
                          object buttonContent = null,
                          object dataContext = null,
                          bool createNewInstanceAtEachNavigation = false)
        {
            ViewType = viewType;
            Presenter = presenter;
            ButtonContent = buttonContent;
            DataContext = dataContext;
            CreateNewInstanceAtEachNavigation = createNewInstanceAtEachNavigation;
            NavCommandsList.Add(this);

            _instanceCount++;

            if (_instanceCount == 1)
            {
                Vortex.Framework.Abstractions.Security.SecurityProvider.OnAnyAuthenticationEvent += SecurityProvider_OnAnyAuthenticationEvent;
            }
        }

        private FrameworkElement MenuIcon(string menuTitle) => new Border()
        {
            Background = (Brush)Application.Current.FindResource("Accent"),
            BorderBrush = (Brush)Application.Current.FindResource("OnAccent"),
            BorderThickness = new Thickness(2.0),
            CornerRadius = new CornerRadius(5.0),
            Width = 25.0,
            Height = 25.0,
            Child = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 15,
                FontWeight = FontWeight.FromOpenTypeWeight(700),
                Foreground = (Brush)Application.Current.FindResource("OnAccent"),
                Style = null,
                Text = menuTitle.ToUpper()[0].ToString()
            }
        };

        public object Tag
        {
            get
            {
                if (_tag is null)
                    try
                    {
                        return MenuIcon(ButtonContent.ToString());
                    }
                    catch (IOException)
                    {
                        return MenuIcon("X");
                    }
                return _tag;
            }
            set => _tag = value;
        }

        private void SecurityProvider_OnAnyAuthenticationEvent(string username)
        {
            EraseCachedElements();
        }

        private readonly int _instanceCount = 0;
        internal static void EraseCachedElements()
        {
            foreach (var navcommand in NavCommandsList)
            {
                navcommand._element = null;
            }
        }

        private static List<NavCommand> NavCommandsList = new List<NavCommand>();

        public object ButtonContent { get; private set; }

        public Type ViewType { get; private set; }
        private INavigable Presenter;
        private readonly object DataContext;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        private FrameworkElement _element;
        private readonly bool CreateNewInstanceAtEachNavigation;
        private object _tag;

        private FrameworkElement CreateElement()
        {

            if (this._element == null || CreateNewInstanceAtEachNavigation)
            {

                var element = (FrameworkElement)Activator.CreateInstance(ViewType);

                if (this.DataContext != null)
                {
                    element.DataContext = DataContext;
                }

                _element = element;
            }

            return _element;
        }

        public FrameworkElement PreviewElement => CreateElement();

        public void Execute(object parameter)
        {
            switch (Presenter.ContentPresentationMode)
            {
                case ContentOpeningMode.Spa:
                    Presenter.CurrentView = CreateElement();
                    break;
                case ContentOpeningMode.ShowWindow:
                    ShowInWindow(Presenter.CurrentView?.DataContext);
                    break;
                case ContentOpeningMode.ShowDialogWindow:
                    ShowDialogueInWindow(Presenter.CurrentView?.DataContext);
                    break;
                default:
                    Presenter.CurrentView = CreateElement();
                    break;
            }
        }


        /// <summary>
        /// Shows content in separate window.
        /// </summary>
        private void ShowInWindow(object dataContext)
        {
            var window = new Window();
            window.Topmost = true;
            window.Owner = Application.Current.MainWindow;
            window.DataContext = dataContext;
            window.Content = (CreateElement());
            window.Show();
        }


        /// <summary>
        /// Shows content in separate dialogue window.
        /// </summary>
        private void ShowDialogueInWindow(object dataContext)
        {
            var window = new Window();
            window.Topmost = true;
            window.Owner = Application.Current.MainWindow;
            window.DataContext = dataContext;
            window.Content = (CreateElement());
            window.ShowDialog();
        }

        public void OpenView(FrameworkElement view)
        {
            Presenter.CurrentView = CreateElement(view);
        }

        private FrameworkElement CreateElement(FrameworkElement view)
        {
            var element = view;
            return element;
        }
    }
}
