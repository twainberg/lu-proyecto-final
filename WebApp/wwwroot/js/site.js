// Write your JavaScript code.
var td_color_confirmado = 'rgba(42, 245, 24, 0.42)';
var td_color = 'rgba(0, 0, 255, 0.58)';

function SeleccionarMateria(id, materiaId, nombre, dia, turno) 
{
    var opciones = $('.opciones');
    for (let i = 0; i < opciones.length; i++) 
    {
        if (opciones[i].parentElement.style.backgroundColor == td_color) 
        {
            opciones[i].parentElement.style.backgroundColor = 'initial';
        }              
    }

    var materia = $('#' + id);
    var materia0 = materia[0];
    materia0.parentElement.style.backgroundColor = td_color;

    var confirmar = $('#confirmar');
    var confirmarbtn = confirmar[0];
    var href = '/Inscripcion/ConfirmaInscripcion?id=' + materiaId + '&nombre=' + nombre + '&dia=' + dia + '&turno=' + turno;
    confirmarbtn.href = href;
    confirmarbtn.style.cursor = 'pointer';
}

function CambiarCelda() 
{
    var materiasAnotadas = $('.materias-anotadas');

    for (let i = 0; i < materiasAnotadas.length; i++) 
    {
        if (materiasAnotadas[i].textContent != '') 
        {
            materiasAnotadas[i].parentElement.style.backgroundColor = td_color_confirmado;   
        }
    }
}

function ValidarEnvio(claseFormulario) 
{
    var formulario = $('.' + claseFormulario);
    var formulario0 = formulario[0];
    var id = $('#Id');
    var dni = $('#DNI');
    var error = $('#Error');
    var alumno = $('#alumno');
    alumno.text("");

    if (id.val() == '' &&  dni.val() == '') 
    {
        error.text('Debe completar uno de los campos');
        return false;
    }
    else if(id.val() != '' &&  dni.val() != '')
    {
        error.text('Debe completar solamente un campo');
        return false;
    }
    else
    {
        formulario0.submit();
    }
}

function CalendarioAmigo(id)
{
    var div = $('#Calendario');
    
    $("#friendSearch, .buscadoramigos2").hide();
    div.empty();
    
    $.get("../Calendario/CalendarioAmigo", { id: id } , function(data) {
        div.append(data);
        $('#boton').toggle();
    });
}

function BotonCalendario()
{ 
    $("#friendSearch, .buscadoramigos2").show();
    $('#Calendario').empty();  

    $('#boton').toggle(); 
}

function ActualizarMateria(id) 
{
    var estadoMateria = $('#' + id + '-estado');
    var notaMateria = $('#' + id + '-nota');
    var resultado = $("#" + id + "-resultado");

    $.getJSON("../Administrador/ActualizarMateria", { materiaAlumnoId: id, nota: notaMateria.val(), estado: estadoMateria.val() }, 
        function(data) 
        {
            resultado.text(data.resultado);
            if (data.codigo) 
            {
                estadoMateria.val("Cursando");
                notaMateria.val(0);
            }

            setTimeout(() => {resultado.text("");}, 2000);
        }
    )
}  