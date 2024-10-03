function getQueryParam(param) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(param);
}

async function fetchProductDetails() {
    const auctionId = getQueryParam('auctionId'); 

    if (!auctionId) {
        document.getElementById('productName').innerText = "Auction ID is missing.";
        return;
    }

    try {
        const response = await fetch(`https://localhost:7046/api/Products/GetProductByAuctionID/${auctionId}`);
        const product = await response.json();

        if (!response.ok) {
            throw new Error('Failed to fetch product details');
        }
        document.getElementById('productName').innerText = product.productName;
        document.getElementById('productCountry').innerText = product.country || "Unknown";
        document.getElementById('startBid').innerText = `$${product.startingPrice}`;
        document.getElementById('latestBid').innerText = `$${product.currentHighestBid}`;
        document.getElementById('totalBids').innerText = product.totalBids || 0;
        document.getElementById('productImage').src = `assets/images/${product.imageUrl}`;
        document.getElementById('productQuantity').innerText = `${product.quantity} items`;
        document.getElementById('productView').innerText = product.view || 0;
        document.getElementById('productBrand').innerText = product.brand || "N/A";
        document.getElementById('productPublishDate').innerText = new Date(product.publishDate).toLocaleDateString();
        document.getElementById('productDescription').innerText = product.description;

        // Set the initial bid amount
        document.getElementById('bidAmount').value = product.currentHighestBid == 0 ? product.startingPrice : product.currentHighestBid;

        // Start the countdown timer using the product's EndTime
        startCountdown(product.endTime);
    } catch (error) {
        console.error('Error fetching product details:', error);
        Swal.fire('Error', 'Product details could not be loaded.', 'error');
    }
}

function startCountdown(endTime) {
    const timerElement = document.getElementById('countdown-timer');
    const endDate = new Date(endTime).getTime();

    const countdownInterval = setInterval(() => {
        const now = new Date().getTime();
        const timeLeft = endDate - now;

        if (timeLeft <= 0) {
            clearInterval(countdownInterval);
            timerElement.innerHTML = "EXPIRED";
        } else {
            const days = Math.floor(timeLeft / (1000 * 60 * 60 * 24));
            const hours = Math.floor((timeLeft % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            const minutes = Math.floor((timeLeft % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((timeLeft % (1000 * 60)) / 1000);

            timerElement.innerHTML = `
                <span class="days">${days}D</span>:<span class="hours">${hours}H</span>:<span class="minutes">${minutes}M</span>:<span class="seconds">${seconds}S</span>
            `;
        }
    }, 1000); 
}

document.getElementById('plus').addEventListener('click', function () {
    const bidAmountInput = document.getElementById('bidAmount');
    if (bidAmountInput) {
        bidAmountInput.value = parseFloat(bidAmountInput.value) + 25;
    } else {
        console.error('Bid Amount input field not found.');
    }
});

document.getElementById('minus').addEventListener('click', function () {
    const bidAmountInput = document.getElementById('bidAmount');
    if (bidAmountInput) {
        const newBid = Math.max(0, parseFloat(bidAmountInput.value) - 25); 
        bidAmountInput.value = newBid;
    } else {
        console.error('Bid Amount input field not found.');
    }
});

async function placeBid() {
    const userId = localStorage.getItem('userId');
    const auctionId = getQueryParam('auctionId');
    const bidAmount = parseFloat(document.getElementById('bidAmount').value);

    if (isNaN(bidAmount)) {
        Swal.fire('Error', 'Please enter a valid bid amount.', 'error');
        return;
    }

    if (userId == null) {
       localStorage.setItem('trytoBid', true);
       localStorage.setItem('savedUrl', window.location.href);
        Swal.fire({
            title: 'Login Required',
            text: 'You need to log in to place a bid. Do you want to log in now?',
            icon: 'info',
            showCancelButton: true,
            confirmButtonText: 'Yes, log me in',
            cancelButtonText: 'No, cancel',
        }).then((result) => {
            if (result.isConfirmed) {
               
                window.location.href = 'login.html';
            } else {
               
                Swal.fire('Cancelled', 'You have not logged in.', 'info');
            }
        });
        return;
    }

    try {
        const response = await fetch('https://localhost:7046/api/Products/AuctionBiding', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                id: auctionId,
                bidAmount: bidAmount,
                userId: userId 
            })
        });

        const result = await response.json();

        if (response.ok) {
            Swal.fire('Success', 'Bid placed successfully!', 'success');
            fetchProductDetails(); // Refresh product details (e.g., update latest bid)
        } else {
            Swal.fire('Error', `Bid failed: ${result.message}`, 'error');
        }
    } catch (error) {
        console.error('Error placing bid:', error);
        Swal.fire('Error', 'An error occurred while placing the bid.', 'error');
    }
}

// Attach event listener to the "Bid Now" button
document.querySelector('.primary-btn').addEventListener('click', function (event) {
    event.preventDefault(); 
    placeBid(); 
});

// Fetch product details and start the countdown when the page is loaded
document.addEventListener("DOMContentLoaded", function () {
    fetchProductDetails(); 
});
