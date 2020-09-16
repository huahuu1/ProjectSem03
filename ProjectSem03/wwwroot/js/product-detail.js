"use strict";

//PlusMinus
$(function () {
  $("#btnPlus").click(function () {
    $("#txtQuantity").val(Number($("#txtQuantity").val()) + 1);
  });
  $("#btnMinus").click(function () {
    if ($(":text[name='txtQuantity']").val() == 0) return;
    $(":text[name='txtQuantity']").val(Number($(":text[name='txtQuantity']").val()) - 1);
  });
}); // //Carousel
// $('#productOthers').on('slide.bs.carousel', function (e) {
//     var $e = $(e.relatedTarget);
//     var idx = $e.index();
//     var itemsPerSlide = 5;
//     var totalItems = $('.carousel-item').length;
//     if (idx >= totalItems - (itemsPerSlide - 1)) {
//         var it = itemsPerSlide - (totalItems - idx);
//         for (var i = 0; i < it; i++) {
//             //append slides to end
//             if (e.direction == "left") {
//                 $('.carousel-item').eq(i).appendTo('.carousel-inner');
//             }
//             else {
//                 $('.carousel-item').eq(0).appendTo('.carousel-inner');
//             }
//         }
//     }
// });