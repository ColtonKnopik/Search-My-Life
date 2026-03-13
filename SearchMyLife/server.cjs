const http = require('http')
const fs = require('fs')
const path = require('path')

const PORT = process.env.PORT || 8080
const DIST = __dirname

const MIME_TYPES = {
  '.html': 'text/html; charset=utf-8',
  '.js':   'application/javascript',
  '.css':  'text/css',
  '.json': 'application/json',
  '.png':  'image/png',
  '.jpg':  'image/jpeg',
  '.jpeg': 'image/jpeg',
  '.svg':  'image/svg+xml',
  '.ico':  'image/x-icon',
  '.woff': 'font/woff',
  '.woff2':'font/woff2',
  '.ttf':  'font/ttf',
  '.eot':  'application/vnd.ms-fontobject',
}

http.createServer((req, res) => {
  const url = new URL(req.url, `http://localhost`)
  let filePath = path.join(DIST, url.pathname === '/' ? 'index.html' : url.pathname)

  fs.access(filePath, fs.constants.F_OK, (accessErr) => {
    if (accessErr) {
      filePath = path.join(DIST, 'index.html')
    }

    const ext = path.extname(filePath).toLowerCase()
    const contentType = MIME_TYPES[ext] || 'application/octet-stream'

    fs.readFile(filePath, (readErr, data) => {
      if (readErr) {
        res.writeHead(404, { 'Content-Type': 'text/plain' })
        res.end('Not found')
        return
      }
      res.writeHead(200, { 'Content-Type': contentType })
      res.end(data)
    })
  })
}).listen(PORT, () => {
  console.log(`Serving on port ${PORT}`)
})
