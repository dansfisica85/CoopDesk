import { mkdirSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = dirname(dirname(fileURLToPath(import.meta.url)));
const target = join(root, 'public', 'config.json');
const apiBaseUrl = process.env.COOPDESK_API_BASE_URL || 'http://localhost:5298';

mkdirSync(dirname(target), { recursive: true });
writeFileSync(target, `${JSON.stringify({ apiBaseUrl }, null, 2)}\n`);
