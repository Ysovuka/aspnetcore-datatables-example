using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.DataTables.Web.Models.DataTables
{
    public class AjaxPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<Column> columns { get; set; } = new List<Column>();
        public List<Order> order { get; set; } = new List<Order>();
        public Search search { get; set; } = new Search();
    }
}
