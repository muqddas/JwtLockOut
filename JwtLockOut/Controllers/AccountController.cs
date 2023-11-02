using JwtLockOut.Data;
using JwtLockOut.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace JwtLockOut.Controllers
{
    public class AccountController : Controller
        {
            public readonly ApplicationDbContext _db;  //using Depencency Injection for database access (thru ApplicationDbCondext)

            SignInManager<ApplicationUser> _signInManager;
            UserManager<ApplicationUser> _userManager;
            RoleManager<IdentityRole> _roleManager;


            public AccountController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,

                RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager
                )
            {
                _db = db;
                _signInManager = signInManager;
                _userManager = userManager;

                _roleManager = roleManager;




            }
            public IActionResult Login()
            {

                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Login(LoginViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Appoint");

                    }
                    ModelState.AddModelError("", "Invalid Login Attempt");

                }
                return View(model);
            }


            public async Task<IActionResult> Register()
            {
                if (!_roleManager.RoleExistsAsync(Helper.Admin).GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole(Helper.Admin));
                    await _roleManager.CreateAsync(new IdentityRole(Helper.User));
                    


                }
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Register(RegisterViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = model.Name };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, model.RoleName);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> LogOff()
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("LogIn", "Account");

            }

        }
    }
