using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;

namespace RevitSugar.UI
{
    /// <summary>
    /// 外部命令基类
    /// </summary>
    public abstract class ExternalCommandBase : IExternalCommand, IExternalCommandAvailability
    {
        /// <summary>
        /// 
        /// </summary>
        protected Document RevitDoc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected UIDocument RevitUidoc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected Application RevitApp { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected UIApplication RevitUiApp { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        protected abstract Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements);

        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitUidoc = commandData.Application.ActiveUIDocument;
            RevitDoc = RevitUidoc.Document;
            RevitApp = RevitDoc.Application;
            RevitUiApp = RevitUidoc.Application;

            if (PreExecute())
            {
                RefreshGraphicalView();
                return Execute(commandData, ref message, elements);
            }
            return Result.Cancelled;
        }

        private void RefreshGraphicalView()
        {
            try
            {
                if (RevitUidoc.ActiveView != null && RevitUidoc.ActiveGraphicalView != null &&
                    RevitUidoc.ActiveView.Id != RevitUidoc.ActiveGraphicalView.Id)
                {
                    RevitUidoc.ActiveView = RevitUidoc.ActiveGraphicalView;
                }
            }
            catch (Exception e)
            {
                Trace.Write(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationData"></param>
        /// <param name="selectedCategories"></param>
        /// <returns></returns>
        public virtual bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return applicationData.ActiveUIDocument?.Document != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool PreExecute()
        {
            return true;
        }
    }
}
