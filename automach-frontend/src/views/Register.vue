<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

// Configure axios defaults
const api = axios.create({
    baseURL: 'http://localhost:5174',
    withCredentials: true,
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    }
})

const router = useRouter()
const username = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const error = ref('')
const isLoading = ref(false)

// Password strength validation
const passwordStrength = computed(() => {
    if (!password.value) return 0
    
    let strength = 0
    // Length check
    if (password.value.length >= 8) strength += 1
    // Contains number
    if (/\d/.test(password.value)) strength += 1
    // Contains lowercase
    if (/[a-z]/.test(password.value)) strength += 1
    // Contains uppercase
    if (/[A-Z]/.test(password.value)) strength += 1
    // Contains special character
    if (/[!@#$%^&*(),.?":{}|<>]/.test(password.value)) strength += 1
    
    return strength
})

const strengthLabel = computed(() => {
    const strength = passwordStrength.value
    if (strength <= 2) return { text: 'Weak', color: '#ff4444' }
    if (strength <= 4) return { text: 'Medium', color: '#ffbb33' }
    return { text: 'Strong', color: '#00C851' }
})

const passwordRequirements = computed(() => {
    return [
        { met: password.value.length >= 8, text: 'At least 8 characters' },
        { met: /\d/.test(password.value), text: 'Contains a number' },
        { met: /[a-z]/.test(password.value), text: 'Contains a lowercase letter' },
        { met: /[A-Z]/.test(password.value), text: 'Contains an uppercase letter' },
        { met: /[!@#$%^&*(),.?":{}|<>]/.test(password.value), text: 'Contains a special character' }
    ]
})

const isPasswordValid = computed(() => {
    return passwordStrength.value >= 3
})

const handleSubmit = async (e) => {
    e.preventDefault()
    error.value = ''
    isLoading.value = true

    try {
        if (!isPasswordValid.value) {
            error.value = 'Password does not meet the requirements'
            return
        }

        if (password.value !== confirmPassword.value) {
            error.value = 'Passwords do not match'
            return
        }

        const response = await api.post('/api/auth/register', {
            username: username.value,
            email: email.value,
            password: password.value,
            name: username.value,
            phoneNumber: ''
        })

        if (response.data) {
            router.push('/login')
        }
    } catch (err) {
        console.error('Registration error:', err)
        if (err.code === 'ERR_NETWORK') {
            error.value = 'Unable to connect to the server. Please check if the server is running.'
        } else if (err.response?.status === 0) {
            error.value = 'Connection error. Please check if the server is running at http://localhost:5174'
        } else {
            error.value = err.response?.data?.message || 'Registration failed. Please try again.'
        }
    } finally {
        isLoading.value = false
    }
}
</script>

<template>
    <main>
        <h1>Create Account</h1>
        <form @submit="handleSubmit">
            <p>USERNAME</p>
            <input 
                type="text" 
                v-model="username"
                required
                placeholder="Enter your username"
                :disabled="isLoading"
            >
            <p>EMAIL</p>
            <input 
                type="email" 
                v-model="email"
                required
                placeholder="Enter your email"
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
            <div class="password-strength">
                <div class="strength-meter">
                    <div 
                        class="strength-meter-fill"
                        :style="{ width: (passwordStrength * 20) + '%', backgroundColor: strengthLabel.color }"
                    ></div>
                </div>
                <span :style="{ color: strengthLabel.color }">{{ strengthLabel.text }}</span>
            </div>
            <div class="password-requirements">
                <p v-for="(req, index) in passwordRequirements" 
                   :key="index"
                   :class="{ 'requirement-met': req.met }">
                    {{ req.met ? '✓' : '×' }} {{ req.text }}
                </p>
            </div>
            <p>CONFIRM PASSWORD</p>
            <input 
                type="password" 
                v-model="confirmPassword"
                required
                placeholder="Confirm your password"
                :disabled="isLoading"
            >
            <p v-if="error" class="error-message">{{ error }}</p>
            <button 
                type="submit" 
                class="button button--action"
                :disabled="!isPasswordValid || isLoading"
            >
                {{ isLoading ? 'Creating Account...' : 'Create Account' }}
            </button>
            <p style="align-self: center;">
                Already have an account? 
                <router-link to="/login" style="color: #f3f3f3">Sign in</router-link>
            </p>
        </form>
    </main>
</template>

<style scoped>
main {
  font: var(--Text-BodyMedium);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  align-items: center;
  /* height: 100vh; */
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
  justify-content: center;
  background-color: var(--Color-Background-Main50);
  padding: 60px;
  border-radius: 3px;
  box-shadow: 0px 0px 10px 0px rgba(0, 0, 0, 0.2);
  margin-bottom: 100px;
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

.error-message {
  color: #ff4444 !important;
  font-size: 0.9em;
  margin-top: 8px;
  text-align: center;
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

.password-strength {
    margin: 8px 0;
    display: flex;
    align-items: center;
    gap: 10px;
}

.strength-meter {
    flex-grow: 1;
    height: 4px;
    background-color: #333;
    border-radius: 2px;
    overflow: hidden;
}

.strength-meter-fill {
    height: 100%;
    transition: all 0.3s ease;
}

.password-requirements {
    margin: 8px 0;
    font-size: 0.8em;
}

.password-requirements p {
    margin: 4px 0;
    color: var(--Color-Text-Dim);
    transition: color 0.3s ease;
}

.requirement-met {
    color: #00C851 !important;
}

button:disabled {
    opacity: 0.7;
    cursor: not-allowed;
}
</style>