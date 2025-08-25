using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Core.Models
{
    public class LoginRequest
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
    }
}