$(function () {
    $.datetimepicker.setDateFormatter('moment');
    $(".datetimepickerForm").datetimepicker({
        format: 'DD/MM/YYYY hh:mm A',
        formatTime: 'hh:mm A'
    });
});