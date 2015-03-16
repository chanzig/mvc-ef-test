using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvc_EF_访问数据.Models
{
    public class StudentInfo
    {
        public int ID { get; set; }
        public string StuNO { get; set; }
        public string StuName { get; set; }
        public string StuPhoto { get; set; }
        public DateTime StuBirthday { get; set; }
        public string StuAddress { get; set; }
    }
}