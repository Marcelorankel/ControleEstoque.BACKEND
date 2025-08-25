using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ControleEstoque.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eTipoUsuario
    {
        [Description("Administrador")]
        ADMIN = 0,
        [Description("Vendedor")]
        VENDEDOR = 1
    }
}