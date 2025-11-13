using Google.Cloud.Firestore;

namespace AutismoEmFoco.API.Models
{
    [FirestoreData]
    public class Usuario
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Nome { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Email { get; set; } = string.Empty;

        [FirestoreProperty]
        public string SenhaHash { get; set; } = string.Empty;

        [FirestoreProperty]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public string UserType { get; set; } = "usuario";
    }
}