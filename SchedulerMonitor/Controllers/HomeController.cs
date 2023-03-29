using SchedulerMonitor.Models;
using SchedulerMonitor.Utility;
using System;
using System.Web.Mvc;

namespace SchedulerMonitor.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int? pointId)
        {
            if (!pointId.HasValue)
            {
                int.TryParse(Properties.Settings.Default.pointID, out int settingPointID);
                pointId = settingPointID;
            }

            return InformationBoard(pointId.Value);
        }

        [HttpPost]
        public ActionResult InformationBoard(int pointId)
        {
            decimal start = DataServiceAssign.GetPointName(pointId).Start;
            decimal end = DataServiceAssign.GetPointName(pointId).End;
            int.TryParse(Properties.Settings.Default.timeReloadPage, out int timeReloadPage);
            int j = (int)Math.Ceiling(end/60) - (int)Math.Ceiling(start/60);

            ViewModelService viewService = new ViewModelService ()
            {
                DateNow = DateTime.Now,
                Start = (int)Math.Ceiling(start/60),
                End = (int)Math.Ceiling(end/60),
                PointID = pointId,
                Hour = j,
                Name = DataServiceAssign.GetPointName(pointId).Name,
                TimeReloadPage = timeReloadPage
            };

            return View("InformationBoard", viewService);
        }

        public JsonResult GetList(int pointId)
        {
            return Json(DataServiceAssign.GetServiceAssignData(pointId));
        }
    }
}