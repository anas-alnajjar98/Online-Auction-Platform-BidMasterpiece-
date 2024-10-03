document.addEventListener("DOMContentLoaded", function(
){
    const loginLink = document.getElementById('login-link');
    const registerLink = document.getElementById('register-link');
    const Userid=localStorage.getItem('userId');
    const dasbord=document.getElementById('Dashboard');
    const dasbord2=document.getElementById('Dashbord2');
    if (Userid) {
        // User is logged in
        loginLink.style.display = 'none';
        registerLink.style.display = 'none';
        dasbord.style.display = 'block';
        dasbord2.style.display = 'block';
        document.getElementById('logout-link').style.display = 'block';
    } else {
        // User is not logged in
        loginLink.style.display = 'block';
        registerLink.style.display = 'block';
        dasbord.style.display = 'none';
        dasbord2.style.display = 'none';
        document.getElementById('logout-link').style.display = 'none';
    }
    const logoutLink = document.getElementById('logout-link');
    logoutLink.addEventListener('click', function () {
        localStorage.removeItem('userId'); // Remove user ID
        localStorage.removeItem('Token'); // Remove token
        localStorage.removeItem('user');
        window.location.href = 'login.html'; // Redirect to login page
    });
});