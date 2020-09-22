using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//step 1
using ProjectSem03.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartBreadcrumbs.Attributes;
using X.PagedList; //using for pagination

namespace ProjectSem03.Controllers
{
    public class DesignsController : Controller
    {
        //step 2
        //Connection to database
        ProjectDB db;
        public DesignsController(ProjectDB db)
        {
            this.db = db;
        }

        [Breadcrumb("Design List")]
        public IActionResult Index(string dname, int? page)
        {
            if (HttpContext.Session.GetString("staffId") == null) //check session
            {
                return RedirectToAction("Login");
            }
            else
            {
                //set number of records per page and starting page
                int maxsize = 5;
                int numpage = page ?? 1;

                //get combine list for Design
                var list = from d in db.Design
                           join e in db.Exhibition
                           on d.ExhibitionID equals e.ExhibitionId into groupjoin
                           from e in groupjoin.DefaultIfEmpty()
                           select new CombineModels
                           {
                               Designs = d,
                               Exhibitions = e
                           };
                var model = list.ToList().ToPagedList(numpage, maxsize); //pagination

                //check if result is found or not
                if (string.IsNullOrEmpty(dname)) //empty
                {
                    ViewBag.page = model;
                }
                else
                {
                    //show the result
                    var filter = list.Where(d => d.Designs.DesignName.ToLower().Contains(dname)).ToList().ToPagedList();
                    ViewBag.page = filter;
                }
                return View();
            }
        }

        [HttpGet]
        [Breadcrumb("Edit Design")]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetInt32("staffRole") == 2) //check session for Staff Role
            {
                var exhibition = db.Exhibition.ToList(); //get list of exhibitions
                ViewBag.data = new SelectList(exhibition, "ExhibitionId", "ExhibitionName");

                var design = db.Design.Find(id); //Find design Id
                if (design != null)
                {
                    return View(design);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                //return to Index page of Staffs
                return RedirectToAction("Index", "Staffs");
            }
        }
        [HttpPost]
        public IActionResult Edit(Design design) //edit design
        {
            var exhibition = db.Exhibition.ToList();
            ViewBag.data = new SelectList(exhibition, "ExhibitionID", "ExhibitionName");

            try
            {
                var editDesign = db.Design.SingleOrDefault(d => d.DesignId.Equals(design.DesignId)); //check Design Id
                if (ModelState.IsValid)
                {
                    if (editDesign != null)
                    {
                        //edit Design values
                        editDesign.ExhibitionID = design.ExhibitionID;
                        db.SaveChanges(); // save changes
                        return RedirectToAction("Index", "Designs"); //return to Index page of Designs
                    }
                    else
                    {
                        ViewBag.Msg = "Failed ......."; //show error message if Design Id is not found
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message; //show other error messages
            }
            return View();
        }
    }
}
