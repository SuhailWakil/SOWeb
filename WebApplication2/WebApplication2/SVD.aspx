<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SVD.aspx.cs" Inherits="SOWebService.SVD" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="">
    <title>TCBMP Verification</title>
    
    <link href="public/accela/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="public/accela/css/1-col-portfolio.css" rel="stylesheet" type="text/css" />
    
    
    <link href="public/accela/css/SVD/style.css" rel="stylesheet" type="text/css" />
    <link href="public/accela/css/googleapis.css" rel="stylesheet" type="text/css" />
    
</head>
<body>
    <div id="Results" v-cloak>
         <!-- County Heading -->
            <div class="container-1">
            <section class="container">
		        <div class="container-page">				
		            <div class="col-lg-12" id="sdcounty">
                        <p><a href="http://www.sandiegocounty.gov/content/sdc/home.html">
                        <span id="dept-cosd-domain">
                            <span id="sdc-link">SanDiegoCounty.gov</span>
                        </span>
                        </a></p>
                    </div>

                    <div class="col-lg-12" id="header_c">
                        <div id="cosd-header" class="dept-header">
                            <a id="cosd-seal-link" href="http://www.sandiegocounty.gov/content/sdc/dpw.html"><img title="" alt="" id="cosd-seal" src="http://www.sandiegocounty.gov/content/dam/sdc/initialcontent/logo/cosd-seal-sm.png"></a>
                            <a id="dept-name-link" href="http://www.sandiegocounty.gov/content/sdc/dpw.html"><span id="dept-name">Department of Public Works</span></a>
                                    
                            <div id="terrain-dept">
                                <img id="terrain-mobile" src="http://www.sandiegocounty.gov/etc/designs/sdc/county/countydesign/images/terrain-mobile.png">
                            </div>
                        </div>
                        <div id="navbar-wrapper" >
                        </div>
                    </div>
		        </div>
	        </section>
            </div>


    <div v-if="!isSubmitted">
        <div class="container-fluid">
            <section class="container">
		        <div class="container-page">				
			        <div class="col-md-6">
				
				        <h3 class="heading"><strong>TCBMP </strong> Self Verification<span></span></h3><br />
				        <div class="form-group col-lg-12">
					        <label>TCBMP Identification Number</label>
					        <input type="text" v-model="tcbmpid" required="" value="" name="tcbmpid" id="search_tcbmp" class="form-control">
                    
				        </div>
				
				        <div class="form-group col-lg-12">
					        <label>PIN #</label>
					        <input type="text" v-model="inspId" required="" value="" name="inspid" id="inspid" class="form-control">
				        </div>
			        </div>
		
			        <div class="col-md-6">
                        <h3 class="heading">Details about Self Verification</h3><br />
				
				        <p>
					        If your facility utilizes proprietary treatment control devices for
                            stormwater runoff, a maintenance agreement and detailed maintenance plan should be
                            developed to ensure that they are well maintained, and operate according to design
                            specifications. For many manufactured devices, municipalities can contract with the
                            manufacturer or representative to provide maintenance services. 
				        </p>
				        
                        <input type="submit" v-bind:disabled="!tcbmpid || !inspId" onclick="MyVue.ValidateTCBMP()" id="search_button" value="Search" name="submit" class="btn btn-primary">
                                            <label class="help-block" v-if="errMesg">Invalid Entry</label> 
			        </div>

		        </div>
	        </section>
        </div>
        <div v-if="verified" class="container-fluid">
            <section class="container">
		        <div class="container-page">				
			        <div class="col-md-6">
				
				        <h3 class="heading">TCBMP Information Verified</h3><br />
				        <p>
					        Treatment Control BMP Identification Number is {{TCBMPReturnObject.recordId}} <br /> Permit Number is {{TCBMPReturnObject.spcText}}.
				        </p>
                    
                        <p v-for="contact in contacts">
                            {{contact.fullName}} with email address {{contact.email}}<br>
                        </p>
			        </div>
		
			        <div class="col-md-6">
                        <h3 class="heading">Upload a Picture of TCBMP</h3><br />
				        <p>
					        By checking this box, I am certifying that the structural BMP identified has been inspected and all necessary maintenance conducted.  The structural BMP is operating effectively and has not been altered from its approved design.
                        </p>
                        <input type="checkbox" id="checkbox" v-model="checked">
                        <label for="checkbox">I have read and accepted the above terms.</label>
				        
                        
                        <div class="fileupload fileupload-new" data-provides="fileupload">
                            <div class="form-group" v-if="!submitObject.image">
                                <div class="form-group">
                                    <input type="file" v-on:change="onFileChange" name="file">
                                </div>
                            </div>
                            <div class="form-group" v-else>
                                <button type="submit" class="btn btn-link" v-on:click="removeImage">Remove image</button>  
                                <img :src="submitObject.image" class="img-rounded image-mobile" .img-responsive alt="Your Uploaded Image" />
                            </div>
                         </div>
                        <button type="submit" id="submitbut" v-bind:disabled="isSubmitting || !submitObject.image || !checked" value="Submit" v-on:click="SubmtImage();" name="submit" class="btn btn-primary pull-right">Submit Request <span class="glyphicon glyphicon-chevron-right"></span></button><br><br>                
			        </div>
		        </div>
	        </section>
        </div>
        </div>
        <div v-if="isSubmitted" class="container-fluid">
            <section class="container">
		        <div class="container-page">				
			        <div class="col-md-6">
				        <h3>Thank You for Submitting your Verification.</h3>
                         <h4>Your request confirmation number is {{docId}}</h4>
                         <!-- 5. Submit New Request -->
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <button type="submit" id="newButt" class="btn btn-primary pull-right" v-on:click="anotherRequestPage();"> Submit Another Verification <span class="glyphicon glyphicon-chevron-right"></span></button>
                                    </div>
                                </div>
                            </div>
                            <!-- /.row -->
			        </div>
		        </div>
	        </section>
        </div>

        <div class="container-fluid">
            <div class="container-1">
            <section class="container">
		        <div class="container-page">				
		            <div class="">
                        <div class="row">
                          <div id="footer">
                            <div class="footer">
                                <div id="footer-container">
                                    <div id="cosd-seal-link" class="footer-column">
                                         <img alt="cosd logo" id="cosd-logo" src="http://www.sandiegocounty.gov/content/dam/sdc/initialcontent/logo/cosd-logo.png">
                                     </div>   
                                    <div id="footer-contact" class="footer-column">
                                        <h2 class="footer-heading">County of San Diego </h2>
                                        
                                    </div>     
                                </div>
                            </div>
                        </div>
                      </div>
                    </div>
		        </div>
	        </section>
            </div>
        </div>
        

    </div>
    

    <script src="public/accela/js/vue.js" type="text/javascript"></script>
    <script src="public/accela/js/vue-validator.min.js" type="text/javascript"></script>
    <script src="public/accela/js/jquery.js" type="text/javascript"></script>
    
    <script src="public/accela/js/bootstrap.min.js"></script>

    <script src="public/accela/js/SVD/fileuploader.js" type="text/javascript"></script>
    <script src="public/accela/js/SVD/SVD.js" type="text/javascript"></script>
    <!--<script src="public/accela/js/googlemap.js" type="text/javascript"></script>-->

</body>
</html>
