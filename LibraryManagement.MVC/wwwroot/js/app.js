/**
 * app.js – Library Management System
 * Global UI helpers: Toast, SweetAlert confirms, Spinner, Active Nav, Utilities.
 */

// ─── SweetAlert2 CDN Loader ────────────────────────────────────────────────────
// Dynamically load SweetAlert2 if not present
(function loadSweetAlert() {
  if (typeof Swal !== 'undefined') return;
  const s = document.createElement('script');
  s.src = 'https://cdn.jsdelivr.net/npm/sweetalert2@11';
  document.head.appendChild(s);
})();

// ─── Global Page Spinner ──────────────────────────────────────────────────────
const Spinner = {
  el: null,

  init() {
    let el = document.getElementById('global-spinner');
    if (!el) {
      el = document.createElement('div');
      el.id = 'global-spinner';
      el.className = 'hidden';
      el.innerHTML = `<div class="spinner-ring"></div><p>Loading…</p>`;
      document.body.prepend(el);
    }
    this.el = el;
  },

  show(msg = 'Loading…') {
    if (!this.el) this.init();
    this.el.querySelector('p').textContent = msg;
    this.el.classList.remove('hidden');
  },

  hide() {
    if (this.el) this.el.classList.add('hidden');
  }
};

// ─── Toast Notifications ──────────────────────────────────────────────────────

/**
 * Show a toast notification.
 * @param {string} message
 * @param {'success'|'danger'|'warning'|'info'} type
 */
function showToast(message, type = 'success') {
  // Ensure container exists
  let container = document.getElementById('toast-container');
  if (!container) {
    container = document.createElement('div');
    container.id = 'toast-container';
    document.body.appendChild(container);
  }

  const id = `toast-${Date.now()}`;
  const icons = {
    success: 'bi-check-circle-fill',
    danger:  'bi-x-circle-fill',
    warning: 'bi-exclamation-triangle-fill',
    info:    'bi-info-circle-fill'
  };

  const bgMap = {
    success: 'rgba(16,185,129,0.95)',
    danger:  'rgba(220,38,38,0.95)',
    warning: 'rgba(217,119,6,0.95)',
    info:    'rgba(6,182,212,0.95)'
  };

  const html = `
    <div id="${id}" style="
        background:${bgMap[type] || bgMap.info};
        color:#fff;
        padding:0.9rem 1.2rem;
        border-radius:10px;
        display:flex;
        align-items:center;
        gap:0.75rem;
        box-shadow:0 8px 24px rgba(0,0,0,0.4);
        font-size:0.875rem;
        font-weight:500;
        animation:slideInRight 0.3s ease;
        cursor:pointer;
        min-width:280px;
      "
      onclick="this.remove()"
    >
      <i class="bi ${icons[type] || icons.info}" style="font-size:1.2rem;flex-shrink:0;"></i>
      <span style="flex-grow:1;">${message}</span>
      <i class="bi bi-x" style="font-size:1.1rem;opacity:0.75;"></i>
    </div>`;

  container.insertAdjacentHTML('beforeend', html);

  setTimeout(() => {
    const el = document.getElementById(id);
    if (el) {
      el.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
      el.style.opacity = '0';
      el.style.transform = 'translateX(20px)';
      setTimeout(() => el.remove(), 320);
    }
  }, 4000);
}

// ─── SweetAlert Delete Confirmation ──────────────────────────────────────────

/**
 * Intercept all Delete anchor/buttons to show a SweetAlert confirmation.
 * Call once on DOMContentLoaded.
 */
function initDeleteConfirmations() {
  document.querySelectorAll('[data-confirm-delete]').forEach(el => {
    el.addEventListener('click', function (e) {
      e.preventDefault();
      const href   = this.getAttribute('href') || this.dataset.href;
      const name   = this.dataset.name   || 'this item';
      const entity = this.dataset.entity || 'record';

      if (typeof Swal === 'undefined') {
        // Fallback if SweetAlert hasn't loaded
        if (confirm(`Delete ${entity}: "${name}"? This cannot be undone.`)) {
          window.location.href = href;
        }
        return;
      }

      Swal.fire({
        title: 'Delete ' + entity + '?',
        html: `You are about to permanently delete <strong>"${name}"</strong>.<br>This action <strong>cannot be undone</strong>.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: '<i class="bi bi-trash3 me-1"></i>Yes, Delete',
        cancelButtonText: '<i class="bi bi-x me-1"></i>Cancel',
        confirmButtonColor: '#dc2626',
        cancelButtonColor: '#334155',
        background: '#0f2744',
        color: '#f1f5f9',
        iconColor: '#ef4444',
        customClass: {
          popup:         'swal-lms-popup',
          confirmButton: 'btn btn-danger me-2',
          cancelButton:  'btn btn-secondary',
          actions:       'gap-2 mt-2',
        },
        buttonsStyling: false,
        reverseButtons: true,
      }).then(result => {
        if (result.isConfirmed) {
          Spinner.show('Deleting…');
          window.location.href = href;
        }
      });
    });
  });
}

/**
 * Attach submit spinner to all forms with data-loading attribute
 */
function initFormSpinners() {
  document.querySelectorAll('form[data-loading]').forEach(form => {
    form.addEventListener('submit', function () {
      const msg = this.dataset.loading || 'Saving…';
      Spinner.show(msg);
    });
  });
}

// ─── Active Nav Link ──────────────────────────────────────────────────────────
function setActiveNav() {
  const currentPath = window.location.pathname.toLowerCase();
  document.querySelectorAll('.sidebar .sidebar-link').forEach(link => {
    const href = (link.getAttribute('href') || '').toLowerCase();
    if (href && href !== '/' && href !== '#' && currentPath.startsWith(href)) {
      link.classList.add('active');
      // Open parent collapse if it has one
      const parentMenu = link.closest('.sidebar-submenu');
      if (parentMenu) {
        try {
          const collapse = new bootstrap.Collapse(parentMenu, { toggle: false });
          collapse.show();
          const trigger = document.querySelector(`[aria-controls="${parentMenu.id}"]`);
          if (trigger) trigger.setAttribute('aria-expanded', 'true');
        } catch(e) { /* ignore */ }
      }
    }
  });
}

// ─── URL Query Helpers ────────────────────────────────────────────────────────
function getParam(name) {
  return new URLSearchParams(window.location.search).get(name);
}

function goTo(page, id) {
  window.location.href = id !== undefined ? `${page}?id=${id}` : page;
}

// ─── Date Helpers ─────────────────────────────────────────────────────────────
function formatDate(iso) {
  if (!iso) return '—';
  const [year, month, day] = iso.split('-');
  const months = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
  return `${months[parseInt(month, 10) - 1]} ${parseInt(day, 10)}, ${year}`;
}

// ─── Badge Helper ─────────────────────────────────────────────────────────────
function availabilityBadge(available) {
  return available
    ? `<span class="badge badge-available"><i class="bi bi-check-circle-fill me-1"></i>Available</span>`
    : `<span class="badge badge-borrowed"><i class="bi bi-x-circle-fill me-1"></i>Borrowed</span>`;
}

// ─── Form Validation ──────────────────────────────────────────────────────────
function validateForm(formEl) {
  formEl.classList.add('was-validated');
  return formEl.checkValidity();
}

// ─── Check for TempData Success Message ───────────────────────────────────────
function checkTempDataToast() {
  const el = document.getElementById('temp-data-toast');
  if (el) {
    const msg  = el.dataset.message;
    const type = el.dataset.type || 'success';
    if (msg) showToast(msg, type);
    el.remove();
  }
}

// ─── DOMContentLoaded Bootstrap ──────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
  Spinner.init();
  setActiveNav();
  initDeleteConfirmations();
  initFormSpinners();
  checkTempDataToast();

  // Sidebar mobile toggle
  const sidebar   = document.getElementById('mainSidebar');
  const overlay   = document.getElementById('sidebarOverlay');
  const toggleBtn = document.getElementById('sidebarToggle');

  if (toggleBtn && sidebar) {
    toggleBtn.addEventListener('click', () => {
      sidebar.classList.toggle('show');
      if (overlay) overlay.classList.toggle('show');
    });
  }

  if (overlay) {
    overlay.addEventListener('click', () => {
      if (sidebar) sidebar.classList.remove('show');
      overlay.classList.remove('show');
    });
  }

  // Stat card hover lift animation
  document.querySelectorAll('.stat-card').forEach(card => {
    card.addEventListener('mouseenter', () => card.style.transform = 'translateY(-4px)');
    card.addEventListener('mouseleave', () => card.style.transform = 'translateY(0)');
  });

  // Show page spinner on full-page link clicks (optional, skip ajax)
  document.querySelectorAll('a[data-page-load]').forEach(link => {
    link.addEventListener('click', () => Spinner.show());
  });
});

// SweetAlert dark popup overrides via global style injection
const swalStyle = document.createElement('style');
swalStyle.textContent = `
  .swal2-popup.swal-lms-popup {
    border: 1px solid #1e3a5f !important;
    border-radius: 14px !important;
  }
  .swal2-popup.swal-lms-popup .swal2-title { font-weight: 800; }
  .swal2-popup.swal-lms-popup .swal2-html-container { color: #94a3b8; }
  .swal2-popup.swal-lms-popup .swal2-actions { gap: 0.5rem; }
`;
document.head.appendChild(swalStyle);
