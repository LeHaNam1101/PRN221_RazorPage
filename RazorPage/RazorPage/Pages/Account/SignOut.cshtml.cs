using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage.Pages.Account
{
    public class SignOutModel : PageModel
    {
        public async Task OnGetAsync()
        {
            var check_login_guest = HttpContext.Session.GetString("CusSession");
            var check_login_admin = HttpContext.Session.GetString("EmpSession");
            if (check_login_guest != null)
            {
                HttpContext.Session.Remove("CusSession");
                Response.Redirect("/index");
            }
            else
            {
                if (check_login_admin != null)
                {
                    var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    await HttpContext.SignOutAsync(scheme);

                    HttpContext.Session.Remove("EmpSession");
                    Response.Redirect("/index");
                }
                Response.Redirect("/index");
            }
        }
    }
}
