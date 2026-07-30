const puppeteer = require('puppeteer-core');
const path = require('path');
const fs = require('fs');

const BASE_URL = 'http://localhost:5242';
const SCREENSHOT_DIR = path.join(__dirname, 'screenshots');

if (!fs.existsSync(SCREENSHOT_DIR)){
    fs.mkdirSync(SCREENSHOT_DIR, { recursive: true });
}

async function captureScreenshots() {
    console.log('Launching browser...');
    const browser = await puppeteer.launch({ 
        headless: "new",
        executablePath: 'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe',
        defaultViewport: { width: 1920, height: 1080 }
    });
    const page = await browser.newPage();
    
    // Helper to take screenshot
    async function takeScreenshot(route, filename) {
        console.log(`Capturing ${filename}...`);
        await page.goto(`${BASE_URL}${route}`, { waitUntil: 'networkidle0', timeout: 30000 });
        await new Promise(r => setTimeout(r, 1000)); // wait for animations
        await page.screenshot({ path: path.join(SCREENSHOT_DIR, filename) });
    }

    try {
        // Public pages
        await takeScreenshot('/', 'landing-page.png');
        await takeScreenshot('/Account/Login', 'login.png');
        await takeScreenshot('/Account/Register', 'register.png');

        // Login
        console.log('Logging in...');
        await page.goto(`${BASE_URL}/Account/Login`);
        await page.type('input[name="Email"]', 'admin@lms.com');
        await page.type('input[name="Password"]', 'Admin123!');
        await Promise.all([
            page.waitForNavigation({ waitUntil: 'networkidle0' }),
            page.click('button[type="submit"]')
        ]);
        
        await new Promise(r => setTimeout(r, 1000));
        await page.screenshot({ path: path.join(SCREENSHOT_DIR, 'dashboard.png') });

        // Admin pages
        await takeScreenshot('/Books', 'books.png');
        await takeScreenshot('/Student', 'students.png');
        await takeScreenshot('/Librarian', 'librarians.png');
        await takeScreenshot('/Borrow', 'borrow.png');
        await takeScreenshot('/Fines', 'fines.png');
        await takeScreenshot('/Reports', 'reports.png');
        await takeScreenshot('/Magazines', 'magazines.png');
        
        // Search
        await takeScreenshot('/Books?searchQuery=a', 'search.png');
        
    } catch (err) {
        console.error('Error during capture:', err);
    } finally {
        await browser.close();
        console.log('Done!');
    }
}

captureScreenshots();
