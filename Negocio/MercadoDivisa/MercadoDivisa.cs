using Datos.MercadoDivisa;
using Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Negocio.MercadoDivisa
{
    public class MercadoDivisa
    {
        public List<CatDivisa> ObtenerCatDivisa(List<Parametro> listParametro)
        {
            object Resultado = new object();
            const string spName = "ObtenerCatDivisa";
            List<CatDivisa> ListCatDivisa = new List<CatDivisa>();
            BDMercadoDivisa bdMercadoDivisa = new BDMercadoDivisa();
            DataTable dtResultado = new DataTable();
            StringBuilder sbResultado = new StringBuilder();

            try
            {
                Resultado = bdMercadoDivisa.ObtenerCatDivisa(spName, listParametro);

                dtResultado = (DataTable)Resultado;

                if (dtResultado.Rows.Count > 0)
                {
                    sbResultado.Append("[");

                    dtResultado.Rows.Cast<DataRow>().ToList().ForEach(n =>
                    {
                        sbResultado.Append(n[0].ToString());
                    });

                    sbResultado.Append("]");
                }

                var jsonCatDivisa = JsonConvert.DeserializeObject<CatDivisa[]>(sbResultado.ToString());
                ListCatDivisa = jsonCatDivisa.ToList();
            }
            catch (Exception ex)
            {
            }

            return ListCatDivisa;
        }
    }
}
