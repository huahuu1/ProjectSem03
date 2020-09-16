"use strict";

$(document).ready(function () {
  $('.slider-main').owlCarousel({
    margin: 10,
    loop: true,
    responsiveClass: true,
    dots: true,
    autoplay: true,
    lazyLoad: true,
    nav: true,
    navText: ["<i class='fa fa-chevron-left prev-next-button'></i>", "<i class='fa fa-chevron-right prev-next-button'></i>"],
    responsive: {
      0: {
        items: 1
      }
    }
  });
});