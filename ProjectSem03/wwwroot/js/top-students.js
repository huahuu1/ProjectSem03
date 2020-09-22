"use strict";

$(document).ready(function () {
  $('.student-slider').owlCarousel({
    margin: 10,
    loop: true,
    responsiveClass: true,
    dots: true,
    autoplay: false,
    lazyLoad: true,
    slideBy: 2,
    responsive: {
      0: {
        items: 2
      }
    }
  });
});