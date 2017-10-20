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
            
            var unfilteredCompanies = Data.CompanyContext.Companies.ToList();
            IEnumerable<Company> filteredCompanies = unfilteredCompanies.Filter(viewModel.columns, viewModel.search.value);

            IEnumerable<Company> orderedFilteredCompanies = filteredCompanies.Sort(viewModel.order, viewModel.columns);

            var results = orderedFilteredCompanies
                .Skip(viewModel.start)
                .Take(viewModel.length);

            return Json(new
            {
                draw = viewModel.draw,
                recordsTotal = unfilteredCompanies.Count,
                recordsFiltered = orderedFilteredCompanies.Count(),
                data = results
            });
        }

        [HttpPost]
        public IActionResult SaveRowDetails([FromBody] Company model)
        {
            var company = Data.CompanyContext.Companies.FirstOrDefault(c => c.Id == model.Id);
            if (company != null)
            {
                Data.CompanyContext.Companies.Remove(company);
                Data.CompanyContext.Companies.Add(model);
            }

            return Json(new { success = true });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
