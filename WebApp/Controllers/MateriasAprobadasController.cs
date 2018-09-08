using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Repository;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Alumno")]
    public class MateriasAprobadasController : Controller
    {
        private static int alumnoId;

        public IActionResult MateriasAprobadas()
        {    
            alumnoId = Convert.ToInt16(User.Identity.Name);

            ViewData["MateriasAprobadas"] = MateriasAprobadasRepository.ListaMateriasAprobadas(alumnoId);
            return View();
        }
    }
}