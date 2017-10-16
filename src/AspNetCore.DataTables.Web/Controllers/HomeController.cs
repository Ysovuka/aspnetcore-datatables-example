using AspNetCore.DataTables.Web.Models;
using AspNetCore.DataTables.Web.Models.DataTables;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCore.DataTables.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AjaxHandler([FromBody] AjaxPostModel viewModel)
        {
            var context = new Data.CompanyContext();
            var unfilteredCompanies = context.Companies.ToList();
            IEnumerable<Company> filteredCompanies = unfilteredCompanies.Filter(viewModel.columns, viewModel.search.value);

            IOrderedEnumerable<Company> orderedFilteredCompanies = filteredCompanies.Sort(viewModel.order, viewModel.columns);
            
            orderedFilteredCompanies
                .Skip(viewModel.start)
                .Take(viewModel.length);

            return Json(new
            {
                draw = viewModel.draw,
                recordsTotal = unfilteredCompanies.Count,
                recordsFiltered = orderedFilteredCompanies.Count(),
                data = orderedFilteredCompanies
            });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
