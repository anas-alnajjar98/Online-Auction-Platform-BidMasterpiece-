document.addEventListener("DOMContentLoaded", function () {
    const auctionContainer = document.getElementById('auctionContainer');
    let largeCardContainer = document.getElementById('largeCardContainer');
    let auctionProducts = []; 

    // Fetch and render the large card product
    async function LargeCardProduct() {
        try {
            const response = await fetch('https://localhost:7046/api/Products/GetAuctionProductsForHomePageForLargeCard');
            
            if (!response.ok) {
                throw new Error('No auction products found');
            }
            
            const largeProduct = await response.json();
    
            // Clear the container before adding content
            largeCardContainer.innerHTML = '';
    
            const currentBid = (typeof largeProduct.currentHighestBid === 'number')
                ? largeProduct.currentHighestBid.toFixed(2)
                : (typeof largeProduct.StartingPrice === 'number' ? largeProduct.StartingPrice.toFixed(2) : 'N/A');
    
            const largeCardHtml = `
                <div class="auction-card">
                    <div class="card-image left-card-image">
                        <img src="assets/images/${largeProduct.imageUrl}" class="h-100 w-100" alt="auction-card-img">
                        <div class="timer-wrapper">
                            <div class="timer-inner" id="large-timer">
                                <span class="days">0D</span>:<span class="hours">0H</span>:<span class="minutes">0M</span>:<span class="seconds">0S</span>
                            </div>
                        </div>
                    </div>
                    <div class="card-content">
                        <h4>${largeProduct.productName}</h4>
                        <p>Current bid <span>${currentBid}$</span></p>
                        <div class="d-flex justify-content-between align-items-center">
                            <a href="bid-detail.html?auctionId=${largeProduct.auctionId}" class="primary-btn">Bid Now</a>
                            <button class="like-btn"><i class="fa-regular fa-heart"></i></button>
                        </div>
                    </div>
                </div>
            `;
    
            largeCardContainer.innerHTML = largeCardHtml;
    
            // Start countdown for the large card
            startCountdown('large-timer', largeProduct.endTime);
    
        } catch (error) {
            console.error('Error fetching large auction product:', error);
            largeCardContainer.innerHTML = '<p>No auction product available at this time.</p>'; // Display error message to the user
        }
    }

    // Fetch and render the small auction products
    async function fetchAuctionProducts() {
        try {
            const response = await fetch('https://localhost:7046/api/Products/GetAuctionProductsForHomePage');
            auctionProducts = await response.json(); // Store response globally

            // Clear the existing content (if any)
            auctionContainer.innerHTML = '';

            // Loop through each auction product and create a card
            auctionProducts.forEach(product => {
                // Ensure both values are valid numbers before applying toFixed()
                const currentBid = (typeof product.currentHighestBid === 'number')
                    ? product.currentHighestBid.toFixed(2)
                    : (typeof product.StartingPrice === 'number' ? product.StartingPrice.toFixed(2) : 'N/A');

                const auctionCard = `
                    <div class="col-md-6">
                        <div class="auction-card">
                            <div class="card-image">
                                <img src="assets/images/${product.imageUrl}" class="img-fluid" alt="auction-card-img">
                                <div class="timer-wrapper">
                                    <div class="timer-inner" id="timer-${product.auctionId}">
                                        <!-- Timer elements will be dynamically updated -->
                                        <span class="days">0D</span>:<span class="hours">0H</span>:<span class="minutes">0M</span>:<span class="seconds">0S</span>
                                    </div>
                                </div>
                            </div>
                            <div class="card-content">
                                <h4>${product.productName}</h4>
                                <p>Current bid <span>${currentBid}$</span></p>
                                <div class="d-flex justify-content-between align-items-center">
                                    <a href="bid-detail.html?auctionId=${product.auctionId}" class="primary-btn">Bid Now</a>
                                    <button class="like-btn"><i class="fa-regular fa-heart"></i></button>
                                </div>
                            </div>
                        </div>
                    </div>
                `;

                // Append the auction card to the container
                auctionContainer.innerHTML += auctionCard;
            });

            // Start the global countdown interval
            startGlobalCountdown();

        } catch (error) {
            console.error('Error fetching auction products:', error);
        }
    }

  
    fetchAuctionProducts();
    LargeCardProduct();
    function startGlobalCountdown() {
        const countdownInterval = setInterval(() => {
            auctionProducts.forEach(product => {
                const timerElement = document.getElementById(`timer-${product.auctionId}`);
                if (!timerElement) return;

                const timeLeft = calculateTimeLeft(product.endTime);

                if (timeLeft.total <= 0) {
                    timerElement.innerHTML = 'EXPIRED';
                } else {
                    timerElement.innerHTML = `
                        <span class="days">${timeLeft.days}D</span>:<span class="hours">${timeLeft.hours}H</span>:<span class="minutes">${timeLeft.minutes}M</span>:<span class="seconds">${timeLeft.seconds}S</span>
                    `;
                }
            });
        }, 1000); 
    }
    function startCountdown(timerId, endTime) {
        const timerElement = document.getElementById(timerId);

        const countdownInterval = setInterval(() => {
            const timeLeft = calculateTimeLeft(endTime);

            if (timeLeft.total <= 0) {
                clearInterval(countdownInterval);
                timerElement.innerHTML = 'EXPIRED';
            } else {
                timerElement.innerHTML = `
                    <span class="days">${timeLeft.days}D</span>:<span class="hours">${timeLeft.hours}H</span>:<span class="minutes">${timeLeft.minutes}M</span>:<span class="seconds">${timeLeft.seconds}S</span>
                `;
            }
        }, 1000);
    }

    // Helper function to calculate time left
    function calculateTimeLeft(endTime) {
        const end = new Date(endTime).getTime();
        const now = new Date().getTime();
        const difference = end - now;

        let timeLeft = {};

        if (difference > 0) {
            timeLeft = {
                total: difference,
                days: Math.floor(difference / (1000 * 60 * 60 * 24)),
                hours: Math.floor((difference / (1000 * 60 * 60)) % 24),
                minutes: Math.floor((difference / 1000 / 60) % 60),
                seconds: Math.floor((difference / 1000) % 60),
            };
        } else {
            timeLeft = {
                total: 0,
                days: 0,
                hours: 0,
                minutes: 0,
                seconds: 0
            };
        }

        return timeLeft;
    }
});
