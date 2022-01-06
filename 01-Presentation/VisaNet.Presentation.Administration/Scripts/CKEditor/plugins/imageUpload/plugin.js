var _editor;

(function () {

    var o = {
        exec: function (editor) {
            _editor = editor;
            $("#Content").val(editor.name);
            $("#lbUploadImage").modal("show");
            $("#lbUploadImage").on("shown", function (e) {
                $("#lbUploadImage").on("click", ".btn-success", function (f) {
                    f.preventDefault();
                    $("#formImage").ajaxForm({
                        url: urlUploadImage,
                        dataType: 'json',
                        type: "POST",
                        success: function (url) {
                            editor.insertHtml('<img src="' + url.Content + '"/>');
                            $("#lbUploadImage").modal("hide");
                            hideBlockUI();
                        }
                    });
                    $("#formImage").submit();
                });
            });
        }
    };

    CKEDITOR.plugins.add('imageUpload',
    {
        init: function (editor) {
            var pluginName = 'imageUpload';
            editor.addCommand(pluginName, o);
            editor.ui.addButton('imageUpload',
            {
                label: 'Subir imagen',
                command: pluginName,
                /*icon: CKEDITOR.plugins.getPath(pluginName) + 'docgallery.png'*/
                icon: CKEDITOR.config.baseHref + 'imageUpload' + 'docgallery.png'
            });
        }
    });
})();

