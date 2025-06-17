// Initialize state variables
let initialTotal = 0;
let currentMode = 'amount'; // 'amount' or 'percent'
let feedback = '';
let selectedPaymentMethod = null;
let selectedPaymentType = null;
let amountPerPerson = 0
let leftToPay = 0
let toPay = 0
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
    // Set initial values
    initialTotal = parseFloat(document.getElementById('grandTotal').textContent.replace('€', '').replace(',', '.'));
    leftToPay = parseFloat(document.getElementById('leftAmountDisplay').textContent.replace('€', '').replace(',', '.'));
    toPay = leftToPay
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
    document.getElementById('splitPeopleSelect')?.addEventListener('input', (event) => { processPeopleInput(event) })
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
    toPay = leftToPay
    paymentMethodModal.show();
}

function selectPaymentMethod(e) {
    selectedPaymentMethod = e.currentTarget.dataset.method;
    selectedPaymentType = selectedPaymentMethod.toLowerCase();
    const buttons = document.querySelectorAll('.payment-method-btn');
    buttons.forEach(btn => btn.classList.remove('selected'));
    e.currentTarget.classList.add('selected');

    openPaymentModal()
}

function openPaymentModal() {
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
    const total = toPay;
    const change = amount - total;
    document.getElementById('changeAmount').textContent = `Change: €${change.toFixed(2).replace('.', ',')}`;
}

function handleGiftCardInput(e) {
    // Add any gift card validation logic here
    const input = e.target.value.replace(/[^0-9]/g, '');
    e.target.value = input;
}

function processPayment(method) {
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
        totalAmount: (parseFloat(toPay) + parseFloat(tipAmount)).toFixed(2),
        vatValues: vat
    };

    // Add method-specific data
    if (method === 'cash') {
        const cashAmount = parseFloat(document.getElementById('cashAmount').value.replace('€', '').replace(',', '.'));
        paymentData.cashAmount = cashAmount;
    } else if (method === 'giftcard') {
        const giftCardNumber = document.getElementById('giftCardNumber').value;
        paymentData.giftCardNumber = giftCardNumber;
    }

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
        } else if (data.error) {
            alert('Payment failed: ' + (data.error || 'Unknown error'));
        } else {
            window.location.reload()
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

function toggleDishSelection(btn) {
    btn.querySelector('i').classList.toggle('text-success');
    updateSplitDishToPay();
}

function updateSplitDishToPay() {
    let total = 0;
    document.querySelectorAll('.dish-select-btn').forEach(function (btn) {
        if (btn.querySelector('i').classList.contains('text-success')) {
            const price = parseFloat(btn.getAttribute('data-price').replace(',', '.'));
            total += price;
        }
    });
    toPay = total
    document.getElementById('toPayFinalDisplay').textContent = '€' + total.toFixed(2).replace('.', ',');
    document.getElementById('splitDishToPay').textContent = '€' + total.toFixed(2).replace('.', ',');
}

function openCardPaymentModalFromSplit(type) {
    var splitDishModal = bootstrap.Modal.getInstance(document.getElementById('splitByDishModal'));
    var splitAmountModal = bootstrap.Modal.getInstance(document.getElementById('splitByAmountModal'));
    if (splitDishModal) splitDishModal.hide();
    if (splitAmountModal) splitAmountModal.hide();
    setTimeout(function () {
        selectedPaymentType = type
        openPaymentModal()
    }, 400);
}
function processPeopleInput(event) {
    const amount = parseInt(event.target.value)
    amountPerPerson = parseFloat(leftToPay / amount).toFixed(2)
    toPay = amountPerPerson
    document.getElementById('toPayFinalDisplay').textContent = '€' + amountPerPerson.replace('.', ',');
    document.getElementById('splitToPay').textContent = '€' + amountPerPerson.replace('.', ',');
} 

function openSplitByAmountModal() {
    var modal = new bootstrap.Modal(document.getElementById('splitByAmountModal'));
    modal.show();
}
function openSplitByDishModal() {
    var modal = new bootstrap.Modal(document.getElementById('splitByDishModal'));
    modal.show();
}

