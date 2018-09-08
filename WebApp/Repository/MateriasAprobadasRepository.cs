using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApp.Models;

namespace WebApp.Repository
{
    public class MateriasAprobadasRepository
    {
        public static List<MateriaAlumno> ListaMateriasAprobadas(int alumnoId)
        {
            var materiaAprobada = new List<MateriaAlumno>();

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT *
                                            FROM MateriaAlumno MA
                                            JOIN MateriaHorarios MH
                                              ON MA.MateriaHorarioId = MH.Id
                                            JOIN Materias M
                                              ON MH.MateriaId = M.Id
                                           WHERE (MA.AlumnoId = @AlumnoId)
                                             AND (MA.Estado = 1)";
                cmdSelect.Parameters.Add( new SqlParameter("AlumnoId", alumnoId));
                cmdSelect.Connection = conn;
                
                using(var resultSelect = cmdSelect.ExecuteReader())
                {
                    while(resultSelect.Read())
                    {
                        var materia = new MateriaAlumno((int)resultSelect["MateriaId"], (string)resultSelect["Nombre"], Dias.Lunes, Turnos.Mañana);
                        materia.Nota = (Int16)resultSelect["Nota"];
                        materiaAprobada.Add(materia);
                    }
                }
            }

            return materiaAprobada;
        }
    }
}
