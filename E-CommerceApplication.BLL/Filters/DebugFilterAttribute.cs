using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_CommerceApplication.BLL.Filters
{
    public class DebugFilterAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            DateTime reference = new DateTime(2020, 1, 21);
            TimeSpan time = DateTime.Now - reference;
            Console.WriteLine($"[Stats Filter OnActionExecuting = {time.TotalMilliseconds}] ms");
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            DateTime reference = new DateTime(2020, 1, 21);
            TimeSpan time = DateTime.Now - reference;
            Console.WriteLine($"[Stats Filter OnActionExecuted = {time.TotalMilliseconds}] ms");
        }

       
    }
}
