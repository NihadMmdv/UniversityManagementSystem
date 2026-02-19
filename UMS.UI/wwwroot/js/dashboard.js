// Check authentication
const storage = localStorage.getItem('token') ? localStorage : sessionStorage;

// ── Lazy cache ──
const cache = {
    students: { data: null, stale: true },
    teachers: { data: null, stale: true },
    lessons: { data: null, stale: true },
    sections: { data: null, stale: true },
    exams: { data: null, stale: true },
    schedules: { data: null, stale: true },
};

const endpointMap = {
    students: 'Student',
    teachers: 'Teacher',
    lessons: 'Lesson',
    sections: 'Section',
    exams: 'Exam',
    schedules: 'Schedule',
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

// ── Photo helpers ──
function renderPersonDetail(person, extraHtml = '') {
    const photoHtml = person.photoUrl
        ? `<img src="${escHtml(person.photoUrl)}" alt="photo" class="rounded-circle" style="width:100px;height:100px;object-fit:cover;">`
        : '<i class="bi bi-person-circle text-muted" style="font-size:5rem;"></i>';

    return `
        <div class="d-flex align-items-start gap-4">
            <div class="flex-grow-1">
                <table class="table table-sm table-bordered mb-0">
                    <tbody>
                        <tr><th class="text-muted" style="width:140px;">Email</th><td>${escHtml(person.email)}</td></tr>
                        <tr><th class="text-muted">Phone</th><td>${escHtml(person.phone)}</td></tr>
                        <tr><th class="text-muted">Date of Birth</th><td>${person.dateOfBirth}</td></tr>
                        ${extraHtml}
                    </tbody>
                </table>
            </div>
            <div class="text-center flex-shrink-0">${photoHtml}</div>
        </div>`;
}

function setupPhotoUpload(fileInputId, hiddenInputId, previewId, removeBtnId) {
    const fileInput = document.getElementById(fileInputId);
    const hidden = document.getElementById(hiddenInputId);
    const preview = document.getElementById(previewId);
    const removeBtn = document.getElementById(removeBtnId);

    fileInput.addEventListener('change', async () => {
        const file = fileInput.files[0];
        if (!file) return;
        try {
            const result = await api.uploadFile('Photo/upload', file);
            hidden.value = result.photoUrl;
            preview.src = result.photoUrl;
            preview.style.display = 'block';
            removeBtn.classList.remove('d-none');
        } catch (e) {
            showToast('Upload Error', e.message, false);
        }
    });

    removeBtn.addEventListener('click', () => {
        hidden.value = '';
        preview.src = '';
        preview.style.display = 'none';
        fileInput.value = '';
        removeBtn.classList.add('d-none');
    });
}

function setPhotoPreview(hiddenInputId, previewId, removeBtnId, fileInputId, photoUrl) {
    const hidden = document.getElementById(hiddenInputId);
    const preview = document.getElementById(previewId);
    const removeBtn = document.getElementById(removeBtnId);
    const fileInput = document.getElementById(fileInputId);

    fileInput.value = '';
    if (photoUrl) {
        hidden.value = photoUrl;
        preview.src = photoUrl;
        preview.style.display = 'block';
        removeBtn.classList.remove('d-none');
    } else {
        hidden.value = '';
        preview.src = '';
        preview.style.display = 'none';
        removeBtn.classList.add('d-none');
    }
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

function initPicker(id, { multi = true, placeholder = 'Search...', onChange = null } = {}) {
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

    const state = { items: [], selected: new Map(), multi, id, onChange };
    pickers[id] = state;

    const input = document.getElementById(inputId);
    const dd = document.getElementById(ddId);

    function refreshDropdown() {
        const q = input.value.trim().toLowerCase();

        const matches = state.items
            .filter(i => !state.selected.has(i.id) &&
                (q.length === 0 || i.label.toLowerCase().split(/\s+/).some(w => w.startsWith(q))))
            .slice(0, 10);

        dd.innerHTML = matches.length === 0
            ? '<div class="picker-item disabled">No results</div>'
            : matches.map(i => `<div class="picker-item" data-id="${i.id}">${escHtml(i.label)}</div>`).join('');
        dd.classList.add('show');
    }

    input.addEventListener('input', refreshDropdown);
    input.addEventListener('focus', refreshDropdown);

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
        if (state.onChange) state.onChange([...state.selected.keys()]);
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
            if (state.onChange) state.onChange([...state.selected.keys()]);
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
            if (row.classList.contains('person-detail') || row.classList.contains('section-detail')) {
                if (q) row.classList.add('d-none');
                return;
            }
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
    initPicker('scheduleLessonPicker', { multi: false, onChange: onScheduleLessonChange });
    initPicker('scheduleSectionPicker', { multi: false });
    initPicker('scheduleTeacherPicker', { multi: true });

    // Initialize photo upload handlers
    setupPhotoUpload('studentPhotoFile', 'studentPhoto', 'studentPhotoPreview', 'studentPhotoRemove');
    setupPhotoUpload('teacherPhotoFile', 'teacherPhoto', 'teacherPhotoPreview', 'teacherPhotoRemove');

    // Initialize search bars
    setupSearch('studentsSearch', 'studentsTable');
    setupSearch('teachersSearch', 'teachersTable');
    setupSearch('lessonsSearch', 'lessonsTable');
    setupSearch('sectionsSearch', 'sectionsTable');
    setupSearch('examsSearch', 'examsTable');
    setupSearch('schedulesSearch', 'schedulesTable');

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

    const loaders = { dashboard: loadDashboard, students: loadStudents, teachers: loadTeachers, lessons: loadLessons, sections: loadSections, exams: loadExams, schedules: loadSchedules };
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
    const colCount = 9; // chevron + ID + Name + Surname + Email + Phone + DOB + Class + Actions
    document.getElementById('studentsTable').innerHTML = data.map(s => {
        const sectionLabel = s.sectionId
            ? `<span class="badge bg-secondary bg-opacity-75">${escHtml(lookupName(sections, s.sectionId))}</span>`
            : '<span class="text-muted">-</span>';
        const extraRows = `<tr><th class="text-muted">Class</th><td>${sectionLabel}</td></tr>
                           <tr><th class="text-muted">Enrollment</th><td>${s.enrollmentDate || '-'}</td></tr>`;
        return `
            <tr class="person-row" data-target="student-detail-${s.id}">
                <td class="text-center"><i class="bi bi-chevron-right section-chevron"></i></td>
                <td>${s.id}</td>
                <td>${escHtml(s.name)}</td><td>${escHtml(s.surname)}</td><td>${escHtml(s.email)}</td><td>${escHtml(s.phone)}</td>
                <td>${s.dateOfBirth}</td>
                <td>${sectionLabel}</td>
                <td>
                    <button class="btn btn-sm btn-warning" onclick="event.stopPropagation(); editStudent(${s.id})"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-sm btn-danger" onclick="event.stopPropagation(); deleteStudent(${s.id})"><i class="bi bi-trash"></i></button>
                </td>
            </tr>
            <tr class="person-detail d-none" id="student-detail-${s.id}">
                <td colspan="${colCount}" class="p-0 border-0">
                    <div class="section-detail-wrap">${renderPersonDetail(s, extraRows)}</div>
                </td>
            </tr>`;
    }).join('');
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
    setPhotoPreview('studentPhoto', 'studentPhotoPreview', 'studentPhotoRemove', 'studentPhotoFile', data?.photoUrl || '');
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
        sectionId: sectionIds[0] || null
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
    const colCount = 10; // chevron + ID + Name + Surname + Email + Phone + DOB + Salary + Lessons + Actions
    document.getElementById('teachersTable').innerHTML = data.map(t => {
        const extraRows = `<tr><th class="text-muted">Salary</th><td>${t.salary}</td></tr>
                           <tr><th class="text-muted">Lessons</th><td>${renderNames(lessons, t.lessonIds)}</td></tr>`;
        return `
            <tr class="person-row" data-target="teacher-detail-${t.id}">
                <td class="text-center"><i class="bi bi-chevron-right section-chevron"></i></td>
                <td>${t.id}</td>
                <td>${escHtml(t.name)}</td><td>${escHtml(t.surname)}</td><td>${escHtml(t.email)}</td><td>${escHtml(t.phone)}</td>
                <td>${t.dateOfBirth}</td>
                <td>${t.salary}</td>
                <td>${renderNames(lessons, t.lessonIds)}</td>
                <td>
                    <button class="btn btn-sm btn-warning" onclick="event.stopPropagation(); editTeacher(${t.id})"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-sm btn-danger" onclick="event.stopPropagation(); deleteTeacher(${t.id})"><i class="bi bi-trash"></i></button>
                </td>
            </tr>
            <tr class="person-detail d-none" id="teacher-detail-${t.id}">
                <td colspan="${colCount}" class="p-0 border-0">
                    <div class="section-detail-wrap">${renderPersonDetail(t, extraRows)}</div>
                </td>
            </tr>`;
    }).join('');
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
    setPhotoPreview('teacherPhoto', 'teacherPhotoPreview', 'teacherPhotoRemove', 'teacherPhotoFile', data?.photoUrl || '');
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

// ── Expandable row toggle (students, teachers, sections) ──
document.addEventListener('click', (e) => {
    const row = e.target.closest('.person-row, .section-row');
    if (!row) return;
    const detail = document.getElementById(row.dataset.target);
    if (!detail) return;
    const isHidden = detail.classList.toggle('d-none');
    const chevron = row.querySelector('.section-chevron');
    if (chevron) chevron.classList.toggle('bi-chevron-right', isHidden);
    if (chevron) chevron.classList.toggle('bi-chevron-down', !isHidden);
});

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
    document.getElementById('sectionsTable').innerHTML = data.map(s => {
        const sectionStudents = students.filter(st => (s.studentIds || []).includes(st.id));
        const count = sectionStudents.length;

        const detailRows = sectionStudents.length === 0
            ? '<tr><td colspan="5" class="text-muted text-center fst-italic">No students assigned</td></tr>'
            : sectionStudents.map(st => `
                <tr>
                    <td>${escHtml(st.name)}</td>
                    <td>${escHtml(st.surname)}</td>
                    <td>${st.dateOfBirth}</td>
                    <td>${escHtml(st.email)}</td>
                    <td>${escHtml(st.phone)}</td>
                </tr>`).join('');

        return `
            <tr class="section-row" data-target="section-detail-${s.id}">
                <td class="text-center"><i class="bi bi-chevron-right section-chevron"></i></td>
                <td>${s.id}</td>
                <td>${escHtml(s.name)}</td>
                <td><span class="badge bg-info">${count} student${count !== 1 ? 's' : ''}</span></td>
                <td>${renderNames(lessons, s.lessonIds)}</td>
                <td>
                    <button class="btn btn-sm btn-warning" onclick="event.stopPropagation(); editSection(${s.id})"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-sm btn-danger" onclick="event.stopPropagation(); deleteSection(${s.id})"><i class="bi bi-trash"></i></button>
                </td>
            </tr>
            <tr class="section-detail d-none" id="section-detail-${s.id}">
                <td colspan="6" class="p-0 border-0">
                    <div class="section-detail-wrap">
                        <table class="table table-sm table-bordered mb-0">
                            <thead class="table-light"><tr><th>Name</th><th>Surname</th><th>Date of Birth</th><th>Email</th><th>Phone</th></tr></thead>
                            <tbody>${detailRows}</tbody>
                        </table>
                    </div>
                </td>
            </tr>`;
    }).join('');
}

async function openSectionModal(data = null) {
    const [students, lessons] = await Promise.all([
        getCached('students'), getCached('lessons')
    ]);

    // Only show students not assigned to another section (or already in this section)
    const editId = data?.id || null;
    const available = students.filter(st => !st.sectionId || st.sectionId === editId);

    document.getElementById('sectionModalTitle').textContent = data ? 'Edit Class' : 'Add Class';
    document.getElementById('sectionId').value = data?.id || '';
    document.getElementById('sectionName').value = data?.name || '';
    resetPicker('sectionStudentPicker', toPickerItems(available), data?.studentIds || []);
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
            <td>${escHtml(lookupName(students, e.studentId))}</td>
            <td>${escHtml(lookupName(lessons, e.lessonId))}</td>
            <td>${e.examDate}</td>
            <td>${e.score}</td>
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
        studentId: studentIds[0] || null,
        lessonId: lessonIds[0] || null,
        examDate: document.getElementById('examDate').value,
        score: parseInt(document.getElementById('examScore').value)
    };
    try {
        if (id) await api.put(`Exam/${id}`, body);
        else await api.post('Exam', body);
        bootstrap.Modal.getInstance(document.getElementById('examModal')).hide();
        showToast('Exam', id ? 'Updated successfully' : 'Created successfully');
        invalidate('exams');
        loadExams();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteExam(id) {
    if (!confirm('Delete this exam?')) return;
    try {
        await api.delete(`Exam/${id}`);
        showToast('Exam', 'Deleted');
        invalidate('exams');
        loadExams();
    } catch (e) { showToast('Error', e.message, false); }
}

// ── Schedules ──
const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

let _allTeacherItems = [];
let _cachedLessons = [];

async function onScheduleLessonChange(selectedIds) {
    if (_cachedLessons.length === 0) _cachedLessons = await getCached('lessons');
    const lessonId = selectedIds[0];
    const lesson = lessonId ? _cachedLessons.find(l => l.id === lessonId) : null;

    const validIds = lesson ? new Set(lesson.teacherIds || []) : null;
    const filtered = validIds
        ? _allTeacherItems.filter(t => validIds.has(t.id))
        : _allTeacherItems;

    // Keep only currently selected teachers that are still valid
    const currentIds = getPickerIds('scheduleTeacherPicker');
    const keptIds = validIds ? currentIds.filter(id => validIds.has(id)) : currentIds;

    resetPicker('scheduleTeacherPicker', filtered, keptIds);
}

async function loadSchedules() {
    const [data, lessons, sections, teachers] = await Promise.all([
        getCached('schedules'), getCached('lessons'), getCached('sections'), getCached('teachers')
    ]);
    document.getElementById('schedulesTable').innerHTML = data.map(s => `
        <tr>
            <td>${s.id}</td>
            <td>${escHtml(lookupName(lessons, s.lessonId))}</td>
            <td>${escHtml(lookupName(sections, s.sectionId))}</td>
            <td>${renderNames(teachers, s.teacherIds)}</td>
            <td>${dayNames[s.dayOfWeek] || s.dayOfWeek}</td>
            <td>${escHtml(s.startTime)}</td>
            <td>${escHtml(s.endTime)}</td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editSchedule(${s.id})"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" onclick="deleteSchedule(${s.id})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>`).join('');
}

async function openScheduleModal(data = null) {
    const [lessons, sections, teachers] = await Promise.all([
        getCached('lessons'), getCached('sections'), getCached('teachers')
    ]);

    _cachedLessons = lessons;
    _allTeacherItems = toPickerItems(teachers);

    document.getElementById('scheduleModalTitle').textContent = data ? 'Edit Schedule' : 'Add Schedule';
    document.getElementById('scheduleId').value = data?.id || '';
    document.getElementById('scheduleDayOfWeek').value = data?.dayOfWeek ?? 1;
    document.getElementById('scheduleStartTime').value = data?.startTime || '';
    document.getElementById('scheduleEndTime').value = data?.endTime || '';
    resetPicker('scheduleLessonPicker', toPickerItems(lessons), data?.lessonId ? [data.lessonId] : []);
    resetPicker('scheduleSectionPicker', toPickerItems(sections), data?.sectionId ? [data.sectionId] : []);

    // Filter teachers by the selected lesson
    const lessonId = data?.lessonId;
    const lesson = lessonId ? lessons.find(l => l.id === lessonId) : null;
    const validIds = lesson ? new Set(lesson.teacherIds || []) : null;
    const filteredTeachers = validIds
        ? _allTeacherItems.filter(t => validIds.has(t.id))
        : _allTeacherItems;
    resetPicker('scheduleTeacherPicker', filteredTeachers, data?.teacherIds || []);

    new bootstrap.Modal(document.getElementById('scheduleModal')).show();
}

async function editSchedule(id) {
    const data = await api.get(`Schedule/${id}`);
    if (data) openScheduleModal(data);
}

async function saveSchedule() {
    const id = document.getElementById('scheduleId').value;
    const lessonIds = getPickerIds('scheduleLessonPicker');
    const sectionIds = getPickerIds('scheduleSectionPicker');
    const teacherIds = getPickerIds('scheduleTeacherPicker');
    const body = {
        lessonId: lessonIds[0] || null,
        sectionId: sectionIds[0] || null,
        teacherIds: teacherIds,
        dayOfWeek: parseInt(document.getElementById('scheduleDayOfWeek').value),
        startTime: document.getElementById('scheduleStartTime').value,
        endTime: document.getElementById('scheduleEndTime').value
    };
    try {
        if (id) await api.put(`Schedule/${id}`, body);
        else await api.post('Schedule', body);
        bootstrap.Modal.getInstance(document.getElementById('scheduleModal')).hide();
        showToast('Schedule', id ? 'Updated successfully' : 'Created successfully');
        invalidate('schedules');
        loadSchedules();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteSchedule(id) {
    if (!confirm('Delete this schedule?')) return;
    try {
        await api.delete(`Schedule/${id}`);
        showToast('Schedule', 'Deleted');
        invalidate('schedules');
        loadSchedules();
    } catch (e) { showToast('Error', e.message, false); }
}