"use strict";

$(function () {
  $("a.circle").click(function () {
    // remove classes from all
    $("a.circle").removeClass("active"); // add class to the one we clicked

    $(this).addClass("active");
  });
  $(document).ready(function () {
    $("a.circle[href]").each(function () {
      if (this.href == window.location.href) {
        if ($("a.circle").hasClass("active")) {
          $("a.circle").removeClass("active");
        }

        $(this).addClass("active");
      }
    });
  });
});