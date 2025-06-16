// Tip Modal functionality
document.addEventListener('DOMContentLoaded', function() {
    // Elements
    const tipModalElement = document.getElementById('tipModal');
    const tipModal = new bootstrap.Modal(tipModalElement);
    const tipInput = document.getElementById('tipInput');
    const totalInput = document.getElementById('totalInput');
    const errorLabel = document.getElementById('error-label');
    const tipTypeButtons = document.querySelectorAll('.tip-type-btn');
    const percentSymbol = document.querySelector('.percent-symbol');
    const euroSymbol = document.querySelector('.fixed-symbol:not(.percent-symbol)');
    const openModalButton = document.getElementById('addTipButton');

    // Variables to store state
    let currentMode = 'euro';
    let originalTotal = parseFloat(document.querySelector('#grandTotal')?.textContent?.replace('€', '').replace(',', '.') || '0');

    // Initialize the modal
    function initializeTipModal() {
        tipInput.value = '';
        totalInput.value = formatCurrency(originalTotal);
        setTipMode('euro');
    }

    function openTipModal() {
        tipModal.show()
    }

    // Format currency (€)
    function formatCurrency(value) {
        return value.toFixed(2).replace('.', ',');
    }

    // Parse currency string to number
    function parseCurrency(value) {
        return parseFloat(value.replace('€', '').replace(',', '.').trim());
    }

    // Handle tip type button clicks
    tipTypeButtons.forEach(button => {
        button.addEventListener('click', () => {
            const mode = button.dataset.mode;
            setTipMode(mode);
        });
    });

    // Set tip mode (euro or percent)
    function setTipMode(mode) {
        currentMode = mode;
        tipTypeButtons.forEach(btn => {
            btn.classList.toggle('active', btn.dataset.mode === mode);
        });
        percentSymbol.style.display = mode === 'percent' ? 'block' : 'none';
        tipInput.value = '';
        tipInput.placeholder = mode === 'percent' ? '0' : '0,00';
    }

    // Handle tip input changes
    tipInput.addEventListener('input', function(e) {
        let value = e.target.value.replace(',', '.');
        
        if (currentMode === 'euro') {
            // Handle euro input
            if (value && !isNaN(value)) {
                const tipAmount = parseFloat(value);
                if (tipAmount >= 0) {
                    totalInput.value = formatCurrency(originalTotal + tipAmount);
                    errorLabel.classList.add('d-none');
                } else {
                    errorLabel.classList.remove('d-none');
                }
            }
        } else {
            // Handle percentage input
            if (value && !isNaN(value)) {
                const percentage = parseFloat(value);
                if (percentage >= 0) {
                    const tipAmount = (originalTotal * percentage) / 100;
                    totalInput.value = formatCurrency(originalTotal + tipAmount);
                    errorLabel.classList.add('d-none');
                } else {
                    errorLabel.classList.remove('d-none');
                }
            }
        }
    });

    // Handle total input changes
    totalInput.addEventListener('input', function(e) {
        let value = parseCurrency(e.target.value);
        if (!isNaN(value)) {
            const tipAmount = value - originalTotal;
            if (tipAmount >= 0) {
                if (currentMode === 'euro') {
                    tipInput.value = formatCurrency(tipAmount);
                } else {
                    const percentage = (tipAmount / originalTotal) * 100;
                    tipInput.value = percentage.toFixed(0);
                }
                errorLabel.classList.add('d-none');
            } else {
                errorLabel.classList.remove('d-none');
            }
        }
    });

    // Save tip
    window.saveTip = function() {
        const tipAmount = currentMode === 'euro' 
            ? parseCurrency(tipInput.value)
            : (parseCurrency(totalInput.value) - originalTotal);

        if (tipAmount >= 0) {
            // Update UI
            const tipDisplay = document.querySelector('#tipAmount');
            if (tipDisplay) {
                tipDisplay.textContent = `€${formatCurrency(tipAmount)}`;
            }

            // Update grand total
            const grandTotalDisplay = document.querySelector('#grandTotal');
            if (grandTotalDisplay) {
                grandTotalDisplay.textContent = `€${formatCurrency(originalTotal + tipAmount)}`;
            }

            // Close modal
            tipModal.hide();

            // You can add an AJAX call here to save the tip to the server
            // Example:
            // saveTipToServer(tipAmount);
        }
    };

    // Reset modal when it's opened
    tipModalElement.addEventListener('show.bs.modal', function() {
        initializeTipModal();
        errorLabel.classList.add('d-none');
    });
}); 