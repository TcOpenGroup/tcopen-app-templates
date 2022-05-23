using Cake.Frosting;
using System;
using System.Linq;

namespace Build
{
    [TaskName("Default")]
    [IsDependentOn(typeof(Scaffolder.LastTask))]
    public class DefaultTask : FrostingTask
    {

    }
}
