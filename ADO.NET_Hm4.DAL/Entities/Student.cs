using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pract4.DAL.Entities
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        [StringLength(100)]
        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; } 

        public string Address { get; set; }

        public StudentCard StudentCard { get; set; }
    }
}
