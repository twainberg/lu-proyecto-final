using System;
using System.Collections.Generic;
using System.Linq;
using WebApp.Controllers;

namespace WebApp.Models
{ 
    public class Alumno : Persona
    {
        int id;

        List<MateriaAlumno> materias;

        List<Alumno> amigos;

        public int Id 
        { 
            get { return this.id; }
        }

        public List<MateriaAlumno> Materias
        {
            get{ return this.materias; }
            set{ this.materias = new List<MateriaAlumno>(value); }
        }

        public List<Alumno> Amigos
        {
            get{ return this.amigos; }
            set{ this.amigos = new List<Alumno>(value); }
        }

        public Alumno(){}

        public Alumno(string dni, string nombre, int id)
        {
            this.DNI = dni;
            this.Nombre = nombre;
            this.id = id;
            this.materias = new List<MateriaAlumno>();
        }

        public Alumno(Alumno alumno)
        {
            this.DNI = alumno.DNI;
            this.Nombre = alumno.Nombre;
            this.id = alumno.Id;
            this.materias = new List<MateriaAlumno>(alumno.Materias);
        }
    }
}