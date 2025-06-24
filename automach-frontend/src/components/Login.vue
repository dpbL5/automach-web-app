<template>
  <div class="login-page">
    <div class="login-container">
      <div class="login-header">
        <img src="../assets/images/logo.png" alt="AutoMach Logo" class="logo" />
        <h1>Welcome Back</h1>
        <p class="subtitle">Sign in to your account to continue</p>
      </div>
      
      <div class="login-form">
        <form @submit.prevent="handleLogin">
          <div class="form-group">
            <label for="username">Username</label>
            <div class="input-container">
              <i class="icon-user"></i>
              <input 
                type="text" 
                id="username" 
                placeholder="Enter your username" 
                v-model="username"
                :class="{ 'input-error': error }"
              />
            </div>
          </div>
          
          <div class="form-group">
            <label for="password">Password</label>
            <div class="input-container">
              <i class="icon-lock"></i>
              <input 
                type="password" 
                id="password" 
                placeholder="Enter your password" 
                v-model="password" 
                :class="{ 'input-error': error }"
              />
            </div>
          </div>
          
          <div class="remember-forgot">
            <div class="remember">
              <input type="checkbox" id="remember" v-model="remember" />
              <label for="remember">Remember me</label>
            </div>
            <a href="#" class="forgot-password">Forgot password?</a>
          </div>
          
          <button 
            class="login-button" 
            type="submit"
            :disabled="isLoading"
          >
            <span v-if="isLoading" class="loader"></span>
            <span v-else>Login</span>
          </button>
          
          <div v-if="error" class="error-message">
            <i class="icon-error"></i>
            <span>{{ error }}</span>
          </div>
          
          <div v-if="success" class="success-message">
            <i class="icon-success"></i>
            <span>{{ success }}</span>
          </div>
        </form>
        
        <div class="signup-section">
          <p>Don't have an account? <router-link to="/register" class="signup-link">Sign up</router-link></p>
        </div>
      </div>
      
      <div class="login-footer">
        <p>&copy; 2024 AutoMach Games. All rights reserved.</p>
      </div>
    </div>
    
    <div class="decoration">
      <div class="circle circle-1"></div>
      <div class="circle circle-2"></div>
      <div class="circle circle-3"></div>
    </div>
  </div>
</template>

<script>
import { eventBus } from '../eventBus';
import { cacheManager } from '../helpers/cacheManager';

export default {
  data() {
    return {
      username: '',
      password: '',
      remember: false,
      error: '',
      success: '',
      isLoading: false
    };
  },
  methods: {
    async handleLogin() {
      this.error = '';
      this.success = '';
      
      if (!this.username.trim()) {
        this.error = 'Please enter your username';
        return;
      }
      
      if (!this.password) {
        this.error = 'Please enter your password';
        return;
      }
      
      this.isLoading = true;
      
      try {
        const response = await fetch('http://localhost:5174/api/auth/login', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            username: this.username,
            password: this.password,
          }),
        });
        
        if (!response.ok) {
          const err = await response.text();
          this.error = err || 'Invalid username or password';
          this.isLoading = false;
          return;
        }
        
        const data = await response.json();
        
        // Save user data to localStorage
        if (data.token) {
          localStorage.setItem('token', data.token);
        } else {
          console.error('No token in login response:', data);
        }
        
        localStorage.setItem('userName', data.name || data.username);
        localStorage.setItem('username', data.username);
        
        // Save role if available
        if (data.role) {
          localStorage.setItem('role', data.role);
        }
        
        // Save userId
        if (data.accountId) {
          localStorage.setItem('userId', data.accountId);
        }
        
        // Create payload for login event
        const userInfo = { 
          name: data.name || data.username, 
          role: data.role,
          accountId: data.accountId,
          username: data.username,
          email: data.email,
          id: data.accountId
        };
        
        // Cache user information
        cacheManager.user.set(userInfo);
        
        // Emit event to update SupNav
        eventBus.emit('user-logged-in', userInfo);
        
        // Show success message
        this.success = `Login successful! Welcome, ${data.name || data.username}`;
        
        // Redirect based on role
        setTimeout(() => {
          if (data.role === 'admin') {
            this.$router.push('/admin/dashboard');
          } else {
            this.$router.push('/');
          }
        }, 1500);
      } catch (e) {
        console.error('Login error:', e);
        this.error = 'Network error. Please try again later.';
      } finally {
        this.isLoading = false;
      }
    }
  }
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: var(--Gradient-Background-Main);
  position: relative;
  overflow: hidden;
  padding: 20px;
}

.login-container {
  width: 480px;
  max-width: 100%;
  background-color: var(--Color-Background-Main80);
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
  backdrop-filter: blur(10px);
  z-index: 10;
  position: relative;
  overflow: hidden;
  animation: fadeIn 0.5s ease;
}

.login-header {
  padding: 30px 30px 10px;
  text-align: center;
}

.logo {
  width: 120px;
  margin-bottom: 20px;
  animation: pulse 2s infinite;
}

h1 {
  font: var(--Text-HeadingLarge);
  color: var(--Color-Text-Main);
  margin-bottom: 10px;
  font-weight: 600;
}

.subtitle {
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Dim);
  margin-bottom: 25px;
}

.login-form {
  padding: 0 30px 20px;
}

.form-group {
  margin-bottom: 24px;
}

label {
  display: block;
  font: var(--Text-BodySmall);
  color: var(--Color-Text-Main);
  margin-bottom: 8px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.input-container {
  position: relative;
}

.input-container i {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--Color-Text-Dim);
  font-size: 18px;
}

.icon-user:before {
  content: "👤";
}

.icon-lock:before {
  content: "🔒";
}

input[type="text"],
input[type="password"] {
  width: 100%;
  height: 50px;
  padding: 0 15px 0 40px;
  border-radius: 6px;
  background-color: var(--Color-Background-System);
  border: 1px solid var(--Color-Background-Highlight);
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Main);
  transition: border-color 0.3s, box-shadow 0.3s;
}

input:focus {
  outline: none;
  border-color: var(--Color-Primary);
  box-shadow: 0 0 0 3px rgba(103, 193, 245, 0.2);
}

.input-error {
  border-color: var(--Color-Accent-Red) !important;
  animation: shake 0.5s ease;
}

.remember-forgot {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  font-size: 14px;
}

.remember {
  display: flex;
  align-items: center;
}

.remember input[type="checkbox"] {
  margin-right: 8px;
  accent-color: var(--Color-Primary);
}

.forgot-password {
  color: var(--Color-Primary);
  text-decoration: none;
  transition: color 0.2s;
}

.forgot-password:hover {
  color: var(--Color-Secondary);
  text-decoration: underline;
}

.login-button {
  width: 100%;
  height: 50px;
  background: var(--Gradient-Button-Action);
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s, opacity 0.2s;
  margin-bottom: 20px;
  position: relative;
}

.login-button:hover {
  transform: translateY(-2px);
}

.login-button:active {
  transform: translateY(0);
}

.login-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.loader {
  display: inline-block;
  width: 20px;
  height: 20px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  border-top-color: white;
  animation: spin 1s infinite linear;
  vertical-align: middle;
}

.error-message, 
.success-message {
  padding: 12px;
  border-radius: 6px;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  animation: fadeIn 0.3s ease;
}

.error-message {
  background-color: rgba(255, 107, 107, 0.1);
  border-left: 4px solid var(--Color-Accent-Red);
  color: #ff6b6b;
}

.success-message {
  background-color: rgba(76, 175, 80, 0.1);
  border-left: 4px solid #4caf50;
  color: #4caf50;
}

.icon-error:before {
  content: "⚠️";
  margin-right: 8px;
}

.icon-success:before {
  content: "✅";
  margin-right: 8px;
}

.signup-section {
  text-align: center;
  margin-top: 24px;
  padding-top: 24px;
  border-top: 1px solid var(--Color-Background-Highlight);
}

.signup-link {
  color: var(--Color-Primary);
  text-decoration: none;
  font-weight: 600;
  transition: color 0.2s;
}

.signup-link:hover {
  color: var(--Color-Secondary);
  text-decoration: underline;
}

.login-footer {
  text-align: center;
  padding: 20px;
  font-size: 12px;
  color: var(--Color-Text-Dim);
}

/* Decorative elements */
.decoration {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  pointer-events: none;
}

.circle {
  position: absolute;
  border-radius: 50%;
  opacity: 0.1;
}

.circle-1 {
  width: 300px;
  height: 300px;
  background: linear-gradient(to right, var(--Color-Primary), var(--Color-Secondary));
  top: -100px;
  right: -100px;
  animation: float 8s ease-in-out infinite;
}

.circle-2 {
  width: 500px;
  height: 500px;
  background: linear-gradient(to right, var(--Color-Secondary), var(--Color-Accent-Green));
  bottom: -200px;
  left: -200px;
  animation: float 12s ease-in-out infinite reverse;
}

.circle-3 {
  width: 200px;
  height: 200px;
  background: linear-gradient(to right, var(--Color-Accent-Purple), var(--Color-Primary));
  top: 50%;
  right: 10%;
  animation: float 10s ease-in-out infinite 2s;
}

/* Animations */
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  10%, 30%, 50%, 70%, 90% { transform: translateX(-5px); }
  20%, 40%, 60%, 80% { transform: translateX(5px); }
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-20px); }
}

@keyframes pulse {
  0% { transform: scale(1); }
  50% { transform: scale(1.05); }
  100% { transform: scale(1); }
}

/* Responsive design */
@media (max-width: 520px) {
  .login-container {
    width: 100%;
    border-radius: 8px;
  }
  
  .login-form {
    padding: 0 20px 20px;
  }
  
  input[type="text"],
  input[type="password"],
  .login-button {
    height: 45px;
  }
  
  .logo {
    width: 80px;
  }
  
  h1 {
    font-size: 24px;
  }
}
</style>
