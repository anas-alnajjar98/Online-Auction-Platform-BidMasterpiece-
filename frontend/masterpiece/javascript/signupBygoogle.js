import { initializeApp } from "https://www.gstatic.com/firebasejs/10.13.1/firebase-app.js";
import {
  getAuth,
  GoogleAuthProvider,
  signInWithPopup,
} from "https://www.gstatic.com/firebasejs/10.13.1/firebase-auth.js";

// Your Firebase configuration
const firebaseConfig = {
  apiKey: "AIzaSyAv5vCoLdqubm_IJAOjNlgF7o9zo-1-VfE",
  authDomain: "login-3e8e4.firebaseapp.com",
  projectId: "login-3e8e4",
  storageBucket: "login-3e8e4.appspot.com",
  messagingSenderId: "251369161445",
  appId: "1:251369161445:web:ef0c157a6b0cdcdb1a0a0c",
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const auth = getAuth(app);
auth.languageCode = "en";
const provider = new GoogleAuthProvider();

// Get the Google login button element
const googleLogin = document.getElementById("google-login-btn");

if (googleLogin) {
  googleLogin.addEventListener("click", async function (event) {
    debugger
    event.preventDefault(); // Prevent default anchor behavior

    try {
      const result = await signInWithPopup(auth, provider);

      // The signed-in user info.
      const user = result.user;

      // Extract user details
      const { uid, displayName, email, photoURL } = user;

      
      const userData = {
        Username: displayName || email, 
        Email: email,
        Password: uid, // Ensure security handling of password
        ImageUrl: photoURL || "", // If no image, pass an empty string
      };

      
      const response = await fetch(
        "https://localhost:7046/api/User/GoogleSignUp",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(userData), 
        }
      );

      
      if (!response.ok) {
        const errorText = await response.text();
        console.error(`HTTP error! Status: ${response.status}, Message: ${errorText}`);
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      
      const data = await response.json();

      
      if (data.token && data.userId) {
        localStorage.setItem("Token", data.token);
        localStorage.setItem("userId", data.userId);

       
        Swal.fire({
          icon: 'success',
          title: 'Signup Successful!',
          text: 'You have successfully signed up. Redirecting to the homepage...',
          showConfirmButton: false,
          timer: 2000
        }).then(() => {
         
          window.location.href = "index.html";
        });
      } else {
        throw new Error("Invalid response: Token or User ID missing");
      }
    } catch (error) {
      
      Swal.fire({
        icon: 'error',
        title: 'Error!',
        text: 'Error during login. Please try again.',
        confirmButtonText: 'OK'
      });
      console.error("Error during login or API request:", error);
    }
  });
} else {
  console.error("Login button not found");
}
