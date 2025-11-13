using AutismoEmFoco.API.Models;

namespace AutismoEmFoco.API.DTOs
{
    public class UsuarioResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; }

        public static UsuarioResponse From(Usuario u)
        {
            return new UsuarioResponse
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                UserType = u.UserType,
                CriadoEm = u.CriadoEm
            };
        }
    }
}