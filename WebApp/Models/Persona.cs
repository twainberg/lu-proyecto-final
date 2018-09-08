using System;

namespace WebApp.Models
{    
    public class Persona
    {
        string dni;

        string nombre;

        public string DNI
        {
            get{ return this.dni; }
            protected set{ this.dni = value; }
        }

        public string Nombre
        {
            get{ return this.nombre; }
            set{ this.nombre = value; }
        }
    }
}
