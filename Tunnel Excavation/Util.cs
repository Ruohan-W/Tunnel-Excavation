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

namespace Tunnel_Excavation
{
    class Util
    { 
        internal static string PluralSuffix(int n)
        {
            string str = null;

            if (n > 1)
            {
                str = "s";
            }

            return str;
        }

        internal static string DotOrColon(int n)
        {
            if (n > 1)
            {
                return ".";
            }
            else 
            {
                return ":";
            }
        }
    }
}