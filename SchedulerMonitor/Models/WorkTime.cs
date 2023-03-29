using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchedulerMonitor.Models
{
    public class WorkTime
    {
        public decimal Start { get; set; }
        public decimal End { get; set; }
        public string Name { get; set; }
    }
}