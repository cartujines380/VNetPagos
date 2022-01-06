var sDom = "<'row'<'col-xs-6'T><'col-xs-6'>r>t<'row'<'col-xs-6'i><'col-xs-6'p>>";


function applyDatatableSimple(selector, aoColumns) {
    //$('table').block({ message: "Procesando..." });
    showBlockUI('Procesando...');
    var x = selector.dataTable({
        //"bProcessing": true,
        "oLanguage": TABLES_LOCALE,
        "bPaginate": true,
        "bDestroy": true,
        "bAutoWidth": true,
        "aoColumns": aoColumns
    });

    oTable.on('processing.dt', function (e, settings, processing) {
        if (processing)
            showBlockUI('Procesando...');
        else
            hideBlockUI();
    }).on('preDraw', function () {
        showBlockUI('Procesando...');
    });

    return x;
}

function applyDatatableSimpleNoPaginate(selector) {
    showBlockUI('Procesando...');
    var x = selector.dataTable({
        "bFilter": false,
        "bSort": false,
        "bProcessing": false,
        "oLanguage": TABLES_LOCALE,
        "bPaginate": false,
        "bDestroy": true,
        "sDom": sDom,
        "sPaginationType": "bootstrap",
        "bAutoWidth": false,
        "sScrollY": "500px",

    });

    oTable.on('processing.dt', function (e, settings, processing) {
        if (processing)
            showBlockUI('Procesando...');
        else
            hideBlockUI();
    }).on('preDraw', function () {
        showBlockUI('Procesando...');
    });

    return x;
}

function applyDatatableStandard(selector, mColumns, aoColumns, ajaxResource, entityIdValue, aaSorting) {
    if (mColumns == null)
        mColumns = "all";

    if (aaSorting == null)
        aaSorting = [[0, "desc"]];

    showBlockUI('Procesando...');

    oTable = selector.dataTable({
        "sAjaxSource": ajaxResource,
        "fnServerParams": function (aoData) {
            if (entityIdValue != null)
                aoData.push({ "name": "EntityId", "value": entityIdValue });
        },
        //"bFilter": true,
        "bServerSide": true,
        //"bProcessing": true,
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


    oTable.on('processing.dt', function (e, settings, processing) {
        if (processing)
            showBlockUI('Procesando...');
        else
            hideBlockUI();
    }).on('preDraw', function () {
        showBlockUI('Procesando...');
    });
    //oTable.fnSetFilteringDelay();
    //fill_search_fields();

    return oTable;
}

function fill_search_fields() {
    var oSettings = oTable.fnSettings();
    $("[data-type = filtro]").each(function (i) {
        if (oSettings.aoPreSearchCols[i]['sSearch'] != '') {
            $(this).val(oSettings.aoPreSearchCols[i]['sSearch']);
            $(this).removeClass("search_init");
        }
    });
}

function applyDatatableNoFilters(selector, mColumns, aoColumns, ajaxResource, entityIdValue, aaSorting) {
    if (mColumns == null)
        mColumns = "all";

    if (aaSorting == null)
        aaSorting = [[0, "desc"]];

    showBlockUI('Procesando...');

    oTable = selector.dataTable({
        "sAjaxSource": ajaxResource,
        "fnServerParams": function (aoData) {
            if (entityIdValue != null)
                aoData.push({ "name": "EntityId", "value": entityIdValue });
        },
        "bFilter": false,
        "bServerSide": true,
        //"bProcessing": true,
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

    oTable.on('processing.dt', function (e, settings, processing) {
        if (processing)
            showBlockUI('Procesando...');
        else
            hideBlockUI();
    }).on('preDraw', function () {
        showBlockUI('Procesando...');
    });
    //oTable.fnSetFilteringDelay();
    fill_search_fields();
}

function fill_search_fields() {
    var oSettings = oTable.fnSettings();
    $("[data-type = filtro]").each(function (i) {
        if (oSettings.aoPreSearchCols[i]['sSearch'] != '') {
            $(this).val(oSettings.aoPreSearchCols[i]['sSearch']);
            $(this).removeClass("search_init");
        }
    });
}

function applyDatatableExternalFilters(selector, mColumns, aoColumns, ajaxResource, fnServerParams, bFilter, aaSorting) {
    if (mColumns == null)
        mColumns = "all";

    if (bFilter == null)
        bFilter = true;

    if (aaSorting == null)
        aaSorting = [[0, "desc"]];

    showBlockUI('Procesando...');

    oTable = selector.dataTable({
        "sAjaxSource": ajaxResource,
        "fnServerParams": fnServerParams,
        "bServerSide": true,
        //"bProcessing": true,
        "oLanguage": TABLES_LOCALE,
        "bPaginate": true,
        //"bFilter":bFilter,
        "sDom": sDom,
        "sPaginationType": "bootstrap",
        "bDestroy": true,
        "bAutoWidth": true,
        "aoColumns": aoColumns,
        "aaSorting": aaSorting        
        //"aLengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
    });   

    oTable.on('processing.dt', function (e, settings, processing) {
        if (processing)
            showBlockUI('Procesando...');
        else
            hideBlockUI();
    }).on('preDraw', function () {
        showBlockUI('Procesando...');
    });    

    oTable.fnSetFilteringDelay();
    return oTable;
}

function applyDatatableExternalFiltersDisplay(selector, mColumns, aoColumns, ajaxResource, fnServerParams, bFilter, aaSorting, iDisplayLength) {
    if (mColumns == null)
        mColumns = "all";

    if (bFilter == null)
        bFilter = true;

    if (aaSorting == null)
        aaSorting = [[0, "desc"]];

    showBlockUI('Procesando...');

    oTable = selector.dataTable({
        "sAjaxSource": ajaxResource,
        "fnServerParams": fnServerParams,
        "bServerSide": true,
        //"bProcessing": true,
        "oLanguage": TABLES_LOCALE,
        "bPaginate": true,
        //"bFilter":bFilter,
        "sDom": sDom,
        "sPaginationType": "bootstrap",
        "bDestroy": true,
        "bAutoWidth": true,
        "aoColumns": aoColumns,
        "aaSorting": aaSorting,
        "iDisplayLength": iDisplayLength,
        //"aLengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
    });

    oTable.on('processing.dt', function (e, settings, processing) {
        if (processing)
            showBlockUI('Procesando...');
        else
            hideBlockUI();
    }).on('preDraw', function () {
        showBlockUI('Procesando...');
    });

    oTable.fnSetFilteringDelay();
    return oTable;
}


function applyDatatableExternalFiltersDeferredLoading(selector, mColumns, aoColumns, ajaxResource, fnServerParams) {
    if (mColumns == null)
        mColumns = "all";

    showBlockUI('Procesando...');

    oTable = selector.dataTable({
        "sAjaxSource": ajaxResource,
        "fnServerParams": fnServerParams,
        "bServerSide": true,
        //"bProcessing": true,
        "oLanguage": TABLES_LOCALE,
        "bPaginate": true,
        "sDom": sDom,
        "sPaginationType": "bootstrap",
        "bDestroy": true,
        "bAutoWidth": true,
        "aoColumns": aoColumns,
    });

    oTable.on('processing.dt', function (e, settings, processing) {
        if (processing)
            showBlockUI('Procesando...');
        else
            hideBlockUI();
    }).on('preDraw', function () {
        showBlockUI('Procesando...');
    });

    oTable.fnSetFilteringDelay();
}


$(document).ready(function () {
    $(".numeric").numeric();
    $("a.active").parent().addClass("active");

    $("span.field-validation-valid, span.field-validation-error").each(function () {
        $(this).addClass("help-inline");
    });

    $("input.date-picker").datepicker({
        format: "dd/mm/yyyy"
    });

    $('input.date-picker').on('changeDate', function (ev) {
        $(this).datepicker('hide');
    });
    $('input.date-picker').on('click', function (ev) {
        $(this).datepicker('show');
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
        hide: false,
        sticker: false,
        type: type,
        history: false,
        styling: "fontawesome",
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


function showBlockUI(message) {
    $.blockUI({
        message: message ? message : $('#divBlockUi'),
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '20px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
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

function showLoaderFullScreen(message) {
    $("#loaderMessage").html(message);
    $("#loaderFullScreen").show();
}

function hideLoaderFullScreen() { $("#loaderFullScreen").hide(); }