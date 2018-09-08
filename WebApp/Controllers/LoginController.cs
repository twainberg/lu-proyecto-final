using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp.Models;
using WebApp.Repository;

namespace WebApp.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private static ILogger logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public LoginController(ILogger<LoginController> loggerParam, UserManager<ApplicationUser> userManagerParam, SignInManager<ApplicationUser> signInManagerParam)
        {
            logger = loggerParam;
            userManager = userManagerParam;
            signInManager = signInManagerParam;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Legajo, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("RedireccionarUsuario", "Login");
                }
                else
                { 
                    ModelState.AddModelError("CustomError", "Alguno de los campos ingresados es incorrecto");
                    return View(model);         
                }
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> RedireccionarUsuario()
        {
            var legajo = User.Identity.Name;

            var usuario = await userManager.FindByNameAsync(legajo);

            IList<string> roles = await userManager.GetRolesAsync(usuario);

            if(roles.Contains("Admin"))
                return RedirectToAction("Administrador", "Administrador");
            else
                return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout() 
        { 
            await signInManager.SignOutAsync(); 

            return RedirectToAction("Login", "Login"); 
        } 

        [HttpPost]
        public async Task<IActionResult> RegistrarAlumno(LoginModel model)
        {
            try
            {
                var id = LoginRepository.RegistrarAlumnoDDBB(model.NombreCompleto, model.Dni);
                model.Legajo = id.ToString();
                await LoginRepository.CrearUsuarioAuth(model, userManager);
                await signInManager.PasswordSignInAsync(model.Legajo, model.Password, false, false);

                return RedirectToAction("LegajoAsignado", "Login");;
            }
            catch (SqlException e)
            {
                var metodo = MethodBase.GetCurrentMethod().Name;
                logger.LogError(e.Message + "Metodo: {0}, Nombre: {1}, Dni: {2}, Code: {3}", metodo, model.NombreCompleto, model.Dni, e.Number);
                
                if(e.Number == 2627)
                    ModelState.AddModelError("CustomError", "DNI ya se encuentra registrado");
                else
                    ModelState.AddModelError("CustomError", "No se pudo registrar al alumno");

                return View("Login");
            }
            catch (Exception e)
            {
                var metodo = MethodBase.GetCurrentMethod().Name;
                logger.LogError(e.Message + "Metodo: {0}, Nombre: {1}, Dni: {2}", metodo, model.NombreCompleto, model.Dni);
                ModelState.AddModelError("CustomError", "No se pudo registrar al alumno");

                return View("Login");
            }
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

        [Authorize(Roles = "Alumno")]
        public IActionResult LegajoAsignado()
        {
            this.ViewData["IdAlumno"] = User.Identity.Name;

            return View();
        }
    }
}
