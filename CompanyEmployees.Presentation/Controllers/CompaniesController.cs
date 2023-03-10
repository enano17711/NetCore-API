using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompaniesController : ControllerBase
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}