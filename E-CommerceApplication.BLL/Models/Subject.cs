using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Interfaces;

namespace E_CommerceApplication.BLL.Models
{
    public class Subject : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
