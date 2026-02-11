// Check authentication
const storage = localStorage.getItem('token') ? localStorage : sessionStorage;

// ── Reference data cache ──
let refData = { students: [], teachers: [], lessons: [], sections: [] };

async function loadRefData() {
    const [students, teachers, lessons, sections] = await Promise.all([
        api.get('Student'), api.get('Teacher'), api.get('Lesson'), api.get('Section')
    ]);
    refData.students = students || [];
    refData.teachers = teachers || [];
    refData.lessons = lessons || [];
    refData.sections = sections || [];
}

function lookupName(list, id, fallback = 'Unknown') {
    const item = list.find(x => x.id === id);
    if (!item) return fallback;
    return item.name + (item.surname ? ` ${item.surname}` : '');
}

function lookupNames(list, ids) {
    if (!ids || ids.length === 0) return '-';
    return ids.map(id => lookupName(list, id, `#${id}`)).join(', ');
}

// Auth guard
document.addEventListener('DOMContentLoaded', async () => {
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

    await loadRefData();
    loadDashboard();
});

// Logout — also clear swagger cookie
document.getElementById('logoutBtn').addEventListener('click', () => logout());

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

    const loaders = { students: loadStudents, teachers: loadTeachers, lessons: loadLessons, sections: loadSections, exams: loadExams };
    loaders[name]?.();
}

function showToast(title, message, success = true) {
    document.getElementById('toastTitle').textContent = title;
    document.getElementById('toastBody').textContent = message;
    const el = document.getElementById('toast');
    el.className = `toast text-white ${success ? 'bg-success' : 'bg-danger'}`;
    new bootstrap.Toast(el).show();
}

function parseIds(str) {
    return str ? str.split(',').map(s => parseInt(s.trim())).filter(n => !isNaN(n)) : [];
}

// ── Dashboard ──
function loadDashboard() {
    document.getElementById('studentCount').textContent = refData.students.length;
    document.getElementById('teacherCount').textContent = refData.teachers.length;
    document.getElementById('lessonCount').textContent = refData.lessons.length;
    document.getElementById('sectionCount').textContent = refData.sections.length;
}

// ── Students ──
async function loadStudents() {
    const data = await api.get('Student');
    if (!data) return;
    refData.students = data;
    document.getElementById('studentsTable').innerHTML = data.map(s => `
        <tr>
            <td>${s.id}</td><td>${s.name}</td><td>${s.surname}</td><td>${s.email}</td><td>${s.phone}</td>
            <td>${s.dateOfBirth}</td>
            <td>${lookupName(refData.sections, s.sectionId, '#' + s.sectionId)}</td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editStudent(${s.id})"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" onclick="deleteStudent(${s.id})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>`).join('');
}

function openStudentModal(data = null) {
    document.getElementById('studentModalTitle').textContent = data ? 'Edit Student' : 'Add Student';
    document.getElementById('studentId').value = data?.id || '';
    document.getElementById('studentName').value = data?.name || '';
    document.getElementById('studentSurname').value = data?.surname || '';
    document.getElementById('studentEmail').value = data?.email || '';
    document.getElementById('studentPhone').value = data?.phone || '';
    document.getElementById('studentPhoto').value = data?.photoUrl || '';
    document.getElementById('studentDob').value = data?.dateOfBirth || '';
    document.getElementById('studentEnrollment').value = data?.enrollmentDate || '';
    document.getElementById('studentSectionId').value = data?.sectionId || '';
    new bootstrap.Modal(document.getElementById('studentModal')).show();
}

async function editStudent(id) {
    const data = await api.get(`Student/${id}`);
    if (data) openStudentModal(data);
}

async function saveStudent() {
    const id = document.getElementById('studentId').value;
    const body = {
        name: document.getElementById('studentName').value,
        surname: document.getElementById('studentSurname').value,
        email: document.getElementById('studentEmail').value,
        phone: document.getElementById('studentPhone').value,
        photoUrl: document.getElementById('studentPhoto').value,
        dateOfBirth: document.getElementById('studentDob').value,
        enrollmentDate: document.getElementById('studentEnrollment').value,
        sectionId: parseInt(document.getElementById('studentSectionId').value)
    };
    try {
        if (id) await api.put(`Student/${id}`, body);
        else await api.post('Student', body);
        bootstrap.Modal.getInstance(document.getElementById('studentModal')).hide();
        showToast('Student', id ? 'Updated successfully' : 'Created successfully');
        await loadRefData(); loadStudents(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteStudent(id) {
    if (!confirm('Delete this student?')) return;
    try {
        await api.delete(`Student/${id}`);
        showToast('Student', 'Deleted');
        await loadRefData(); loadStudents(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

// ── Teachers ──
async function loadTeachers() {
    const data = await api.get('Teacher');
    if (!data) return;
    refData.teachers = data;
    document.getElementById('teachersTable').innerHTML = data.map(t => `
        <tr>
            <td>${t.id}</td><td>${t.name}</td><td>${t.surname}</td><td>${t.email}</td><td>${t.phone}</td>
            <td>${t.dateOfBirth}</td>
            <td>${t.salary}</td>
            <td>${lookupNames(refData.lessons, t.lessonIds)}</td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editTeacher(${t.id})"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" onclick="deleteTeacher(${t.id})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>`).join('');
}

function openTeacherModal(data = null) {
    document.getElementById('teacherModalTitle').textContent = data ? 'Edit Teacher' : 'Add Teacher';
    document.getElementById('teacherId').value = data?.id || '';
    document.getElementById('teacherName').value = data?.name || '';
    document.getElementById('teacherSurname').value = data?.surname || '';
    document.getElementById('teacherEmail').value = data?.email || '';
    document.getElementById('teacherPhone').value = data?.phone || '';
    document.getElementById('teacherPhoto').value = data?.photoUrl || '';
    document.getElementById('teacherDob').value = data?.dateOfBirth || '';
    document.getElementById('teacherSalary').value = data?.salary || '';
    document.getElementById('teacherLessonIds').value = data?.lessonIds?.join(',') || '';
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
        lessonIds: parseIds(document.getElementById('teacherLessonIds').value)
    };
    try {
        if (id) await api.put(`Teacher/${id}`, body);
        else await api.post('Teacher', body);
        bootstrap.Modal.getInstance(document.getElementById('teacherModal')).hide();
        showToast('Teacher', id ? 'Updated successfully' : 'Created successfully');
        await loadRefData(); loadTeachers(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteTeacher(id) {
    if (!confirm('Delete this teacher?')) return;
    try {
        await api.delete(`Teacher/${id}`);
        showToast('Teacher', 'Deleted');
        await loadRefData(); loadTeachers(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

// ── Lessons ──
async function loadLessons() {
    const data = await api.get('Lesson');
    if (!data) return;
    refData.lessons = data;
    document.getElementById('lessonsTable').innerHTML = data.map(l => `
        <tr>
            <td>${l.id}</td><td>${l.name}</td>
            <td>${lookupNames(refData.teachers, l.teacherIds)}</td>
            <td>${lookupNames(refData.sections, l.sectionIds)}</td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editLesson(${l.id})"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" onclick="deleteLesson(${l.id})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>`).join('');
}

function openLessonModal(data = null) {
    document.getElementById('lessonModalTitle').textContent = data ? 'Edit Lesson' : 'Add Lesson';
    document.getElementById('lessonId').value = data?.id || '';
    document.getElementById('lessonName').value = data?.name || '';
    document.getElementById('lessonSectionIds').value = data?.sectionIds?.join(',') || '';
    document.getElementById('lessonTeacherIds').value = data?.teacherIds?.join(',') || '';
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
        sectionIds: parseIds(document.getElementById('lessonSectionIds').value),
        teacherIds: parseIds(document.getElementById('lessonTeacherIds').value)
    };
    try {
        if (id) await api.put(`Lesson/${id}`, body);
        else await api.post('Lesson', body);
        bootstrap.Modal.getInstance(document.getElementById('lessonModal')).hide();
        showToast('Lesson', id ? 'Updated successfully' : 'Created successfully');
        await loadRefData(); loadLessons(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteLesson(id) {
    if (!confirm('Delete this lesson?')) return;
    try {
        await api.delete(`Lesson/${id}`);
        showToast('Lesson', 'Deleted');
        await loadRefData(); loadLessons(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

// ── Sections (displayed as "Classes" in UI) ──
async function loadSections() {
    const data = await api.get('Section');
    if (!data) return;
    refData.sections = data;
    document.getElementById('sectionsTable').innerHTML = data.map(s => `
        <tr>
            <td>${s.id}</td><td>${s.name}</td>
            <td>${lookupNames(refData.students, s.studentIds)}</td>
            <td>${lookupNames(refData.lessons, s.lessonIds)}</td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editSection(${s.id})"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" onclick="deleteSection(${s.id})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>`).join('');
}

function openSectionModal(data = null) {
    document.getElementById('sectionModalTitle').textContent = data ? 'Edit Class' : 'Add Class';
    document.getElementById('sectionId').value = data?.id || '';
    document.getElementById('sectionName').value = data?.name || '';
    document.getElementById('sectionStudentIds').value = data?.studentIds?.join(',') || '';
    document.getElementById('sectionLessonIds').value = data?.lessonIds?.join(',') || '';
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
        studentIds: parseIds(document.getElementById('sectionStudentIds').value),
        lessonIds: parseIds(document.getElementById('sectionLessonIds').value)
    };
    try {
        if (id) await api.put(`Section/${id}`, body);
        else await api.post('Section', body);
        bootstrap.Modal.getInstance(document.getElementById('sectionModal')).hide();
        showToast('Class', id ? 'Updated successfully' : 'Created successfully');
        await loadRefData(); loadSections(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteSection(id) {
    if (!confirm('Delete this class?')) return;
    try {
        await api.delete(`Section/${id}`);
        showToast('Class', 'Deleted');
        await loadRefData(); loadSections(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

// ── Exams ──
async function loadExams() {
    const data = await api.get('Exam');
    if (!data) return;
    document.getElementById('examsTable').innerHTML = data.map(e => `
        <tr>
            <td>${e.id}</td>
            <td>${lookupName(refData.students, e.studentId, '#' + e.studentId)}</td>
            <td>${lookupName(refData.lessons, e.lessonId, '#' + e.lessonId)}</td>
            <td>${e.examDate}</td><td>${e.score}</td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editExam(${e.id})"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" onclick="deleteExam(${e.id})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>`).join('');
}

function openExamModal(data = null) {
    document.getElementById('examModalTitle').textContent = data ? 'Edit Exam' : 'Add Exam';
    document.getElementById('examId').value = data?.id || '';
    document.getElementById('examStudentId').value = data?.studentId || '';
    document.getElementById('examLessonId').value = data?.lessonId || '';
    document.getElementById('examDate').value = data?.examDate || '';
    document.getElementById('examScore').value = data?.score || '';
    new bootstrap.Modal(document.getElementById('examModal')).show();
}

async function editExam(id) {
    const data = await api.get(`Exam/${id}`);
    if (data) openExamModal(data);
}

async function saveExam() {
    const id = document.getElementById('examId').value;
    const body = {
        studentId: parseInt(document.getElementById('examStudentId').value),
        lessonId: parseInt(document.getElementById('examLessonId').value),
        examDate: document.getElementById('examDate').value,
        score: parseInt(document.getElementById('examScore').value)
    };
    try {
        if (id) await api.put(`Exam/${id}`, body);
        else await api.post('Exam', body);
        bootstrap.Modal.getInstance(document.getElementById('examModal')).hide();
        showToast('Exam', id ? 'Updated successfully' : 'Created successfully');
        loadExams(); loadDashboard();
    } catch (e) { showToast('Error', e.message, false); }
}

async function deleteExam(id) {
    if (!confirm('Delete this exam?')) return;
    try { await api.delete(`Exam/${id}`); showToast('Exam', 'Deleted'); loadExams(); loadDashboard(); }
    catch (e) { showToast('Error', e.message, false); }
}