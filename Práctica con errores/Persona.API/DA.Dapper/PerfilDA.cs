using Abstracciones.DA;
using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DA.Dapper
{
    public class PerfilDA : IPerfilDA
    {
        private IRepositorioDapper _repositorioDapper;
        private SqlConnection _sqlConnection;

        public PerfilDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
            _sqlConnection = _repositorioDapper.ObtenerRepositorioDapper();
        }

        public async Task<Guid> Agregar(PerfilBD perfil)
        {
            string sql = @"AgregarPerfil";
            var resultadoConsulta=await _sqlConnection.ExecuteScalarAsync<Guid>(sql, new {  IdPersona = perfil.IdPersona, Foto = perfil.Foto, Video=perfil.Video, Curriculum=perfil.Curriculum });
            return resultadoConsulta;
        }

        
        public async Task<PerfilBD> Obtener(Guid Id)
        {
            PerfilBD? resultadoConsulta = await ObtenerPerfil(Id);
            if (resultadoConsulta==null)
                return null;
            return resultadoConsulta;
        }

        private async Task<PerfilBD?> ObtenerPerfil(Guid Id)
        {
            string sql = @"ObtenerPerfil";
            var resultadoConsulta = await _sqlConnection.QueryAsync<PerfilBD>(sql, new { IdPersona = Id });
            return resultadoConsulta.FirstOrDefault();
        }
    }
}
