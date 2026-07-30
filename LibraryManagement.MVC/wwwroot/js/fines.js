/**
 * fines.js
 * Fine Management Logic.
 */

const FINES_STORAGE_KEY = "lms_fines";
const FINE_RATE_PER_DAY = 10;
const BORROW_PERIOD_DAYS = 15;

/**
 * Automatically calculates and generates fines for all borrow records
 * that have exceeded the borrow period.
 */
function calculateFines() {
  const history = JSON.parse(localStorage.getItem("lms_history")) || [];
  let fines = JSON.parse(localStorage.getItem(FINES_STORAGE_KEY)) || [];
  
  // Create a map of borrow actions
  const borrows = history.filter(h => h.action === "borrow");
  const returns = history.filter(h => h.action === "return");
  
  const today = new Date();
  
  borrows.forEach(b => {
    // If the borrow record lacks a historyId, we can't reliably track it.
    // We generated historyId in recent updates.
    if (!b.historyId) return;

    const borrowDate = new Date(b.date);
    let returnDate = today; // default to today if not returned yet
    
    // Check if it was returned
    const returnRecord = returns.find(r => r.historyId === b.historyId);
    if (returnRecord) {
      returnDate = new Date(returnRecord.date);
    }
    
    // Calculate difference in days
    const diffTime = Math.abs(returnDate - borrowDate);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    
    if (diffDays > BORROW_PERIOD_DAYS) {
      const lateDays = diffDays - BORROW_PERIOD_DAYS;
      const amount = lateDays * FINE_RATE_PER_DAY;
      
      // Check if fine already exists
      const existingFine = fines.find(f => f.historyId === b.historyId);
      
      if (existingFine) {
        // If not paid, update the amount (in case it's still accumulating)
        if (!existingFine.paid) {
          existingFine.amount = amount;
        }
      } else {
        // Generate a new fine
        fines.push({
          fineId: Date.now().toString() + Math.floor(Math.random()*1000),
          historyId: b.historyId,
          studentId: b.studentId,
          bookId: b.id,
          amount: amount,
          paid: false,
          paidDate: null
        });
      }
    }
  });
  
  localStorage.setItem(FINES_STORAGE_KEY, JSON.stringify(fines));
  return fines;
}

function getAllFines() {
  // Always recalculate to keep them fresh
  return calculateFines();
}

function markFineAsPaid(fineId) {
  let fines = JSON.parse(localStorage.getItem(FINES_STORAGE_KEY)) || [];
  const fine = fines.find(f => f.fineId === fineId);
  if (fine) {
    fine.paid = true;
    fine.paidDate = new Date().toISOString().split("T")[0];
    localStorage.setItem(FINES_STORAGE_KEY, JSON.stringify(fines));
    return true;
  }
  return false;
}

function getFineStats() {
  const fines = getAllFines();
  const totalFine = fines.reduce((sum, f) => sum + f.amount, 0);
  const collectedFine = fines.filter(f => f.paid).reduce((sum, f) => sum + f.amount, 0);
  const pendingFine = totalFine - collectedFine;
  return { totalFine, pendingFine, collectedFine };
}
