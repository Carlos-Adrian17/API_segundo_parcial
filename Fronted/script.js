// SUSTITUYE ESTO POR LA URL DE TU API EN AZURE 
const API_URL = "https://tu-api-backend.azurewebsites.net/api/pacientes";

// 1. Cargar Pacientes (GET)
async function cargarPacientes() {
    try {
        const respuesta = await fetch(API_URL);
        if (!respuesta.ok) throw new Error("Error al obtener los datos");

        const pacientes = await respuesta.json();
        const tbody = document.getElementById('tablaPacientesBody');
        tbody.innerHTML = '';

        // Renderizar cada paciente
        pacientes.forEach(paciente => {
            // Fila roja clara si la gravedad es 5 
            const esGrave = paciente.nivelGravedad === 5 ? 'table-danger' : '';

            const tr = document.createElement('tr');
            tr.className = esGrave;
            tr.innerHTML = `
                <td>${paciente.idPaciente}</td>
                <td>${paciente.nombreCompleto}</td>
                <td>${paciente.nivelGravedad}</td>
                <td>${paciente.estado}</td>
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

    // Obtener valores del formulario [cite: 76]
    const nuevoPaciente = {
        nombreCompleto: document.getElementById('nombre').value,
        sintomas: document.getElementById('sintomas').value,
        nivelGravedad: parseInt(document.getElementById('gravedad').value),
        carnetMedico: document.getElementById('carnetMedico').value
    };

    try {
        const respuesta = await fetch(API_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(nuevoPaciente)
        });

        // Validar si el carnet del médico es inválido (Error 401 u otro error de negocio) [cite: 95]
        if (respuesta.status === 401 || !respuesta.ok) {
            alertaDiv.innerHTML = `<div class="alert alert-danger">Acceso Denegado: Carnet de médico no válido.</div>`;[cite: 78]
            return;
        }

        // Si es exitoso
        alertaDiv.innerHTML = `<div class="alert alert-success">Paciente registrado exitosamente.</div>`;
        document.getElementById('formRegistro').reset();

        // Refrescar el tablero (opcional pero recomendado)
        cargarPacientes();

    } catch (error) {
        console.error("Error al guardar:", error);
        alertaDiv.innerHTML = `<div class="alert alert-danger">Error de conexión con el servidor.</div>`;
    }
});

// Cargar pacientes al iniciar la página
document.addEventListener('DOMContentLoaded', cargarPacientes);