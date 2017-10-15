using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.DataTables.Web.Models;
using AspNetCore.DataTables.Web.Models.DataTables;

namespace AspNetCore.DataTables.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AjaxHandler(AjaxPostModel viewModel)
        {
            var context = new Data.CompanyContext();
            var allCompanies = context.Companies.ToList();

            return Json(new
            {
                draw = viewModel.draw,
                recordsTotal = allCompanies.Count,
                recordsFiltered = allCompanies.Count,
                data = allCompanies
            });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
