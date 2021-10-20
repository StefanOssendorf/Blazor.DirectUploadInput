using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Frosting;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Common.Diagnostics;

namespace Build.Tasks;

public class RestoreTask : FrostingTask<BuildContext> {
    public override void Run(BuildContext context) {
        context.DotNetCoreRestore(context.SolutionFile.FullPath);
        context.Information($"{context.SolutionFile} restored.");
    }
}
