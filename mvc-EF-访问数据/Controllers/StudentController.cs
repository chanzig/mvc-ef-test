using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc_EF_访问数据.Models;
using System.IO;
using System.Text;
using mvc_EF_访问数据.Commons;
using mvc_EF_访问数据.ViewModel;


namespace mvc_EF_访问数据.Controllers
{ 
    public class StudentController : Controller
    {
        private StuDBContest db = new StuDBContest();

        //
        // GET: /Student/

        public ViewResult Index()
        {
            return View(db.Students.ToList());
        }
        public ViewResult list()
        {
            return View(db.Students.ToList());
        }
        public ActionResult GetUsers()
        {
 

            int dtStart = 0, dtDraw = 0, dtLlength = 0, dtOrder = 0;
            string dtSearchValue = "", dtOrderAscOrDes = "";

            try
            {
                dtStart = Int32.Parse(Request.QueryString["start"]);
                dtDraw = Int32.Parse(Request.QueryString["Draw"]);
                dtLlength = Int32.Parse(Request.QueryString["Length"]);
                dtOrder = Int32.Parse(Request.QueryString["order[0][column]"]);
                dtSearchValue = Request.QueryString["search[value]"];
                dtOrderAscOrDes = Request.QueryString["order[0][dir]"];
            }
            catch (Exception ex)
            {
                return View("dssssssssss");
            }

            UserInfoSearch search = null;

            if (!string.IsNullOrEmpty(dtSearchValue))
            {
                search = new UserInfoSearch
                {
                    StuStuNO = dtSearchValue,
                    StuName = dtSearchValue,
                };
            }
            UserInfoSort sort = null;

            switch (dtOrder)
            {
                case 0:
                    sort = new UserInfoSort
                    {
                        StuNO = dtOrderAscOrDes == "asc" ? OrderByDirection.Ascending : OrderByDirection.Descending
                    };
                    break;
                case 1:
                    sort = new UserInfoSort
                    {
                        StuName = dtOrderAscOrDes == "asc" ? OrderByDirection.Ascending : OrderByDirection.Descending
                    };
                    break;
                case 2:
                    sort = new UserInfoSort
                    {
                        StuPhoto = dtOrderAscOrDes == "asc" ? OrderByDirection.Ascending : OrderByDirection.Descending
                    };
                    break;
                case 3:
                    sort = new UserInfoSort
                    {
                        StuBirthday = dtOrderAscOrDes == "asc" ? OrderByDirection.Ascending : OrderByDirection.Descending
                    };
                    break;
                case 4:
                    sort = new UserInfoSort
                    {
                        StuAddress = dtOrderAscOrDes == "asc" ? OrderByDirection.Ascending : OrderByDirection.Descending
                    };
                    break;
            }

            var users = GetAllUserList(search, sort);

            var totalUsers = users.Count();


            users = users.Skip(dtStart).Take(dtLlength).ToList();

            return Json(new
            {
                draw = dtDraw,
                iTotalRecords = totalUsers,
                iTotalDisplayRecords = totalUsers,
                data = users
            }, JsonRequestBehavior.AllowGet);

        }

        public IEnumerable<StudentInfo> GetAllUserList(UserInfoSearch search, UserInfoSort sort)
        {

            var q = db.Students.ToList();

            #region Search
            if (search != null)
            {
                q = db.Students.Where(p => p.StuNO.Contains(search.StuStuNO) || p.StuName.Contains(search.StuName)).ToList();
            }
            #endregion

            #region Sort
            if (sort != null)
            {
                if (sort.StuNO != null)
                {
                    q = ((sort.StuNO == OrderByDirection.Ascending) ? q.OrderBy(t => t.StuNO) : q.OrderByDescending(t => t.StuNO)).ToList();
                }
                if (sort.StuName != null)
                {
                    q = ((sort.StuName == OrderByDirection.Ascending) ? q.OrderBy(t => t.StuName) : q.OrderByDescending(t => t.StuName)).ToList();
                }
                if (sort.StuBirthday != null)
                {
                    q = ((sort.StuBirthday == OrderByDirection.Ascending) ? q.OrderBy(t => t.StuBirthday) : q.OrderByDescending(t => t.StuBirthday)).ToList();
                }
                if (sort.StuAddress != null)
                {
                    q = ((sort.StuAddress == OrderByDirection.Ascending) ? q.OrderBy(t => t.StuAddress) : q.OrderByDescending(t => t.StuAddress)).ToList();
                }
                if (sort.StuPhoto != null)
                {
                    q = ((sort.StuPhoto == OrderByDirection.Ascending) ? q.OrderBy(t => t.StuPhoto) : q.OrderByDescending(t => t.StuPhoto)).ToList();
                }
            #endregion
            }
                return q;
        }
        

        
         //GET: /Student/Details/5

        public ViewResult Details(int id)
        {
            StudentInfo studentinfo = db.Students.Find(id);
            return View(studentinfo);
        }

        //
        // GET: /Student/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Student/Create

        [HttpPost]
        public ActionResult Create(StudentInfo studentinfo)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(studentinfo);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(studentinfo);
        }
        
        //
        // GET: /Student/Edit/5
 
        public ActionResult Edit(int id)
        {
            StudentInfo studentinfo = db.Students.Find(id);
            return View(studentinfo);
        }

        //
        // POST: /Student/Edit/5

        [HttpPost]
        public ActionResult Edit(StudentInfo studentinfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentinfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(studentinfo);
        }

        //
        // GET: /Student/Delete/5
 
        public ActionResult Delete(int id)
        {
            StudentInfo studentinfo = db.Students.Find(id);
            return View(studentinfo);
        }

        //
        // POST: /Student/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            StudentInfo studentinfo = db.Students.Find(id);
            db.Students.Remove(studentinfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult MM() {
            MemoryStream stream = new MemoryStream();
            ExportCsv(stream);
            var bytes = stream.ToArray();
            return File(bytes, "text/csv", "用户信息.csv");
          
        }
        public ActionResult Export()
        {

            MemoryStream stream = new MemoryStream();
            ExportCsv(stream);
            var bytes = stream.ToArray();
            return File(bytes, "text/csv", "用户信息.csv");
        }
        /// <summary>
        /// Controller Services
        /// </summary>
        /// <param name="stream"></param>
        public void ExportCsv(Stream stream)
        {
            var data = db.Students.ToList();
            CsvSerializer<StudentInfo> serializer = new CsvSerializer<StudentInfo>();
            serializer.UseLineNumbers = false;
            string s = serializer.Serialize(data, new[] 
            {   
                "ID",
                "StuNO",
                "StuName",
                "StuPhoto",
                "StuBirthday",
                "StuAddress"
            });
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            byte[] bom = new[] { (byte)0xEF, (byte)0xBB, (byte)0xBF };
            stream.Write(bom, 0, bom.Length);
            stream.Write(bytes, 0, bytes.Length);
        }
        
    }
}