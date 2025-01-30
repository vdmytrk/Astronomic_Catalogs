// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function CallStoreProcedureADO() {
    $.ajax({
        url: '/ActualDates/GetDateFromProcedureADO',
        type: 'GET'
    }).done(function (data) {
        alert(data);
    }).fail(function (data) {
        alert("ADO. НА СЕРВЕРІ ЯКІСЬ ПРОБЛЕМИ! :( ");
        console.log(data);
    }).always(function () {
        console.log("Виконання запиту завершено!");
    });
}

function CallStoreProcedureEF() {
    $.ajax({
        url: '/ActualDates/GetDateFromProcedureEF',
        type: 'GET'
    }).done(function (data) {
        alert(data);
    }).fail(function (data) {
        alert("ЗА ДОПОМОГОЮ Entity Framework ДАНІ НЕ ВИТЯГНЕШ. ДЕТАЛЬНІШЕ ДИВИСЯ КОМЕНТАР ДО МЕТОДУ GetDateFromProcedureEF КОНТРОЛЛЕРА DateTables! ");
        console.log(data);
    });
}