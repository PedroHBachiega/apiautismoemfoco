namespace AutismoEmFoco.API.DTOs
{
    public class RegisterRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string UserType { get; set; } = "usuario";
    }
}