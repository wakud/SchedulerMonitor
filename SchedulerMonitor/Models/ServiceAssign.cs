using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchedulerMonitor.Models
{
    public class ServiceAssign
    {
        public int Id { get; set; }
        public int PointId { get; set; }            //айді точки
        public string PointName { get; set; }       //назва точки
        public DateTime Start { get; set; }         //початок сеансу
        public string StartStr { get; set; }        //текстовий формат початку
        public DateTime End { get; set; }           //кінець сеансу
        public string EndStr { get; set; }          //текстовий формат кінця
        public string Length { get; set; }           //Тривалість
        public string Performer { get; set; }       //ПІБ виконавця
        public string PersonName { get; set; }      //ПІБ замовника
        public string ArticleName { get; set; }     //назва послуги
        public string Description { get; set; }     //опис
    }
}