using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;

namespace x_template_xHmi.Wpf
{
    /// <summary>
    /// The system diagnostics singleton.
    /// </summary>
    public class SystemDiagnosticsSingleton : INotifyPropertyChanged
    {
        /// <summary>
        /// The instance.
        /// </summary>
        private static volatile SystemDiagnosticsSingleton instance;

        /// <summary>
        /// The sync root.
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// The console writer.
        /// </summary>
        private readonly ConsoleWriter consoleWriter = new ConsoleWriter();

        /// <summary>
        /// The output builder.
        /// </summary>
        private StringBuilder outputBuilder = new StringBuilder();

        private Timer updateTimer;

        /// <summary>
        /// Prevents a default instance of the <see cref="SystemDiagnosticsSingleton"/> class from being created.
        /// </summary>
        private SystemDiagnosticsSingleton()
        {
            this.consoleWriter.WriteEvent += this.ConsoleWriterWriteEvent;
            this.consoleWriter.WriteLineEvent += this.ConsoleWriterWriteLineEvent;
            Console.SetOut(this.consoleWriter);
            this.Process = Process.GetCurrentProcess();

            this.updateTimer = new System.Timers.Timer(1000);
            this.updateTimer.Elapsed += new ElapsedEventHandler(this.OnTimedEvent);
            this.updateTimer.Interval = 1000;
            this.updateTimer.Enabled = true;
        }

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The console writer write event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ConsoleWriterWriteEvent(object sender,
                                             ConsoleWriterEventArgs e)
        {
            Write(e.Value);
        }


        private void Write(string message)
        {
            lock (syncRoot)
            {
                if (this.outputBuilder.Length > 5000)
                {
                    this.outputBuilder.Clear();
                }

                this.outputBuilder.AppendLine(message);
            }
            this.OnPropertyChanged("Output");
        }

        /// <summary>
        /// The console writer write line event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ConsoleWriterWriteLineEvent(object sender,
                                                 ConsoleWriterEventArgs e)
        {
            Write(e.Value);
            //Console.WriteLine(e.Value);
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            this.OnPropertyChanged("BasePriority");
            this.OnPropertyChanged("NumberOfThreads");
            this.OnPropertyChanged("ProcessId");
            this.OnPropertyChanged("ProcessName");
            this.OnPropertyChanged("MachineName");
            this.OnPropertyChanged("StartTime");
            this.OnPropertyChanged("TotalProcessorTime");
            this.OnPropertyChanged("UserProcessorTime");
            this.OnPropertyChanged("PrivilegedProcessorTime");
            this.OnPropertyChanged("NonpagedSystemMemorySize64");
            this.OnPropertyChanged("PagedMemorySize64");
        }

        private void process_OutputDataReceived(object sender,
                                                DataReceivedEventArgs e)
        {
            Write(e.Data);
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this,
                                            new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets or sets the process.
        /// </summary>
        internal Process Process
        {
            get; set;
        }

        /// <summary>
        /// Performs series of checks at the start up of the application.
        /// e.g. storage space requirements...
        /// </summary>
        public void PerformStartUpChecks()
        {
        }


        public void RedirectProcessOutputToConsole(Process process)
        {
            process.OutputDataReceived += process_OutputDataReceived;
        }

        /// <summary>
        /// Gets the base priority.
        /// </summary>
        public int BasePriority
        {
            get
            {
                return this.Process.BasePriority;
            }
        }

        public ConsoleWriter ConsoleWriter
        {
            get
            {
                return this.consoleWriter;
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static SystemDiagnosticsSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new SystemDiagnosticsSingleton();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the machine name.
        /// </summary>
        public string MachineName
        {
            get
            {
                return this.Process.MachineName;
            }
        }

        /// <summary>
        /// Gets the modules count.
        /// </summary>
        public int ModulesCount
        {
            get
            {
                return this.Process.Modules.Count;
            }
        }

        public long NonpagedSystemMemorySize64
        {
            get
            {
                return this.Process.NonpagedSystemMemorySize64;
            }
        }

        /// <summary>
        /// Gets the number of threads.
        /// </summary>
        public int NumberOfThreads
        {
            get
            {
                return this.Process.Threads.Count;
            }
        }

        /// <summary>
        /// Gets the output.
        /// </summary>
        public string Output
        {
            get
            {
                lock (syncRoot)
                {
                    return this.outputBuilder.ToString();
                }
            }
        }

        public long PagedMemorySize64
        {
            get
            {
                return this.Process.PagedMemorySize64;
            }
        }

        public TimeSpan PrivilegedProcessorTime
        {
            get
            {
                return this.Process.PrivilegedProcessorTime;
            }
        }

        /// <summary>
        /// Gets the process id.
        /// </summary>
        public int ProcessId
        {
            get
            {
                return this.Process.Id;
            }
        }

        /// <summary>
        /// Gets the process name.
        /// </summary>
        public string ProcessName
        {
            get
            {
                return this.Process.ProcessName;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return this.Process.StartTime;
            }
        }

        public TimeSpan TotalProcessorTime
        {
            get
            {
                return this.Process.TotalProcessorTime;
            }
        }

        public TimeSpan UserProcessorTime
        {
            get
            {
                return this.Process.UserProcessorTime;
            }
        }
    }
}
