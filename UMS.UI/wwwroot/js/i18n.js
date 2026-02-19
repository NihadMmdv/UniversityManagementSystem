// ── Internationalization ──
const translations = {
    en: {
        // Navbar
        'nav.dashboard': 'Dashboard',
        'nav.students': 'Students',
        'nav.teachers': 'Teachers',
        'nav.lessons': 'Lessons',
        'nav.classes': 'Classes',
        'nav.exams': 'Exams',
        'nav.schedules': 'Schedules',
        'nav.api': 'API',
        'nav.changePassword': 'Change Password',
        'nav.logout': 'Logout',

        // Dashboard
        'dashboard.title': 'Dashboard',
        'dashboard.students': 'Students',
        'dashboard.teachers': 'Teachers',
        'dashboard.lessons': 'Lessons',
        'dashboard.classes': 'Classes',

        // Students section
        'students.title': 'Students',
        'students.add': 'Add Student',
        'students.searchPlaceholder': 'Search students by name, email, class...',
        'students.th.id': 'ID',
        'students.th.name': 'Name',
        'students.th.surname': 'Surname',
        'students.th.email': 'Email',
        'students.th.phone': 'Phone',
        'students.th.dob': 'Date of Birth',
        'students.th.class': 'Class',
        'students.th.actions': 'Actions',

        // Teachers section
        'teachers.title': 'Teachers',
        'teachers.add': 'Add Teacher',
        'teachers.searchPlaceholder': 'Search teachers by name, email, lesson...',
        'teachers.th.id': 'ID',
        'teachers.th.name': 'Name',
        'teachers.th.surname': 'Surname',
        'teachers.th.email': 'Email',
        'teachers.th.phone': 'Phone',
        'teachers.th.dob': 'Date of Birth',
        'teachers.th.salary': 'Salary',
        'teachers.th.lessons': 'Lessons',
        'teachers.th.actions': 'Actions',

        // Lessons section
        'lessons.title': 'Lessons',
        'lessons.add': 'Add Lesson',
        'lessons.searchPlaceholder': 'Search lessons by name, teacher, class...',
        'lessons.th.id': 'ID',
        'lessons.th.name': 'Name',
        'lessons.th.teachers': 'Teachers',
        'lessons.th.classes': 'Classes',
        'lessons.th.actions': 'Actions',

        // Sections/Classes section
        'sections.title': 'Classes',
        'sections.add': 'Add Class',
        'sections.searchPlaceholder': 'Search classes by name, lesson...',
        'sections.th.id': 'ID',
        'sections.th.name': 'Name',
        'sections.th.students': 'Students',
        'sections.th.lessons': 'Lessons',
        'sections.th.actions': 'Actions',
        'sections.noStudents': 'No students assigned',

        // Exams section (admin)
        'exams.title': 'Exams',
        'exams.add': 'Add Exam',
        'exams.searchPlaceholder': 'Search exams by student, lesson, score...',
        'exams.th.id': 'ID',
        'exams.th.student': 'Student',
        'exams.th.lesson': 'Lesson',
        'exams.th.date': 'Date',
        'exams.th.score': 'Score',
        'exams.th.actions': 'Actions',

        // Schedules section (admin)
        'schedules.title': 'Schedules',
        'schedules.add': 'Add Schedule',
        'schedules.searchPlaceholder': 'Search schedules by lesson, class, teacher, day...',
        'schedules.th.id': 'ID',
        'schedules.th.lesson': 'Lesson',
        'schedules.th.class': 'Class',
        'schedules.th.teacher': 'Teacher',
        'schedules.th.day': 'Day',
        'schedules.th.start': 'Start',
        'schedules.th.end': 'End',
        'schedules.th.actions': 'Actions',

        // Student modal
        'modal.student.add': 'Add Student',
        'modal.student.edit': 'Edit Student',
        'modal.student.name': 'Name',
        'modal.student.surname': 'Surname',
        'modal.student.email': 'Email',
        'modal.student.phone': 'Phone',
        'modal.student.photo': 'Photo',
        'modal.student.dob': 'Date of Birth',
        'modal.student.enrollment': 'Enrollment Date',
        'modal.student.class': 'Class',
        'modal.student.remove': 'Remove',

        // Teacher modal
        'modal.teacher.add': 'Add Teacher',
        'modal.teacher.edit': 'Edit Teacher',
        'modal.teacher.name': 'Name',
        'modal.teacher.surname': 'Surname',
        'modal.teacher.email': 'Email',
        'modal.teacher.phone': 'Phone',
        'modal.teacher.photo': 'Photo',
        'modal.teacher.dob': 'Date of Birth',
        'modal.teacher.salary': 'Salary',
        'modal.teacher.lessons': 'Lessons',
        'modal.teacher.remove': 'Remove',

        // Lesson modal
        'modal.lesson.add': 'Add Lesson',
        'modal.lesson.edit': 'Edit Lesson',
        'modal.lesson.name': 'Name',
        'modal.lesson.classes': 'Classes',
        'modal.lesson.teachers': 'Teachers',

        // Section modal
        'modal.section.add': 'Add Class',
        'modal.section.edit': 'Edit Class',
        'modal.section.name': 'Name',
        'modal.section.students': 'Students',
        'modal.section.lessons': 'Lessons',

        // Exam modal
        'modal.exam.add': 'Add Exam',
        'modal.exam.edit': 'Edit Exam',
        'modal.exam.student': 'Student',
        'modal.exam.lesson': 'Lesson',
        'modal.exam.date': 'Exam Date',
        'modal.exam.score': 'Score',

        // Schedule modal
        'modal.schedule.add': 'Add Schedule',
        'modal.schedule.edit': 'Edit Schedule',
        'modal.schedule.lesson': 'Lesson',
        'modal.schedule.class': 'Class',
        'modal.schedule.teacher': 'Teacher',
        'modal.schedule.day': 'Day of Week',
        'modal.schedule.start': 'Start Time',
        'modal.schedule.end': 'End Time',

        // Change password modal
        'modal.password.title': 'Change Password',
        'modal.password.current': 'Current Password',
        'modal.password.new': 'New Password',
        'modal.password.confirm': 'Confirm New Password',
        'modal.password.mismatch': 'New passwords do not match',
        'modal.password.tooShort': 'Password must be at least 6 characters',
        'modal.password.success': 'Password changed successfully!',
        'modal.password.incorrect': 'Current password is incorrect',

        // Common
        'btn.cancel': 'Cancel',
        'btn.save': 'Save',
        'btn.back': 'Back',
        'btn.goBack': 'Go Back',
        'toast.uploadError': 'Upload Error',
        'toast.error': 'Error',
        'toast.updated': 'Updated successfully',
        'toast.created': 'Created successfully',
        'toast.deleted': 'Deleted',
        'confirm.deleteStudent': 'Delete this student?',
        'confirm.deleteTeacher': 'Delete this teacher?',
        'confirm.deleteLesson': 'Delete this lesson?',
        'confirm.deleteSection': 'Delete this class?',
        'confirm.deleteExam': 'Delete this exam?',
        'confirm.deleteSchedule': 'Delete this schedule?',
        'confirm.deleteMaterial': 'Delete this material?',
        'label.student': 'Student',
        'label.teacher': 'Teacher',
        'label.lesson': 'Lesson',
        'label.class': 'Class',
        'label.exam': 'Exam',
        'label.schedule': 'Schedule',
        'label.email': 'Email',
        'label.phone': 'Phone',
        'label.dob': 'Date of Birth',
        'label.birthDate': 'Birth Date',
        'label.salary': 'Salary',
        'label.lessons': 'Lessons',
        'label.enrollment': 'Enrollment',
        'student.singular': 'student',
        'student.plural': 'students',

        // Day names
        'day.sunday': 'Sunday',
        'day.monday': 'Monday',
        'day.tuesday': 'Tuesday',
        'day.wednesday': 'Wednesday',
        'day.thursday': 'Thursday',
        'day.friday': 'Friday',
        'day.saturday': 'Saturday',

        // Schedule day options
        'schedule.day.1': 'Monday',
        'schedule.day.2': 'Tuesday',
        'schedule.day.3': 'Wednesday',
        'schedule.day.4': 'Thursday',
        'schedule.day.5': 'Friday',
        'schedule.day.6': 'Saturday',
        'schedule.day.0': 'Sunday',

        // Picker
        'picker.search': 'Search...',
        'picker.noResults': 'No results',

        // Login page
        'login.title': 'UMS Login',
        'login.email': 'Email',
        'login.emailPlaceholder': 'Enter your email',
        'login.password': 'Password',
        'login.passwordPlaceholder': 'Enter your password',
        'login.rememberMe': 'Remember me',
        'login.submit': 'Login',
        'login.footer': 'University Management System',
        'login.failed': 'Login failed',

        // Landing page
        'landing.welcome': 'Welcome to UMS',
        'landing.subtitle': 'Your academic portal for courses, schedules, and more.',
        'landing.courses': 'Courses',
        'landing.coursesDesc': 'Browse available lessons and course materials.',
        'landing.exams': 'Exams',
        'landing.examsDesc': 'View your exam results and scores.',
        'landing.schedule': 'Schedule',
        'landing.scheduleDesc': 'View your class schedule and exam dates.',
        'landing.materials': 'Materials',
        'landing.materialsDesc': 'Upload teaching materials and exam questions for your lessons.',

        // Courses page
        'courses.title': 'My Courses',
        'courses.empty': "You don't have any courses assigned yet.",
        'courses.teachers': 'Teacher(s)',
        'courses.sections': 'Section(s)',
        'courses.materials': 'Materials',
        'courses.noMaterials': 'No materials available yet.',
        'courses.download': 'Download',

        // Materials page (teacher)
        'materials.title': 'My Materials',
        'materials.upload': 'Upload Material',
        'materials.empty': "You haven't uploaded any materials yet.",
        'materials.th.title': 'Title',
        'materials.th.lesson': 'Lesson',
        'materials.th.type': 'Type',
        'materials.th.file': 'File',
        'materials.th.date': 'Date',
        'materials.th.actions': 'Actions',
        'materials.type.material': 'Material',
        'materials.type.question': 'Question',
        'materials.selectLesson': 'Select Lesson',
        'materials.fileLabel': 'File',
        'materials.titleLabel': 'Title',
        'materials.typeLabel': 'Type',
        'materials.uploadSuccess': 'Material uploaded successfully',
        'materials.uploadError': 'Failed to upload material',
        'materials.deleteSuccess': 'Material deleted',

        // My Exams page (student)
        'myExams.title': 'My Exams',
        'myExams.empty': "You don't have any exams yet.",
        'myExams.th.num': '#',
        'myExams.th.lesson': 'Lesson',
        'myExams.th.date': 'Date',
        'myExams.th.score': 'Score',

        // My Schedule page
        'mySchedule.title': 'My Weekly Schedule',
        'mySchedule.empty': 'No schedule entries found.',
        'mySchedule.failed': 'Failed to load schedule.',
        'mySchedule.th.time': 'Time',
    },
    az: {
        // Navbar
        'nav.dashboard': 'İdarə paneli',
        'nav.students': 'Tələbələr',
        'nav.teachers': 'Müəllimlər',
        'nav.lessons': 'Dərslər',
        'nav.classes': 'Siniflər',
        'nav.exams': 'İmtahanlar',
        'nav.schedules': 'Cədvəllər',
        'nav.api': 'API',
        'nav.changePassword': 'Şifrəni dəyiş',
        'nav.logout': 'Çıxış',

        // Dashboard
        'dashboard.title': 'İdarə paneli',
        'dashboard.students': 'Tələbələr',
        'dashboard.teachers': 'Müəllimlər',
        'dashboard.lessons': 'Dərslər',
        'dashboard.classes': 'Siniflər',

        // Students section
        'students.title': 'Tələbələr',
        'students.add': 'Tələbə əlavə et',
        'students.searchPlaceholder': 'Tələbələri ad, e-poçt, sinif üzrə axtar...',
        'students.th.id': 'ID',
        'students.th.name': 'Ad',
        'students.th.surname': 'Soyad',
        'students.th.email': 'E-poçt',
        'students.th.phone': 'Telefon',
        'students.th.dob': 'Doğum tarixi',
        'students.th.class': 'Sinif',
        'students.th.actions': 'Əməliyyatlar',

        // Teachers section
        'teachers.title': 'Müəllimlər',
        'teachers.add': 'Müəllim əlavə et',
        'teachers.searchPlaceholder': 'Müəllimləri ad, e-poçt, dərs üzrə axtar...',
        'teachers.th.id': 'ID',
        'teachers.th.name': 'Ad',
        'teachers.th.surname': 'Soyad',
        'teachers.th.email': 'E-poçt',
        'teachers.th.phone': 'Telefon',
        'teachers.th.dob': 'Doğum tarixi',
        'teachers.th.salary': 'Maaş',
        'teachers.th.lessons': 'Dərslər',
        'teachers.th.actions': 'Əməliyyatlar',

        // Lessons section
        'lessons.title': 'Dərslər',
        'lessons.add': 'Dərs əlavə et',
        'lessons.searchPlaceholder': 'Dərsləri ad, müəllim, sinif üzrə axtar...',
        'lessons.th.id': 'ID',
        'lessons.th.name': 'Ad',
        'lessons.th.teachers': 'Müəllimlər',
        'lessons.th.classes': 'Siniflər',
        'lessons.th.actions': 'Əməliyyatlar',

        // Sections/Classes section
        'sections.title': 'Siniflər',
        'sections.add': 'Sinif əlavə et',
        'sections.searchPlaceholder': 'Sinifləri ad, dərs üzrə axtar...',
        'sections.th.id': 'ID',
        'sections.th.name': 'Ad',
        'sections.th.students': 'Tələbələr',
        'sections.th.lessons': 'Dərslər',
        'sections.th.actions': 'Əməliyyatlar',
        'sections.noStudents': 'Tələbə təyin olunmayıb',

        // Exams section (admin)
        'exams.title': 'İmtahanlar',
        'exams.add': 'İmtahan əlavə et',
        'exams.searchPlaceholder': 'İmtahanları tələbə, dərs, bal üzrə axtar...',
        'exams.th.id': 'ID',
        'exams.th.student': 'Tələbə',
        'exams.th.lesson': 'Dərs',
        'exams.th.date': 'Tarix',
        'exams.th.score': 'Bal',
        'exams.th.actions': 'Əməliyyatlar',

        // Schedules section (admin)
        'schedules.title': 'Cədvəllər',
        'schedules.add': 'Cədvəl əlavə et',
        'schedules.searchPlaceholder': 'Cədvəlləri dərs, sinif, müəllim, gün üzrə axtar...',
        'schedules.th.id': 'ID',
        'schedules.th.lesson': 'Dərs',
        'schedules.th.class': 'Sinif',
        'schedules.th.teacher': 'Müəllim',
        'schedules.th.day': 'Gün',
        'schedules.th.start': 'Başlanğıc',
        'schedules.th.end': 'Son',
        'schedules.th.actions': 'Əməliyyatlar',

        // Student modal
        'modal.student.add': 'Tələbə əlavə et',
        'modal.student.edit': 'Tələbəni redaktə et',
        'modal.student.name': 'Ad',
        'modal.student.surname': 'Soyad',
        'modal.student.email': 'E-poçt',
        'modal.student.phone': 'Telefon',
        'modal.student.photo': 'Şəkil',
        'modal.student.dob': 'Doğum tarixi',
        'modal.student.enrollment': 'Qeydiyyat tarixi',
        'modal.student.class': 'Sinif',
        'modal.student.remove': 'Sil',

        // Teacher modal
        'modal.teacher.add': 'Müəllim əlavə et',
        'modal.teacher.edit': 'Müəllimi redaktə et',
        'modal.teacher.name': 'Ad',
        'modal.teacher.surname': 'Soyad',
        'modal.teacher.email': 'E-poçt',
        'modal.teacher.phone': 'Telefon',
        'modal.teacher.photo': 'Şəkil',
        'modal.teacher.dob': 'Doğum tarixi',
        'modal.teacher.salary': 'Maaş',
        'modal.teacher.lessons': 'Dərslər',
        'modal.teacher.remove': 'Sil',

        // Lesson modal
        'modal.lesson.add': 'Dərs əlavə et',
        'modal.lesson.edit': 'Dərsi redaktə et',
        'modal.lesson.name': 'Ad',
        'modal.lesson.classes': 'Siniflər',
        'modal.lesson.teachers': 'Müəllimlər',

        // Section modal
        'modal.section.add': 'Sinif əlavə et',
        'modal.section.edit': 'Sinifi redaktə et',
        'modal.section.name': 'Ad',
        'modal.section.students': 'Tələbələr',
        'modal.section.lessons': 'Dərslər',

        // Exam modal
        'modal.exam.add': 'İmtahan əlavə et',
        'modal.exam.edit': 'İmtahanı redaktə et',
        'modal.exam.student': 'Tələbə',
        'modal.exam.lesson': 'Dərs',
        'modal.exam.date': 'İmtahan tarixi',
        'modal.exam.score': 'Bal',

        // Schedule modal
        'modal.schedule.add': 'Cədvəl əlavə et',
        'modal.schedule.edit': 'Cədvəli redaktə et',
        'modal.schedule.lesson': 'Dərs',
        'modal.schedule.class': 'Sinif',
        'modal.schedule.teacher': 'Müəllim',
        'modal.schedule.day': 'Həftənin günü',
        'modal.schedule.start': 'Başlanğıc vaxtı',
        'modal.schedule.end': 'Son vaxt',

        // Change password modal
        'modal.password.title': 'Şifrəni dəyiş',
        'modal.password.current': 'Cari şifrə',
        'modal.password.new': 'Yeni şifrə',
        'modal.password.confirm': 'Yeni şifrəni təsdiqlə',
        'modal.password.mismatch': 'Yeni şifrələr uyğun gəlmir',
        'modal.password.tooShort': 'Şifrə ən azı 6 simvol olmalıdır',
        'modal.password.success': 'Şifrə uğurla dəyişdirildi!',
        'modal.password.incorrect': 'Cari şifrə yanlışdır',

        // Common
        'btn.cancel': 'Ləğv et',
        'btn.save': 'Saxla',
        'btn.back': 'Geri',
        'btn.goBack': 'Geri qayıt',
        'toast.uploadError': 'Yükləmə xətası',
        'toast.error': 'Xəta',
        'toast.updated': 'Uğurla yeniləndi',
        'toast.created': 'Uğurla yaradıldı',
        'toast.deleted': 'Silindi',
        'confirm.deleteStudent': 'Bu tələbə silinsin?',
        'confirm.deleteTeacher': 'Bu müəllim silinsin?',
        'confirm.deleteLesson': 'Bu dərs silinsin?',
        'confirm.deleteSection': 'Bu sinif silinsin?',
        'confirm.deleteExam': 'Bu imtahan silinsin?',
        'confirm.deleteSchedule': 'Bu cədvəl silinsin?',
        'confirm.deleteMaterial': 'Bu material silinsin?',
        'label.student': 'Tələbə',
        'label.teacher': 'Müəllim',
        'label.lesson': 'Dərs',
        'label.class': 'Sinif',
        'label.exam': 'İmtahan',
        'label.schedule': 'Cədvəl',
        'label.email': 'E-poçt',
        'label.phone': 'Telefon',
        'label.dob': 'Doğum tarixi',
        'label.birthDate': 'Doğum tarixi',
        'label.salary': 'Maaş',
        'label.lessons': 'Dərslər',
        'label.enrollment': 'Qeydiyyat',
        'student.singular': 'tələbə',
        'student.plural': 'tələbə',

        // Day names
        'day.sunday': 'Bazar',
        'day.monday': 'Bazar ertəsi',
        'day.tuesday': 'Çərşənbə axşamı',
        'day.wednesday': 'Çərşənbə',
        'day.thursday': 'Cümə axşamı',
        'day.friday': 'Cümə',
        'day.saturday': 'Şənbə',

        // Schedule day options
        'schedule.day.1': 'Bazar ertəsi',
        'schedule.day.2': 'Çərşənbə axşamı',
        'schedule.day.3': 'Çərşənbə',
        'schedule.day.4': 'Cümə axşamı',
        'schedule.day.5': 'Cümə',
        'schedule.day.6': 'Şənbə',
        'schedule.day.0': 'Bazar',

        // Picker
        'picker.search': 'Axtar...',
        'picker.noResults': 'Nəticə yoxdur',

        // Login page
        'login.title': 'UMS Giriş',
        'login.email': 'E-poçt',
        'login.emailPlaceholder': 'E-poçtunuzu daxil edin',
        'login.password': 'Şifrə',
        'login.passwordPlaceholder': 'Şifrənizi daxil edin',
        'login.rememberMe': 'Yadda saxla',
        'login.submit': 'Daxil ol',
        'login.footer': 'Universitet İdarəetmə Sistemi',
        'login.failed': 'Giriş uğursuz oldu',

        // Landing page
        'landing.welcome': 'UMS-ə xoş gəlmisiniz',
        'landing.subtitle': 'Dərslər, cədvəllər və daha çoxu üçün akademik portalınız.',
        'landing.courses': 'Dərslər',
        'landing.coursesDesc': 'Mövcud dərsləri və kurs materiallarını nəzərdən keçirin.',
        'landing.exams': 'İmtahanlar',
        'landing.examsDesc': 'İmtahan nəticələrinizi və ballarınızı görün.',
        'landing.schedule': 'Cədvəl',
        'landing.scheduleDesc': 'Dərs cədvəlinizi və imtahan tarixlərinizi görün.',
        'landing.materials': 'Materiallar',
        'landing.materialsDesc': 'Dərsləriniz üçün tədris materialları və imtahan sualları yükləyin.',

        // Courses page
        'courses.title': 'Dərslərim',
        'courses.empty': 'Hələ heç bir dərs təyin edilməyib.',
        'courses.teachers': 'Müəllim(lər)',
        'courses.sections': 'Bölmə(lər)',
        'courses.materials': 'Materiallar',
        'courses.noMaterials': 'Hələ material yoxdur.',
        'courses.download': 'Yüklə',

        // Materials page (teacher)
        'materials.title': 'Materiallarım',
        'materials.upload': 'Material yüklə',
        'materials.empty': 'Hələ heç bir material yükləməmisiniz.',
        'materials.th.title': 'Başlıq',
        'materials.th.lesson': 'Dərs',
        'materials.th.type': 'Növ',
        'materials.th.file': 'Fayl',
        'materials.th.date': 'Tarix',
        'materials.th.actions': 'Əməliyyatlar',
        'materials.type.material': 'Material',
        'materials.type.question': 'Sual',
        'materials.selectLesson': 'Dərs seçin',
        'materials.fileLabel': 'Fayl',
        'materials.titleLabel': 'Başlıq',
        'materials.typeLabel': 'Növ',
        'materials.uploadSuccess': 'Material uğurla yükləndi',
        'materials.uploadError': 'Material yüklənə bilmədi',
        'materials.deleteSuccess': 'Material silindi',

        // My Exams page (student)
        'myExams.title': 'İmtahanlarım',
        'myExams.empty': 'Hələ heç bir imtahanınız yoxdur.',
        'myExams.th.num': '#',
        'myExams.th.lesson': 'Dərs',
        'myExams.th.date': 'Tarix',
        'myExams.th.score': 'Bal',

        // My Schedule page
        'mySchedule.title': 'Həftəlik Cədvəlim',
        'mySchedule.empty': 'Cədvəl qeydləri tapılmadı.',
        'mySchedule.failed': 'Cədvəl yüklənə bilmədi.',
        'mySchedule.th.time': 'Vaxt',
    }
};

let currentLang = localStorage.getItem('umsLang') || 'en';

function t(key) {
    return translations[currentLang]?.[key] || translations['en']?.[key] || key;
}

function getDayNames() {
    return [t('day.sunday'), t('day.monday'), t('day.tuesday'), t('day.wednesday'), t('day.thursday'), t('day.friday'), t('day.saturday')];
}

function applyLanguage(lang) {
    currentLang = lang;
    localStorage.setItem('umsLang', lang);

    // Update all elements with data-i18n attribute (textContent)
    document.querySelectorAll('[data-i18n]').forEach(el => {
        el.textContent = t(el.dataset.i18n);
    });

    // Update all elements with data-i18n-placeholder attribute
    document.querySelectorAll('[data-i18n-placeholder]').forEach(el => {
        el.placeholder = t(el.dataset.i18nPlaceholder);
    });

    // Update schedule day <option> elements
    document.querySelectorAll('#scheduleDayOfWeek option').forEach(opt => {
        const key = `schedule.day.${opt.value}`;
        opt.textContent = t(key);
    });

    // Update the language toggle button (works as dropdown item or standalone)
    const langBtn = document.getElementById('langToggleBtn');
    if (langBtn) {
        langBtn.innerHTML = lang === 'en'
            ? '\uD83C\uDDE6\uD83C\uDDFF Azərbaycanca'
            : '\uD83C\uDDEC\uD83C\uDDE7 English';
    }

    // Refresh the visible section to re-render dynamic content
    const activeLink = document.querySelector('.nav-link.active[data-section]');
    if (activeLink) {
        const section = activeLink.dataset.section;
        const loaders = {
            dashboard: typeof loadDashboard === 'function' ? loadDashboard : null,
            students: typeof loadStudents === 'function' ? loadStudents : null,
            teachers: typeof loadTeachers === 'function' ? loadTeachers : null,
            lessons: typeof loadLessons === 'function' ? loadLessons : null,
            sections: typeof loadSections === 'function' ? loadSections : null,
            exams: typeof loadExams === 'function' ? loadExams : null,
            schedules: typeof loadSchedules === 'function' ? loadSchedules : null,
        };
        loaders[section]?.();
    }

    // Notify other inline scripts that the language changed
    document.dispatchEvent(new CustomEvent('languageChanged', { detail: { lang } }));
}

// Universal language toggle click handler (works on every page)
document.addEventListener('click', (e) => {
    const btn = e.target.closest('#langToggleBtn');
    if (!btn) return;
    e.preventDefault();
    applyLanguage(currentLang === 'en' ? 'az' : 'en');
});

// Initialize language on load
document.addEventListener('DOMContentLoaded', () => {
    applyLanguage(currentLang);
});