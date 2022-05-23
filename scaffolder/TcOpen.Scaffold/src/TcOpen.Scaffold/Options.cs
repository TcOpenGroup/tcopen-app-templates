﻿using CommandLine;
using System.ComponentModel;

namespace TcOpen.Scaffold
{
    public class Options : INotifyPropertyChanged
    {        
        string source;
        [Option('b', "source", Default = null, HelpText = "Branch from which draw the scaffold.")]
        public string Source
        {
            get => source;
            set
            {
                if (source == value)
                {
                    return;
                }

                source = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Source)));
            }
        }
     
        string projectName;
        [Option('n', "project-name", Default = "MyProject", HelpText = "Project name.")]
        public string ProjectName
        {
            get => projectName;
            set
            {
                if (projectName == value)
                {
                    return;
                }

                projectName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProjectName)));
            }
        }

        string templateName;
        [Option('n', "template-name", Default = "mts-s-template", HelpText = "Name of the template from which the project will be scaffolded.")]
        public string TemplateName
        {
            get => templateName;
            set
            {
                if (templateName == value)
                {
                    return;
                }

                templateName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TemplateName)));
            }
        }

        string outputDirectory;
        [Option('o', "output-directory", Default = null, HelpText = "Target directory for the scaffold.")]
        public string OutputDirectory
        {
            get => outputDirectory;
            set
            {
                if (outputDirectory == value)
                {
                    return;
                }

                outputDirectory = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputDirectory)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
