using System;
using System.Collections;
using System.Collections.Generic;

namespace WebApp.Models
{ 
    public class Materia
    {
        int id;

        string nombre;

        List<Horario> horarios;

        public int Id
        {
            get{ return this.id; }
        }

        public string Nombre
        {
            get{ return this.nombre; }
        }

        public List<Horario> Horarios
        {
            get{ return this.horarios; }
            set
            {
                if (value.Count == 0)
                {
                    throw new Exception ("La lista de horarios no puede estar vacía");
                }

                this.horarios = new List<Horario>(value);
            }
        }

        public Materia(int idMateria, string nombre, List<Horario> horarios)
        {
            this.id = idMateria;
            this.nombre = nombre;
            this.Horarios = new List<Horario>(horarios);
        }

        public Materia(Materia materia)
        {
            this.id = materia.Id;
            this.nombre = materia.Nombre;
            this.Horarios = new List<Horario>(materia.Horarios);
        }
    }
}