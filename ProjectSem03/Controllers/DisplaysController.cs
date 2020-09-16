using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectSem03.Controllers
{
    public class DisplaysController : Controller
    {
        ProjectDB db;
        public DisplaysController(ProjectDB db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var list = from ds in db.Display
                       join d in db.Design on ds.DesignID equals d.DesignId
                       join e in db.Exhibition on ds.ExhibitionID equals e.ExhibitionId
                       orderby ds.ExhibitionID

                       select new CombineModels
                       {
                           Displays = ds,
                           Exhibitions = e,
                           Designs = d
                       };
            return View(list);
        }

        public IActionResult Create()
        {
            var list = db.Exhibition.ToList();
            ViewBag.data = new SelectList(list, "ExhibitionId", "ExhibitionName");
            var list2 = db.Design.ToList();
            ViewBag.data2 = list2;
            return View();
        }
        [HttpPost]
        public IActionResult Create(int ExhibitionId, int[] DesignId)
        {
            var list = db.Exhibition.ToList();
            ViewBag.data = new SelectList(list, "ExhibitionId", "ExhibitionName");
            var list2 = db.Design.ToList();
            ViewBag.data2 = list2;

            try
            {
                foreach (int item in DesignId)
                {
                    Display display = new Display();
                    display.ExhibitionID = ExhibitionId;
                    display.DesignID = item;
                    db.Display.Add(display);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "Displays");
            }
            catch (Exception e)
            {
                ViewBag.msg = e.Message;
            }
            return View();
        }
    }
}
