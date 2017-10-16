using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

namespace collision
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Program : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Transaction trans =new Transaction(doc,"ExComm");
            trans.Start();

            Selection select = uidoc.Selection;
            Reference r = select.PickObject(ObjectType.Element, "选择需要检查的墙");
            Element column = doc.GetElement(r);
            FilteredElementCollector collect=new FilteredElementCollector(doc);
            
            //ElementIntersectFilter冲突检查
            ElementIntersectsElementFilter  iFilter=new ElementIntersectsElementFilter(column,false);
            collect.WherePasses(iFilter);
            List<ElementId> excludes = new List<ElementId>();
            excludes.Add(column.Id);
            collect.Excluding(excludes);
            List<ElementId> ids = new List<ElementId>();
            select.SetElementIds(ids);

            foreach (Element elem in collect)
            {
                ids.Add(elem.Id);
            }
            select.SetElementIds(ids);
            trans.Commit();

            return Result.Succeeded;
        }
    }
}
