// ===== GAMESHOP - JAVASCRIPT =====

// Inicjalizacja po załadowaniu strony
document.addEventListener('DOMContentLoaded', function() {
    initAnimations();
    initTooltips();
    initFormValidation();
    initTableSearch();
    initAlerts();
});

// ===== ANIMACJE =====
function initAnimations() {
    // Dodaj animacje fade-in do kart
    const cards = document.querySelectorAll('.card, .game-card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        setTimeout(() => {
            card.style.transition = 'all 0.5s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 50);
    });

    // Animuj wiersze tabeli
    const rows = document.querySelectorAll('tbody tr');
    rows.forEach((row, index) => {
        row.style.opacity = '0';
        setTimeout(() => {
            row.style.transition = 'opacity 0.3s ease';
            row.style.opacity = '1';
        }, index * 30);
    });
}

// ===== TOOLTIPS =====
function initTooltips() {
    // Dodaj tooltips do przycisków
    const buttons = document.querySelectorAll('[title]');
    buttons.forEach(button => {
        button.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px) scale(1.05)';
        });
        button.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });
}

// ===== WALIDACJA FORMULARZY =====
function initFormValidation() {
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            const submitBtn = form.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Przetwarzanie...';
                
                // Przywróć po 3 sekundach (zabezpieczenie)
                setTimeout(() => {
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = submitBtn.getAttribute('data-original-text') || 'Wyślij';
                }, 3000);
            }
        });

        // Zapisz oryginalny tekst przycisku
        const submitBtn = form.querySelector('button[type="submit"]');
        if (submitBtn && !submitBtn.hasAttribute('data-original-text')) {
            submitBtn.setAttribute('data-original-text', submitBtn.innerHTML);
        }
    });

    // Real-time walidacja pól email
    const emailInputs = document.querySelectorAll('input[type="email"]');
    emailInputs.forEach(input => {
        input.addEventListener('blur', function() {
            if (this.value && !isValidEmail(this.value)) {
                this.classList.add('is-invalid');
                showFieldError(this, 'Wprowadź poprawny adres email');
            } else {
                this.classList.remove('is-invalid');
                hideFieldError(this);
            }
        });
    });

    // Walidacja pól wymaganych
    const requiredInputs = document.querySelectorAll('input[required], select[required], textarea[required]');
    requiredInputs.forEach(input => {
        input.addEventListener('blur', function() {
            if (!this.value.trim()) {
                this.classList.add('is-invalid');
                showFieldError(this, 'To pole jest wymagane');
            } else {
                this.classList.remove('is-invalid');
                hideFieldError(this);
            }
        });
    });
}

function isValidEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function showFieldError(field, message) {
    let errorDiv = field.parentElement.querySelector('.invalid-feedback');
    if (!errorDiv) {
        errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback';
        field.parentElement.appendChild(errorDiv);
    }
    errorDiv.textContent = message;
}

function hideFieldError(field) {
    const errorDiv = field.parentElement.querySelector('.invalid-feedback');
    if (errorDiv) {
        errorDiv.remove();
    }
}

// ===== WYSZUKIWANIE W TABELI =====
function initTableSearch() {
    // Dodaj pole wyszukiwania do tabel
    const tables = document.querySelectorAll('.table');
    tables.forEach(table => {
        if (!table.id) return; // Pomiń tabele bez ID
        
        const tableContainer = table.parentElement;
        const searchExists = tableContainer.querySelector('.table-search');
        
        if (!searchExists) {
            const searchDiv = document.createElement('div');
            searchDiv.className = 'table-search mb-3';
            searchDiv.innerHTML = `
                <input type="text" class="form-control" placeholder="🔍 Szukaj w tabeli..." 
                       onkeyup="filterTable(this, '${table.id}')">
            `;
            tableContainer.insertBefore(searchDiv, table);
        }
    });
}

// Funkcja filtrowania tabeli
window.filterTable = function(input, tableId) {
    const filter = input.value.toLowerCase();
    const table = document.getElementById(tableId);
    if (!table) return;
    
    const rows = table.getElementsByTagName('tbody')[0].getElementsByTagName('tr');
    
    Array.from(rows).forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(filter) ? '' : 'none';
    });
};

// ===== AUTOMATYCZNE ZAMYKANIE ALERTÓW =====
function initAlerts() {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        // Dodaj przycisk zamknięcia jeśli nie istnieje
        if (!alert.querySelector('.btn-close')) {
            const closeBtn = document.createElement('button');
            closeBtn.type = 'button';
            closeBtn.className = 'btn-close';
            closeBtn.setAttribute('data-bs-dismiss', 'alert');
            closeBtn.setAttribute('aria-label', 'Close');
            alert.appendChild(closeBtn);
        }

        // Auto-ukryj po 5 sekundach
        if (!alert.classList.contains('alert-permanent')) {
            setTimeout(() => {
                alert.style.transition = 'opacity 0.5s ease';
                alert.style.opacity = '0';
                setTimeout(() => alert.remove(), 500);
            }, 5000);
        }
    });
}

// ===== KONFIRMACJA USUNIĘCIA =====
window.confirmDelete = function(itemName) {
    return confirm(`Czy na pewno chcesz usunąć: ${itemName}?\n\nTej operacji nie można cofnąć.`);
};

// ===== DODAWANIE DO KOSZYKA (PLACEHOLDER) =====
window.addToCart = function(gameId, gameName) {
    showNotification(`✅ ${gameName} został dodany do koszyka!`, 'success');
};

// ===== POWIADOMIENIA =====
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} position-fixed top-0 start-50 translate-middle-x mt-3`;
    notification.style.zIndex = '9999';
    notification.style.minWidth = '300px';
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" onclick="this.parentElement.remove()"></button>
    `;
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
        notification.style.transition = 'opacity 0.5s ease';
        notification.style.opacity = '0';
        setTimeout(() => notification.remove(), 500);
    }, 3000);
}

// ===== SMOOTH SCROLL =====
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// ===== LICZNIK W NAGŁÓWKU (KOSZYK) =====
window.updateCartCount = function(count) {
    let badge = document.querySelector('.cart-badge');
    if (!badge && count > 0) {
        const cartLink = document.querySelector('a[href*="cart"]');
        if (cartLink) {
            badge = document.createElement('span');
            badge.className = 'badge bg-danger cart-badge ms-1';
            cartLink.appendChild(badge);
        }
    }
    if (badge) {
        badge.textContent = count;
        badge.style.display = count > 0 ? 'inline' : 'none';
    }
};

// ===== LAZY LOADING OBRAZÓW =====
if ('IntersectionObserver' in window) {
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                if (img.dataset.src) {
                    img.src = img.dataset.src;
                    img.removeAttribute('data-src');
                    observer.unobserve(img);
                }
            }
        });
    });

    document.querySelectorAll('img[data-src]').forEach(img => {
        imageObserver.observe(img);
    });
}

// ===== CONSOLE LOG =====
console.log('%c🎮 GameShop ', 'background: linear-gradient(135deg, #6366f1 0%, #ec4899 100%); color: white; font-size: 20px; padding: 10px; border-radius: 5px;');
console.log('Frontend załadowany pomyślnie ✅');
