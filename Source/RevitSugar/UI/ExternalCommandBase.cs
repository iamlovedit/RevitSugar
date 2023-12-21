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
        protected Document RevitDoc { get; private set; }

        protected UIDocument RevitUidoc { get; private set; }

        protected Application RevitApp { get; private set; }

        protected UIApplication RevitUiApp { get; private set; }

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

        public virtual bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return applicationData.ActiveUIDocument?.Document != null;
        }

        protected virtual bool PreExecute()
        {
            return true;
        }
    }
}
