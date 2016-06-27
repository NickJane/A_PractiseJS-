using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.DAL;
using WebApplication2.Models;
using WebApplication2.Repository;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            IRepository<SysUser> _repo = new EfRepository<SysUser>(new XEngineContext());
            _repo.GetSysUsers();
            return View();
        }
	}
}