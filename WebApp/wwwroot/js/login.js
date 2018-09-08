$(document).ready(function(){
    $("#reg_btn").click(function(){
        $("#main").animate({left:"22.5%"},400); 
        $("#main").animate({left:"30%"},500); 
        $("#loginform").css("visibility","hidden");
        $("#loginform").animate({left:"25%"},400);
        
        $("#regform").animate({left:"17%"},400);
        $("#regform").animate({left:"30%"},500);
        $("#regform").css("visibility","visible");

        $('#Password').val("");
    }); 
    
    $("#login_btn").click(function(){
        $("#main").animate({left:"77.5%"},400); 
        $("#main").animate({left:"70%"},500);
        $("#regform").css("visibility","hidden");
        $("#regform").animate({left:"75%"},400);
        
        $("#loginform").animate({left:"83.5%"},400);
        $("#loginform").animate({left:"70%"},500);
        $("#loginform").css("visibility","visible");

        $('#Legajo').val("");
        $('#PasswordLogin').val("");
        $('#ErrorLogin').text("");
    });

    if(window.location.href.includes("RegistrarAlumno"))
    {
        $("#reg_btn").trigger("click");
    }
});

function ValidarRegistro() 
{
    var Nombre = $('#Nombre');
    var Dni = $('#Dni');
    var Password = $('#Password');
    var Info = $('#Info');
    var Error = $('#Error');

    if (Nombre.val() == '' || Dni.val() == '' || Password.val() == '')
    {
        Error.text('Debe completar todos los campos');
        return false;
    }
    else if(Password.val().length < 6)
    {
        Info.text("");
        Error.text('La contraseña debe contener por lo menos 6 caracteres');
        return false;
    }
    else
    {
        return true;
    }
}