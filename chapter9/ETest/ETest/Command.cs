using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using ETest;

namespace Command
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Program : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //commandData.Application.Application.DocumentChanged += appChange;//界面级别

            //commandData.Application.ActiveUIDocument.Document.DocumentClosing += docClosing;//报错，没有委托

            //MainWindow nWin = new MainWindow();//外部事件
            //nWin.Show();

            commandData.Application.Idling += IdlingTest;
            return Result.Succeeded;
        }

        /// <summary>
        /// 界面级别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // private void appChange(object sender, Autodesk.Revit.DB.Events.DocumentChangedEventArgs e)
        //{
        //    TaskDialog.Show("已改", "已改动");
        //}

        ///文档级别事件
        //private void docClosing(object sender, Autodesk.Revit.DB.Events.DocumentClosedEventArgs e)
        //{
        //    TaskDialog.Show("关闭", "已关闭");
        //}

        private void IdlingTest(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {
            UIApplication m_uiapp = sender as UIApplication;
            Autodesk.Revit.DB.Document m_doc = m_uiapp.ActiveUIDocument.Document;
            Transaction trans=new Transaction(m_doc,"idling");
            trans.Start();
            ElementId id=new ElementId(194987);
            TextNote tn = m_doc.GetElement(id) as TextNote;
            string str = tn.Text;
            int i = 0;
            int.TryParse(str, out i);
            tn.Text=(i+1).ToString();
            trans.Commit();
        }
    }

    public class ExternalCommand : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            TaskDialog.Show("测试", "测试中");
        }

        public string GetName()
        {
            return "名称";
        }
    }

}
