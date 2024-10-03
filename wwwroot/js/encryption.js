/**
 * SecureChat Encryption Utilities
 * Implements end-to-end encryption using Web Crypto API
 */
class SecureChatEncryption {
    constructor() {
        this.keyPair = null;
        this.sessionKey = null;
        this.participantPublicKeys = {};
    }
    
    /**
     * Generate RSA key pair for this participant
     * @returns {Promise<CryptoKeyPair>} The generated key pair
     */
    async generateKeyPair() {
        try {
            this.keyPair = await window.crypto.subtle.generateKey(
                {
                    name: "RSA-OAEP",
                    modulusLength: 2048,
                    publicExponent: new Uint8Array([1, 0, 1]),
                    hash: "SHA-256",
                },
                true,
                ["encrypt", "decrypt"]
            );
            
            console.log("RSA key pair generated successfully");
            return this.keyPair;
        } catch (error) {
            console.error("Error generating RSA key pair:", error);
            throw error;
        }
    }
    
    /**
     * Export public key in spki format for sharing
     * @returns {Promise<string>} Base64 encoded public key
     */
    async exportPublicKey() {
        try {
            if (!this.keyPair) {
                throw new Error("No key pair available. Call generateKeyPair first.");
            }
            
            const spkiKey = await window.crypto.subtle.exportKey(
                "spki", 
                this.keyPair.publicKey
            );
            
            return btoa(String.fromCharCode(...new Uint8Array(spkiKey)));
        } catch (error) {
            console.error("Error exporting public key:", error);
            throw error;
        }
    }
    
    /**
     * Import a public key from another participant
     * @param {string} publicKeyString Base64 encoded public key
     * @returns {Promise<CryptoKey>} Imported public key
     */
    async importPublicKey(publicKeyString) {
        try {
            const binaryKey = Uint8Array.from(atob(publicKeyString), c => c.charCodeAt(0));
            
            const publicKey = await window.crypto.subtle.importKey(
                "spki",
                binaryKey,
                {
                    name: "RSA-OAEP",
                    hash: "SHA-256"
                },
                false,
                ["encrypt"]
            );
            
            return publicKey;
        } catch (error) {
            console.error("Error importing public key:", error);
            throw error;
        }
    }
    
    /**
     * Generate AES session key for encrypting messages
     * @returns {Promise<CryptoKey>} Generated AES key
     */
    async generateSessionKey() {
        try {
            this.sessionKey = await window.crypto.subtle.generateKey(
                {
                    name: "AES-GCM",
                    length: 256,
                },
                true,
                ["encrypt", "decrypt"]
            );
            
            console.log("AES session key generated successfully");
            return this.sessionKey;
        } catch (error) {
            console.error("Error generating AES session key:", error);
            throw error;
        }
    }
    
    /**
     * Encrypt message with AES session key
     * @param {string} message Message to encrypt
     * @returns {Promise<Object>} Encrypted message and IV
     */
    async encryptMessage(message) {
        try {
            if (!this.sessionKey) {
                throw new Error("No session key available. Call generateSessionKey first.");
            }
            
            const encodedMessage = new TextEncoder().encode(message);
            const iv = window.crypto.getRandomValues(new Uint8Array(12));
            
            const encryptedData = await window.crypto.subtle.encrypt(
                {
                    name: "AES-GCM",
                    iv: iv,
                },
                this.sessionKey,
                encodedMessage
            );
            
            return {
                encryptedMessage: btoa(String.fromCharCode(...new Uint8Array(encryptedData))),
                iv: btoa(String.fromCharCode(...iv))
            };
        } catch (error) {
            console.error("Error encrypting message:", error);
            throw error;
        }
    }
    
    /**
     * Decrypt received message
     * @param {string} encryptedMessage Base64 encoded encrypted message
     * @param {string} iv Base64 encoded initialization vector
     * @returns {Promise<string>} Decrypted message
     */
    async decryptMessage(encryptedMessage, iv) {
        try {
            if (!this.sessionKey) {
                throw new Error("No session key available. Call generateSessionKey first.");
            }
            
            const encryptedData = Uint8Array.from(atob(encryptedMessage), c => c.charCodeAt(0));
            const ivData = Uint8Array.from(atob(iv), c => c.charCodeAt(0));
            
            const decryptedData = await window.crypto.subtle.decrypt(
                {
                    name: "AES-GCM",
                    iv: ivData
                },
                this.sessionKey,
                encryptedData
            );
            
            return new TextDecoder().decode(decryptedData);
        } catch (error) {
            console.error("Error decrypting message:", error);
            return "[Encrypted message - unable to decrypt]";
        }
    }
    
    /**
     * Encrypt session key for a participant
     * @param {string} participantId ID of the participant
     * @param {string} publicKeyString Base64 encoded public key of participant
     * @returns {Promise<string>} Encrypted session key
     */
    async encryptSessionKeyForParticipant(publicKeyString) {
        try {
            if (!this.sessionKey) {
                throw new Error("No session key available. Call generateSessionKey first.");
            }
            
            const publicKey = await this.importPublicKey(publicKeyString);
            
            const exportedSessionKey = await window.crypto.subtle.exportKey(
                "raw",
                this.sessionKey
            );
            
            const encryptedSessionKey = await window.crypto.subtle.encrypt(
                {
                    name: "RSA-OAEP"
                },
                publicKey,
                exportedSessionKey
            );
            
            return btoa(String.fromCharCode(...new Uint8Array(encryptedSessionKey)));
        } catch (error) {
            console.error("Error encrypting session key:", error);
            throw error;
        }
    }
    
    /**
     * Decrypt session key received from another participant
     * @param {string} encryptedSessionKey Base64 encoded encrypted session key
     * @returns {Promise<void>} Resolves when key is imported
     */
    async decryptSessionKey(encryptedSessionKey) {
        try {
            if (!this.keyPair) {
                throw new Error("No key pair available. Call generateKeyPair first.");
            }
            
            const encryptedKeyData = Uint8Array.from(atob(encryptedSessionKey), c => c.charCodeAt(0));
            
            const decryptedKeyData = await window.crypto.subtle.decrypt(
                {
                    name: "RSA-OAEP"
                },
                this.keyPair.privateKey,
                encryptedKeyData
            );
            
            this.sessionKey = await window.crypto.subtle.importKey(
                "raw",
                decryptedKeyData,
                {
                    name: "AES-GCM",
                    length: 256
                },
                false,
                ["encrypt", "decrypt"]
            );
            
            console.log("Session key decrypted and imported successfully");
        } catch (error) {
            console.error("Error decrypting session key:", error);
            throw error;
        }
    }
    
    /**
     * Store a participant's public key
     * @param {string} participantId ID of the participant
     * @param {string} publicKeyString Base64 encoded public key
     */
    storeParticipantPublicKey(participantId, publicKeyString) {
        this.participantPublicKeys[participantId] = publicKeyString;
    }
}

// Create a global instance
window.secureChatEncryption = new SecureChatEncryption();