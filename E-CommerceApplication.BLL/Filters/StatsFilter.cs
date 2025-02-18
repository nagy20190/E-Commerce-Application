using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_CommerceApplication.BLL.Filters
{
    public class StatsFilter : IActionFilter
    {
        // executing before the execution of the action
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // create reference time
            DateTime reference = new DateTime(2020, 1, 21);
            TimeSpan time= DateTime.Now - reference;
            Console.WriteLine($"[Stats Filter OnActionExecuting = {time.TotalMilliseconds}] ms");

        }
        // executing after the execution of the action
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // create reference time
            DateTime reference = new DateTime(2020, 1, 21);
            TimeSpan time = DateTime.Now - reference;
            Console.WriteLine($"[Stats Filter OnActionExecuted = {time.TotalMilliseconds}] ms");
        }

    }
}
