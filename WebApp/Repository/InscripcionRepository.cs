using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Repository
{
    public class InscripcionRepository
    {
        public static void AgregarMateria(int id, int idMateria, Dias dia, Turnos turno) 
        {      
            int idMateriaHorario = ObtenerIdMateriaHorario(idMateria, dia, turno);

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdInsert = new SqlCommand();
                cmdInsert.CommandText = @"INSERT INTO MateriaAlumno 
                                                 (AlumnoId, MateriaHorarioId, Nota, Estado)
                                          VALUES (@AlumnoId, @MateriaHorarioId, 0, 0 )";
                cmdInsert.Parameters.Add( new SqlParameter("AlumnoId", id));
                cmdInsert.Parameters.Add( new SqlParameter("MateriaHorarioId", idMateriaHorario));
                cmdInsert.Connection = conn;
                
                var resultInsert = cmdInsert.ExecuteNonQuery();

                if (resultInsert != 1)
                {
                    throw new Exception("Se inserto incorrectamente la materia");
                }
            }
        }

        public static int ObtenerIdMateriaHorario(int idMateria, Dias dia, Turnos turno)
        {
            int idMateriaHorario = 0;

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT Id
                                            FROM MateriaHorarios
                                           WHERE (MateriaId = (SELECT Id
                                                                 FROM Materias
                                                                WHERE (Id = @IdMateria )))
                                             AND (HorarioId = (SELECT Id 
                                                                 FROM Horarios 
                                                                WHERE (Dia = @Dia)
                                                                  AND (Turno = @Turno)))";
                cmdSelect.Parameters.Add( new SqlParameter("IdMateria", idMateria));
                cmdSelect.Parameters.Add( new SqlParameter("Dia", dia.ToString()));
                cmdSelect.Parameters.Add( new SqlParameter("Turno", turno.ToString()));
                cmdSelect.Connection = conn;
                
                using(var resultSelect = cmdSelect.ExecuteReader())
                {
                    while(resultSelect.Read())
                    {
                        idMateriaHorario = (int)resultSelect["Id"];
                    }
                }
            }

            if (idMateriaHorario == 0)
            {
                throw new Exception("IdMateriaHorario no encontrado");
            }

            return idMateriaHorario;
        }

        public static string ValidarHorarioLibre(int idAlumno, Dias dia, Turnos turno)
        {
            var nombre = Constantes.Default;

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT M.Nombre
                                            FROM Materias M
                                            JOIN MateriaHorarios MH
                                              ON M.Id = MH.MateriaId
                                            JOIN Horarios H
                                              ON MH.HorarioId = H.Id
                                            JOIN MateriaAlumno MA
                                              ON MH.Id = MA.MateriaHorarioId
                                           WHERE (MA.AlumnoId = @IdAlumno)
                                             AND (H.Dia = @Dia)
                                             AND (H.Turno = @Turno)
                                             AND (MA.Estado = 0)";
                cmdSelect.Parameters.Add( new SqlParameter("IdAlumno", idAlumno));
                cmdSelect.Parameters.Add( new SqlParameter("Dia", dia.ToString()));
                cmdSelect.Parameters.Add( new SqlParameter("Turno", turno.ToString()));
                cmdSelect.Connection = conn;
                
                using(var resultSelect = cmdSelect.ExecuteReader())
                {
                    while(resultSelect.Read())
                    {
                        nombre = resultSelect["Nombre"].ToString();
                    }
                }
            }

            return nombre;
        }

        public static bool ValidarMateriaEnCurso(int idMateria, int idAlumno)
        {
            var enCurso = false;

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
                                           WHERE (M.Id = @IdMateria)
                                             AND (MA.AlumnoId = @IdAlumno)
                                             AND (MA.Estado = 0)";
                cmdSelect.Parameters.Add( new SqlParameter("IdMateria", idMateria));
                cmdSelect.Parameters.Add( new SqlParameter("IdAlumno", idAlumno));
                cmdSelect.Connection = conn;
                
                enCurso = cmdSelect.ExecuteReader().HasRows;
            }

            return enCurso;
        }

        public static bool ValidarCorrelativas(int idMateria, int idAlumno)
        {
            var listaCorrelativas = ObtenerCorrelativas(idMateria);
            var correlativasAprobadas = true;

            if(listaCorrelativas.Count != 0)
            {
                foreach (var id in listaCorrelativas)
                {
                    if(!ValidarMateriaAprobada(id, idAlumno))
                    {
                        correlativasAprobadas = false;
                        break;
                    }
                }
            }

            return correlativasAprobadas;
        }

        public static List<int> ObtenerCorrelativas(int idMateria)
        {
            var listaCorrelativas = new List<int>();

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT C.Materia_correlativa
                                            FROM Correlativas C
                                            JOIN Materias M
                                              ON C.Materia_ppal = M.Id
                                           WHERE (M.Id = @IdMateria)";
                cmdSelect.Parameters.Add( new SqlParameter("IdMateria", idMateria));
                cmdSelect.Connection = conn;
                
                using(var resultSelect = cmdSelect.ExecuteReader())
                {
                    while(resultSelect.Read())
                    {
                        listaCorrelativas.Add((int)resultSelect["Materia_correlativa"]);
                    }
                }
            }

            return listaCorrelativas;        
        }

        public static bool ValidarMateriaAprobada(int idMateria, int idAlumno)
        {
            var materiaAprobada = false;

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT *
                                            FROM MateriaAlumno MA
                                            JOIN MateriaHorarios MH
                                              ON MA.MateriaHorarioId = MH.Id
                                           WHERE (MH.MateriaId = @IdMateria)
                                             AND (MA.AlumnoId = @IdAlumno)
                                             AND (MA.Estado = 1)";
                cmdSelect.Parameters.Add( new SqlParameter("IdMateria", idMateria));
                cmdSelect.Parameters.Add( new SqlParameter("IdAlumno", idAlumno));
                cmdSelect.Connection = conn;
                
                materiaAprobada = cmdSelect.ExecuteReader().HasRows;
            }

            return materiaAprobada;
        }

        public static List<Materia> ObtenerMaterias()
        {
            var materiasObtenidas = new List<Materia>();
            var materiaAnterior = "";
            var idMateria = 0;
            var horarios = new List<Horario>();
            
            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT *
                                            FROM Materias M
                                            JOIN MateriaHorarios MH
                                              ON M.Id = MH.MateriaId
                                            JOIN Horarios H
                                              ON MH.HorarioId = H.Id
                                           ORDER BY M.Id";
                cmdSelect.Connection = conn;

                using(var resultSelectMaterias = cmdSelect.ExecuteReader())
                {
                    while(resultSelectMaterias.Read())
                    {
                        if (materiaAnterior != resultSelectMaterias["Nombre"].ToString())
                        {
                            if (materiaAnterior != "")
                            {
                                materiasObtenidas.Add(new Materia(idMateria, materiaAnterior, horarios));
                            }
                            idMateria = (int)resultSelectMaterias["Id"];
                            materiaAnterior = resultSelectMaterias["Nombre"].ToString();
                            horarios.Clear();
                        }

                        var diaEnum = Enum.Parse<Dias>(resultSelectMaterias["Dia"].ToString());
                        var turnoEnum = Enum.Parse<Turnos>(resultSelectMaterias["Turno"].ToString());
                        horarios.Add( new Horario(diaEnum, turnoEnum));   
                    }
                    
                    materiasObtenidas.Add(new Materia(idMateria, materiaAnterior, horarios));
                }
            }

            return materiasObtenidas;
        }
    }
}