using AutismoEmFoco.API.DTOs;
using AutismoEmFoco.API.Repositories;
using AutismoEmFoco.API.Services;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace AutismoEmFoco.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioRepository _repo;

        public AuthController(UsuarioRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Senha)) return BadRequest();
            if (_repo.ObterPorEmail(req.Email) != null) return Conflict(new { message = "Email já cadastrado" });
            var hash = PasswordHasher.Hash(req.Senha);
            var u = _repo.Criar(req.Nome, req.Email, hash, req.UserType);
            var token = Guid.NewGuid().ToString();
            var firebaseToken = FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(u.Id).GetAwaiter().GetResult();
            return Ok(new { user = UsuarioResponse.From(u), token, firebaseToken });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var u = _repo.ObterPorEmail(req.Email);
            if (u == null) return Unauthorized(new { message = "Email ou senha inválidos" });
            var ok = PasswordHasher.Verify(req.Senha, u.SenhaHash);
            if (!ok) return Unauthorized(new { message = "Email ou senha inválidos" });
            var token = Guid.NewGuid().ToString();
            var firebaseToken = FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(u.Id).GetAwaiter().GetResult();
            return Ok(new { user = UsuarioResponse.From(u), token, firebaseToken });
        }
    }
}