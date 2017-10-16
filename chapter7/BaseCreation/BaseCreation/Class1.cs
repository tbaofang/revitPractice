using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace BaseCreation
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Program : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Transaction t1 = new Transaction(doc,"创建");
            t1.Start();
            Wall wall = Wall.Create(doc, Line.CreateBound(new XYZ(), new XYZ(0, 10, 0)), Level.Create(doc, 0).Id, false);
            t1.Commit();
            TaskDialog.Show("Tip", "结构墙已创建");

            Transaction t2 = new Transaction(doc,"复制");
            t2.Start();
            ElementTransformUtils.CopyElement(doc,wall.Id,new XYZ(30,30,30));
            t2.Commit();
            TaskDialog.Show("Tip", "结构墙已复制");

            Transaction t3 = new Transaction(doc, "移动");
            t3.Start();
            ElementTransformUtils.MoveElement(doc,wall.Id,new XYZ(10,20,10));
            t3.Commit();
            TaskDialog.Show("Tip","移动");

            Transaction t4=new Transaction(doc,"镜像");
            t4.Start();
            Plane p1=new Plane(new XYZ(0,-1,0),XYZ.Zero);
            ElementTransformUtils.MirrorElement(doc,wall.Id,p1);
            t4.Commit();
            TaskDialog.Show("tip", "结构墙已镜像");

            return Result.Succeeded;
        }
    }
}
