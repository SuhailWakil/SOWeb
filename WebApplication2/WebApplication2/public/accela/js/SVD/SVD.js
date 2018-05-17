var MyVue = new Vue({
    el: '#Results',
    data: {
        message: '',
        checked: false,
        isSubmitting: false,
        isSubmitted: false,
        recordId: '',
        tcbmpid: "",
        inspId: "",
        docId: "",
        TCBMPReturnObject: {},
        submitObject: {
            "image": ""
        },
        contacts: [],
        verified: false,
        errMesg: false
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
        },
        ValidateTCBMP: function () {
            var search_button = document.getElementById("search_button");
            search_button.innerHTML = "Verifying...";
            search_button.disabled = true;

            var verifytext = document.getElementById("verifytext");
            
            var dataValue = {
                "tcbmpid": this.tcbmpid,
                "inspId": this.inspId
            };
            $.ajax({
                type: "POST",
                url: "SOWebService.asmx/ValidateTCBMP",
                dataType: "json",
                async: true,
                data: JSON.stringify(dataValue),
                contentType: "application/json; charset=utf-8",
                cache: false,
                success: function (res) {
                    if (res) {
                        res = res.d;
                        if (res.valid == "FOUND") {     
                            MyVue.TCBMPReturnObject = res,
                            MyVue.TCBMPReturnObject.inspId = res.inspId;
                            MyVue.TCBMPReturnObject.token = res.token;
                            MyVue.contacts = res.contacts;
                            MyVue.verified = true;
                            MyVue.errMesg = false;
                        } else {
                            MyVue.TCBMPReturnObject = '',
                            MyVue.contacts = ''
                            MyVue.verified = false;
                            MyVue.errMesg = true;
                            search_button.innerHTML = "Search";
                            search_button.disabled = false;
                        }
                        MyVue.tcbmpid = '';//Clear Field
                        MyVue.inspId = '';//Clear Field
                        search_button.innerHTML = "Search";
                    } else {
                        alert("Please try again at a later time! " + res);
                    }
                },
                error: function (textStatus, errorThrown) {
                    alert("No response from Server!");
                }
            });
        },
        SubmtImage: function () {
            var submitbut = document.getElementById("submitbut");
            submitbut.innerHTML = "Submitting...";
            MyVue.isSubmitting = true;

            var dataValue = {
                "token": MyVue.TCBMPReturnObject.token,
                "image": this.submitObject.image.replace(/^data:image\/(png|jpg|jpeg|gif);base64,/, ""),
                "inspId": MyVue.TCBMPReturnObject.inspId,
                "recordId": MyVue.TCBMPReturnObject.recordId
            };
            $.ajax({
                type: "POST",
                url: "SOWebService.asmx/TCBMPSetDocument",
                dataType: "json",
                async: true,
                data: JSON.stringify(dataValue),
                contentType: "application/json; charset=utf-8",
                cache: false,
                success: function (res) {
                    if (res) {
                        MyVue.docId = res.d;
                        MyVue.isSubmitted = true;
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
        },
        anotherRequestPage: function(){
            window.location.href = 'SVD.aspx';
        },
    }
});

$("#inspid").keyup(function (event) {
    if (event.keyCode == 13) {
        $("#search_button").click();
    }
});