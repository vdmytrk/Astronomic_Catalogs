// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function CallStoreProcedureADO() {
    $.ajax({
        url: '/Home/GetDateFromProcedureADO',
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
        url: '/Home/GetDateFromProcedureEF',
        type: 'GET'
    }).done(function (data) {
        alert(data);
    }).fail(function (data) {
        alert("USING Entity Framework, DATA CANNOT BE RETRIEVED. FOR MORE DETAILS, SEE THE COMMENT FOR THE GetDateFromProcedureEF METHOD IN THE DateTables CONTROLLER! ");
        console.log(data);
    });
}