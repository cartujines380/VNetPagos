$(document).ready( function(){
	$('#nextTab').click(function(){
	  $('.nav-tabs > .active').next('li').find('a').trigger('click');
	});

	$(function () {
  	$('[data-toggle="tooltip"]').tooltip()
	})
});
