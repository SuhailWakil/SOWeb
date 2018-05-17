<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="requestPage.aspx.cs" Inherits="SOWebService.requestPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <meta name="description" content="">
    <title>Roads Service Request</title>

    <link href="public/accela/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="public/accela/css/1-col-portfolio.css" rel="stylesheet" type="text/css" />

    <link href="public/accela/css/SO/style.css" rel="stylesheet" type="text/css" />
    <link href="public/accela/css/googleapis.css" rel="stylesheet" type="text/css" />
    <script src='https://www.google.com/recaptcha/api.js'></script>

</head>
<body>

    <div id="Results" v-cloak>
        <validator name="testValidator">
               <!-- Page Content -->
                <div class="container">
                    <!-- Page Heading -->
                    <div class="row">
                        <div class="col-lg-12">
                            <h1 class="h1 blue">Roads Service Request
                                <small class="pull-right"><span class="glyphicon glyphicon-phone-alt"></span> For emergencies, call 911</small>
                            </h1>
                            <span v-if="!isSubmitted" class="h3 blue">Complete steps 1 through 4 to report a road related issue.</span>
                            <asp:Label ID="ErrorMessage" runat="server" Text="" Visible="False" class="help-block"></asp:Label>
                        </div>
                    </div>
                    <hr />
                    <!-- /.row -->
                    <div v-if="isSubmitted">
                        <h3>Thank You for Submitting your Request.</h3>
                         <h4>Your request confirmation number is {{recordId}}</h4>
                            <h3>Issue Information</h3><hr>
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group"><label>Issue Location: </label> {{submitObject.resolvedAddress}}
                                    </div>
                                    <div class="form-group"><label>Reported Category: </label> {{submitObject.problemCategory}}
                                    </div>
                                    <div class="form-group"><label>Reported Subcategory: </label> {{submitObject.problemSubcategory}}
                                    </div> 
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group"><label>Issue Details: </label> {{submitObject.customerComments}}
                                    </div>  
                                </div>
                                <div class="col-md-4">
                                    <img :src="submitObject.image" class="img-rounded" .img-responsive alt="Uploaded Image" />
                                </div>
                            </div>
                            <!-- /.row -->
                            <h3>Contact Information</h3><hr>
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group"><label>First Name:</label> {{submitObject.firstName}}
                                    </div>
                                    <div class="form-group"><label>Last Name:</label> {{submitObject.lastName}}
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group"><label>Email:</label> {{submitObject.email}}
                                    </div>
                                    <div class="form-group"><label>Phone:</label> {{submitObject.phone1}}
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group"><label>Address:</label> {{submitObject.address}}
                                    </div>
                                    <div class="form-group"><label>City:</label> {{submitObject.city}}
                                    </div>
                                </div>
                            </div>
                            <!-- /.row -->
                            <!-- 5. Submit New Request -->
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <button type="submit" id="newButt" class="btn btn-primary pull-right" v-on:click="anotherRequestPage();"> Submit Another Request <span class="glyphicon glyphicon-chevron-right"></span></button>
                                    </div>
                                </div>
                            </div>
                            <!-- /.row -->
                            <hr>
                    </div>
                    <div v-else>
                      <!-- 1. Where is the Problem? -->
                            <div class="row">
                                <div class="col-md-12">
                                    <h3>1. <span class="glyphicon glyphicon-map-marker"></span> Where is the Issue?</h3>
                                    <p>Enter the nearest street address, or use the map to zoom to your location and drop a pin.</p> 
                                    
                                    <div class="form-group">
                                        <div class="line_br">
                                            <label>Address Search</label> 
                                            <label v-if="validatedAddress" class="label label-success">Go to Step 2</label>
                                            <%--<label class="help-block" v-if="validatedAddress">{{message}} </label> 
                                            <label class="help-block" v-if="showCityLink"><a v-on:click="citiWebsite();">Take Me to City-Websites >></a></label>--%>
                                        </div>
                                        <input type="text" v-model="googleText" name="search_address" id="search_address" class="form-control1">
                                        <button type="submit" v-bind:disabled="!googleText" class="btn btn-common" onclick="search();" id="search_button"> Search</button> 
                                    </div>
                                    
                                    <div id="mapholder" class="form-group">
                                    </div>
                                    
                                    <div class="form-group">
                                        <label>Issue Location*</label><span class="help-block" v-show="$testValidator.pl.required"> Required</span>
                                        <label v-if="validatedAddress" class="label label-success">Verified Address</label>
                                        <input type="text" :disabled="true" v-model="submitObject.resolvedAddress" name="problemLocation" v-validate:pl="{required: true}" initial="off" class="form-control2">
                                    </div>
                                    
                                </div>
                            </div>
                            <!-- /.row -->

                            <hr>

		                    <!-- 2. What is the Problem? -->
                            <div class="row">
                                <div class="col-md-12">
                                    <h3>2. <span class="glyphicon glyphicon-alert"></span> What is the Issue?</h3>
                                    <div class="form-group"><label>Report Category*</label><span class="help-block" v-show="$testValidator.pc.required"> Required</span>
                                        <select v-model="submitObject.problemCategory" v-on:change="setSubCatList(submitObject.problemCategory);" name="problemCategory" v-validate:pc="{required: true}" initial="off" class="form-control2">
                                            <option></option>
                                            <option value='Road Maintenance'>Road Maintenance</option>
                                            <option value='Drainage'>Drainage</option>
                                            <option value='Traffic Issues'>Traffic Issues</option>
                                            <option value='Signs (Damaged/Missing/Requests)'>Signs (Damaged/Missing/Requests)</option>
                                        </select>
                                    </div>
                                    <div class="form-group"><label>Report Subcategory*</label><span class="help-block" v-show="$testValidator.psc.required"> Required</span>
                                        <select v-model="submitObject.problemSubcategory" name="problemSubcategory" v-validate:psc="{required: true}" initial="off" class="form-control2">
                                            <option></option>
                                            <option v-for="lp in subcatlist" v-bind:value="lp">
                                            {{ lp }}
                                            </option>
                                        </select>
                                    </div>  
                                    <div class="form-group"><label>Describe the issue in detail*</label><span class="help-block" v-show="$testValidator.cc.required"> Required</span>
                                        <textarea rows="5" v-model="submitObject.customerComments" name="customerComments" v-validate:cc="{required: true}" initial="off" class="form-control2"></textarea>
                                       
                                    </div>  
                                </div>
                            </div>
                            <!-- /.row -->

                            <hr>
                            <!-- 4. Upload any Pictures -->
                            <div class="row">
                                <div class="col-md-4">
                                    <h3>3. <span class="glyphicon glyphicon-camera"></span> Upload a Picture</h3>
                                    <div class="form-group" v-if="!submitObject.image">
                                        <div class="form-group">
                                            <input type="file" v-on:change="onFileChange" name="file">
                                        </div>
                                    </div>
                                    <div class="form-group" v-else>
                                        <button type="submit" class="btn btn-link" v-on:click="removeImage">Remove image</button>  
                                        <img :src="submitObject.image" class="img-rounded" .img-responsive alt="Your Uploaded Image" />
                                    </div>
                                </div>
                            </div>
                            <!-- /.row -->

                             <hr>

                            <!-- 3. Who are you? -->
                            <div class="row">
                                <div class="col-md-12">
                                    <h3>4. <span class="glyphicon glyphicon-user"></span> How can we contact you?</h3>
                                    <p>Providing your contact info will allow us to provide progress updates and follow up with any questions. Your contact information will not be visible to the public.</p>
                                    <div class="form-group"><label>First Name*</label><span class="help-block" v-show="$testValidator.fn.required"> Required</span>
                                    <input type="text" v-model="submitObject.firstName" name="firstName" v-validate:fn="{required: true}" initial="off" class="form-control2"></div>

                                    <div class="form-group"><label>Last Name*</label><span class="help-block" v-show="$testValidator.ln.required"> Required</span>
                                    <input type="text" v-model="submitObject.lastName" name="lastName" v-validate:ln="{required: true}" initial="off" class="form-control2"></div>
                                        
                                    <div class="form-group"><label>Email*</label><span class="help-block" v-show="$testValidator.email.required"> Valid Email Required</span>
                                    <input type="text" v-model="submitObject.email" name="email" v-validate:email="{required: true}" initial="off"  @blur="validateEmail(submitObject.email)" class="form-control2">
                                    </div>
     
                                    <div class="form-group"><label>Phone #*</label><span class="help-block" v-show="$testValidator.ph.required"> Valid Phone # Required (10 digits only)</span>
                                    <input type="text" v-model="submitObject.phone1 | phoneNumber" name="phone1" v-validate:ph="{required: true}" initial="off" @blur="validatePhone(submitObject.phone1)" class="form-control2"></div>
                                        
                                    <div class="form-group"><label>Address</label>
                                    <input type="text" v-model="submitObject.address" name="address" @blur="validateAddress(submitObject.address)" class="form-control2"></div>

                                    <div class="form-group"><label>City</label>
                                    <input type="text" v-model="submitObject.city" name="city" @blur="validateCity(submitObject.city)" class="form-control2"></div> 
                                </div>
                            </div>

                            <!-- /.row -->

                            <div class="g-recaptcha" :data-sitekey="rcapt_sig_key"></div>      
                             
                            <!-- 5. Submit your Request -->
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <button type="submit" id="submitbut" v-bind:disabled="!$testValidator.valid || isSubmitting" class="btn btn-primary pull-right" v-on:click="SubmitRecordForProcessing();"> Submit Request <span class="glyphicon glyphicon-chevron-right"></span></button>
                                    </div>
                                </div>
                            </div>
                            <!-- /.row -->
                            <hr>
                        </div>
                </div>
                <!-- /.container -->
        </validator>
    </div>


    <script src="public/accela/js/vue.js" type="text/javascript"></script>
    <script src="public/accela/js/vue-validator.min.js" type="text/javascript"></script>
    <script src="public/accela/js/jquery.js" type="text/javascript"></script>
    <script src="public/accela/js/bootstrap.min.js"></script>
    <script src="public/accela/js/SO/requestPage.js" type="text/javascript"></script>
    <script src="public/accela/js/SO/googlemap.js" type="text/javascript"></script>

</body>
</html>
