using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;

namespace FilterPractise
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);//创建元素收集器


            collector = collector.OfCategory(BuiltInCategory.OST_Windows).OfClass(typeof(FamilySymbol));//QuickFilter过滤所有的墙

            //FamilyInsetanceFilter找406*610mm的窗
            var query = from Element in collector
                        where Element.Name == "0406 x 0610mm"
                        select Element;
            List<Autodesk.Revit.DB.Element> famSyms = query.ToList<Autodesk.Revit.DB.Element>();
            ElementId symbolId = famSyms[0].Id;
            FamilyInstanceFilter fiFilter = new FamilyInstanceFilter(doc, symbolId);

            FilteredElementCollector c1 = new FilteredElementCollector(doc);
            ICollection<Element> found = c1.WherePasses(fiFilter).ToElements();

            //LogicalOrFilter计算门窗总和
            ElementCategoryFilter doorFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            ElementCategoryFilter windowFilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            LogicalOrFilter lFilter = new LogicalOrFilter(doorFilter, windowFilter);
            FilteredElementCollector c3 = new FilteredElementCollector(doc);
            ICollection<Element> fds = c3.OfClass(typeof(FamilyInstance)).WherePasses(lFilter).ToElements();

            //taskdialog输出结果
            TaskDialog.Show("查找",
                "已找到型号为“406*610mm的窗：”" + found.Count.ToString() + "个" + "\n" + "门窗个数总和为：" + fds.Count.ToString());

            return Result.Succeeded;
        }
    }
}

