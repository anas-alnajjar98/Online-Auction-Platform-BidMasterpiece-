document.addEventListener("DOMContentLoaded", async function () {
    const urlParams = new URLSearchParams(window.location.search);
    const paymentId = urlParams.get("paymentId");

    if (!paymentId) {
        alert("Payment ID is missing from the URL.");
        return;
    }

    try {

        const response = await fetch(`https://localhost:7046/api/Payment/GetThankYouDetailsByPayment/${paymentId}`);
        
        if (!response.ok) {
            throw new Error('Failed to fetch payment details.');
        }

        const data = await response.json();
        console.log("Payment details:", data);

        // Dynamically update the page with the fetched data
        document.getElementById("product-name").textContent = data.productName;
        document.getElementById("delivery-date").textContent = data.deliveryDate;
        document.getElementById("delivery-address").textContent = data.deliveryAddress;
        document.getElementById("product-image").src = `https://localhost:7046/${data.imageUrl}`; 
    } catch (error) {
        console.error("Error fetching payment details:", error);
        alert("There was an error fetching the payment details. Please try again later.");
    }
});
