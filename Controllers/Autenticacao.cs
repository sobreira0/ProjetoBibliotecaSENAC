using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using System.Linq;
using System.Collections.Generic;

namespace Biblioteca.Controllers
{
    public class Autenticacao
    {
        public static void CheckLogin(Controller controller)
        {   
            if(string.IsNullOrEmpty(controller.HttpContext.Session.GetString("login")))
            {
                controller.Request.HttpContext.Response.Redirect("/Home/Login");
            }
        }

        public static bool verificaLoginSenha(string login,string senha,Controller controller)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                verificaSeUsuarioAdminExiste(bc);

                senha = Criptografo.TextoCriptografado(senha);

                IQueryable<Usuario> UsuarioEncontrado = bc.Usuarios.Where(u => u.Login==login && u.Senha==senha);
                List<Usuario>ListaUsuarioEncontrado = UsuarioEncontrado.ToList();

                if(ListaUsuarioEncontrado.Count==0)
                {
                    return false; //retorna que nao encontrou
                }
                else
                {
                    //registra na sessao
                    controller.HttpContext.Session.SetString("login",ListaUsuarioEncontrado[0].Login);
                    controller.HttpContext.Session.SetString("Nome",ListaUsuarioEncontrado[0].Nome);
                    controller.HttpContext.Session.SetInt32("tipo",ListaUsuarioEncontrado[0].Tipo);
                    return true;
                }
            }

        }
        public static void verificaSeUsuarioAdminExiste(BibliotecaContext bc)
        {
            IQueryable<Usuario> userEncontrado =  bc.Usuarios.Where(u => u.Login=="admin");

                //se não existir será criado o usuario admin padrão
                if(userEncontrado.ToList().Count==0)
                {
                    Usuario admin = new Usuario();
                    admin.Login = "admin";
                    admin.Senha = Criptografo.TextoCriptografado("123");
                    admin.Tipo = Usuario.ADMIN;
                    admin.Nome = "Administrador";

                    bc.Usuarios.Add(admin);
                    bc.SaveChanges();
                }
        }

        public static void verificaSeUsuarioEAdmin(Controller controller)
        {
            if(!(controller.HttpContext.Session.GetInt32("tipo")==Usuario.ADMIN))
            {
                controller.Request.HttpContext.Response.Redirect("/Usuarios/NeedAdmin");
            }
        }
    }
}