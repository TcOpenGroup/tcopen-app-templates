namespace x_template_xHmi.Wpf
{

    public static class MenuControlsHelper
    {
        public static INavigable MenuRoot { get; internal set; }

        public static void SetRoot(INavigable navigable)
        {
            if (MenuRoot == null)
            {
                MenuRoot = navigable;
            }
        }
    }



    public class MenuControlViewModel : NavigableViewModelBase
    {
        public MenuControlViewModel() : base()
        {
            MenuControlsHelper.SetRoot(this);
        }
    }

    public abstract class MenuRenderableControlViewModel : NavigableRenderableViewModelBase
    {
        public MenuRenderableControlViewModel() : base()
        {
            MenuControlsHelper.SetRoot(this);
        }
    }
}
