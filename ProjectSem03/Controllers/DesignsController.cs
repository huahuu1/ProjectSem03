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
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.AspNetCore.HttpOverrides;
namespace ProjectSem03.Controllers
{
    public class DesignsController : Controller
    {
        //step 2
        ProjectDB db;
        public DesignsController(ProjectDB db)
        {
            this.db = db;
        }

        public IActionResult Index(string dname)
        {
            if (HttpContext.Session.GetString("staffId") == null) //check session
            {
                return RedirectToAction("Login");
            }
            else
            {
                var list = from d in db.Design
                           join e in db.Exhibition
                           on d.ExhibitionID equals e.ExhibitionId into groupjoin
                           from e in groupjoin.DefaultIfEmpty()
                           select new CombineModels
                           {
                               Designs = d,
                               Exhibitions = e
                           };
                if (string.IsNullOrEmpty(dname))
                {
                    return View(list);
                }
                else
                {
                    var filter = list.Where(d => d.Designs.DesignName.Contains(dname));
                    return View(filter);
                }

            }
        }


        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetInt32("staffRole") == 2)
            {
                var exhibition = db.Exhibition.ToList();
                ViewBag.data = new SelectList(exhibition, "ExhibitionId", "ExhibitionName");

                var design = db.Design.Find(id);
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
                return RedirectToAction("Index", "Staffs");
            }
        }
        [HttpPost]
        public IActionResult Edit(Design design)
        {
            var exhibition = db.Exhibition.ToList();
            ViewBag.data = new SelectList(exhibition, "ExhibitionID", "ExhibitionName");

            try
            {
                var editDesign = db.Design.SingleOrDefault(d => d.DesignId.Equals(design.DesignId));
                if (ModelState.IsValid)
                {
                    if (editDesign != null)
                    {
                        editDesign.ExhibitionID = design.ExhibitionID;
                        //editDesign.Price = design.Price;
                        db.SaveChanges();
                        return RedirectToAction("Index", "Designs");
                        //if (design.Price > 0)
                        //{
                            
                        //}
                        //else
                        //{
                        //    return RedirectToAction("Edit", "Designs");
                        //}
                    }
                    else
                    {
                        ViewBag.Msg = "Failed .......";
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;
            }
            return View();
        }
    }
}
