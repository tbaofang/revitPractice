using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace MoreCreation
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Program : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Transaction trans = new Transaction(doc,"LS");
            trans.Start();

            Curve c1 = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 10, 0));
            Curve c2 = Line.CreateBound(new XYZ(0, 10, 0), new XYZ(10, 10, 0));
            Curve c3 = Line.CreateBound(new XYZ(10, 10, 0), new XYZ(10, 0, 0));
            Curve c4 = Line.CreateBound(new XYZ(10, 0, 0), new XYZ(0, 0, 0));
            CurveArray curvearray=new CurveArray();
            curvearray.Append(c1);
            curvearray.Append(c2);
            curvearray.Append(c3);
            curvearray.Append(c4);
            CurveArrArray curvearrarray=new CurveArrArray();
            curvearrarray.Append(curvearray);

            doc.FamilyCreate.NewExtrusion(true, curvearrarray,
                SketchPlane.Create(doc, new Plane(new XYZ(0, 0, 1), XYZ.Zero)), 10);

            doc.FamilyManager.NewType("我创建的类");

            trans.Commit();
            return Result.Succeeded;
        }
    }
}
