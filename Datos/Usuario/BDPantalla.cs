using Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Usuario
{
    public class BDPantalla:BD
    {
        public object ObtenerCatPantalla(string spName, List<Parametro> listParametro)
        {
            object Resultado = new object();
            List<SqlParameter> listParametrosSQL = new List<SqlParameter>();

            try
            {
                listParametrosSQL = listParametro.Select(n => new SqlParameter
                {
                    ParameterName = "@" + n.Nombre,
                    Value = n.Valor
                }).ToList();

                Resultado = ExecuteScalar(spName, listParametrosSQL);
            }
            catch (Exception ex)
            {
            }

            return Resultado;
        }
    }
}
