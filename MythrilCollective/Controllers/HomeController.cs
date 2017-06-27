using MythrilCollective.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MythrilCollective.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Rules()
        {
            return View();
        }
        public ActionResult SitePolicy()
        {
            return View();
        }
        public ActionResult Events()
        {
            return View();
        }
        public ActionResult UpcomingEvents()
        {
            return View(GetEvents());
        }



        public IList<Event> GetEvents()
        {
            IList<Event> result = new List<Event>();
            result = (new Comm.GoogleComm()).GetUpcoming();

            return result;
        }

        public ActionResult AddEvent()
        {
            (new Comm.GoogleComm()).GetColors();
            return View(new Event());
        }
        


        [HttpPost]
        public ActionResult AddEvent(Event cevent)
        {
            var title = cevent.Title;
            var code = cevent.OfficerCode;
            var desc = cevent.Description;
            bool suceeded = (new Comm.GoogleComm()).InsertEvent(ref cevent);
            if(suceeded)
            {
                RedirectToAction("Events");
            }
            ViewBag.Message = string.IsNullOrEmpty(cevent.Message) ? "COULD NOT SAVE EVENT" : cevent.Message;
            return View(cevent);
        }
    }
}
