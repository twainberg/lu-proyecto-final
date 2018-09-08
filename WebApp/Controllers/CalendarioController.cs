using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Alumno")]
    public class CalendarioController : Controller
    {
        public ActionResult CalendarioAmigo( int id = 0 )
        {
            this.ViewData["IdAlumno"] = id;
            this.ViewData["Materias"] = new List<MateriaAlumno>();

            return PartialView("Calendario");
        }   
    }
}