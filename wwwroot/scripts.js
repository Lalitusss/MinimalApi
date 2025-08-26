let currentPage = 1;
const pageSize = 25;
let totalRecords = 0;

async function cargarTarjetas(page = 1) {
    clearMessage();
    try {
        currentPage = page;
        const resp = await fetch(`/tarjetas?pageNumber=${currentPage}&pageSize=${pageSize}`);
        if (!resp.ok) {
            mostrarMensaje('Error al cargar las tarjetas.');
            return;
        }
        const data = await resp.json();
        totalRecords = data.totalRecords;
        mostrarTarjetas(data.results);
        actualizarInfoPagina();
    } catch (error) {
        mostrarMensaje('Error al conectarse con el servidor.');
    }
}

function mostrarTarjetas(tarjetas) {
    const tbody = document.querySelector('#tarjetasTable tbody');
    tbody.innerHTML = '';
    if (!tarjetas || tarjetas.length === 0) {
        mostrarMensaje('No se encontraron tarjetas.');
        return;
    }
    tarjetas.forEach(t => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${t.id}</td>
            <td>${t.nombreTitular}</td>
            <td>${t.numeroTarjeta}</td>
            <td>${t.estado}</td>
            <td>${t.activa ? 'Sí' : 'No'}</td>
            <td><button class="btnEliminar" onclick="eliminarTarjeta(${t.id})">Eliminar</button></td>
        `;
        tbody.appendChild(tr);
    });
}

function actualizarInfoPagina() {
    const totalPages = Math.ceil(totalRecords / pageSize);
    document.getElementById('pageInfo').textContent = `Página ${currentPage} de ${totalPages}`;
    document.getElementById('btnPrev').disabled = currentPage <= 1;
    document.getElementById('btnNext').disabled = currentPage >= totalPages;
}

function paginaSiguiente() {
    if (currentPage * pageSize < totalRecords) {
        cargarTarjetas(currentPage + 1);
    }
}

function paginaAnterior() {
    if (currentPage > 1) {
        cargarTarjetas(currentPage - 1);
    }
}

async function buscarPorId() {
    clearMessage();
    const id = document.getElementById('searchId').value.trim();
    if (!id) {
        mostrarMensaje('Por favor, ingrese un ID para buscar.');
        return;
    }
    try {
        const resp = await fetch(`/tarjetas/${id}`);
        if (!resp.ok) {
            if (resp.status === 404) {
                mostrarMensaje('No se encontró una tarjeta con ese ID.');
            } else {
                mostrarMensaje('Error al buscar la tarjeta.');
            }
            limpiarTabla();
            actualizarInfoPagina();
            return;
        }
        const tarjeta = await resp.json();
        mostrarTarjetas([tarjeta]);
        document.getElementById('pageInfo').textContent = 'Resultado de búsqueda';
        document.getElementById('btnPrev').disabled = true;
        document.getElementById('btnNext').disabled = true;
    } catch (error) {
        mostrarMensaje('Error al realizar la búsqueda.');
        limpiarTabla();
        actualizarInfoPagina();
    }
}

function mostrarTodos() {
    document.getElementById('searchId').value = '';
    cargarTarjetas(1);
}

async function eliminarTarjeta(id) {
    clearMessage();
    if (!confirm(`¿Está seguro de eliminar la tarjeta con ID ${id}?`)) {
        return;
    }
    try {
        const resp = await fetch(`/tarjetas/${id}`, { method: 'DELETE' });
        if (resp.ok) {
            mostrarMensaje(`Tarjeta con ID ${id} eliminada correctamente.`);
            cargarTarjetas(currentPage);
        } else if (resp.status === 404) {
            mostrarMensaje('No se encontró una tarjeta con ese ID para eliminar.');
        } else {
            mostrarMensaje('Error al eliminar la tarjeta.');
        }
    } catch (error) {
        mostrarMensaje('Error al realizar la operación de eliminación.');
    }
}

function mostrarMensaje(msg) {
    const messageDiv = document.getElementById('message');
    messageDiv.textContent = msg;
}

function clearMessage() {
    document.getElementById('message').textContent = '';
}

function limpiarTabla() {
    const tbody = document.querySelector('#tarjetasTable tbody');
    tbody.innerHTML = '';
}

// Carga inicial
cargarTarjetas();
