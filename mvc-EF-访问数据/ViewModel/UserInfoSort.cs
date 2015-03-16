using mvc_EF_访问数据.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvc_EF_访问数据.ViewModel
{
    public class UserInfoSort
    {
        public OrderByDirection? StuNO { get; set; }
        public OrderByDirection? StuName { get; set; }
        public OrderByDirection? StuPhoto { get; set; }
        public OrderByDirection? StuBirthday { get; set; }
        public OrderByDirection? StuAddress { get; set; }
    }
}