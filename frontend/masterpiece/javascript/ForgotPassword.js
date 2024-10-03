// Function to request password reset
async function requestPasswordReset() {
    debugger
    const email = document.getElementById("ResetEmail").value;

    if (!email) {
        Swal.fire({
            icon: 'error',
            title: 'Email required',
            text: 'Please enter your email to receive a password reset link.',
        });
        return;
    }

    try {
        const response = await fetch("https://localhost:7046/api/User/ForgotPassword", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ Email: email })
        });

        if (response.ok) {
            Swal.fire({
                icon: 'success',
                title: 'Reset Link Sent!',
                text: 'Please check your email for the password reset link.',
                 confirmButtonText: 'OK'
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = 'passwordresetpage.html'; 
                }
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Failed to send reset link. Please try again.',
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
