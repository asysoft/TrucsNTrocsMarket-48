$(document).ready(function () {

  //  jQuery.noConflict();

    (function () {



    // Initalize modal dialog
    // attach modal-container bootstrap attributes to links with .modal-link class.
    // when a link is clicked with these attributes, bootstrap will display the href content in a modal dialog.
    $('body').on('click', '.modal-link', function (e) {
        e.preventDefault();
        $(this).attr('data-target', '#myModal');
        $(this).attr('data-toggle', 'modal');
    });

    // Attach listener to .modal-close-btn's so that when the button is pressed the modal dialog disappears
    $('body').on('click', '.modal-close-btn', function () {
        $('#myModal').modal('hide');
    });

    //clear modal cache, so that new content can be loaded
    $('#myModal').on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');
    });

    $('#CancelModal').on('click', function () {
        return false;
    });

    $('.viewInfo').click(function () {


        var url = $(this).attr('href');//$('#signup').data('url');

        $.get(url, function (data) {
            $('#myModal').html(data);

            $('#myModal').modal('show');
        });
    });
});

(function () {
    $('#approve-btn').click(function () {
        $('#myModal').modal('hide');
    });
});

(function () {
    $(".modal-link").click(function () {
        jQuery.noConflict();
        $('#myModal').modal('show');
    });
});


    // Avec le bouton

(function () {
    $('#mySubmit').click(function () {
        $('#myModal').modal('hide');
    });
});

(function () {
    $('#myClose').click(function () {
        $('#myModal').modal('hide');
    });
});



});
