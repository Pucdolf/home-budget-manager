document.addEventListener('DOMContentLoaded', function () {
    // DOM Elements
    const calendarView = document.getElementById('calendar-view');
    const eventsList = document.getElementById('events-list');
    const currentDateElement = document.getElementById('current-date');
    const todayBtn = document.getElementById('today-btn');
    const prevBtn = document.getElementById('prev-btn');
    const nextBtn = document.getElementById('next-btn');
    const viewOptions = document.querySelectorAll('.view-option');
    const addEventBtn = document.getElementById('add-event-btn');
    const eventModal = document.getElementById('event-modal');
    const eventDetailsModal = document.getElementById('event-details-modal');
    const closeBtns = document.querySelectorAll('.close-btn');
    const eventForm = document.getElementById('event-form');
    const eventAmountInput = document.getElementById('event-amount');
    const eventDateInput = document.getElementById('event-date');
    const eventStartTimeInput = document.getElementById('event-start-time');
    const eventEndTimeInput = document.getElementById('event-end-time');
    const eventDescriptionInput = document.getElementById('event-description');
    const eventColorInput = document.getElementById('event-color');
    const eventReminderInput = document.getElementById('event-reminder');
    const detailsTitle = document.getElementById('details-title');
    const detailsDate = document.getElementById('details-date');
    const detailsTime = document.getElementById('details-time');
    const detailsDescription = document.getElementById('details-description');
    const deleteEventBtn = document.getElementById('delete-event-btn');
    const editEventBtn = document.getElementById('edit-event-btn');
    const closeDetailsBtn = document.getElementById('close-details-btn');

    // App State
    let currentView = 'month';
    let currentDate = new Date();
    let events = []; // Start empty, fetch from API
    let selectedEventId = null;

    // Initialize the app
    init();

    function init() {
        fetchEvents().then(() => {
            switchView(currentView);
            renderEventsList();
            setupEventListeners();
        });
    }

    async function fetchEvents() {
        try {
            const response = await fetch('/api/calendar-events?t=' + new Date().getTime());
            if (response.ok) {
                events = await response.json();
            } else {
                console.error('Failed to fetch events');
                events = JSON.parse(localStorage.getItem('events')) || [];
            }
        } catch (error) {
            console.error('Error fetching events:', error);
            events = JSON.parse(localStorage.getItem('events')) || [];
        }
    }

    function setupEventListeners() {
        // Navigation buttons
        todayBtn.addEventListener('click', goToToday);
        prevBtn.addEventListener('click', navigatePrevious);
        nextBtn.addEventListener('click', navigateNext);

        // View options
        viewOptions.forEach(option => {
            option.addEventListener('click', () => switchView(option.dataset.view));
        });

        // Event modal
        addEventBtn.addEventListener('click', openEventModal);
        closeBtns.forEach(btn => btn.addEventListener('click', closeModals));
        eventForm.addEventListener('submit', saveEvent);

        // Event details modal
        deleteEventBtn.addEventListener('click', deleteEvent);
        editEventBtn.addEventListener('click', editEvent);
        closeDetailsBtn.addEventListener('click', closeModals);

        // Close modal when clicking outside
        window.addEventListener('click', (e) => {
            if (e.target === eventModal) {
                closeModals();
            }
            if (e.target === eventDetailsModal) {
                closeModals();
            }
        });
    }

    function renderCalendar() {
        calendarView.innerHTML = '';

        switch (currentView) {
            case 'day':
                renderDayView();
                break;
            case 'week':
                renderWeekView();
                break;
            case 'month':
                renderMonthView();
                break;
        }

        updateCurrentDateDisplay();
    }

    function renderMonthView() {
        const monthContainer = document.createElement('div');
        monthContainer.className = 'month-view';

        // Get first day of month and total days
        const firstDay = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
        const lastDay = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
        const daysInMonth = lastDay.getDate();
        const startingDay = firstDay.getDay(); // 0 = Sunday, 1 = Monday, etc.

        // Month header
        const monthHeader = document.createElement('div');
        monthHeader.className = 'month-header';

        // Day names
        const dayNames = ['Ndz', 'Pon', 'Wt', 'Śr', 'Czw', 'Pt', 'Sob'];
        dayNames.forEach(day => {
            const dayElement = document.createElement('div');
            dayElement.className = 'day-header';
            dayElement.textContent = day;
            monthHeader.appendChild(dayElement);
        });

        monthContainer.appendChild(monthHeader);

        // Month days grid
        const daysGrid = document.createElement('div');
        daysGrid.className = 'month-days';

        // Add empty cells for days before the first day of the month
        for (let i = 0; i < startingDay; i++) {
            const prevMonthDay = new Date(currentDate.getFullYear(), currentDate.getMonth(), 0 - (startingDay - i - 1));
            const dayCell = createDayCell(prevMonthDay, true);
            daysGrid.appendChild(dayCell);
        }

        // Add cells for each day of the month
        const today = new Date();
        for (let i = 1; i <= daysInMonth; i++) {
            const dayDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), i);
            const isToday = dayDate.getDate() === today.getDate() &&
                dayDate.getMonth() === today.getMonth() &&
                dayDate.getFullYear() === today.getFullYear();
            const dayCell = createDayCell(dayDate, false, isToday);
            daysGrid.appendChild(dayCell);
        }

        // Add empty cells for days after the last day of the month
        const totalCells = Math.ceil((startingDay + daysInMonth) / 7) * 7;
        const remainingCells = totalCells - (startingDay + daysInMonth);
        for (let i = 1; i <= remainingCells; i++) {
            const nextMonthDay = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, i);
            const dayCell = createDayCell(nextMonthDay, true);
            daysGrid.appendChild(dayCell);
        }

        monthContainer.appendChild(daysGrid);
        calendarView.appendChild(monthContainer);
    }

    function createDayCell(date, isOtherMonth, isToday = false) {
        const dayCell = document.createElement('div');
        dayCell.className = `day-cell ${isOtherMonth ? 'other-month' : ''} ${isToday ? 'current-day' : ''}`;

        const dayNumber = document.createElement('div');
        dayNumber.className = 'day-number';
        dayNumber.textContent = date.getDate();
        dayCell.appendChild(dayNumber);

        const dayEventsContainer = document.createElement('div');
        dayEventsContainer.className = 'day-events';

        // Get events for this day
        const dayEvents = getEventsForDate(date);

        // --- NEW: Calculate Daily Summary ---
        let expenseTotal = 0;
        let incomeTotal = 0;
        let transactionCount = dayEvents.length;

        dayEvents.forEach(evt => {
            const val = Number(evt.amount) || 0;
            if (val < 0) {
                expenseTotal += val;
            } else {
                incomeTotal += val;
            }
        });

        // Create summary element if there are transactions
        if (transactionCount > 0) {
            const summaryDiv = document.createElement('div');
            summaryDiv.className = 'day-summary';
            summaryDiv.style.fontSize = '0.75rem';
            summaryDiv.style.marginTop = '2px';
            summaryDiv.style.fontWeight = 'bold';
            summaryDiv.style.display = 'flex';
            summaryDiv.style.flexDirection = 'column';
            summaryDiv.style.alignItems = 'center';
            
            // Display Expenses if any
            if (expenseTotal < 0) {
                const expEl = document.createElement('div');
                expEl.style.color = '#e74a3b'; // RED
                expEl.textContent = expenseTotal.toLocaleString('pl-PL', { style: 'currency', currency: 'PLN' });
                summaryDiv.appendChild(expEl);
            }

            // Display Incomes if any
            if (incomeTotal > 0) {
                const incEl = document.createElement('div');
                incEl.style.color = '#1cc88a'; // GREEN
                incEl.textContent = '+' + incomeTotal.toLocaleString('pl-PL', { style: 'currency', currency: 'PLN' });
                summaryDiv.appendChild(incEl);
            }
            
            const countEl = document.createElement('div');
            countEl.style.fontSize = '0.65rem';
            countEl.style.color = '#858796';
            countEl.textContent = `(${transactionCount} tr.)`;
            summaryDiv.appendChild(countEl);
            
            dayCell.appendChild(summaryDiv);
        }
        // ------------------------------------

        // Display up to 3 events (or 2 if one is multi-line)
        const maxEventsToShow = 3;
        let eventsShown = 0;
        let spaceUsed = 0;

        // In month view, we only show the summary, not individual events
        // The following block which rendered individual events is removed/commented out
        
        /* 
        for (const event of dayEvents) {
            // ... (original rendering logic)
        }
        */

        dayCell.appendChild(dayEventsContainer);

        dayCell.addEventListener('click', () => {
            if (isOtherMonth) {
                // Navigate to that month
                currentDate = new Date(date);
                if (currentView === 'month') {
                    renderCalendar();
                } else {
                    switchView('month');
                }
            } else {
                // Switch to day view for this date
                currentDate = new Date(date);
                switchView('day');
            }
        });

        return dayCell;
    }

    function createEventListItem(event, showDate = false) {
        const item = document.createElement('div');
        item.className = 'event-item-row';
        item.style.backgroundColor = '#fff';
        item.style.border = '1px solid #e0e0e0';
        item.style.borderRadius = '8px';
        item.style.padding = '15px';
        item.style.marginBottom = '10px';
        item.style.display = 'flex';
        item.style.alignItems = 'center';
        item.style.borderLeft = `5px solid ${event.color}`;
        item.style.cursor = 'pointer';
        item.style.boxShadow = '0 2px 4px rgba(0,0,0,0.05)';

        const timeString = formatTime(new Date(event.startTime));
        const dateString = showDate ? new Date(event.startTime).toLocaleDateString('en-US', { month: 'short', day: 'numeric' }) + ', ' : '';

        item.innerHTML = `
            <div style="flex: 1;">
                <div style="font-weight: bold; font-size: 1.1em; color: #333;">${event.title}</div>
                <div style="color: #666; font-size: 0.9em; margin-top: 4px;">
                    <i class="far fa-clock"></i> ${dateString}${timeString}
                </div>
                ${event.description ? `<div style="color: #888; font-size: 0.9em; margin-top: 4px;">${event.description}</div>` : ''}
            </div>
            <div style="font-weight: bold; color: ${event.color}; font-size: 1.1em;">
                ${Number(event.amount).toLocaleString('pl-PL', { style: 'currency', currency: 'PLN' })}
            </div>
        `;

        item.addEventListener('click', () => showEventDetails(event.id));
        return item;
    }

    function renderWeekView() {
        const weekContainer = document.createElement('div');
        weekContainer.className = 'week-view-list';
        weekContainer.style.padding = '20px';

        // Week header
        const startOfWeek = new Date(currentDate);
        startOfWeek.setDate(currentDate.getDate() - currentDate.getDay());
        const endOfWeek = new Date(startOfWeek);
        endOfWeek.setDate(startOfWeek.getDate() + 6);

        const header = document.createElement('div');
        header.className = 'week-header-title';
        header.style.marginBottom = '20px';
        header.innerHTML = `<h2>Week of ${startOfWeek.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })} - ${endOfWeek.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })}</h2>`;
        weekContainer.appendChild(header);

        // Fetch events for the whole week
        const weekEvents = [];
        for (let i = 0; i < 7; i++) {
            const dayDate = new Date(startOfWeek);
            dayDate.setDate(startOfWeek.getDate() + i);
            const dayEvents = getEventsForDate(dayDate);
            weekEvents.push(...dayEvents);
        }

        weekEvents.sort((a, b) => new Date(a.startTime) - new Date(b.startTime));

        if (weekEvents.length === 0) {
            weekContainer.innerHTML += '<p style="color: #888; text-align: center;">No transactions for this week.</p>';
        } else {
            weekEvents.forEach(event => {
                weekContainer.appendChild(createEventListItem(event, true));
            });
        }

        calendarView.appendChild(weekContainer);
    }

    function renderDayView() {
        const dayContainer = document.createElement('div');
        dayContainer.className = 'day-view-list';
        dayContainer.style.padding = '20px';

        // Day header
        const dayHeader = document.createElement('div');
        dayHeader.className = 'day-header-title';
        dayHeader.style.marginBottom = '20px';
        dayHeader.innerHTML = `<h2>${currentDate.toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' })}</h2>`;
        dayContainer.appendChild(dayHeader);

        // Get events for the day
        const dayEvents = getEventsForDate(currentDate);
        dayEvents.sort((a, b) => new Date(a.startTime) - new Date(b.startTime));

        if (dayEvents.length === 0) {
            dayContainer.innerHTML += '<p style="color: #888; text-align: center;">No transactions for this day.</p>';
        } else {
             dayEvents.forEach(event => {
                 dayContainer.appendChild(createEventListItem(event, false));
             });
        }
        
        calendarView.appendChild(dayContainer);
    }

    function renderEventsList() {
        eventsList.innerHTML = '';

        // Get upcoming events (today and future)
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const upcomingEvents = events
            .filter(event => new Date(event.startTime) >= today)
            .sort((a, b) => new Date(a.startTime) - new Date(b.startTime));

        if (upcomingEvents.length === 0) {
            const noEvents = document.createElement('div');
            noEvents.className = 'no-events';
            noEvents.textContent = 'No upcoming events. Add one!';
            eventsList.appendChild(noEvents);
            return;
        }

        upcomingEvents.forEach(event => {
            const eventElement = document.createElement('div');
            eventElement.className = 'event-item';
            eventElement.style.borderLeftColor = event.color;

            const startDate = new Date(event.startTime);
            const endDate = new Date(event.endTime);

            eventElement.innerHTML = `
                <div class="event-title">
                    <span>${event.title}</span>
                    <span style="color: ${event.color}">●</span>
                </div>
                <div class="event-time">${formatTime(startDate)}</div>
                ${event.description ? `<div class="event-description">${event.description}</div>` : ''}
            `;

            eventsList.appendChild(eventElement);

            eventElement.addEventListener('click', () => {
                showEventDetails(event.id);
            });
        });
    }

    function toLocalDateString(date) {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    function getEventsForDate(date) {
        const dateStr = toLocalDateString(date);
        return events.filter(event => {
            const eventDate = new Date(event.startTime);
            return toLocalDateString(eventDate) === dateStr;
        });
    }

    function getEventsForDateAndHour(date, hour) {
        const dateStr = toLocalDateString(date);
        return events.filter(event => {
            const eventDate = new Date(event.startTime);
            const eventHour = eventDate.getHours();
            return toLocalDateString(eventDate) === dateStr && eventHour === hour;
        });
    }

    function updateCurrentDateDisplay() {
        switch (currentView) {
            case 'day':
                currentDateElement.textContent = currentDate.toLocaleDateString('en-US', {
                    weekday: 'long',
                    month: 'long',
                    day: 'numeric',
                    year: 'numeric'
                });
                break;
            case 'week':
                const startOfWeek = new Date(currentDate);
                startOfWeek.setDate(currentDate.getDate() - currentDate.getDay());

                const endOfWeek = new Date(startOfWeek);
                endOfWeek.setDate(startOfWeek.getDate() + 6);

                currentDateElement.textContent = `
                    ${startOfWeek.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })} - 
                    ${endOfWeek.toLocaleDateString('en-US', {
                    month: endOfWeek.getMonth() !== startOfWeek.getMonth() ? 'short' : undefined,
                    day: 'numeric',
                    year: endOfWeek.getFullYear() !== startOfWeek.getFullYear() ? 'numeric' : undefined
                })}
                `;
                break;
            case 'month':
                currentDateElement.textContent = currentDate.toLocaleDateString('en-US', {
                    month: 'long',
                    year: 'numeric'
                });
                break;
        }
    }

    function switchView(view) {
        currentView = view;

        // Update active view button
        viewOptions.forEach(option => {
            option.classList.toggle('active', option.dataset.view === view);
        });

        renderCalendar();
    }

    function navigatePrevious() {
        switch (currentView) {
            case 'day':
                currentDate.setDate(currentDate.getDate() - 1);
                break;
            case 'week':
                currentDate.setDate(currentDate.getDate() - 7);
                break;
            case 'month':
                currentDate.setMonth(currentDate.getMonth() - 1);
                break;
        }
        renderCalendar();
    }

    function navigateNext() {
        switch (currentView) {
            case 'day':
                currentDate.setDate(currentDate.getDate() + 1);
                break;
            case 'week':
                currentDate.setDate(currentDate.getDate() + 7);
                break;
            case 'month':
                currentDate.setMonth(currentDate.getMonth() + 1);
                break;
        }
        renderCalendar();
    }

    function goToToday() {
        currentDate = new Date();
        renderCalendar();
    }

    function openEventModal() {
        // Reset form
        eventForm.reset();
        eventDateInput.valueAsDate = currentDate;
        
        // Default time (current time)
        const now = new Date();
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');
        eventStartTimeInput.value = `${hours}:${minutes}`;

        document.getElementById('form-error').textContent = '';
        
        // Trigger HTMX to load categories if empty or just rely on the hx-trigger="load"
        if (window.htmx) {
            htmx.process(document.getElementById('categories-loader'));
            // Manually trigger if needed, but hx-trigger="load" runs when element is present.
            // Since modal is hidden, we might need to trigger it.
            // Actually, best to just trigger a swap manually or ensure the div is re-processed
            htmx.trigger('#categories-loader', 'load');
        }

        // Show modal
        eventModal.style.display = 'flex';
    }

    function openEventModalWithTime(hour) {
        openEventModal();
        eventStartTimeInput.value = `${hour.toString().padStart(2, '0')}:00`;
    }

    function closeModals() {
        eventModal.style.display = 'none';
        eventDetailsModal.style.display = 'none';
    }

    async function saveEvent(e) {
        e.preventDefault();

        // Validate Category
        const categorySelect = document.querySelector('#target-container-for-select select');
        if (!categorySelect || !categorySelect.value || categorySelect.value === "Wybierz...") {
            document.getElementById('form-error').textContent = 'Wybierz kategorię!';
            return;
        }

        // Construct FormData for the endpoint
        const formData = new FormData(eventForm);
        // Ensure categoryId is set correctly (the select might have name="categoryId" or we append it)
        // The endpoint expects "categoryId"
        if (!formData.has('categoryId')) {
            formData.append('categoryId', categorySelect.value);
        }
        
        // The endpoint expects "transactionType" as 0 or 1. Radio buttons handle this.
        
        try {
            const response = await fetch('/new-transaction/add', {
                method: 'POST',
                body: formData
            });

            const text = await response.text();
            
            if (text.includes('success')) {
                // Reload events
                await fetchEvents();
                renderCalendar();
                renderEventsList();
                closeModals();
            } else {
                // Show error (extract from response div)
                const tempDiv = document.createElement('div');
                tempDiv.innerHTML = text;
                const errorDiv = tempDiv.querySelector('.error');
                document.getElementById('form-error').textContent = errorDiv ? errorDiv.textContent : 'Błąd podczas zapisywania.';
            }
        } catch (err) {
            console.error(err);
            document.getElementById('form-error').textContent = 'Błąd połączenia.';
        }
    }

    function showEventDetails(eventId) {
        const event = events.find(e => e.id === eventId);
        if (!event) return;

        selectedEventId = eventId;

        // Populate details
        detailsTitle.textContent = event.title;
        detailsDate.textContent = new Date(event.startTime).toLocaleDateString('en-US', {
            weekday: 'long',
            month: 'long',
            day: 'numeric',
            year: 'numeric'
        });

        // Removed end time display
        detailsTime.textContent = formatTime(new Date(event.startTime));
        detailsDescription.textContent = event.description || 'No description';

        // Show modal
        eventDetailsModal.style.display = 'flex';
    }

    function editEvent() {
        if (!selectedEventId) return;

        const event = events.find(e => e.id === selectedEventId);
        if (!event) return;

        // Populate form with event data
        const val = Number(event.amount);
        eventAmountInput.value = Math.abs(val).toFixed(2);
        
        // Radios
        const radios = document.getElementsByName('transactionType');
        if (radios.length > 0) {
            if (val < 0) radios[0].checked = true; // Expense
            else radios[1].checked = true; // Income
        }

        const dt = new Date(event.startTime);
        const yyyy = dt.getFullYear();
        const mm = String(dt.getMonth() + 1).padStart(2, '0');
        const dd = String(dt.getDate()).padStart(2, '0');
        eventDateInput.value = `${yyyy}-${mm}-${dd}`;

        const hh = String(dt.getHours()).padStart(2, '0');
        const min = String(dt.getMinutes()).padStart(2, '0');
        eventStartTimeInput.value = `${hh}:${min}`;

        eventDescriptionInput.value = event.description || '';
        
        // Category selection
        const categorySelect = document.querySelector('#target-container-for-select select');
        if (categorySelect && event.categoryId) {
             categorySelect.value = event.categoryId;
        }

        // Change form submit to update instead of create
        eventForm.onsubmit = async function (e) {
            e.preventDefault();

            const formData = new FormData(eventForm);
            if (!formData.has('categoryId') && categorySelect) {
                formData.append('categoryId', categorySelect.value);
            }

            try {
                const response = await fetch('/transactions?id=' + selectedEventId, {
                    method: 'PUT',
                    body: formData
                });

                if (response.ok) {
                    await fetchEvents(); // Reload all events
                    renderCalendar();
                    renderEventsList();
                    closeModals();
                } else {
                    alert('Failed to update event');
                }
            } catch (err) {
                console.error(err);
                alert('Error updating event');
            }

            // Reset form submit handler
            eventForm.onsubmit = saveEvent;
        };

        // Show edit modal
        closeModals();
        eventModal.style.display = 'flex';
    }

    async function deleteEvent() {
        if (!selectedEventId) return;

        if (confirm('Are you sure you want to delete this event?')) {
            try {
                const response = await fetch('/transactions?id=' + selectedEventId, {
                    method: 'DELETE'
                });

                if (response.ok) {
                     await fetchEvents();
                     renderCalendar();
                     renderEventsList();
                     closeModals();
                } else {
                    alert('Failed to delete event');
                }
            } catch (err) {
                console.error(err);
                alert('Error deleting event');
            }
        }
    }

    function saveEventsToStorage() {
        localStorage.setItem('events', JSON.stringify(events));
    }

    function setReminder(event) {
        const reminderTime = new Date(event.startTime);
        reminderTime.setMinutes(reminderTime.getMinutes() - 15); // 15 minutes before

        const now = new Date();
        const timeUntilReminder = reminderTime - now;

        if (timeUntilReminder > 0) {
            setTimeout(() => {
                showReminderNotification(event);
            }, timeUntilReminder);
        }
    }

    function showReminderNotification(event) {
        if (Notification.permission === 'granted') {
            new Notification(`Reminder: ${event.title}`, {
                body: `Your event starts at ${formatTime(new Date(event.startTime))}`,
                icon: 'https://cdn-icons-png.flaticon.com/512/3652/3652191.png'
            });
        } else if (Notification.permission !== 'denied') {
            Notification.requestPermission().then(permission => {
                if (permission === 'granted') {
                    showReminderNotification(event);
                }
            });
        }
    }

    // Helper functions
    function formatTime(date) {
        return date.toLocaleTimeString('en-US', {
            hour: 'numeric',
            minute: '2-digit',
            hour12: true
        });
    }

    function formatDateTime(startDate, endDate) {
        const isSameDay = startDate.toDateString() === endDate.toDateString();

        if (isSameDay) {
            return `${startDate.toLocaleDateString('en-US', {
                month: 'short',
                day: 'numeric'
            })} • ${formatTime(startDate)} - ${formatTime(endDate)}`;
        } else {
            return `${startDate.toLocaleDateString('en-US', {
                month: 'short',
                day: 'numeric'
            })} ${formatTime(startDate)} - ${endDate.toLocaleDateString('en-US', {
                month: 'short',
                day: 'numeric'
            })} ${formatTime(endDate)}`;
        }
    }

    // Request notification permission on page load
    if ('Notification' in window) {
        Notification.requestPermission();
    }
});