function loadEditor(id, html) {
    var instance = CKEDITOR.instances[id];

    if (instance) {

        CKEDITOR.remove(instance);
    } else {

        CKEDITOR.replace(id);
        if (html) {
            CKEDITOR.instances[id].setData(html);
        }
    }
    CKEDITOR.config.height = "500";
}
