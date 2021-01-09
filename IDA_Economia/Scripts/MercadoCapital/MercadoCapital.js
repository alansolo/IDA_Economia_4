$('.collapse').collapse()

var app = angular.module("MyApp", []);

document.addEventListener('DOMContentLoaded', function () {
    document.querySelector('main').className += 'loaded';
});

//funcion inicial para agregar las empresas
app.controller("MyController", function ($scope, $http, $window) {

    var urlPathSystem = "";

    var myDate = new Date();
    var dd = myDate.getDate();

    var mm = myDate.getMonth() + 1;
    var yyyy = myDate.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }

    if (mm < 10) {
        mm = '0' + mm;
    } 

    myDate = dd + '/' + mm + '/' + yyyy;

    //Inicializar fecha
    var date_input = $(".fecha");
    date_input.datepicker({
        format: 'dd/mm/yyyy',
        todayHighlight: true,
        autoclose: true,
        language: "es",
        setDate: myDate
    });

    $scope.FechaInicio = myDate;
    $scope.FechaFinal = myDate;

    google.charts.load('current', { 'packages': ['line'] });

    $('#myModalLoader').modal('show');

    //$.ajax({
    //    type: "POST",
    //    url: urlPathSystem + "/MercadoCapital/ObtenerMenu",
    //    contentType: 'application/json; charset=utf-8',
    //    dataType: 'json',
    //    success: function (datos) {

    //        $scope.ListPantalla = datos;

    //        $scope.$apply();

    //        $("#myModalLoader").modal('hide');

    //    },
    //    error: function (error) {

    //        $("#myModalLoader").modal('hide');
    //    }
    //});

    ////////////////////////////
    //ESTRUCTURA BASICA DE LA GRAFICA
    var ctx = document.getElementById("myChart");

    var mixedChart = new Chart(ctx, {
        type: 'line',
        data: {
            datasets: [{
                label: 'Bar',
                data: [10, 20, 5, 40],
                borderColor: '#2196f3', // Add custom color border (Line)
                //backgroundColor: '#2196f3',
                type: 'line',
                lineTension: 0
            }, {
                label: 'Line Dataset',
                data: [15, 15, 15, 15],
                borderColor: '#2196f3',
                // Changes this dataset to become a line
                type: 'line'
            }],
            labels: ['01/01/2020', '02/01/2020', '03/01/2020', '04/01/2020']
        },
        options: {
            responsive: true, // Instruct chart js to respond nicely.
            maintainAspectRatio: false, // Add to prevent default behaviour of full-width/height
        }
    });

    //$scope.$apply();


    $.ajax({
        type: "POST",
        url: urlPathSystem + "/MercadoCapital/ObtenerCatCapital",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (datos) {

            if (datos.Mensaje !== "OK") {
                MessageDanger("Mercado de Capitales", "No se pudo cargar la informacion de inicio, intenta de nuevo y si persiste el error contacta al administrador de sistemas.");

                $scope.$apply();

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                });

                return;
            }

            $scope.ListCatCapital = datos.ListaCatCapital;

            $scope.$apply();

            $('#myModalLoader').on('shown.bs.modal', function (e) {
                $("#myModalLoader").modal('hide');
            });
        },
        error: function (error) {

            $('#myModalLoader').on('shown.bs.modal', function (e) {
                $("#myModalLoader").modal('hide');
            });
        }
    });

    $scope.ObtenerEstadistico = function () {
        $('#myModalLoader').modal('show');

        var ListCatCapitalFiltro = $scope.ListCatCapital.filter(function (i, n) {
            return i.Check === true;
        });

        $.ajax({
            type: "POST",
            url: urlPathSystem + "/MercadoCapital/ObtenerEstadistico",
            data: JSON.stringify(
                {
                    'strFechaInicio': $scope.FechaInicio,
                    'strFechaFinal': $scope.FechaFinal,
                    'ListCatCapital': ListCatCapitalFiltro
                }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (datos) {

                if (datos.Mensaje === "Sesion Expirada") {

                    $scope.$apply();

                    $('#myModalLoader').on('shown.bs.modal', function (e) {
                        $("#myModalLoader").modal('hide');
                    });

                    $window.location.href = urlPathSystem + "/Login/Login";
                }
                else if (datos.Mensaje.indexOf("Se debe") !== -1)
                {
                    MessageDanger("Mercado de Capitales", datos.Mensaje);

                    $scope.$apply();

                    $('#myModalLoader').on('shown.bs.modal', function (e) {
                        $("#myModalLoader").modal('hide');
                    });

                    return;
                }
                else if (datos.Mensaje !== "OK")
                {                  
                    MessageDanger("Mercado de Capitales", "No se pudo obtener el calculo de estadisticas, intenta de nuevo con otras acciones y si persiste el error contacta al administrador de sistemas.");

                    $scope.$apply();

                    $('#myModalLoader').on('shown.bs.modal', function (e) {
                        $("#myModalLoader").modal('hide');
                    });

                    return;
                }

                $scope.ListGraficoLinea = datos.ListaCurvaVarianza;

                $scope.ListEncabezadoEmpresa = datos.ListaEncabezadoEmpresa;

                $scope.ListDatos = datos.ListaDatos;

                $scope.ListEmpresa = datos.ListaEmpresa;               

                google.charts.setOnLoadCallback(drawChart);
                google.charts.setOnLoadCallback(drawChart2);

                MessageSuccess("Mercado de Capitales", "Se realizo correctamente el calculo.");

                $scope.$apply();

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                });

            },
            error: function (error) {

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                });
            }
        });
    };

    $scope.ObtenerSoporteResistencia = function () {
        $('#myModalLoader').modal('show');

        var ListCatCapitalFiltro = $scope.ListCatCapital.filter(function (i, n) {
            return i.Check === true;
        });

        $.ajax({
            type: "POST",
            url: urlPathSystem + "/MercadoCapital/ObtenerSoporteResistencia",
            data: JSON.stringify(
                {
                    'ListCatCapital': ListCatCapitalFiltro
                }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (datos) {

                if (datos.Mensaje === "Sesion Expirada") {

                    $scope.$apply();

                    $('#myModalLoader').on('shown.bs.modal', function (e) {
                        $("#myModalLoader").modal('hide');
                    });

                    $window.location.href = urlPathSystem + "/Login/Login";
                }
                else {

                    $scope.Soporte = datos;

                    LlenarGraficaSoporteResistencia();

                    $scope.$apply();
                }

                $scope.$apply();
                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                });

            },
            error: function (error) {

                $('#myModalLoader').on('shown.bs.modal', function (e) {
                    $("#myModalLoader").modal('hide');
                });
            }
        });
    };

    function LlenarGraficaSoporteResistencia() {

        var ctx = document.getElementById("myChart");

        var mixedChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: $scope.Soporte.ListaX
            },
            options: {
                responsive: true, // Instruct chart js to respond nicely.
                maintainAspectRatio: false, // Add to prevent default behaviour of full-width/height
            }
        });

        //mixedChart.data.labels.push($scope.Soporte.ListaX);
        

        $scope.Soporte.ConjuntoListaY.forEach(function (n, index) {
            mixedChart.data.datasets.push({
                label: n.Label,
                data: n.ListaValor,
                borderColor: '#2196f3',
                type: 'line',
                lineTension: 0
            });
        });

        mixedChart.update();

        //var mixedChart = new Chart(ctx, {
        //    type: 'line',
        //    data: {
        //        datasets: [
        //        {
        //            label: 'Bar',
        //            data: [10, 20, 5, 40],
        //            borderColor: '#2196f3', // Add custom color border (Line)
        //            //backgroundColor: '#2196f3',
        //            type: 'line',
        //            lineTension: 0
        //        }, {
        //            label: 'Line Dataset',
        //            data: [15, 15, 15, 15],
        //            borderColor: '#2196f3',
        //            // Changes this dataset to become a line
        //            type: 'line'
        //        }],
        //        labels: $scope.Soporte.ListaX
        //    },
        //    options: {
        //        responsive: true, // Instruct chart js to respond nicely.
        //        maintainAspectRatio: false, // Add to prevent default behaviour of full-width/height
        //    }
        //});
    }

    function addData(chart, label, data) {
        chart.data.labels.push(label);
        chart.data.datasets.forEach((dataset) => {
            dataset.data.push(data);
        });
        chart.update();
    }

    function drawChart() {

        var ListGrafico = $scope.ListGraficoLinea;

        var data = new google.visualization.DataTable();
        data.addColumn('number', 'Pérdida del portafolio');
        data.addColumn('number', 'Ganancia del portafolio');

        //LLENAR INFORMACION
        $.each(ListGrafico, function (key, value) {

            data.addRow([value.Sigma, value.RendimientoAsumido]);
        });

        var options = {
            chart: {
                title: 'Cada punto muestra la perdida y la ganancia de la inversión',
                subtitle: 'Resultados a un dia con una inversión de 10,000 pesos mexicanos'
            },
            width: 850,
            height: 500,
            vAxis: { format: '#,##0.00' },
            hAxis: { format: '#,##0.00' },
            colors: ['#1c91c0']
        };
        
        var chart = new google.charts.Line(document.getElementById('linechart_material'));

        chart.draw(data, google.charts.Line.convertOptions(options));
    }

    function drawChart2() {

        var ListGrafico2 = $scope.ListGraficoLinea;

        var data = new google.visualization.DataTable();
        data.addColumn('number', 'Pérdida del portafolio');
        data.addColumn('number', 'Ganancia del portafolio');

        //LLENAR INFORMACION
        $.each(ListGrafico2, function (key, value) {

            data.addRow([value.Sigma, value.RendimientoAsumido]);
        });

        var options2 = {
            chart: {
                title: 'Cada punto muestra la perdida y la ganancia de la inversión',
                subtitle: 'Resultados a un dia con una inversión de 10,000 pesos mexicanos'
            },
            width: 700,
            height: 500,
            vAxis: { format: '#,##0.00' },
            hAxis: { format: '#,##0.00' },
            colors: ['#1c91c0']
        };

        var chart2 = new google.charts.Line(document.getElementById('linechart_material2'));

        chart2.draw(data, google.charts.Line.convertOptions(options2));
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