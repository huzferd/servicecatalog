$(document).ready(function () {
    $("#spinner").bind("ajaxSend", function () {
        $(this).show();
    }).bind("ajaxStop", function () {
        $(this).hide();
    }).bind("ajaxError", function () {
        $(this).hide();
    });
});

$(document).ready(function () {
    $("#delete").click(function () {
        $("#spinner").show();
    });
    $("#save").click(function () {
        $("#spinner").show();
    });
});