using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.Common.IO;
using System.Linq;
using Build.Tasks;
using Cake.Common.Diagnostics;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string Target { get; }

    public FilePath SolutionFile { get; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        Target = context.Arguments.GetArgument("Target");
        SolutionFile = context.GetFiles(new GlobPattern("../**/StefanOssendorf.Blazor.sln")).Single();

        context.Information($"{Target} and {SolutionFile}");
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(RestoreTask))]
public class DefaultTask : FrostingTask
{
}