/**
 * students.js
 * Seed data and CRUD operations for Students.
 */

const SEED_STUDENTS = [
  {
    studentId: 1,
    name: "Alice Johnson",
    email: "alice@example.com",
    phone: "555-0101",
    department: "Computer Science",
    semester: 4,
    enrollmentNo: "CS2024-001"
  },
  {
    studentId: 2,
    name: "Bob Martinez",
    email: "bob@example.com",
    phone: "555-0202",
    department: "Information Technology",
    semester: 2,
    enrollmentNo: "IT2025-042"
  },
  {
    studentId: 3,
    name: "Charlie Davis",
    email: "charlie@example.com",
    phone: "555-0303",
    department: "Mechanical Engineering",
    semester: 6,
    enrollmentNo: "ME2023-112"
  }
];

const STUDENT_STORAGE_KEY = "lms_students";
const STUDENT_ID_COUNTER_KEY = "lms_next_student_id";

function initStudents() {
  if (!localStorage.getItem(STUDENT_STORAGE_KEY)) {
    localStorage.setItem(STUDENT_STORAGE_KEY, JSON.stringify(SEED_STUDENTS));
    localStorage.setItem(STUDENT_ID_COUNTER_KEY, "4");
  }
}

function getAllStudents() {
  return JSON.parse(localStorage.getItem(STUDENT_STORAGE_KEY)) || [];
}

function getStudentById(id) {
  return getAllStudents().find(s => s.studentId === Number(id)) || null;
}

function saveStudents(students) {
  localStorage.setItem(STUDENT_STORAGE_KEY, JSON.stringify(students));
}

function addStudent(studentData) {
  const students = getAllStudents();
  const nextId = parseInt(localStorage.getItem(STUDENT_ID_COUNTER_KEY) || "1", 10);
  const newStudent = {
    studentId: nextId,
    name: studentData.name.trim(),
    email: studentData.email.trim(),
    phone: studentData.phone.trim(),
    department: studentData.department.trim(),
    semester: Number(studentData.semester),
    enrollmentNo: studentData.enrollmentNo.trim()
  };
  students.push(newStudent);
  saveStudents(students);
  localStorage.setItem(STUDENT_ID_COUNTER_KEY, String(nextId + 1));
  return newStudent;
}

function updateStudent(id, updates) {
  const students = getAllStudents();
  const idx = students.findIndex(s => s.studentId === Number(id));
  if (idx === -1) return null;
  students[idx] = { ...students[idx], ...updates };
  saveStudents(students);
  return students[idx];
}

function deleteStudent(id) {
  let students = getAllStudents();
  const before = students.length;
  students = students.filter(s => s.studentId !== Number(id));
  if (students.length === before) return false;
  saveStudents(students);
  return true;
}

initStudents();
