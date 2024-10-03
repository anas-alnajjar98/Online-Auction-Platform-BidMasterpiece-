function hideAllSections() {
    const sections = document.querySelectorAll('.dashboard-data-section');
    sections.forEach(section => {
        section.style.display = 'none';
    });
}

document.addEventListener('DOMContentLoaded', function () {
    // Dashboard
    document.getElementById('dashboard-link').addEventListener('click', async function () {
        hideAllSections();
        document.getElementById('dashboard-section').style.display = 'block';
        // Fetch dashboard data
        try {
            const response = await fetch('https://yourapi.com/dashboard');
            const data = await response.json();
            document.getElementById('active-bids-count').textContent = data.activeBids;
            document.getElementById('items-won-count').textContent = data.itemsWon;
            document.getElementById('favorites-count').textContent = data.favorites;
        } catch (error) {
            console.error('Error fetching dashboard data', error);
        }
    });

    // Load Pending Products
    async function loadPendingProducts() {
        const tableBody = document.querySelector('#pending-products-table tbody');
        tableBody.innerHTML = '';  
        try {
            const response = await fetch('https://localhost:7046/api/Admin/GetAllProductTopalceAuction'); 
            const data = await response.json();
            if (data.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="9">No pending products found.</td></tr>';
                return;
            }

            // Loop through each product and create table rows dynamically
            data.forEach(product => {
                const row = document.createElement('tr');

                row.innerHTML = `
                    <td>${product.productName}</td>
                    <td>${product.description}</td>
                    <td>$${product.startingPrice.toFixed(2)}</td>
                    <td>${product.stock}</td>
                    <td>${product.condition}</td>
                    <td>${product.location}</td>
                    <td>${product.brand}</td>
                    <td>${product.category}</td>
                    <td>
                       <button class="btn btn-success btn-accept" 
                        data-id="${product.productId}" 
                        data-starting-price="${product.startingPrice.toFixed(2)}">
                        Accept
                        </button>

                        <button class="btn btn-danger btn-reject" data-id="${product.productId}">Reject</button>
                    </td>
                `;

                tableBody.appendChild(row);
            });

            document.querySelectorAll('.btn-accept').forEach(button => {
                button.addEventListener('click', function() {
                    const productId = this.getAttribute('data-id');
                    const startingPrice = this.getAttribute('data-starting-price');
                    acceptProduct(productId, startingPrice); 
                });
            });

            document.querySelectorAll('.btn-reject').forEach(button => {
                button.addEventListener('click', function() {
                    const productId = this.getAttribute('data-id');
                    rejectProduct(productId);
                });
            });

        } catch (error) {
            console.error('Error fetching pending products', error);
        }
    }

    // Function to accept a product
    async function acceptProduct(productId, startingPrice) {
        // Populate the modal fields with product data
        document.getElementById('productIdInput').value = productId;
        document.getElementById('startingPriceInput').value = startingPrice;
    
        // Show the modal
        var auctionModal = new bootstrap.Modal(document.getElementById('auctionModal'));
        auctionModal.show();
    }

    // Function to reject a product
    async function rejectProduct(productId) {
        if (confirm('Are you sure you want to reject this product?')) {
            try {
                const response = await fetch(`https://localhost:7046/api/Admin/RejectProduct/${productId}`, {
                    method: 'PUT'
                });

                if (response.ok) {
                    alert('Product rejected successfully.');
                    loadPendingProducts(); // Reload the list of pending products
                } else {
                    alert('Failed to reject the product.');
                }
            } catch (error) {
                console.error('Error rejecting product:', error);
            }
        }
    }

    // Load Pending Products when the 'Active Bids' section is clicked
    document.getElementById('active-bids-link').addEventListener('click', async function () {
       hideAllSections();
        document.getElementById('active-bids-section').style.display = 'block';
        loadPendingProducts();
    });

    // User Information
    document.getElementById('user-info-link').addEventListener('click', async function () {
        hideAllSections();
        document.getElementById('user-info-section').style.display = 'block';

        try {
            const response = await fetch('https://localhost:7046/api/Admin/GetAllUsers');
            const data = await response.json();
            const tableBody = document.querySelector('#user-info-table tbody');
            tableBody.innerHTML = '';
            data.forEach(user => {
                const row = document.createElement('tr');

                row.innerHTML = `
                    <td>${user.userId}</td>
                    <td>${user.username}</td>
                    <td>${user.email}</td>
                    <td>${user.address || 'N/A'}</td>
                    <td>${user.gender}</td>
                    <td>
                        <button class="btn btn-primary btn-edit" data-id="${user.userId}">Edit</button>
                        <button class="btn btn-danger btn-delete" data-id="${user.userId}">Delete</button>
                    </td>
                `;

                tableBody.appendChild(row);
            });

            // Add event listeners for the Edit and Delete buttons
            document.querySelectorAll('.btn-edit').forEach(button => {
                button.addEventListener('click', function () {
                    const userId = this.getAttribute('data-id');
                    editUser(userId); // Call the edit function
                });
            });

            document.querySelectorAll('.btn-delete').forEach(button => {
                button.addEventListener('click', function () {
                    const userId = this.getAttribute('data-id');
                    deleteUser(userId); // Call the delete function
                });
            });

        } catch (error) {
            console.error('Error fetching user info', error);
        }
    });

    // Edit User
    async function editUser(userId) {
        try {
            const response = await fetch(`https://localhost:7046/api/User/GetUserByID/${userId}`);
            const userData = await response.json();
            
            document.getElementById('editUsername').value = userData.username;
            document.getElementById('editEmail').value = userData.email;
            document.getElementById('editAddress').value = userData.address || '';
            document.getElementById('editGender').value = userData.gender;
            
            var editUserModal = new bootstrap.Modal(document.getElementById('editUserModal'));
            editUserModal.show();

            // Save the changes
            document.getElementById('saveUserChanges').addEventListener('click', async function () {
                const formData = new FormData();
                formData.append('Username', document.getElementById('editUsername').value);
                formData.append('Email', document.getElementById('editEmail').value);
                formData.append('Address', document.getElementById('editAddress').value);
                formData.append('Gender', document.getElementById('editGender').value);

                const imageFile = document.getElementById('editImage').files[0];
                if (imageFile) {
                    formData.append('Image', imageFile);
                }

                try {
                    const response = await fetch(`https://localhost:7046/api/User/UpdateUserInfoWithImage/${userId}`, {
                        method: 'POST',
                        body: formData
                    });

                    if (response.ok) {
                        alert('User updated successfully');
                        editUserModal.hide();
                        document.getElementById('user-info-link').click();
                    } else {
                        alert('Failed to update user');
                    }
                } catch (error) {
                    console.error('Error updating user:', error);
                }
            });
        } catch (error) {
            console.error('Error fetching user info:', error);
        }
    }

    // Delete User
    async function deleteUser(userId) {
        if (confirm('Are you sure you want to delete this user?')) {
            try {
                const response = await fetch(`https://localhost:7046/api/Admin/DeleteUser/${userId}`, {
                    method: 'DELETE'
                });
                if (response.ok) {
                    alert('User deleted successfully');
                    document.getElementById('user-info-link').click();
                } else {
                    alert('Error deleting user');
                }
            } catch (error) {
                console.error('Error deleting user:', error);
            }
        }
    }

    // Fetch ended auctions and display in table with email button
    async function messegeToWinners() {
        try {
            const response = await fetch(`https://localhost:7046/api/Admin/EndedAuction`);
            const data = await response.json();

            const tableBody = document.querySelector("#winners-table tbody");
            tableBody.innerHTML = "";  // Clear existing rows

            if (!data.endedAuctions || data.endedAuctions.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="6">No auctions have ended.</td></tr>';
                return;
            }

            data.endedAuctions.forEach(auction => {
                const row = document.createElement("tr");

                const auctionIdCell = document.createElement("td");
                auctionIdCell.textContent = auction.AuctionId;

                const productNameCell = document.createElement("td");
                productNameCell.textContent = auction.ProductName;

                const highestBidCell = document.createElement("td");
                highestBidCell.textContent = `$${auction.CurrentHighestBid.toFixed(2)}`;

                const highestBidderCell = document.createElement("td");
                highestBidderCell.textContent = auction.HighestBidder ? auction.HighestBidder.Username : "No Bidder";

                const bidderEmailCell = document.createElement("td");
                bidderEmailCell.textContent = auction.HighestBidder ? auction.HighestBidder.Email : "N/A";

                const actionCell = document.createElement("td");
                const emailButton = document.createElement("button");
                emailButton.textContent = "Send Email";
                emailButton.classList.add("btn", "btn-primary");
                emailButton.onclick = function () {
                    sendEmail(auction.HighestBidder.Email, auction.ProductName, auction.AuctionId);
                };
                actionCell.appendChild(emailButton);

                row.appendChild(auctionIdCell);
                row.appendChild(productNameCell);
                row.appendChild(highestBidCell);
                row.appendChild(highestBidderCell);
                row.appendChild(bidderEmailCell);
                row.appendChild(actionCell);

                tableBody.appendChild(row);
            });

            document.getElementById("messages-section").style.display = "block";
        } catch (error) {
            console.error("Error fetching or displaying auction data:", error);
        }
    }

    // Send email to highest bidder
    async function sendEmail(email, productName, auctionId) {
        try {
            const emailResponse = await fetch('https://localhost:7046/api/sendEmail', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email: email,
                    subject: `Auction ${auctionId} - ${productName} Winner`,
                    message: `Congratulations! You've won the auction for ${productName}.`
                })
            });

            if (!emailResponse.ok) {
                throw new Error('Failed to send email');
            }

            alert("Email sent successfully to " + email);
        } catch (error) {
            console.error("Error sending email:", error);
            alert("Failed to send email.");
        }
    }

    // Event listener to display winners section
    document.getElementById('winners-link').addEventListener('click', messegeToWinners);

    // Set default section
    document.getElementById('dashboard-link').click();
});

// Handle auction creation
document.getElementById('auctionForm').addEventListener('submit', async function(event) {
    event.preventDefault(); // Prevent the form from submitting in the traditional way

    const productId = document.getElementById('productIdInput').value;
    const startingPrice = document.getElementById('startingPriceInput').value;
    const duration = document.getElementById('durationInput').value;

    const auctionDto = {
        ProductId: parseInt(productId),
        StartingPrice: parseFloat(startingPrice),
        DurationHours: parseInt(duration)
    };

    try {
        const response = await fetch(`https://localhost:7046/api/Auction/CreateAuction`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(auctionDto)
        });

        if (response.ok) {
            alert('Auction created successfully.');
            var auctionModal = bootstrap.Modal.getInstance(document.getElementById('auctionModal'));
            auctionModal.hide(); 
            loadPendingProducts(); 
        } else {
            alert('Failed to create the auction.');
        }
    } catch (error) {
        console.error('Error creating auction:', error);
    }
});
