using ControleEstoque.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Core.Interfaces.Service
{
    public interface IAuthService
    {
        Task<string> Login(LoginRequest loginRequest);
    }
}