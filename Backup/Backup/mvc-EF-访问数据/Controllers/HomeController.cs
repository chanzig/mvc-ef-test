using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvc_EF_访问数据.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        //返回值类型 json
        public ActionResult jsonStr() 
        {
            var jsonObj = new
            {
                id = 1,
                Name = "lulu",
                sex = "men",
                age = "12"
            };
            return Json(jsonObj, JsonRequestBehavior.AllowGet);
        }
    }
}
