async function login() {
    event.preventDefault();
    debugger
    const email = document.getElementById("LoginEmail").value;
    const password = document.getElementById("LoginPassword").value;

    const userData = {
        Email: email,
        Password: password
    };

    try {
        const response = await fetch("https://localhost:7046/api/User/Login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(userData),
        });

        if (response.ok) {
            debugger
            const data = await response.json();
            localStorage.setItem("token", data.token);
            localStorage.setItem("userId", data.userId);
            localStorage.setItem("username", data.username);
            localStorage.setItem("email", data.email);
            localStorage.setItem("isAdmin", data.isAdmin);  // Store admin status

            if (data.isAdmin) {
                
                Swal.fire({
                    icon: 'success',
                    title: 'Login Successful!',
                    text: 'You are logged in as an admin. Where would you like to go?',
                    showDenyButton: true,
                    confirmButtonText: 'Go to Admin Dashboard',
                    denyButtonText: 'Continue to Homepage',
                }).then((result) => {
                    if (result.isConfirmed) {
                        // Go to admin dashboard
                        window.location.href = 'adimin_dashboard.html';
                    } else if (result.isDenied) {
                        // Continue to the homepage
                        window.location.href = 'index.html';
                    }
                });
            } else {
                // If not admin, use the existing logic
                Swal.fire({
                    icon: 'success',
                    title: 'Login Successful!',
                    text: 'You will be redirected shortly.',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed && localStorage.getItem("trytoBid") == null) {
                        window.location.href = 'index.html';
                    } else if (result.isConfirmed && localStorage.getItem("trytoBid") == "true") {
                        window.location.href = localStorage.getItem("savedUrl");
                    }
                });
            }
        } else {
            let errorMessage = 'Invalid email or password. Please try again.';
            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorMessage; 
            } catch (err) {
                console.error('Failed to parse error response:', err);
            }

            Swal.fire({
                icon: 'error',
                title: 'Login Failed',
                text: errorMessage,
            });
        }
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Connection Error',
            text: 'Unable to connect to the server. Please try again later.',
        });
        console.error("Error:", error);
    }
}
