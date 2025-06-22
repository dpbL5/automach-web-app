<template>
  <div class="register-container">
    <div class="register-wrapper">
      <h1>Create an account</h1>
      <form @submit.prevent="handleSubmit">
        <label for="username">Username</label>
        <input type="text" v-model="form.username" />
        <div v-if="errors.username" class="error">{{ errors.username }}</div>
        <label for="email">Email</label>
        <input type="email" v-model="form.email" />
        <div v-if="errors.email" class="error">{{ errors.email }}</div>
        <label for="password">Password</label>
        <input type="password" v-model="form.password" />
        <div v-if="errors.password" class="error">{{ errors.password }}</div>
        <label for="confirmPassword">Confirm password</label>
        <input type="password" v-model="form.confirmPassword" />
        <div v-if="errors.confirmPassword" class="error">
          {{ errors.confirmPassword }}
        </div>
        <button class="button button--action" type="submit">Register</button>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      form: {
        username: "",
        email: "",
        password: "",
        confirmPassword: "",
      },
      errors: {},
    };
  },
  methods: {
    validateEmail(email) {
      // Simple email regex
      const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return re.test(email);
    },
    handleSubmit() {
      this.errors = {};
      if (!this.form.username) {
        this.errors.username = "Username is required.";
      }
      if (!this.form.email) {
        this.errors.email = "Email is required.";
      } else if (!this.validateEmail(this.form.email)) {
        this.errors.email = "Please enter a valid email address.";
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
      if (Object.keys(this.errors).length === 0) {
        // Submit form or emit event
        alert("Registration successful!");
      }
    },
  },
};
</script>

<style scoped>
label {
  margin-top: 38px;
  text-transform: uppercase;
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Main);
  font-weight: 600;
}

h1 {
  margin-top: 100px;
  font: var(--Text-HeadingLarge);
  font-weight: 300;
  color: var(--Color-Text-Main);
}

button {
  margin-top: 40px;
  width: 200px;
  align-self: center;
}

.register-container {
  display: flex;
  flex-direction: column;
  align-items: center;

  height: 100vh;
  background: var(--Gradient-Background-System);
}
.register-wrapper form {
  width: 780px;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

input {
  width: 525px;
  height: 38px;
  background-color: #3d4450;
  border: none;
  color: var(--Color-Text-Main);
}

.error {
  color: #ff6b6b;
  font-size: 0.95em;
  margin-bottom: 8px;
  margin-top: 2px;
}
</style>
