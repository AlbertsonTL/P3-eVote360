// eVote360 JavaScript
// Sistema global de modals + utilidades de UI (mobile-first, sin alert()/confirm() nativos)

(function () {
    'use strict';

    // ---------------------------------------------------------------
    // Auto-hide de banners de alerta (TempData Success/Error) tras 6s
    // ---------------------------------------------------------------
    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.alert:not(.alert-permanent)').forEach(function (el) {
            setTimeout(function () {
                if (window.bootstrap) {
                    var instance = bootstrap.Alert.getOrCreateInstance(el);
                    instance.close();
                }
            }, 6000);
        });
    });

    // ---------------------------------------------------------------
    // Validación de formularios Bootstrap (needs-validation)
    // ---------------------------------------------------------------
    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.needs-validation').forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    });

    // ---------------------------------------------------------------
    // Formateo de cédula (mantenido de la versión anterior)
    // ---------------------------------------------------------------
    window.formatCedula = function (input) {
        let value = input.value.replace(/[^\d]/g, '');
        if (value.length >= 3) value = value.substring(0, 3) + '-' + value.substring(3);
        if (value.length >= 11) value = value.substring(0, 11) + '-' + value.substring(11, 12);
        input.value = value;
    };

    // ---------------------------------------------------------------
    // Spinner de pantalla completa (para operaciones asíncronas largas, ej. OCR)
    // ---------------------------------------------------------------
    window.showLoading = function (message) {
        hideLoading();
        const spinner = document.createElement('div');
        spinner.className = 'spinner-overlay';
        spinner.id = 'evoteGlobalSpinner';
        spinner.innerHTML =
            '<div class="text-center">' +
            '  <div class="spinner-border text-primary" role="status" style="width:3rem;height:3rem;"></div>' +
            '  <p class="mt-3 mb-0 fw-semibold" style="color: var(--secondary-color);">' + (message || 'Procesando...') + '</p>' +
            '</div>';
        document.body.appendChild(spinner);
    };

    window.hideLoading = function () {
        const spinner = document.getElementById('evoteGlobalSpinner');
        if (spinner) spinner.remove();
    };

    // ---------------------------------------------------------------
    // Estado "cargando" en un botón individual + bloqueo anti doble-submit
    // ---------------------------------------------------------------
    window.setButtonLoading = function (btn, isLoading) {
        if (!btn) return;
        if (isLoading) {
            btn.dataset.wasDisabled = btn.disabled ? '1' : '0';
            btn.classList.add('is-loading');
            btn.disabled = true;
        } else {
            btn.classList.remove('is-loading');
            btn.disabled = btn.dataset.wasDisabled === '1';
        }
    };

    // Evita doble submit en cualquier formulario de la aplicación
    document.addEventListener('submit', function (e) {
        const form = e.target;
        if (!(form instanceof HTMLFormElement)) return;
        if (form.dataset.noAutoLoading === 'true') return;
        if (form.dataset.submitting === 'true') {
            e.preventDefault();
            return;
        }
        form.dataset.submitting = 'true';
        const submitBtn = form.querySelector('[type="submit"]');
        if (submitBtn) window.setButtonLoading(submitBtn, true);
    }, true);

    // =================================================================
    // SISTEMA GLOBAL DE MODALS (reemplaza alert()/confirm() nativos)
    // =================================================================
    // Uso declarativo — cualquier <form> o <a> puede pedir confirmación
    // visual antes de ejecutar su acción, sin duplicar HTML:
    //
    //   <form method="post" asp-action="Toggle"
    //         data-confirm="true"
    //         data-confirm-title="Confirmar desactivación"
    //         data-confirm-message="¿Está seguro que desea desactivar este ciudadano?"
    //         data-confirm-consequence="Esta acción impedirá que participe en el proceso de votación."
    //         data-confirm-variant="danger"
    //         data-confirm-ok="Desactivar">
    //
    // El modal se encarga de: mostrar contexto, ofrecer Cancelar,
    // mostrar loading al confirmar y evitar doble clic.
    const EVoteModal = {
        el: null,
        pendingTarget: null,

        init: function () {
            this.el = document.getElementById('evoteConfirmModal');
            if (!this.el) return;
            this.bsModal = new bootstrap.Modal(this.el);
            this.confirmBtn = this.el.querySelector('[data-role="confirm-ok"]');
            this.iconEl = this.el.querySelector('[data-role="confirm-icon"]');
            this.titleEl = this.el.querySelector('[data-role="confirm-title"]');
            this.messageEl = this.el.querySelector('[data-role="confirm-message"]');
            this.consequenceEl = this.el.querySelector('[data-role="confirm-consequence"]');

            this.confirmBtn.addEventListener('click', function () {
                EVoteModal.proceed();
            });

            // Intercepta cualquier submit/click en elementos con data-confirm
            document.addEventListener('submit', function (e) {
                const form = e.target;
                if (!(form instanceof HTMLFormElement)) return;
                if (form.dataset.confirm !== 'true') return;
                if (form.dataset.confirmed === 'true') return; // ya confirmado, dejar pasar
                e.preventDefault();
                e.stopImmediatePropagation();
                form.dataset.submitting = 'false';
                EVoteModal.open(form);
            }, true);

            document.addEventListener('click', function (e) {
                const link = e.target.closest('a[data-confirm="true"]');
                if (!link) return;
                e.preventDefault();
                EVoteModal.open(link);
            });
        },

        open: function (target) {
            this.pendingTarget = target;
            const variant = target.dataset.confirmVariant || 'warning'; // warning | danger | success | info
            const iconClass = {
                danger: 'bi-exclamation-triangle-fill type-danger',
                warning: 'bi-exclamation-triangle-fill type-warning',
                success: 'bi-check-circle-fill type-success',
                info: 'bi-info-circle-fill type-info'
            }[variant] || 'bi-exclamation-triangle-fill type-warning';

            this.iconEl.className = 'evote-modal-icon ' + iconClass.split(' ')[1];
            this.iconEl.innerHTML = '<i class="bi ' + iconClass.split(' ')[0] + '"></i>';
            this.titleEl.textContent = target.dataset.confirmTitle || 'Confirmar acción';
            this.messageEl.textContent = target.dataset.confirmMessage || '¿Está seguro que desea realizar esta acción?';

            if (target.dataset.confirmConsequence) {
                this.consequenceEl.textContent = target.dataset.confirmConsequence;
                this.consequenceEl.classList.remove('d-none');
            } else {
                this.consequenceEl.classList.add('d-none');
            }

            this.confirmBtn.className = 'btn btn-' + (variant === 'warning' ? 'warning' : variant);
            this.confirmBtn.textContent = target.dataset.confirmOk || 'Confirmar';
            window.setButtonLoading(this.confirmBtn, false);

            this.bsModal.show();
        },

        proceed: function () {
            if (!this.pendingTarget) return;
            window.setButtonLoading(this.confirmBtn, true);
            const target = this.pendingTarget;

            // Pequeño delay para que el usuario perciba el feedback de carga
            // antes de navegar/enviar (evita sensación de doble clic sin respuesta).
            setTimeout(function () {
                if (target.tagName === 'FORM') {
                    target.dataset.confirmed = 'true';
                    if (typeof target.requestSubmit === 'function') {
                        target.requestSubmit();
                    } else {
                        target.submit();
                    }
                } else if (target.tagName === 'A') {
                    window.location.href = target.href;
                }
            }, 150);
        }
    };

    document.addEventListener('DOMContentLoaded', function () {
        EVoteModal.init();
    });

    window.EVoteModal = EVoteModal;

    console.log('eVote360 System Ready');
})();
