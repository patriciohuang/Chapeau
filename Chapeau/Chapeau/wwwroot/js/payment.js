// Initialize state variables
let initialTotal = 0;
let currentTip = 0;
let currentMode = 'amount'; // 'amount' or 'percent'
let feedback = '';
let selectedPaymentMethod = null;

// Initialize modal references
const paymentMethodModal = new bootstrap.Modal(document.getElementById('paymentMethodModal'));
const tipModal = new bootstrap.Modal(document.getElementById('tipModal'));
const feedbackModal = new bootstrap.Modal(document.getElementById('feedbackModal'));
const splitModal = new bootstrap.Modal(document.getElementById('splitModal'));

// Initialize when document is ready
document.addEventListener('DOMContentLoaded', function() {
    // Set initial values
    initialTotal = parseFloat(document.getElementById('grandTotal').textContent.replace('€', '').replace(',', '.'));
    
    // Initialize input handlers
    document.getElementById('tipInput').addEventListener('input', handleTipInput);
    document.getElementById('totalInput').addEventListener('input', handleTotalInput);
    
    // Initialize tip type buttons
    document.querySelectorAll('.tip-type-btn').forEach(btn => {
        btn.addEventListener('click', function(e) {
            setTipMode(e.target.dataset.mode, e);
        });
    });
    
    // Set initial total value
    document.getElementById('totalInput').value = initialTotal.toFixed(2).replace('.', ',');
    
    // Set initial mode to amount
    const amountButton = document.querySelector('[data-mode="amount"]');
    if (amountButton) {
        setTipMode('amount', { currentTarget: amountButton });
    }
    
    // Initialize tip display
    updateAmounts();

    // Show feedback display if there's feedback
    const feedbackDisplay = document.getElementById('feedbackDisplay');
    const feedbackText = document.getElementById('feedbackText');
    const modelFeedback = document.querySelector('[data-model-feedback]')?.dataset.modelFeedback;
    
    if (modelFeedback && modelFeedback.trim()) {
        feedbackText.textContent = modelFeedback;
        feedbackDisplay.style.display = 'flex';
    }
});

// Tip handling functions
function openTipModal() {
    document.getElementById('tipInput').value = currentTip.toFixed(2).replace('.', ',');
    document.getElementById('totalInput').value = initialTotal.toFixed(2).replace('.', ',');
    tipModal.show();
}

function setTipMode(mode, e) {
    currentMode = mode;
    const buttons = document.querySelectorAll('.tip-type-btn');
    buttons.forEach(btn => btn.classList.remove('active'));
    e.currentTarget.classList.add('active');
    
    const tipInput = document.getElementById('tipInput');
    const percentSymbol = document.querySelector('.percent-symbol');
    const euroSymbol = document.querySelector('.fixed-symbol:not(.percent-symbol)');
    
    if (mode === 'percent') {
        percentSymbol.style.display = 'flex';
        euroSymbol.style.display = 'none';
        if (currentTip > 0) {
            const orderId = getOrderIdFromUrl();
            const baseAmount = parseFloat(document.getElementById('totalInput').value.replace(',', '.')) - currentTip;
            const percentage = ((currentTip / baseAmount) * 100).toFixed(0);
            tipInput.value = percentage;
        } else {
            tipInput.value = '0';
        }
    } else {
        percentSymbol.style.display = 'none';
        euroSymbol.style.display = 'flex';
        tipInput.value = currentTip.toFixed(2).replace('.', ',');
    }
    
    // Trigger tip recalculation
    handleTipInput({ target: tipInput });
}

function handleTipInput(event) {
    let value = event.target.value;
    if (value === '') {
        value = '0';
    }
    
    value = Math.max(0, parseFloat(value) || 0);
    
    const orderId = getOrderIdFromUrl();
    
    // Call the server to calculate tip
    fetch('/Payment/CalculateTip', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            orderId: orderId,
            value: value,
            isPercentage: currentMode === 'percent'
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            currentTip = data.tipAmount;
            
            // Update displayed values
            document.getElementById('tipAmount').textContent = data.formattedTip;
            document.getElementById('grandTotal').textContent = data.formattedTotal;
            
            // Format the input value
            if (currentMode === 'percent') {
                event.target.value = value.toString();
            } else {
                event.target.value = currentTip.toFixed(2).replace('.', ',');
            }
        } else {
            console.error('Error calculating tip:', data.error);
        }
    })
    .catch(error => {
        console.error('Error:', error);
    });
}

function handleTotalInput(event) {
    let value = event.target.value.replace(',', '.');
    value = Math.max(initialTotal, parseFloat(value) || initialTotal); // Ensure total is not less than initial
    
    currentTip = value - initialTotal;
    
    // Update tip input based on mode
    const tipInput = document.getElementById('tipInput');
    if (currentMode === 'percent') {
        const percentage = ((currentTip / initialTotal) * 100).toFixed(0);
        tipInput.value = percentage;
    } else {
        tipInput.value = currentTip.toFixed(0).replace('.', ',');
    }
    
    // Update the total input to show the valid value
    event.target.value = value.toFixed(2).replace('.', ',');
}

function updateAmounts() {
    document.getElementById('tipAmount').textContent = `€${currentTip.toFixed(2)}`.replace('.', ',');
    const newTotal = initialTotal + currentTip;
    document.getElementById('grandTotal').textContent = `€${newTotal.toFixed(2)}`.replace('.', ',');
}

function saveTip() {
    updateAmounts();
    tipModal.hide();
}

// Feedback handling functions
function openFeedbackModal() {
    document.getElementById('feedbackNotes').value = feedback;
    feedbackModal.show();
}

function saveFeedback() {
    feedback = document.getElementById('feedbackNotes').value;
    updateFeedbackDisplay();
    feedbackModal.hide();
}

function updateFeedbackDisplay() {
    const feedbackDisplay = document.getElementById('feedbackDisplay');
    const feedbackText = document.getElementById('feedbackText');
    
    if (feedback.trim()) {
        feedbackText.textContent = feedback;
        feedbackDisplay.style.display = 'flex';
    } else {
        feedbackDisplay.style.display = 'none';
    }
}

// Payment handling functions
function openPaymentMethodModal() {
    paymentMethodModal.show();
}

function selectPaymentMethod(method, e) {
    selectedPaymentMethod = method;
    const buttons = document.querySelectorAll('.payment-method-btn');
    buttons.forEach(btn => btn.classList.remove('selected'));
    e.currentTarget.classList.add('selected');

    // If cash payment is selected, redirect to cash payment page
    if (method.toLowerCase() === 'cash') {
        const orderId = getOrderIdFromUrl();
        paymentMethodModal.hide();
        window.location.href = `/Payment/CashPayment?orderId=${orderId}`;
        return;
    }

    // Process other payment methods
    processPayment();
}

function processPayment() {
    if (!selectedPaymentMethod) {
        alert('Please select a payment method');
        return;
    }

    const orderId = getOrderIdFromUrl();
    const tipAmount = parseFloat(document.getElementById('tipAmount').textContent.replace('€', '').replace(',', '.'));
    const feedback = document.getElementById('feedbackNotes')?.value || '';

    const paymentData = {
        orderId: orderId,
        tipAmount: tipAmount,
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
    return parseInt(urlParams.get('orderId'));
}

// Split payment functions
function openSplitModal() {
    splitModal.show();
}

function splitByDish() {
    splitModal.hide();
    const orderId = getOrderIdFromUrl();
    window.location.href = `/Payment/SplitByDish?orderId=${orderId}`;
}

function splitByAmount() {
    splitModal.hide();
    const orderId = getOrderIdFromUrl();
    window.location.href = `/Payment/SplitByAmount?orderId=${orderId}`;
} 