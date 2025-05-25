import { createApp } from "vue";
import { createPinia } from "pinia";
import { createRouter, createWebHistory } from 'vue-router';
import App from "./App.vue";
import './assets/css/main.css';

// Import các components cho routes
const Home = () => import('./views/Home.vue');
const Login = () => import('./views/Login.vue');
const Register = () => import('./views/Register.vue');

// Cấu hình routes
const routes = [
  { path: '/', component: Home },
  { path: '/login', component: Login },
  { path: '/register', component: Register }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

const app = createApp(App);

app.use(createPinia());
app.use(router);

app.mount("#app");
