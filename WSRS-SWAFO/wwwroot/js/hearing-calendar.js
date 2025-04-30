document.addEventListener('DOMContentLoaded', function () {
    const calendarEl = document.getElementById('calendar');
    if (!calendarEl) {
        console.warn('Calendar container not found.');
        return;
    }

    const calendar = new FullCalendar.Calendar(calendarEl, {
        themeSystem: 'bootstrap5',
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        },
        selectable: true,
        editable: false,
        droppable: true,
        events: '/HearingScheduling/GetHearings',

        eventClick: function (info) {
            const event = info.event;
            const start = new Date(event.start);

            const formattedDate = start.getFullYear() + '-' +
                String(start.getMonth() + 1).padStart(2, '0') + '-' +
                String(start.getDate()).padStart(2, '0') + 'T' +
                String(start.getHours()).padStart(2, '0') + ':' +
                String(start.getMinutes()).padStart(2, '0');

            document.getElementById("eventId").value = event.id || "";
            document.getElementById("eventTitleInput").value = event.title || "";
            document.getElementById("eventDateInput").value = formattedDate;

            document.getElementById("eventStudentNumber").value = event.extendedProps.studentNumber || "";
            document.getElementById("eventStudentName").value = event.extendedProps.fullName || "";
            document.getElementById("eventStudentEmail").value = event.extendedProps.email || "";

            const modal = new bootstrap.Modal(document.getElementById('eventModal'));
            modal.show();
        }
    });

    const editBtn = document.getElementById('editEvent');
    if (editBtn) {
        editBtn.addEventListener('click', function () {
            document.getElementById('eventTitleInput').readOnly = false;
            document.getElementById('eventDateInput').readOnly = false;
            document.getElementById('saveEvent').style.display = 'inline-block';
        });
    }

    const editForm = document.getElementById('editEventForm');
    if (editForm) {
        editForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const updatedEvent = {
                Id: parseInt(document.getElementById('eventId').value),
                Title: document.getElementById('eventTitleInput').value,
                ScheduledDate: document.getElementById('eventDateInput').value
            };

            fetch('/HearingScheduling/UpdateHearing', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(updatedEvent)
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        alert("Updated successfully!");
                        location.reload();
                    } else {
                        alert("Update failed.");
                    }
                });
        });
    }

    const deleteBtn = document.getElementById('deleteEvent');
    if (deleteBtn) {
        deleteBtn.addEventListener('click', function () {
            const eventId = parseInt(document.getElementById('eventId').value);
            if (confirm("Are you sure you want to delete this schedule?")) {
                fetch(`/HearingScheduling/DeleteHearing/${eventId}`, { method: 'DELETE' })
                    .then(res => res.json())
                    .then(data => {
                        if (data.success) {
                            alert("Deleted successfully.");
                            location.reload();
                        } else {
                            alert("Deletion failed.");
                        }
                    });
            }
        });
    }

    const studentInput = document.getElementById('studentNumber');
    if (studentInput) {
        studentInput.addEventListener('input', function () {
            const studentNumber = this.value;
            const infoField = document.getElementById('studentInfo');

            if (studentNumber.length >= 4) {
                fetch(`/HearingScheduling/GetStudent?studentNumber=${encodeURIComponent(studentNumber)}`)
                    .then(response => response.json())
                    .then(data => {
                        if (data) {
                            infoField.textContent = `Student: ${data.firstName} ${data.lastName}`;
                        } else {
                            infoField.textContent = "No student found.";
                        }
                    });
            } else {
                infoField.textContent = "";
            }
        });
    }

    const addScheduleModal = document.getElementById('addScheduleModal');
    if (addScheduleModal) {
        addScheduleModal.addEventListener('shown.bs.modal', () => {
            const dateInput = document.getElementById('scheduledDate');
            const now = new Date();
            const localNow = now.toISOString().slice(0, 16); // Format to YYYY-MM-DDTHH:MM
            dateInput.min = localNow;
        });
    }

    const addForm = document.getElementById('addScheduleForm');
    if (addForm) {
        addForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const hearingData = {
                StudentNumber: parseInt(document.getElementById("studentNumber").value),
                Title: document.getElementById("title").value,
                ScheduledDate: document.getElementById("scheduledDate").value // Ensure the datetime is in the correct format
            };

            fetch('/HearingScheduling/AddHearing', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(hearingData)
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        alert("Hearing scheduled successfully.");
                        location.reload();
                    } else {
                        alert("Error: " + data.error);
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    alert('An error occurred while saving.');
                });
        });
    }

    calendar.render();
});