
//EVENTOS DEL FORMULARIO POR ETAPAS
const movPag = document.querySelector("._movPag");
const btn_adelante2 = document.querySelector("._sigPag");

const btn_atras1 = document.querySelector("._volver-pag1");
const btn_adelante3 = document.querySelector("._adelante-pag3");
const btn_atras2 = document.querySelector("._volver-pag2");
const btn_final = document.querySelector("._fin");

const progressText = document.querySelectorAll("._step p");
const progressCheck = document.querySelectorAll("._step ._check");
const num = document.querySelectorAll("._step ._num");

let max = 3;
let cont = 1;

btn_adelante2.addEventListener("click", function (e) {
    e.preventDefault();

    movPag.style.marginLeft = "-33.33%";
    num[cont - 1].classList.add("_active");
    progressCheck[cont - 1].classList.add("_active");
    progressText[cont - 1].classList.add("_active");
    cont += 1;
});

btn_adelante3.addEventListener("click", function (e) {
    e.preventDefault();

    movPag.style.marginLeft = "-66.66%";
    num[cont - 1].classList.add("_active");
    progressCheck[cont - 1].classList.add("_active");
    progressText[cont - 1].classList.add("_active");
    cont += 1;
});

//btn_final.addEventListener("click", function (e) {
//    e.preventDefault();

//    movPag.style.marginLeft = "-99.99%";
//    num[cont - 1].classList.add("_active");
//    progressCheck[cont - 1].classList.add("_active");
//    progressText[cont - 1].classList.add("_active");
//    cont += 1;
//    alert("Aquí finaliza el registro.");
//});

btn_atras1.addEventListener("click", function (e) {
    e.preventDefault();

    movPag.style.marginLeft = "0%";
    num[cont - 2].classList.remove("_active");
    progressCheck[cont - 2].classList.remove("_active");
    progressText[cont - 2].classList.remove("_active");
    cont -= 1;
});

btn_atras2.addEventListener("click", function (e) {
    e.preventDefault();

    movPag.style.marginLeft = "-33.33%";
    num[cont - 2].classList.remove("_active");
    progressCheck[cont - 2].classList.remove("_active");
    progressText[cont - 2].classList.remove("_active");
    cont -= 1;
});
//EVENTOS DEL FORMULARIO POR ETAPAS

//VALIDACIÓN DEL FORMULARIO

const formulario = document.getElementById('formulario');
const inputs = document.querySelectorAll('#formulario input');

const expresiones = {
    usuario: /^[a-zA-Z0-9\_\-]{4,40}$/, // Letras, numeros, guion y guion_bajo
    nombre: /^[a-zA-ZÀ-ÿ\s]{1,40}$/, // Letras y espacios, pueden llevar acentos.
    password: /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,10}$/, // 4 a 12 digitos.
    correo: /^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$/,
    telefono: /^\d{7,14}$/, // 7 a 14 numeros.
}

const campos = {
    nombre: false,
    correo: false,
    password: false,
    estudios: false,
    perfil: false,
    ocupacion: false,
    experiencia: false,
    marketing: false
}

const validarFormulario = (e) => {
    switch (e.target.name) {
        case "nombre":
            validarCampo(expresiones.usuario, e.target, 'nombre');
            break;
        case "correo":
            validarCampo(expresiones.correo, e.target, 'correo');

            break;
        case "password":
            validarCampo(expresiones.password, e.target, 'password');
            validarPassword();
            break;
        case "confirmacion":
            validarPassword();

            break;
        case "estudios":
            validarCampo(expresiones.nombre, e.target, 'estudios');
            break;
        case "perfil":
            validarCampo(expresiones.nombre, e.target, 'perfil');
            break;
        case "ocupacion":
            validarCampo(expresiones.nombre, e.target, 'ocupacion');
            break;
        case "experiencia":
            validarCampo(expresiones.nombre, e.target, 'experiencia');
            break;
        case "marketing":
            validarCampo(expresiones.nombre, e.target, 'marketing');
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

btn_final.addEventListener('click', (e) => {
    e.preventDefault();

    const terminos = document.getElementById('terminos');
    if (campos.nombre && campos.correo && campos.password && campos.estudios && campos.perfil
        && campos.ocupacion && campos.experiencia && campos.marketing && terminos.checked) {
        campos.reset();

        document.querySelectorAll('_form-group-correcto').forEach((icono) => {
            icono.classList.remove('_form-group-correcto');
        });
    } else {
        document.getElementById('form-warning').classList.add('_form-warning-active');
    }
});

//VALIDACIÓN DEL FORMULARIO


//ENVIAR REGISTRO A LA BASE DE DATOS
var app = angular.module("MyApp", []);
app.controller("MyController", function ($scope, $http, $window) {
    var urlPathSystem = "";

    $scope.CrearPerfilUsuario = function (perfilusuario) {

        $('#myModalLoader').modal('show');

        $.ajax({
            type: "POST",
            url: urlPathSystem + "/Usuario/AgregarPerfilUsuario",
            data: JSON.stringify(
                {
                    'perfilusuario': perfilusuario,
                }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (datos) {

                if (datos.Mensaje == "OK") {
                    $scope.ListUsuario = datos.ListaUsuario;

                    MessageSuccess("Agregar Usuario", "Se agrego correctamente el usuario, verifique su bandeja de entrada o correo no deseado para activar la cuenta.");

                    $scope.$apply();
                    
                }
                else {
                    $scope.ListUsuario = null;

                    MessageDanger("Agregar Usuario", "Ocurrio un error inesperado, intente de nuevo o consulte al administrador de sistemas.");

                    $scope.$apply();
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

//ENVIAR REGISTRO A LA BASE DE DATOS