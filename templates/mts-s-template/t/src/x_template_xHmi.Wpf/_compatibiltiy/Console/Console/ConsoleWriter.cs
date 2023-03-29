using System;
using System.IO;
using System.Linq;
using System.Text;

namespace x_template_xHmi.Wpf
{
    /// <summary>
    /// The console writer.
    /// </summary>
    public class ConsoleWriter : TextWriter
    {
        /// <summary>
        /// The write event.
        /// </summary>
        public event EventHandler<ConsoleWriterEventArgs> WriteEvent;

        /// <summary>
        /// The write line event.
        /// </summary>
        public event EventHandler<ConsoleWriterEventArgs> WriteLineEvent;

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void Write(string value)
        {
            if (this.WriteEvent != null)
            {
                this.WriteEvent(this, new ConsoleWriterEventArgs(value));
            }

            base.Write(value);
        }

        /// <summary>
        /// The write line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void WriteLine(string value)
        {
            if (this.WriteLineEvent != null)
            {
                this.WriteLineEvent(this, new ConsoleWriterEventArgs(value));
            }

            base.WriteLine(value);
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }
}
