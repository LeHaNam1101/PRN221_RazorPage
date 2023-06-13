using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace RazorPage.Pages
{

    public class ErrorModel : PageModel
    {
        public int Code { get; set; }
        
        public void OnGet(int? code)
        {
            Code = code ?? 0;
        }
    }
}
