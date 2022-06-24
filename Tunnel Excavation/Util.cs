#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endregion

namespace Tunnel_Excavation
{
    class Util
    {
        public static string PluralSuffix(int n)
        {
            return 1 == n ? "" : "s";
        }
        public static string PluralSuffixY(int n)
        {
            return 1 == n ? "y" : "ies";
        }

        internal static string DotOrColon(int n)
        {
            return n < 0 ? ":" : ".";
        }

        internal static string RealString(double a)
        {
            return a.ToString();
        }

        internal static string AngleString(double angle)
        {
            return RealString(angle * 180 / Math.PI) + "degree";
        }
    }
}