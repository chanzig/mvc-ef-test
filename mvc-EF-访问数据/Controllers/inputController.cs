
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using mvc_EF_访问数据.Models;

namespace mvc_EF_访问数据.Controllers
{
    public class inputController : Controller
    {
        private MyEnDbContest db = new MyEnDbContest();

        public ActionResult cookieTest( string c1)
        {
            c1 =string.IsNullOrWhiteSpace(c1) ? "c1 为空" : "c1 = " + c1 ;
            ViewBag.c1 = c1;
            return View();
        }
    
        public ActionResult FormTest() 
        {
            return View();
        }

        [HttpPost]
        public ActionResult FormTest(string strInput)
        {
            ViewBag.msg = strInput;
            return View();
        }


        public ActionResult entityTest()
        {
         
            return View();
        }
        [HttpPost]
        public ActionResult entityTest(MyEntity entity) 
        {
            ViewBag.Entity = entity;
            if (ModelState.IsValid)
            {
                db.myentity.Add(entity);
                db.SaveChanges();
            }
            
            return View();
        }
    }
}
