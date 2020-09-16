"use strict";

var Shuffle = window.Shuffle;
var element = document.querySelector('.my-shuffle');
var sizer = element.querySelector('.my-sizer-element');
var myShuffle = new Shuffle(element, {
  itemSelector: '.js-item',
  sizer: sizer,
  buffer: 1
}); // $(document).ready(function() { 
//   myShuffle.filter(function (element) {
//     return element.getAttribute('data-title').length > 10;
//   });
// });