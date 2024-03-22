using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smashing.Core.Features.Users;

namespace Smashing.Api.Controllers;

[ApiController]
[Route("v1/account")]
public class HomeController : Controller
{
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model)
    {
        var user = UserRepository.Get(model.Username, model.Password);

        if (user == null)
            return NotFound(new { message = "Usuário ou senha inválidos" });

        var token = TokenService.GenerateToken(user);
        user.Password = "";
        return new
        {
            user, token
        };
    }

    [HttpGet]
    [Route("anonymous")]
    [AllowAnonymous]
    public string Anonymous()
    {
        return "Anônimo";
    }

    [HttpGet]
    [Route("authenticated")]
    [Authorize]
    public string Authenticated()
    {
        return string.Format("Autenticado - {0}", User.Identity.Name);
    }

    [HttpGet]
    [Route("employee")]
    [Authorize(Roles = "employee,manager")]
    public string Employee()
    {
        return "Funcionário";
    }

    [HttpGet]
    [Route("manager")]
    [Authorize(Roles = "manager")]
    public string Manager()
    {
        return "Gerente";
    }
}