using bilgi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace bilgi.Controllers
{
    public class HomeController : Controller
    {



        public ActionResult Index()
        {
            // DbContext instance oluşturuyoruz
            using (var report = new ziyaretciEntities3())
            {
                // Veritabanından News tablosundaki tüm verileri alıyoruz
                var newsList = report.News.ToList();

                // Verileri view'a gönderiyoruz
                return View(newsList);
            }
        }



        /// <summary>
        /// ///////////////////////////////////////////////////////////////////7
        /// </summary>

        private readonly ziyaretciEntities3 _context = new ziyaretciEntities3();

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [HttpPost]
        public ActionResult SubmitContactForm(Contact model)
        {
            if (ModelState.IsValid)
            {
                // Add the contact form data to the database using the existing context
                _context.Contact.Add(model);
                _context.SaveChanges();

                // Optionally, add a success message to ViewBag or TempData
                ViewBag.Message = "Your message has been sent successfully.";

                // Redirect to the same page to clear the form (or redirect to a success page)
                return RedirectToAction("Contact");
            }

            // If model state is not valid, return the same view with the data entered
            return View("Contact", model);
        }



        //////////////////////////////////////////////////////////////////////

        public ActionResult Login()
        {
            return View();
        }



        ziyaretciEntities3 db = new ziyaretciEntities3();


        [HttpGet]
        public ActionResult Login_Visitor()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login_Visitor(Visitor visit)
        {
            var visitor = db.Visitor.FirstOrDefault(v => v.Email == visit.Email && v.Password == visit.Password);
            if (visitor != null)
            {
                // Store the visitor's email in the session
                Session["VisitorEmail"] = visitor.Email;

                // Redirect to another action or page on successful login
                return RedirectToAction("Visitor_Main");
            }
            else
            {
                // Add an error message to the ViewBag to display it in the view
                ViewBag.ErrorMessage = "The email or password you are using may be incorrect.";
                return View();
            }
        }

        //////////////////////////////////////////////////////////////////////

        public ActionResult Login_Admin()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login_Admin(Admin model)
        {
            if (ModelState.IsValid)
            {
                var admin = _dbContext.Admin.FirstOrDefault(a => a.Email == model.Email && a.Password == model.Password);
                if (admin != null)
                {
                    // Store Admin's name, surname, and email in session variables

                    Session["AdminId"] = admin.Id_a;
                    Session["AdminName"] = admin.Name;
                    Session["AdminSurname"] = admin.Surname;
                    Session["AdminEmail"] = admin.Email;

                    return RedirectToAction("Admin_Main");
                }
                else
                {
                    // Add an error message to the ViewBag to display it in the view
                    ViewBag.ErrorMessage = "Email or password you are using may be wrong.";
                    return View();
                }
            }
            return View(model);
        }



        //////////////////////////////////////////////////////////////////////

        public ActionResult Login_Employee()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login_Employee(Employee employee)
        {
            var employee_1 = db.Employee.FirstOrDefault(v => v.Email == employee.Email && v.Password == employee.Password);
            if (employee_1 != null)
            {
                // Employee bilgilerini session'a kaydet
                Session["EmployeeName"] = employee_1.Name;
                Session["EmployeeSurname"] = employee_1.Surname;
                Session["EmployeeEmail"] = employee_1.Email;

                // Başarılı giriş sonrası Employee_Main sayfasına yönlendir
                return RedirectToAction("Employee_Main", "Home"); // Controller adı ve aksiyon adı kontrol edin
            }
            else
            {
                // Hata mesajı gönder
                ViewBag.ErrorMessage = "Email or password you are using may be wrong.";
                return View();
            }
        }







        ////////////////////////////////////////////////////////////////////////////





        public ActionResult SeeAllMessages()
        {
            // Retrieve all contact messages from the database
            var messages = _context.Contact.ToList();

            // Pass the messages to the view
            return View(messages);
        }

        public ActionResult DeleteMessage(int id)
        {
            // Veritabanından mesajı sil
            var message = _context.Contact.Find(id);
            if (message != null)
            {
                _context.Contact.Remove(message); // Mesajı sil
                _context.SaveChanges(); // Değişiklikleri kaydet
            }

            return RedirectToAction("SeeAllMessages"); // Silme işleminden sonra mesajların listesini göster
        }

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// </summary>


        private ziyaretciEntities3 db_1 = new ziyaretciEntities3();

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Visitor visitor)
        {
            if (ModelState.IsValid)
            {
                db_1.Visitor.Add(visitor);
                db_1.SaveChanges();
                return RedirectToAction("Login"); // Redirect to a success page or another action
            }
            return View(visitor);

        }

        ////////////////////////////////////////////////////////////////////////////




        public ActionResult Visitor_Main()
        {
            string visitorEmail = Session["VisitorEmail"] as string;
            if (string.IsNullOrEmpty(visitorEmail))
            {
                // Session süresi dolduysa ya da kullanıcı giriş yapmadıysa login sayfasına yönlendir
                return RedirectToAction("Login_Visitor");
            }

            // Giriş yapan ziyaretçinin verilerini çek
            var visitor = db.Visitor.FirstOrDefault(v => v.Email == visitorEmail);
            if (visitor == null)
            {
                return HttpNotFound();
            }

            // ViewBag ile isim ve soyisimi View'a gönder
            ViewBag.VisitorFullName = visitor.Name + " " + visitor.Surname;

            // Visitor modelini View'a gönder
            return View(visitor);
        }


        public ActionResult VisitorDashboard()
        {
            return View();
        }



        ////////////////////////////////////////////////////////////////////////////




        public ActionResult MakeAppointment()
        {
            // Session'dan ziyaretçinin email'ini alıyoruz
            string email = (string)Session["VisitorEmail"];

            if (string.IsNullOrEmpty(email))
            {
                // Eğer email oturumda yoksa (giriş yapılmamışsa), login sayfasına yönlendir
                return RedirectToAction("Login_Visitor");
            }

            // Email bilgisini ViewBag ile View'a gönderiyoruz
            ViewBag.Email = email;

            return View();
        }

        [HttpPost]
        public ActionResult Appointment(Visitor_Appointment appointment)
        {
            if (ModelState.IsValid) // Check if the model is valid
            {
                // Get the visitor's email from the session
                string visitorEmail = (string)Session["VisitorEmail"];
                appointment.Email = visitorEmail; // Set the email for the appointment

                // Set the appointment date
                appointment.Date = appointment.Date.HasValue ? appointment.Date.Value : DateTime.Now;

                // Save to the database
                appoint.Visitor_Appointment.Add(appointment);
                appoint.SaveChanges();

                // Set a confirmation message
                TempData["SuccessMessage"] = "Your appointment has been successfully made.";

                // Redirect to the confirmation page
                return RedirectToAction("AppointmentConfirmation");
            }

            // If the model state is not valid, show the form again
            return View("MakeAppointment", appointment);
        }

        public ActionResult AppointmentConfirmation()
        {
            // Pass the success message to the view if available
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View();
        }


        private ziyaretciEntities3 appoint = new ziyaretciEntities3(); // Veritabanı bağlamı (DbContext sınıfını kendi adınla değiştir)


        public ActionResult MyAppointments()
        {
            // Ziyaretçinin email'ini session'dan al
            string visitorEmail = (string)Session["VisitorEmail"];

            if (string.IsNullOrEmpty(visitorEmail))
            {
                // Eğer email oturumda yoksa (giriş yapılmamışsa), login sayfasına yönlendir
                return RedirectToAction("Login_Visitor");
            }

            // Veritabanından ziyaretçiye ait randevuları çek
            var appointments = appoint.Visitor_Appointment
                                      .Where(a => a.Email == visitorEmail)
                                      .ToList();

            // Model olarak view'e gönder
            return View(appointments);
        }







        ////////////////////////////////////////////////////////////////////////////





        /// ////////////////////////////////////////////////////////////////////////////////////////

        // GET: EditVisitor
        public ActionResult EditVisitor()
        {
            // Kullanıcının e-postasını al
            var email = Session["VisitorEmail"]?.ToString(); // Oturumdan email'i al

            if (string.IsNullOrWhiteSpace(email))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Ziyaretçiyi email ile al
            var visitor = db.Visitor.SingleOrDefault(v => v.Email == email);
            if (visitor == null)
            {
                return HttpNotFound();
            }

            var model = new Visitor
            {
                Id_v = visitor.Id_v,
                Name = visitor.Name,
                Surname = visitor.Surname,
                Tc = visitor.Tc,
                Tel = visitor.Tel,
                Email = visitor.Email,
                Address = visitor.Address,
                Password = visitor.Password
            };

            return View(model);
        }


        // POST: EditVisitor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVisitor(Visitor model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcının e-postasını al
                var email = Session["VisitorEmail"]?.ToString(); // Oturumdan email'i al

                if (string.IsNullOrWhiteSpace(email))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Ziyaretçiyi email ile al
                var visitor = db.Visitor.SingleOrDefault(v => v.Email == email);
                if (visitor == null)
                {
                    return HttpNotFound();
                }

                visitor.Name = model.Name;
                visitor.Surname = model.Surname;
                visitor.Tc = model.Tc;
                visitor.Tel = model.Tel;
                visitor.Email = model.Email; // Email'i güncelleyebilirsiniz
                visitor.Address = model.Address;

                // Şifre güncelleme
                if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != visitor.Password)
                {
                    visitor.Password = model.Password; // Gerçek uygulamada şifreyi hash'leyin
                }

                db.SaveChanges();

                return RedirectToAction("Visitor_Main"); // Güncellemeden sonra ana sayfaya yönlendir
            }

            return View(model); // Model geçersizse, aynı modeli tekrar göster
        }




        //////////////////////////////////////////////////////////////////////////////


        private ziyaretciEntities3 report = new ziyaretciEntities3();

        [HttpPost]
        public ActionResult PublishReport(string Content, int SenderId)
        {
            if (!string.IsNullOrEmpty(Content))
            {
                try
                {
                    // Create a new News entry
                    News newReport = new News
                    {
                        Content = Content,
                        SenderId = SenderId.ToString(),  // Admin'in Id'sini kullanarak
                        PublishedDate = DateTime.Now
                    };

                    // Save the news to the database
                    report.News.Add(newReport);
                    report.SaveChanges();

                    TempData["SuccessMessage"] = "The report has been published successfully!";
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                    TempData["ErrorMessage"] = "An error occurred while publishing the report.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please write something before publishing.";
            }

            return RedirectToAction("Admin_Main");
        }


        public ActionResult Admin_Main()
        {
            // Get the Admin's name and surname from the session
            var adminName = Session["AdminName"] as string;
            var adminSurname = Session["AdminSurname"] as string;

            // Pass the Admin's full name to the view using ViewBag
            ViewBag.AdminFullName = $"{adminName} {adminSurname}";

            return View();
        }


        

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////
        /// </summary>

        private readonly ziyaretciEntities3 _dbContext;

        public HomeController()
        {
            _dbContext = new ziyaretciEntities3();
        }

        // This method fetches the employee data from your database
        public ActionResult ShowAllEmployees()
        {
            // Get employees from the database
            var employees = _dbContext.Employee.ToList();

            // Mask the password or exclude it from being shown in the view
            foreach (var employee in employees)
            {
                employee.Password = "******"; // Masking the password
            }

            return View(employees);
        }

        // This method handles the deletion of a visitor
        [HttpGet]
        public ActionResult DeleteEmployee(int id)
        {
            // Find the visitor with the given ID
            var employee = _dbContext.Employee.Find(id);
            if (employee == null)
            {
                // If no visitor is found, return a 404 error
                return HttpNotFound();
            }

            // Remove the visitor from the database
            _dbContext.Employee.Remove(employee);
            _dbContext.SaveChanges();

            // Redirect to the list of all visitors
            return RedirectToAction("ShowAllEmployees");
        }


        //////////////////////////////////////////////////////////////////////////////




        // This method fetches the employee data from your database
        public ActionResult ShowAllVisitors()
        {
            // Get employees from the database
            var visitors = _dbContext.Visitor.ToList();

            // Mask the password or exclude it from being shown in the view
            foreach (var visitor in visitors)
            {
                visitor.Password = "******"; // Masking the password
            }

            return View(visitors);
        }


        // This method handles the deletion of a visitor
        [HttpGet]
        public ActionResult DeleteVisitor(int id)
        {
            // Find the visitor with the given ID
            var visitor = _dbContext.Visitor.Find(id);
            if (visitor == null)
            {
                // If no visitor is found, return a 404 error
                return HttpNotFound();
            }

            // Remove the visitor from the database
            _dbContext.Visitor.Remove(visitor);
            _dbContext.SaveChanges();

            // Redirect to the list of all visitors
            return RedirectToAction("ShowAllVisitors");
        }


        public ActionResult SeeAllAppointments()
        {
            using (var db = new ziyaretciEntities3()) // Replace with your actual DbContext
            {
                var appointments = db.Visitor_Appointment.Include("Visitor").ToList();
                return View(appointments);
            }
        }

        public ActionResult DeleteAppointment(int id)
        {
            using (var db = new ziyaretciEntities3()) // Replace with your actual DbContext
            {
                // Veritabanında silinecek randevuyu bul
                var appointment = db.Visitor_Appointment.Find(id);
                if (appointment != null)
                {
                    db.Visitor_Appointment.Remove(appointment); // Randevuyu sil
                    db.SaveChanges(); // Değişiklikleri kaydet
                }
            }

            return RedirectToAction("SeeAllAppointments"); // Silme işlemi sonrası tüm randevuları göster
        }

        //////////////////////////////////////////////////////////////////////////////




        public ActionResult EditAdmin()
        {
            string adminEmail = Session["AdminEmail"] as string;
            if (string.IsNullOrEmpty(adminEmail))
            {
                return RedirectToAction("Login_Admin");
            }

            var admin = _dbContext.Admin.FirstOrDefault(a => a.Email == adminEmail);
            if (admin == null)
            {
                return HttpNotFound();
            }

            var model = new Admin
            {
                Id_a = admin.Id_a,
                Name = admin.Name,
                Surname = admin.Surname,
                Email = admin.Email,
                Password = admin.Password
            };

            return View(model);
        }




        [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult EditAdmin(Admin model)
            {
                if (ModelState.IsValid)
                {
                    var admin = _dbContext.Admin.SingleOrDefault(a => a.Id_a == model.Id_a);
                    if (admin == null)
                    {
                        return HttpNotFound();
                    }

                    admin.Name = model.Name;
                    admin.Surname = model.Surname;
                    admin.Email = model.Email;

                    if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != admin.Password)
                    {
                        admin.Password = model.Password; // Ensure password is hashed before saving
                    }

                    _dbContext.SaveChanges();
                    return RedirectToAction("Admin_Main");
                }
                return View(model);
            }



        ////////////////////////////////////////////////////////////////////////////////
        ///


        [HttpGet]
        public ActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdmin(Admin admin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db_1.Admin.Add(admin);
                    db_1.SaveChanges();
                    TempData["SuccessMessage"] = "Admin has been successfully created.";
                    return RedirectToAction("Admin_Main");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while saving the admin: " + ex.Message;
                    return View(admin);
                }
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Log errors to console or save them in a log file
                }
            }

            return View(admin);
        }



        ////////////////////////////////////////////////////////////////////////////////


        [HttpGet]
        public ActionResult CreateEmployee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db_1.Employee.Add(employee);
                    db_1.SaveChanges();
                    TempData["SuccessMessage"] = "Employee has been successfully created.";
                    return RedirectToAction("Admin_Main");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while saving the employee: " + ex.Message;
                    return View(employee);
                }
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View(employee);
        }





        /////////////////////////////////////////////////////////////////////////////////////////////


        [HttpGet]
        public ActionResult CreateVisitor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVisitor(Visitor visitor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db_1.Visitor.Add(visitor);
                    db_1.SaveChanges();
                    TempData["SuccessMessage"] = "Visitor has been successfully created.";
                    return RedirectToAction("Admin_Main");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while saving the visitor: " + ex.Message;
                    return View(visitor);
                }
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Log errors to console or save them in a log file
                }
            }

            return View(visitor);
        }



        /////////////////////////////////////////////////////////////////////////////////////////////



        public ActionResult Employee_Main()
        {
            if (Session["EmployeeEmail"] != null)
            {
                var employeeEmail = Session["EmployeeEmail"].ToString();
                var employee = db.Employee.FirstOrDefault(e => e.Email == employeeEmail);

                if (employee != null)
                {
                    ViewBag.EmployeeFullName = $"{employee.Name} {employee.Surname}";
                    return View();
                }
                else
                {
                    // Session'dan bilgi bulamazsa login sayfasına yönlendir
                    return RedirectToAction("Login_Employee", "Home");
                }
            }
            else
            {
                // Session boşsa login sayfasına yönlendir
                return RedirectToAction("Login_Employee", "Home");
            }
        }


        public ActionResult EditEmployee(int id)
        {
            var employee = _dbContext.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            var model = new Employee
            {
                Id_e = employee.Id_e,
                Name = employee.Name,
                Surname = employee.Surname,
                Tc = employee.Tc,
                Tel = employee.Tel,
                Email = employee.Email,
                Address = employee.Address,
                Department = employee.Department,
                Password = employee.Password // Consider if you want to show/hide this
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmployee(Employee model)
        {
            if (ModelState.IsValid)
            {
                var employee = _dbContext.Employee.SingleOrDefault(e => e.Id_e == model.Id_e);
                if (employee == null)
                {
                    return HttpNotFound();
                }

                employee.Name = model.Name;
                employee.Surname = model.Surname;
                employee.Email = model.Email;
                employee.Tc = model.Tc;
                employee.Tel = model.Tel;
                employee.Address = model.Address;
                employee.Department = model.Department;

                if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != employee.Password)
                {
                    employee.Password = model.Password; // Hash the password here
                }

                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Employee details updated successfully.";
                return RedirectToAction("Employee_Main");
            }
            return View(model);
        }



    }
}