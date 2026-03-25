using AppCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController(IPersonService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int size = 10)
    {
        var result = await service.FindAllPeoplePaged(page, size);
        return Ok(result);
    }
}