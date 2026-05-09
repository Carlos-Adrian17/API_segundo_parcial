// SUSTITUYE ESTO POR LA URL DE TU API EN AZURE (Ej. https://segundoparcial3364.azurewebsites.net/scalar/#tag/pacientes/POST/api/Pacientes)
const API_URL = "https://segundoparcial3364.azurewebsites.net/scalar/#tag/pacientes/POST/api/Pacientes";

// 1. Cargar Pacientes (GET)
async function cargarPacientes() {
    try {
        const respuesta = await fetch(API_URL);
        if (!respuesta.ok) throw new Error("Error al obtener los datos");

        const pacientes = await respuesta.json();
        const tbody = document.getElementById('tablaPacientesBody');
        tbody.innerHTML = '';

        pacientes.forEach(paciente => {
            // Fila roja clara si la gravedad es 5 (Requerimiento de la hoja)
            const esGrave = paciente.nivelGravedad === 5 ? 'table-danger' : '';

            const tr = document.createElement('tr');
            tr.className = esGrave;
            // Usamos los nombres exactos que .NET genera basados en tu base de datos (camelCase)
            tr.innerHTML = `
                <td>${paciente.pacienteId || 'N/A'}</td>
                <td>${paciente.nombreCompleto || 'Desconocido'}</td>
                <td>${paciente.sintomas || 'No registrados'}</td> <td>${paciente.nivelGravedad}</td>
                <td>${paciente.estado || 'En espera'}</td>
                <td>${paciente.medicoResponsable || 'N/A'}</td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error("Error:", error);
    }
}

// 2. Registrar Nuevo Paciente (POST)
document.getElementById('formRegistro').addEventListener('submit', async function (e) {
    e.preventDefault();
    const alertaDiv = document.getElementById('alertaFormulario');
    alertaDiv.innerHTML = '';

    // Construimos el objeto con TODAS las columnas que tienes en tu BD
    const nuevoPaciente = {
        nombreCompleto: document.getElementById('nombre').value,
        sintomas: document.getElementById('sintomas').value, // Ya se enviará a la base de datos
        nivelGravedad: parseInt(document.getElementById('gravedad').value),
        medicoResponsable: document.getElementById('carnetMedico').value,
        estado: "En espera", // Valor por defecto
        fechaIngreso: new Date().toISOString() // Genera la fecha actual automáticamente
    };

    try {
        const respuesta = await fetch(API_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(nuevoPaciente)
        });

        if (respuesta.status === 401 || !respuesta.ok) {
            alertaDiv.innerHTML = `<div class="alert alert-danger">Error: Acceso Denegado o datos inválidos.</div>`;
            return;
        }

        alertaDiv.innerHTML = `<div class="alert alert-success">Paciente registrado exitosamente.</div>`;
        document.getElementById('formRegistro').reset();

        cargarPacientes();

    } catch (error) {
        console.error("Error al guardar:", error);
        alertaDiv.innerHTML = `<div class="alert alert-danger">Error de conexión con el servidor.</div>`;
    }
});

// Cargar pacientes al iniciar la página
document.addEventListener('DOMContentLoaded', cargarPacientes);