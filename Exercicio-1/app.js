const rooms = [
  { name: "Sala Aurora", location: "Torre A", capacity: 8 },
  { name: "Sala Horizonte", location: "Torre A", capacity: 12 },
  { name: "Sala Criativa", location: "Torre B", capacity: 6 },
  { name: "Auditório", location: "Torre B", capacity: 20 }
];

const reservations = [
  { room: "Sala Aurora", start: "09:00", end: "10:30", title: "Planejamento comercial", responsible: "Ana Silva", color: "" },
  { room: "Sala Horizonte", start: "10:00", end: "11:30", title: "Daily de produtos", responsible: "Bruno Costa", color: "orange" },
  { room: "Sala Criativa", start: "13:00", end: "15:00", title: "Workshop do projeto", responsible: "Carla Mendes", color: "pink" },
  { room: "Auditório", start: "14:30", end: "16:30", title: "Apresentação trimestral", responsible: "Ana Silva", color: "green" }
];

const roomsGrid = document.getElementById("roomsGrid");
const reservationList = document.getElementById("reservationList");
const modalBackdrop = document.getElementById("modalBackdrop");
const form = document.getElementById("reservationForm");
const formError = document.getElementById("formError");
let currentDate = new Date(2026, 10, 17);

function timeToPercent(time) {
  const [hours, minutes] = time.split(":").map(Number);
  return ((hours - 8) * 60 + minutes) / (10 * 60) * 100;
}

function renderRooms() {
  const location = document.getElementById("locationFilter").value;
  const capacity = Number(document.getElementById("capacityFilter").value);
  const filtered = rooms.filter(room =>
    (location === "todos" || room.location === location) &&
    (!capacity || room.capacity <= capacity)
  );

  roomsGrid.style.gridTemplateColumns = `repeat(${Math.max(filtered.length, 1)}, minmax(135px, 1fr))`;
  roomsGrid.innerHTML = "";

  filtered.forEach(room => {
    const column = document.createElement("div");
    column.className = "room-column";

    const header = document.createElement("div");
    header.className = "room-header";
    header.innerHTML = `<div><div class="room-name">${room.name}</div><div class="room-meta">${room.location} · ${room.capacity} pessoas</div></div>`;
    column.appendChild(header);

    reservations.filter(item => item.room === room.name).forEach(item => {
      const top = timeToPercent(item.start);
      const height = timeToPercent(item.end) - top;
      const booking = document.createElement("button");
      booking.className = `booking ${item.color}`;
      booking.style.top = `calc(51px + (100% - 51px) * ${top / 100})`;
      booking.style.height = `calc((100% - 51px) * ${height / 100})`;
      booking.innerHTML = `<strong>${item.start}–${item.end}</strong><span>${item.title}</span>`;
      column.appendChild(booking);
    });

    roomsGrid.appendChild(column);
  });

  document.getElementById("availableCount").textContent = Math.max(filtered.length - 1, 0);
}

function renderReservations() {
  reservationList.innerHTML = "";
  reservations.forEach(item => {
    const element = document.createElement("div");
    element.className = "reservation-item";
    element.innerHTML = `
      <div class="reservation-time"><strong>${item.start}</strong><span>até ${item.end}</span></div>
      <div class="reservation-info"><strong>${item.title}</strong><span>${item.room}</span></div>
      <span class="reservation-status"></span>
    `;
    reservationList.appendChild(element);
  });
}

function renderReservationsTable(search = "") {
  const table = document.getElementById("reservationsTable");
  const term = search.toLowerCase();
  const visible = reservations.filter(item =>
    `${item.title} ${item.room} ${item.responsible}`.toLowerCase().includes(term)
  );

  table.innerHTML = `
    <div class="data-row header">
      <span>Reserva</span><span>Sala</span><span>Responsável</span><span>Status</span>
    </div>
  `;

  visible.forEach(item => {
    const row = document.createElement("div");
    row.className = "data-row";
    row.innerHTML = `
      <div><strong>${item.title}</strong><small>${item.start} às ${item.end}</small></div>
      <div><strong>${item.room}</strong><small>17 nov. 2026</small></div>
      <div>${item.responsible}</div>
      <div><span class="status-pill">Confirmada</span></div>
    `;
    table.appendChild(row);
  });
}

function renderRoomCards() {
  const cards = document.getElementById("roomCards");
  cards.innerHTML = "";
  rooms.forEach(room => {
    const card = document.createElement("article");
    card.className = "room-card";
    card.innerHTML = `
      <div class="room-card-top">
        <div><h3>${room.name}</h3><p>${room.location}</p></div>
        <span class="status-pill">Disponível</span>
      </div>
      <div class="room-card-footer">
        <span>Capacidade: ${room.capacity} pessoas</span>
        <button class="secondary-button room-reserve" data-room="${room.name}">Reservar</button>
      </div>
    `;
    cards.appendChild(card);
  });
}

function renderEmbeddedForm() {
  const container = document.getElementById("embeddedReservationForm");
  container.innerHTML = `
    <form id="quickReservationForm">
      <label>Sala<select name="room" required>
        <option value="">Selecione uma sala</option>
        ${rooms.map(room => `<option value="${room.name}">${room.name} · ${room.location} · ${room.capacity} pessoas</option>`).join("")}
      </select></label>
      <div class="form-row">
        <label>Data<input type="date" name="date" value="2026-11-17" required></label>
        <label>Objetivo<input type="text" name="objective" placeholder="Ex: Reunião de equipe" required></label>
      </div>
      <div class="form-row">
        <label>Início<select name="start"><option>08:00</option><option>09:00</option><option>10:00</option><option>11:00</option><option>13:00</option><option>14:00</option><option>15:00</option></select></label>
        <label>Término<select name="end"><option>09:00</option><option>10:00</option><option>11:00</option><option>12:00</option><option>14:00</option><option>15:00</option><option>16:00</option><option>17:00</option></select></label>
      </div>
      <label>Responsável<input type="text" name="responsible" value="Ana Silva" required></label>
      <p class="form-error" id="quickFormError" hidden></p>
      <div class="modal-actions"><button type="submit" class="primary-button">Confirmar reserva</button></div>
    </form>
  `;

  document.getElementById("quickReservationForm").addEventListener("submit", event => {
    event.preventDefault();
    const data = new FormData(event.currentTarget);
    const room = data.get("room");
    const start = data.get("start");
    const end = data.get("end");
    const error = document.getElementById("quickFormError");
    const conflict = reservations.find(item => item.room === room && start < item.end && end > item.start);

    if (!room) {
      error.textContent = "Selecione uma sala para continuar.";
      error.hidden = false;
      return;
    }
    if (start >= end) {
      error.textContent = "O horário de término deve ser posterior ao início.";
      error.hidden = false;
      return;
    }
    if (conflict) {
      error.textContent = `Conflito de agenda: ${room} já está reservada entre ${conflict.start} e ${conflict.end}.`;
      error.hidden = false;
      return;
    }

    reservations.push({ room, start, end, title: data.get("objective"), responsible: data.get("responsible"), color: "" });
    renderRooms();
    renderReservations();
    renderReservationsTable();
    switchView("reservas");
  });
}

function formatDate(date) {
  return date.toLocaleDateString("pt-BR", { day: "2-digit", month: "long" });
}

function updateDate(delta) {
  currentDate.setDate(currentDate.getDate() + delta);
  document.getElementById("selectedDate").querySelector("span:last-child").textContent = formatDate(currentDate);
  document.getElementById("agendaTitle").textContent = currentDate.toLocaleDateString("pt-BR", { day: "2-digit", month: "short" });
  document.querySelector(".week-label").textContent = currentDate.toLocaleDateString("pt-BR", { weekday: "long" }).toUpperCase();
  document.querySelector("input[name='date']").value = currentDate.toISOString().slice(0, 10);
}

document.getElementById("openModal").addEventListener("click", () => {
  formError.hidden = true;
  modalBackdrop.hidden = false;
});

document.getElementById("closeModal").addEventListener("click", () => modalBackdrop.hidden = true);
document.getElementById("cancelModal").addEventListener("click", () => modalBackdrop.hidden = true);
modalBackdrop.addEventListener("click", event => {
  if (event.target === modalBackdrop) modalBackdrop.hidden = true;
});

form.addEventListener("submit", event => {
  event.preventDefault();
  const data = new FormData(form);
  const room = data.get("room");
  const start = data.get("start");
  const end = data.get("end");

  if (!room) {
    formError.textContent = "Selecione uma sala para continuar.";
    formError.hidden = false;
    return;
  }

  if (start >= end) {
    formError.textContent = "O horário de término deve ser posterior ao horário de início.";
    formError.hidden = false;
    return;
  }

  const conflict = reservations.find(item =>
    item.room === room && start < item.end && end > item.start
  );

  if (conflict) {
    formError.textContent = `Conflito de agenda: ${room} já está reservada entre ${conflict.start} e ${conflict.end}.`;
    formError.hidden = false;
    return;
  }

  reservations.push({
    room,
    start,
    end,
    title: data.get("objective"),
    responsible: data.get("responsible"),
    color: ""
  });

  form.reset();
  form.elements.date.value = currentDate.toISOString().slice(0, 10);
  form.elements.responsible.value = "Ana Silva";
  modalBackdrop.hidden = true;
  renderRooms();
  renderReservations();
});

document.getElementById("previousDay").addEventListener("click", () => updateDate(-1));
document.getElementById("nextDay").addEventListener("click", () => updateDate(1));
document.getElementById("todayButton").addEventListener("click", () => {
  currentDate = new Date(2026, 10, 17);
  updateDate(0);
});
document.getElementById("locationFilter").addEventListener("change", renderRooms);
document.getElementById("capacityFilter").addEventListener("change", renderRooms);

document.querySelectorAll(".nav-item[data-view]").forEach(button => {
  button.addEventListener("click", () => {
    const view = button.dataset.view;
    if (view === "nova") {
      switchView("nova");
    } else {
      switchView(view);
    }
  });
});

function switchView(view) {
  document.querySelectorAll(".nav-item[data-view]").forEach(item =>
    item.classList.toggle("active", item.dataset.view === view)
  );
  ["agenda", "reservas", "salas", "nova"].forEach(name => {
    const screen = document.getElementById(`${name}Screen`);
    if (screen) screen.hidden = name !== view;
  });

  const title = document.querySelector(".topbar h1");
  const labels = {
    agenda: "Agenda de salas",
    reservas: "Reservas",
    salas: "Salas",
    nova: "Nova reserva"
  };
  title.textContent = labels[view];

  if (view === "reservas") renderReservationsTable(document.getElementById("reservationSearch").value);
  if (view === "salas") renderRoomCards();
  if (view === "nova") {
    renderEmbeddedForm();
    document.getElementById("availabilitySlots").innerHTML = `
      <div class="availability-slot"><span>08:00 – 09:00</span><span>Livre</span></div>
      <div class="availability-slot"><span>12:00 – 13:00</span><span>Livre</span></div>
      <div class="availability-slot"><span>16:00 – 17:00</span><span>Livre</span></div>
    `;
  }
}

document.getElementById("reservationSearch").addEventListener("input", event =>
  renderReservationsTable(event.target.value)
);

document.querySelectorAll("[data-open-reservation]").forEach(button =>
  button.addEventListener("click", () => switchView("nova"))
);

document.getElementById("openRoomModal").addEventListener("click", () => {
  const name = window.prompt("Nome da sala:");
  if (!name) return;
  const location = window.prompt("Localização (ex: Torre A):", "Torre A");
  const capacity = Number(window.prompt("Capacidade:", "8"));
  if (!location || !capacity) return;
  rooms.push({ name, location, capacity });
  renderRoomCards();
  renderRooms();
});

document.getElementById("roomCards").addEventListener("click", event => {
  const button = event.target.closest(".room-reserve");
  if (!button) return;
  switchView("nova");
  renderEmbeddedForm();
  const roomSelect = document.querySelector("#quickReservationForm select[name='room']");
  roomSelect.value = button.dataset.room;
});

document.getElementById("roomsShortcut").addEventListener("click", () => switchView("salas"));

renderRooms();
renderReservations();
