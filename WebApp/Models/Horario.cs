using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public class Horario
    {
        Dias dia;

        List<Turnos> turnos;

        public Dias Dia
        {
            get{ return this.dia; }
            set{ this.dia = value; }
        } 

        public List<Turnos> Turnos
        {
            get{ return this.turnos; }
            set
            { 
                if (value.Count == 0)
                {
                    throw new Exception ("La lista de turnos no puede estar vacía");
                }
                
                this.turnos = new List<Turnos>(value);
            }
        } 

        public Horario(Dias dia, Turnos turno)
        {
            this.Dia = dia;
            this.turnos = new List<Turnos>();
            this.turnos.Add(turno);
        }

        public Horario(Dias dia, List<Turnos> turnos)
        {
            this.Dia = dia;
            this.Turnos = new List<Turnos>(turnos);
        }

        public Horario(Horario horario)
        {
            this.Dia = horario.Dia;
            this.Turnos = new List<Turnos>(horario.Turnos);
        }
    }
}
