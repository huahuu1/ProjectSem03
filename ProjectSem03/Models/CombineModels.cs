using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSem03.Models
{
    public class CombineModels
    {
        public Competition Competitions { get; set; }
        public Design Designs { get; set; }
        public Posting Postings { get; set; }
        public Staff Staffs { get; set; }
        public Student Students { get; set; }
        public Award Awards { get; set; }
        public Exhibition Exhibitions { get; set; }
        public Display Displays { get; set; }
    }
}
