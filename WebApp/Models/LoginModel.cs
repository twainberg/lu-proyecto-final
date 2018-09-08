using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Legajo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Text)]
        public string NombreCompleto { get; set; }

        [DataType(DataType.Text)]
        public string Dni { get; set; }
    }
}