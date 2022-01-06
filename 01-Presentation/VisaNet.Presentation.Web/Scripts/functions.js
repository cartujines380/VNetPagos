var sDom = "<'row'<'col-xs-6'T><'col-xs-6'f>r>t<'row'<'col-xs-6'i><'col-xs-6'p>>";


function applyDatatableSimple(selector, aoColumns) {
    var x = selector.dataTable({
        "bProcessing": true,
        "oLanguage": TABLES_LOCALE,
        "bPaginate": true,
        "bDestroy": true,
        "bAutoWidth": true,
        "aoColumns": aoColumns
    });
    return x;
}

function applyDatatableSimpleNoPaginate(selector) {
    var x = selector.dataTable({
        //"sAjaxSource": ajaxResource,
        //"bServerSide": true,
        "bProcessing": true,
        "oLanguage": TABLES_LOCALE,
        "bPaginate": false,
        "bDestroy": true,
        "sDom": sDom,
        "bAutoWidth": true,
    });
    return x;
}

function applyDatatableStandard(selector, mColumns, aoColumns, ajaxResource, entityIdValue, aaSorting) {
    if (mColumns == null)
        mColumns = "all";

    if (aaSorting == null)
        aaSorting = [[0, "desc"]];

    oTable = selector.dataTable({
        "sAjaxSource": ajaxResource,
        "fnServerParams": function (aoData) {
            if (entityIdValue != null)
                aoData.push({ "name": "EntityId", "value": entityIdValue });
        },
        "bFilter": true,
        "bServerSide": true,
        "bProcessing": true,
        "bSort": true,
        "aaSorting": aaSorting,
        "oLanguage": TABLES_LOCALE,
        "bPaginate": true,
        "sDom": sDom,
        "sPaginationType": "bootstrap",
        "bDestroy": true,
        "bAutoWidth": true,
        "aoColumns": aoColumns
    });
    oTable.fnSetFilteringDelay();
}

function applyDatatableExternalFilters(selector, mColumns, aoColumns, ajaxResource, fnServerParams) {
    if (mColumns == null)
        mColumns = "all";

    oTable = selector.dataTable({
        "sAjaxSource": ajaxResource,
        "fnServerParams": fnServerParams,
        "bServerSide": true,
        "bProcessing": true,
        "oLanguage": TABLES_LOCALE,
        "bPaginate": true,
        "sDom": sDom,
        "sPaginationType": "bootstrap",
        "bDestroy": true,
        "bAutoWidth": true,
        "aoColumns": aoColumns
    });
    oTable.fnSetFilteringDelay();
}



function applyDatatableExternalFiltersDeferredLoading(selector, mColumns, aoColumns, ajaxResource, fnServerParams) {
    if (mColumns == null)
        mColumns = "all";

    oTable = selector.dataTable({
        "sAjaxSource": ajaxResource,
        "fnServerParams": fnServerParams,
        "bServerSide": true,
        "bProcessing": true,
        "oLanguage": TABLES_LOCALE,
        "bPaginate": true,
        "sDom": sDom,
        "sPaginationType": "bootstrap",
        "bDestroy": true,
        "bAutoWidth": true,
        "aoColumns": aoColumns,
    });
    oTable.fnSetFilteringDelay();
}


//jQuery.validator.setDefaults({
//    highlight: function (element, errorClass, validClass) {
//        if (element.type === 'radio') {
//            this.findByName(element.name).addClass(errorClass).removeClass(validClass);
//        } else {

//            $(element).addClass(errorClass).removeClass(validClass);
//            $(element).closest('.control-group').addClass('error');
//        }
//    },
//    unhighlight: function (element, errorClass, validClass) {
//        if (element.type === 'radio') {
//            this.findByName(element.name).removeClass(errorClass);
//        } else {
//            $(element).removeClass(errorClass).addClass(validClass);
//            $(element).closest('.control-group').removeClass('error');
//        }
//    }
//});

$(document).ready(function () {
    $(".numeric").numeric();
    
    $("span.field-validation-valid, span.field-validation-error").each(function () {
        $(this).addClass("help-inline");
    });

    $("input.date-picker").datepicker({
        format: "dd/mm/yyyy"
    });

    $("input[data-type = filtro]").keyup(function () {
        /* Filter on the column (the index) of this element */

        oTable.fnFilter(this.value, $("input[data-type = filtro]").index(this));
    });

    $("input[data-type = filtro]").each(function (i) {
        asInitVals[i] = this.value;
    });

    $("input[data-type = filtro]").focus(function () {
        if (this.className == "search_init") {
            this.className = "";
            this.value = "";
        }
    });

    $("input[data-type = filtro]").blur(function (i) {
        if (this.value == "") {
            this.className = "search_init";
            this.value = asInitVals[$("input[data-type = filtro]").index(this)];
        }
    });

    $("select[data-type = filtro]").change(function () {
        /* Filter on the column (the index) of this element */

        oTable.fnFilter(this.value, $("[data-type = filtro]").index(this));
    });

    $("[data-type = filtro]").each(function (i) {
        asInitVals[i] = this.value;

    });
});

function preventDisabledLinks() {

    $("body").on("click", 'a[disabled="disabled"]', function (e) {
        e.preventDefault();
    });

}

function ShowNotification(title, text, type) {
    $.pnotify({
        title: title,
        text: text,
        sticker: false,
        type: type,
        history: false,
        styling: "fontawesome",
        hide: true,
        delay: 8000,
        mouse_reset: true,
    });
}

var asInitVals = new Array();

function fill_search_fields() {
    var oSettings = oTable.fnSettings();
    $("[data-type = filtro]").each(function (i) {
        if (oSettings.aoPreSearchCols[i]['sSearch'] != '') {
            $(this).val(oSettings.aoPreSearchCols[i]['sSearch']);
            $(this).removeClass("search_init");
        }
    });
}

function excelExport(action) {
    showLoadingDiv();
    var filters = getSpecificFilters();
    var hdTokenCookieVal = $("#hdTokenCookie").val();
    var formIe = document.createElement("form");
    var $formIe = $(formIe);
    $formIe.attr("id", "fromIe")
             .attr("action", action)
             .attr("method", "post");

    $formIe.append("<input name='hdTokenCookie' type='hidden' id='hdTokenCookie' value='" + hdTokenCookieVal + "'/><input name='hdFiltros' type='hidden' id='hdFiltros' value=''/>");
    $('#hdFiltros', $formIe).val(filters);

    document.body.appendChild(formIe);
    formIe.submit();
    document.body.removeChild(formIe);
}

function getSpecificFilters() {
    var spacificFilters = new Object();
    jQuery.each($("[data-type = filtro]"), function () {
        if ($(this).is(":checkbox")) {
            spacificFilters[$(this).attr("name")] = $(this).is(":checked");
        }
        else if ($(this).is("select")) {
            spacificFilters[$(this).attr("name")] = $(this).val();
        }
        else {
            spacificFilters[$(this).attr("name")] = $(this).val();
        }
    });
    jQuery.each($("[data-type = filtroExcel]"), function () {
        if ($(this).is(":checkbox")) {
            spacificFilters[$(this).attr("name")] = $(this).is(":checked");
        }
        else if ($(this).is("select")) {
            spacificFilters[$(this).attr("name")] = $(this).val();
        }
        else {
            spacificFilters[$(this).attr("name")] = $(this).val();
        }
    });
    spacificFilters["sSearch"] = $(".dataTables_filter input[type=text]").val();
    return JSON.stringify(spacificFilters);
}

var verificarCookie;
function showLoadingDiv() {
    var token = new Date().getTime();
    if ($("#hdTokenCookie").length == 0) {
        alert("PONER EL hdTokenCookie");
    }
    $("#hdTokenCookie").val(token);

    $.blockUI({ message: $('#divBlockUi') });

    verificarCookie = window.setInterval(function () {
        var valorCookie = $.cookie('hdTokenCookie');
        if (valorCookie == token) {
            hideLoadingDiv();
        }
    }, 1000);
}

function hideLoadingDiv() {
    window.clearInterval(verificarCookie);
    $.cookie('hdTokenCookie', null); //clears this cookie value
    $.unblockUI();
}


function showBlockUI() {
    $.blockUI({
        message: $('#divBlockUi')
    });
}

function hideBlockUI() {
    $.unblockUI();
}

function autoComplete(elemento, hiddenID, accion, successFun) {
    $(elemento).autocomplete({
        autoFocus: true,
        source: function (request, response) {
            var url = accion;
            $.ajax({
                url: url,
                type: 'GET',
                data: {
                    maxRows: 15,
                    name_startsWith: request.term
                },

                success: function (data) {
                    successFun(data, response);
                }
            });
        },
        minLength: 3,
        focus: function (event, ui) {
        },

        select: function (event, ui) {
            if (ui.item.valueID != '00000000-0000-0000-0000-000000000000') {
                $(this).val(ui.item.label);
                $(hiddenID).val(ui.item.valueID);
            }
            else {
                $(hiddenID).val("");
                $(this).val("");
            }
            $(hiddenID).change();
            return false;
        },
        open: function () {
            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
        },
        close: function () {
            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
        }
    });
    //Se debe hacer el preventDefault en el mouseUp porque sino no mantiene el texto seleccionado
    $(elemento).mouseup(function (e) { e.preventDefault(); });
    $(elemento).focus(function () { $(this).select(); });
}

function autoCompleteSelectFun(elemento, hiddenID, accion, successFun, selectFun) {
    $(elemento).autocomplete({
        autoFocus: true,
        source: function (request, response) {
            var url = accion;
            $.ajax({
                url: url,
                type: 'POST',
                dataType: "json",
                data: {
                    featureClass: "P",
                    style: "full",
                    maxRows: 15,
                    name_startsWith: request.term
                },

                success: function (data) {
                    successFun(data, response);
                }
            });
        },
        minLength: 1,
        focus: function (event, ui) {
        },

        select: function (event, ui) {
            selectFun(event, ui, hiddenID);
        },
        open: function () {
            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
        },
        close: function () {
            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
        }
    });
    //Se debe hacer el preventDefault en el mouseUp porque sino no mantiene el texto seleccionado
    $(elemento).mouseup(function (e) { e.preventDefault(); });
    $(elemento).focus(function () { $(this).select(); });
}


(function ($) {
    $.widget("custom.combobox", {
        _create: function () {
            this.wrapper = $("<span>")
              .addClass("custom-combobox")
              .insertAfter(this.element);

            this.element.hide();
            this._createAutocomplete();
            this._createShowAllButton();
        },

        _createAutocomplete: function () {
            var selected = this.element.children(":selected"),
                value = selected.val() ? selected.text() : "";

            this.input = $("<input>")
               .appendTo(this.wrapper)
               .val(value)
               .attr("placeholder", "Buscar un servicio")
               .addClass("custom-combobox-input")
               .autocomplete({
                   delay: 0,
                   minLength: 0,
                   source: $.proxy(this, "_source")
               })
               .tooltip({
                   tooltipClass: "custom-state-highlight"
               });

            this._on(this.input, {
                autocompleteselect: function (event, ui) {
                    ui.item.option.selected = true;
                    this._trigger("select", event, {
                        item: ui.item.option
                    });
                    this.element.change();
                },

                autocompletechange: "_removeIfInvalid"
            });
        },

        _createShowAllButton: function () {
            var input = this.input,
              wasOpen = false;

            $("<a>")
              .attr("tabIndex", -1)
              .appendTo(this.wrapper)
              .html("<i class='icon-angle-down'></i><span class='ui-button-icon-primary'></span>")
              .removeClass("ui-corner-all")
              .addClass("ui-button ui-widget ui-state-default ui-button-icon-only custom-combobox-toggle ui-corner-right")
              .mousedown(function () {
                  wasOpen = input.autocomplete("widget").is(":visible");
              })
              .click(function () {
                  input.focus();

                  // Close if already visible
                  if (wasOpen) {
                      return;
                  }

                  // Pass empty string as value to search for, displaying all results
                  input.autocomplete("search", "");
              });
        },

        _source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response(this.element.children("option").map(function () {
                var tags = $(this).attr("data-tags");
                var text = $(this).text();
                if (this.value && (!request.term || matcher.test(text) || matcher.test(tags)))
                    return {
                        label: text,
                        value: text,
                        option: this
                    };
            }));
        },

        _removeIfInvalid: function (event, ui) {

            // Selected an item, nothing to do
            if (ui.item) { return; }

            // Search for a match (case-insensitive)
            var value = this.input.val(),
              valueLowerCase = value.toLowerCase(),
              valid = false;
            this.element.children("option").each(function () {
                if ($(this).text().toLowerCase() === valueLowerCase) {
                    this.selected = valid = true;
                    return false;
                }
            });

            // Found a match, nothing to do
            if (valid) { return; }

            // Remove invalid value
            this.input.val("");
            this.element.val("");
            this.input.data("ui-autocomplete").term = "";
        },

        _destroy: function () {
            this.wrapper.remove();
            this.element.show();
        }
    });
})(jQuery);

function showLoaderFullScreen(message) {
    $("#loaderMessage").html(message);
    $("#loaderFullScreen").show();
}

function hideLoaderFullScreen() { $("#loaderFullScreen").hide(); }

function removeStateGeneralErrorForm() {
    if (!$("#generalError").hasClass("hide")) {
        $("#generalError").addClass("hide");
    }
}
function changeStateGeneralErrorForm() {
    if ($("#generalError").hasClass("hide")) {
        $("#generalError").removeClass("hide");
    } else {
        $("#generalError").addClass("hide");
    }
}
function CardNotSelected() {
    if ($("#cardNotSelected").hasClass("hide")) {
        $("#cardNotSelected").removeClass("hide");
    } else {
        $("#cardNotSelected").addClass("hide");
    }
}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function validateCardCVV(number) {
    if (number.length >= 3 && number.length <= 4) {
        return true;
    }
    return false;
}

/* NAVBAR SCROLL EFFECT */

// Hide Header on on scroll down
var didScroll;
var lastScrollTop = 0;
var delta = 5;
var navbarHeight = $('.navbar').outerHeight();

$(window).scroll(function (event) {
    didScroll = true;
});

setInterval(function () {
    if (didScroll) {
        hasScrolled();
        didScroll = false;
    }
}, 250);

function hasScrolled() {
    var st = $(this).scrollTop();

    // Make sure they scroll more than delta
    if (Math.abs(lastScrollTop - st) <= delta)
        return;

    // If they scrolled down and are past the navbar, add class .nav-up.
    // This is necessary so you never see what is "behind" the navbar.
    if (st > lastScrollTop && st > navbarHeight) {
        // Scroll Down
        $('.navigationBar').removeClass('nav-down').addClass('nav-up');
    } else {
        // Scroll Up
        if (st + $(window).height() < $(document).height()) {
            $('.navigationBar').removeClass('nav-up').addClass('nav-down');
        }
    }

    lastScrollTop = st;
}