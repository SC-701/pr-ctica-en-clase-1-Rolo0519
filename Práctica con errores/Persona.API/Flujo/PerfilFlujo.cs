using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos;
using Flujo.Helper;

namespace Flujo
{
    public class PerfilFlujo : IPerfilFlujo
    {        
        private IPerfilDA _perfilDA;
        private IDocumentoDA _documentoDA;
        private IDocumentoServicio _documentoServicio;

        public PerfilFlujo(IDocumentoServicio documentoServicio, IDocumentoDA documentoDA, IPerfilDA perfilDA)
        {
            _documentoServicio = documentoServicio;
            _documentoDA = documentoDA;
            _perfilDA  = perfilDA;
        }

        public async Task<Guid> Agregar(PerfilRequest perfil)
        {
            var idCurriculum = Guid.NewGuid();
            var idFoto = Guid.NewGuid();
            var curriculum = await _documentoServicio.Agregar(perfil.Curriculum, idCurriculum);
            await _documentoDA.Agregar(perfil.Curriculum,idCurriculum);            
            var foto = await _documentoServicio.Agregar(perfil.Foto, idFoto);
            await _documentoDA.Agregar(perfil.Foto, idFoto);
            var resultado = await _perfilDA.Agregar(new PerfilBD { IdPersona =perfil.IdPersona, Video = perfil.Video, Curriculum = idCurriculum, Foto = idFoto  });
            return resultado;
        }

        public async Task<PerfilRequest> Obtener(Guid Id)
        {
            var perfil = await _perfilDA.Obtener(Id);
            if (perfil == null)
                return null;
            var fotoBytes= await _documentoServicio.Obtener(perfil.Foto);
            var foto=await _documentoDA.ObtenerPorId(perfil.Foto);
            foto.Contenido = fotoBytes;
            var curriculum = await _documentoDA.ObtenerPorId(perfil.Curriculum);            
            var curriculumBytes = await _documentoServicio.Obtener(perfil.Curriculum);
            curriculum.Contenido = curriculumBytes;
            return new PerfilRequest {IdPersona=perfil.IdPersona ,Video=perfil.Video, Curriculum=curriculum, Foto=foto };
        }
    }
}
