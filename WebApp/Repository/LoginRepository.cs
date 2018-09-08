using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Repository
{

    public class LoginRepository 
    {
        public static async Task CrearUsuarioAuth(LoginModel model, UserManager<ApplicationUser> userManager)
        {
            var usuario = new ApplicationUser
            {
                UserName = model.Legajo
            };

            var busquedaUsuario = await userManager.FindByNameAsync(model.Legajo);

            if(busquedaUsuario == null)
            {
                var crearUsuario = await userManager.CreateAsync(usuario, model.Password);
                if (crearUsuario.Succeeded)
                {
                    await userManager.AddToRoleAsync(usuario, "Alumno");
                }
                else
                {
                    throw new Exception("Error al crear usuario");
                }
            }
            else
            {
                throw new Exception("Ya existe usuario");
            }
        }

        public static int RegistrarAlumnoDDBB(string nombre, string dni)
        {
            using(var conn = new SqlConnection(Program.Configuration["ConnectionStrings:DefaultConnection"]))
            {
                conn.Open();

                var cmdInsert = new SqlCommand();
                cmdInsert.CommandText = @"INSERT INTO Alumnos 
                                                    (Nombre, Dni)
                                            VALUES (@Nombre, @Dni)";
                cmdInsert.Parameters.Add( new SqlParameter("Nombre", nombre));
                cmdInsert.Parameters.Add( new SqlParameter("DNI", dni));
                cmdInsert.Connection = conn;
                
                var resultInsert = cmdInsert.ExecuteNonQuery();
            
                if (resultInsert != 1)
                {
                    throw new Exception("Ocurrió un error al registrar al alumno.");
                }

                cmdInsert.Parameters.Clear();
                cmdInsert.CommandText = "SELECT @@IDENTITY";
                var legajoGenerado = Convert.ToInt16(cmdInsert.ExecuteScalar());
                
                return legajoGenerado;
            }
        }
    }
}