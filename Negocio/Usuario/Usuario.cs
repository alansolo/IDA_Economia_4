using Datos.Usuario;
using Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace Negocio.Usuario
{
    public class Usuario
    {
        public List<Entidades.Usuario> ObtenerUsuario(List<Parametro> listParametro)
        {
            object Resultado = new object();
            List<Entidades.Usuario> ListUsuario = new List<Entidades.Usuario>();
            const string spName = "ObtenerUsuario";
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
                }

                var jsonListUsuario = JsonConvert.DeserializeObject<Entidades.Usuario[]>(sbResultado.ToString());
                ListUsuario = jsonListUsuario.ToList();

                ListUsuario.ForEach(n =>
                {
                    if (n.Estatus)
                    {
                        n.StrEstatus = "Activo";
                    }
                    else
                    {
                        n.StrEstatus = "Inactivo";
                    }
                });
            }
            catch (Exception ex)
            {
            }

            return ListUsuario;
        }
        public List<Entidades.Usuario> BuscarUsuario(List<Parametro> listParametro)
        {
            object Resultado = new object();
            List<Entidades.Usuario> ListUsuario = new List<Entidades.Usuario>();
            const string spName = "ObtenerUsuario";
            BDUsuario bdUsuario = new BDUsuario();
            DataTable dtResultado = new DataTable();
            StringBuilder sbResultado = new StringBuilder();

            try
            {
                Resultado = bdUsuario.BuscarUsuario(spName, listParametro);

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

                var jsonListUsuario = JsonConvert.DeserializeObject<Entidades.Usuario[]>(sbResultado.ToString());
                ListUsuario = jsonListUsuario.ToList();

                ListUsuario.ForEach(n =>
                {
                    if (n.Estatus)
                    {
                        n.StrEstatus = "Activo";
                    }
                    else
                    {
                        n.StrEstatus = "Inactivo";
                    }
                });
            }
            catch (Exception ex)
            {
            }

            return ListUsuario;
        }
        public Entidades.Usuario AgregarUsuario(List<Parametro> listParametro)
        {
            object Resultado = new object();
            Entidades.Usuario usuario = new Entidades.Usuario();
            const string spName = "InsertUsuario";
            BDUsuario bdUsuario = new BDUsuario();
            DataTable dtResultado = new DataTable();

            try
            {
                Resultado = bdUsuario.InsertUsuario(spName, listParametro);

                dtResultado = (DataTable)Resultado;

                if (dtResultado.Rows.Count > 0)
                {
                    var jsonListUsuario = JsonConvert.DeserializeObject<Entidades.Usuario>(dtResultado.Rows[0][0].ToString());
                    usuario = jsonListUsuario;

                    if (usuario.Estatus)
                    {
                        usuario.StrEstatus = "Activo";
                    }
                    else
                    {
                        usuario.StrEstatus = "Inactivo";
                    }

                    usuario.ConfirmarPassword = usuario.Password;
                }
                
            }
            catch (Exception ex)
            {
            }

            return usuario;
        }

        public Entidades.PerfilUsuario AgregarPerfilUsuario(List<Parametro> listParametro)
        {
            object Resultado = new object();
            Entidades.PerfilUsuario perfilusuario = new Entidades.PerfilUsuario();
            const string spName = "InsertPerfilUsuario";
            BDUsuario bdUsuario = new BDUsuario();
            DataTable dtResultado = new DataTable();
            DataSet dsResultado = new DataSet();

            try
            {
                Resultado = bdUsuario.InsertUsuariodt(spName, listParametro);

                dtResultado = (DataTable)Resultado;

                if (dtResultado.Rows.Count > 0)
                {
                    var jsonListUsuario = JsonConvert.DeserializeObject<Entidades.PerfilUsuario>(dtResultado.Rows[0][0].ToString());
                    perfilusuario = jsonListUsuario;

                    if (perfilusuario.Estatus)
                    {
                        perfilusuario.StrEstatus = "Activo";
                    }
                    else
                    {
                        perfilusuario.StrEstatus = "Inactivo";
                    }

                    perfilusuario.Confirmacion = perfilusuario.Password;
                }

            }
            catch (Exception ex)
            {
            }

            return perfilusuario;
        }
        public Entidades.Usuario EditarUsuario(List<Parametro> listParametro)
        {
            object Resultado = new object();
            Entidades.Usuario usuario = new Entidades.Usuario();
            const string spName = "UpdateUsuario";
            BDUsuario bdUsuario = new BDUsuario();
            DataTable dtResultado = new DataTable();

            try
            {
                Resultado = bdUsuario.EditarUsuario(spName, listParametro);

                dtResultado = (DataTable)Resultado;
                    
                if (dtResultado.Rows.Count > 0)
                {
                    var jsonListUsuario = JsonConvert.DeserializeObject<Entidades.Usuario>(dtResultado.Rows[0][0].ToString());
                    usuario = jsonListUsuario;

                    if (usuario.Estatus)
                    {
                        usuario.StrEstatus = "Activo";
                    }
                    else
                    {
                        usuario.StrEstatus = "Inactivo";
                    }

                    usuario.ConfirmarPassword = usuario.Password;
                }

            }
            catch (Exception ex)
            {
            }

            return usuario;
        }
        public Entidades.Usuario EliminarUsuario(List<Parametro> listParametro)
        {
            object Resultado = new object();
            Entidades.Usuario usuario = new Entidades.Usuario();
            const string spName = "UpdateUsuario";
            BDUsuario bdUsuario = new BDUsuario();
            DataTable dtResultado = new DataTable();

            try
            {
                Resultado = bdUsuario.EliminarUsuario(spName, listParametro);

                if (dtResultado.Rows.Count > 0)
                {
                    var jsonListUsuario = JsonConvert.DeserializeObject<Entidades.Usuario>(dtResultado.Rows[0][0].ToString());
                    usuario = jsonListUsuario;
                }

            }
            catch (Exception ex)
            {
            }

            return usuario;
        }

        public Entidades.PerfilUsuario RecuperarContrasena(List<Parametro> listParametro)
        {
            object Resultado = new object();
            DataSet dsResultado = new DataSet();
            Entidades.PerfilUsuario usuario = new Entidades.PerfilUsuario();
            const string spName = "RecuperarContrasena";
            BDUsuario bdUsuario = new BDUsuario();
            DataTable dtResultado = new DataTable();

            try
            {
                Resultado = bdUsuario.InsertUsuario(spName, listParametro);

                dtResultado = (DataTable)Resultado;

                if (dtResultado.Rows.Count > 0)
                {
                    var jsonListUsuario = JsonConvert.DeserializeObject<Entidades.PerfilUsuario>(dtResultado.Rows[0][0].ToString());
                    usuario = jsonListUsuario;

                    if (usuario.Estatus)
                    {
                        usuario.StrEstatus = "Activo";
                    }
                    else
                    {
                        usuario.StrEstatus = "Inactivo";
                    }

                    usuario.Confirmacion = usuario.Password;
                }

            }
            catch (Exception ex)
            {
            }

            return usuario;
        }

        public Entidades.PerfilUsuario Reestablecer(List<Parametro> listParametro)
        {
            object Resultado = new object();
            DataSet dsResultado = new DataSet();
            Entidades.PerfilUsuario usuario = new Entidades.PerfilUsuario();
            const string spName = "Reestablecer";
            BDUsuario bdUsuario = new BDUsuario();
            DataTable dtResultado = new DataTable();

            try
            {
                Resultado = bdUsuario.InsertUsuario(spName, listParametro);

                dtResultado = (DataTable)Resultado;


                if (dtResultado.Rows.Count > 0)
                {
                    var jsonListUsuario = JsonConvert.DeserializeObject<Entidades.PerfilUsuario>(dtResultado.Rows[0][0].ToString());
                    usuario = jsonListUsuario;

                    if (usuario.Estatus)
                    {
                        usuario.StrEstatus = "Activo";
                    }
                    else
                    {
                        usuario.StrEstatus = "Inactivo";
                    }

                    usuario.Confirmacion = usuario.Password;
                }
                

            }
            catch (Exception ex)
            {
            }

            return usuario;
        }

        public Entidades.Usuario AgregarPagoUsuario(List<Parametro> listParametro)
        {
            object Resultado = new object();
            DataSet dsResultado = new DataSet();
            Entidades.Usuario usuario = new Entidades.Usuario();
            const string spName = "RegistrodePagoUsuario";
            BDUsuario bdUsuario = new BDUsuario();
            DataTable dtResultado = new DataTable();
            StringBuilder sbResultado = new StringBuilder();

            try
            {
                Resultado = bdUsuario.InsertUsuariodt(spName, listParametro);

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

                var jsonListUsuario = JsonConvert.DeserializeObject<Entidades.Usuario>(dtResultado.Rows[0][0].ToString());
                usuario = jsonListUsuario;

                if (usuario.Estatus)
                {
                    usuario.StrEstatus = "Activo";
                }
                else
                {
                    usuario.StrEstatus = "Inactivo";
                }

                usuario.ConfirmarPassword = usuario.Password;
                
            }
            catch (Exception ex)
            {
            }

            return usuario;
        }
    }
}

