using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace Trans1
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Program : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            TransactionGroup tg = new TransactionGroup(doc, "TG");
            tg.Start();

            Transaction t1 = new Transaction(doc, "T1");
            t1.Start();
            Wall.Create(doc, Line.CreateBound(new XYZ(), new XYZ(0, 10, 0)), Level.Create(doc, 0).Id, false);
            t1.Commit();
            TaskDialog.Show("提示", "已经生成第一道墙");
            Transaction t2 = new Transaction(doc, "T2");
            t2.Start();
            Wall.Create(doc, Line.CreateBound(new XYZ(), new XYZ(0, 10, 0)), Level.Create(doc, 0).Id, false);
            t2.Commit();
            tg.Commit();

            Transaction tt=new Transaction(doc,"Transact");
            tt.Start();
            SubTransaction st1=new SubTransaction(doc);
            st1.Start();
            SubTransaction st2=new SubTransaction(doc);
            st2.Start();
            st2.Commit();
            TaskDialog.Show("提示", "t2已提交");
            st1.Commit();
            TaskDialog.Show("提示", "t1已提交");
            tt.Commit();

            return Result.Succeeded;
        }
    }
}
