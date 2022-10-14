using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace x_template_xHmi.Blazor
{
    public static class TaskToStateBlazorConverter
    {
        public static bool IsVisibleConverter(object value)
        {
            TcoCore.eTaskState state = (TcoCore.eTaskState)Enum.Parse(typeof(TcoCore.eTaskState), value.ToString());

            switch (state)
            {
                case TcoCore.eTaskState.Ready:
                case TcoCore.eTaskState.Requested:
                    return false;
                case TcoCore.eTaskState.Done:
                case TcoCore.eTaskState.Busy:
                case TcoCore.eTaskState.Error:
                    return true; ;
                default:
                    return false;
            }
        }

    }
}
