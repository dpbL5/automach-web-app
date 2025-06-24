import { createRouter, createWebHistory } from 'vue-router';
import Home from './components/Home.vue';
import Login from './components/Login.vue';
import Register from './components/Register.vue';
import GamePage from './components/GamePage.vue';
import AccountDetail from './components/AccountDetail.vue';
import GameLibrary from './components/GameLibrary.vue';
import AdminDashboard from './components/AdminDashboard.vue';
import AdminGameList from './components/AdminGameList.vue';
import AdminTagList from './components/AdminTagList.vue';
import AddGame from './components/AddGame.vue';
import Browse from './components/Browse.vue';
import Cart from './components/Cart.vue';
import Checkout from './components/Checkout.vue';
import CheckoutSuccess from './components/CheckoutSuccess.vue';

const routes = [
  { path: '/', name: 'Home', component: Home },
  { path: '/login', name: 'Login', component: Login },
  { path: '/register', name: 'Register', component: Register },
  { path: '/game/:id', name: 'GamePage', component: GamePage },
  { path: '/account', name: 'AccountDetail', component: AccountDetail },
  { path: '/library', name: 'GameLibrary', component: GameLibrary },
  { path: '/browse', name: 'Browse', component: Browse },
  { path: '/cart', name: 'Cart', component: Cart },
  { 
    path: '/checkout', 
    name: 'Checkout', 
    component: Checkout,
    meta: { requiresAuth: true }
  },
  { 
    path: '/checkout/success', 
    name: 'CheckoutSuccess', 
    component: CheckoutSuccess,
    meta: { requiresAuth: true }
  },
  
  // Admin routes
  { 
    path: '/admin/dashboard', 
    name: 'AdminDashboard', 
    component: AdminDashboard,
    meta: { requiresAdmin: true }
  },
  { 
    path: '/admin/games', 
    name: 'AdminGameList', 
    component: AdminGameList,
    meta: { requiresAdmin: true }
  },
  { 
    path: '/admin/games/add', 
    name: 'AddGame', 
    component: AddGame,
    meta: { requiresAdmin: true }
  },
  { 
    path: '/admin/games/edit/:id', 
    name: 'EditGame', 
    component: AddGame,
    meta: { requiresAdmin: true }
  },
  { 
    path: '/admin/tags', 
    name: 'AdminTagList', 
    component: AdminTagList,
    meta: { requiresAdmin: true }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Navigation guard for admin and auth routes
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  const role = localStorage.getItem('role');
  
  if (to.meta.requiresAdmin) {
    if (role !== 'admin') {
      next('/login');
      return;
    }
  }
  
  if (to.meta.requiresAuth) {
    if (!token) {
      next('/login');
      return;
    }
  }
  
  next();
});

export default router; 