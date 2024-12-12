using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Work_01.Models;
using Work_01.ViewModels;

namespace Work_01.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeDbContext db;
        private readonly IWebHostEnvironment env;
        public EmployeesController(EmployeeDbContext db, IWebHostEnvironment env) { this.db = db; this.env = env; }
        public IActionResult Index()
        {
            //Eager loading
            var data = db.Employees.Include(x => x.Qualifications).ToList();
            return View(data);
        }
        public IActionResult Aggregates()
        {
            return View(db.Employees.Include(x => x.Qualifications).ToList());
        }
        public IActionResult Grouping()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Grouping(string groupby)
        {
            if (groupby == "gender")
            {
                var data = db.Employees.GroupBy(x => x.Gender).ToList().Select(g => new GroupedData<Employee> { Key = g.Key.ToString(), Items = g }).ToList();
                return View("GroupingResult", data);
            }
            else if (groupby == "year-month")
            {
                var data = db.Employees.ToList().GroupBy(x => new { x.JoiningDate.Year, x.JoiningDate.Month }).Select(g => new GroupedData<Employee> { Key = $"{g.Key.Month}-{g.Key.Year}", Items = g }).ToList();
                return View("GroupingResult", data);
            }
            return NoContent();
        }
        public IActionResult Create()
        {
            var model = new EmployeeInputModel();
            model.Qualifications.Add(new Qualification { });
            
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(EmployeeInputModel model, string operation = "")
        {
            if (operation == "add")
            {
                model.Qualifications.Add(new Qualification { });
                foreach (var item in ModelState.Values)
                {
                    item.Errors.Clear();
                    item.RawValue = null;
                }
            }
            if (operation.StartsWith("remove"))
            {
                
                int index = int.Parse(operation.Substring(operation.IndexOf("_") + 1));
                model.Qualifications.RemoveAt(index);
                foreach (var item in ModelState.Values)
                {
                    item.Errors.Clear();
                    item.RawValue = null;
                }
            }
            if (operation == "insert")
            {
                if (ModelState.IsValid)
                {
                    var w = new Employee
                    {
                        EmployeeName = model.EmployeeName,
                        Gender = model.Gender,
                        Address = model.Address,    
                        Salary = model.Salary ?? 0,
                        JoiningDate = model.JoiningDate,

                        IsaCurrentEmployee = model.IsaCurrentEmployee
                    };
                    string ext = Path.GetExtension(model.Picture.FileName);
                    string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string filePath = Path.Combine(env.WebRootPath, "Pictures", f);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    model.Picture.CopyTo(fs);
                    fs.Close();
                    w.Picture = f;

                    db.Employees.Add(w);
                    db.SaveChanges();
                    foreach (var wl in model.Qualifications)
                    {
                       
                        db.Database.ExecuteSqlInterpolated($"EXECUTE InsertQualification {wl.PassingYear}, {wl.Degree}, {w.EmployeeId}");
                    }
                    return RedirectToAction("Index");
                }


            }
           
            return View(model);
        }
        public IActionResult Edit(int id)
        {
            var d = db.Employees.FirstOrDefault(x => x.EmployeeId == id);
            if (d == null) return NotFound();
            //Explicit loading
            db.Entry(d).Collection(x => x.Qualifications).Load();
            var data = new EmployeeEditModel
            {
                EmployeeId = d.EmployeeId,
                EmployeeName = d.EmployeeName,
                Gender = d.Gender,
                Address = d.Address,
                JoiningDate = d.JoiningDate,
                Salary = d.Salary,
                IsaCurrentEmployee = d.IsaCurrentEmployee


            };
            data.Qualifications = d.Qualifications.ToList();
            
            ViewBag.CurrentPic = d.Picture;
            return View(data);
        }
        [HttpPost]
        public IActionResult Edit(EmployeeEditModel model, string operation)
        {
            var d = db.Employees.FirstOrDefault(x => x.EmployeeId == model.EmployeeId);
            if (d == null) return NotFound();
            if (operation == "add")
            {
                model.Qualifications.Add(new Qualification { });
                foreach (var item in ModelState.Values)
                {
                    item.Errors.Clear();
                    item.RawValue = null;
                }
            }
            if (operation.StartsWith("remove"))
            {
                
                int index = int.Parse(operation.Substring(operation.IndexOf("_") + 1));
                model.Qualifications.RemoveAt(index);
                foreach (var item in ModelState.Values)
                {
                    item.Errors.Clear();
                    item.RawValue = null;
                }
            }
            if (operation == "update")
            {
                if (ModelState.IsValid)
                {

                    d.EmployeeName = model.EmployeeName;
                    d.Gender = model.Gender;
                    d.Address = model.Address;  
                    d.Salary = model.Salary ?? 0;
                    d.JoiningDate = model.JoiningDate;

                    d.IsaCurrentEmployee = model.IsaCurrentEmployee;

                    if (model.Picture != null)
                    {
                        string ext = Path.GetExtension(model.Picture.FileName);
                        string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string filePath = Path.Combine(env.WebRootPath, "Pictures", f);
                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        model.Picture.CopyTo(fs);
                        fs.Close();
                        d.Picture = f;
                    }


                    db.SaveChanges();
                    db.Database.ExecuteSqlInterpolated($"EXEC DeleteQualifications {d.EmployeeId}");
                    foreach (var wl in model.Qualifications)
                    {
                        
                        db.Database.ExecuteSqlInterpolated($"EXECUTE InsertQualification {wl.PassingYear}, {wl.Degree}, {d.EmployeeId}");
                    }
                    return RedirectToAction("Index");
                }


            }
           
            ViewBag.CurrentPic = d.Picture;
            return View(model);
        }
        public IActionResult Delete(int id)
        {
            if (!db.Employees.Any(x => x.EmployeeId == id)) return NotFound();
            db.Database.ExecuteSqlInterpolated($"EXEC DeleteEmployee {id}");
            db.SaveChanges(true);
            return RedirectToAction("Index");

          
        }
    }

}

