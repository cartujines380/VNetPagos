/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
    // Define changes to default configuration here. For example: 
    config.language = 'es';
    //config.height = '700'; 
    // config.uiColor = '#AADC6E'; 
    config.extraPlugins = 'imageUpload';
    config.toolbar = 'Full'; 

    config.toolbar_Full = 
    [ 
    { name: 'document', items: ['Source'] }, 
    { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] }, 
    { name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker', 'Scayt'] }, 
    '/',

    { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] }, 

    { name: 'insert', items: ['Image', 'imageUpload', 'Table', 'HorizontalRule', 'SpecialChar'] },

    '/', 
    { name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl'] },

    { name: 'links', items: ['Link', 'Unlink', 'Anchor'] }, 
    '/', 
    { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] }, 
    { name: 'colors', items: ['TextColor', 'BGColor'] }, 
    { name: 'tools', items: ['Maximize', 'ShowBlocks', '-', 'About'] } 
    ];
    config.allowedContent = true;
};