<template>
  <nav class="navbar">
    <div class="nav-container">
      <div class="nav-left">
        <router-link to="/browse" class="browse-btn">
          <i class="fas fa-gamepad"></i>
          Browse
        </router-link>
      </div>
      
      <div class="nav-center">
        <div class="search-container">
          <input 
            type="text" 
            class="search-input" 
            placeholder="Search games..."
            v-model="searchQuery"
            @keyup.enter="handleSearch"
          >
          <button class="search-btn" @click="handleSearch">
            <i class="fas fa-search"></i>
          </button>
        </div>
      </div>

      <div class="nav-right">
        <router-link to="/cart" class="cart-btn">
          <i class="fas fa-shopping-cart"></i>
          <span class="cart-count" v-if="cartItemCount">{{ cartItemCount }}</span>
        </router-link>
      </div>
    </div>
  </nav>
</template>

<script>
export default {
  name: 'Nav',
  data() {
    return {
      searchQuery: '',
      cartItemCount: 0
    }
  },
  methods: {
    handleSearch() {
      if (this.searchQuery.trim()) {
        this.$router.push({ 
          path: '/browse', 
          query: { search: this.searchQuery }
        })
      }
    }
  }
}
</script>

<style scoped>
.navbar {
  background-color: #1a1a1a;
  padding: 1rem 0;
  position: sticky;
  top: 0;
  z-index: 1000;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.nav-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.nav-left, .nav-center, .nav-right {
  display: flex;
  align-items: center;
}

.browse-btn, .cart-btn {
  color: #ffffff;
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  transition: background-color 0.3s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.browse-btn:hover, .cart-btn:hover {
  background-color: #333;
}

.search-container {
  display: flex;
  align-items: center;
  background-color: #333;
  border-radius: 4px;
  padding: 0.25rem;
}

.search-input {
  background: none;
  border: none;
  color: #ffffff;
  padding: 0.5rem;
  width: 300px;
  outline: none;
}

.search-input::placeholder {
  color: #888;
}

.search-btn {
  background: none;
  border: none;
  color: #ffffff;
  padding: 0.5rem;
  cursor: pointer;
  transition: color 0.3s;
}

.search-btn:hover {
  color: #00ff88;
}

.cart-count {
  background-color: #00ff88;
  color: #1a1a1a;
  border-radius: 50%;
  padding: 0.2rem 0.5rem;
  font-size: 0.8rem;
  font-weight: bold;
}

@media (max-width: 768px) {
  .search-input {
    width: 200px;
  }
  
  .nav-container {
    padding: 0 0.5rem;
  }
}

@media (max-width: 480px) {
  .search-input {
    width: 150px;
  }
  
  .browse-btn span, .cart-btn span {
    display: none;
  }
}
</style> 