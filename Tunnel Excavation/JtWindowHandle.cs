#region Namespaces
using System;
using System.Diagnostics;
using System.Windows.Forms;
#endregion

namespace Tunnel_Excavation
{
    // wrapper class for converting Inptr to Iwin32Window
    public class JtWindowHandle : IWin32Window 
    {
        public JtWindowHandle(IntPtr h)
        {
            Debug.Assert(IntPtr.Zero != h, "expected non-null window handle");
            Handle = h;
        }

        public IntPtr Handle { get; }
    }
}