/**
 * app.js
 * Shared utility helpers used across all pages.
 */

// ─── URL Query Helpers ────────────────────────────────────────────────────────

/** Read a single query parameter from the current URL. */
function getParam(name) {
  return new URLSearchParams(window.location.search).get(name);
}

/** Navigate to a page, optionally appending ?id=<value>. */
function goTo(page, id) {
  window.location.href = id !== undefined ? `${page}?id=${id}` : page;
}

// ─── Date Helpers ─────────────────────────────────────────────────────────────

/** Format an ISO date string (YYYY-MM-DD) into a human-readable form. */
function formatDate(iso) {
  if (!iso) return "—";
  const [year, month, day] = iso.split("-");
  const months = [
    "Jan", "Feb", "Mar", "Apr", "May", "Jun",
    "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
  ];
  return `${months[parseInt(month, 10) - 1]} ${parseInt(day, 10)}, ${year}`;
}

// ─── Badge Helper ─────────────────────────────────────────────────────────────

/**
 * Return a Bootstrap badge HTML string for availability status.
 * @param {boolean} available
 */
function availabilityBadge(available) {
  return available
    ? `<span class="badge badge-available"><i class="bi bi-check-circle-fill me-1"></i>Available</span>`
    : `<span class="badge badge-borrowed"><i class="bi bi-x-circle-fill me-1"></i>Borrowed</span>`;
}

// ─── Toast Notifications ──────────────────────────────────────────────────────

/**
 * Show a lightweight toast notification.
 * @param {string} message  - Text to display.
 * @param {'success'|'danger'|'warning'|'info'} type
 */
function showToast(message, type = "success") {
  const container = document.getElementById("toast-container");
  if (!container) return;

  const id = `toast-${Date.now()}`;
  const icons = {
    success: "bi-check-circle-fill",
    danger: "bi-x-circle-fill",
    warning: "bi-exclamation-triangle-fill",
    info: "bi-info-circle-fill"
  };

  const html = `
    <div id="${id}" class="toast align-items-center text-bg-${type} border-0 show" role="alert" aria-live="assertive" aria-atomic="true">
      <div class="d-flex">
        <div class="toast-body">
          <i class="bi ${icons[type] || icons.info} me-2"></i>${message}
        </div>
        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
      </div>
    </div>`;

  container.insertAdjacentHTML("beforeend", html);

  // Auto-remove after 3.5 seconds
  setTimeout(() => {
    const el = document.getElementById(id);
    if (el) el.remove();
  }, 3500);
}

// ─── Active Nav Link ──────────────────────────────────────────────────────────

/**
 * Highlight the navbar link that corresponds to the current page.
 * Call this on DOMContentLoaded in each page script.
 */
function setActiveNav() {
  const currentPage = window.location.pathname.split("/").pop() || "index.html";
  document.querySelectorAll(".navbar-nav .nav-link, .sidebar .sidebar-link").forEach(link => {
    const href = link.getAttribute("href") || "";
    // Remove active class first
    link.classList.remove("active");
    if (href === currentPage || (currentPage === "" && href === "index.html")) {
      link.classList.add("active");
    }
  });
}

// ─── Form Validation Helper ───────────────────────────────────────────────────

/**
 * Trigger Bootstrap's native validation on a form.
 * Returns true only if the form passes all constraints.
 */
function validateForm(formEl) {
  formEl.classList.add("was-validated");
  return formEl.checkValidity();
}

// ─── Stats (Dashboard) ───────────────────────────────────────────────────────

/** Compute summary stats from the books array. */
function computeStats(books) {
  const total = books.length;
  const available = books.filter(b => b.available).length;
  const borrowed = total - available;
  
  const today = new Date().toISOString().split("T")[0];
  const history = JSON.parse(localStorage.getItem("lms_history")) || [];
  
  const todayBorrowed = history.filter(h => h.date === today && h.action === "borrow").length;
  const todayReturned = history.filter(h => h.date === today && h.action === "return").length;
  
  return { total, available, borrowed, todayBorrowed, todayReturned };
}
