// eVote360 JavaScript

// Auto-hide alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function() {
    const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
    alerts.forEach(function(alert) {
        setTimeout(function() {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
});

// Form validation enhancement
(function() {
    'use strict';
    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach(function(form) {
        form.addEventListener('submit', function(event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
})();

// Confirm dialogs
function confirmAction(message) {
    return confirm(message || '¿Está seguro de realizar esta acción?');
}

// Format cedula input
function formatCedula(input) {
    let value = input.value.replace(/[^\d]/g, '');
    if (value.length >= 3) {
        value = value.substring(0, 3) + '-' + value.substring(3);
    }
    if (value.length >= 11) {
        value = value.substring(0, 11) + '-' + value.substring(11, 12);
    }
    input.value = value;
}

// Loading spinner
function showLoading() {
    const spinner = document.createElement('div');
    spinner.className = 'spinner-overlay';
    spinner.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Cargando...</span></div>';
    document.body.appendChild(spinner);
}

function hideLoading() {
    const spinner = document.querySelector('.spinner-overlay');
    if (spinner) {
        spinner.remove();
    }
}

console.log('eVote360 System Ready');
