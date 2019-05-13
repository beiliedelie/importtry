using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

namespace Revit2016Template1
{
    [Transaction(TransactionMode.Manual)]
    public class CmdClipWall : IExternalCommand
    {
        Document doc;
        Application app;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;

            app = uiApp.Application;
            doc = uiDoc.Document;

            Reference myref = uiDoc.Selection.PickObject(ObjectType.Element);
            Wall wall = doc.GetElement(myref) as Wall;
            Curve curve = (wall.Location as LocationCurve).Curve;

            if (curve is Arc)
            {
                Arc arc = curve as Arc;
                Arc newArc1 = Arc.Create(arc.Evaluate(0, true), arc.Evaluate(0.5, true), arc.Evaluate(0.25, true));
                Arc newArc2 = Arc.Create(arc.Evaluate(0.5, true), arc.Evaluate(1.0, true), arc.Evaluate(0.75, true));

                using (Transaction tr = new Transaction(doc, "切分圆弧墙"))
                {
                    tr.Start();
                    ElementId id = ElementTransformUtils.CopyElement(doc, wall.Id, XYZ.Zero).First();
                    Wall newWall1 = doc.GetElement(id) as Wall;
                    (newWall1.Location as LocationCurve).Curve = newArc1;

                    id = ElementTransformUtils.CopyElement(doc, wall.Id, XYZ.Zero).First();
                    Wall newWall2 = doc.GetElement(id) as Wall;
                    (newWall2.Location as LocationCurve).Curve = newArc2;
                    doc.Regenerate();
                    doc.Delete(wall.Id);

                    tr.Commit();
                }
            }

            return Result.Succeeded;
        }
    }
}
