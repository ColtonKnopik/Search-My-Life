const ALGORITHM = 'AES-GCM'
const KEY_LENGTH = 256
const PBKDF2_ITERATIONS = 100000

function getEncoder() {
  return new TextEncoder()
}

function getDecoder() {
  return new TextDecoder()
}

async function deriveKey(password, salt) {
  const encoder = getEncoder()
  const keyMaterial = await crypto.subtle.importKey(
    'raw',
    encoder.encode(password),
    'PBKDF2',
    false,
    ['deriveKey'],
  )
  return crypto.subtle.deriveKey(
    {
      name: 'PBKDF2',
      salt,
      iterations: PBKDF2_ITERATIONS,
      hash: 'SHA-256',
    },
    keyMaterial,
    { name: ALGORITHM, length: KEY_LENGTH },
    false,
    ['encrypt', 'decrypt'],
  )
}

function arrayBufferToBase64(buffer) {
  const bytes = new Uint8Array(buffer)
  let binary = ''
  for (let i = 0; i < bytes.byteLength; i++) {
    binary += String.fromCharCode(bytes[i])
  }
  return btoa(binary)
}

function base64ToArrayBuffer(base64) {
  const binary = atob(base64)
  const bytes = new Uint8Array(binary.length)
  for (let i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i)
  }
  return bytes.buffer
}

/**
 * Encrypt plaintext using AES-GCM with a key derived from the user's password.
 * Returns an object with base64-encoded ciphertext, iv, and salt.
 */
export async function encrypt(plaintext, password) {
  const encoder = getEncoder()
  const salt = crypto.getRandomValues(new Uint8Array(16))
  const iv = crypto.getRandomValues(new Uint8Array(12))
  const key = await deriveKey(password, salt)

  const ciphertext = await crypto.subtle.encrypt(
    { name: ALGORITHM, iv },
    key,
    encoder.encode(plaintext),
  )

  return {
    ciphertext: arrayBufferToBase64(ciphertext),
    iv: arrayBufferToBase64(iv),
    salt: arrayBufferToBase64(salt),
  }
}

/**
 * Decrypt ciphertext using AES-GCM with a key derived from the user's password.
 * All inputs (ciphertext, iv, salt) are base64-encoded strings.
 */
export async function decrypt(ciphertextBase64, ivBase64, saltBase64, password) {
  const ciphertext = base64ToArrayBuffer(ciphertextBase64)
  const iv = new Uint8Array(base64ToArrayBuffer(ivBase64))
  const salt = new Uint8Array(base64ToArrayBuffer(saltBase64))
  const key = await deriveKey(password, salt)

  const plainBuffer = await crypto.subtle.decrypt({ name: ALGORITHM, iv }, key, ciphertext)

  return getDecoder().decode(plainBuffer)
}
