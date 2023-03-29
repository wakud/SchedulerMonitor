using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchedulerMonitor.Models
{
    public class ServiceModel
    {
        public string PointName { get; set; }
        public string WeekDate { get; set; }
        public List<string> Days { get; set; }
        public List<ServiceAssign> Items { get; set; }
        public string getHeight(double bodySize, double value)
        {
            return (value*bodySize).ToString().Replace(",", ".") + "vh";
        }
    }
}