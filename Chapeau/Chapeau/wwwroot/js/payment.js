// Initialize state variables
let initialTotal = 0;
let currentMode = 'amount'; // 'amount' or 'percent'
let feedback = '';
let selectedPaymentMethod = null;
let selectedPaymentType = null;

// Initialize modal references
const paymentMethodModal = new bootstrap.Modal(document.getElementById('paymentMethodModal'));
const splitModal = new bootstrap.Modal(document.getElementById('splitModal'));
const paymentModal = new bootstrap.Modal(document.getElementById('paymentModal'));

// Section references
const cashSection = document.getElementById('cashSection');
const cardSection = document.getElementById('cardSection');
const giftCardSection = document.getElementById('giftCardSection');

// Initialize when document is ready
document.addEventListener('DOMContentLoaded', function () {
    console.log('initializing')
    // Set initial values
    initialTotal = parseFloat(document.getElementById('grandTotal').textContent.replace('€', '').replace(',', '.'));

    document.getElementById('cashAmount')?.addEventListener('input', handleCashAmountInput);
    document.getElementById('giftCardNumber')?.addEventListener('input', handleGiftCardInput);

    // Initialize tip type buttons
    document.querySelectorAll('.tip-type-btn').forEach(btn => {
        btn.addEventListener('click', function (e) {
            setTipMode(e.target.dataset.mode, e);
        });
    });

    // Set initial total value
    const totalInput = document.getElementById('totalInput');
    if (totalInput) {
        totalInput.value = initialTotal.toFixed(2).replace('.', ',');
    }

    // Set initial mode to amount
    const amountButton = document.querySelector('[data-mode="amount"]');
    if (amountButton) {
        setTipMode('amount', { currentTarget: amountButton });
    }

    // Show feedback display if there's feedback
    const feedbackDisplay = document.getElementById('feedbackDisplay');
    const feedbackText = document.getElementById('feedbackText');
    const modelFeedback = document.querySelector('[data-model-feedback]')?.dataset.modelFeedback;

    if (modelFeedback && modelFeedback.trim()) {
        feedbackText.textContent = modelFeedback;
        feedbackDisplay.style.display = 'flex';
    }

    // Initialize finish button for the combined modal
    document.getElementById('finishPaymentButton')?.addEventListener('click', () => processPayment(selectedPaymentType));
});

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

function selectPaymentMethod(e) {
    selectedPaymentMethod = e.currentTarget.dataset.method;
    selectedPaymentType = selectedPaymentMethod.toLowerCase();
    const buttons = document.querySelectorAll('.payment-method-btn');
    buttons.forEach(btn => btn.classList.remove('selected'));
    e.currentTarget.classList.add('selected');

    // Hide all sections first
    cashSection.classList.add('d-none');
    cardSection.classList.add('d-none');
    giftCardSection.classList.add('d-none');

    // Show the appropriate section
    switch (selectedPaymentType) {
        case 'cash':
            cashSection.classList.remove('d-none');
            break;
        case 'debitcard':
        case 'creditcard':
            cardSection.classList.remove('d-none');
            break;
        case 'giftcard':
            giftCardSection.classList.remove('d-none');
            break;
    }
    paymentMethodModal.hide();
    paymentModal.show();
}

function handleCashAmountInput(e) {
    const input = e.target.value.replace(/[^0-9,]/g, '');
    const amount = parseFloat(input.replace(',', '.'));
    const total = initialTotal;
    const change = amount - total;
    document.getElementById('changeAmount').textContent = `Change: €${change.toFixed(2).replace('.', ',')}`;
}

function handleGiftCardInput(e) {
    // Add any gift card validation logic here
    const input = e.target.value.replace(/[^0-9]/g, '');
    e.target.value = input;
}

function processPayment(method) {
    console.log(document.getElementById('tipDisplay'))
    const orderId = getOrderIdFromUrl();
    const tipAmount = parseFloat(document.getElementById('tipDisplay')?.textContent.replace('€', '').replace(',', '.') || '0');
    const feedback = document.getElementById('feedbackNotes')?.value || '';
    const vat = parseFloat(document.getElementById('totalVatDisplay')?.textContent.replace('€', '').replace(',', '.') || '0');

    // Map the method string to the corresponding enum value
    let paymentMethodEnum;
    switch (method) {
        case 'cash':
            paymentMethodEnum = 'Cash';
            break;
        case 'card':
        case 'debitcard':
        case 'creditcard':
            paymentMethodEnum = 'CreditCard';
            break;
        case 'giftcard':
            paymentMethodEnum = 'GiftCard';
            break;
        default:
            paymentMethodEnum = 'Cash';
    }

    let paymentData = {
        orderId: orderId,
        tipAmount: tipAmount,
        paymentMethod: paymentMethodEnum,
        feedback: feedback,
        totalAmount: initialTotal + tipAmount,
        vatValues: vat
    };
    console.log(paymentData)

    // Add method-specific data
    if (method === 'cash') {
        const cashAmount = parseFloat(document.getElementById('cashAmount').value.replace('€', '').replace(',', '.'));
        paymentData.cashAmount = cashAmount;
    } else if (method === 'giftcard') {
        const giftCardNumber = document.getElementById('giftCardNumber').value;
        paymentData.giftCardNumber = giftCardNumber;
    }

    console.log('Payment data being sent:', paymentData);
    console.log('JSON string being sent:', JSON.stringify(paymentData));

    fetch('/Payment/ProcessPayment', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        body: JSON.stringify(paymentData)
    })
    .then(response => {
        console.log('Response status:', response.status);
        return response.json();
    })
    .then(data => {
        console.log('Response data:', data);
        if (data.success) {
            paymentModal.hide();
            if (data.redirectUrl) {
                window.location.href = data.redirectUrl;
            } else {
                window.location.href = '/Waiter/Orders';
            }
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
