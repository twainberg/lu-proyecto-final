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
    [Authorize(Roles = "Alumno")]
    public class AmigosController : Controller
    {
        private static ILogger logger;

        private static int alumnoId;

        public AmigosController(ILogger<AmigosController> loggerParam)
        {
            logger = loggerParam;
        }

        public IActionResult Amigos()
        {
            alumnoId = Convert.ToInt16(User.Identity.Name);

            this.ViewData["Error"] = "";
            this.ViewData["Alumno"] = "";
            this.ViewData["Amigos"] = new List<Alumno>(AmigosRepository.ObtenerAmigos(alumnoId));
            this.ViewData["Id"] = 0;

            return View();
        }
    
        [HttpPost]
        public IActionResult BuscarAlumno(int id = 0, string dni = "0")
        {
            var alumnoLogeado = HomeRepository.ObtenerAlumno(alumnoId);

            this.ViewData["Alumno"] = "";
            
            if ((id != alumnoLogeado.Id && dni == null) || (id == 0 && dni != alumnoLogeado.DNI))
            {
                try
                {
                    var alumno = new Alumno(HomeRepository.ObtenerAlumno(id, dni));
                    id = alumno.Id;
                    this.ViewData["Alumno"] = "Legajo " + id + "| " + alumno.Nombre;
                }
                catch(Exception e)
                {
                    var metodo = MethodBase.GetCurrentMethod().Name;
                    logger.LogError(e.Message + "Metodo: {0}, Dni: {1}, Id: {2}", metodo, dni, id);
                    this.ViewData["Error"] = "Alumno no encontrado";
                }
            }
            else this.ViewData["Error"] = "No se puede buscar al usuario que esta iniciado en la sesion";

            this.ViewData["Amigos"] = new List<Alumno>(AmigosRepository.ObtenerAmigos(alumnoId));
            this.ViewData["Id"] = id;

            return View("Amigos");
        }

        public IActionResult AgregarAmigo(int seguidoId)
        {
            try
            {
                if(!AmigosRepository.VerificarAmigo(seguidoId, alumnoId))
                {
                        AmigosRepository.AgregarAmigo(alumnoId, seguidoId);
                        this.ViewData["Error"] = "Se agregó correctamente al amigo";                    
                }
                else this.ViewData["Error"] = "No se puede seguir dos veces a la misma persona";
            }
            catch(Exception e)
            {
                var metodo = MethodBase.GetCurrentMethod().Name;
                logger.LogError(e.Message + "Metodo: {0}, AlumnoId: {1}, SeguidoId: {2}", metodo, alumnoId, seguidoId);
                this.ViewData["Error"] = "No se pudo agregar amigo";
            }

            this.ViewData["Amigos"] = new List<Alumno>(AmigosRepository.ObtenerAmigos(alumnoId));
            this.ViewData["Alumno"] = "";
            this.ViewData["Id"] = 0;
            
            return View("Amigos");
        }

        public IActionResult EliminarAmigo(int seguidoId)
        {
            try
            {
                AmigosRepository.EliminarAmigo(alumnoId, seguidoId);
                this.ViewData["Error"] = "Se eliminó correctamente al amigo";
            }
            catch(Exception e)
            {
                var metodo = MethodBase.GetCurrentMethod().Name;
                logger.LogError(e.Message + "Metodo: {0}, AlumnoId: {1}, SeguidoId: {2}", metodo, alumnoId, seguidoId);
                this.ViewData["Error"] = "No se pudo eliminar amigo";
            }

            this.ViewData["Amigos"] = new List<Alumno>(AmigosRepository.ObtenerAmigos(alumnoId));
            this.ViewData["Alumno"] = "";
            this.ViewData["Id"] = 0;

            return View("Amigos"); 
        }

        public static bool AmigosEnMateriaDDBB(int materiaId)
        {
            var amigosEnMateria = false;

            try
            {
                amigosEnMateria = AmigosRepository.AmigosEnMateria(materiaId, alumnoId);
            }
            catch(Exception e)
            {
                var metodo = MethodBase.GetCurrentMethod().Name;
                logger.LogError(e.Message + "Metodo: {0}, MateriaId: {1}", metodo, materiaId);
            }

            return amigosEnMateria;
        }

        public static List<string> AmigosEnMateriaDDBB(int materiaId, Dias dia, Turnos turno)
        {
            var amigosEnMateria = new List<string>();

            try
            {
                amigosEnMateria = new List<string>(AmigosRepository.AmigosEnMateria(alumnoId, materiaId, dia, turno));
            }
            catch(Exception e)
            {
                var metodo = MethodBase.GetCurrentMethod().Name;
                logger.LogError(e.Message + "Metodo: {0}, MateriaId: {1}, Dia: {2}, Turno: {3}", metodo, materiaId, dia, turno);
            }

            return amigosEnMateria;
        }
    }
}