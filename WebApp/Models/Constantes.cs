using System;

namespace WebApp.Models
{  
    public enum Turnos { Mañana, Tarde, Noche}
    public enum Dias { Lunes, Martes, Miércoles, Jueves, Viernes, Sábado }
    public enum StatusMateria { Cursando, Aprobada }

    public static class Constantes
    {
        public const string Default = "";

        public const int MateriasDisponibles = 0;

        public const int MateriasNoDisponibles = 1;
    }
}