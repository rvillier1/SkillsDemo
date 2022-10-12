using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillsDemo.Models
{
    [Keyless]
    public class EmpTerr
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string TerritoryID { get; set; }
    }
}
