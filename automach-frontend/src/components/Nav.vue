<template>
  <div class="limitbox">
    <nav>
      <div>
        <router-link class="item" :class="{ 'item--selected': isHome }" to="/">Home</router-link>
        <router-link class="item" :class="{ 'item--selected': isBrowse }" to="/browse">Browse</router-link>
      </div>
      <input 
        class="searchbar" 
        type="text" 
        placeholder="Search games..." 
        v-model="searchQuery"
        @keyup.enter="performSearch"
      />
      <router-link class="item cart-item" :class="{ 'item--selected': isCart }" to="/cart">
        Cart
        <span v-if="cartItemCount > 0" class="cart-badge">{{ cartItemCount }}</span>
      </router-link>
    </nav>
  </div>
</template>

<script>
import { getCart } from '../helpers/cartHelper.js';

export default {
  data() {
    return {
      cartItemCount: 0,
      searchQuery: ''
    };
  },
  computed: {
    isHome() {
      return this.$route.path === '/';
    },
    isBrowse() {
      return this.$route.path === '/browse';
    },
    isCart() {
      return this.$route.path === '/cart';
    }
  },
  mounted() {
    this.updateCartCount();
    // Lắng nghe sự kiện cập nhật cart từ cartHelper
    window.addEventListener('cart-updated', this.updateCartCount);
  },
  beforeDestroy() {
    // Cleanup event listener
    window.removeEventListener('cart-updated', this.updateCartCount);
  },
  methods: {
    updateCartCount() {
      this.cartItemCount = getCart().length;
    },
    performSearch() {
      if (this.searchQuery.trim()) {
        // Chuyển đến trang Browse với query parameter
        this.$router.push({
          path: '/browse',
          query: { search: this.searchQuery.trim() }
        });
        
        // Clear search input sau khi search
        this.searchQuery = '';
      }
    }
  }
};
</script>

<style scoped>
.limitbox {
  display: flex;
  height: 48px;
  background-color: var(--Color-Background-Tertiary);
  align-items: center;
  justify-content: center;
  box-shadow: var(--ShadowLight);
  position: relative;
  z-index: 100;
}

nav {
  width: 940px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

a {
  text-decoration: none;
  color: var(--Color-Text-Main);
  font: var(--Text-BodyLarge);
}

.item {
  padding: 6px 24px;
  margin: 0px 6px;
  position: relative;
  transition: all 0.2s ease;
}

.item--selected,
.item:hover {
  border-radius: 3px;
  background-color: var(--Color-Secondary);
}

.cart-item {
  display: flex;
  align-items: center;
  gap: 4px;
  position: relative;
}

.cart-badge {
  background-color: var(--Color-Primary);
  color: white;
  border-radius: 50%;
  min-width: 18px;
  height: 18px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 600;
  line-height: 1;
}

.searchbar {
  width: 300px;
  background-color: var(--Color-Background-Main20);
  border: none;
  height: 40px;
  color: var(--Color-Text-Main);
  border-radius: 3px;
  padding: 0 12px;
}

.searchbar:focus {
  outline: none;
  box-shadow: 0 0 0 2px var(--Color-Primary);
}

.searchbar::placeholder {
  color: var(--Color-Text-Dim);
}
</style>
