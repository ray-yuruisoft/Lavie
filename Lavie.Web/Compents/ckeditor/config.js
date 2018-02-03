/**
 * @license Copyright (c) 2003-2014, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
config.language = 'zh-cn';
config.enterMode = CKEDITOR.ENTER_P;
config.width = '96%';
config.toolbar = 'MyToolbar';

config.toolbar_MyBasic =
[
    ['Source'],
    ['Bold','Italic'],
    ['Link','Unlink']
];

config.toolbar_MyToolbar =
[
    ['Source','-','Templates','SelectAll','RemoveFormat','Preview'],
    ['Cut','Copy','Paste','PasteText','PasteFromWord'],
    ['Undo','Redo','-','Find','Replace'],
    ['NumberedList','BulletedList','-','Outdent','Indent','Blockquote'],
    '/',
    ['Bold','Italic','Underline','Strike','-','Subscript','Superscript'],
    ['JustifyLeft','JustifyCenter','JustifyRight','JustifyBlock'],
    ['Link','Unlink','Anchor'],
    ['Image','Flash','Table','HorizontalRule','Smiley','SpecialChar','PageBreak'],
    '/',
    ['Styles','Format','Font','FontSize'],
    ['TextColor','BGColor'],
    ['Maximize', 'ShowBlocks']
];

config.toolbar_MyFile =
[
    ['Source'],
    ['Image', 'Flash']
];

config.removePlugins = 'about';


};
