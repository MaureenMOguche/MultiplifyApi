const fs = require('fs');

// Function to recursively convert JSON object to .env format
function convertToJsonEnv(obj, prefix = '') {
    let result = '';

    for (const key in obj) {
        if (typeof obj[key] === 'object' && !Array.isArray(obj[key])) {
            result += convertToJsonEnv(obj[key], `${prefix}${key}__`);
        } else if (Array.isArray(obj[key])) {
            obj[key].forEach((value, index) => {
                result += convertToJsonEnv(value, `${prefix}${key}_${index}__`);
            });
        } else {
            result += `${prefix}${key}=${obj[key]}\n`;
        }
    }

    return result;
}

// Read appsettings.json
const jsonContent = fs.readFileSync('appsettings.json', 'utf8');
const jsonObject = JSON.parse(jsonContent);

// Convert JSON to .env format
const envContent = convertToJsonEnv(jsonObject);

// Write .env file
fs.writeFileSync('.env', envContent);

console.log('Conversion complete.');
