
var geocoder;
var map;
var mapOptions;
var marker;

function initializeMapCanvas() {
    var latlng;
    var lat = null;
    var long = null;
    var zoomIle;

    if ((document.getElementById('Latitude') != null) && (document.getElementById('Longitude') != null)) {
        lat = parseFloat( document.getElementById('Latitude').value.replace(',' , '.' ) ) ;
        long = parseFloat( document.getElementById('Longitude').value.replace(',', '.'));
    }

    if (lat != null && long != null) {
        latlng = new google.maps.LatLng(lat, long);
        zoomIle = 16;
    }
    else {
        latlng = new google.maps.LatLng(-20.1608912, 57.50122220000003); // Port Louis
        zoomIle = 10;
    }

    mapOptions = {
        center: latlng,
        zoom: zoomIle
    };

    // set Map
    map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

}


function codeAddress() {
    var address = document.getElementById('LocSelectedName').value;

    if (geocoder = "undefined")
        geocoder = new google.maps.Geocoder();

    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {

            if (map != null) {
                map.setCenter(results[0].geometry.location);
                if (marker)
                    marker.setMap(null);
                marker = new google.maps.Marker({
                    map: map,
                    zoom: 16,
                    position: results[0].geometry.location,
                    draggable: true
                });
                google.maps.event.addListener(marker, "dragend", function () {
                    document.getElementById('Latitude').value = marker.getPosition().lat();
                    document.getElementById('Longitude').value = marker.getPosition().lng();
                });
            }
            //
            document.getElementById('Latitude').value = results[0].geometry.location.lat();
            document.getElementById('Longitude').value = results[0].geometry.location.lng();
        } 

    });
}