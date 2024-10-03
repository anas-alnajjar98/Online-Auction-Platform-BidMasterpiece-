async function Signup() {
    event.preventDefault(); 
    
   
    const PasswordForSignUp = document.getElementById("PasswordForSignUp").value;
    const RePasswordForSignUp = document.getElementById("RePasswordForSignUp").value;
    const UsernameForSignUp = document.getElementById("Username").value;
    const EmailForSignUp = document.getElementById("Email").value;

    
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    const usernameRegex = /^[a-zA-Z][a-zA-Z0-9_]{5,19}$/;
    const lengthRegex = /^.{8,}$/;
    const lowercaseRegex = /[a-z]/;
    const uppercaseRegex = /[A-Z]/;
    const digitRegex = /\d/;
    const specialCharRegex = /[@$!%*?&#]/;
    document.getElementById("usernameHelp").style.display = "none";
    document.getElementById("emailHelp").style.display = "none";
    document.getElementById("passwordHelp").style.display = "none";

    
    if (!usernameRegex.test(UsernameForSignUp)) {
        document.getElementById("usernameHelp").style.display = "block";
        document.getElementById("usernameHelp").innerHTML = "Username must start with a letter and be 6-20 characters long.";
        return false;
    }

    if (!emailRegex.test(EmailForSignUp)) {
        document.getElementById("emailHelp").style.display = "block";
        document.getElementById("emailHelp").innerHTML = "Please enter a valid email address.";
        return false;
    }

    if (!lengthRegex.test(PasswordForSignUp)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must be at least 8 characters long.";
        return false;
    }
    if (!lowercaseRegex.test(PasswordForSignUp)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one lowercase letter.";
        return false;
    }
    if (!uppercaseRegex.test(PasswordForSignUp)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one uppercase letter.";
        return false;
    }
    if (!digitRegex.test(PasswordForSignUp)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one digit.";
        return false;
    }
    if (!specialCharRegex.test(PasswordForSignUp)) {
        document.getElementById("passwordHelp").style.display = "block";
        document.getElementById("passwordHelp").innerHTML = "Password must contain at least one special character (@$!%*?&#).";
        return false;
    }

    
    if (PasswordForSignUp !== RePasswordForSignUp) {
        
        Swal.fire({
            icon: 'error',
            title: 'Passwords do not match!',
            text: 'Please ensure both password fields match.',
            confirmButtonText: 'OK'
        });
        return false;
    }

    
    const userData = {
        Username: UsernameForSignUp,
        Email: EmailForSignUp,
        Password: PasswordForSignUp,
    };

    try {
        const response = await fetch("https://localhost:7046/api/User/register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(userData),
        });

        if (response.ok) {
           
            Swal.fire({
                icon: 'success',
                title: 'Signup Successful!',
                text: 'Your account has been registered.',
                confirmButtonText: 'OK'
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = 'index.html'; 
                }
            });
        } else {
            
            let errorMessage = 'An error occurred during registration.';
            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorMessage; 
            } catch (err) {
                console.error('Failed to parse error response:', err);
            }

            
            document.getElementById("emailHelp").style.display = "block";
            document.getElementById("emailHelp").innerHTML = errorMessage;
        }
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error!',
            text: 'Unable to connect to the server. Please try again later.',
        });
        console.error("Error:", error);
    }
}
