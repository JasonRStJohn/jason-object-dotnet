// Full-page screenshots of the preview (:5055) for before/after comparison.
// usage: node snap.mjs <outdir> <path,...> <dark,light> <d,m> [full]
import { chromium } from 'playwright-core';
import fs from 'fs';

const [out, paths, modes, vps, full] = process.argv.slice(2);
const BASE = 'http://127.0.0.1:5055';
const VP = {
  d: { viewport: { width: 1440, height: 900 } },
  m: { viewport: { width: 390, height: 844 }, isMobile: true, deviceScaleFactor: 2, hasTouch: true },
};
const name = p => (p === '/' ? 'home' : p.replace(/^\//, '').replace(/[/?=&]/g, '_'));

fs.mkdirSync(out, { recursive: true });
const browser = await chromium.launch();
for (const mode of modes.split(','))
  for (const v of vps.split(',')) {
    const ctx = await browser.newContext({ ...VP[v], reducedMotion: 'reduce' });
    await ctx.addCookies([{ name: 'theme', value: mode, url: BASE }]);
    const page = await ctx.newPage();
    for (const p of paths.split(',')) {
      await page.goto(BASE + p, { waitUntil: 'networkidle' });
      const file = `${out}/${name(p)}-${mode}-${v}.png`;
      await page.screenshot({ path: file, fullPage: full === 'full' });
      console.log(file);
    }
    await ctx.close();
  }
await browser.close();
