using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp.Models;
using WebApp.Repository;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Alumno")]
    public class InscripcionController : Controller
    {
        private static ILogger logger;

        private static int alumnoId;

        public InscripcionController(ILogger<InscripcionController> loggerParam)
        {
            logger = loggerParam;
        }

        public IActionResult Inscripcion(int id = -1)
        {
            alumnoId = Convert.ToInt16(User.Identity.Name);
            
            var materiaElegida = new List<MateriaAlumno>();
            var materias = new List<Materia>[2];

            try
            {
                materias= Distinguir();
            }
            catch(Exception e)
            {
                var metodo = MethodBase.GetCurrentMethod().Name;
                logger.LogError(e.Message + "Metodo: {0}, Id: {1}", metodo, id);
            }

            foreach (var materia in materias[Constantes.MateriasDisponibles])
            {
                if (materia.Id == id)
                {
                    foreach (var horario in materia.Horarios)
                    {
                        foreach (var turno in horario.Turnos)
                        {
                            materiaElegida.Add(new MateriaAlumno(materia.Id, materia.Nombre, horario.Dia, turno));
                        }
                    }

                    break;
                }
            } 

            this.ViewData["IdAlumno"] = alumnoId;
            this.ViewData["MateriasDisponibles"] = materias[Constantes.MateriasDisponibles];
            this.ViewData["MateriasNoDisponibles"] = materias[Constantes.MateriasNoDisponibles];
            this.ViewData["Materias"] = materiaElegida; 

            return View();
        }

        public IActionResult ConfirmaInscripcion(int id, string nombre, Dias dia, Turnos turno)
        {
            var nuevaMateria = new MateriaAlumno(id, nombre, dia, turno);
            
            try
            {
                AgregarMateria(nuevaMateria, alumnoId);
            }
            catch(Exception e)
            {
                var metodo = MethodBase.GetCurrentMethod().Name;
                logger.LogError(e.Message + "Metodo: {0}, Id: {1}, Nombre: {2}", metodo, id, nombre);
            }

            return RedirectToAction("Inscripcion");
        }

        List<Materia>[] Distinguir()
        {
            var materias = new List<Materia>(InscripcionRepository.ObtenerMaterias());  
            var vectorListas = new List<Materia>[2];

            vectorListas = DividirListaMaterias(materias, alumnoId);

            return vectorListas;
        }

        void AgregarMateria(MateriaAlumno materia, int idAlumno)
        {
            if (ValidarMateria(materia.Id, materia.Dia, materia.Turno, idAlumno))
            {
                InscripcionRepository.AgregarMateria(idAlumno, materia.Id, materia.Dia, materia.Turno); 
            }
            else
            {
                throw new Exception ("Error al agregar la materia");
            }
        }

        bool ValidarMateria(int idMateria, Dias dia, Turnos turno, int idAlumno)
        {
            if(!InscripcionRepository.ValidarCorrelativas(idMateria, idAlumno))
            {
                return false;
            }

            if(InscripcionRepository.ValidarMateriaAprobada(idMateria, idAlumno))
            {
                return false;
            }

            if(InscripcionRepository.ValidarMateriaEnCurso(idMateria, idAlumno))
            {
                return false;
            }
                
            if(!InscripcionRepository.ValidarHorarioLibre(idAlumno, dia, turno).Equals(Constantes.Default))
            {
                return false;
            }

            return true;
        }

        List<Materia>[] DividirListaMaterias(List<Materia> materias, int idAlumno)
        {
            var materiasDisponibles = new List<Materia>();
            var materiasNoDisponibles = new List<Materia>();
            var vectorListas = new List<Materia>[2]; 

            foreach (var materia in materias)
            {   
                var horariosValidos = HorariosDisponibles(materia.Horarios, idAlumno);

                if (InscripcionRepository.ValidarMateriaAprobada(materia.Id, idAlumno)
                || (!InscripcionRepository.ValidarCorrelativas(materia.Id, idAlumno))
                || (InscripcionRepository.ValidarMateriaEnCurso(materia.Id, idAlumno))
                || (horariosValidos.Count == 0 ) )
                {
                    materiasNoDisponibles.Add(materia);
                }
                else
                {
                    materiasDisponibles.Add(materia);
                    materiasDisponibles.Last().Horarios = new List<Horario>(horariosValidos);
                }
            }

            vectorListas[Constantes.MateriasDisponibles] = materiasDisponibles;
            vectorListas[Constantes.MateriasNoDisponibles] = materiasNoDisponibles;
            
            return vectorListas;
        }

        List<Horario> HorariosDisponibles(List<Horario> horarios, int idAlumno)
        {
            var horariosValidos = new List<Horario>(horarios);

            for (int i = 0; i < horariosValidos.Count; i++)
            {
                for (int j = 0; j < horariosValidos[i].Turnos.Count; j++)
                {
                    if(!(InscripcionRepository.ValidarHorarioLibre(idAlumno, horariosValidos[i].Dia, horariosValidos[i].Turnos[j]) == Constantes.Default))
                    {
                        horariosValidos[i].Turnos.Remove(horariosValidos[i].Turnos[j]);
                        j--;
                    }
                }

                if (horariosValidos[i].Turnos.Count == 0)
                {
                    horariosValidos.Remove(horariosValidos[i]);
                    i--;
                }
            }

            return horariosValidos;
        } 

        public static string ValidarHorarioLibreDDBB(int idAlumno, Dias dia, Turnos turno)
        {
            var materia = "";

            try
            {
                materia = InscripcionRepository.ValidarHorarioLibre(idAlumno, dia, turno);
            }
            catch(Exception e)
            {
                var metodo = MethodBase.GetCurrentMethod().Name;
                logger.LogError(e.Message + "Metodo: {0}, Id: {1}", metodo, idAlumno);
            }

            return materia;
        }
    }
}