using System;

namespace WebApp.Models
{ 
    public class MateriaAlumno
    {
        int id;
        
        string nombre;

        Dias dia;

        Turnos turno;

        int nota;

        StatusMateria status;

        public int Id
        {
            get{ return this.id; }
        }

        public string Nombre 
        { 
            get{ return this.nombre; } 
        }

        public int Nota 
        { 
            get{ return this.nota; } 
            set
            { 
                if (this.status == StatusMateria.Cursando)
                {
                    this.nota = value; 
                }
                else
                {
                    throw new Exception("No puede modificar la nota de una materia aprobada");
                }                
            } 
        }

        public Dias Dia
        { 
            get{ return this.dia; } 
        }

        public Turnos Turno
        { 
            get{ return this.turno; } 
        }

        public StatusMateria Status 
        { 
            get{ return this.status; } 
            set
            { 
                if (value == StatusMateria.Aprobada && this.nota < 7)
                {
                    throw new Exception ("Para marcar aprobada la materia, la nota debe ser 7 o más");
                }

                this.status = value; 
            } 
        }

        public MateriaAlumno(int id, string nombre, Dias dia, Turnos turno)
        {
            this.id = id;
            this.nombre = nombre;
            this.nota = 0;
            this.dia = dia;
            this.turno = turno;
            this.status = StatusMateria.Cursando;
        }

        public MateriaAlumno(MateriaAlumno materia)
        {
            this.id = materia.Id;
            this.nombre = materia.Nombre;
            this.nota = materia.Nota;
            this.dia = materia.Dia;
            this.turno = materia.Turno;
            this.status = materia.Status;
        }
    }
}