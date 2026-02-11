const API_BASE = '/api';

function getStorage() {
    return localStorage.getItem('token') ? localStorage : sessionStorage;
}

const api = {
    getHeaders() {
        const headers = { 'Content-Type': 'application/json' };
        const token = getStorage().getItem('token');
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }
        return headers;
    },

    async get(endpoint) {
        const response = await fetch(`${API_BASE}/${endpoint}`, {
            headers: this.getHeaders()
        });
        if (response.status === 401) { logout(); return; }
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    async post(endpoint, data) {
        const response = await fetch(`${API_BASE}/${endpoint}`, {
            method: 'POST',
            headers: this.getHeaders(),
            body: JSON.stringify(data)
        });
        if (response.status === 401) { logout(); return; }
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    async put(endpoint, data) {
        const response = await fetch(`${API_BASE}/${endpoint}`, {
            method: 'PUT',
            headers: this.getHeaders(),
            body: JSON.stringify(data)
        });
        if (response.status === 401) { logout(); return; }
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    async delete(endpoint) {
        const response = await fetch(`${API_BASE}/${endpoint}`, {
            method: 'DELETE',
            headers: this.getHeaders()
        });
        if (response.status === 401) { logout(); return; }
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    }
};

function logout() {
    document.cookie = 'swagger_token=; Max-Age=0; path=/;';
    localStorage.clear();
    sessionStorage.clear();
    window.location.href = 'index.html';
}