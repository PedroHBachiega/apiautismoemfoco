using AutismoEmFoco.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AutismoEmFoco.API.Controllers
{
    [ApiController]
    [Route("profile")]
    public class ProfileController : ControllerBase
    {
        private readonly UsuarioRepository _repo;

        public ProfileController(UsuarioRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{id}")]
        public IActionResult Obter(string id)
        {
            var p = _repo.ObterPerfil(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(string id, [FromBody] Dictionary<string, object> dados)
        {
            var ok = _repo.AtualizarPerfil(id, dados);
            if (!ok) return NotFound();
            var p = _repo.ObterPerfil(id);
            return Ok(p);
        }
    }
}