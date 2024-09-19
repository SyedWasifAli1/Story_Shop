using Book_kharido.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentMethodStripe.Models;
using Stripe.Checkout;

using Stripe;
using System.Drawing;
using ZXing;

namespace Book_kharido.Controllers
{
    public class Users : Controller
    {
        public Project Project { get; }
		private readonly StripeSettings _stripeSettings;

		public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; }

        public Users(Project project, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, IOptions<StripeSettings> stripeSetting)
        {
            Project = project;
            Environment = environment;
			_stripeSettings = stripeSetting.Value;

		}
		public IActionResult About()
        {
            return View();
        }   
        public IActionResult More()
        {
            return View();
        }   

        public IActionResult Home()
        {
            var showProduct = Project.Products
                 .Include(p => p.ProductCategory)
                 .Include(p => p.Publisher)
                 .Include(p => p.Writer)
                 .ToList().Take(3);
            return View(showProduct);
        }
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public IActionResult FAQs()
        {
         return View(Project.FAQs.ToList()); 
        }
        [HttpPost]
        public IActionResult Subcriber(string names,string email)
        {

            AddSubscriber(names,email);

            


            // Redirect to another action or URL
            return RedirectToAction("Home","Users");
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




        public IActionResult OrderTracking()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");
        
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

        public IActionResult Contact()
        {
            if (HttpContext.Session.GetString("User") != null)
            {

                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.id = HttpContext.Session.GetInt32("Userid");

                ViewData["FAQsId"] = new SelectList(Project.FAQs, "FAQsId", "Question");

            return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public IActionResult Contact(Contact contact)
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.id = HttpContext.Session.GetInt32("Userid");


                contact.Ordertime= DateTime.Now;
            Project.Contacts.Add(contact);
            Project.SaveChanges();
            return RedirectToAction("Shop","Users");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public IActionResult ContactView()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.id = HttpContext.Session.GetInt32("Userid");


                var project = Project.Contacts.Include(c => c.FAQ).Include(c => c.User);
            return View(project.ToList());

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public IActionResult Shop()
        {
			if (HttpContext.Session.GetString("User") != null)
			{
				ViewBag.email = HttpContext.Session.GetString("User");
				ViewBag.id = HttpContext.Session.GetInt32("Userid");

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
		public IActionResult Cart()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");


                var orders = Project.Orders
                                 .Include(o => o.Product)
                                     .ThenInclude(p => p.ProductCategory)
                                .Include(o => o.Product)
                                 .ThenInclude(P => P.Publisher)
                                 .Include(o => o.Product)
                                 .ThenInclude(P => P.Writer)
                                 .Include(o => o.User)
                                     .ToList();

                return View(orders);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Checkout()
        {
			if (HttpContext.Session.GetString("User") != null)
			{
				ViewBag.email = HttpContext.Session.GetString("User");
				ViewBag.userid = HttpContext.Session.GetInt32("Userid");

				DropdownProductShow dropdownProductShow = new DropdownProductShow();

				dropdownProductShow.CountryName = new List<SelectListItem>();

				var Publish = Project.Countries.ToList();
				foreach (var Cate in Publish)
				{
					dropdownProductShow.CountryName.Add(new SelectListItem
					{
						Text = Cate.ConutryName,
						Value = Cate.CountryId.ToString()
					});
				}

				ViewBag.CountryName = dropdownProductShow.CountryName;
				dropdownProductShow.CityName = new List<SelectListItem>();

				var City = Project.Cities.ToList();
				foreach (var Cate in City)
				{
					dropdownProductShow.CityName.Add(new SelectListItem
					{
						Text = Cate.CityName,
						Value = Cate.CityId.ToString()
					});
				}


				ViewBag.CityName = dropdownProductShow.CityName;

             
              

                ViewBag.TownName = Project.TownAddress.ToList();

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
				return View();
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		} 
        

		public IActionResult Product()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.id = HttpContext.Session.GetInt32("Userid");

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
        [HttpPost]
        public IActionResult SubmitFeedback(int productId, int userid, string message ,int rating)
        {
            //Feedback(productId, userid, message); // Call your Feedback method

            Feed feedback = new Feed
            {
                ProductId = productId,
                FeedbackMessages = message,
                Id = userid,
                ProductRating = rating
                
            };
            Project.Feedis.Add(feedback);
            Project.SaveChanges();

            // Redirect to the detail page passing the productId
            return RedirectToAction("Details", new { id = productId });


        }
        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.id = HttpContext.Session.GetInt32("Userid");
                ViewBag.Feedback = Project.Feedis.ToList();


                var DetailProduct = Project.Products
        .Include(p => p.ProductCategory)
        .Include(p => p.Publisher)
        .Include(p => p.Writer)
        .FirstOrDefault(i => i.ProductId == id);
                ViewBag.Productid = id;

                if (DetailProduct != null)
                {
                    var averageRating = Project.Feedis
             .Where(f => f.ProductId == id)
             .Any() ? Project.Feedis.Where(f => f.ProductId == id).Average(f => f.ProductRating) : 0;

                    ViewBag.AverageRating = averageRating;
                    var divide = DetailProduct.ProductPrice / 100;
                var dicount = DetailProduct.ProductDiscount * divide;
                ViewBag.result = DetailProduct.ProductPrice - dicount;

                    ViewBag.ProductQty = DetailProduct.ProductQuantity;
                }

                // Assuming DetailProduct.ProductCode is an integer
                //string encodedBillCode = Uri.EscapeDataString(DetailProduct.ProductCode.ToString());

                //string barcodeImage = $"https://barcodes4.me/barcode/c128a/{encodedBillCode}.png?s=6"; // Change barcode type and size as needed
                //ViewBag.BarcodeImage = barcodeImage;

                return View(DetailProduct);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }


        //private void Feedback(int productId, int userid, string message)
        //{
        //    Feed  feedback = new Feed
        //    {
        //        ProductId = productId,
        //        FeedbackMessages = message,
        //        Id = userid

        //    };
        //    Project.Feeds.Add(feedback);
        //    Project.SaveChanges();

        //}

        public IActionResult AddtocartDelete(int id)
        {


            var delete = Project.Orders.Find(id);
            if (delete != null)
            {
                Project.Orders.Remove(delete);
                Project.SaveChanges();
            }
            return RedirectToAction("Shop");
        }


        [HttpPost]
        public IActionResult comfirmorder(Dictionary<string, Order> orders)
        {
            foreach (var kvp in orders)
            {
                var orderId = kvp.Value.OrderId;
                var productId = kvp.Value.ProductId;
                var quantity = kvp.Value.Quantity;

                var order = Project.Orders.FirstOrDefault(o => o.OrderId == orderId);
                var product = Project.Products.FirstOrDefault(p => p.ProductId == productId);
                var divide = product.ProductPrice / 100;
                var dicount = product.ProductDiscount * divide;
                var price = product.ProductPrice - dicount;

                
                if (order != null)
                {
                    order.Quantity = quantity;
                order.Total = quantity*price;
                    Project.Orders.Update(order);
                }
            }

            Project.SaveChanges();

            return RedirectToAction("Checkout", "Users");
        }


        public IActionResult Read(int id)
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.id = HttpContext.Session.GetInt32("Userid");

                var DetailProduct = Project.Products
                    .Include(p => p.ProductCategory)
                    .Include(p => p.Publisher)
                    .Include(p => p.Writer)
                    .FirstOrDefault(i => i.ProductId == id);

                if (DetailProduct != null)
                {
                    ViewBag.Productid = id;
                    ViewBag.ProductName = DetailProduct.ProductName;
                    ViewBag.Category = DetailProduct.ProductCategory.ProductCategoryName;

                    var divide = DetailProduct.ProductPrice / 100;
                    var discount = DetailProduct.ProductDiscount * divide;
                    ViewBag.result = DetailProduct.ProductPrice - discount;
                    ViewBag.ProductQty = DetailProduct.ProductQuantity;

                    // Assuming DetailProduct.ProductCode is an integer
                    //string encodedBillCode = Uri.EscapeDataString(DetailProduct.ProductCode.ToString());
                    //string barcodeImage = $"https://barcodes4.me/barcode/c128a/{encodedBillCode}.png?s=6"; // Change barcode type and size as needed
                    //ViewBag.BarcodeImage = barcodeImage;

                    return View(DetailProduct);
                }
                else
                {
                    // Handle the case where the product was not found
                    return NotFound(); // or return a view that displays an error message
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }





        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, int userid)
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");

                var OrderCode = GenerateOrderCode();
               

                var product = Project.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product == null)
                {
                    return NotFound();
                }
                
                    var divide = product.ProductPrice / 100;
                var dicount = product.ProductDiscount * divide;
                var result = product.ProductPrice - dicount;
                //product.ProductQuantity = product.ProductQuantity - quantity;
                // Create a new order for the current user
                var order = new Order
                {
                    OrderCode = OrderCode, // Generate order code
                    Type = 0, // Assuming a cart type for now
                    Total = result * quantity, // Calculate total price
                    Quantity = quantity,
                    Id = userid, // Retrieve user from session (example implementation)
                    ProductId = productId,
                    Ordertime = DateTime.Now // Set current time
                };


                var existingOrder = Project.Orders.FirstOrDefault(o => o.Id == userid && o.ProductId == productId && o.Type == 0);
                if (existingOrder != null)
                {
                    // Order with the same user id and product id already exists
                    //TempData["Message"] = "Order already exists for this user and product.";
                    //return RedirectToAction("Show");
                }
                else
                {

                Project.Orders.Add(order);
                }
                    
                // Add the order to the database
                //Project.Products.Update(product);
                Project.SaveChanges();
                
              

                return RedirectToAction("Cart");
                // Redirect to cart page
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private string GenerateOrderCode()
        {
            const string chars = "0123456789";
            Random random = new Random();
            string randomCode = new string(Enumerable.Repeat(chars, 6) // Change 8 to the desired length of the code
              .Select(s => s[random.Next(s.Length)]).ToArray());
            string prefixedCode = "OR" + randomCode;
            return prefixedCode;
        }


        public IActionResult OrderRecieved(int id)
        {
            var Accept = Project.Bills.Find(id);
            if (Accept != null)
            {
                Accept.Status = 4;

                Project.SaveChanges();
            }

            return RedirectToAction("Ordered", "Users");
        }

        public IActionResult Show()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");


                var orders = Project.Orders
                                 .Include(o => o.Product)
                                     .ThenInclude(p => p.ProductCategory)
                                .Include(o => o.Product)
                                 .ThenInclude(P => P.Publisher)
                                 .Include(o => o.Product)
                                 .ThenInclude(P => P.Writer)
                                 .Include(o => o.User)
                                     .ToList();

            return View(orders);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }

        public IActionResult OrderDetail()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");

                DropdownProductShow dropdownProductShow = new DropdownProductShow();

                dropdownProductShow.CountryName = new List<SelectListItem>();

                var Publish = Project.Countries.ToList();
                foreach (var Cate in Publish)
                {
                    dropdownProductShow.CountryName.Add(new SelectListItem
                    {
                        Text = Cate.ConutryName,
                        Value = Cate.CountryId.ToString()
                    });
                }

                ViewBag.CountryName = dropdownProductShow.CountryName;
                
                dropdownProductShow.CityName = new List<SelectListItem>();

                var City = Project.Cities.ToList();
                foreach (var Cate in City)
                {
                    dropdownProductShow.CityName.Add(new SelectListItem
                    {
                        Text = Cate.CityName,
                        Value = Cate.CityId.ToString()
                    });
                }

                ViewBag.CityName = dropdownProductShow.CityName;

				dropdownProductShow.TownName = new List<SelectListItem>();

				var towns = Project.TownAddress?.ToList(); 
				if (towns != null)
				{
					foreach (var town in towns)
					{
						dropdownProductShow.TownName.Add(new SelectListItem
						{
							Text = town.TownName,
							Value = town.TownsAId.ToString()
						});
					}
				}
                else
                {

                }

				ViewBag.TownName = dropdownProductShow.TownName;


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
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }





        public IActionResult OrderedDetail(int id)
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");
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
           
                //string encodedBillCode = Uri.EscapeDataString(bill.BillCode);
                //string qrCodeImage = $"https://api.qrserver.com/v1/create-qr-code/?data={encodedBillCode}&size=200x200";
                //ViewBag.QRCodeImage = qrCodeImage;

                //string barcodeImage = $"https://barcodes4.me/barcode/c128a/{encodedBillCode}.png?s=6"; // Change barcode type and size as needed
                //ViewBag.BarcodeImage = barcodeImage;
                // Fetching orders based on the provided orderedcode
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
        

        public IActionResult OrderDelete(int id)
        {


            var delete = Project.Orders.Find(id);
            var product = Project.Products.Find(delete.ProductId);
         
            if (delete != null)
            {
                delete.Type = 2;
                product.ProductQuantity = product.ProductQuantity + delete.Quantity;
                Project.Orders.Update(delete);
                Project.Products.Update(product);
                Project.SaveChanges();
            }
            return RedirectToAction("Ordered");
        }
        public IActionResult OrderedDelete(int id)
        {


            var delete = Project.Bills.Find(id);
            if (delete != null)
            {
                delete.Status = 5;
                Project.Bills.Update(delete);

                Project.SaveChanges();
            }
            //var product = Project.Products.Find(delete.ProductId);

            var ordersToUpdate = Project.Orders.Where(o => o.OrderCode == delete.BillCode && o.Type == 1 ).ToList();

            foreach (var order in ordersToUpdate)
            {
                var product = Project.Products.FirstOrDefault(p => p.ProductId == order.ProductId);
                if (product != null)
                {
                    product.ProductQuantity = product.ProductQuantity + order.Quantity; // Reduce product quantity
                }
            }
            foreach (var order in ordersToUpdate)
            {
                order.Type = 2;
            }
            Project.Orders.UpdateRange(ordersToUpdate);
            Project.SaveChanges();
            return RedirectToAction("Ordered");
        }


        public IActionResult Ordered()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");
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
        public IActionResult CancelOrder()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");
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
        public IActionResult RecievedOrder()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");
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


        public IActionResult UserUpdate(User user)
{

            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");


         var users = Project.Users.FirstOrDefault(p => p.Id == user.Id);
                // Retrieve all orders matching the condition
                
                users.Address = user.Address;
            users.CountryId = user.CountryId;
            users.CityId = user.CityId;
            users.PhoneNumber = user.PhoneNumber;
                users.TownsAId = user.TownsAId;
           
                Project.Users.Update(users);

                Project.SaveChanges();
				StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

				// Define options for creating a checkout session
				var options = new SessionCreateOptions
				{
					PaymentMethodTypes = new List<string>
				{
					"card" // Add payment method types as needed
                },
					Mode = "payment",
					SuccessUrl = "https://localhost:7042/Users/Checkoutconfirm",
					CancelUrl = "https://localhost:7042/Users/Checkout",
					LineItems = new List<SessionLineItemOptions>
				{
					new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							Currency = "usd",
							UnitAmount = Convert.ToInt32(user.Amount), // Amount in cents
                            ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = "T-shirt", // Name of your product
                            },
						},
						Quantity = 1, // Quantity of the product
                    },
				},
				};

				// Create the checkout session
				var service = new SessionService();
				var session = service.Create(options);

				return Redirect(session.Url);


				//return RedirectToAction("Product");


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public IActionResult UserUpdatecash(User user)
{

            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");


         var users = Project.Users.FirstOrDefault(p => p.Id == user.Id);
                // Retrieve all orders matching the condition
                
                users.Address = user.Address;
            users.CountryId = user.CountryId;
            users.CityId = user.CityId;
            users.PhoneNumber = user.PhoneNumber;
                users.TownsAId = user.TownsAId;
                Project.Users.Update(users);

                Project.SaveChanges();

                return RedirectToAction("CheckoutConfirm");


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
		public IActionResult CheckoutConfirm()
		{
			// Retrieve user information from session
			var userId = HttpContext.Session.GetInt32("Userid");
			if (userId != null)
			{
				// Retrieve user details from the database
				var user = Project.Users
					.Include(p => p.TownsAdress)
					.Include(p => p.Country)
					.Include(p => p.City)
					.FirstOrDefault(u => u.Id == userId);

				if (user != null)
				{
					ViewBag.email = HttpContext.Session.GetString("User");
					ViewBag.userid = userId;
					ViewBag.user = user;

					// Retrieve orders associated with the user
					var orders = Project.Orders
						.Include(o => o.Product)
							.ThenInclude(p => p.ProductCategory)
						.Include(o => o.Product)
							.ThenInclude(p => p.Publisher)
						.Include(o => o.Product)
							.ThenInclude(p => p.Writer)
						.Include(o => o.User)
						.Where(o => o.Id == userId && o.Type == 0)
						.ToList();

					ViewBag.orders = orders;

					return View();
				}
			}

			// If user session or details not found, redirect to the home page
			return RedirectToAction("Index", "Home");
		}

		public IActionResult PlaceOrder(int id)
{

            if (HttpContext.Session.GetString("User") != null)
            {
                ViewBag.email = HttpContext.Session.GetString("User");
                ViewBag.userid = HttpContext.Session.GetInt32("Userid");

               
                var ordersToUpdate = Project.Orders.Where(o => o.Id == id && o.Type == 0).ToList();
              
                foreach (var order in ordersToUpdate)
                {
                    var product = Project.Products.FirstOrDefault(p => p.ProductId == order.ProductId);
                    if (product != null)
                    {
                        product.ProductQuantity -= order.Quantity; // Reduce product quantity
                    }
                }
                var OrderCode = GenerateOrderCode();
                var currentTime = DateTime.Now;
                var expirationTime = currentTime.AddHours(24);
                var Bill = new Bill()
                {
                    BillCode = OrderCode,
                    Status = 1,
                    Id = id,
                    Ordertime = expirationTime

            };
                foreach (var order in ordersToUpdate)
                {
                    order.Type = 1;
                    order.OrderCode = OrderCode;
                    order.Ordertime = expirationTime;

                }
                Project.Bills.Add(Bill);
                Project.Orders.UpdateRange(ordersToUpdate);


                Project.SaveChanges();

                return RedirectToAction("Shop");


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }



        //public IActionResult Index()
        //{


        //    return View();

        //    //if (HttpContext.Session.GetString("User") != null)
        //    //{
        //    //    ViewBag.email = HttpContext.Session.GetString("User");
        //    //    ViewBag.id = HttpContext.Session.GetInt32("Userid");

        //    //    return View();
        //    //}
        //    //else
        //    //{
        //    //    return RedirectToAction("Index", "Home");
        //    //}
        //}

    }
}
