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

        // Event creation - Redirect to new transaction page
        addEventBtn.addEventListener('click', () => {
            window.location.href = '/new-transaction?returnUrl=' + encodeURIComponent('/calendar');
        });

        // Event details modal
        deleteEventBtn.addEventListener('click', deleteEvent);
        editEventBtn.addEventListener('click', editEvent);
        closeDetailsBtn.addEventListener('click', closeModals);
        closeBtns.forEach(btn => {
            btn.addEventListener('click', closeModals);
        });

        // Close buttons (X)
        closeBtns.forEach(btn => {
            btn.addEventListener('click', closeModals);
        });

        // Close modal when clicking outside
        window.addEventListener('click', (e) => {
            if (e.target === eventDetailsModal || e.target === eventModal) {
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
    // Generates a calendar day cell displaying the date and a financial summary (total income vs expenses) for that day.
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

        // Calculate Daily Summary
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

    // Renders a single event item for the sidebar list.
    function createEventListItem(event, showDate = false) {
        // Create a new DIV element for the list item
        const item = document.createElement('div');
        item.className = 'event-item-row';

        // Apply inline styles for the card appearance (white bg, border, shadow)
        item.style.backgroundColor = '#fff';
        item.style.border = '1px solid #e0e0e0';
        item.style.borderRadius = '8px';
        item.style.padding = '15px';
        item.style.marginBottom = '10px';

        // Use Flexbox to align content (details left, amount right)
        item.style.display = 'flex';
        item.style.alignItems = 'center';

        // Set the left border color based on event type (e.g., green for income, red for expense)
        item.style.borderLeft = `5px solid ${event.color}`;

        // Add pointer cursor only for non-recurring events (clickable)
        if(!event.isRecurring) {
            item.style.cursor = 'pointer';
        }
        item.style.boxShadow = '0 2px 4px rgba(0,0,0,0.05)';

        // Format time and optionally the date string (e.g., "Jan 20, ")
        const timeString = formatTime(new Date(event.startTime));
        const dateString = showDate ? new Date(event.startTime).toLocaleDateString('pl-PL', { month: 'short', day: 'numeric' }) + ', ' : '';

        // Add a "redo" icon if the event is recurring
        const recurringIcon = event.isRecurring ? '<i class="fas fa-redo-alt" style="margin-left: 8px; color: #858796;"></i>' : '';

        // Inject HTML structure: Title, Time/Date, Description, and Formatted Amount (PLN)
        item.innerHTML = `
            <div style="flex: 1;">
                <div style="font-weight: bold; font-size: 1.1em; color: #333;">${event.title}${recurringIcon}</div>
                <div style="color: #666; font-size: 0.9em; margin-top: 4px;">
                    <i class="far fa-clock"></i> ${dateString}${timeString}
                </div>
                ${event.description ? `<div style="color: #888; font-size: 0.9em; margin-top: 4px;">${event.description}</div>` : ''}
            </div>
            <div style="font-weight: bold; color: ${event.color}; font-size: 1.1em;">
                ${Number(event.amount).toLocaleString('pl-PL', { style: 'currency', currency: 'PLN' })}
            </div>
        `;

        if(!event.isRecurring) {
            item.addEventListener('click', () => showEventDetails(event.id));
        }
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
        header.innerHTML = `<h2>Tydzień ${startOfWeek.toLocaleDateString('pl-PL', { month: 'short', day: 'numeric' })} - ${endOfWeek.toLocaleDateString('pl-PL', { month: 'short', day: 'numeric', year: 'numeric' })}</h2>`;
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
            weekContainer.innerHTML += '<p style="color: #888; text-align: center;">Brak transakcji tego tygodnia</p>';
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
        dayHeader.innerHTML = `<h2>${currentDate.toLocaleDateString('pl-PL', { weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' })}</h2>`;
        dayContainer.appendChild(dayHeader);

        // Get events for the day
        const dayEvents = getEventsForDate(currentDate);
        dayEvents.sort((a, b) => new Date(a.startTime) - new Date(b.startTime));

        if (dayEvents.length === 0) {
            dayContainer.innerHTML += '<p style="color: #888; text-align: center;">Brak transakcji tego dnia.</p>';
        } else {
             dayEvents.forEach(event => {
                 dayContainer.appendChild(createEventListItem(event, false));
             });
        }
        
        calendarView.appendChild(dayContainer);
    }

    function renderEventsList() {
        eventsList.innerHTML = '';

        // 1. Get upcoming events (today and future)
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        // 2. Filter & Sort
        // We need ALL future recurring events to find the 'next' one for each type, 
        // regardless of how far in the future it is (e.g. quarterly bill in 2 months).
        const candidates = events
            .filter(event => {
                const eventDate = new Date(event.startTime);
                return event.isRecurring === true && eventDate >= today;
            })
            .sort((a, b) => new Date(a.startTime) - new Date(b.startTime));

        // 3. Deduplicate (Smart "Next Only" Logic)
        // Keep only the FIRST occurrence of each event ID.
        // Since list is sorted by date, the first one found is the nearest one.
        const uniqueUpcoming = [];
        const seenIds = new Set();

        candidates.forEach(event => {
            if (!seenIds.has(event.id)) {
                seenIds.add(event.id);
                uniqueUpcoming.push(event);
            }
        });

        // 4. Render
        if (uniqueUpcoming.length === 0) {
            const noEvents = document.createElement('div');
            noEvents.className = 'no-events';
            noEvents.textContent = 'Brak nadchodzących transakcji cyklicznych';
            eventsList.appendChild(noEvents);
            return;
        }
        // Renders the dashboard interface with a navigation sidebar and a direct SQL console for database management.
        uniqueUpcoming.forEach(event => {
            // Create card element
            const eventElement = document.createElement('div');
            eventElement.className = 'event-item';
            eventElement.style.borderLeftColor = event.color;

            // Format date and currency
            const startDate = new Date(event.startTime);
            const amountVal = Number(event.amount);
            const amountStr = amountVal.toLocaleString('pl-PL', { style: 'currency', currency: 'PLN' });
            
            // Set amount color (expense vs income)
            const amountColor = amountVal < 0 ? '#e74a3b' : '#1cc88a';

            eventElement.innerHTML = `
                <div style="display:flex; justify-content:space-between; align-items: flex-start; width:100%;">
                    <div class="event-info">
                        <div class="event-title" style="font-weight:600;">${event.title}</div>
                        <div class="event-time" style="color:#858796; font-size:0.85em;">
                            <i class="far fa-calendar"></i> ${startDate.toLocaleDateString('pl-PL')}
                        </div>
                    </div>
                    <div class="event-amount" style="color:${amountColor}; font-weight:bold; white-space:nowrap;">
                        ${amountStr}
                    </div>
                </div>
                ${event.description ? `<div class="event-description" style="margin-top:4px; font-size:0.8em; color:#aaa;">${event.description}</div>` : ''}
            `;

            eventsList.appendChild(eventElement);

            eventElement.addEventListener('click', () => {
                showEventDetails(event.id);
            });
        });
    }

    // Converts a Date object into a standard YYYY-MM-DD string format.
    function toLocalDateString(date) {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }
    // Filters and returns all events occurring on the specified date.
    function getEventsForDate(date) {
        const dateStr = toLocalDateString(date);
        return events.filter(event => {
            const eventDate = new Date(event.startTime);
            return toLocalDateString(eventDate) === dateStr;
        });
    }
    

    // Updates the header text to show the current date or range based on the active view.
    function updateCurrentDateDisplay() {
        switch (currentView) {
            case 'day':
                currentDateElement.textContent = currentDate.toLocaleDateString('pl-PL', {
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
                    ${startOfWeek.toLocaleDateString('pl-PL', { month: 'short', day: 'numeric' })} - 
                    ${endOfWeek.toLocaleDateString('pl-PL', {
                    month: endOfWeek.getMonth() !== startOfWeek.getMonth() ? 'short' : undefined,
                    day: 'numeric',
                    year: endOfWeek.getFullYear() !== startOfWeek.getFullYear() ? 'numeric' : undefined
                })}
                `;
                break;
            case 'month':
                currentDateElement.textContent = currentDate.toLocaleDateString('pl-PL', {
                    month: 'long',
                    year: 'numeric'
                });
                break;
        }
    }
    // Changes the calendar view mode (Day/Week/Month) and triggers a re-render.
    function switchView(view) {
        currentView = view;

        // Update active view button
        viewOptions.forEach(option => {
            option.classList.toggle('active', option.dataset.view === view);
        });

        renderCalendar();
    }

    // Moves the calendar back by one day, week, or month depending on the current view.
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

    // Moves the calendar forward by one day, week, or month depending on the current view.
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

    // Resets the calendar focus to the current date.
    function goToToday() {
        currentDate = new Date();
        renderCalendar();
    }
    // Hides all active modal windows (details and edit forms).
    function closeModals() {
        if (eventDetailsModal) eventDetailsModal.style.display = 'none';
        if (eventModal) eventModal.style.display = 'none';
    }
    // Populates and displays the modal with details for a specific selected transaction.
    function showEventDetails(eventId) {
        const event = events.find(e => e.id === eventId);
        if (!event) return;

        selectedEventId = eventId;

        // Populate details
        detailsTitle.textContent = event.title;
        detailsDate.textContent = new Date(event.startTime).toLocaleDateString('pl-PL', {
            weekday: 'long',
            month: 'long',
            day: 'numeric',
            year: 'numeric'
        });

        // Removed end time display
        detailsTime.textContent = formatTime(new Date(event.startTime));
        detailsDescription.textContent = event.description || 'Brak opisu';

        if (event.isRecurring) {
            editEventBtn.style.display = 'none';
            deleteEventBtn.style.display = 'none';
        } else {
            editEventBtn.style.display = 'inline-block';
            deleteEventBtn.style.display = 'inline-block';
        }

        // Show modal
        eventDetailsModal.style.display = 'flex';
    }
    // Pre-fills the form with transaction data and handles the update logic via API.
    function editEvent() {
        console.log('Edit event clicked', selectedEventId);
        if (!selectedEventId) return;

        const event = events.find(e => e.id === selectedEventId);
        if (!event) return;

        // Populate form with event data
        const val = Number(event.amount);
        const isExpense = val < 0;

        // Elements
        const titleInput = document.getElementById('event-title');
        const typeSelect = document.getElementById('event-type');
        const amountInput = document.getElementById('event-amount');
        const dateInput = document.getElementById('event-date');
        const descInput = document.getElementById('event-description');

        if (titleInput) titleInput.value = event.title || "";
        if (typeSelect) typeSelect.value = isExpense ? "0" : "1";
        if (amountInput) amountInput.value = Math.abs(val).toFixed(2);
        
        const dt = new Date(event.startTime);
        const yyyy = dt.getFullYear();
        const mm = String(dt.getMonth() + 1).padStart(2, '0');
        const dd = String(dt.getDate()).padStart(2, '0');
        if (dateInput) dateInput.value = `${yyyy}-${mm}-${dd}`;

        if (descInput) descInput.value = event.description || '';

        // Change form submit to update instead of create
        if (eventForm) {
            eventForm.onsubmit = async function (e) {
                e.preventDefault();
                console.log('Submitting calendar edit form');

                const formData = new FormData();
                if (titleInput) formData.append('title', titleInput.value);
                if (typeSelect) formData.append('transactionType', typeSelect.value);
                if (amountInput) formData.append('amount', amountInput.value);
                if (dateInput) formData.append('date', dateInput.value);
                if (descInput) formData.append('description', descInput.value);

                try {
                    const response = await fetch('/transactions?id=' + selectedEventId, {
                        method: 'PUT',
                        body: formData
                    });

                                    if (response.ok) {
                                        console.log('Update successful');
                                        await fetchEvents(); // Reload all events
                                        renderCalendar();
                                        renderEventsList();
                                        closeModals();
                                    } else {
                                        console.error('Failed to update event', response);
                                        alert('Błąd podczas aktualizacji transakcji');
                                    }
                                } catch (err) {
                                    console.error('Error updating event:', err);
                                    alert('Wystąpił błąd podczas aktualizacji');
                                }
                            };
                        }
                    
                        // Show edit modal
                        closeModals();
                        if (eventModal) eventModal.style.display = 'flex';
                    }
                    // Sends a request to delete the selected transaction after user confirmation.
                    async function deleteEvent() {
                        if (!selectedEventId) return;
                    
                        if (confirm('Czy na pewno chcesz usunąć tę transakcję?')) {
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
                                    alert('Błąd podczas usuwania transakcji');
                                }
                            } catch (err) {
                                console.error(err);
                                alert('Wystąpił błąd podczas usuwania');
                            }
                        }
                    }
   


    // Helper functions
    function formatTime(date) {
        return date.toLocaleTimeString('pl-PL', {
            hour: 'numeric',
            minute: '2-digit',
            hour12: true
        });
    }

    
    // Request notification permission on page load
    if ('Notification' in window) {
        Notification.requestPermission();
    }
});