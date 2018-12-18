$(function () {
    var CategID;
    $('#jstreeCategNav').jstree({
        'core': {
            "multiple": false,
            "check_callback": false,
            'data': {
                'url': '/jsTree3/GetJsTree3CategData/',
                "data": function (node) {
                    return {
                        id: CategID
                    };
                },
                'dataType': 'json',
            },
            "themes": {
                "responsive": true,
                "name": "default-dark",
                "variant": 'larg',
                "stripes": false,
                "icons": true,
                "ellipsis": false,
                "dots": false
            }

        },
        "plugins": ['themes', "html_data", "ui", "types"]
    });

    $('#jstreeCategNav').on("changed.jstree", function (e, data) {
         if (data != null && data.selected != null && data.selected.length > 0 && (data.node.children.length == 0)) {
            // on fait l appel au controleur que pour les child de niveau 1
            var CategID = data.selected[0];
             window.location.href = "/Home/SearchByCategID/" + CategID;

        }
    });

});
