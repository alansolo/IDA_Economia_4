using Datos.Usuario;
using Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Usuario
{
    public class Rol
    {
        public List<Entidades.CatRol> ObtenerCatRol(List<Parametro> listParametro)
        {
            object Resultado = new object();
            List<Entidades.CatRol> ListCatRol = new List<Entidades.CatRol>();
            const string spName = "ObtenerCatRol";
            BDRol bdRol = new BDRol();
            DataTable dtResultado = new DataTable();
            StringBuilder sbResultado = new StringBuilder();

            try
            {
                Resultado = bdRol.ObtenerCatRol(spName, listParametro);

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

                var jsonListUsuario = JsonConvert.DeserializeObject<Entidades.CatRol[]>(sbResultado.ToString());
                ListCatRol = jsonListUsuario.ToList();
            }
            catch (Exception ex)
            {
            }

            return ListCatRol;
        }
    }
}
