using Datos.Usuario;
using Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Login
{
    public class Login
    {
        public Entidades.Usuario ObtenerUsuario(List<Parametro> listParametro)
        {
            object Resultado = new object();
            Entidades.Usuario Usuario = new Entidades.Usuario();
            const string spName = "ObtenerUsuario";
            BDUsuario bdUsuario = new BDUsuario();
            DataTable dtResultado = new DataTable();

            try
            {
                Resultado = bdUsuario.ObtenerUsuario(spName, listParametro);

                dtResultado = (DataTable)Resultado;

                if (dtResultado.Rows.Count > 0)
                {
                    var jsonUsuario = JsonConvert.DeserializeObject<Entidades.Usuario>(dtResultado.Rows[0][0].ToString());
                    Usuario = jsonUsuario;
                }
            }
            catch (Exception ex)
            {
            }

            return Usuario;
        }
        public List<Entidades.CatPantalla> ObtenerPantalla(List<Parametro> listParametro)
        {
            List<Entidades.CatPantalla> listaPantalla = new List<CatPantalla>();
            object Resultado = new object();
            const string spName = "ObtenerCatPantalla";
            BDUsuario bdUsuario = new BDUsuario();
            DataTable dtResultado = new DataTable();
            StringBuilder sbResultado = new StringBuilder();

            try
            {
                Resultado = bdUsuario.ObtenerUsuario(spName, listParametro);

                dtResultado = (DataTable)Resultado;

                if (dtResultado.Rows.Count > 0)
                {
                    sbResultado.Append("[");

                    dtResultado.Rows.Cast<DataRow>().ToList().ForEach(n =>
                    {
                        sbResultado.Append(n[0].ToString());
                    });

                    sbResultado.Append("]");

                    var jsonCatCapital = JsonConvert.DeserializeObject<CatPantalla[]>(sbResultado.ToString());
                    listaPantalla = jsonCatCapital.ToList();
                }
            }
            catch (Exception ex)
            {
            }

            return listaPantalla;
        }
    }
}
