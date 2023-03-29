using System;
using System.Linq;

namespace x_template_xHmi.Wpf
{
    public class ConsoleWriterEventArgs : EventArgs
    {
        public ConsoleWriterEventArgs(string value)
        {
            this.Value = value;
        }

        public string Value
        {
            get; private set;
        }
    }
}
