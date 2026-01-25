let selectedTransactionId = null;

function openDashboardTransactionDetails(id, amount, title, description, date) {
    selectedTransactionId = id;
    
    // Elements
    const detailsModal = document.getElementById('event-details-modal');
    const detailsTitle = document.getElementById('details-title');
    const detailsAmount = document.getElementById('details-amount');
    const detailsDate = document.getElementById('details-date');
    const detailsDescription = document.getElementById('details-description');

    // Populate
    detailsTitle.textContent = title || 'Bez tytułu';
    detailsAmount.textContent = parseFloat(amount).toLocaleString('pl-PL', { style: 'currency', currency: 'PLN' });
    
    // Color amount
    if (parseFloat(amount) < 0) {
        detailsAmount.style.color = '#e74a3b';
    } else {
        detailsAmount.style.color = '#1cc88a';
    }

    detailsDate.textContent = date;
    detailsDescription.textContent = description || 'Brak opisu';
    
    // Store raw data for edit
    detailsModal.dataset.amount = amount;
    detailsModal.dataset.title = title;
    detailsModal.dataset.description = description;
    detailsModal.dataset.date = date;

    // Show
    detailsModal.style.display = 'flex';
}

function closeDashboardModals() {
    document.getElementById('event-details-modal').style.display = 'none';
    document.getElementById('event-modal').style.display = 'none';
}

function editDashboardTransaction() {
    console.log('Edit button clicked for transaction:', selectedTransactionId);
    const detailsModal = document.getElementById('event-details-modal');
    const editModal = document.getElementById('event-modal');
    
    if (!detailsModal || !editModal) {
        console.error('Modals not found!');
        return;
    }

    const amount = detailsModal.dataset.amount;
    const title = detailsModal.dataset.title;
    const description = detailsModal.dataset.description;
    const date = detailsModal.dataset.date;

    console.log('Editing data:', { amount, title, description, date });

    // Populate Edit Form
    // Determine type based on sign
    let val = parseFloat(amount);
    if (isNaN(val)) val = 0;

    const isExpense = val < 0;
    
    const typeSelect = document.getElementById('event-type');
    const amountInput = document.getElementById('event-amount');
    const titleInput = document.getElementById('event-title');
    const descInput = document.getElementById('event-description');
    const dateInput = document.getElementById('event-date');

    if (typeSelect) typeSelect.value = isExpense ? "0" : "1";
    if (amountInput) amountInput.value = Math.abs(val); // Show positive value in input
    if (titleInput) titleInput.value = title || "";
    if (descInput) descInput.value = description || "";
    if (dateInput) dateInput.value = date || "";
    
    // Hide details, show edit
    detailsModal.style.display = 'none';
    editModal.style.display = 'flex';
    
    // Hook up save
    const form = document.getElementById('event-form');
    if (!form) {
        console.error('Event form not found!');
        return;
    }

    form.onsubmit = async function(e) {
        e.preventDefault();
        console.log('Submitting edit form...');
        
        const formData = new FormData();
        if (typeSelect) formData.append('transactionType', typeSelect.value);
        if (amountInput) formData.append('amount', amountInput.value);
        if (titleInput) formData.append('title', titleInput.value);
        if (descInput) formData.append('description', descInput.value);
        if (dateInput) formData.append('date', dateInput.value);

        try {
            const response = await fetch('/transactions?id=' + selectedTransactionId, {
                method: 'PUT',
                body: formData
            });

            if (response.ok) {
                console.log('Update successful');
                closeDashboardModals();
                // Reload transactions list via HTMX manually or just reload page/part
                // HTMX trigger
                if (window.htmx) {
                    htmx.trigger('#transactions-list', 'load');
                    // Also refresh balance
                    location.reload(); // Simplest way to refresh everything including balance
                } else {
                    location.reload();
                }
            } else {
                console.error('Update failed', response);
                alert('Błąd aktualizacji transakcji');
            }
        } catch (err) {
            console.error('Error submitting form:', err);
            alert('Wystąpił błąd');
        }
    };
}

async function deleteDashboardTransaction() {
    if (!selectedTransactionId) return;

    if (confirm('Czy na pewno chcesz usunąć tę transakcję?')) {
        try {
            const response = await fetch('/transactions?id=' + selectedTransactionId, {
                method: 'DELETE'
            });

            if (response.ok) {
                closeDashboardModals();
                if (window.htmx) {
                    htmx.trigger('#transactions-list', 'load');
                    location.reload(); 
                } else {
                    location.reload();
                }
            } else {
                alert('Błąd usuwania transakcji');
            }
        } catch (err) {
            console.error(err);
            alert('Wystąpił błąd');
        }
    }
}

document.addEventListener('DOMContentLoaded', () => {
    // Setup close buttons
    document.querySelectorAll('.close-btn').forEach(btn => {
        btn.addEventListener('click', closeDashboardModals);
    });
    
    document.getElementById('edit-event-btn').addEventListener('click', editDashboardTransaction);
    document.getElementById('delete-event-btn').addEventListener('click', deleteDashboardTransaction);
    document.getElementById('close-details-btn').addEventListener('click', closeDashboardModals);
    
    // Close on outside click
    window.addEventListener('click', (e) => {
        if (e.target.classList.contains('modal')) {
            closeDashboardModals();
        }
    });
});
