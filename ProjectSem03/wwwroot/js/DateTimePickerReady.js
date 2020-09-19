$(function () {
    $.datetimepicker.setDateFormatter('moment');
    $(".datetimepicker").datetimepicker({
        format: 'DD/MM/YYYY hh:mm A',
        formatTime: 'hh:mm A'
    });
});