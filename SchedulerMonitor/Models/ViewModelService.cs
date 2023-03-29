using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchedulerMonitor.Models
{
    public class ViewModelService
    {
        public int PointID { get; set; }
        public List<ServiceAssign> ServiceAssign { get; set; }
        public DateTime DateNow { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Hour { get; set; }
        public string Name { get; set; }
        public int TimeReloadPage { get; set; }
        public string Text { get; set; }
    }
}