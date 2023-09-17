using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pract4.DAL.Entities
{
    public class StudentCard
    {
        [Key]
        public int Id { get; set; }

        public string IdNumber { get; set; }

        public DateTime DateOfIssue { get; set; }

        public bool Status { get; set; }

        public virtual Student Student { get; set; }
    }
}
