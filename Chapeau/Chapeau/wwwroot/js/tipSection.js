// Tip Section functionality (for inline tip input, not modal)
document.addEventListener('DOMContentLoaded', function() {
    // Elements
    const tipInput = document.getElementById('tipInputSimple');
    const tipTypeButtons = document.querySelectorAll('.tip-type-btn-simple');
    let currentMode = 'euro';
    let originalTotal = parseFloat(document.querySelector('#grandTotal')?.textContent?.replace('€', '').replace(',', '.') || '0');

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
            setTipMode(button.textContent.trim() === '%' ? 'percent' : 'euro');
        });
    });

    // Set tip mode (euro or percent)
    function setTipMode(mode) {
        currentMode = mode;
        tipTypeButtons.forEach(btn => {
            btn.classList.toggle('active', (mode === 'percent' && btn.textContent.trim() === '%') || (mode === 'euro' && btn.textContent.trim() === '€'));
        });
        tipInput.value = '';
        tipInput.placeholder = mode === 'percent' ? '0' : '0,00';
    }

    // Handle tip input changes
    tipInput.addEventListener('input', function(e) {
        let value = e.target.value.replace(',', '.');
        let tipAmount = 0;
        if (currentMode === 'euro') {
            if (value && !isNaN(value)) {
                tipAmount = parseFloat(value);
            }
        } else {
            if (value && !isNaN(value)) {
                const percentage = parseFloat(value);
                tipAmount = (originalTotal * percentage) / 100;
            }
        }
        if (tipAmount < 0) {
            return
        }
        // Update UI
        const tipDisplay = document.getElementById('tipDisplay');
        if (tipDisplay) {
            tipDisplay.textContent = `€${formatCurrency(tipAmount)}`;
        }
    });

    // Set initial mode
    setTipMode('euro');
}); 