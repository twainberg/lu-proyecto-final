using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Repository
{
    public class AdministradorRepository 
    {
        public static List<MateriaAlumno> ListaMateriasAprobadas(int alumnoId)
        {
            var materiasAlumno = new List<MateriaAlumno>();

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT MA.Id as MateriaAlumnoId, *
                                            FROM MateriaAlumno MA
                                            JOIN MateriaHorarios MH
                                              ON MA.MateriaHorarioId = MH.Id
                                            JOIN Materias M
                                              ON MH.MateriaId = M.Id
                                            JOIN Horarios H
                                              ON MH.HorarioId = H.Id
                                           WHERE (MA.AlumnoId = @AlumnoId)";
                cmdSelect.Parameters.Add( new SqlParameter("AlumnoId", alumnoId));
                cmdSelect.Connection = conn;
                
                using(var resultSelect = cmdSelect.ExecuteReader())
                {
                    while(resultSelect.Read())
                    {
                        var id = (int)resultSelect["MateriaAlumnoId"];
                        var nombre = (string)resultSelect["Nombre"];
                        var dia = Enum.Parse<Dias>(resultSelect["Dia"].ToString());;
                        var turno = Enum.Parse<Turnos>(resultSelect["Turno"].ToString());
                        var materia = new MateriaAlumno(id, nombre, dia, turno);
                        
                        materia.Nota = (Int16)resultSelect["Nota"];
                        
                        if((bool)resultSelect["Estado"] == true)
                        {
                            materia.Status = StatusMateria.Aprobada;
                        }
                        else
                        {
                            materia.Status = StatusMateria.Cursando;
                        }  
                    
                        materiasAlumno.Add(materia);
                    }            
                }
            }

            return materiasAlumno;
        }

        public static void ActualizarMateria(int materiaAlumnoId, int nota, StatusMateria estado)
        {
            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdUpdate = new SqlCommand();
                cmdUpdate.CommandText = @"UPDATE MateriaAlumno
                                             SET Nota = @Nota,
                                                 Estado = @Estado
                                           WHERE Id = @Id";
                cmdUpdate.Parameters.Add( new SqlParameter("Nota", nota));
                cmdUpdate.Parameters.Add( new SqlParameter("Estado", estado.GetHashCode()));
                cmdUpdate.Parameters.Add( new SqlParameter("Id", materiaAlumnoId));
                cmdUpdate.Connection = conn;
                
                var resultUpdate = cmdUpdate.ExecuteNonQuery();

                if (resultUpdate != 1)
                {
                    throw new Exception("Se inserto incorrectamente la materia");
                }
            }
        }
    }
}