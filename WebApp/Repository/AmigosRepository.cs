using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Repository
{
    public class AmigosRepository 
    {
        public static bool VerificarAmigo(int seguidoId, int alumnoId)
        {
            var resultSelect = false;

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT Id 
                                            FROM Seguidores
                                           WHERE AlumnoId = @AlumnoId 
                                             AND SeguidoId = @SeguidoId";
                cmdSelect.Parameters.Add( new SqlParameter("AlumnoId", alumnoId));
                cmdSelect.Parameters.Add( new SqlParameter("SeguidoId", seguidoId));
                cmdSelect.Connection = conn;

                resultSelect = cmdSelect.ExecuteReader().HasRows;
            }
            
            return resultSelect;
        }

        public static void AgregarAmigo(int alumnoId, int seguidoId)
        {
            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdInsert = new SqlCommand();
                cmdInsert.CommandText = @"INSERT INTO Seguidores 
                                                    (AlumnoId, SeguidoId)
                                            VALUES (@AlumnoId, @SeguidoId)";
                cmdInsert.Parameters.Add( new SqlParameter("AlumnoId", alumnoId));
                cmdInsert.Parameters.Add( new SqlParameter("SeguidoId", seguidoId));
                cmdInsert.Connection = conn;
                
                var resultInsert = cmdInsert.ExecuteNonQuery();
            
                if (resultInsert != 1)
                {
                    throw new Exception("No se pudo agregar al amigo");
                }
            }
        }

        public static void EliminarAmigo(int alumnoId, int seguidoId)
        {
            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdDelete = new SqlCommand();
                cmdDelete.CommandText = @"DELETE Seguidores
                                           WHERE AlumnoId = @AlumnoId 
                                             AND SeguidoId = @SeguidoId";
                cmdDelete.Parameters.Add( new SqlParameter("AlumnoId", alumnoId));
                cmdDelete.Parameters.Add( new SqlParameter("SeguidoId", seguidoId));
                cmdDelete.Connection = conn;
                
                var resultDelete = cmdDelete.ExecuteNonQuery();

                if (resultDelete != 1)
                {
                    throw new Exception("No se pudo eliminar al amigo");
                }
            }  
        }

        public static List<Alumno> ObtenerAmigos(int alumnoId)
        {
            var alumnosObtenidos = new List<Alumno>();
            
            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT A.*
                                            FROM Seguidores S
                                            JOIN Alumnos A
                                              ON S.SeguidoId = A.Id
                                           WHERE S.AlumnoId = @AlumnoId
                                           ORDER BY A.Id";
                cmdSelect.Parameters.Add( new SqlParameter("AlumnoId", alumnoId));
                cmdSelect.Connection = conn;

                using(var resultSelectAmigos = cmdSelect.ExecuteReader())
                {
                    while(resultSelectAmigos.Read())
                    {
                        var alumno = new Alumno((string)resultSelectAmigos["DNI"], (string)resultSelectAmigos["Nombre"], (int)resultSelectAmigos["Id"]);
                        alumnosObtenidos.Add(alumno);
                    }
                }
            }

            return alumnosObtenidos; 
        }

        public static bool AmigosEnMateria(int materiaId, int alumnoId)
        {
            var cursaAmigo = false;

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT *
                                            FROM MateriaAlumno MA
                                            JOIN Seguidores S
                                              ON MA.AlumnoId = S.SeguidoId
                                            JOIN MateriaHorarios MH
                                              ON MA.MateriaHorarioId = MH.Id
                                           WHERE (S.AlumnoId = @AlumnoId)
                                             AND (MH.MateriaId = @MateriaId)
                                             AND (MA.Estado = 0)";
                cmdSelect.Parameters.Add( new SqlParameter("AlumnoId", alumnoId));
                cmdSelect.Parameters.Add( new SqlParameter("MateriaId", materiaId));
                cmdSelect.Connection = conn;

                cursaAmigo = cmdSelect.ExecuteReader().HasRows;
            }

            return cursaAmigo;
        }

        public static List<string> AmigosEnMateria(int alumnoId, int materiaId, Dias dia, Turnos turno)
        {
            var amigosEnMateria = new List<string>();

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT A.Nombre
                                            FROM MateriaAlumno MA
                                            JOIN Alumnos A
                                              ON MA.AlumnoId = A.Id
                                            JOIN Seguidores S
                                              ON MA.AlumnoId = S.SeguidoId
                                            JOIN MateriaHorarios MH
                                              ON MA.MateriaHorarioId = MH.Id
                                            JOIN Horarios H
                                              ON MH.HorarioId = H.Id
                                           WHERE (S.AlumnoId = @AlumnoId)
                                             AND (MH.MateriaId = @MateriaId)
                                             AND (H.Dia = @Dia)
                                             AND (H.Turno = @Turno)
                                             AND (MA.Estado = 0)";
                cmdSelect.Parameters.Add( new SqlParameter("AlumnoId", alumnoId));
                cmdSelect.Parameters.Add( new SqlParameter("MateriaId", materiaId));
                cmdSelect.Parameters.Add( new SqlParameter("Dia", dia.ToString()));
                cmdSelect.Parameters.Add( new SqlParameter("Turno", turno.ToString()));
                cmdSelect.Connection = conn;

                using(var resultSelect = cmdSelect.ExecuteReader())
                {
                    while(resultSelect.Read())
                    {
                        amigosEnMateria.Add((string)resultSelect["Nombre"]);
                    }
                }
            }

            return amigosEnMateria;
        }
    }
}