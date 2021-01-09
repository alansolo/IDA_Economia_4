using Datos.MercadoDinero;
using Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.MercadoDinero
{
    public class MercadoDinero
    {
        public List<CatDinero> ObtenerCatDinero(List<Parametro> listParametro)
        {
            object Resultado = new object();
            const string spName = "ObtenerCatDinero";
            List<CatDinero> ListCatDinero = new List<CatDinero>();
            BDMercadoDinero bdMercadoDivisa = new BDMercadoDinero();
            DataTable dtResultado = new DataTable();
            StringBuilder sbResultado = new StringBuilder();

            try
            {
                Resultado = bdMercadoDivisa.ObtenerCatDinero(spName, listParametro);

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

                var jsonCatDinero = JsonConvert.DeserializeObject<CatDinero[]>(sbResultado.ToString());
                ListCatDinero = jsonCatDinero.ToList();
            }
            catch (Exception ex)
            {
            }

            return ListCatDinero;
        }
    }
}
