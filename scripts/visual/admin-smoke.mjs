// End-to-end check of the copy admin against the PREVIEW (:5055, MeDotNet_Preview).
// Reads ADMIN_EMAIL / ADMIN_PASSWORD from the environment; never prints them.
// Edits a project, hides and shows it, trips a validation rule, then restores everything.
import { chromium } from 'playwright-core';

const BASE = 'http://127.0.0.1:5055';
const TITLE = 'Chasma & CYOA';
const fail = msg => { console.error('FAIL: ' + msg); process.exit(1); };
const ok = msg => console.log('ok   ' + msg);

const browser = await chromium.launch();
const page = await (await browser.newContext({ viewport: { width: 1280, height: 900 } })).newPage();
// Blazor Server wires events once the circuit connects; give it a moment after each load.
const go = async path => { await page.goto(BASE + path, { waitUntil: 'networkidle' }); await page.waitForTimeout(1200); };
const publicText = async () => { await go('/projects'); return page.locator('body').innerText(); };

await go('/account/login');
await page.fill('#Input_Email', process.env.ADMIN_EMAIL ?? '');
await page.fill('#Input_Password', process.env.ADMIN_PASSWORD ?? '');
await Promise.all([page.waitForURL(u => !u.pathname.startsWith('/account/login')), page.click('button[type=submit]')]);
ok('logged in');

// 1. Edit the body
await go('/admin/projects');
await page.getByLabel(`Edit ${TITLE}`).click();
await page.waitForURL(/\/admin\/projects\/\d+$/); await page.waitForTimeout(1200);
const body = page.getByLabel('Body', { exact: true });
const original = await body.inputValue();
if (!original.includes('in dark mode')) fail('expected the seeded Chasma body to mention "in dark mode"');
await body.fill(original.replace('in dark mode', 'in both themes')); await body.press('Tab');
await page.getByRole('button', { name: 'Save' }).click();
await page.waitForURL(u => u.pathname === '/admin/projects');
if (!(await publicText()).includes('in both themes')) fail('/projects does not show the edited body');
ok('edited body shows on /projects');

// 2. Hide, then show
const toggle = async () => { await go('/admin/projects'); await page.getByLabel(`Show ${TITLE} on /projects`).click(); await page.waitForTimeout(800); };
await toggle();
if ((await publicText()).includes(TITLE)) fail('hidden project still shows on /projects');
ok('hidden project is gone from /projects');
await toggle();
if (!(await publicText()).includes(TITLE)) fail('re-shown project missing from /projects');
ok('re-shown project is back');

// 3. Validation: a blank line in "The problem"
await go('/admin/categories/platforms');
const problem = page.getByLabel('The problem', { exact: true });
const problemBefore = await problem.inputValue();
await problem.fill('One.\n\nTwo.'); await problem.press('Tab');
await page.getByRole('button', { name: 'Save' }).click();
await page.getByText('The problem must be one paragraph.').waitFor({ timeout: 5000 }).catch(() => fail('no one-paragraph error shown'));
await go('/admin/categories/platforms');
if ((await page.getByLabel('The problem', { exact: true }).inputValue()) !== problemBefore) fail('invalid problem text was saved');
ok('blank line in The problem is rejected and not saved');

// 4. Restore
await go('/admin/projects');
await page.getByLabel(`Edit ${TITLE}`).click();
await page.waitForURL(/\/admin\/projects\/\d+$/); await page.waitForTimeout(1200);
await page.getByLabel('Body', { exact: true }).fill(original); await page.getByLabel('Body', { exact: true }).press('Tab');
await page.getByRole('button', { name: 'Save' }).click();
await page.waitForURL(u => u.pathname === '/admin/projects');
if (!(await publicText()).includes('in dark mode')) fail('restore did not take');
ok('original body restored');

await browser.close();
console.log('ALL PASSED');
