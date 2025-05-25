<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

const router = useRouter()
const username = ref('')
const password = ref('')
const error = ref('')
const isLoading = ref(false)

const handleSubmit = async (e) => {
    e.preventDefault()
    error.value = ''
    isLoading.value = true

    try {
        const response = await axios.post('http://localhost:5174/api/auth/login', {
            username: username.value,
            password: password.value
        }, {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            }
        })

        if (response.data.token) {
            // Store the token in localStorage
            localStorage.setItem('token', response.data.token)
            // Force reload the page to update the navigation
            window.location.href = '/'
        }
    } catch (err) {
        console.error('Login error:', err)
        error.value = err.response?.data?.message || 'Login failed. Please check your credentials.'
    } finally {
        isLoading.value = false
    }
}
</script>

<template>
    <main>
        <h1>Sign In</h1>
        <form @submit="handleSubmit">
            <p>SIGN IN WITH USERNAME</p>
            <input 
                type="text" 
                v-model="username"
                required
                placeholder="Enter your username"
                :disabled="isLoading"
            >
            <p>PASSWORD</p>
            <input 
                type="password" 
                v-model="password"
                required
                placeholder="Enter your password"
                :disabled="isLoading"
            >
            <p v-if="error" class="error-message">{{ error }}</p>
            <button 
                type="submit" 
                class="button button--action"
                :disabled="isLoading"
            >
                {{ isLoading ? 'Signing in...' : 'Sign In' }}
            </button>
            <p style="align-self: center;">
                New to Automach? 
                <router-link to="/register" style="color: #f3f3f3">Create an account</router-link>
            </p>
            <router-link to="/forgot-password" style="align-self: center;">Forgot Password?</router-link>
        </form>
    </main>
</template>

<style scoped>
main {
  font: var(--Text-BodyMedium);

  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: var(--Gradient-Background-Main);
}

main h1 {
  color: var(--Color-Text-Main);
  padding: 24px 0px;
}

main form {
  max-width: fit-content;
  display: flex;
  flex-direction: column;
  /* align-items: center; */
  justify-content: center;
  background-color: var(--Color-Background-Main50);
  padding: 60px;
  border-radius: 3px;
  box-shadow: 0px 0px 10px 0px rgba(0, 0, 0, 0.2);
}

main form > * {
  margin: 8px 0px 0px 0px;
  color: var(--Color-Text-Main);
}

main form input {
  font: var(--Text-BodyMedium);
  width: 350px;
  height: 30px;
  padding: 8px;
  border-radius: 3px;
  border: none;
  color: var(--Color-Text-Main);
  background-color: var(--Color-Background-Secondary);
}

main form input::placeholder {
  color: var(--Color-Text-Main);
  opacity: 0.7;
}

main form p {
  color: var(--Color-Text-Main);
  font: var(--Text-BodySmall);
  font-weight: bold;
}

.button--action {
  margin-top: 16px;
  width: 100%;
}

a {
  text-decoration: none;
}

a:hover {
  color: var(--Color-Primary);
}

.error-message {
    color: #ff4444 !important;
    font-size: 0.9em;
    margin-top: 8px;
}
</style>