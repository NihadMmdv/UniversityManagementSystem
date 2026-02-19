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
    },

    async uploadFile(endpoint, file) {
        const formData = new FormData();
        formData.append('file', file);
        const headers = {};
        const token = getStorage().getItem('token');
        if (token) headers['Authorization'] = `Bearer ${token}`;
        const response = await fetch(`${API_BASE}/${endpoint}`, {
            method: 'POST',
            headers,
            body: formData
        });
        if (response.status === 401) { logout(); return; }
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    async postForm(endpoint, formData) {
        const headers = {};
        const token = getStorage().getItem('token');
        if (token) headers['Authorization'] = `Bearer ${token}`;
        const response = await fetch(`${API_BASE}/${endpoint}`, {
            method: 'POST',
            headers,
            body: formData
        });
        if (response.status === 401) { logout(); return; }
        if (!response.ok) {
            const errorBody = await response.text();
            throw new Error(errorBody || `HTTP error! status: ${response.status}`);
        }
        return response.json();
    },

    async downloadFile(endpoint) {
        const headers = {};
        const token = getStorage().getItem('token');
        if (token) headers['Authorization'] = `Bearer ${token}`;
        const response = await fetch(`${API_BASE}/${endpoint}`, { headers });
        if (response.status === 401) { logout(); return; }
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);

        const disposition = response.headers.get('Content-Disposition');
        let fileName = 'download';
        if (disposition) {
            const match = disposition.match(/filename\*?=(?:UTF-8'')?["']?([^;"'\n]+)/i);
            if (match) fileName = decodeURIComponent(match[1]);
        }

        const blob = await response.blob();
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        a.remove();
        URL.revokeObjectURL(url);
    }
};

async function logout() {
    document.cookie = 'swagger_token=; Max-Age=0; path=/;';
    try {
        await fetch(`${API_BASE}/Auth/logout`, { method: 'POST' });
    } catch { /* ignore */ }
    localStorage.clear();
    sessionStorage.clear();
    window.location.href = 'index.html';
}