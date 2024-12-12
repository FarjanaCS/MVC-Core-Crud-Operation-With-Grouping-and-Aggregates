using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Work_01.Models
{
    public enum Gender { Male = 1, Female }
    public class Employee
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
        [Required, StringLength(50)]
        public string Picture { get; set; } = default!;
        //Navigation
        public virtual ICollection<Qualification> Qualifications { get; set; } = new List<Qualification>();

    }
    public class Qualification

    {
        public int QualificationId { get; set; }
        [Required, DataType(DataType.Date)]
        public int? PassingYear { get; set; }
        [Required]
        public string Degree { get; set; }= default!;
        //FK
        [Required, ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        //Navigation
        public virtual Employee? Employee { get; set; } = default!;
    }
    public class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : DbContext(options)
    {


        public DbSet<Employee> Employees { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
    }

}
