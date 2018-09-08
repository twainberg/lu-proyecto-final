using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp.Models;
using WebApp.Repository;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministradorController : Controller
    {
        private static ILogger logger;

        public AdministradorController(ILogger<AdministradorController> loggerParam)
        {
            logger = loggerParam;
        }

        public IActionResult Administrador()
        {
            this.ViewData["Alumno"] = "";
            this.ViewData["Error"] = "";
            this.ViewData["MateriasAlumno"] = new List<MateriaAlumno>();

            return View();
        }

        public IActionResult BuscarAlumno(int id = 0, string dni = "0")
        {
            var materiasAlumno = new List<MateriaAlumno>();
            this.ViewData["Error"] = "";

            try
            {
                var alumno = new Alumno (HomeRepository.ObtenerAlumno(id, dni));
                materiasAlumno = new List<MateriaAlumno>(AdministradorRepository.ListaMateriasAprobadas(alumno.Id));
                this.ViewData["Alumno"] = alumno.Nombre;
            }
            catch(Exception e)
            {
                var metodo = MethodBase.GetCurrentMethod().Name;
                logger.LogError(e.Message + "Metodo: {0}, Dni: {1}, Id: {2}", metodo, dni, id);
                this.ViewData["Error"] = "Alumno no encontrado";
                this.ViewData["Alumno"] = "";
            }

            this.ViewData["MateriasAlumno"] = materiasAlumno;

            return View("Administrador");
        }

        public JsonResult ActualizarMateria(int materiaAlumnoId, int nota, StatusMateria estado)
        {
            var resultado = "No se pudo actualizar la materia";
            var codigo = -1;
            
            try
            {
                if (estado == StatusMateria.Aprobada && nota < 7)
                {
                    resultado = "El estado no puede ser aprobada, si la nota es menor a 7";
                    throw new Exception("El estado no puede ser aprobada, si la nota es menor a 7");
                }

                AdministradorRepository.ActualizarMateria(materiaAlumnoId, nota, estado);   
                codigo = 0;
                resultado = "Materia actualizada correctamente";
            }
            catch (Exception e)
            {
                var metodo = MethodBase.GetCurrentMethod().Name;
                logger.LogError(e.Message + "Metodo: {0}, Id: {1}, Nota: {2}, Estado: {3}", metodo, materiaAlumnoId, nota, estado);
            }

            return Json(new {resultado = resultado, codigo = codigo});
        }
    }
}