<template>
  <div class="register-container">
    <div class="register-wrapper">
      <div class="register-card">
        <h1>Create Account</h1>
        
        <!-- Loading indicator -->
        <div v-if="isLoading" class="loading-container">
          <div class="loader"></div>
          <p>Creating your account...</p>
        </div>

        <!-- Success message -->
        <div v-if="success" class="success-message">
          <i class="icon-check"></i>
          {{ success }}
        </div>

        <!-- Error message -->
        <div v-if="error" class="error-message">
          <i class="icon-warning"></i>
          {{ error }}
        </div>

        <form @submit.prevent="handleSubmit" v-if="!isLoading && !success">
          <div class="form-group">
            <label for="name">Full Name</label>
            <input 
              type="text" 
              id="name"
              v-model="form.name" 
              :class="{ 'input-error': errors.name }"
              placeholder="Enter your full name"
            />
            <div v-if="errors.name" class="error">{{ errors.name }}</div>
          </div>

          <div class="form-group">
            <label for="username">Username</label>
            <input 
              type="text" 
              id="username"
              v-model="form.username" 
              :class="{ 'input-error': errors.username }"
              placeholder="Enter your username"
            />
            <div v-if="errors.username" class="error">{{ errors.username }}</div>
          </div>

          <div class="form-group">
            <label for="email">Email</label>
            <input 
              type="email" 
              id="email"
              v-model="form.email" 
              :class="{ 'input-error': errors.email }"
              placeholder="Enter your email address"
            />
            <div v-if="errors.email" class="error">{{ errors.email }}</div>
          </div>

          <div class="form-group">
            <label for="phoneNumber">Phone Number</label>
            <input 
              type="tel" 
              id="phoneNumber"
              v-model="form.phoneNumber" 
              :class="{ 'input-error': errors.phoneNumber }"
              placeholder="Enter your phone number"
            />
            <div v-if="errors.phoneNumber" class="error">{{ errors.phoneNumber }}</div>
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <input 
              type="password" 
              id="password"
              v-model="form.password" 
              :class="{ 'input-error': errors.password }"
              placeholder="Enter your password"
            />
            <div v-if="errors.password" class="error">{{ errors.password }}</div>
          </div>

          <div class="form-group">
            <label for="confirmPassword">Confirm Password</label>
            <input 
              type="password" 
              id="confirmPassword"
              v-model="form.confirmPassword" 
              :class="{ 'input-error': errors.confirmPassword }"
              placeholder="Confirm your password"
            />
            <div v-if="errors.confirmPassword" class="error">
              {{ errors.confirmPassword }}
            </div>
          </div>

          <button 
            class="button button--action submit-btn" 
            type="submit"
            :disabled="isLoading"
          >
            <span v-if="!isLoading">Register</span>
            <span v-else>Creating Account...</span>
          </button>
        </form>

        <div class="login-link" v-if="!isLoading">
          <span>Already have an account? </span>
          <router-link to="/login" class="link">Sign in here</router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      form: {
        name: "",
        username: "",
        email: "",
        phoneNumber: "",
        password: "",
        confirmPassword: "",
      },
      errors: {},
      isLoading: false,
      success: "",
      error: "",
    };
  },
  methods: {
    validateEmail(email) {
      // Simple email regex
      const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return re.test(email);
    },
    
    validatePhoneNumber(phone) {
      // Vietnamese phone number regex (simple version)
      const re = /^[0-9]{10,11}$/;
      return re.test(phone.replace(/\s/g, ''));
    },

    validateForm() {
      this.errors = {};
      
      if (!this.form.name) {
        this.errors.name = "Full name is required.";
      } else if (this.form.name.length < 2) {
        this.errors.name = "Full name must be at least 2 characters.";
      }
      
      if (!this.form.username) {
        this.errors.username = "Username is required.";
      } else if (this.form.username.length < 3) {
        this.errors.username = "Username must be at least 3 characters.";
      } else if (!/^[a-zA-Z0-9_]+$/.test(this.form.username)) {
        this.errors.username = "Username can only contain letters, numbers, and underscores.";
      }
      
      if (!this.form.email) {
        this.errors.email = "Email is required.";
      } else if (!this.validateEmail(this.form.email)) {
        this.errors.email = "Please enter a valid email address.";
      }
      
      if (!this.form.phoneNumber) {
        this.errors.phoneNumber = "Phone number is required.";
      } else if (!this.validatePhoneNumber(this.form.phoneNumber)) {
        this.errors.phoneNumber = "Please enter a valid phone number (10-11 digits).";
      }
      
      if (!this.form.password) {
        this.errors.password = "Password is required.";
      } else if (this.form.password.length < 6) {
        this.errors.password = "Password must be at least 6 characters.";
      }
      
      if (!this.form.confirmPassword) {
        this.errors.confirmPassword = "Please confirm your password.";
      } else if (this.form.password !== this.form.confirmPassword) {
        this.errors.confirmPassword = "Passwords do not match.";
      }
      
      return Object.keys(this.errors).length === 0;
    },

    async registerUser() {
      try {
        const registerData = {
          name: this.form.name,
          username: this.form.username,
          email: this.form.email,
          phoneNumber: this.form.phoneNumber,
          password: this.form.password
        };

        const response = await fetch('http://localhost:5174/api/auth/register', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(registerData),
        });

        const data = await response.json();

        if (response.ok) {
          // Registration successful
          this.success = `Account created successfully! Welcome, ${data.name}!`;
          
          // Clear form
          this.form = {
            name: "",
            username: "",
            email: "",
            phoneNumber: "",
            password: "",
            confirmPassword: "",
          };

          // Redirect to login page after a delay
          setTimeout(() => {
            this.$router.push('/login');
          }, 2000);
        } else {
          // Registration failed
          this.error = data.message || data || 'Registration failed. Please try again.';
        }
      } catch (e) {
        console.error('Registration error:', e);
        this.error = 'Network error. Please check your connection and try again.';
      }
    },

    async handleSubmit() {
      // Clear previous messages
      this.success = "";
      this.error = "";
      
      // Validate form
      if (!this.validateForm()) {
        return;
      }

      this.isLoading = true;
      
      try {
        await this.registerUser();
      } finally {
        this.isLoading = false;
      }
    },
  },
};
</script>

<style scoped>
.register-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--Gradient-Background-System);
  padding: var(--Gap-Large);
}

.register-wrapper {
  width: 100%;
  max-width: 500px;
}

.register-card {
  background: var(--Color-Background-Highlight);
  border-radius: 8px;
  padding: 48px;
  box-shadow: var(--ShadowHeavy);
  border: 1px solid var(--Color-Background-Hover);
}

h1 {
  font: var(--Text-HeadingLarge);
  color: var(--Color-Text-Main);
  text-align: center;
  margin-bottom: 32px;
  font-weight: 700;
}

.form-group {
  margin-bottom: var(--Gap-Large);
}

label {
  display: block;
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Main);
  font-weight: 600;
  margin-bottom: 8px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

input {
  width: 100%;
  height: 48px;
  background-color: var(--Color-Background-System);
  border: 2px solid var(--Color-Background-Hover);
  border-radius: 4px;
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  padding: 0 16px;
  transition: all 0.3s ease;
}

input:focus {
  outline: none;
  border-color: var(--Color-Primary);
  box-shadow: 0 0 0 3px rgba(102, 192, 244, 0.1);
}

input::placeholder {
  color: var(--Color-Text-Dim);
}

.input-error {
  border-color: var(--Color-Accent-Red) !important;
}

.input-error:focus {
  box-shadow: 0 0 0 3px rgba(205, 84, 68, 0.1) !important;
}

.error {
  color: var(--Color-Accent-Red);
  font: var(--Text-BodySmall);
  margin-top: 6px;
  display: flex;
  align-items: center;
}

.error::before {
  content: "⚠";
  margin-right: 6px;
}

.submit-btn {
  width: 100%;
  height: 48px;
  margin-top: var(--Gap-Large);
  font: var(--Text-BodyMedium);
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  transition: all 0.3s ease;
}

.submit-btn:hover {
  transform: translateY(-1px);
  box-shadow: var(--ShadowLight);
}

.login-link {
  text-align: center;
  margin-top: 32px;
  padding-top: 24px;
  border-top: 1px solid var(--Color-Background-Hover);
}

.login-link span {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
}

.link {
  color: var(--Color-Primary);
  text-decoration: none;
  font: var(--Text-BodyMedium);
  font-weight: 600;
  transition: color 0.3s ease;
}

.link:hover {
  color: var(--Color-Text-Main);
  text-decoration: underline;
}

/* Loading */
.loading-container {
  text-align: center;
  padding: 40px 20px;
}

.loader {
  border: 3px solid var(--Color-Background-Hover);
  border-top: 3px solid var(--Color-Primary);
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.loading-container p {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
}

/* Success Message */
.success-message {
  background: linear-gradient(135deg, rgba(76, 175, 80, 0.1), rgba(76, 175, 80, 0.05));
  border: 1px solid rgba(76, 175, 80, 0.3);
  border-radius: 6px;
  padding: 16px;
  margin-bottom: 24px;
  display: flex;
  align-items: center;
  color: #4CAF50;
  font: var(--Text-BodyMedium);
}

.success-message .icon-check::before {
  content: "✓";
  margin-right: 12px;
  font-size: 18px;
  font-weight: bold;
}

/* Error Message */
.error-message {
  background: linear-gradient(135deg, rgba(244, 67, 54, 0.1), rgba(244, 67, 54, 0.05));
  border: 1px solid rgba(244, 67, 54, 0.3);
  border-radius: 6px;
  padding: 16px;
  margin-bottom: 24px;
  display: flex;
  align-items: center;
  color: #F44336;
  font: var(--Text-BodyMedium);
}

.error-message .icon-warning::before {
  content: "⚠";
  margin-right: 12px;
  font-size: 18px;
  font-weight: bold;
}

/* Disabled button */
.submit-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none !important;
}

/* Responsive */
@media (max-width: 768px) {
  .register-container {
    padding: var(--Gap-Medium);
  }
  
  .register-card {
    padding: 32px 24px;
  }
  
  h1 {
    font: var(--Text-HeadingMedium);
    margin-bottom: 24px;
  }
}

@media (max-width: 480px) {
  .register-card {
    padding: 24px 16px;
  }
  
  input {
    height: 44px;
  }
  
  .submit-btn {
    height: 44px;
  }
}
</style>
