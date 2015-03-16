using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc_EF_访问数据.Models;

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

        //
        // GET: /Student/Details/5

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
    }
}