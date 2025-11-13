using AutismoEmFoco.API.Models;
using Google.Cloud.Firestore;
using System.Text.Json;

namespace AutismoEmFoco.API.Repositories
{
    public class UsuarioRepository
    {
        private readonly FirestoreDb _db;
        private const string CollectionName = "users";

        public UsuarioRepository(FirestoreDb db)
        {
            _db = db;
        }

        public IEnumerable<Usuario> Listar()
        {
            var snap = _db.Collection(CollectionName).GetSnapshotAsync().GetAwaiter().GetResult();
            return snap.Documents.Select(d => d.ConvertTo<Usuario>()).OrderBy(u => u.Nome);
        }

        public Usuario? ObterPorId(string id)
        {
            var doc = _db.Collection(CollectionName).Document(id);
            var snap = doc.GetSnapshotAsync().GetAwaiter().GetResult();
            if (!snap.Exists) return null;
            return snap.ConvertTo<Usuario>();
        }

        public Usuario? ObterPorEmail(string email)
        {
            var query1 = _db.Collection(CollectionName).WhereEqualTo("Email", email).Limit(1);
            var snap1 = query1.GetSnapshotAsync().GetAwaiter().GetResult();
            var doc1 = snap1.Documents.FirstOrDefault();
            if (doc1 != null) return doc1.ConvertTo<Usuario>();
            var query2 = _db.Collection(CollectionName).WhereEqualTo("email", email).Limit(1);
            var snap2 = query2.GetSnapshotAsync().GetAwaiter().GetResult();
            var doc2 = snap2.Documents.FirstOrDefault();
            return doc2 == null ? null : doc2.ConvertTo<Usuario>();
        }

        public Usuario Criar(string nome, string email, string senhaHash, string userType = "usuario")
        {
            var id = Guid.NewGuid().ToString();
            var u = new Usuario
            {
                Id = id,
                Nome = nome,
                Email = email,
                SenhaHash = senhaHash,
                CriadoEm = DateTime.UtcNow,
                UserType = userType
            };
            var doc = _db.Collection(CollectionName).Document(id);
            var profile = new Dictionary<string, object>
            {
                { "uid", id },
                { "Email", email },
                { "email", email },
                { "displayName", string.IsNullOrWhiteSpace(nome) ? "" : nome },
                { "bio", "" },
                { "telefone", "" },
                { "cidade", "" },
                { "estado", "" },
                { "especialidade", "" },
                { "registroProfissional", "" },
                { "experienciaAutismo", "" },
                { "atendimentoOnline", false },
                { "atendimentoPresencial", false },
                { "latitude", "" },
                { "longitude", "" },
                { "endereco", "" },
                { "UserType", userType },
                { "userType", userType },
                { "Nome", nome },
                { "SenhaHash", senhaHash },
                { "CriadoEm", DateTime.UtcNow }
            };
            doc.SetAsync(profile, SetOptions.MergeAll).GetAwaiter().GetResult();
            return u;
        }

        public bool Atualizar(string id, string nome, string email)
        {
            var doc = _db.Collection(CollectionName).Document(id);
            var snap = doc.GetSnapshotAsync().GetAwaiter().GetResult();
            if (!snap.Exists) return false;
            doc.UpdateAsync(new Dictionary<string, object>
            {
                { "Nome", nome },
                { "Email", email }
            }).GetAwaiter().GetResult();
            return true;
        }

        public bool AtualizarUserType(string id, string userType)
        {
            var doc = _db.Collection(CollectionName).Document(id);
            var snap = doc.GetSnapshotAsync().GetAwaiter().GetResult();
            if (!snap.Exists) return false;
            doc.UpdateAsync(new Dictionary<string, object>
            {
                { "UserType", userType },
                { "userType", userType }
            }).GetAwaiter().GetResult();
            return true;
        }

        public bool Remover(string id)
        {
            var doc = _db.Collection(CollectionName).Document(id);
            var snap = doc.GetSnapshotAsync().GetAwaiter().GetResult();
            if (!snap.Exists) return false;
            doc.DeleteAsync().GetAwaiter().GetResult();
            return true;
        }

        public Dictionary<string, object>? ObterPerfil(string id)
        {
            var doc = _db.Collection("users").Document(id);
            var snap = doc.GetSnapshotAsync().GetAwaiter().GetResult();
            if (!snap.Exists) return null;
            return snap.ToDictionary();
        }

        public bool AtualizarPerfil(string id, Dictionary<string, object> dados)
        {
            var doc = _db.Collection("users").Document(id);
            var snap = doc.GetSnapshotAsync().GetAwaiter().GetResult();
            if (!snap.Exists) return false;
            var normalized = NormalizeForFirestore(dados);
            doc.SetAsync(normalized, SetOptions.MergeAll).GetAwaiter().GetResult();
            return true;
        }

        private static object ConvertJsonElement(JsonElement je)
        {
            switch (je.ValueKind)
            {
                case JsonValueKind.String:
                    return je.GetString()!;
                case JsonValueKind.Number:
                    if (je.TryGetInt64(out var i64)) return i64;
                    if (je.TryGetDouble(out var d)) return d;
                    return je.GetDecimal();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return je.GetBoolean();
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null!;
                case JsonValueKind.Object:
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in je.EnumerateObject())
                    {
                        dict[prop.Name] = ConvertUnknown(prop.Value);
                    }
                    return dict;
                case JsonValueKind.Array:
                    var list = new List<object?>();
                    foreach (var item in je.EnumerateArray())
                    {
                        list.Add(ConvertUnknown(item));
                    }
                    return list;
                default:
                    return je.ToString();
            }
        }

        private static object? ConvertUnknown(object? value)
        {
            if (value is null) return null;
            if (value is JsonElement je) return ConvertJsonElement(je);
            return value;
        }

        private static Dictionary<string, object> NormalizeForFirestore(Dictionary<string, object> orig)
        {
            var result = new Dictionary<string, object>();
            foreach (var kv in orig)
            {
                var converted = ConvertUnknown(kv.Value);
                if (converted is null) continue;
                result[kv.Key] = converted;
            }
            return result;
        }
    }
}