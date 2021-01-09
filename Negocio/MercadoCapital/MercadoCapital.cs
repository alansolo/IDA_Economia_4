using Datos.MercadoCapital;
using Datos.Usuario;
using Entidades;
using Negocio.MercadoCapital;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.MercadoCapital
{
    public class MercadoCapital
    {
        public List<CatCapital> ObtenerCatCapital(List<Parametro> listParametro)
        {
            object Resultado = new object();
            const string spName = "ObtenerCatCapital";
            List<CatCapital> ListCatCapital = new List<CatCapital>();
            BDMercadoCapital bdMercadoCapital = new BDMercadoCapital();
            DataTable dtResultado = new DataTable();
            StringBuilder sbResultado = new StringBuilder();

            try
            {
                Resultado = bdMercadoCapital.ObtenerCatCapital(spName, listParametro);

                dtResultado = (DataTable)Resultado;

                if (dtResultado.Rows.Count > 0)
                {
                    sbResultado.Append("[");

                    dtResultado.Rows.Cast<DataRow>().ToList().ForEach(n =>
                    {
                        sbResultado.Append(n[0].ToString());
                    });

                    sbResultado.Append("]");

                    var jsonCatCapital = JsonConvert.DeserializeObject<CatCapital[]>(sbResultado.ToString());
                    ListCatCapital = jsonCatCapital.ToList();
                }
            }
            catch (Exception ex)
            {
            }

            return ListCatCapital;
        }

        public object ObtenerSoporteResistencia(string query)
        {
            object Resultado = new object();
            BDMercadoCapital bdMercadoCapital = new BDMercadoCapital();

            try
            {
                Resultado = bdMercadoCapital.ObtenerSoporteResistencia(query);
            }
            catch (Exception ex)
            {
            }

            return Resultado;
        }
    }
}
