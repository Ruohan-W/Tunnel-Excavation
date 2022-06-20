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
    internal class App : IExternalApplication
    {
        const string RIBBON_TAB = "ruohan test tools";
        const string RIBBON_PANEL = "Tunnel Excavation";
        public Result OnStartup(UIControlledApplication a)
        {
            // get the RIBBON_TAB 
            try
            {
                a.CreateRibbonTab(RIBBON_TAB);
            }
            catch (Exception) { } // the exception catched when the tab already exists

            // get or create RIBBON_PANEL and add it to the RIBBON_TAB
            // get the RIBBON_PANEL if it exists
            RibbonPanel panel = null;
            List<RibbonPanel> panels = a.GetRibbonPanels(RIBBON_TAB);
            foreach (RibbonPanel pnl in panels)
            {
                if (pnl.Name == RIBBON_PANEL)
                {
                    panel = pnl;
                    break;
                }
            }
            // create RIBBON_PANEL if it doesn't exist
            if (panel == null)
            {
                panel = a.CreateRibbonPanel(RIBBON_TAB, RIBBON_PANEL);
            }

            // get the image for the button - make sure the image is 32 x 32 pixels and 69 dpi 
            Image img = Properties.Resources.sample_icon_32x32;
            ImageSource imgSrc = GetBitmapSource(img);

            // create the button data
            PushButtonData btnData = new PushButtonData(
                "Tunnel",
                "Create Tunnel",
                Assembly.GetExecutingAssembly().Location,
                "Tunnel_Excavation.Command"
                )
            {
                ToolTip = "short description that is shown when you hover over the button",
                LongDescription = "longer desciption shown when you hover over the button for a few seconds",
                Image = imgSrc,
                LargeImage = imgSrc
            };

            // add the button to the ribbon 
            PushButton button = panel.AddItem(btnData) as PushButton;
            button.Enabled = true;

            //for a detail list for all properties of the panel and button, search RibbonPanel Class in Revit API.

            return Result.Succeeded;
        }


        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        private BitmapSource GetBitmapSource(Image img)
        {
            BitmapImage bmp = new BitmapImage();

            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                bmp.BeginInit();

                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;

                bmp.EndInit();
            }

            return bmp;
        }
    }
}
