using CommandLine;

namespace TcOpen.Scaffold
{
    public class Options 
    {
        [Option('b', "branch", Default = "dev", HelpText = "Branch from which draw the scaffold.")]
        public string Branch { get; set; }

        [Option('n', "project-name", Default = "MyProject", HelpText = "Project name.")]
        public string ProjectName { get; set; }

        [Option('n', "template-name", Default = "mts-s-template", HelpText = "Name of the template from which the project will be scaffolded.")]
        public string TemplateName { get; set; }

        [Option('o', "output-directory", Default = null, HelpText = "Target directory for the scaffold.")]
        public string OutputDirectory { get; set; }
    }
}
