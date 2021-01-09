//REESTABLECER LA CONTRASEÑA
var app = angular.module("MyApp", []);

app.controller("MyController", function ($scope, $http, $window) {

    const formulario = document.getElementById('formulario');
    const inputs = document.querySelectorAll('#formulario input');

    const expresiones = {
        password: /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,10}$/, // 6 a 10 digitos.
    }

    const campos = {
        password: false,
    }

    const validarFormulario = (e) => {
        switch (e.target.name) {
            case "password":
                validarCampo(expresiones.password, e.target, 'password');
                validarPassword();
                break;
            case "confirmacion":
                validarPassword();
                break;
        }
    }

    const validarCampo = (expresion, input, campo) => {
        if (expresion.test(input.value)) {
            document.getElementById(`group-${campo}`).classList.remove('_form-group-incorrecto');
            document.getElementById(`group-${campo}`).classList.add('_form-group-correcto');
            document.querySelector(`#group-${campo} ._form-info`).classList.remove('_form-info-activo');
            campos[campo] = true;
        } else {
            document.getElementById(`group-${campo}`).classList.add('_form-group-incorrecto');
            document.getElementById(`group-${campo}`).classList.remove('_form-group-correcto');
            document.querySelector(`#group-${campo} ._form-info`).classList.add('_form-info-activo');
            campos[campo] = false;
        }
    }

    const validarPassword = () => {
        const inputPassword1 = document.getElementById('password');
        const inputPassword2 = document.getElementById('confirmacion');

        if (inputPassword1.value !== inputPassword2.value) {
            document.getElementById('group-confirmacion').classList.add('_form-group-incorrecto');
            document.getElementById('group-confirmacion').classList.remove('_form-group-correcto');
            document.querySelector('#group-confirmacion ._form-info').classList.add('_form-info-activo');
            campos['password'] = false;
        } else {
            document.getElementById('group-confirmacion').classList.remove('_form-group-incorrecto');
            document.getElementById('group-confirmacion').classList.add('_form-group-correcto');
            document.querySelector('#group-confirmacion ._form-info').classList.remove('_form-info-activo');
            campos['password'] = true;
        }
    }

    inputs.forEach((input) => {
        input.addEventListener('keyup', validarFormulario);
        input.addEventListener('blur', validarFormulario);
    });



    var urlPathSystem = "";


    $scope.Reestablecer = function (password, confirmacion) {

        if (campos.password) {

            $.ajax({
                type: "POST",
                url: urlPathSystem + "/Usuario/ReestablecerPassword",
                data: JSON.stringify(
                    {
                        'password': password,
                        'confirmacion': confirmacion
                    }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (datos) {

                    //if (datos.IdRol === 4) {
                    //    $window.location.href = urlPathSystem + "/Membresias/Mimascota";
                    //}
                    //else if (datos.IdRol === 5) {
                    //    $window.location.href = urlPathSystem + "/Membresias/Micriadero";
                    //}

                    if (datos !== '') {

                        document.getElementById('success').classList.add('_alert-success-active');
                    }
                    else {
                        document.getElementById('fail').classList.add('_alert-fail-active');;
                    }


                },


            });
        } else {
            document.getElementById('form-warning').classList.add('_form-warning-active');
        }


    };


});