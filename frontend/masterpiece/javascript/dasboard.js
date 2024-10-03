document.addEventListener("DOMContentLoaded", function () {
    async function category() {
        debugger
        const select = document.getElementById("category");
    
        try {
            
            const response = await fetch('https://localhost:7046/api/Products/GetALLCategoryWithTottalProducts');
            
            // Ensure the response is successful
            if (!response.ok) {
                throw new Error("Failed to fetch categories.");
            }
    
            // Parse the JSON data from the response
            const data = await response.json();
            console.log(data);
    
            // Populate the <select> element with options dynamically
            data.forEach(element => {
                const option = document.createElement("option");
                option.text = element.categoryName; // Category name
                option.value = element.categoryId;  // Category ID
                select.add(option);
            });
        } catch (error) {
            // Handle any errors that occurred during the fetch operation
            console.error("Error fetching categories:", error);
        }
    }
    
    function handleSidebarClicks() {
        const sidebarLinks = document.querySelectorAll('.sidebar-menu a');
        
        sidebarLinks.forEach(link => {
            link.addEventListener('click', function () {
                // Remove 'active' class from all links
                sidebarLinks.forEach(link => link.parentElement.classList.remove('active'));
                // Add 'active' class to the clicked link
                this.parentElement.classList.add('active');

                // Get the name of the section to show (from the <span> inside the link)
                const sectionToShow = this.querySelector('span').innerText;

                // Hide all sections
                document.querySelectorAll('.dashboard-section').forEach(section => {
                    section.style.display = 'none';
                });

                // Show the corresponding section
                const sectionElement = document.getElementById(sectionToShow.replace(/\s/g, ''));
                if (sectionElement) {
                    sectionElement.style.display = 'block';
                } else {
                    console.error(`Section "${sectionToShow}" not found`);
                }
            });
        });
    }

    // Initialize the sidebar click handler
    

    async function getUserInfo() {
        const userId = localStorage.getItem("userId");
        
        if (!userId) {
            console.error('User ID not found in localStorage');
            return;
        }

        try {
            const response = await fetch(`https://localhost:7046/api/User/GetUserByID/${userId}`);
            if (!response.ok) {
                throw new Error('Failed to fetch user info');
            }
            const data = await response.json();
            console.log(data);
            //document.getElementById("userImg").src =`https://localhost:7046/${data.imageUrl}`

            document.getElementById("userImg").src = data.imageUrl.startsWith("/images/")
                ? `https://localhost:7046/${data.imageUrl}`
                : data.imageUrl;
            
            document.getElementById("userName").innerText = data.username;
            document.getElementById("userEmail").innerText = data.email;

            document.getElementById("usernameInput").value = data.username;
            document.getElementById("emailInput").value = data.email;
            document.getElementById("imageInput").value = data.imageUrl;
        } catch (e) {
            console.error('Error fetching user info:', e);
        }
    }

    async function fetchDashboardData() {
        try {
           
            const userId = localStorage.getItem("userId");
            const response = await fetch(`https://localhost:7046/api/User/GetUserDashboard?userId=${userId}`);
            
            if (!response.ok) {
                throw new Error('Failed to fetch dashboard data');
            }

            const data = await response.json();
            console.log('Dashboard data:', data);

            document.getElementById('activeBidsCount').innerText = data.activeBids || 0;
            document.getElementById('winningBidsCount').innerText = data.winningBids || 0;
            document.getElementById('favoritesCount').innerText = data.favorites || 0;

            const bidHistoryTableBody = document.getElementById('bidHistoryTableBody');
            bidHistoryTableBody.innerHTML = ''; 

            data.bidHistory.forEach((bid) => {
                const row = `
                    <tr>
                        <td>${bid.itemName}</td>
                        <td>$${bid.lastBid.toFixed(2)}</td>
                        <td>$${bid.openingBid.toFixed(2)}</td>
                        <td>${new Date(bid.endTime).toLocaleString()}</td>
                    </tr>
                `;
                bidHistoryTableBody.innerHTML += row;
            });
        } catch (error) {
            console.error('Error fetching dashboard data:', error);
        }
    }

    const editProfileForm = document.getElementById("editProfileForm");
    
    editProfileForm.addEventListener("submit", async function(event) {
        event.preventDefault(); 
        debugger
        const userId = localStorage.getItem("userId");
        if (!userId) {
            console.error('User ID not found in localStorage');
            return;
        }

        const formData = new FormData(editProfileForm);

        try {
            const response = await fetch(`https://localhost:7046/api/User/UpdateUserInfoWithImage/${userId}`, {
                method: 'POST',
                body: formData  
            });

            if (response.ok) {
                const responseData = await response.json();
                
                document.getElementById("profileUpdateSuccess").style.display = 'block';
                document.getElementById("profileUpdateError").style.display = 'none';

                document.getElementById("userName").innerText = formData.get("Username");
                document.getElementById("userImg").src = responseData.ImageUrl.startsWith("https")
                    ? responseData.ImageUrl
                    : `assets/images/${responseData.ImageUrl}`;
            } else {
                // Show error message
                document.getElementById("profileUpdateError").style.display = 'block';
                document.getElementById("profileUpdateSuccess").style.display = 'none';
            }
        } catch (e) {
            console.error('Error updating profile:', e);
            document.getElementById("profileUpdateError").style.display = 'block';
            document.getElementById("profileUpdateSuccess").style.display = 'none';
        }
    });
    const ChangePassword = document.getElementById("changePasswordForm");
ChangePassword.addEventListener("submit", async function(event) {
    event.preventDefault(); 
    debugger
    const userId = localStorage.getItem("userId");
    if (!userId) {
        console.error('User ID not found in localStorage');
        return;
    }

    const Password = document.getElementById("currentPasswordInput").value;
    const NewPassword = document.getElementById("newPasswordInput").value;
    const ConfirmPassword = document.getElementById("confirmPasswordInput").value;

    // Password validation regex
    const lengthRegex = /^.{8,}$/;
    const lowercaseRegex = /[a-z]/;
    const uppercaseRegex = /[A-Z]/;
    const digitRegex = /\d/;
    const specialCharRegex = /[@$!%*?&#]/;

    // Check password requirements
    if (!lengthRegex.test(NewPassword)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must be at least 8 characters long.";
        return false;
    }
    if (!lowercaseRegex.test(NewPassword)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one lowercase letter.";
        return false;
    }
    if (!uppercaseRegex.test(NewPassword)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one uppercase letter.";
        return false;
    }
    if (!digitRegex.test(NewPassword)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one digit.";
        return false;
    }
    if (!specialCharRegex.test(NewPassword)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one special character (@$!%*?&#).";
        return false;
    }
    if (NewPassword !== ConfirmPassword) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Passwords do not match.";
        return false;
    }

    try {
        // Create a plain JavaScript object for JSON
        const passwordData = {
            userID: userId,
            password: Password,
            confirmPassword: NewPassword
        };

        const response = await fetch(`https://localhost:7046/api/User/EditUserResetPassword`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'  
            },
            body: JSON.stringify(passwordData)
        });

        if (response.ok) {
            Swal.fire({
                icon: 'success',
                title: 'Password Updated',
                text: 'Your password has been successfully updated.',
                confirmButtonText: 'OK'
            }).then(() => {
                ConfirmPassword.value = "";
                NewPassword.value = "";
                Password.vlaue="";
            });
        } else {
            const errorData = await response.json();
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: errorData.message || 'An error occurred while updating the password.',
                confirmButtonText: 'OK'
            });
        }

    } catch (e) {
        console.error('Error updating password:', e);
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'An error occurred while trying to update your password.',
            confirmButtonText: 'OK'
        });
    }
});
async function winningBids() {
    const tableBody = document.getElementById("winingBids");
    const userId = localStorage.getItem("userId");
    if (!userId) {
        console.error('User ID not found in localStorage');
        return;
    }

    try {
        const response = await fetch(`https://localhost:7046/api/Bids/UserWinningBids/${userId}`);
        if (!response.ok) {
            throw new Error('Failed to fetch winning bids');
        }

        const data = await response.json();
        tableBody.innerHTML = ''; // Clear previous entries

        data.forEach((element, index) => {
            const row = `
                <tr>
                    <td>${index + 1}</td>
                    <td>${element.productName}</td>
                    <td>$${element.amount}</td>
                    <td>${element.productQuantity}</td>
                    <td>${element.productCondition}</td>
                    <td>${element.productDescription}</td>
                    <td>
                        <img src="${element.productImage}" alt="${element.productName}" class="img-thumbnail" 
                             title="${element.productName}">
                    </td>
                </tr>
            `;
            tableBody.innerHTML += row;
        });

        // Activate tooltips
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });

    } catch (e) {
        console.error('Error fetching winning bids:', e);
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'An error occurred while trying to fetch your winning bids.',
            confirmButtonText: 'OK'
        });
    }
}




   
    fetchDashboardData();
    getUserInfo();
    handleSidebarClicks();
    winningBids();
    category();
});
document.getElementById('productForm').addEventListener('submit', async function(event) {
    event.preventDefault();

    // Get the userId from local storage
    var userId = localStorage.getItem('userId');
    if (!userId) {
        alert("User is not logged in.");
        return;
    }

    // Create FormData object
    var formData = new FormData();

    formData.append('ProductName', document.getElementById('productName').value);
    formData.append('Description', document.getElementById('description').value);
    formData.append('StartingPrice', document.getElementById('startingPrice').value);
    formData.append('Stock', document.getElementById('stock').value);
    formData.append('Condition', document.getElementById('condition').value);
    formData.append('Location', document.getElementById('location').value);
    formData.append('Country', document.getElementById('country').value);
    formData.append('Brand', document.getElementById('brand').value);
    formData.append('CategoryId', document.getElementById('category').value);
    formData.append('Image', document.getElementById('image').files[0]);  
    formData.append('userId', userId);  

    try {
        const response = await fetch('https://localhost:7046/api/Products/PostProduct', {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            document.getElementById('productSubmitSuccess').style.display = 'block';
        } else {
            document.getElementById('productSubmitError').style.display = 'block';
        }
    } catch (error) {
        console.error('Error:', error);
        document.getElementById('productSubmitError').style.display = 'block';
    }
});
