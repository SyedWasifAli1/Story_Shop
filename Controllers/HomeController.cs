using Book_kharido.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Humanizer;

namespace Book_kharido.Controllers
{
    public class HomeController : Controller
    {
        public Project Project { get; }
        public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; }

        public HomeController(Project project, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            Project = project;
            Environment = environment;
        }
        public static String encrypt(string password)
        {
            byte[] storepassword=ASCIIEncoding.ASCII.GetBytes(password);    
            string pass=Convert.ToBase64String(storepassword);
            return pass;
        }

        public IActionResult SomeAction()
        {
            // Assuming you have set some value in the session in a different action
            HttpContext.Session.SetString("User", "someValue");

            // Rest of your logic
            return View();
        }
        public IActionResult AdminLogin()
        {
            HttpContext.Session.SetString("Admin", "adminValue");
            // Rest of your logic
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(User user)
        {
            /*var myuser = Project.Users.Where(y => y.Email == user.Email && y.Password == encrypt(user.Password)).FirstOrDefault();
*/
            var myuser = Project.Users.Where(y => y.Email == user.Email && y.Password == user.Password).FirstOrDefault();


            if (myuser != null)
            {
                if (myuser.Roll == "Admin")
                {
                    HttpContext.Session.SetString("Admin", myuser.Name);
                    HttpContext.Session.SetInt32("Adminid", myuser.Id);

                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (myuser.Roll == "User")
                {
                    HttpContext.Session.SetString("User", myuser.Name);
                    HttpContext.Session.SetInt32("Userid", myuser.Id);
                    return RedirectToAction("Home", "Users");

                }
            }
            else
            {
                    return RedirectToAction("Register", "Home");

            }
            return View();

        }


        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User users)
        {
            if (ModelState.IsValid)
            {
                if (Project.Users.Any(y => y.Email == users.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered");
                    return View(); 
                }
                users.Password =encrypt(users.Password);
                users.OTP = GenerateOTPCode();
                users.TownsAId = 1;
                Project.Users.Add(users);
                Project.SaveChanges();
                var senderEmail = new MailAddress("syedwasifali080@gmail.com", "Wasif");
                var receiverEmail = new MailAddress(users.Email, "Receiver");
                var password = "hlnt odjp quhd mgwh";
                //var password = "vede mrnf kzgi wuvw";
                var sub = "Story Shop";
                var body = "OTP" + users.OTP;
               
                var smtp = new SmtpClient   
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = sub,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(mess);
                }

                return RedirectToAction("Otp", "Home");
                

                //var path = Environment.WebRootPath;
                //var filepath = "Profile/" + users.FormFile.FileName;
                //var fullpath = Path.Combine(path, filepath);
                //ProfilePic(users.FormFile, fullpath);
                //var data = new User()
                //{
                //    Name = users.Name,
                //    Email = users.Email,
                //    Password = encrypt(users.Password),
                //    Address = users.Address,
                //    PhoneNumber = users.PhoneNumber,
                //    CountryId = users.CountryId,
                //    CityId = users.CityId,
                //    Roll = users.Roll,
                //    Picture = fullpath

                //};

                //users.Picture = fullpath;



            }

            return View();
        }

        [HttpPost]
        public IActionResult Subcriber(string names, string email)
        {

            AddSubscriber(names, email);

            var senderEmail = new MailAddress("syedwasifali080@gmail.com", "Wasif");
            var receiverEmail = new MailAddress(email, "Receiver");
            //var password = "szgq evwr xlbf xqdl";
            var password = "hlnt odjp quhd mgwh";
            var sub = "Story Shop";
            var body = "Thanks for Subcriber";

            var smtp = new SmtpClient


            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = sub,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(mess);
            }



            // Redirect to another action or URL
            return RedirectToAction("Home", "Users");
        }


        private void AddSubscriber(string names, string email)
        {
            var subcrib = new Subcriber
            {
                SubName = names,
                Email = email
            };

            Project.Subcribers.Add(subcrib);
            Project.SaveChanges();

        }

        private int GenerateOTPCode()
        {

            return new Random().Next(100000, 999999);
        }

   
public IActionResult Otp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Otp(int Otp)
        {
             var myotp = Otp;
            var myuser = Project.Users.Where(y => y.OTP == myotp).FirstOrDefault();
            if (myuser != null)
            {
                if (myuser.Roll == "Admin")
                {
                    HttpContext.Session.SetString("Admin", myuser.Name);
                    HttpContext.Session.SetInt32("Adminid", myuser.Id);


                    return RedirectToAction("Index", "Admin");
                }
                else if (myuser.Roll == "User")
                {
                    HttpContext.Session.SetString("User", myuser.Name);
                    HttpContext.Session.SetInt32("Userid", myuser.Id);
                    return RedirectToAction("Home", "Users");

                }
            }
            else
            {
                return RedirectToAction("Register", "Home");

            }
            return View();

        }

        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                HttpContext.Session.Remove("Admin");

                return RedirectToAction("Index", "Home");
            }

            else if (HttpContext.Session.GetString("User") != null)
            {
                HttpContext.Session.Remove("User");

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        private void ProfilePic(IFormFile file, string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);
            file.CopyTo(fileStream);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
