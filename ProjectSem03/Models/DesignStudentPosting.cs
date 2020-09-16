using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSem03.Models
{
    public class DesignStudentPosting
    {
        public Student Student { get; set; }
        public Design Design { get; set; }
        public Posting Posting { get; set; }
    }
}
