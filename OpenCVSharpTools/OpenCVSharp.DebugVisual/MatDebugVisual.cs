using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System.Windows.Forms;
using OpenCvSharp;

[assembly:System.Diagnostics.DebuggerVisualizer(
    typeof(OpenCVSharpTools.MatDebugVisual),
    typeof(OpenCVSharpTools.MatObjectSource),
    Target = typeof(Mat),
    Description = "my mat visual")]

namespace OpenCVSharpTools
{
    
    public class MatDebugVisual : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            IVisualizerObjectProvider2 provider2 = objectProvider as IVisualizerObjectProvider2;

            MessageBox.Show(provider2.GetObject().ToString());
        }
    }
}
