//getLocation();
//function loadScript(src)
//{
//    var script = document.createElement("script");
//    script.type = "text/javascript";
//    script.src = src;
//    document.body.appendChild(script);
//}

//function getLocation()
//{
//    if (navigator.geolocation)
//    {
//        mapholder=document.getElementById('mapholder');
//        mapholder.innerHTML="Please wait...";	
//        loadScriptMapScript();
//    }
//    else
//    {
//        mapholder=document.getElementById('mapholder');
//        mapholder.innerHTML="Geolocation is not supported by this browser.";
//    }
//}

var map;
var markersArray = [];

mapholder=document.getElementById('mapholder');
if (mapholder) {
    loadScriptMapScript();
}

function loadScriptMapScript()
{
    var script = document.createElement("script");
    script.type = "text/javascript";
    script.src = "https://maps.googleapis.com/maps/api/js?key=AIzaSyARbVz0hVsGdfqgF47iLXTPu5RBM-noMdw&libraries=places&callback=initialize";
    document.body.appendChild(script);
}


function initialize() {
    var input = document.getElementById('search_address');
    var autocomplete = new google.maps.places.Autocomplete(input);

    showPosition();

    //if (navigator.geolocation) {
    //    var pos = navigator.geolocation.getCurrentPosition(showPosition, showError);
    //}
    //else {
    //    mapholder = document.getElementById('mapholder');
    //    mapholder.innerHTML = "Geolocation is not supported by this browser.";
    //}
}

function showPosition()
{
    mapholder=document.getElementById('mapholder');
    lat='33.040831' 
    lon='-116.8689'
    //lat = position.coords.latitude;
    //lon = position.coords.longitude;

    latlon = new google.maps.LatLng(lat, lon);

    mapholder.style.height='400px'; 
    mapholder.style.width='99%';
    mapholder.style.border = "thin solid #888888";
    mapholder.style.borderRadius = "13px";
    mapholder.style.boxShadow = "10px 10px 5px #888888";

    var myOptions={
        center: latlon,
        zoom: 10,
        mapTypeId:google.maps.MapTypeId.ROADMAP,
        mapTypeControl: true,
        zoomControl: true,
        gestureHandling: 'greedy',
        scaleControl: true,
        streetViewControl: true,
        rotateControl: true,
        fullscreenControl: false,
        navigationControlOptions:{style:google.maps.NavigationControlStyle.SMALL}
    };

    map = new google.maps.Map(mapholder, myOptions);


    //var marker = new google.maps.Marker({ position: latlon, map: map, animation: google.maps.Animation.DROP, title: "You are here!" });
    //markersArray.push(marker);

    var geocoder = new google.maps.Geocoder;
    //var result = geocodeLatLng(geocoder, lat, lon);
    // add a click event handler to the map object   

    function LongClick(map, maxTime) {
        this.maxTime = maxTime;
        this.isDragging = false;
        var me = this;
        me.map = map;
        google.maps.event.addListener(map, 'mousedown', function (e) {
            me.onMouseDown_(e);
        });
        google.maps.event.addListener(map, 'mouseup', function (e) {
            me.onMouseUp_(e);
        });
        google.maps.event.addListener(map, 'drag', function (e) {
            me.onMapDrag_(e);
        });
    }
    LongClick.prototype.onMouseUp_ = function (e) {
        var now = +new Date;
        if (now - this.downTime > this.maxTime && this.isDragging === false) {
            google.maps.event.trigger(this.map, 'longpress', e);
        }
    }
    LongClick.prototype.onMouseDown_ = function () {
        this.downTime = +new Date;
        this.isDragging = false;
    }
    LongClick.prototype.onMapDrag_ = function (e) {
        this.isDragging = true;
    };
    new LongClick(map, 150);

    google.maps.event.addListener(map, "longpress", function (event) {
        
        MyVue.resetAddressSearch()
        var latit = shortenString(String(event.latLng.lat()), 9);
        var longit = shortenString(String(event.latLng.lng()), 9);
        var result = geocodeLatLng(geocoder, latit, longit);
        MyVue.submitObject.xcoordinate = latit;
        MyVue.submitObject.ycoordinate = longit;
    });



}

function search() {
    var addressField = document.getElementById('search_address');
    var geocoder = new google.maps.Geocoder();
    geocoder.geocode(
        { 'address': addressField.value },
        function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                var loc = results[0].geometry.location;
                var result = geocodeLatLng(geocoder, loc.lat(), loc.lng());
            }
            else {
                alert("Not found: " + status);
            }
        }
    );
};

function geocodeLatLng(geocoder, latit, longit) {
    var latlng = { lat: parseFloat(latit), lng: parseFloat(longit) };
    geocoder.geocode({ 'location': latlng }, function (results, status) {
        if (status === 'OK') {
            if (results[0]) {
                //Clear Search Object from Address Components
                MyVue.resetAddressSearch()

                var address_components = results[0].address_components;
                var components = {};
                jQuery.each(address_components, function (k, v1) { jQuery.each(v1.types, function (k2, v2) { components[v2] = v1.short_name }); })
                
                //Populate Search Object with Address Components
                if (components.street_number) {
                    //if (components.street_number.indexOf('-') > -1) {
                     //   MyVue.searchObject.street_number = components.street_number.substr(0, components.street_number.indexOf('-')).toUpperCase();
                    //} else {
                        MyVue.searchObject.street_number = components.street_number.toUpperCase();
                    //}  
                }
                if (components.locality) MyVue.searchObject.locality = components.locality.toUpperCase();
                if (components.route) MyVue.searchObject.route = components.route.substring(0, components.route.length - 3).toUpperCase();
                if (components.neighborhood) MyVue.searchObject.neighborhood = components.neighborhood.toUpperCase();
                if (components.postal_code) MyVue.searchObject.postal_code = components.postal_code.toUpperCase();

                MyVue.googleText = results[0].formatted_address;
                //Added after change to Validate functionality
                MyVue.submitObject.resolvedAddress = results[0].formatted_address;
                MyVue.submitObject.xcoordinate = latit;
                MyVue.submitObject.ycoordinate = longit;
                MyVue.validatedAddress = true;
                // place a marker
                placeMarker(latlng, results[0].formatted_address);

            } else {
                //Clear Search Object from Address Components
                MyVue.resetAddressSearch()
                window.alert('No results found');
            }
        } else {
            //Clear Search Object from Address Components
            MyVue.resetAddressSearch()
            window.alert('Geocoder failed due to: ' + status);
        }
    });    
}


function placeMarker(location, add) {
    // first remove all markers if there are any
    deleteOverlays();

    // InfoWindow content
    var content = '<div id="content">' +
        '<p>' + add + '</p>'
        //+'<div class="buttonHolder">' +
        //'<button type="button" id="verifybutton" class="btn-info" onclick="MyVue.ValidateAddress();"> Verify Address</button>' +
        //'<h5><label id="verifytext" class="label label-info"></label></h5>' +
        //'</div>'+
        '</div>';

    var infowindow = new google.maps.InfoWindow({
        content: content
    });
    var marker = new google.maps.Marker({
        position: location, 
        map: map
    });
    infowindow.open(map, marker);

    // add marker in markers array
    markersArray.push(marker);
    //map.setCenter(location);
}

// Deletes all markers in the array by removing references to them
function deleteOverlays() {
    if (markersArray) {
        for (i in markersArray) {
            try{
                markersArray[i].setMap(null);
            }catch(err){
                // catch error.
            }
        }
        markersArray.length = 0;
    }
}

function showError(error)
{
    var maphold=document.getElementById('mapholder');
    switch(error.code) 
    {
        case error.PERMISSION_DENIED:
            maphold.innerHTML="User denied the request for Geolocation."
            break;
        case error.POSITION_UNAVAILABLE:
            maphold.innerHTML="Location information is unavailable."
            break;
        case error.TIMEOUT:
            maphold.innerHTML="The request to get user location timed out."
            break;
        case error.UNKNOWN_ERROR:
            maphold.innerHTML="An unknown error occurred."
            break;
    }
}


function shortenString(str, limit){ 
    if(str.length > limit) str = str.substring(0,limit); 
    return str;
}
