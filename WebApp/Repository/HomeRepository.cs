using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repository;

namespace WebApp.Repository
{
    public class HomeRepository
    {
        public static Alumno ObtenerAlumno(int id, string dni)
        {
            var alumnoObtenido = new Alumno("","",0);

            if (id != 0)
            {
                alumnoObtenido = new Alumno (HomeRepository.ObtenerAlumno(id));
            }
            else if (dni != "0")
            {
                alumnoObtenido = new Alumno (HomeRepository.ObtenerAlumno(dni));
            }

            return alumnoObtenido;
        }

        public static Alumno ObtenerAlumno(int id) 
        {
            string dni = "";
            string nombre = "";

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();


                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT *
                                            FROM Alumnos
                                           WHERE (Id = @id )";
                cmdSelect.Parameters.Add( new SqlParameter("id", id));
                cmdSelect.Connection = conn;

                using(var result = cmdSelect.ExecuteReader())
                {
                    while(result.Read())
                    {
                        dni = result["DNI"].ToString();
                        nombre = result["Nombre"].ToString();
                    }
                }
            }

            if (dni == "" || nombre == "")
            {
                throw new Exception("Alumno no encontrado");
            }

            var alumno = new Alumno(dni, nombre, id);

            return alumno;
        }

        public static Alumno ObtenerAlumno(string dni) 
        {
            string nombre = "";
            int id = 0;

            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdSelect = new SqlCommand();
                cmdSelect.CommandText = @"SELECT *
                                            FROM Alumnos 
                                           WHERE (DNI = @Dni)";
                cmdSelect.Parameters.Add( new SqlParameter("Dni", dni));
                cmdSelect.Connection = conn;
                
                using(var result = cmdSelect.ExecuteReader())
                {
                    while(result.Read())
                    {
                        nombre = result["Nombre"].ToString();
                        id = (int)result["Id"];
                    }
                }
            }

            if (id == 0 || nombre == "")
            {
                throw new Exception("Alumno no encontrado");
            }

            var alumno = new Alumno(dni, nombre, id);
            
            return alumno;
        }
    }
}