$(function () {
    
    $.ajaxSetup({ cache: false });

    $("#login-modal").on("click", function (e) {
        // hide dropdown if any
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');

        $('#myModal').load(this.href, function () {
            $('#myModal').modal({
                /*backdrop: 'static',*/
                keyboard: true
            }, 'show');
            bindForm(this);
        });
        return false;
    });
});

function bindForm(dialog) {

    $('.login-form', dialog).submit(function () {
        $.ajax({
            url: '@Url.Action("Login", "Home")',
            type: 'POST',

            data: $('.login-form').serialize(),
            success: function (result) {
                if (result.success) {
                    $('#myModal').modal('show');
                    //Refresh
                    location.reload();
                } else {
                    $('#myModal').html(result);
                    //bindForm();
                }
            }
        });
        return false;
    });
}