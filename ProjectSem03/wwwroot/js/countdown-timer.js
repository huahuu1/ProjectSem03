"use strict";
$("#countdown").ClassyCountdown({
    end: parseInt(new Date(endDate) / 1000),
    now: parseInt($.now() / 1000),
    labels: true,
    style: {
        element: "",
        textResponsive: .5,
        days: {
            gauge: {
                thickness: .03,
                color: "#FC6",
                bgColor: "rgba(0,0,0,0.2)",
                fgColor: "#ee5b54"
            },
            textCSS: 'font-family:\'Montserrat\'; font-size:25px; font-weight:600; color:#181818;'
        },
        hours: {
            gauge: {
                thickness: .03,
                bgColor: "rgba(0,0,0,0.2)",
                fgColor: "#ee5b54"
            },
            textCSS: 'font-family:\'Montserrat\'; font-size:25px; font-weight:600; color:#181818;'
        },
        minutes: {
            gauge: {
                thickness: .03,
                bgColor: "rgba(0,0,0,0.2)",
                fgColor: "#ee5b54"
            },
            textCSS: 'font-family:\'Montserrat\'; font-size:25px; font-weight:600; color:#181818;'
        },
        seconds: {
            gauge: {
                thickness: .03,
                bgColor: "rgba(0,0,0,0.2)",
                fgColor: "#ee5b54"
            },
            textCSS: 'font-family:\'Montserrat\'; font-size:25px; font-weight:600; color:#181818;'
        }
    },
    onEndCallback: function onEndCallback() {
        console.log("Time out!");
    }
});
//$('#countdown2').ClassyCountdown({
//  end: parseInt(new Date("2020-09-30") / 1000),
//  now: parseInt(new Date("2020-09-07") / 1000),
//  labels: true,
//  style: {
//    element: "",
//    textResponsive: .5,
//    days: {
//      gauge: {
//        thickness: .03,
//        color: "#FC6",
//        bgColor: "rgba(0,0,0,0.2)",
//        fgColor: "#ee5b54"
//      },
//      textCSS: 'font-family:\'Montserrat\'; font-size:25px; font-weight:600; color:#181818;'
//    },
//    hours: {
//      gauge: {
//        thickness: .03,
//        bgColor: "rgba(0,0,0,0.2)",
//        fgColor: "#ee5b54"
//      },
//      textCSS: 'font-family:\'Montserrat\'; font-size:25px; font-weight:600; color:#181818;'
//    },
//    minutes: {
//      gauge: {
//        thickness: .03,
//        bgColor: "rgba(0,0,0,0.2)",
//        fgColor: "#ee5b54"
//      },
//      textCSS: 'font-family:\'Montserrat\'; font-size:25px; font-weight:600; color:#181818;'
//    },
//    seconds: {
//      gauge: {
//        thickness: .03,
//        bgColor: "rgba(0,0,0,0.2)",
//        fgColor: "#ee5b54"
//      },
//      textCSS: 'font-family:\'Montserrat\'; font-size:25px; font-weight:600; color:#181818;'
//    }
//  },
//  onEndCallback: function onEndCallback() {
//    console.log("Time out!");
//  }
//});