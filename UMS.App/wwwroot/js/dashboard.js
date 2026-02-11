// Check authentication
const storage = localStorage.getItem('token') ? localStorage : sessionStorage;

// ── Lazy cache ──
const cache = {
    students: { data: null, stale: true },
    teachers: { data: null, stale: true },
    lessons: { data: null, stale: true },
    sections: { data: null, stale: true },
    exams: { data: null, stale: true },
};

const endpointMap = {
    students: 'Student',
    teachers: 'Teacher',
    lessons: 'Lesson',
    sections: 'Section',
    exams: 'Exam',
};

async function getCached(key) {
    const entry = cache[key];
    if (!entry.stale && entry.data !== null) return entry.data;
    entry.data = await api.get(endpointMap[key]) || [];
    entry.stale = false;
    return entry.data;
}

function invalidate(...keys) {
    keys.forEach(k => { if (cache[k]) cache[k].stale = true; });
}

// ── Helpers ──
function escHtml(str) {
    if (!str) return '';
    return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

function lookupName(list, id, fallback = 'Unknown') {
    const item = list.find(x => x.id === id);
    if (!item) return fallback;
    return item.name + (item.surname ? ` ${item.surname}` : '');
}

function toPickerItems(list) {
    return list.map(x => ({
        id: x.id,
        label: x.name + (x.surname ? ` ${x.surname}` : '')
    }));
}

// ── Collapsible name badges ──
let _collapseId = 0;

function renderNames(list, ids, max = 3) {
    if (!ids || ids.length === 0) return '<span class="text-muted">-</span>';
    const badges = ids.map(id => {
        const n = lookupName(list, id, `#${id}`);
        return `<span class="badge bg-secondary bg-opacity-75 me-1 mb-1">${escHtml(n)}</span>`;
    });
    if (badges.length <= max) return `<div class="names-cell">${badges.join('')}</div>`;
    const uid = 'nc' + (++_collapseId);
    return `<div><div class="names-cell">${badges.slice(0, max).join('')} <a href="#" class="badge bg-primary names-toggle" data-target="${uid}">+${badges.length - max} more</a></div><div class="names-extra d-none" id="${uid}">${badges.slice(max).join('')}</div></div>`;
}

// Global toggle handler for collapsible names
document.addEventListener('click', (e) => {
    const btn = e.target.closest('.names-toggle');
    if (!btn) return;
    e.preventDefault();
    const extra = document.getElementById(btn.dataset.target);
    if (!extra) return;
    const nowHidden = extra.classList.toggle('d-none');
    const count = extra.querySelectorAll('.badge').length;
    btn.textContent = nowHidden ? `+${count} more` : 'show less';
});

// ── Smart Picker (search dropdown with 3+ char trigger) ──
const pickers = {};

function initPicker(id, { multi = true, placeholder = 'Type 3+ letters to search...' } = {}) {
    const container = document.getElementById(id);
    if (!container) return;

    const tagsId = `${id}_tags`;
    const inputId = `${id}_input`;
    const ddId = `${id}_dd`;

    container.innerHTML = `
        <div class="picker-tags" id="${tagsId}"></div>
        <div class="picker-wrap">
            <input type="text" class="form-control form-control-sm" placeholder="${placeholder}" id="${inputId}" autocomplete="off">
            <div class="picker-dropdown" id="${ddId}"></div>
        </div>`;

    const state = { items: [], selected: new Map(), multi, id };
    pickers[id] = state;

    const input = document.getElementById(inputId);
    const dd = document.getElementById(ddId);

    input.addEventListener('input', () => {
        const q = input.value.trim().toLowerCase();
        if (q.length < 3) { dd.innerHTML = ''; dd.classList.remove('show'); return; }

        const matches = state.items
            .filter(i => !state.selected.has(i.id) &&
                i.label.toLowerCase().split(/\s+/).some(w => w.startsWith(q)))
            .slice(0, 10);

        dd.innerHTML = matches.length === 0
            ? '<div class="picker-item disabled">No results</div>'
            : matches.map(i => `<div class="picker-item" data-id="${i.id}">${escHtml(i.label)}</div>`).join('');
        dd.classList.add('show');
    });

    dd.addEventListener('mousedown', (e) => {
        const el = e.target.closest('.picker-item[data-id]');
        if (!el) return;
        e.preventDefault();
        const itemId = parseInt(el.dataset.id);
        const item = state.items.find(i => i.id === itemId);
        if (!item) return;

        if (!state.multi) state.selected.clear();
        state.selected.set(itemId, item.label);
        input.value = '';
        dd.innerHTML = '';
        dd.classList.remove('show');
        renderPickerTags(state);
    });

    input.addEventListener('blur', () => {
        setTimeout(() => dd.classList.remove('show'), 150);
    });
}

function renderPickerTags(state) {
    const tags = document.getElementById(`${state.id}_tags`);
    if (!tags) return;
    tags.innerHTML = [...state.selected.entries()].map(([id, label]) =>
        `<span class="badge bg-primary me-1 mb-1 picker-tag">${escHtml(label)} <span class="picker-remove" data-id="${id}" role="button">&times;</span></span>`
    ).join('');

    tags.querySelectorAll('.picker-remove').forEach(btn => {
        btn.addEventListener('click', (e) => {
            e.stopPropagation();
            state.selected.delete(parseInt(btn.dataset.id));
            renderPickerTags(state);
        });
    });
}

function resetPicker(id, items, selectedIds) {
    const state = pickers[id];
    if (!state) return;
    state.items = items;
    state.selected.clear();
    (selectedIds || []).forEach(sid => {
        const item = items.find(i => i.id === sid);
        if (item) state.selected.set(sid, item.label);
    });
    const input = document.getElementById(`${id}_input`);
    const dd = document.getElementById(`${id}_dd`);
    if (input) input.value = '';
    if (dd) { dd.innerHTML = ''; dd.classList.remove('show'); }
    renderPickerTags(state);
}

function getPickerIds(id) {
    return pickers[id] ? [...pickers[id].selected.keys()] : [];
}

// ── Table search ──
function setupSearch(inputId, tableId) {
    const input = document.getElementById(inputId);
    if (!input) return;
    input.addEventListener('input', () => {
        const q = input.value.toLowerCase();
        document.querySelectorAll(`#${tableId} tr`).forEach(row => {
            row.style.display = row.textContent.toLowerCase().includes(q) ? '' : 'none';
        });
    });
}

// ── Auth guard & init ──
document.addEventListener('DOMContentLoaded', () => {
    const token = storage.getItem('token');
    const role = storage.getItem('userRole');

    if (!token) { window.location.href = 'index.html'; return; }
    if (role !== 'Admin') { window.location.href = 'landing.html'; return; }

    document.getElementById('userEmail').textContent = storage.getItem('userEmail');

    const swaggerBtn = document.getElementById('swaggerBtn');
    if (swaggerBtn) {
        const url = `/swagger/index.html?token=${encodeURIComponent(token)}`;
        swaggerBtn.addEventListener('click', (e) => { e.preventDefault(); window.open(url, '_blank'); });
    }

    // Initialize all pickers once
    initPicker('studentSectionPicker', { multi: false });
    initPicker('teacherLessonPicker', { multi: true });
    initPicker('lessonSectionPicker', { multi: true });
    initPicker('lessonTeacherPicker', { multi: true });
    initPicker('sectionStudentPicker', { multi: true });
    initPicker('sectionLessonPicker', { multi: true });
    initPicker('examStudentPicker', { multi: false });
    initPicker('examLessonPicker', { multi: false });

    // Initialize search bars
    setupSearch('studentsSearch', 'studentsTable');
    setupSearch('teachersSearch', 'teachersTable');
    setupSearch('lessonsSearch', 'lessonsTable');
    setupSearch('sectionsSearch', 'sectionsTable');
    setupSearch('examsSearch', 'examsTable');

    loadDashboard();
});

// Logout & Settings
document.getElementById('logoutBtn').addEventListener('click', (e) => { e.preventDefault(); logout(); });

document.getElementById('changePasswordBtn').addEventListener('click', (e) => {
    e.preventDefault();
    document.getElementById('currentPassword').value = '';
    document.getElementById('newPassword').value = '';
    document.getElementById('confirmPassword').value = '';
    document.getElementById('passwordError').classList.add('d-none');
    document.getElementById('passwordSuccess').classList.add('d-none');
    new bootstrap.Modal(document.getElementById('changePasswordModal')).show();
});

document.getElementById('savePasswordBtn').addEventListener('click', async () => {
    const errorDiv = document.getElementById('passwordError');
    const successDiv = document.getElementById('passwordSuccess');
    errorDiv.classList.add('d-none');
    successDiv.classList.add('d-none');

    const newPass = document.getElementById('newPassword').value;
    const confirmVal = document.getElementById('confirmPassword').value;

    if (newPass !== confirmVal) {
        errorDiv.textContent = 'New passwords do not match';
        errorDiv.classList.remove('d-none');
        return;
    }
    if (newPass.length < 6) {
        errorDiv.textContent = 'Password must be at least 6 characters';
        errorDiv.classList.remove('d-none');
        return;
    }

    try {
        await api.post('Auth/change-password', {
            currentPassword: document.getElementById('currentPassword').value,
            newPassword: newPass
        });
        successDiv.classList.remove('d-none');
        setTimeout(() => bootstrap.Modal.getInstance(document.getElementById('changePasswordModal')).hide(), 1500);
    } catch (e) {
        errorDiv.textContent = 'Current password is incorrect';
        errorDiv.classList.remove('d-none');
    }
});

// Navigation
document.querySelectorAll('[data-section]').forEach(link => {
    link.addEventListener('click', (e) => {
        e.preventDefault();
        const section = e.target.closest('[data-section]').dataset.section;
        showSection(section);
    });
});

function showSection(name) {
    document.querySelectorAll('.section').forEach(s => s.classList.add('d-none'));
    document.getElementById(`${name}-section`)?.classList.remove('d-none');
    document.querySelectorAll('.nav-link').forEach(l => l.classList.remove('active'));
    document.querySelector(`[data-section="${name}"]`)?.classList.add('active');

    // Clear search when switching tabs
    const searchInput = document.getElementById(`${name}Search`);
    if (searchInput) searchInput.value = '';

    const loaders = { dashboard: loadDashboard, students: loadStudents, teachers: loadTeachers, lessons: loadLessons, sections: loadSections, exams: loadExams };
    loaders[name]?.();
}

function showToast(title, message, success = true) {
    document.getElementById('toastTitle').textContent = title;
    document.getElementById('toastBody').textContent = message;
    const el = document.getElementById('toast');
    el.className = `toast text-white ${success ? 'bg-success' : 'bg-danger'}`;
    new bootstrap.Toast(el).show();
}

// ── Dashboard ──
async function loadDashboard() {
    const [students, teachers, lessons, sections] = await Promise.all([
        getCached('students'), getCached('teachers'), getCached('lessons'), getCached('sections')
    ]);
    document.getElementById('studentCount').textContent = students.length;
    document.getElementById('teacherCount').textContent = teachers.length;
    document.getElementById('lessonCount').textContent = lessons.length;
    document.getElementById('sectionCount').textContent = sections.length;
}

// ── Students ──
async function loadStudents() {
    const [data, sections] = await Promise.all([
        getCached('students'), getCached('sections')
    ]);
    document.getElementById('studentsTable').innerHTML = data.map(s => `
        <tr>
            <td>${s.id}</td><td>${escHtml(s.name)}</td><td>${escHtml(s.surname)}</td><td>${escHtml(s.email)}</td><td>${escHtml(s.phone)}</td>
            <td>${s.dateOfBirth}</td>
            <td><span class="badge bg-secondary bg-opacity-75">${escHtml(lookupName(sections, s.sectionId, '#' + s.sectionId))}</span></td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editStudent(${s.id})"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" onclick="deleteStudent(${s.id})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>`).join('');
}

async function openStudentModal(data = null) {
    const sections = await getCached('sections');
    const items = toPickerItems(sections);

    document.getElementById('studentModalTitle').textContent = data ? 'Edit Student' : 'Add Student';
    document.getElementById('studentId').value = data?.id || '';
    document.getElementById('studentName').value = data?.name || '';
    document.getElementById('studentSurname').value = data?.surname || '';
    document.getElementById('studentEmail').value = data?.email || '';
    document.getElementById('studentPhone').value = data?.phone || '';
    document.getElementById('studentPhoto').value = data?.photoUrl || '';
    document.getElementById('studentDob').value = data?.dateOfBirth || '';
    document.getElementById('studentEnrollment').value = data?.enrollmentDate || '';
    resetPicker('studentSectionPicker', items, data?.sectionId ? [data.sectionId] : []);
    new bootstrap.Modal(document.getElementById('studentModal')).show();
}

async function editStudent(id) {
    const data = await api.get(`Student/${id}`);
    if (data) openStudentModal(data);
}

async function saveStudent() {
    const id = document.getElementById('studentId').value;
    const sectionIds = getPickerIds('studentSectionPicker');
    const body = {
        name: document.getElementById('studentName').value,
        surname: document.getElementById('studentSurname').value,
        email: document.getElementById('studentEmail').value,
        phone: document.getElementById('studentPhone').value,
        photoUrl: document.getElementById('studentPhoto').value,
        dateOfBirth: document.getElementById('studentDob').value,
        enrollmentDate: document.getElementById('studentEnrollment').value,
        sectionId: sectionIds[0] || 0
    };
    try {
        if (id) await api.put(`Student/${id}`, body);
        else await api.post('Student', body);
        bootstrap.Modal.getInstance(document.getElementById('studentModal')).hide();
        showToast('Student', id ? 'Updated successfully' : 'Created successfully');
        invalidate('students', 'sections');
        loadStudents(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteStudent(id) {
    if (!confirm('Delete this student?')) return;
    try {
        await api.delete(`Student/${id}`);
        showToast('Student', 'Deleted');
        invalidate('students', 'sections');
        loadStudents(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

// ── Teachers ──
async function loadTeachers() {
    const [data, lessons] = await Promise.all([
        getCached('teachers'), getCached('lessons')
    ]);
    document.getElementById('teachersTable').innerHTML = data.map(t => `
        <tr>
            <td>${t.id}</td><td>${escHtml(t.name)}</td><td>${escHtml(t.surname)}</td><td>${escHtml(t.email)}</td><td>${escHtml(t.phone)}</td>
            <td>${t.dateOfBirth}</td>
            <td>${t.salary}</td>
            <td>${renderNames(lessons, t.lessonIds)}</td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editTeacher(${t.id})"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" onclick="deleteTeacher(${t.id})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>`).join('');
}

async function openTeacherModal(data = null) {
    const lessons = await getCached('lessons');
    const items = toPickerItems(lessons);

    document.getElementById('teacherModalTitle').textContent = data ? 'Edit Teacher' : 'Add Teacher';
    document.getElementById('teacherId').value = data?.id || '';
    document.getElementById('teacherName').value = data?.name || '';
    document.getElementById('teacherSurname').value = data?.surname || '';
    document.getElementById('teacherEmail').value = data?.email || '';
    document.getElementById('teacherPhone').value = data?.phone || '';
    document.getElementById('teacherPhoto').value = data?.photoUrl || '';
    document.getElementById('teacherDob').value = data?.dateOfBirth || '';
    document.getElementById('teacherSalary').value = data?.salary || '';
    resetPicker('teacherLessonPicker', items, data?.lessonIds || []);
    new bootstrap.Modal(document.getElementById('teacherModal')).show();
}

async function editTeacher(id) {
    const data = await api.get(`Teacher/${id}`);
    if (data) openTeacherModal(data);
}

async function saveTeacher() {
    const id = document.getElementById('teacherId').value;
    const body = {
        name: document.getElementById('teacherName').value,
        surname: document.getElementById('teacherSurname').value,
        email: document.getElementById('teacherEmail').value,
        phone: document.getElementById('teacherPhone').value,
        photoUrl: document.getElementById('teacherPhoto').value,
        dateOfBirth: document.getElementById('teacherDob').value,
        salary: parseInt(document.getElementById('teacherSalary').value),
        lessonIds: getPickerIds('teacherLessonPicker')
    };
    try {
        if (id) await api.put(`Teacher/${id}`, body);
        else await api.post('Teacher', body);
        bootstrap.Modal.getInstance(document.getElementById('teacherModal')).hide();
        showToast('Teacher', id ? 'Updated successfully' : 'Created successfully');
        invalidate('teachers', 'lessons');
        loadTeachers(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteTeacher(id) {
    if (!confirm('Delete this teacher?')) return;
    try {
        await api.delete(`Teacher/${id}`);
        showToast('Teacher', 'Deleted');
        invalidate('teachers', 'lessons');
        loadTeachers(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

// ── Lessons ──
async function loadLessons() {
    const [data, teachers, sections] = await Promise.all([
        getCached('lessons'), getCached('teachers'), getCached('sections')
    ]);
    document.getElementById('lessonsTable').innerHTML = data.map(l => `
        <tr>
            <td>${l.id}</td><td>${escHtml(l.name)}</td>
            <td>${renderNames(teachers, l.teacherIds)}</td>
            <td>${renderNames(sections, l.sectionIds)}</td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editLesson(${l.id})"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" onclick="deleteLesson(${l.id})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>`).join('');
}

async function openLessonModal(data = null) {
    const [sections, teachers] = await Promise.all([
        getCached('sections'), getCached('teachers')
    ]);

    document.getElementById('lessonModalTitle').textContent = data ? 'Edit Lesson' : 'Add Lesson';
    document.getElementById('lessonId').value = data?.id || '';
    document.getElementById('lessonName').value = data?.name || '';
    resetPicker('lessonSectionPicker', toPickerItems(sections), data?.sectionIds || []);
    resetPicker('lessonTeacherPicker', toPickerItems(teachers), data?.teacherIds || []);
    new bootstrap.Modal(document.getElementById('lessonModal')).show();
}

async function editLesson(id) {
    const data = await api.get(`Lesson/${id}`);
    if (data) openLessonModal(data);
}

async function saveLesson() {
    const id = document.getElementById('lessonId').value;
    const body = {
        name: document.getElementById('lessonName').value,
        sectionIds: getPickerIds('lessonSectionPicker'),
        teacherIds: getPickerIds('lessonTeacherPicker')
    };
    try {
        if (id) await api.put(`Lesson/${id}`, body);
        else await api.post('Lesson', body);
        bootstrap.Modal.getInstance(document.getElementById('lessonModal')).hide();
        showToast('Lesson', id ? 'Updated successfully' : 'Created successfully');
        invalidate('lessons', 'teachers', 'sections');
        loadLessons(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteLesson(id) {
    if (!confirm('Delete this lesson?')) return;
    try {
        await api.delete(`Lesson/${id}`);
        showToast('Lesson', 'Deleted');
        invalidate('lessons', 'teachers', 'sections');
        loadLessons(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

// ── Sections / Classes ──
async function loadSections() {
    const [data, students, lessons] = await Promise.all([
        getCached('sections'), getCached('students'), getCached('lessons')
    ]);
    document.getElementById('sectionsTable').innerHTML = data.map(s => `
        <tr>
            <td>${s.id}</td><td>${escHtml(s.name)}</td>
            <td>${renderNames(students, s.studentIds)}</td>
            <td>${renderNames(lessons, s.lessonIds)}</td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editSection(${s.id})"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" onclick="deleteSection(${s.id})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>`).join('');
}

async function openSectionModal(data = null) {
    const [students, lessons] = await Promise.all([
        getCached('students'), getCached('lessons')
    ]);

    document.getElementById('sectionModalTitle').textContent = data ? 'Edit Class' : 'Add Class';
    document.getElementById('sectionId').value = data?.id || '';
    document.getElementById('sectionName').value = data?.name || '';
    resetPicker('sectionStudentPicker', toPickerItems(students), data?.studentIds || []);
    resetPicker('sectionLessonPicker', toPickerItems(lessons), data?.lessonIds || []);
    new bootstrap.Modal(document.getElementById('sectionModal')).show();
}

async function editSection(id) {
    const data = await api.get(`Section/${id}`);
    if (data) openSectionModal(data);
}

async function saveSection() {
    const id = document.getElementById('sectionId').value;
    const body = {
        name: document.getElementById('sectionName').value,
        studentIds: getPickerIds('sectionStudentPicker'),
        lessonIds: getPickerIds('sectionLessonPicker')
    };
    try {
        if (id) await api.put(`Section/${id}`, body);
        else await api.post('Section', body);
        bootstrap.Modal.getInstance(document.getElementById('sectionModal')).hide();
        showToast('Class', id ? 'Updated successfully' : 'Created successfully');
        invalidate('sections', 'students', 'lessons');
        loadSections(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteSection(id) {
    if (!confirm('Delete this class?')) return;
    try {
        await api.delete(`Section/${id}`);
        showToast('Class', 'Deleted');
        invalidate('sections', 'students', 'lessons');
        loadSections(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

// ── Exams ──
async function loadExams() {
    const [data, students, lessons] = await Promise.all([
        getCached('exams'), getCached('students'), getCached('lessons')
    ]);
    document.getElementById('examsTable').innerHTML = data.map(e => `
        <tr>
            <td>${e.id}</td>
            <td><span class="badge bg-secondary bg-opacity-75">${escHtml(lookupName(students, e.studentId, '#' + e.studentId))}</span></td>
            <td><span class="badge bg-secondary bg-opacity-75">${escHtml(lookupName(lessons, e.lessonId, '#' + e.lessonId))}</span></td>
            <td>${e.examDate}</td><td>${e.score}</td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editExam(${e.id})"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" onclick="deleteExam(${e.id})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>`).join('');
}

async function openExamModal(data = null) {
    const [students, lessons] = await Promise.all([
        getCached('students'), getCached('lessons')
    ]);

    document.getElementById('examModalTitle').textContent = data ? 'Edit Exam' : 'Add Exam';
    document.getElementById('examId').value = data?.id || '';
    document.getElementById('examDate').value = data?.examDate || '';
    document.getElementById('examScore').value = data?.score || '';
    resetPicker('examStudentPicker', toPickerItems(students), data?.studentId ? [data.studentId] : []);
    resetPicker('examLessonPicker', toPickerItems(lessons), data?.lessonId ? [data.lessonId] : []);
    new bootstrap.Modal(document.getElementById('examModal')).show();
}

async function editExam(id) {
    const data = await api.get(`Exam/${id}`);
    if (data) openExamModal(data);
}

async function saveExam() {
    const id = document.getElementById('examId').value;
    const studentIds = getPickerIds('examStudentPicker');
    const lessonIds = getPickerIds('examLessonPicker');
    const body = {
        studentId: studentIds[0] || 0,
        lessonId: lessonIds[0] || 0,
        examDate: document.getElementById('examDate').value,
        score: parseInt(document.getElementById('examScore').value)
    };
    try {
        if (id) await api.put(`Exam/${id}`, body);
        else await api.post('Exam', body);
        bootstrap.Modal.getInstance(document.getElementById('examModal')).hide();
        showToast('Exam', id ? 'Updated successfully' : 'Created successfully');
        invalidate('exams');
        loadExams(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteExam(id) {
    if (!confirm('Delete this exam?')) return;
    try {
        await api.delete(`Exam/${id}`);
        showToast('Exam', 'Deleted');
        invalidate('exams');
        loadExams(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}