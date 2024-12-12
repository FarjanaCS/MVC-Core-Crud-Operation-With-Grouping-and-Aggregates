using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Work_01.Models;

namespace Work_01.ViewModels
{
    public class EmployeeEditModel
    {
        public int EmployeeId { get; set; }
        [Required, StringLength(50)]
        public string EmployeeName { get; set; } = default!;
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
        [Required, StringLength(50)]
        public string Address { get; set; } = default!;
        [Required, Column(TypeName = "money")]
        public decimal? Salary { get; set; }
        [Required, Column(TypeName = "date")]
        public DateTime JoiningDate { get; set; }


        public bool IsaCurrentEmployee { get; set; }
        
        public IFormFile? Picture { get; set; } = default!;
        //Navigation
        public List<Qualification> Qualifications { get; set; } = new List<Qualification>();
    }
}
