const puppeteer = require('puppeteer-core');

const BASE_URL = 'http://localhost:5242';

async function captureScreenshots() {
    console.log('Launching browser...');
    const browser = await puppeteer.launch({ 
        headless: "new",
        executablePath: 'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe',
        defaultViewport: { width: 1920, height: 1080 }
    });
    const page = await browser.newPage();
    
    try {
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

        console.log('Capturing newspapers.png...');
        await page.goto(`${BASE_URL}/Newspapers`, { waitUntil: 'networkidle0' });
        await new Promise(r => setTimeout(r, 1000));
        await page.screenshot({ path: 'C:\\Code\\mponline\\docs\\screenshots\\newspapers.png' });

        console.log('Capturing publications.png...');
        await page.goto(`${BASE_URL}/Publications`, { waitUntil: 'networkidle0' });
        await new Promise(r => setTimeout(r, 1000));
        await page.screenshot({ path: 'C:\\Code\\mponline\\docs\\screenshots\\publications.png' });
        
    } catch (err) {
        console.error('Error during capture:', err);
    } finally {
        await browser.close();
        console.log('Done!');
    }
}

captureScreenshots();
