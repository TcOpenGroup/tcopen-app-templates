using CommandLine;
using System.ComponentModel;

namespace TcOpen.Scaffold
{
    public class Options : INotifyPropertyChanged
    {
        public string _branch;
        [Option('b', "branch", Default = "dev", HelpText = "Branch from which draw the scaffold.")]
        public string Branch { get { return _branch; } set { _branch = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Branch))); } }

        public string _projectName;
        [Option('n', "project-name", Default = "MyProject", HelpText = "Project name.")]
        public string ProjectName { get { return _projectName; } set { _projectName = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProjectName))); } }

        public string _templateName;
        [Option('n', "template-name", Default = "mts-s-template", HelpText = "Name of the template from which the project will be scaffolded.")]
        public string TemplateName { get { return _templateName; } set { _templateName = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TemplateName))); } }

        public string _outputDirectory { get; set; }
        [Option('o', "output-directory", Default = null, HelpText = "Target directory for the scaffold.")]
        public string OutputDirectory { get { return _outputDirectory; } set { _outputDirectory = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputDirectory))); } }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
