using AspNetCore.DataTables.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.DataTables.Web.Data
{
    public class CompanyContext
    {
        public static IList<Company> Companies { get; set; } = new List<Company>()
        {
            new Company { Id = 1, Name = "Microsoft", Address = "Redmond", Town = "USA"},
            new Company { Id = 2, Name = "Google", Address = "Mountain View", Town = "USA"},
            new Company { Id = 3, Name = "Gowi", Address = "Pancevo", Town = "Serbia"}

        };
    }
}
