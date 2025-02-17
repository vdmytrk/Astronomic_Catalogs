// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function CallStoreProcedureADO() {
    $.ajax({
        url: '/Admin/HomeAdmin/GetDateFromProcedureADO',
        type: 'GET'
    }).done(function (data) {
        alert(data);
    }).fail(function (data) {
        alert("ADO. THERE ARE SOME ISSUES ON THE SERVER! PLEASE CONTACT THE ADMINISTRATION.");
        console.log(data);
    }).always(function () {
        console.log("Request execution completed!");
    });
}

function CallStoreProcedureEF() {
    $.ajax({
        url: '/Admin/HomeAdmin/GetDateFromProcedureEF',
        type: 'GET'
    }).done(function (data) {
        alert(data);
    }).fail(function (data) {
        alert("EF. THERE ARE SOME ISSUES ON THE SERVER! PLEASE CONTACT THE ADMINISTRATION.");
        console.log(data);
    });
}