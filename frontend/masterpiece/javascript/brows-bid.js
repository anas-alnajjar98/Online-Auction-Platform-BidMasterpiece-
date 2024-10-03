function getQueryParam(param) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(param);
}

document.addEventListener("DOMContentLoaded", function() {

    async function getAllCategoryWithCount() {
        const categories = document.getElementById("CategorywithCount");

        if (!categories) {
            console.error("CategorywithCount element not found in the DOM.");
            return;
        }

        try {
            const response = await fetch('https://localhost:7046/api/Products/GetALLCategoryWithTottalProducts');
            
            if (!response.ok) {
                throw new Error('Failed to fetch categories');
            }

            const data = await response.json();
            categories.innerHTML = '';

            if (!data.length) {
                categories.innerHTML = '<li>No categories available</li>';
                return;
            }

            data.forEach(category => {
                categories.innerHTML += `
                    <li>
                        <a href="browse-bid.html?CategoryId=${category.categoryId}">
                            <span>${category.categoryName}</span> 
                            <span>(${category.totalProducts})</span>
                        </a>
                    </li>
                `;
            });
        } catch (error) {
            console.error('Error fetching categories:', error);
            categories.innerHTML = '<li>Error loading categories</li>';
        }
    }

    async function fetchProducts(page = 1) {
        try {
            let response;
            const categoryId = getQueryParam('CategoryId'); // Assuming a function to get query parameters
            debugger
            if (categoryId) {
                response = await fetch(`https://localhost:7046/api/Products/GetProductsByCategory/${categoryId}?pageNumber=${page}&pageSize=9`);
            } else {
                response = await fetch(`https://localhost:7046/api/Products/GetAllProducts?pageNumber=${page}&pageSize=9`);
            }
    
            if (!response.ok) {
                throw new Error('Failed to fetch products');
            }
    
            const data = await response.json();
            console.log('API response:', data);
    
            const productsContainer = document.getElementById('auction-products-container');
            const paginationContainer = document.getElementById('pagination');
    
            if (!productsContainer || !paginationContainer) {
                console.error("Products or pagination container not found in the DOM.");
                return;
            }
    
            productsContainer.innerHTML = '';
            paginationContainer.innerHTML = '';
    
            // Check if products exist in the response data
            if (!data.auctions || !Array.isArray(data.auctions) || data.auctions.length === 0) {
                productsContainer.innerHTML = '<p>No products found.</p>';
                return;
            }
    
            // Render each product in the productsContainer
            data.auctions.forEach(product => {
                productsContainer.innerHTML += `
                    <div class="col-sm-6 col-lg-4">
                        <div class="auction-card">
                            <div class="card-image">
                                <img src="${(product.productDetails.imageUrl.startsWith("/images/"))?`https://localhost:7046/${product.productDetails.imageUrl}`:product.productDetails.imageUrl}" alt="auction-card-img">
                                <div class="timer-wrapper">
                                    <div class="timer-inner" id="timer-${product.productDetails.productId}">
                                        <!-- Timer will be dynamically inserted here -->
                                    </div>
                                </div>
                            </div>
                            <div class="card-content">
                                <a href="bid-detail.html?auctionId=${product.auctionId}" class="card-title">${product.productDetails.productName}</a>
                                <div class="d-flex justify-content-between align-items-center">
                                    <p class="p-0">Current bid <span>${product.currentHighestBid || product.productDetails.startingPrice}$</span></p>
                                    <button class="like-btn"><i class="fa-regular fa-heart"></i></button>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
    
                // Start countdown for each product
                startCountdown(product.endTime, `timer-${product.productDetails.productId}`);
            });
    
            // Generate pagination based on total pages and current page
            generatePagination(data.totalPages, page);
        } catch (error) {
            console.error('Error fetching products:', error);
            document.getElementById('auction-products-container').innerHTML = '<p>Error loading products</p>';
        }
    }
    

    function generatePagination(totalPages, currentPage) {
        const paginationContainer = document.getElementById('pagination');
        if (!paginationContainer) return;

        
        paginationContainer.innerHTML = '';

        for (let i = 1; i <= totalPages; i++) {
            paginationContainer.innerHTML += `
                <li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#">${i}</a>
                </li>
            `;
        }

       
        document.querySelectorAll('.page-link').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const selectedPage = parseInt(e.target.textContent);

                const productsContainer = document.getElementById('auction-products-container');
                productsContainer.innerHTML = '<p>Loading...</p>';

                fetchProducts(selectedPage); 
            });
        });
    }

    function startCountdown(endTime, timerElementId) {
        const timerElement = document.getElementById(timerElementId);
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

    
    getAllCategoryWithCount();
    fetchProducts(); 
});
