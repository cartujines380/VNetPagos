"use strict";

$(document).ready(function () {
    var requiredElements = $("input[data-val-required]");
    var focusedFirstRequiredElement = false;
    requiredElements.each(function (i) {
        var element = $(requiredElements[i]);
        if (!element.val() && !focusedFirstRequiredElement) {
            focusedFirstRequiredElement = true;
            element.focus();
        }
    });

    $('#nextTab').click(function () {
        $('.nav-tabs > .active').next('li').find('a').trigger('click');
    });

    $('[data-toggle="tooltip"]').tooltip();
});

var showNotification = function(text, type) {
    switch (type) {
        case "success":
            toastr.success(text, { timeOut: 5000 });
            break;
        case "error":
            toastr.error(text, { timeOut: 5000 });
            break;
        case "alert":
            toastr.alert(text, { timeOut: 5000 });
            break;
        case "info":
            toastr.info(text, { timeOut: 5000 });
            break;
    }
};

function validateCardNumber(number) {
    if (number.length != 16) { return false; }
    if (number.charAt(0) != 4) { return false; }

    var length = number.length;
    var value = null;
    var value_cad = null;
    var sum = 0;
    for (var i = 0; i < length; i += 2) {
        value = parseInt(number.charAt(i)) * 2;
        if (value > 9) {
            value_cad = value.toString();
            value = parseInt(value_cad.charAt(0)) +
            parseInt(value_cad.charAt(1));
        }
        sum += value;
    }
    for (var i = 1; i < length; i += 2) {
        sum += parseInt(number.charAt(i));
    }
    if ((sum % 10) == 0) { return true; }
    return false;
}

function showLoaderFullScreen() {
    $.blockUI({
        message: '<div class="sk-fading-circle"><div class="sk-circle1 sk-circle"></div><div class="sk-circle2 sk-circle"></div><div class="sk-circle3 sk-circle"></div><div class="sk-circle4 sk-circle"></div><div class="sk-circle5 sk-circle"></div><div class="sk-circle6 sk-circle"></div><div class="sk-circle7 sk-circle"></div><div class="sk-circle8 sk-circle"></div><div class="sk-circle9 sk-circle"></div><div class="sk-circle10 sk-circle"></div><div class="sk-circle11 sk-circle"></div><div class="sk-circle12 sk-circle"></div></div>'
    });
}