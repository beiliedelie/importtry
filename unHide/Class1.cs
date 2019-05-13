using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;

namespace unHide
{
    [Transaction(TransactionMode.Manual)]
    public class UnHideFamily : IExternalCommand
    {
        Document doc;
        Application app;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;

            doc = uiDoc.Document;

            FilteredElementCollector hideEleCollector = new FilteredElementCollector(doc).WhereElementIsNotElementType();
            List<ElementId> hideElementIds = new List<ElementId>();
            foreach (var item in hideEleCollector)
            {
                if (item.CanBeHidden(doc.ActiveView) && item.IsHidden(doc.ActiveView))
                {
                    hideElementIds.Add(item.Id);
                }
            }
            Transaction trans = new Transaction(doc, "显示");
            trans.Start();
            doc.ActiveView.UnhideElements(hideElementIds);
            trans.Commit();

            return Result.Succeeded;

        }
    }
}
