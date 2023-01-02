using System;
using System.Windows;

namespace x_template_xHmi.Wpf
{
    public static class MvvmVisualState
    {
        public static readonly DependencyProperty CurrentStateProperty
         = DependencyProperty.RegisterAttached(
             "CurrentState",
             typeof(string),
             typeof(MvvmVisualState),
             new PropertyMetadata(OnCurrentStateChanged));

        public static string GetCurrentState(DependencyObject obj)
        {
            return (string)obj.GetValue(CurrentStateProperty);
        }

        public static void SetCurrentState(DependencyObject obj, string value)
        {
            obj.SetValue(CurrentStateProperty, value);
        }

        private static void OnCurrentStateChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var e = sender as FrameworkElement;

            if (e == null)
                throw new Exception($"CurrentState is only supported on {nameof(FrameworkElement)}.");

            VisualStateManager.GoToElementState(e, (string)args.NewValue, useTransitions: true);
        }
    }
}
