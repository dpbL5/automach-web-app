<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

const router = useRouter()
const isLoggedIn = ref(false)
const username = ref('')
const isLoading = ref(true)

// Function to decode JWT token
const decodeToken = (token) => {
    try {
        // JWT token consists of three parts: header.payload.signature
        const base64Url = token.split('.')[1]
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
        }).join(''))

        return JSON.parse(jsonPayload)
    } catch (error) {
        console.error('Error decoding token:', error)
        return null
    }
}

// Check authentication status on component mount
onMounted(() => {
    console.log('SuperNav mounted, checking auth status...')
    const token = localStorage.getItem('token')
    console.log('Token exists:', !!token)
    
    if (token) {
        const decodedToken = decodeToken(token)
        console.log('Decoded token:', decodedToken)
        
        if (decodedToken && decodedToken.name) {
            isLoggedIn.value = true
            username.value = decodedToken.name
        } else {
            localStorage.removeItem('token')
            isLoggedIn.value = false
            username.value = ''
        }
    }
    isLoading.value = false
})

const handleLogout = () => {
    console.log('Logging out...')
    localStorage.removeItem('token')
    isLoggedIn.value = false
    username.value = ''
    window.location.href = '/login'
}
</script>

<template>
    <header class="header">
        <div class="header-container">
            <router-link to="/">
                <img src="../assets/images/AUTOMACH.png" alt="Logo">
            </router-link>
            <ul>
                <li><a href="#">STORE</a></li>
                <li><a href="#">LIBRARY</a></li>
            </ul>
            <div class="cta-container">
                <template v-if="isLoading">
                    <span class="loading">Loading...</span>
                </template>
                <template v-else-if="isLoggedIn">
                    <span class="username">{{ username }}</span>
                    <button @click="handleLogout" class="button button--logout">Logout</button>
                </template>
                <template v-else>
                    <router-link to="/login" class="login-link">Log in</router-link>
                    <router-link to="/register" class="button">Register</router-link>
                </template>
            </div>
        </div>
    </header>    
</template>

<style scoped>
.cta-container a {
    font: var(--Text-BodyMedium);
    color: var(--Color-Text-Main);
}

.cta-container {
    display: flex;
    align-items: center;
    gap: 1rem;
}

.header {
    background-color: var(--Color-Background-Main);
    display: flex;
    justify-content: center;
    flex-direction: row;
    padding: 10px 0px;
}

.header-container {
    width: 940px;
    justify-content: space-between;
    display: flex;
    flex-direction: row;
}

ul {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
}

li {
    list-style: none;
    padding: 0px 20px;
}

a { 
    align-self: center;
    text-decoration: none;
    color: var(--Color-Text-Main);
    font: var(--Text-HeadingSmall);
}

a:hover {
    color: var(--Color-Primary);
}

.login-link {
    text-decoration: none;
    color: inherit;
}

.button {
    padding: 8px 16px;
    border-radius: 4px;
    background-color: var(--Color-Primary);
    color: var(--Color-Text-Main);
    border: none;
    cursor: pointer;
    transition: background-color 0.3s ease;
}

.button:hover {
    background-color: var(--Color-Primary-Hover);
    color: var(--Color-Text-Main);
}

.button--logout {
    background-color: var(--Color-Background-Secondary);
}

.button--logout:hover {
    background-color: var(--Color-Background-Secondary-Hover);
}

.username {
    color: var(--Color-Text-Main);
    font: var(--Text-BodyMedium);
    font-weight: bold;
}

.loading {
    color: var(--Color-Text-Main);
    font: var(--Text-BodyMedium);
    opacity: 0.7;
}
</style>