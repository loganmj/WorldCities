const { env } = require('process');

const target = 'https://localhost:40443';

const PROXY_CONFIG = [
  {
    context: [
      "/api",
    ],
    target,
    secure: false
  }
]

module.exports = PROXY_CONFIG;
