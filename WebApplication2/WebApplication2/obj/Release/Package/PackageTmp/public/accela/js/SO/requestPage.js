var MyVue = new Vue({
    el: '#Results',
    data: {
        results: [],
        subcatlist: [],
        validatedAddress: false,
        showCityLink: false,
        message: '',
        googleText: '',
        isSubmitting: false,
        isSubmitted: false,
        recordId: '',
        //imageFile: '',
        submitObject: {
            "problemCategory": "",
            "problemSubcategory": "",
            "otherDetails": "",
            "customerComments": "",
            "firstName": "",
            "lastName": "",
            "email": "",
            "phone1": '',
            "address": '',
            "city": '',
            "xcoordinate": 0.0,
            "ycoordinate": 0.0,
            "resolvedAddress": '',
            "refaddressid": '',
            "jurisdiction_code": '',
            "image": ''
        },
        searchObject: {
            "street_number": "",
            "route": "",
            "neighborhood": "",
            "locality": "",
            "postal_code": ""
        },
        rcapt_sig_key: "6LcnaCwUAAAAAO2iw90XL0TaLQFU1BNID6_PTXvY",
        rcapt_id: 0
    },
    methods: {
        onFileChange: function (e) {
            var files = e.target.files || e.dataTransfer.files;
            //this.imageFile = files[0];
            if (!files.length)
                return;
            this.createImage(files[0]);
        },
        createImage: function (file) {
            var image = new Image();
            var reader = new FileReader();
            var vm = this.submitObject;
            reader.onload = function (e) {
                vm.image = e.target.result;
                vm.hovering = false;
            };
            reader.readAsDataURL(file);
        },
        removeImage: function (e) {
            this.submitObject.image = '';
            //this.imageFile ="";
        },
        resetAddressSearch: function () {
            this.validatedAddress = false;
            this.showCityLink = false;
            this.message = '';
            this.submitObject.resolvedAddress = '';
            this.submitObject.refaddressid = '';
            this.submitObject.jurisdiction_code = '';

            this.searchObject.street_number = '';
            this.searchObject.locality = '';
            this.searchObject.route = '';
            this.searchObject.neighborhood = '';
            this.searchObject.postal_code = '';
        },
        //ValidateAddress: function () {
        //    var verifyButton = document.getElementById("verifybutton");
        //    verifyButton.innerHTML = "Verifying Address...";
        //    verifyButton.disabled = true;
        //    var verifytext = document.getElementById("verifytext");

        //    MyVue.submitObject.refaddressid = '';
        //    MyVue.submitObject.resolvedAddress = '';
        //    MyVue.submitObject.jurisdiction_code = '';

        //    MyVue.validatedAddress = false;
        //    var dataValue = {
        //        "street_number": this.searchObject.street_number,
        //        "route": this.searchObject.route,
        //        "neighborhood": this.searchObject.neighborhood,
        //        "locality": this.searchObject.locality,
        //        "postal_code": this.searchObject.postal_code
        //    };
        //    $.ajax({
        //        type: "POST",
        //        url: "SOWebService.asmx/ValidateAddress",
        //        dataType: "json",
        //        async: true,
        //        data: JSON.stringify(dataValue),
        //        contentType: "application/json; charset=utf-8",
        //        cache: false,
        //        success: function (res) {
        //            if (res) {
        //                verifyButton.hidden = true;
        //                res = res.d;
        //                if (res != null && res.length > 0) {
        //                    if (res[0].jurisdiction_code == 'CN') {
        //                        MyVue.submitObject.refaddressid = res[0].addrRefNumber;
        //                        MyVue.submitObject.resolvedAddress = res[0].fullAddress;
        //                        MyVue.submitObject.jurisdiction_code = res[0].jurisdiction_code;
        //                        MyVue.validatedAddress = true;
        //                        verifytext.innerHTML = "Verified County Address";
        //                    } else {
        //                        MyVue.submitObject.refaddressid = 'N/A';
        //                        MyVue.submitObject.resolvedAddress = MyVue.googleText;
        //                        MyVue.submitObject.jurisdiction_code = '';
        //                        MyVue.validatedAddress = false;

        //                        MyVue.showCityLink = true;
        //                        MyVue.message = "Current location is within Incorporated City Limits.";
        //                        verifytext.innerHTML = "Not within County Limits";
        //                    }

        //                } else {
        //                    //alert("Not a County Address")
        //                    MyVue.submitObject.refaddressid = 'N/A';
        //                    MyVue.submitObject.resolvedAddress = MyVue.googleText;
        //                    MyVue.submitObject.jurisdiction_code = '';
        //                    MyVue.validatedAddress = false;
        //                    MyVue.message = "Current location is not a Verified County Address.";
        //                    verifytext.innerHTML = "Not a verified County Address.";
        //                }
        //            } else {
        //                alert("Please try again at a later time! " + res);
        //            }
        //        },
        //        error: function (textStatus, errorThrown) {
        //            alert("No response from Server!");
        //        }
        //    });
        //},
        SubmitRecordForProcessing: function () {
            var submitbut = document.getElementById("submitbut");
            this.$validate(true);
            
            if (this.$testValidator.valid) {
                var g_recaptcha_response = grecaptcha.getResponse(this.rcapt_id);
                if (g_recaptcha_response.length == 0) {
                    alert("Complete Captcha challenge.");
                    return
                } else {
                    MyVue.validateCaptcha(g_recaptcha_response);
                }
            }
        },
        validateCaptcha: function (g_recaptcha_response) {
            submitbut.innerHTML = "Submitting...";
            MyVue.isSubmitting = true;

            $.ajax({
                type: "POST",
                url: "SOWebService.asmx/CaptchaValidate",
                dataType: "json",
                async: true,
                data: JSON.stringify({ "EncodedResponse": g_recaptcha_response }),
                contentType: "application/json; charset=utf-8",
                cache: false,
                success: function (res) {
                    res = res.d;
                    if (res) {
                        //alert("Yes Captcha Verified");
                        MyVue.SubmitValidatedRecordForProcessing();
                    } else {
                        alert("Captcha Not Verified");
                        submitbut.innerHTML = "Submit";
                        MyVue.isSubmitting = false;
                    }
                },
                error: function (textStatus, errorThrown) {
                    alert("No response from Google Server for Validation!");
                }
            });
        },
        SubmitValidatedRecordForProcessing: function () {

            if (this.$testValidator.valid) {
                
                MyVue.results = '';
                var dataValue = {
                    "problemCategory": this.submitObject.problemCategory,
                    "problemSubcategory": this.submitObject.problemSubcategory,
                    "otherDetails": this.submitObject.otherDetails,
                    "customerComments": this.submitObject.customerComments,
                    "firstName": this.submitObject.firstName,
                    "lastName": this.submitObject.lastName,
                    "email": this.submitObject.email,
                    "phone1": this.submitObject.phone1,
                    "address": this.submitObject.address,
                    "city": this.submitObject.city,
                    "xcoordinate": this.submitObject.xcoordinate,
                    "ycoordinate": this.submitObject.ycoordinate,
                    "resolvedAddress": this.submitObject.resolvedAddress,
                    "street_number": this.searchObject.street_number,
                    "route": this.searchObject.route,
                    "locality": this.searchObject.locality,
                    "neighborhood": this.searchObject.neighborhood,
                    "postal_code": this.searchObject.postal_code,
                    "refaddressid": this.submitObject.refaddressid,
                    "image": this.submitObject.image.replace(/^data:image\/(png|jpg|jpeg|gif);base64,/, "")
                };
                $.ajax({
                    type: "POST",
                    url: "SOWebService.asmx/SetDetailsRest",
                    dataType: "json",
                    async: true,
                    data: JSON.stringify(dataValue),
                    contentType: "application/json; charset=utf-8",
                    cache: false,
                    success: function (res) {
                        res = res.d;
                        if (res) {
                            MyVue.recordId = res.AltID;
                            MyVue.isSubmitted = true;

                            $("html, body").animate({
                                scrollTop: 0
                            }, 600);

                            //MyVue.DocumentSubmission(res.RecID, res.token);

                            top.postMessage('Message to the top!', 'http://www.sandiegocounty.gov/content/sdc/dpw/roads/Roads_Service_Request.html');
                            //window.location.href = 'thankyou.aspx';
                            
                        } else {
                            alert("The system timed out, please click Close and Submit your request again.  Thank you");
                            MyVue.isSubmitting = false;
                            submitbut.innerHTML = "Submit Request";
                        }
                    },
                    error: function (textStatus, errorThrown) {
                        alert("No response from Server!");
                        MyVue.isSubmitting = false;
                        submitbut.innerHTML = "Submit Request";
                    }
                });
            }
        },
        //DocumentSubmission: function (RecID, token) {
        //    var form = new FormData();
        //    form.append("uploadedFile", this.imageFile);
        //    form.append("fileInfo", "[{\"serviceProviderCode\": \"LEAMSDPW\",\"fileName\": \"issue.png\",\"type\": \"image/png\",\"description\": \"Reported Image\"}]");

        //    var settings = {
        //        "async": true,
        //        "crossDomain": true,
        //        "url": "https://apis.accela.com/v4/records/"+RecID+"/documents",
        //        "method": "POST",
        //        "headers": {
        //            "authorization": token,
        //            "cache-control": "no-cache"
        //        },
        //        "processData": false,
        //        "contentType": false,
        //        "mimeType": "multipart/form-data",
        //        "data": form
        //    }
        //    setTimeout(function () {
        //        $.ajax(settings).done(function (response) {
        //            console.log(response);
        //        });
        //    }, 1000);
            
        //},
        validateEmail: function (validateEmail) {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            if (reg.test(validateEmail) == false) {
                this.submitObject.email = '';

            }
        },
        validatePhone: function (validatePhone) {
            var reg = /^.{10,14}$/;
            if (reg.test(validatePhone) == false) {
                this.submitObject.phone1 = '';
            }
        },
        validateCity: function (validateCity) {
            var reg = /^.{0,30}$/;
            if (reg.test(validateCity) == false) {
                this.submitObject.city = '';
                
            }
            
        },
        validateAddress: function (validateAddress) {
            var reg = /^.{0,120}$/;
            if (reg.test(validateAddress) == false) {
                this.submitObject.address = '';
                
            }

        },
        setSubCatList: function (category) {
            MyVue.subcatlist = [];
            MyVue.submitObject.problemSubcategory = '';
            if (category == "Road Maintenance") {
                MyVue.subcatlist = ["Glass or Debris in Road"
                                    , "Graffiti Removal"
                                    , "Lifted Sidewalk"
                                    , "Potholes"
                                    , "Raised or Sunken Trench"
                                    , "Rock or Mud Slide"
                                    , "Sweeping Request"
                                    , "Trash/Debris Removal"
                                    , "Other (please specify)"
                ];

            } else if (category == "Drainage") {
                MyVue.subcatlist = [
                     "Berm Repair or Build"
                    , "Culvert/Storm Drain Plugged"
                    , "Debris/Vegetation Channel"
                    , "Flooding"
                    , "Water Flows Down Driveway"
                    , "Other (please specify)"
                ];
            } else if (category == "Traffic Issues") {
                MyVue.subcatlist = [
                     "Limited Visibility"
                    , "Street Lights"
                    , "Other (please specify)"
                ];
            } else if (category == "Signs (Damaged/Missing/Requests)") {
                MyVue.subcatlist = [
                    "Curve Sign"
                    , "Deer/Livestock Sign"
                    , "Intersection Sign"
                    , "Mile Marker Sign"
                    , "Rail Crossing Sign"
                    , "Speed Limit Sign"
                    , "Stop Sign"
                    , "Street Name Sign"
                    , "Yield Sign"
                    , "Other (please specify)"
                ];
            }
        },
        citiWebsite: function () {
                location.href = "http://www.sandiegocounty.gov/content/sdc/dpw/citylinks.html";
        },
        anotherRequestPage: function () {
            window.location.href = 'requestPage.aspx';
        },
        scrollTopParent: function () {
            //window.parent.$("body").animate({ scrollTop: 0 }, 'slow');
            window.parent.parent.scrollTo(0, 0);

        },
        isMobileDevice: function (category) {
            if (navigator.userAgent.toLowerCase().indexOf("android") > -1) {
                window.location.href = 'http://play.google.com/store/apps/details?id=org.CountyofSanDiego.SDEmergency';
            }
            if (navigator.userAgent.toLowerCase().indexOf("iphone") > -1) {
                window.location.href = 'http://itunes.apple.com/us/app/sd-emergency/id561733287?mt=8';
            }
        },
    },
    mounted: function () {
        if (window.grecaptcha) {
            this.rcapt_id = grecaptcha.render($('.g-recaptcha')[0], { sitekey: this.rcapt_sig_key });
        }
    },
    filters: {
        phoneNumber: {
            read: function (phone) {
                return phone;
            },
            write: function (phone) {
                return phone.replace(/[^0-9]/g, '')
                .replace(/(\d{3})(\d{3})(\d{4})/, '($1) $2-$3');
            } 
        }
    }
});

$("#search_address").keyup(function (event) {
    if (event.keyCode == 13) {
        $("#search_button").click();
    }
});

//MyVue.isMobileDevice();
