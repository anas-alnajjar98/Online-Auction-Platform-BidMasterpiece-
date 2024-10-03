
const stripe = Stripe('pk_test_51Q3FzBRqxwpgnuaXO3FvwmrXdMIzL7hn70SO4lHu8W7QBGqWYWIWGzCYGMHtPw3j16Vfv1nRtyhsgK2LazOZGphL00A7laiJOh');
const elements = stripe.elements();
const card = elements.create("card");
card.mount("#card-element");
document.addEventListener("DOMContentLoaded", async function () {
    const urlParams = new URLSearchParams(window.location.search);
    const auctionId = urlParams.get("auctionId");

    // Fetch auction details
    const auctionResponse = await fetch(`https://localhost:7046/api/Payment/GetAuctionDetailsByPayment/${auctionId}`);
    debugger
    const auctionData = await auctionResponse.json();
    document.getElementById("product-name").textContent = auctionData.productName;
    document.getElementById("bid-amount").textContent = `$${auctionData.currentHighestBid.toFixed(2)}`;
    const totalAmount = auctionData.currentHighestBid + 25; // Shipping + Tax
    document.getElementById("total-amount").textContent = `$${totalAmount.toFixed(2)}`;

    
    const paymentIntentResponse = await fetch(`https://localhost:7046/api/Payment/CreatePaymentIntent`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            Amount: totalAmount,
            AuctionId: auctionId,
        }),
    });
    const paymentIntentData = await paymentIntentResponse.json();
    const clientSecret = paymentIntentData.clientSecret;

    // Handle form submission
    const paymentButton = document.getElementById("payment-button");
    paymentButton.addEventListener("click", async function () {
        debugger
        paymentButton.disabled = true;  
        const { paymentIntent, error } = await stripe.confirmCardPayment(clientSecret, {
            payment_method: {
                card: card,
            },
        });

        if (error) {
            document.getElementById("card-errors").textContent = error.message;
            paymentButton.disabled = false;
        } else if (paymentIntent.status === "succeeded") {
            await fetch(`https://localhost:7046/api/Payment/UpdatePaymentStatus/${auctionId}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    Status: "successful-paid",
                }),
            });
        
            const createOrderHistoryResponse = await fetch(`https://localhost:7046/api/Payment/CreateOrderHistory/${auctionId}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
            });
        
            if (createOrderHistoryResponse.ok) {
                // Redirect to thank you page
                window.location.href = `thankyou.html?paymentId=${auctionId}`;
            } else {
                // Handle the error if the order history creation failed
                const errorData = await createOrderHistoryResponse.json();
                document.getElementById("card-errors").textContent = `Order history creation failed: ${errorData.message}`;
            }
        }
        
    });
});