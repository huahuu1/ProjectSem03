"use strict";

$(document).ready(function () {
  $(".tabs .tab-links a").on('click', function (e) {
    var currentAttrValue = $(this).attr('href'); //show/hide tabs
    // $('.tabs ' + currentAttrValue).show().siblings().hide();
    // $('.tabs' + currentAttrValue).siblings().slideUp(400);
    // $('.tabs' + currentAttrValue).delay(400).slideDown(400);

    $('.tabs ' + currentAttrValue).fadeIn(400).siblings().hide(); // $('.tabs ' + currentAttrValue).slideDown(400).siblings().slideUp(400);
    //change/remove current tab to active

    $(this).parent('li').addClass('active').siblings().removeClass('active');
    e.preventDefault();
  });
});