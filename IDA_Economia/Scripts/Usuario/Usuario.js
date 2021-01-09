var app = angular.module("MyApp", []);

document.addEventListener('DOMContentLoaded', function () {
    document.querySelector('main').className += 'loaded';
});

//funcion inicial para agregar las empresas
app.controller("MyController", function ($scope, $http, $window) {

    var urlPathSystem = "";

    //$("#passwordAdd").password('toggle');

    $('#myModalLoader').modal('show');

    $.ajax({
        type: "POST",
        url: urlPathSystem + "/Usuario/ObtenerUsuario",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (datos) {

            $scope.ListUsuario = datos.ListaUsuario;

            $scope.ListCatRol = datos.ListaCatRol;

            $scope.UsuarioAgregar = datos.Usuario;

            $scope.EsSistema = datos.EsSistema;

            $scope.$apply();

            $('#myModalLoader').on('shown.bs.modal', function (e) {
                $("#myModalLoader").modal('hide');
            })

        },
        error: function (error) {

            $('#myModalLoader').on('shown.bs.modal', function (e) {
                $("#myModalLoader").modal('hide');
            })
        }
    });

    $scope.BuscarUsuario = function (usuario, nombre, rol) {

        $('#myModalLoader').modal('show');

        $.ajax({
            type: "POST",
            url: urlPathSystem + "/Usuario/BuscarUsuario",
            data: JSON.stringify(
                {
                    'usuario': usuario,
                    'nombre': nombre,
                    'rol': rol
                }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (datos) {

                if (datos.Mensaje == "OK") {
                    $scope.ListUsuario = datos.ListaUsuario;

                    if(datos.ListaUsuario.length > 0)
                    {
                        MessageSuccess("Buscar Usuario", "Se obtuvieron los resultados correctamente.");
                    }
                    else
                    {
                        MessageInfo("Buscar Usuario", "No se encontro informacion con los filtros seleccionados");
                    }
                }
                else
                {
                    $scope.ListUsuario = null;

                    MessageDanger("Buscar Usuario", "Ocurrio un error inesperado, intente de nuevo o consulte al administrador de sistemas.");
                }

                $scope.$apply();

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })

            },
            error: function (error) {

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })
            }
        });
    }

    $scope.MostrarAgregarUsuario = function () {

        $('#myModalLoader').modal('show');

        $.ajax({
            type: "POST",
            url: urlPathSystem + "/Usuario/MostrarAgregarUsuario",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (datos) {

                $scope.UsuarioAgregar = datos.Usuario;
                $scope.RolAgregar = datos.CatRol;

                $scope.TituloAgregarEditar = "Agregar Usuario";
                $scope.EsGuardar = true;
                $scope.EsEditar = false;

                $scope.$apply();

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })
            },
            error: function (error) {
                
                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })
            }
        });

    }

    $scope.MostrarEditarUsuario = function (usuario) {

        $('#myModalLoader').modal('show');

        $.ajax({
            type: "POST",
            url: urlPathSystem + "/Usuario/MostrarEditarUsuario",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(
            {
                'usuario': usuario
            }),
            success: function (datos) {

                $scope.UsuarioAgregar = datos.Usuario;
                $scope.RolAgregar = datos.CatRol;

                $scope.$apply();

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })

            },
            error: function (error) {
                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })
            }
        });

        //$scope.UsuarioAgregar = usuario;
        //$scope.UsuarioAgregar.ConfirmarPassword = $scope.UsuarioAgregar.Password;

        $scope.TituloAgregarEditar = "Editar Usuario";
        $scope.EsGuardar = false;
        $scope.EsEditar = true;
    }

    $scope.MostrarInactivarUsuario = function (usuario) {

        $scope.UsuarioActivarInactivar = usuario;
        $scope.EsActivar = false;
        $scope.MensajeActivarInactivar = "¿Esta seguro que desea inactivar el usuario?";
        $scope.TituloActivarInactivar = "Inactivar Usuario";
    }

    $scope.MostrarActivarUsuario = function (usuario) {

        $scope.UsuarioActivarInactivar = usuario;
        $scope.EsActivar = true;
        $scope.MensajeActivarInactivar = "¿Esta seguro que desea activar el usuario?";
        $scope.TituloActivarInactivar = "Activar Usuario";
    }

    $scope.AgregarUsuario = function (usuario, catRol) {

        $('#myModalLoader').modal('show');

        $.ajax({
            type: "POST",
            url: urlPathSystem + "/Usuario/AgregarUsuario",
            data: JSON.stringify(
                {
                    'usuario': usuario,
                    'rol': catRol
                }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (datos) {

                if (datos.Mensaje == "OK") {
                    $scope.ListUsuario = datos.ListaUsuario;

                    MessageSuccess("Agregar Usuario", "Se agrego correctamente el usuario.");
                }
                else
                {
                    $scope.ListUsuario = null;

                    MessageDanger("Agregar Usuario", "Ocurrio un error inesperado, intente de nuevo o consulte al administrador de sistemas.");
                }               

                $scope.$apply();

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })

            },
            error: function (error) {

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })
            }
        });

    }

    $scope.EditarUsuario = function (usuario, catRol) {

        $('#myModalLoader').modal('show');

        $.ajax({
            type: "POST",
            url: urlPathSystem + "/Usuario/EditarUsuario",
            data: JSON.stringify(
                {
                    'usuario': usuario,
                    'rol': catRol
                }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (datos) {

                if(datos.Mensaje == "OK")
                {
                    $scope.ListUsuario = datos.ListaUsuario;

                    MessageSuccess("Editar Usuario", "Se edito el usuario de forma correcta.");
                }
                else
                {
                    $scope.ListUsuario = null;

                    MessageDanger("Editar Usuario", "Ocurrio un error inesperado, intente de nuevo o consulte al administrador de sistemas.");
                }             

                $scope.$apply();

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })

            },
            error: function (error) {

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })
            }
        });

    }

    $scope.InactivarUsuario = function (usuario) {

        $('#myModalLoader').modal('show');

        $.ajax({
            type: "POST",
            url: urlPathSystem + "/Usuario/InactivarUsuario",
            data: JSON.stringify(
            {
                'usuario': usuario
            }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (datos) {

                if (datos.Mensaje == "OK")
                {
                    $scope.ListUsuario = datos.ListaUsuario;

                    MessageSuccess("Inactivar Usuario", "Se inactivo correctamente el usuario.");
                }
                else
                {
                    $scope.ListUsuario = null;

                    MessageDanger("Inactivar Usuario", "Ocurrio un error inesperado, intente de nuevo o consulte al administrador de sistemas.");
                }               

                $scope.$apply();

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })

            },
            error: function (error) {

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })
            }
        });

    }

    $scope.ActivarUsuario = function (usuario) {

        $('#myModalLoader').modal('show');

        $.ajax({
            type: "POST",
            url: urlPathSystem + "/Usuario/ActivarUsuario",
            data: JSON.stringify(
            {
                'usuario': usuario
            }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (datos) {

                if (datos.Mensaje == "OK") {
                    $scope.ListUsuario = datos.ListaUsuario;

                    MessageSuccess("Activar Usuario", "Se activo correctamente el usuario.");
                }
                else
                {
                    $scope.ListUsuario = null;

                    MessageDanger("Activar Usuario", "Ocurrio un error inesperado, intente de nuevo o consulte al administrador de sistemas.");
                }            

                $scope.$apply();

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })

            },
            error: function (error) {

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                })
            }
        });

    }

    $scope.ValidarAgregarUsuario = function (Usuario, Rol)
    {
        if (Usuario == null || Usuario.Login == null || Usuario.Login == "" || Usuario.Nombre == null || Usuario.Nombre == "" || Usuario.Password == null || Usuario.Password == "" ||
            Usuario.ConfirmarPassword == null || Usuario.ConfirmarPassword == "" || Rol.Id <= 0 || Usuario.Password != Usuario.ConfirmarPassword)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    function MessageInfo(titulo, message) {
        $.notify({
            // options
            icon: 'fa fa-info-circle fa-lg',
            title: "<span class='title-notify'><strong>" + titulo + "</strong></span><br/>",
            message: "<span class='message-notify'>" + message + "</span><br/>"
        }, {
            // settings
            type: 'info',
            delay: 8000
        });
    }

    function MessageSuccess(titulo, message) {
        $.notify({
            // options
            icon: 'fa fa-check-circle fa-lg',
            title: "<span class='title-notify'><strong>" + titulo + "</strong></span><br/>",
            message: "<span class='message-notify'>" + message + "</span><br/>"
        }, {
            // settings
            type: 'success',
            delay: 8000
        });
    }

    function MessageDanger(titulo, message) {
        $.notify({
            // options
            icon: 'fa fa-window-close fa-lg',
            title: "<span class='title-notify'><strong>" + titulo + "</strong></span><br/>",
            message: "<span class='message-notify'>" + message + "</span><br/>"
        }, {
            // settings
            type: 'danger',
            delay: 8000
        });
    }

});