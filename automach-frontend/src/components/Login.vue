<template>
    <br>
    <div class="login-container">
        <h1>Login</h1>
        <br>
        <div class="login-form">
            <form @submit.prevent="handleLogin">
                <label for="username">Username</label> 
                <input type="text" id="username" placeholder="Username" v-model="username" />
                <label for="password">Password</label>
                <input type="password" id="password" placeholder="Password" v-model="password" />
                <button class="button button--action" type="submit">Login</button>
                <br>
                <p>Don't have an account? <a href="#">Sign up</a></p>
            </form>
        </div>
    </div>
</template>

<script>

export default {
  data() {
    return {
      username: '',
      password: ''
    };
  },
  methods: {
    handleLogin() {
      // Process login here (e.g., call API or emit event)
      // For now, just log the credentials
      console.log('Logging in with:', this.username, this.password);
      // You can add your login logic here
    }
  }
}
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
    font: var(--Text-HeadingLarge);
    color: var(--Color-Text-Main);
    font-weight: 700;
}

p, a {
    font: var(--Text-BodyMedium);
    color: var(--Color-Text-Main);
    font-weight: 400;
    text-align: center;
}

input {
    width: 500px;
    height: 50px;
    border-radius: 3px;
    background-color: #2A475E;
    border: none;
    font: var(--Text-BodyMedium);
    color: var(--Color-Text-Main);
    font-weight: 600;
}

input:focus {
    outline: none;
}

.login-container {
    width: 780px;
    margin: 0 auto;
    height: 100vh;
}

.login-form form {
    height: 500px;
    padding: 0 140px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    border-radius: 3px;
    background-color: var(--Color-Background-Main50);
}

button {
    margin-top: 20px;
    align-self: center;
    width: 390px;
    height: 50px;
}

</style>
