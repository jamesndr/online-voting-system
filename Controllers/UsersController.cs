using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using Onlinevotingsystem.Models;

namespace Onlinevotingsystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly OnlineVotingDB _context;
        const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public UsersController(OnlineVotingDB context)
        {
            _context = context;
        }

     //   List<Users> regusers = _context.Users.ToListAsync();
       // ViewBag.regusers=regusers;
        // GET: Users
        public async Task<IActionResult> Index()
        {
            var x = HttpContext.Session.GetString("email");
            var y = HttpContext.Session.GetString("password");
            if (x == null || y == null)
            {
                TempData["nop"] = "Please Login To Perform this Functionality!";
                return RedirectToAction(nameof(Login));
            }
            if (x != "admin" && y != "admin")
            {
                TempData["nop"] = "You dont Have Permission to access this Functionality!";
                return RedirectToAction(nameof(Login));
            }
            var uslist = _context.Users.ToList();
              return View(await _context.Users.ToListAsync());
        }

        public IActionResult ForgotPassword()
        {
            //var user = await _context.Users
            //    .FirstOrDefaultAsync(m => m.email.Equals(email) && m.PhoneNo.Equals(phoneNo));
            return View();
        }

        public ActionResult UpPassword(string id,string id2,string id3,string? email,string? phoneNo,string? newPassword)
        {
            Reply r = new Reply();
           
            var usrList = _context.Users.ToList();
            if (usrList.Count() > 0)
            {
                foreach (var s in usrList)
                {
                    if (s.email.Equals(email) && s.PhoneNo.Equals(phoneNo))
                    {
                        s.password =newPassword;
                        _context.Update(s);
                        _context.SaveChangesAsync();
                        
                            r.status = true;
                            r.color = "alert alert-success";
                            r.message = "Your Password have been updated Sucessfully!";
                            return View(r);
                }
            }
            
            }
            r.status = false;
            r.color = "alert alert-danger";
            r.message = "Your Password update Failed!";

            return View(r);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var x = HttpContext.Session.GetString("email");
            var y = HttpContext.Session.GetString("password");
            if (x == null || y == null)
            {
                TempData["nop"] = "Please Login To Perform this Functionality!";
                return RedirectToAction(nameof(Login));
            }
            if (x != "admin" && y != "admin")
            {
                TempData["nop"] = "You dont Have Permission to access this Functionality!";
                return RedirectToAction(nameof(Login));
            }
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Createu(Users users)
        {
            Reply r = new Reply();
            if (users != null)
            {
                StringBuilder sb = new StringBuilder();
                Random rnd = new Random();

                for (int i = 0; i < 10; i++)
                {
                    int index = rnd.Next(chars.Length);
                    sb.Append(chars[index]);
                }
                users.CanVote = "false";
                var password = sb.ToString();
                users.password = password;
             
                _context.Add(users);
                 _context.SaveChangesAsync();
                var usrpassword = "This is your password : "+ users.password;
                r.status = true;
                r.color = "alert alert-success";
                r.message = " Registration Sucessful! ";
                r.passcode = usrpassword;
                 return Json(new { status = true, message = "Registration Sucessful! ", pass = usrpassword });
                //return View(r);
            }
           // return View(r);
            return Json(new { status = false, message = "Registration Failed!"});
        }

        public ActionResult Login()
        {
            HttpContext.Session.Remove("password");
            HttpContext.Session.Remove("email");
            return View();
        }
        
        public ActionResult  UsersLogin(UserLogin login)
        {
            if (login == null)
            {
                TempData["logf"] = "..!! Please Enter credentials !!..";
                return Json(new { status = false, message = "email and Password Not Match!" });
            }
            var  loggedin = _context.Users.Where(s => s.email.Equals(login.email) && s.password == login.password).Count();
            var users = _context.Users.Where(s => s.email.Equals(login.email) && s.password == login.password);
            ViewBag.regusers = users;
            
           // if (users.Any())
                if (loggedin == 1)
                 {
                   // var loggedin = _context.Users.Where(s => s.email.Equals(login.email) && s.password == login.password).SingleOrDefault();
                    //Homepage();
                    HttpContext.Session.SetString("email", login.email);
                    HttpContext.Session.SetString("password", login.password);
                    TempData["logs"] = "..!!Login Successful!!..";
                return Json(new { status = true, message = "Login Sucessful!" });


            }
                else
                {
                    TempData["logf"] = "..!! Entered Credentials dont match!!..";
                    return Json(new { status = false, message = "email and Password Not Match!" });
              
            }
            // }
            // else
            // {
            //    TempData["logf"] = "..!!User Dont Exists!!..";
            //    return Json(new { status = false, message = "User NotFound!" });
               
            //}
        }
        public ActionResult Homepage()
        {
            var x = HttpContext.Session.GetString("email");
            var y = HttpContext.Session.GetString("password");
            if (x == null || y == null)
            {
                TempData["nop"] = "Please Login To Access Home Page!";
                return RedirectToAction(nameof(Login));
            }
            return View();
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            TempData["nop"] = null;
            TempData["Vps"] = null;
            TempData["Vpf"] = null;
            var x = HttpContext.Session.GetString("email");
            var y = HttpContext.Session.GetString("password");
            if (x == null || y == null)
            {
                TempData["nop"] = "Please Login To Perform this Functionality!";
                return RedirectToAction(nameof(Login));
            }
            if (x != "admin" && y != "admin")
            {
                TempData["nop"] = "You dont Have Permission to access this Functionality!";
                return RedirectToAction(nameof(Login));
            }
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Userid,FirstName,LastName,DateOfBirth,Gender,email,password,PhoneNo,address,CanVote")] Users users)
        {
            TempData["Vps"] = null;
            TempData["Vpf"] = null;
            var usr = await _context.Users.FindAsync(id);
            users.Userid = id;
            users.FirstName = usr.FirstName;
            users.LastName = usr.LastName;
            users.DateOfBirth = usr.DateOfBirth;
            users.email = usr.email;
            users.password = usr.password;
            users.PhoneNo = usr.PhoneNo;
            users.Gender = usr.Gender;
            users.address = usr.address;

            
            if (id != users.Userid)
            {
                return NotFound();
            }

            if (users!=null)
            {
                try
                {

                    _context.Update(users);
                    await _context.SaveChangesAsync();
                    if (users.CanVote == "false")
                    {
                        TempData["Vpf"] = "Permission to Vote is Deprecated to " + users.FirstName + " " + users.LastName + " user!";
                    }
                    else
                    {
                        TempData["Vps"] = "Permission to Vote is Granted to " + users.FirstName + " " + users.LastName + " user!";
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.Userid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var x = HttpContext.Session.GetString("email");
            var y = HttpContext.Session.GetString("password");
            if (x == null || y == null) {
                TempData["nop"] = "Please Login To Perform this Functionality!";
                return RedirectToAction(nameof(Login));
            }

            if (x != "admin" && y != "admin")
            {
                TempData["nop"] = "You dont Have Permission to access this Delete!";
                return RedirectToAction(nameof(Login));
            }
            
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.Userid == id);
            var elecs = _context.ElectionVotes.ToList();
            foreach(var e in elecs)
            {
                if(e.UserId == id)
                {
                    TempData["alv"] = users.FirstName + " " + users.LastName + " have Already Voted for election now u cannot delete him !..";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'OnlineVotingDB.Users'  is null.");
            }
            var users = await _context.Users.FindAsync(id);

            if (users != null)
            {
                TempData["alvs"] = users.FirstName + " " + users.LastName + " have been deleted successfully !..";
                _context.Users.Remove(users);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
          return _context.Users.Any(e => e.Userid == id);
        }
        
    }
}
