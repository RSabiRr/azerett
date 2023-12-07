using AzerEt.Data;
using AzerEt.Models;
using AzerEt.Services;
using AzerEt.ViewModels;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AzerEt.Controllers
{
    public class HomeController : Controller
    {

         readonly AppDbContext _context;
        readonly PaymentService _paymentService;


        public HomeController(AppDbContext context, PaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }



        public IActionResult Index()
        {
            VmHome model = new VmHome()
            {
                Setting = _context.Settings.FirstOrDefault(),
                Menus = _context.Menus.ToList(),
                Abouts=_context.Abouts.FirstOrDefault(),
                
                Wishlists = _context.Wishlists.ToList(),
                Categories = _context.Categories.ToList(),
                Specials = _context.Specials.ToList(),
                Galleries= _context.Galleries.ToList(),
                Whyus=_context.Whyus.ToList(),
                Cheifs=_context.Cheifs.ToList()
                //Contacts = _context.Contacts.FirstOrDefault(),

            };
            return View(model);
        }

        

        [ActionName("CountAdd")]
        [HttpPost]
        public IActionResult CountAdd([FromBody] Wishlist item)
        {
            var id = item.UserId;
            var list = _context.Wishlists.Where(m => m.UserId == id).ToList();
            var WCount = item.Count;
            if (item == null)
            {
                return BadRequest("Geçersiz veri.");
            }
            var menyu = list.Where(z => z.MenuId == item.MenuId).FirstOrDefault();
            var contact = _context.Wishlists.Find(menyu.Id);
            contact.Count = WCount+1;
            _context.Update(contact);
            _context.SaveChanges();
            var sil = new { success = true, message = "İşlem başarıyla silindi." };
            return Json(sil);
        }


        [ActionName("CountDell")]
        [HttpPost]
        public IActionResult CountDell([FromBody] Wishlist item)
        {
            var id = item.UserId;
            var list = _context.Wishlists.Where(m => m.UserId == id).ToList();
            var WCount = item.Count;
            if (item == null)
            {
                return BadRequest("Geçersiz veri.");
            }
            var menyu = list.Where(z => z.MenuId == item.MenuId).FirstOrDefault();
            var contact = _context.Wishlists.Find(menyu.Id);
            if (WCount==1)
            {
                contact.Count = WCount;

            }
            else
            {
                contact.Count = WCount - 1;

            }
            _context.Update(contact);
            _context.SaveChanges();
            var sil = new { success = true, message = "İşlem başarıyla silindi." };
            return Json(sil);
        }

        public IActionResult Cart(string id)
        {
            var wishlists = _context.Wishlists.Include(z=>z.Menu).Include(r=>r.CustomUser).Where(q=>q.UserId==id).ToList();
            return View(wishlists);
        }


        [Route("message")]
        [HttpPost]
        public IActionResult Message(VmHome model)
        {
             
                model.Contacts.CreatedDate = DateTime.Now;
                _context.Contacts.Add(model.Contacts);
       
                TempData["Message"] = "Mesajınız uğurla göndərildi!";
            var modell = _context.Contacts.FirstOrDefault();
            _context.SaveChanges();
            HttpContext.Session.SetString("Uğur", "Mesajınız uğurla göndərildi!");
                return RedirectToAction("Index", "Home");
            
            HttpContext.Session.SetString("Error", "Model is not valid");
            return RedirectToAction("Index", "Home");


        }

        [Route("Checkout")]
        public IActionResult Checkout(string id)
        {
            var user = _context.CustomUsers.Find(id);
            Checkout checkout = new Checkout();
            var wishsum = _context.Wishlists.Include(z => z.Menu).Include(r => r.CustomUser).Where(q => q.UserId == user.Id).ToList();
            //var wishcount = _context.Wishlists.Include(z => z.Menu).Include(r => r.CustomUser).Where(q=>q.Id==1160);
            double total = 0;
            foreach (var item in wishsum)
            {
                var count = item.Count;
                var price = item.Menu.Price;
                var sum = count * Convert.ToDouble(price);
                total += sum;
            }
            if (total <= 50)
            {
                total+=3 ;
            }
            VmHome model = new VmHome()
            {
                CustomUser = user,
                Total=total,
                Wishlists = _context.Wishlists.Include(z => z.Menu).Include(r => r.CustomUser).Where(q => q.UserId == user.Id).ToList()
            };

           
            var wishlists = _context.Wishlists.Include(z => z.Menu).Include(r => r.CustomUser).Where(q => q.UserId == id).ToList();
            return View(model);
        }

        [Route("Checkout")]
        [HttpPost]
        public IActionResult Checkout(VmHome besdide)
        {
            //var user = _context.CustomUsers.Find(id);

            var wishsum = _context.Wishlists.Include(z => z.Menu).Include(r => r.CustomUser).Where(q => q.UserId == besdide.Checkout.UserId).ToList();
            //var wishcount = _context.Wishlists.Include(z => z.Menu).Include(r => r.CustomUser).Where(q=>q.Id==1160);
            double total = 0;
            foreach (var item in wishsum)
            {
                var count = item.Count;
                var price = item.Menu.Price;
                var sum = count * Convert.ToDouble(price);
                total += sum;
            }
            if (total<=50)
            {
                total += 3;
            }
            var str = "";
            if (besdide.Checkout.Information==null)
            {
                str = " ";
            }
            else
            {
                str = besdide.Checkout.Information;
            }


            Checkout checkout = new Checkout()
            {
                
                UserId= besdide.Checkout.UserId,
                Adress=besdide.Checkout.Adress,
                Iscart=besdide.Checkout.Iscart,
                Information = str,
                ContactPhone = besdide.Checkout.ContactPhone,
                TotalPrice=total,
                CreatedDate = DateTime.Now

            };

            _context.Add(checkout);
             _context.SaveChanges();
            var wishlists = _context.Wishlists.Include(z => z.Menu).Include(r => r.CustomUser).Where(q => q.UserId == besdide.Checkout.UserId).ToList();

            foreach (var item in wishlists)
            {
                CheckWishlist colorToProduct = new CheckWishlist()
                {
                    MenuId = item.MenuId,
                    CheckoutId = checkout.Id,
                    Count=item.Count
                };
                _context.CheckWishlists.Add(colorToProduct);
                _context.SaveChanges();
            }


            foreach (var item in wishlists)
            {
                var wish = _context.Wishlists.Find(item.Id);
                _context.Wishlists.Remove(wish);
                _context.SaveChanges();
            }

            if (checkout.Iscart==true)
            {
                return RedirectToAction("Pay", new { totall = total ,idd= checkout.Id });
            }
            else
            {
                return  RedirectToAction("CheckTime", "Home");
            }
            //return View(checkout);
            //var wishlists = _context.Wishlists.Include(z => z.Menu).Include(r => r.CustomUser).Where(q => q.UserId == id).ToList();

        }

        [ActionName("PostDelete")]
        [HttpPost]
        public IActionResult PostDelete([FromBody] Wishlist item)
        {
            var id = item.UserId;
            var list = _context.Wishlists.Where(m => m.UserId == id).ToList();
            if (item == null)
            {
                return BadRequest("Geçersiz veri.");
            }
            var menyu = list.Where(z => z.MenuId == item.MenuId).FirstOrDefault();
            var contact = _context.Wishlists.Find(menyu.Id);
            _context.Wishlists.Remove(contact);
            _context.SaveChanges();
            var sil = new { success = true, message = "İşlem başarıyla silindi." };
            return Json(sil);
        }

        [Route("CheckTime")]
        public IActionResult CheckTime()
        {
           
            return View();
        }

        

        [Route("UserDetails")]
        public IActionResult UserDetails(string userId)
        {
 
            VmHome vmHome = new VmHome()
            {
                Checkouts = _context.Checkouts.OrderByDescending(m => m.Id).Include(q => q.CustomUser).Where(m => m.UserId == userId).ToList(),
                CheckWishlists = _context.CheckWishlists.Include(z => z.Menu).ThenInclude(x => x.Category).ToList()
            };

            return View(vmHome);
        }


        [Route("Pay")]
        public async Task<IActionResult> Pay(double totall,int idd)
        {

            var price = totall * 100;
            var result =await _paymentService.PayMent(price, idd);

           
            return Redirect(result);
        }

        [Route("CardCheckTime/{id}/{userId}")]
        public IActionResult CardCheckTime(int id,string userId)
        {
            var check = _context.Checkouts.Where(q => q.Id == id).FirstOrDefault();
            check.Success = true;
            _context.SaveChanges();
            var userrr = _context.CustomUsers.FirstOrDefault(q=>q.Id==userId);
            return View(userrr);
        }

        [Route("cancelurl/{id}")]
		public IActionResult cancelurl(int id)
		{
            var check = _context.Checkouts.Where(q => q.Id == id).FirstOrDefault();
            _context.Checkouts.Remove(check);
            _context.SaveChanges();
            return View();
		}

	}
}