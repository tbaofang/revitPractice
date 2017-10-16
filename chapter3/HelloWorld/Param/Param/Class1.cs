using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace Param
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Program : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            var elemList = uidoc.Selection.GetElementIds().ToList();
            Element selElem = uidoc.Document.GetElement(elemList[0]);
            ElementType type = doc.GetElement(selElem.GetTypeId()) as ElementType;
            string str = "元素族名称：" + type.FamilyName + "\n" + "元素类型：" + type.Name;
            TaskDialog.Show("元素参数", str);

            return Result.Succeeded;
        }
    }
}
