using System;

namespace WebApp.Models
{ 
    public class Administrador : Persona
    {
        int id;

        public int Id 
        { 
            get{ return this.id; } 
            set{ this.id = value; } 
        }
    }
}