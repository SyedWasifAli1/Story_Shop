using Book_kharido.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Configuration;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using Microsoft.CodeAnalysis.Differencing;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;


namespace Book_kharido.Controllers
{
    public class Admin : Controller
    {
        public Project Project { get; }
        public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; }

        public Admin(Project project,Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            Project = project;
            Environment = environment;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult RecievedOrder()
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");

                var bill = Project.Bills
                                .Include(o => o.User)
                                .OrderByDescending(o => o.Ordertime)
                                .ToList();


                var orders = Project.Orders
                                .Include(o => o.Product)
                                    .ThenInclude(p => p.ProductCategory)
                               .Include(o => o.Product)
                                .ThenInclude(P => P.Publisher)
                                .Include(o => o.Product)
                                .ThenInclude(P => P.Writer)
                                .Include(o => o.User)
                                    .ToList();
                ViewBag.orders = orders;
                return View(bill);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }

        public IActionResult Contacts()
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");

                var project = Project.Contacts.Include(c => c.FAQ).Include(c => c.User);
                return View(project.ToList());

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }

        public IActionResult More()
        {
            return View();
        }


        public IActionResult AddProduct()
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");

            DropdownProductShow dropdownProductShow = new DropdownProductShow();

            dropdownProductShow.PublisherName = new List<SelectListItem>();
            dropdownProductShow.WriteName = new List<SelectListItem>();
            dropdownProductShow.ProductCategoryName = new List<SelectListItem>();

            var productCategory = Project.ProductCategories.ToList();
            var Writer = Project.Writers.ToList();
            var Publish = Project.Publishers.ToList();
            foreach (var Cate in productCategory)
            {
                dropdownProductShow.ProductCategoryName.Add(new SelectListItem
                {
                    Text = Cate.ProductCategoryName,
                    Value = Cate.ProductCategoryId.ToString()
                });
            }

            foreach (var writ in Writer)
            {
                dropdownProductShow.WriteName.Add(new SelectListItem
                {
                    Text = writ.WriteName,
                    Value = writ.WritetId.ToString()
                });
            }
            foreach (var Pub in Publish)
            {
                dropdownProductShow.PublisherName.Add(new SelectListItem
                {
                    Text = Pub.PublisherName,
                    Value = Pub.PublisherId.ToString()
                });
            }
            ViewBag.ProductCategoryName = dropdownProductShow.ProductCategoryName;

            ViewBag.WriterName = dropdownProductShow.WriteName;
            ViewBag.PublisherName = dropdownProductShow.PublisherName;
        
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }

        [HttpPost]
        public IActionResult AddProduct(Product products)
        {
            
            var productCode = GenerateProductCode();
            var path = Environment.WebRootPath;
            var filepath = "Image/"+products.ProductFile.FileName;
            var fullpath = Path.Combine(path, filepath);
            Productfile(products.ProductFile, fullpath);
            var data = new Product()
            {
                ProductCode = productCode,
                ProductName =products.ProductName,
                ProductIamge=filepath,
                ProductPrice = products.ProductPrice,
                ProductQuantity=products.ProductQuantity,
                Page = products.Page,
                ProductDescription=products.ProductDescription,
                ProductCategoryId=products.ProductCategoryId,
                ProductDiscount= products.ProductDiscount,
                PublisherID=products.PublisherID,
                WriterID=products.WriterID,
                CreatedAt=DateTime.Now

                

            };

            Project.Products.Add(data);
            Project.SaveChanges();
            return RedirectToAction("Index", "Home");

        }

        private void Productfile(IFormFile file, string path)
        {
            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            catch (IOException ex)
            {
                // Handle the exception (e.g., log it or show a user-friendly error message)
                // You might also want to throw or return an error status
            }
        }

        public IActionResult ProductShow()
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");

                var showProduct = Project.Products
         .Include(p => p.ProductCategory)
         .Include(p => p.Publisher)
         .Include(p => p.Writer)
         .ToList();
                return View(showProduct);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult UpdateProduct(int id)
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");

                DropdownProductShow dropdownProductShow = new DropdownProductShow();
                dropdownProductShow.PublisherName = new List<SelectListItem>();
                dropdownProductShow.WriteName = new List<SelectListItem>();
                dropdownProductShow.ProductCategoryName = new List<SelectListItem>();
                var productCategory = Project.ProductCategories.ToList();
                var Writer = Project.Writers.ToList();
                var Publish = Project.Publishers.ToList();
                foreach (var Cate in productCategory)
                {
                    dropdownProductShow.ProductCategoryName.Add(new SelectListItem
                    {
                        Text = Cate.ProductCategoryName,
                        Value = Cate.ProductCategoryId.ToString()
                    });
                }
                foreach (var writ in Writer)
                {
                    dropdownProductShow.WriteName.Add(new SelectListItem
                    {
                        Text = writ.WriteName,
                        Value = writ.WritetId.ToString()
                    });
                }
                foreach (var Pub in Publish)
                {
                    dropdownProductShow.PublisherName.Add(new SelectListItem
                    {
                        Text = Pub.PublisherName,
                        Value = Pub.PublisherId.ToString()
                    });
                }
                ViewBag.ProductCategoryName = dropdownProductShow.ProductCategoryName;
                ViewBag.WriterName = dropdownProductShow.WriteName;
                ViewBag.PublisherName = dropdownProductShow.PublisherName;
                
                var edit = Project.Products.FirstOrDefault(i => i.ProductId == id);
                if(edit != null)
                {

                ViewBag.updateimg = edit.ProductIamge;
                }
                return View(edit);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }




        }
        [HttpPost]
        public IActionResult UpdateProduct(Product product,int id)
        {
          
            var path = Environment.WebRootPath;
            var filepath = "Image/" + product.ProductFile.FileName;
            var fullpath = Path.Combine(path, filepath);
            Productfile(product.ProductFile, fullpath);
            
           
            var edit = Project.Products.FirstOrDefault(i => i.ProductId == id);
            edit.ProductCode = product.ProductCode;
            edit.ProductName = product.ProductName;
            
            edit.ProductIamge = filepath;
            
              edit.ProductPrice = product.ProductPrice;
              edit.ProductQuantity = product.ProductQuantity;
               edit.Page = product.Page;
               edit.ProductDescription = product.ProductDescription;
               edit.ProductCategoryId = product.ProductCategoryId;
               edit.ProductDiscount = product.ProductDiscount;
               edit.PublisherID = product.PublisherID;
               edit.WriterID = product.WriterID;
               edit.CreatedAt = product.CreatedAt;



            Project.Products.Update(edit);
            Project.SaveChanges();
            return RedirectToAction("ProductShow", "Admin");
        }
        public IActionResult DeleteProduct(int id)
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");
                var DeleteProduct = Project.Products
    .Include(p => p.ProductCategory)
    .Include(p => p.Publisher)
    .Include(p => p.Writer)
    .FirstOrDefault(i => i.ProductId == id);
                return View(DeleteProduct);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            

        }
        [HttpPost]
        public IActionResult deleteProduct(int id)
        {
           
            var delete = Project.Products.Find(id);
            if (delete != null)
            {
                Project.Products.Remove(delete);
                Project.SaveChanges();
            }
            return RedirectToAction("ProductShow", "Admin");
        }
        public IActionResult DetailProduct(int id)
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");
                var DetailProduct = Project.Products
.Include(p => p.ProductCategory)
.Include(p => p.Publisher)
.Include(p => p.Writer)
.FirstOrDefault(i => i.ProductId == id);
                return View(DetailProduct);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
          
        }
        public IActionResult AdminOrder()
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");
                var bill = Project.Bills
                           .Include(o => o.User)
                   .ToList();

                return View(bill);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
          
        }

        public IActionResult AdminOrderCancel()
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");
                var bill = Project.Bills
                           .Include(o => o.User)
                   .ToList();

                return View(bill);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
          
        }
        public IActionResult AdminOrderDetail(int id)
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("Admin");
                ViewBag.id = HttpContext.Session.GetInt32("Adminid");

                ViewBag.bill = Project.Bills
               .Include(o => o.User)
                   .ThenInclude(c => c.Country)
               .Include(o => o.User)
                   .ThenInclude(c => c.City)
               .ToList();
                var bill = Project.Bills.Find(id);


                if (bill != null)
                {
                    ViewBag.OD = bill.BillCode;
                    // Other code related to displaying the bill details
                }
                else
                {
                    // Handle case where no bill with the specified id was found
                    ViewBag.OD = "Bill not found"; // Or any other appropriate message
                }

                var orders = Project.Orders
                                .Include(o => o.Product)
                                    .ThenInclude(p => p.ProductCategory)
                                .Include(o => o.Product)
                                    .ThenInclude(p => p.Publisher)
                                .Include(o => o.Product)
                                    .ThenInclude(p => p.Writer)
                                .Include(o => o.User)
                                .ToList();

                return View(orders);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
          
        }



        public IActionResult AdminOrderAccpet(int id)
        {
            var Accept = Project.Bills.Find(id);
            if (Accept != null)
            {
                Accept.Status = 2;
                
                Project.SaveChanges();
            }

            return RedirectToAction("AdminOrder","Admin");
        }



        public IActionResult AdminOrderDelivered(int id,DateTime date)
        {
            var Accept = Project.Bills.Find(id);
            //var currentTime = DateTime.Now;
            //var expirationTime = currentTime.AddHours(72);

            if (Accept != null)
            {
                Accept.Status = 3;
                Accept.Ordertime = date;
                Project.SaveChanges();
            }

            return RedirectToAction("AdminOrder", "Admin");
        }



        private int GenerateProductCode()
        {
            
            return new Random().Next(1000000, 9999999);
        }

      




    }
}
   