document.getElementById('menuTrigger').addEventListener('click',
    () => {
        document.querySelector('section').classList.add('animate');
        document.getElementById('menu').classList.toggle('_menu-list-in')
    })

document.getElementById('closemenu').addEventListener('click',
    () => {
        document.querySelector('section').classList.toggle('animate');
        document.getElementById('menu').classList.toggle('_menu-list-in')
    })

//VALIDACIÓN DEL FORMULARIO

function sendMessage() {
    var url = '@Url.Action("MandarWhats", "Whatsapp")';
    var nombre = $('#nombre').val();
    var celular = $('#celular').val();
    var mensaje = $('#mensaje').val();

    window.location.href = url + '/?nombre' + nombre + '/?celular' + celular + '/?mensaje' + mensaje;
}

const formulario = document.getElementById('formulario');
const inputs = document.querySelectorAll('#formulario input');

const btn_enviar = document.querySelector("._fin");

const expresiones = {
    usuario: /^[a-zA-Z0-9\_\-]{4,40}$/, // Letras, numeros, guion y guion_bajo
    nombre: /^[a-zA-ZÀ-ÿ\s]{1,20}$/, // Letras y espacios, pueden llevar acentos.
    mensaje: /^[a-zA-ZÀ-ÿ\s]{1,50}$/, // Letras y espacios, pueden llevar acentos.
    celular: /^[0-9]{10}$/, //Exacamente 10 dígitos.
    telefono: /^\d{7,14}$/, // 7 a 14 numeros.
}

const campos = {
    nombre: false,
    celular: false,
    mensaje: false
}

const validarFormulario = (e) => {
    switch (e.target.name) {
        case "nombre":
            validarCampo(expresiones.nombre, e.target, 'nombre');
            break;
        case "celular":
            validarCampo(expresiones.celular, e.target, 'celular');
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

inputs.forEach((input) => {
    input.addEventListener('keyup', validarFormulario);
    input.addEventListener('blur', validarFormulario);
});

btn_enviar.addEventListener('click', (e) => {
    e.preventDefault();

    if (campos.nombre && campos.celular) {


        document.querySelectorAll('_form-group-correcto').forEach((icono) => {
            icono.classList.remove('_form-group-correcto');
        });

    } else {
        document.getElementById('form-warning').classList.add('_form-warning-active');
    }
});

//VALIDACIÓN DEL FORMULARIO