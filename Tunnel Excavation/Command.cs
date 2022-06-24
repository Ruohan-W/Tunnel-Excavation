#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using ComponentManager = Autodesk.Windows.ComponentManager;
using IWin32Window = System.Windows.Forms.IWin32Window;
using Keys = System.Windows.Forms.Keys;
#endregion

namespace Tunnel_Excavation
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

            // get the path of family template folder
            string famTemplatePath = app.FamilyTemplatePath;
            string familyTemplateFullName = Path.Combine(famTemplatePath, @"English\Metric Generic Model.rft");

            //declare new family Name and Path
            string nfamilyName = "NewTunnel";
            string nfamilyFormat = ".rfa";
            string nfamilyPath = Path.Combine(@"c:\temp", nfamilyName + nfamilyFormat);

            // check whether this is family file (.rfa) or a project file (.rvt)
            bool isAFamilyDoc = CheckFamily(doc);

            // if it is a project file, check whether the target family already loaded
            if (!isAFamilyDoc)
            {
                Family loadedFamily = (Family)FindElementByName(doc, typeof(Family), nfamilyName);

                if (null != loadedFamily)
                {
                    TaskDialog td = new TaskDialog("Erorr")
                    {
                        Title = "Error 003",
                        AllowCancellation = true,
                        MainInstruction = "Cannot load family",
                        MainContent = "Target family NewTunnel has already been loaded to the project"
                    };

                    td.CommonButtons = TaskDialogCommonButtons.Cancel;
                    td.Show();
                }
                else
                {
                    // if the target has not been loaded yet. create the family first.
                    Document nFamilyDoc = CreateFamily(app, familyTemplateFullName, nfamilyPath);

                    // load the family we just created
                    if (null != nFamilyDoc)
                    {
                        Family nLoadedFamily = Loadfamily(doc, loadedFamily, nfamilyPath, nfamilyName);
                        // place family instance
                        PlaceFamilyInstance(nLoadedFamily, uidoc, app, doc);
                    }
                }
            }
            // tell Revit it is succeeded
            return Result.Succeeded;
        }

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
                    MainContent = "We are in a family file."
                };

                td.CommonButtons = TaskDialogCommonButtons.Ok;
                td.Show();

                return true;
            }
        }

        // create and a new family - swept blend family for tunnel
        private static Document CreateFamily(Application app, string familyTemplateFullName, string nfamilyPath)
        {
            Document familyDocument = null;

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
                    MainContent = $"Created family \n {nfamilyPath} with generic family template"
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

        // check whether the family already loaded
        private static Element FindElementByName(Document doc, Type targetType, string targetName)
        {
            // get all elements of the target type
            FilteredElementCollector targetElementCollector
                = new FilteredElementCollector(doc)
                .OfClass(targetType);

            // get the first element with the target name
            Element foundElement = targetElementCollector.FirstOrDefault(e => e.Name.Equals(targetName)) as Element;

            return foundElement;

            /* shorthand of the code above    
            return new FilteredElementCollector(doc)
                .OfClass(targetType)
                .FirstOrDefault<Element>(
                 e => e.Name.Equals(targetName)
                );
            */
        }

        // load family to the Reivt file
        private static Family Loadfamily(Document doc, Family loadedFamily, string familyPath, string FamilyName)
        {
            if (null == loadedFamily)
            {
                if (!File.Exists(familyPath))
                {
                    TaskDialog td = new TaskDialog("Error")
                    {
                        Title = "Error 002",
                        AllowCancellation = true,
                        MainInstruction = "Cannot Load Family",
                        MainContent = $"Please ensure that the family file {FamilyName} exists in {familyPath}"
                    };

                    td.CommonButtons = TaskDialogCommonButtons.Ok;
                    td.Show();
                }
                else 
                {
                    // start transaction - load family from the tempoary folder
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Load Family");
                        doc.LoadFamily(familyPath, out loadedFamily);
                        tx.Commit();
                    }
                }
            }
            return loadedFamily;
        }

        // place the family symbol
        private static void PlaceFamilyInstance(Family loadedFamily, UIDocument uidoc, Application app, Document doc)
        {           
            List<ElementId> _added_element_ids= new List<ElementId>();

            bool _single_instance = true;
            IWin32Window _revit_window = new JtWindowHandle(ComponentManager.ApplicationWindow);

            FamilySymbol symbol = null;
            foreach (ElementId id in loadedFamily.GetFamilySymbolIds())
            {
                symbol = doc.GetElement(id) as FamilySymbol;
                break;
            }

            void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
            {
                ICollection<ElementId> idsAdded = e.GetAddedElementIds();
                int idsAddedCountNum = idsAdded.Count;

                Debug.Print("{0} id {1} add.",
                    idsAddedCountNum,
                    Util.PluralSuffix(idsAddedCountNum)
                    );

                _added_element_ids.AddRange(idsAdded);

                if (_single_instance && idsAddedCountNum > 0)
                {
                    Press.PostMessage(_revit_window.Handle,
                        (uint)Press.KEYBOARD_MSG.WM_KEYDOWN,
                        (uint)Keys.Escape, 0);

                    Press.PostMessage(_revit_window.Handle,
                        (uint)Press.KEYBOARD_MSG.WM_KEYDOWN,
                        (uint)Keys.Escape, 0);
                }
            }

            _added_element_ids.Clear();
            // register to document changed event to collect newly added family instance
            app.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);

            // place family instance
            try
            {
                uidoc.PromptForFamilyInstancePlacement(symbol);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                Debug.Print(ex.Message);
            }

            // unregister to document changed event to retrive the id of newly added symbol
            app.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);

            // tell the user the operation is successed 
            int addedElementIdsCountNum = _added_element_ids.Count();

            string msg = string.Format
                (
                "Placed {0} {1} family instance{2}{3}",
                addedElementIdsCountNum, 
                loadedFamily.Name,
                Util.PluralSuffix(addedElementIdsCountNum),
                Util.DotOrColon(addedElementIdsCountNum)
                );

            string ids = string.Join
                (
                    ", ",
                    _added_element_ids.Select<ElementId, string>( id => id.IntegerValue.ToString())
                );

            TaskDialog td = new TaskDialog("Success")
            {
                Title = "Success 003",
                AllowCancellation = true,
                MainContent = "Successfully placed family instance",
                MainInstruction = msg + ids,
            };

            td.CommonButtons = TaskDialogCommonButtons.Ok;
            td.Show();
        }



        // edit family
        // create 1st sweepProfile of the tunnel
        // create 2nd sweepProfile of the tunnel
        // create the reference path
        // rotate the sweepProfile to be prependiculate to the reference path

        
        // Delete family file from h

    }

}
