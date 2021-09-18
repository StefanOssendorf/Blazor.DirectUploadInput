using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace LargeFileUpload {

    /// <summary>
    /// The code behind file of <see cref="FileUpload"/> component.
    /// </summary>
    public partial class FileUpload {

#if NET6_0_OR_GREATER
        [EditorRequired]
#endif
        [Parameter]
        public string Name { get; set; } = string.Empty;

        [Parameter]
        public bool Multiple { get; set; }

        [Parameter]
        public bool Disabled {  get; set; }
    }
}
