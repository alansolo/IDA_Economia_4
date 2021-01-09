if (typeof jQuery !== 'undefined') {
    console.log('jQuery Loaded');
}
else {
    console.log('not loaded yet');
} 

//RECUPERAR LA CONTRASEÑA
var app = angular.module("MyApp", []);

app.controller("MyController", function ($scope, $http, $window) {

    var urlPathSystem = "";

    $scope.RecuperarContrasena = function (email) {
        
        $.ajax({
            type: "POST",
            url: urlPathSystem + "/Usuario/RecuperarContrasena",
            data: JSON.stringify(
                {
                    'email': email
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
    };

});