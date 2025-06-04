// Payment handling functions
let tipModal;
let paymentMethodModal;
let splitModal;
let selectedPaymentMethod;
let isTipPercentage = false;

document.addEventListener('DOMContentLoaded', function() {
    tipModal = new bootstrap.Modal(document.getElementById('tipModal'));
    paymentMethodModal = new bootstrap.Modal(document.getElementById('paymentMethodModal'));
    splitModal = new bootstrap.Modal(document.getElementById('splitModal'));
});

function openPaymentMethodModal() {
    paymentMethodModal.show();
}

function openTipModal() {
    tipModal.show();
}

function toggleTipType() {
    isTipPercentage = !isTipPercentage;
    const tipSymbol = document.getElementById('tipSymbol');
    const toggleButton = document.querySelector('.tip-toggle');
    
    if (isTipPercentage) {
        tipSymbol.textContent = '%';
        toggleButton.textContent = 'Switch to €';
    } else {
        tipSymbol.textContent = '€';
        toggleButton.textContent = 'Switch to %';
    }
    
    calculateTip();
}

function calculateTip() {
    const tipInput = document.getElementById('tipInput');
    const tipValue = parseFloat(tipInput.value) || 0;
    const subtotal = parseFloat(document.getElementById('subtotal').textContent.replace('€', '').replace(',', '.'));
    
    let finalTip = tipValue;
    if (isTipPercentage) {
        finalTip = (subtotal * tipValue) / 100;
    }
    
    document.getElementById('tipAmount').textContent = '€' + finalTip.toFixed(2);
    updateTotal();
}

function updateTotal() {
    const subtotal = parseFloat(document.getElementById('subtotal').textContent.replace('€', '').replace(',', '.'));
    const tip = parseFloat(document.getElementById('tipAmount').textContent.replace('€', '').replace(',', '.'));
    const total = subtotal + tip;
    document.getElementById('grandTotal').textContent = '€' + total.toFixed(2);
}

function saveTip() {
    const tipInput = document.getElementById('tipInput');
    const tipValue = parseFloat(tipInput.value) || 0;
    const tipAmount = parseFloat(document.getElementById('tipAmount').textContent.replace('€', ''));
    
    // Update the tip display in the main view
    document.querySelector('.tip-button span:last-child').textContent = '€' + tipAmount.toFixed(2);
    
    // Close the modal
    tipModal.hide();
    
    // Update the grand total in the main view
    updateMainTotal();
}

function updateMainTotal() {
    const subtotal = parseFloat(document.getElementById('subtotal').textContent.replace('€', '').replace(',', '.'));
    const tip = parseFloat(document.getElementById('tipAmount').textContent.replace('€', '').replace(',', '.'));
    const total = subtotal + tip;
    document.querySelector('.total-row span:last-child').textContent = '€' + total.toFixed(2);
}

function processPayment() {
    if (!selectedPaymentMethod) {
        alert('Please select a payment method');
        return;
    }

    const orderId = getOrderIdFromUrl();
    const tipAmount = parseFloat(document.getElementById('tipAmount').textContent.replace('€', '').replace(',', '.')) || 0;
    const feedback = document.getElementById('feedbackNotes')?.value || '';

    const paymentData = {
        orderId: orderId,
        tipAmount: tipAmount,
        isTipPercentage: isTipPercentage,
        paymentMethod: selectedPaymentMethod,
        feedback: feedback
    };

    fetch('/Payment/ProcessPayment', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(paymentData)
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            paymentMethodModal.hide();
            window.location.href = '/Waiter/Orders';
        } else {
            alert('Payment failed: ' + (data.error || 'Unknown error'));
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert('An error occurred while processing the payment');
    });
}

function getOrderIdFromUrl() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('orderId');
}

// Split payment functions
function openSplitModal() {
    splitModal.show();
} 