"use strict";

$(document).ready(function () {
  $('.teacher-slider').owlCarousel({
    margin: 10,
    loop: true,
    responsiveClass: true,
    dots: true,
    autoplay: false,
    responsive: {
      0: {
        items: 1
      }
    }
  });
});