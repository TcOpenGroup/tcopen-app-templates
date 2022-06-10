using CommandLine;
using System.ComponentModel;

namespace TcOpen.Scaffold
{
    public class Options : INotifyPropertyChanged
    {        
        string branchOrTag;
        [Option('b', "branch-tag", Default = null, HelpText = "Branch from which draw the scaffold.")]
        public string BranchOrTag
        {
            get => branchOrTag;
            set
            {
                if (branchOrTag == value)
                {
                    return;
                }

                branchOrTag = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BranchOrTag)));
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

        string release;
        [Option('r', "release", Default = null, HelpText = "Release name.")]
        public string Release
        {
            get => release;
            set
            {
                if (release == value)
                {
                    return;
                }

                release = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Release)));
            }
        }


        string source;
        [Option('s', "source", Default = "release", HelpText = "Source release or repository")]
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

        public event PropertyChangedEventHandler PropertyChanged;
    }    
}
