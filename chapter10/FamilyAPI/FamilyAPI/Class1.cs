using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

namespace FamilyAPI
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Program : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string rftPath = @"C:\ProgramData\Autodesk\RVT 2016\Family Templates\Chinese\公制柱.rft";
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Document faDoc = app.NewFamilyDocument(rftPath);
            Transaction trans=new Transaction(faDoc,"创建族");
            trans.Start();
            FamilyManager manager = faDoc.FamilyManager;
            FamilyParameter mfp = manager.AddParameter("材质", BuiltInParameterGroup.PG_MATERIALS, ParameterType.Material,
                false);

            CurveArrArray arry = GetCurves();
            SketchPlane skplane = GetSketchPlane(faDoc);
            Extrusion extrusion = faDoc.FamilyCreate.NewExtrusion(true, arry, skplane, 4000/304.8);
            faDoc.Regenerate();
            Reference topFaceRef = null;
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Fine;
            GeometryElement gelm = extrusion.get_Geometry(opt);
            foreach (GeometryObject gobj in gelm)
            {
                if (gobj is Solid)
                {
                    Solid s = gobj as Solid;
                    foreach (FaceArray face in s.Faces)
                    {
                        if(face.ComputeNormal(new UV()).IsAlmostEqualTo(new XYZ(0, 0, 1)))
                        {
                            topFaceRef = face.Reference;
                        }
                    }
                }
            }
            View v = GetView(faDoc);
            Reference r = GetTopLevel(faDoc);
            Dimension d = faDoc.FamilyCreate.NewAlignment(v, r, topFaceRef);
            d.IsLocked = true;
            faDoc.Regenerate();

            Parameter p = extrusion.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
            manager.AssociateElementParameterToFamilyParameter(p,mfp);

            trans.Commit();

            Family fa = faDoc.LoadFamily(doc);
            faDoc.Close(false);
            trans = new Transaction(doc,"创建柱");
            trans.Start();
            fa.Name = "我的柱子";
            trans.Commit();
            
                
            return Result.Succeeded;
        }

        private Reference GetTopLevel(Document doc)
        {
            FilteredElementCollector temc=new FilteredElementCollector(doc);
            temc.OfClass(typeof (Level));
            Level lvl = temc.First(m => m.Name == "高于参照标高") as Level;
            return new Reference(lvl);
        }
        private View GetView(Document doc)
        {
            FilteredElementCollector viewFilter = new FilteredElementCollector(doc);
            viewFilter.OfClass(typeof (View));
            View v = viewFilter.First(m => m.Name == "前") as View;
            return v;
        }
        private SketchPlane GetSketchPlane(Document doc)
        {
            FilteredElementCollector temc = new FilteredElementCollector(doc);
            temc.OfClass(typeof (SketchPlane));
            SketchPlane sketchPlane = temc.First(m => m.Name == "低于参照标高") as SketchPlane;
            return sketchPlane;
        }
        private CurveArrArray GetCurves()
        {
            double len = 300/304.8;
            XYZ p1=new XYZ(-len,-len,0);
            XYZ p2 = new XYZ(len, -len, 0);
            XYZ p3 = new XYZ(len, len, 0);
            XYZ p4 = new XYZ(-len, len, 0);

            Line l1 = Line.CreateBound(p1, p2);
            Line l2 = Line.CreateBound(p2, p3);
            Line l3 = Line.CreateBound(p3, p4);
            Line l4 = Line.CreateBound(p4, p1);
            CurveArrArray ary = new CurveArrArray();
            CurveArray arry = new CurveArray();
            arry.Append(l1);
            arry.Append(l2);
            arry.Append(l3);
            arry.Append(l4);
            ary.Append(arry);
            return ary;


        }
    }
}
