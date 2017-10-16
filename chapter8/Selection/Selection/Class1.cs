using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace Selection
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Program : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Autodesk.Revit.UI.Selection.Selection s1 = uidoc.Selection;
            XYZ point = null;

            try
            {
                point = s1.PickPoint("请选择一个点");
            }
            catch
            {
                return Result.Succeeded;
            }
            Transaction t1 = new Transaction(doc,"t1");
            t1.Start();
            FamilySymbol familySymbol = doc.GetElement(new ElementId(52557)) as FamilySymbol;
            if (!familySymbol.IsActive)
            {
                familySymbol.Activate();
            }
            Level level = null;
            FamilyInstance fi = doc.Create.NewFamilyInstance(point, familySymbol, level, StructuralType.NonStructural);
            t1.Commit();

            Autodesk.Revit.UI.Selection.Selection s2 = uidoc.Selection;
            Reference re = s2.PickObject(ObjectType.Element,"请选择一个物体");
            Element ele = doc.GetElement(re);
            Options opt = new Options();
            GeometryElement gelem = ele.get_Geometry(opt);
            double v = 0.0;
            v = GetSolid(gelem).Sum(m => m.Volume)*0.3048*0.3048*0.3048;
            TaskDialog.Show("Hint", "选中物体体积为：" + v.ToString("f3"));

            IList<Element> pickedElements = s2.PickElementsByRectangle(new WallSelectionfilter(), "请选择目标物体");
            double num = pickedElements.Count();
            TaskDialog.Show("Hint", "已选中的墙数为："+num);
          
            return Result.Succeeded;
        }

        private List<Solid> GetSolid(GeometryElement gelem)
        {
            List<Solid>solids =new List<Solid>();
            foreach (GeometryObject obj in gelem)
            {
                if (obj is Solid)
                {
                    solids.Add(obj as Solid);
                }
                if (obj is GeometryElement)
                {
                    solids.AddRange(GetSolid(obj as GeometryElement));
                }
                if (obj is GeometryElement)
                {
                    GeometryInstance gins = obj as GeometryInstance;
                    GeometryElement gelm = gins.GetInstanceGeometry();
                    solids.AddRange(GetSolid(gelm));
                    
                }

            }
            return solids;
        }
    }

    public class WallSelectionfilter : ISelectionFilter
    {
        public bool AllowElement(ElementArray elem)
        {
            return elem is Wall;
        }

        public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
        {
            return true;
        }
    }
}
