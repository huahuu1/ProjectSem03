"use strict";

$(document).ready(function () {
  $("#backtoTop").click(function () {
    $("body, html").animate({
      scrollTop: '0'
    }, 500);
  });
  var checkPoint,
      currentScrollTop = 0,
      backToTop = $('#backtoTop');
  $(window).scroll(function () {
    var a = $(window).scrollTop();
    currentScrollTop = a;

    if (checkPoint > currentScrollTop) {
      backToTop.css('opacity', 0);
    } else if (checkPoint < currentScrollTop) {
      backToTop.css('opacity', 1);
    }

    checkPoint = currentScrollTop;
  });
});