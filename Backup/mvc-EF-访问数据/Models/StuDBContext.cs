using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace mvc_EF_访问数据.Models
{
    public class StuDBContest : DbContext
    {
        public StuDBContest()
            : base("DataConn")
        {
        }

        public DbSet<StudentInfo> Students { get; set; }
    }
}