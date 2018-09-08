using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Alumno")]
    public class HomeController : Controller
    {
        private static int alumnoId;

        public IActionResult Index()
        {
            alumnoId = Convert.ToInt16(User.Identity.Name);
            
            this.ViewData["Materias"] = new List<MateriaAlumno>();
            this.ViewData["IdAlumno"] = alumnoId;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
