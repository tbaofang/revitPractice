using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace EventTest
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Program : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            commandData.Application.Application.DocumentChanged += appChange;
            return Result.Succeeded;
        }

        private void appChange(object sender, Autodesk.Revit.DB.Events.DocumentChangedEventArgs e)
        {
            TaskDialog.Show("已改", "已改动");
        }
    }
}
