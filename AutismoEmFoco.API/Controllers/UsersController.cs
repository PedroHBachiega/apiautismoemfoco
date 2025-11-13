using AutismoEmFoco.API.DTOs;
using AutismoEmFoco.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AutismoEmFoco.API.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly UsuarioRepository _repo;

        public UsersController(UsuarioRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var usuarios = _repo.Listar().Select(UsuarioResponse.From);
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public IActionResult Obter(string id)
        {
            var u = _repo.ObterPorId(id);
            if (u == null) return NotFound();
            return Ok(UsuarioResponse.From(u));
        }

        public class CreateUsuarioRequest
        {
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
            public string UserType { get; set; } = "usuario";
        }

        [HttpPost]
        public IActionResult Criar([FromBody] CreateUsuarioRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Senha)) return BadRequest();
            if (_repo.ObterPorEmail(req.Email) != null) return Conflict(new { message = "Email já cadastrado" });
            var hash = Services.PasswordHasher.Hash(req.Senha);
            var u = _repo.Criar(req.Nome, req.Email, hash, req.UserType);
            return Created($"/users/{u.Id}", UsuarioResponse.From(u));
        }

        public class UpdateUsuarioRequest
        {
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(string id, [FromBody] UpdateUsuarioRequest req)
        {
            var ok = _repo.Atualizar(id, req.Nome, req.Email);
            if (!ok) return NotFound();
            var u = _repo.ObterPorId(id);
            return Ok(UsuarioResponse.From(u!));
        }

        public class UpdateUserTypeRequest
        {
            public string UserType { get; set; } = "usuario";
        }

        [HttpPut("{id}/type")]
        public IActionResult AtualizarTipo(string id, [FromBody] UpdateUserTypeRequest req)
        {
            var ok = _repo.AtualizarUserType(id, req.UserType);
            if (!ok) return NotFound();
            var u = _repo.ObterPorId(id);
            return Ok(UsuarioResponse.From(u!));
        }

        [HttpDelete("{id}")]
        public IActionResult Remover(string id)
        {
            var ok = _repo.Remover(id);
            if (!ok) return NotFound(new { message = "Usuário não encontrado" });
            return NoContent();
        }
    }
}