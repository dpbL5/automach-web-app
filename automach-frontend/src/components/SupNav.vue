<template>
  <header>
    <div class="limitbox">
      <!-- <h1 class="logo-text">AUTOMACH</h1> -->
       <img src="../assets/images/logo.png" @click.prevent="goHome" alt="logo" class="logo">
      <nav class="container">
        <ul>
          <li v-if="!isAdmin"><a @click.prevent="goHome" href="#">Store</a></li>
          <li v-if="userName"><a @click.prevent="goAccount" href="#">{{ userName }}</a></li>
          <li v-else style="opacity:0.5;pointer-events:none" >Guest</li>
          <li v-if="!isAdmin && userName"><a @click.prevent="goLibrary" href="#">Library</a></li>
          <li v-if="!isAdmin && !userName" style="opacity:0.5;pointer-events:none">Library</li>
          <li v-if="isAdmin"><a @click.prevent="goAdminDashboard" href="#">Admin Panel</a></li>
        </ul>
      </nav>
      <div class="cta">
        <router-link v-if="!userName" to="/login">Log in</router-link>
        <button v-if="!userName" @click="goRegister">Register</button>
        <button v-if="userName" @click="logout" style="margin-left:16px;">Sign Out</button>
      </div>
    </div>
  </header>
</template>

<script>
import { eventBus } from '../eventBus';
import { cacheManager } from '../helpers/cacheManager';

export default {
  data() {
    return {
      userName: null,
      isAdmin: false,
    };
  },
  mounted() {
    // Khởi tạo thông tin người dùng từ localStorage
    this.checkUserInfo();
    
    // Đăng ký sự kiện để cập nhật thông tin khi đăng nhập
    eventBus.on('user-logged-in', this.updateUserInfo);
  },
  beforeUnmount() {
    // Hủy đăng ký sự kiện khi component bị hủy
    eventBus.off('user-logged-in', this.updateUserInfo);
  },
  methods: {
    goHome() {
      this.$router.push('/');
    },
    goRegister() {
      this.$router.push('/register');
    },
    goAccount() {
      this.$router.push('/account');
    },
    goLibrary() { 
      this.$router.push('/library'); 
    },
    goAdminDashboard() {
      this.$router.push('/admin/dashboard');
    },
    checkUserInfo() {
      this.userName = localStorage.getItem('userName');
      this.isAdmin = localStorage.getItem('role') === 'admin';
    },
    updateUserInfo(payload) {
      console.log('SupNav received user-logged-in event:', payload);
      
      if (payload && payload.name) {
        this.userName = payload.name;
        localStorage.setItem('userName', payload.name);
      } else {
        this.checkUserInfo();
      }
      
      if (payload && payload.role) {
        this.isAdmin = payload.role === 'admin';
        localStorage.setItem('role', payload.role);
      } else {
        this.isAdmin = localStorage.getItem('role') === 'admin';
      }
      
      // Lưu userId nếu có
      if (payload && payload.accountId) {
        localStorage.setItem('userId', payload.accountId);
      }
    },
    logout() {
      localStorage.removeItem('token');
      localStorage.removeItem('userName');
      localStorage.removeItem('username');
      localStorage.removeItem('userId');
      localStorage.removeItem('role');
      localStorage.removeItem('cart'); // Clear cart when logging out
      
      // Clear all caches
      cacheManager.clearAll();
      
      this.userName = null;
      this.isAdmin = false;
      
      console.log('All caches cleared on logout');
      
      // Emit cart-updated event to update Nav component
      window.dispatchEvent(new Event('cart-updated'));
      
      this.$router.push('/login');
    },
  },
  // Khai báo currentRoute để theo dõi đường dẫn hiện tại
  computed: {
    currentRoute() {
      return this.$route.path;
    }
  },
  // Theo dõi sự thay đổi của route để cập nhật thông tin người dùng
  watch: {
    currentRoute() {
      // Cập nhật thông tin người dùng khi chuyển trang
      this.checkUserInfo();
    }
  }
};
</script>

<style scoped>
header {
  height: 58px;
  background-color: var(--Color-Background-Main);
  display: flex;
  justify-content: center;
}

.limitbox {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 940px;
}

.logo-text {
  color: var(--Color-Primary, #66c0f4);
  font-size: 16px;
  font-weight: 700;
  letter-spacing: 1px;
}

nav,
ul {
  display: flex;
  list-style: none;
  justify-content: space-between;
}

li,
a {
  text-decoration: none;
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingSmall);
  padding: 0px 16px;
}

li {
  text-transform: uppercase;
}

.cta {
  align-items: center;
}

button {
  border: none;
  border-radius: 3px;
  background-color: var(--Color-Background-Highlight);
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingSmall);
  padding: 8px 16px;
}
</style>
