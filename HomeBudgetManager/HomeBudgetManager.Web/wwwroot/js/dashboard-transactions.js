let selectedTransactionId = null;

function openDashboardTransactionDetails(id, amount, description, date) {
    selectedTransactionId = id;
    
    // Elements
    const detailsModal = document.getElementById('event-details-modal');
    const detailsTitle = document.getElementById('details-title');
    const detailsDate = document.getElementById('details-date');
    const detailsTime = document.getElementById('details-time'); // We might not use time or just hide it
    const detailsDescription = document.getElementById('details-description');

    // Populate
    detailsTitle.textContent = parseFloat(amount).toLocaleString('pl-PL', { style: 'currency', currency: 'PLN' });
    detailsDate.textContent = date;
    detailsDescription.textContent = description || 'Brak opisu';
    
    // Store raw data for edit
    detailsModal.dataset.amount = amount;
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
    const detailsModal = document.getElementById('event-details-modal');
    const editModal = document.getElementById('event-modal');
    
    const amount = detailsModal.dataset.amount;
    const description = detailsModal.dataset.description;
    const date = detailsModal.dataset.date;

    // Populate Edit Form
    document.getElementById('event-amount').value = amount;
    document.getElementById('event-description').value = description;
    document.getElementById('event-date').value = date;
    
    // Hide details, show edit
    detailsModal.style.display = 'none';
    editModal.style.display = 'flex';
    
    // Hook up save
    const form = document.getElementById('event-form');
    form.onsubmit = async function(e) {
        e.preventDefault();
        
        const formData = new FormData();
        formData.append('amount', document.getElementById('event-amount').value);
        formData.append('description', document.getElementById('event-description').value);
        formData.append('date', document.getElementById('event-date').value);

        try {
            const response = await fetch('/transactions?id=' + selectedTransactionId, {
                method: 'PUT',
                body: formData
            });

            if (response.ok) {
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
                alert('Błąd aktualizacji transakcji');
            }
        } catch (err) {
            console.error(err);
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
