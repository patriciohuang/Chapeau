// Orders page JavaScript functionality

// Initialize page functionality when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    // Add smooth animations to order cards
    const orderCards = document.querySelectorAll('.order-card');
    orderCards.forEach((card, index) => {
        card.style.animationDelay = (index * 50) + 'ms';
    });

    // Add click handlers for mobile card interactions
    setupMobileCardInteractions();

    // Add hover effects to table filter buttons
    setupTableFilterButtonEffects();
});

// Setup mobile card interactions
function setupMobileCardInteractions() {
    const orderCards = document.querySelectorAll('.order-card');

    orderCards.forEach(card => {
        // Add touch feedback
        card.addEventListener('touchstart', function () {
            this.style.transform = 'translateY(-1px) scale(0.98)';
        });

        card.addEventListener('touchend', function () {
            this.style.transform = '';
        });
    });
}

// Setup table filter button effects
function setupTableFilterButtonEffects() {
    const tableFilterButtons = document.querySelectorAll('.table-filter-btn');

    tableFilterButtons.forEach(button => {
        // Add click animation
        button.addEventListener('click', function () {
            this.style.transform = 'scale(0.95)';
            setTimeout(() => {
                this.style.transform = '';
            }, 150);
        });
    });
}

// Utility function to refresh the page
function refreshOrders() {
    window.location.reload();
}

// Function to clear table filter while keeping main filter
function clearTableFilter() {
    const currentUrl = new URL(window.location);
    currentUrl.searchParams.delete('tableNr');
    window.location.href = currentUrl.toString();
}