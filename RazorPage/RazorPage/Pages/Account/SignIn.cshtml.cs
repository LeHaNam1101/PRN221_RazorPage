using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;
using System.Security.Claims;
using System.Text.Json;

namespace RazorPage.Pages.Account
{
    public class SignInModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;
        public SignInModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        //Khai báo 2 thuộc tính đại diện cho 2 đối tượng kiểu: Account, Customer 

        [BindProperty]
        public Model.Account Account { get; set; }
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                var check_email = await dBContext.Accounts.SingleOrDefaultAsync(x => x.Email.Equals(Account.Email));
                if(check_email == null)
                {
                    ViewData["msg"] = "Email is not existed, pls try again.";
                    return Page();
                }
                else
                {
                    var get_password = await dBContext.Accounts.SingleOrDefaultAsync(x => x.Email.Equals(Account.Email));
                    string password = Decrypt(get_password.Password);
                    var account_guest = await dBContext.Accounts.SingleOrDefaultAsync(x => x.Email.Equals(Account.Email) && password.Equals(Account.Password) && x.Role == 2);
                    var account_admin = await dBContext.Accounts.SingleOrDefaultAsync(x => x.Email.Equals(Account.Email) && password.Equals(Account.Password) && x.Role == 1);
                    if (account_guest != null)
                    {
                        if (account_guest.IsActive)
                        {
                            HttpContext.Session.SetString("CusSession", JsonSerializer.Serialize(account_guest));
                            return RedirectToPage("/index");
                        }
                        else
                        {
                            ViewData["msg"] = "This account has not active yet !";
                            return Page();
                        }
                    }
                    else
                    {
                        if(account_admin != null)
                        {
                            if (account_admin.IsActive)
                            {
                                var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                var identity = new ClaimsIdentity(new[] {
                                                new Claim(ClaimTypes.Name, account_admin.Email)
                                            }, scheme);
                                var user = new ClaimsPrincipal(identity);
                                await HttpContext.SignInAsync(scheme, user);

                                HttpContext.Session.SetString("EmpSession", JsonSerializer.Serialize(account_admin));
                                return RedirectToPage("/admin/dashboard");
                            }
                            else
                            {
                                ViewData["msg"] = "This account has not active yet !";
                                return Page();
                            }
                        }
                        ViewData["msg"] = "Password is not correct. ";
                        return Page();
                    }
                }
            }
        }

        public string Encrypt(string text)
        {
            var encryptString = EncryptDecryptManager.Encrypt(text);
            return encryptString;
        }

        public string Decrypt(string text)
        {
            var decryptString = EncryptDecryptManager.Decrypt(text);
            return decryptString;
        }
    }
}
