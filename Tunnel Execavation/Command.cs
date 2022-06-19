#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

#endregion

namespace Tunnel_Execavation
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            bool isAFamilyDoc = CheckFamily(doc);

            if (!isAFamilyDoc)
            {
                CreateFamily(app);
            }
            // tell Revit it is succeeded
            return Result.Succeeded;
        }

        // Create new swept blend family
        // check whether this is a family file
        private bool CheckFamily(Document doc)
        {

            if (!doc.IsFamilyDocument)
            {
                return false;
            }
            else
            {
                TaskDialog td = new TaskDialog("Sucess")
                {
                    Title = "Success 001",
                    AllowCancellation = true,
                    MainInstruction = "success",
                    MainContent = "we are in a family file."
                };

                td.CommonButtons = TaskDialogCommonButtons.Ok;
                td.Show();

                return true;
            }
        }

        // create and a new family 
        public static Document CreateFamily(Application app)
        {
            Document familyDocument = null;

            // get the path of family template folder
            string famTemplatePath = app.FamilyTemplatePath;
            string familyTemplateFullName = Path.Combine(famTemplatePath, @"English\Metric Generic Model.rft");

            //declare new family Name and Path
            string nfamilyName = @"NewTunnel.rfa";
            string nfamilyPath = Path.Combine(@"c:\temp", nfamilyName);



            // create generic family
            try
            {
                familyDocument = app.NewFamilyDocument(familyTemplateFullName);
                familyDocument.SaveAs(nfamilyPath);

                TaskDialog td = new TaskDialog("Sucess")
                {
                    Title = "Success 002",
                    AllowCancellation = true,
                    MainInstruction = "Success",
                    MainContent = "Created family \n {nfamilyPath} with generic family template"
                };

                td.CommonButtons = TaskDialogCommonButtons.Ok;
                td.Show();

                return familyDocument;
            }
            catch (Exception e)
            {
                TaskDialog td = new TaskDialog("Error")
                {
                    Title = "Error 001",
                    AllowCancellation = true,
                    MainInstruction = "Cannot Create family",
                    MainContent = $"{e.Message} \n {nfamilyPath}"
                };

                td.CommonButtons = TaskDialogCommonButtons.Ok;
                td.Show();

                return familyDocument;
            }
        }

        // load family to the Reivt file
        // check whether the family already loaded
        public static Element FindElementByName(Document doc, Type targetType, string targetName)
        {
            return new FilteredElementCollector(doc)
                .OfClass(targetType)
                .FirstOrDefault<Element>(
                 e => e.Name.Equals(targetName)
                );
        }

        public static void Loadfamily(Element loadedFamily, string FamilyPath, string FamilyName)
        {
            if (null == loadedFamily)
            {
                if (!File.Exists(FamilyPath))
                {
                    TaskDialog td = new TaskDialog("Error")
                    {
                        Title = "Error 002",
                        AllowCancellation = true,
                        MainInstruction = "Cannot Load Family",
                        MainContent = $"Please ensure that the family file {FamilyName} exists in {FamilyPath}"
                    };

                    td.CommonButtons = TaskDialogCommonButtons.Ok;
                    td.Show();
                }
            }
        }

        // edit family
        // create 1st sweepProfile of the tunnel
        // create 2nd sweepProfile of the tunnel
        // create the reference path
        // rotate the sweepProfile to be prependiculate to the reference path

        
        // Delete family file from h

    }

}
