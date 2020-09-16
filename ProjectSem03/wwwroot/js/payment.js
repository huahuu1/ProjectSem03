"use strict";

$('.btnShow').click(function () {
  if ($(this).parent().next().hasClass('d-none')) {
    $(this).parent().next().removeClass('d-none');
  } else {
    $(this).parent().next().addClass('d-none');
  }
});