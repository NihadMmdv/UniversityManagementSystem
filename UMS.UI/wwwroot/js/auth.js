// Check if already logged in
document.addEventListener('DOMContentLoaded', () => {
    // Check localStorage (remembered) or sessionStorage (session only)
    const token = localStorage.getItem('token') || sessionStorage.getItem('token');
    const role = localStorage.getItem('userRole') || sessionStorage.getItem('userRole');
    
    if (token) {
        window.location.href = role === 'Admin' ? 'dashboard.html' : 'landing.html';
    }
});

document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const rememberMe = document.getElementById('rememberMe').checked;
    const errorDiv = document.getElementById('errorMessage');
    
    errorDiv.classList.add('d-none');
    
    try {
        const response = await fetch('/api/Auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });
        
        if (!response.ok) {
            const err = await response.json();
            throw new Error(err.message || t('login.failed'));
        }
        
        const data = await response.json();
        
        // Use localStorage if "Remember Me" checked, otherwise sessionStorage
        const storage = rememberMe ? localStorage : sessionStorage;
        storage.setItem('token', data.token);
        storage.setItem('userEmail', data.email);
        storage.setItem('userRole', data.role);
        storage.setItem('userName', data.name);
        
        window.location.href = data.role === 'Admin' ? 'dashboard.html' : 'landing.html';
        
    } catch (error) {
        errorDiv.textContent = error.message;
        errorDiv.classList.remove('d-none');
    }
});