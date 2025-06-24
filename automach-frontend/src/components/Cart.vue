<template>
  <Nav />
  <div class="view">
    <div class="view-wrapper">
      <div class="cart-container">
        <h1 class="cart-title">Cart ({{ cartItems.length }} {{ itemCountText }})</h1>
        
        <div v-if="loading" class="loading-message">
          Đang tải...
        </div>
        
        <div v-else-if="cartItems.length === 0" class="empty-cart">
          <p>Giỏ hàng của bạn đang trống</p>
          <router-link to="/browse" class="browse-link">Khám phá game</router-link>
        </div>
        
        <div v-else class="cart-content">
          <div class="cart-info">
            <p class="cart-subtitle">Bạn có {{ cartItems.length }} game trong giỏ hàng</p>
          </div>
          <div class="cart-items">
            <div v-for="item in cartItems" :key="item.id" class="cart-item-card">
              <div class="cart-item">
                <div class="game-image">
                  <img 
                    :src="getGameImage(item)" 
                    :alt="item.title"
                    @error="handleImageError"
                    @load="handleImageLoad"
                    class="game-img"
                  />
                  <div v-if="!item.imageLoaded" class="image-placeholder">
                    <span>Loading...</span>
                  </div>
                </div>
                <div class="game-info">
                  <h3 class="game-title">{{ item.title }}</h3>
                  <div class="game-tags">
                    <span v-for="tag in getGameTags(item)" :key="tag.id" class="tag">
                      {{ tag.title }}
                    </span>
                  </div>
                  <div class="game-status" v-if="isGameInLibrary(item.id)">
                    <span class="status-badge">Đã sở hữu</span>
                  </div>
                </div>
                <div class="game-actions">
                  <div class="game-price">{{ formatPrice(item.price) }}</div>
                  <div class="action-buttons">
                    <button 
                      v-if="isGameInLibrary(item.id)"
                      class="library-btn"
                      disabled
                    >
                      In Library
                    </button>
                    <button @click="removeFromCart(item.id)" class="remove-btn">
                      🗑️ Remove
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
          
          <div class="cart-summary">
            <div class="summary-row">
              <span class="summary-label">Items in cart</span>
              <span class="summary-value">{{ cartItems.length }}</span>
            </div>
            <div class="summary-row">
              <span class="summary-label">Estimated total</span>
              <span class="summary-value">{{ formatPrice(totalPrice) }}</span>
            </div>
            <button @click="goToCheckout" class="purchase-btn" :disabled="cartItems.length === 0">
              <i class="fas fa-credit-card"></i>
              Thanh toán
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Nav from './Nav.vue';
import { getCart, removeFromCart, clearCart } from '../helpers/cartHelper.js';

export default {
  components: {
    Nav
  },
  data() {
    return {
      cartItems: [],
      ownedGames: [],
      loading: true
    };
  },
  computed: {
    totalPrice() {
      return this.cartItems.reduce((total, item) => total + item.price, 0);
    },
    itemCountText() {
      return this.cartItems.length === 1 ? 'item' : 'items';
    }
  },
  async mounted() {
    await Promise.all([
      this.loadCartItems(),
      this.loadOwnedGames()
    ]);
  },
  methods: {
    async loadCartItems() {
      this.loading = true;
      try {
        const cartGameIds = getCart();
        if (cartGameIds.length === 0) {
          this.cartItems = [];
          this.loading = false;
          return;
        }

        // Fetch chi tiết của từng game trong cart
        const gamePromises = cartGameIds.map(async gameId => {
          try {
            const res = await fetch(`http://localhost:5174/api/games/${gameId}`);
            if (res.ok) {
              const game = await res.json();
              
              // Fetch ảnh riêng như GameLibrary nếu chưa có
              if (!game.imageUrls || game.imageUrls.length === 0) {
                try {
                  const imgRes = await fetch(`http://localhost:5174/api/games/${game.id}/images`);
                  if (imgRes.ok) {
                    const imgs = await imgRes.json();
                    game.imageUrl = imgs[0]?.url || 'https://via.placeholder.com/460x215?text=Game';
                  } else {
                    game.imageUrl = 'https://via.placeholder.com/460x215?text=Game';
                  }
                } catch (error) {
                  console.error(`Error fetching images for game ${game.id}:`, error);
                  game.imageUrl = 'https://via.placeholder.com/460x215?text=Game';
                }
              }
              
              return game;
            }
            return null;
          } catch (error) {
            console.error(`Error fetching game ${gameId}:`, error);
            return null;
          }
        });

        const games = await Promise.all(gamePromises);
        this.cartItems = games.filter(game => game !== null).map(game => ({
          ...game,
          imageLoaded: false
        }));
      } catch (error) {
        console.error('Error loading cart items:', error);
        this.cartItems = [];
      } finally {
        this.loading = false;
      }
    },

    async loadOwnedGames() {
      try {
        const token = localStorage.getItem('token');
        const userId = localStorage.getItem('userId');
        
        if (!token || !userId) {
          this.ownedGames = [];
          return;
        }

        const res = await fetch(`http://localhost:5174/api/accounts/${userId}/owned-games`, {
          headers: { Authorization: `Bearer ${token}` }
        });

        if (res.ok) {
          this.ownedGames = await res.json();
        } else {
          this.ownedGames = [];
        }
      } catch (error) {
        console.error('Error loading owned games:', error);
        this.ownedGames = [];
      }
    },

    isGameInLibrary(gameId) {
      return this.ownedGames.some(game => game.id === gameId);
    },

    removeFromCart(gameId) {
      removeFromCart(gameId);
      this.cartItems = this.cartItems.filter(item => item.id !== gameId);
    },

    getGameImage(game) {
      // Sử dụng imageUrl đã fetch riêng như GameLibrary
      if (game.imageUrl) {
        return game.imageUrl;
      }
      
      // Fallback: lấy ảnh đầu tiên từ imageUrls nếu có
      if (game.imageUrls && game.imageUrls.length > 0 && game.imageUrls[0].url) {
        return game.imageUrls[0].url;
      }
      
      // Fallback placeholder với tỷ lệ banner
      return `https://via.placeholder.com/460x215/2c3e50/ffffff?text=${encodeURIComponent(game.title || 'Game')}`;
    },

    handleImageError(event) {
      // Fallback khi ảnh load lỗi
      const game = this.cartItems.find(item => 
        event.target.alt === item.title
      );
      if (game) {
        event.target.src = `https://via.placeholder.com/460x215/2c3e50/ffffff?text=${encodeURIComponent(game.title)}`;
      }
    },

    handleImageLoad(event) {
      // Mark image as loaded
      const game = this.cartItems.find(item => 
        event.target.alt === item.title
      );
      if (game) {
        this.$set(game, 'imageLoaded', true);
      }
    },

    getGameTags(game) {
      if (game.gameTags && game.gameTags.length > 0) {
        return game.gameTags.map(gt => gt.tag).slice(0, 4); // Hiển thị tối đa 4 tags
      }
      return [];
    },

    formatPrice(price) {
      return `${price.toFixed(2)}$`;
    },

    goToCheckout() {
      if (this.cartItems.length === 0) return;
      
      const token = localStorage.getItem('token');
      if (!token) {
        alert('Vui lòng đăng nhập để tiếp tục thanh toán');
        this.$router.push('/login');
        return;
      }
      
      this.$router.push('/checkout');
    }
  }
};
</script>

<style scoped>
.view {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  background: var(--Gradient-Background-Main);
}

.view-wrapper {
  width: 940px;
  max-width: 100%;
  padding: 0 16px;
}

.cart-container {
  background: var(--Color-Background-Main50);
  border-radius: 8px;
  margin-top: var(--Gap-Large);
  min-height: 80vh;
  box-shadow: var(--ShadowHeavy);
}

.cart-title {
  font: var(--Text-HeadingLarge);
  color: var(--Color-Text-Main);
  padding: var(--Gap-Large) 32px 0;
  margin: 0;
}

.loading-message,
.empty-cart {
  padding: 80px 32px;
  text-align: center;
  color: var(--Color-Text-Main);
  font: var(--Text-BodyLarge);
}

.browse-link {
  display: inline-block;
  margin-top: 16px;
  padding: 12px 24px;
  background: var(--Color-Primary);
  color: var(--Color-Text-Main);
  text-decoration: none;
  border-radius: 3px;
  transition: background-color 0.3s;
}

.browse-link:hover {
  background: var(--Color-Secondary);
}

.cart-content {
  padding: var(--Gap-Large) 32px 32px;
}

.cart-info {
  margin-bottom: var(--Gap-Large);
}

.cart-subtitle {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
  margin: 0;
}

.cart-items {
  margin-bottom: 180px; /* Tạo space cho fixed summary */
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Large);
}

.cart-item-card {
  background: var(--Color-Background-Main);
  border-radius: 8px;
  box-shadow: var(--ShadowLight);
  border: 1px solid var(--Color-Background-Main20);
  overflow: hidden;
  transition: box-shadow 0.3s ease;
}

.cart-item-card:hover {
  box-shadow: var(--ShadowMedium);
}

.cart-item {
  display: flex;
  align-items: center;
  padding: var(--Gap-Large);
  gap: var(--Gap-Large);
}

.game-image {
  width: 184px;
  height: 86px;
  border-radius: 6px;
  overflow: hidden;
  background: var(--Color-Background-Main20);
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.game-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: opacity 0.3s ease;
}

.image-placeholder {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: var(--Color-Background-Main20);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
}

.game-img[src*="placeholder"] {
  object-fit: contain;
  padding: 8px;
}

.game-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Small);
}

.game-title {
  font: var(--Text-HeadingMedium);
  color: var(--Color-Text-Main);
  margin: 0;
}

.game-tags {
  display: flex;
  gap: var(--Gap-Small);
  flex-wrap: wrap;
}

.game-status {
  margin-top: var(--Gap-Small);
}

.status-badge {
  background: var(--Color-Secondary);
  color: var(--Color-Text-Main);
  padding: 4px 8px;
  border-radius: 3px;
  font: var(--Text-BodySmall);
  font-weight: 600;
}

.game-actions {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: var(--Gap-Medium);
  min-width: 120px;
}

.game-price {
  font: var(--Text-HeadingMedium);
  color: var(--Color-Text-Main);
  font-weight: 700;
}

.action-buttons {
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Small);
  width: 100%;
}

.library-btn {
  background: var(--Color-Background-System);
  color: var(--Color-Text-Dim);
  border: none;
  padding: 8px 16px;
  border-radius: 3px;
  font: var(--Text-BodyMedium);
  cursor: not-allowed;
  text-align: center;
}

.remove-btn {
  background: transparent;
  color: var(--Color-Text-Dim);
  border: 1px solid var(--Color-Background-Main20);
  padding: 8px 16px;
  border-radius: 3px;
  font: var(--Text-BodyMedium);
  cursor: pointer;
  transition: all 0.2s;
  text-align: center;
}

.remove-btn:hover {
  background: var(--Color-Accent-Red);
  color: white;
  border-color: var(--Color-Accent-Red);
}

.cart-summary {
  background: var(--Color-Background-Highlight);
  border-radius: 3px;
  padding: var(--Gap-Large);
  position: fixed;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 940px;
  max-width: calc(100% - 32px);
  border: 1px solid var(--Color-Background-Main20);
  border-bottom: none;
  border-radius: 3px 3px 0 0;
  box-shadow: var(--ShadowHeavy);
  z-index: 1000;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--Gap-Large);
}

.summary-row:first-child {
  margin-bottom: var(--Gap-Medium);
  padding-bottom: var(--Gap-Medium);
  border-bottom: 1px solid var(--Color-Background-Main20);
}

.summary-label {
  font: var(--Text-BodyLarge);
  color: var(--Color-Text-Main);
}

.summary-value {
  font: var(--Text-HeadingMedium);
  color: var(--Color-Text-Main);
}

.purchase-btn {
  width: 100%;
  height: 48px;
  background: var(--Gradient-Button-Action);
  color: var(--Color-Text-Main);
  border: none;
  border-radius: 3px;
  font: var(--Text-BodyLarge);
  font-weight: 700;
  cursor: pointer;
  transition: all 0.3s;
}

.purchase-btn:hover:not(:disabled) {
  background: var(--Color-Primary);
}

.purchase-btn:disabled {
  background: var(--Color-Background-Main20);
  color: var(--Color-Text-Dim);
  cursor: not-allowed;
}
</style> 