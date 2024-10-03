async function resetPassword() {
    debugger
    const password=document.getElementById("NewPassword").value;
    const confirmPassword=document.getElementById("ConfirmPassword").value;
    const Otp=document.getElementById("newOtp").value;
    const lengthRegex = /^.{8,}$/;
    const lowercaseRegex = /[a-z]/;
    const uppercaseRegex = /[A-Z]/;
    const digitRegex = /\d/;
    const specialCharRegex = /[@$!%*?&#]/;
    if (!lengthRegex.test(password)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must be at least 8 characters long.";
        return false;
    }
    if (!lowercaseRegex.test(password)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one lowercase letter.";
        return false;
    }
    if (!uppercaseRegex.test(password)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one uppercase letter.";
        return false;
    }
    if (!digitRegex.test(password)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one digit.";
        return false;
    }
    if (!specialCharRegex.test(password)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one special character (@$!%*?&#).";
        return false;
    }

    
    if (password !== confirmPassword) {
        
        Swal.fire({
            icon: 'error',
            title: 'Passwords do not match!',
            text: 'Please ensure both password fields match.',
            confirmButtonText: 'OK'
        });
        return false;
    }
    try{
        const data={

            Otp:Otp,
            Password:password,
        };
        const response = await fetch("https://localhost:7046/api/User/ResetPassword", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            Swal.fire({
                icon: 'success',
                title: 'Password reset',
                text: 'Password reset successfully',
                 confirmButtonText: 'OK'
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = 'login.html'; 
                }
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'The verification code number is not correct. Please try again.',
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