
function Validate() {
    var usr = { email: $('#email').val(), password: $('#password').val() }

        $.post("/Users/UsersLogin", usr , function (data) {
            if (data.status) {
                var userurl = "/Users/Homepage";
                var adminurl = "/Home/Index/{id?}";
                if (usr.email == "admin") {
                    window.location.href = adminurl;
                }
                else {
                    window.location.href = userurl;
                }
            }
            if (!data.status) {
                RedirectToLogin();
            }
        });
  
}

function RedirectToLogin() {
    var userurl = "/Users/Login";
    window.location.href = userurl;
}
function UpdatePassWord() {
    var userurl = "/Users/ForgotPassword";
    window.location.href = userurl;
}
function ViewElections() {
    var userurl = "/Election/Index";
    window.location.href = userurl;
}

function addcandidate() {
    var userurl = "/Candidate/Create";
    window.location.href = userurl;
}
function addelection() {
    var userurl = "/Election/Create";
    window.location.href = userurl;
}
function addUser() {
    var userurl = "/User/Create";
    window.location.href = userurl;
}
function ViewUser() {
    var userurl = "/Users/Index";
    window.location.href = userurl;
}
function ViewCandidates() {
    var userurl = "/Candidate/Index";
    window.location.href = userurl;
}
function Register() {
    var usr = {
        FirstName: $("#FirstName").val(),
        LastName:$("#LastName").val(),
        Gender:$('input:radio[name=gender]:checked').val(),
        email:$("#email").val(),
        PhoneNo: $("#PhoneNo").val(),
        DateOfBirth:$("#DateOfBirth").val(),
        address:$("#address").val()
       
    }
    $.post("/Users/Createu", usr, function (data) {
        
        if (data.status) 
        {   

            console.log(data.passcode);
           
        }
    });

}
//$('#CanVote').change(function () {
//    $('#CanVote').attr('checked') ? true : false;
//});

