using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    internal class Perfil
    {
        public Guid IdPersona { get; set; }
        public string? Video { get; set; }
    }
    public class PerfilBD:Perfil
    {        
        public Guid? Curriculum { get; set; }
        public Guid? Foto { get; set; }
        //////
    }
    public class PerfilRequest:Perfil
    {
        public DocumentoContenido? Curriculum { get; set; }
        public DocumentoContenido? Foto { get; set; }

    }
}
