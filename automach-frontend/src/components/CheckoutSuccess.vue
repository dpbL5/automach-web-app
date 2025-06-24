<template>
  <Nav />
  <div class="view">
    <div class="view-wrapper">
      <div class="success-container">
        <!-- Success Header -->
        <div class="success-header">
          <div class="success-icon">
            <i class="fas fa-check-circle"></i>
          </div>
          <h1 class="success-title">Thanh toán thành công!</h1>
          <p class="success-subtitle">
            Cảm ơn bạn đã mua hàng. Games đã được thêm vào thư viện của bạn.
          </p>
        </div>

        <!-- Transaction Details -->
        <div class="transaction-details" v-if="transaction">
          <h2>Chi tiết giao dịch</h2>
          
          <div class="transaction-info">
            <div class="info-row">
              <span class="info-label">Mã giao dịch:</span>
              <span class="info-value">#{{ transaction.id }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Ngày thanh toán:</span>
              <span class="info-value">{{ formatDate(transaction.date) }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Phương thức thanh toán:</span>
              <span class="info-value">{{ transaction.paymentMethod }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Tổng tiền:</span>
              <span class="info-value total-amount">{{ formatPrice(transaction.total) }}</span>
            </div>
          </div>

          <!-- Purchased Games -->
          <div class="purchased-games">
            <h3>Games đã mua ({{ transaction.items.length }})</h3>
            <div class="games-list">
              <div v-for="game in transaction.items" :key="game.id" class="game-item">
                <img :src="getGameImage(game)" :alt="game.title" class="game-image" />
                <div class="game-info">
                  <h4>{{ game.title }}</h4>
                  <div class="game-tags">
                    <span v-for="tag in getGameTags(game)" :key="tag.id" class="tag">
                      {{ tag.title }}
                    </span>
                  </div>
                </div>
                <div class="game-price">{{ formatPrice(game.price) }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="success-actions">
          <button @click="goToLibrary" class="primary-btn">
            <i class="fas fa-gamepad"></i>
            Đi đến Thư viện
          </button>
          <button @click="goToHome" class="secondary-btn">
            <i class="fas fa-home"></i>
            Trang chủ
          </button>
          <button @click="continueShopping" class="secondary-btn">
            <i class="fas fa-shopping-bag"></i>
            Tiếp tục mua sắm
          </button>
        </div>

      </div>
    </div>
  </div>
</template>

<script>
import Nav from './Nav.vue';

export default {
  components: {
    Nav
  },
  data() {
    return {
      transaction: null
    };
  },
  mounted() {
    this.loadTransactionData();
  },
  methods: {
    loadTransactionData() {
      const transactionData = sessionStorage.getItem('lastTransaction');
      if (transactionData) {
        this.transaction = JSON.parse(transactionData);
        // Clear the transaction data after loading
        sessionStorage.removeItem('lastTransaction');
      } else {
        // If no transaction data, redirect to home
        this.$router.push('/');
      }
    },

    getGameImage(game) {
      if (game.imageUrl) {
        return game.imageUrl;
      }
      if (game.imageUrls && game.imageUrls.length > 0 && game.imageUrls[0].url) {
        return game.imageUrls[0].url;
      }
      return `https://via.placeholder.com/460x215/2c3e50/ffffff?text=${encodeURIComponent(game.title || 'Game')}`;
    },

    getGameTags(game) {
      if (game.gameTags && game.gameTags.length > 0) {
        return game.gameTags.map(gt => gt.tag).slice(0, 3);
      }
      return [];
    },

    formatPrice(price) {
      return `${price.toFixed(2)}$`;
    },

    formatDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleString('vi-VN', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    },

    goToLibrary() {
      this.$router.push('/library');
    },

    goToHome() {
      this.$router.push('/');
    },

    continueShopping() {
      this.$router.push('/browse');
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
  width: 1000px;
  max-width: 100%;
  padding: 0 16px;
}

.success-container {
  background: var(--Color-Background-Main50);
  border-radius: 8px;
  margin-top: var(--Gap-Large);
  min-height: 80vh;
  box-shadow: var(--ShadowHeavy);
  padding: 40px;
}

/* Success Header */
.success-header {
  text-align: center;
  margin-bottom: 40px;
  padding-bottom: 32px;
  border-bottom: 1px solid var(--Color-Background-Main20);
}

.success-icon {
  font-size: 80px;
  color: #10b981;
  margin-bottom: 20px;
}

.success-title {
  font: var(--Text-HeadingLarge);
  color: var(--Color-Text-Main);
  margin: 0 0 12px 0;
}

.success-subtitle {
  font: var(--Text-BodyLarge);
  color: var(--Color-Text-Dim);
  margin: 0;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

/* Transaction Details */
.transaction-details {
  margin-bottom: 40px;
}

.transaction-details h2 {
  font: var(--Text-HeadingMedium);
  color: var(--Color-Text-Main);
  margin: 0 0 24px 0;
}

.transaction-info {
  background: var(--Color-Background-Main);
  border-radius: 8px;
  padding: 24px;
  margin-bottom: 32px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 0;
  border-bottom: 1px solid var(--Color-Background-Main20);
}

.info-row:last-child {
  border-bottom: none;
}

.info-label {
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Dim);
}

.info-value {
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Main);
  font-weight: 600;
}

.total-amount {
  font-size: 18px;
  color: var(--Color-Primary);
}

/* Purchased Games */
.purchased-games h3 {
  font: var(--Text-HeadingSmall);
  color: var(--Color-Text-Main);
  margin: 0 0 20px 0;
}

.games-list {
  background: var(--Color-Background-Main);
  border-radius: 8px;
  padding: 20px;
}

.game-item {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 16px 0;
  border-bottom: 1px solid var(--Color-Background-Main20);
}

.game-item:last-child {
  border-bottom: none;
}

.game-image {
  width: 80px;
  height: 45px;
  border-radius: 4px;
  object-fit: cover;
}

.game-info {
  flex: 1;
}

.game-info h4 {
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Main);
  margin: 0 0 6px 0;
}

.game-tags {
  display: flex;
  gap: 6px;
  flex-wrap: wrap;
}

.tag {
  background: var(--Color-Background-Main20);
  color: var(--Color-Text-Dim);
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 11px;
}

.game-price {
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Main);
  font-weight: 600;
}

/* Action Buttons */
.success-actions {
  display: flex;
  gap: 16px;
  justify-content: center;
  margin-bottom: 48px;
  flex-wrap: wrap;
}

.primary-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 14px 28px;
  background: var(--Color-Primary);
  border: none;
  border-radius: 6px;
  color: white;
  cursor: pointer;
  font: var(--Text-BodyMedium);
  font-weight: 600;
  transition: all 0.3s ease;
}

.primary-btn:hover {
  background: var(--Color-Secondary);
  transform: translateY(-1px);
}

.secondary-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 14px 28px;
  background: var(--Color-Background-Main);
  border: 1px solid var(--Color-Background-Main20);
  border-radius: 6px;
  color: var(--Color-Text-Main);
  cursor: pointer;
  font: var(--Text-BodyMedium);
  transition: all 0.3s ease;
}

.secondary-btn:hover {
  background: var(--Color-Background-Main20);
  transform: translateY(-1px);
}

/* Additional Info */
.additional-info {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 24px;
}

.info-card {
  display: flex;
  align-items: flex-start;
  gap: 16px;
  background: var(--Color-Background-Main);
  border-radius: 8px;
  padding: 24px;
  border: 1px solid var(--Color-Background-Main20);
}

.card-icon {
  font-size: 24px;
  color: var(--Color-Primary);
  background: rgba(102, 126, 234, 0.1);
  padding: 12px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 48px;
  height: 48px;
}

.card-content h4 {
  font: var(--Text-BodyLarge);
  color: var(--Color-Text-Main);
  margin: 0 0 8px 0;
}

.card-content p {
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Dim);
  margin: 0;
  line-height: 1.5;
}

@media (max-width: 768px) {
  .success-container {
    padding: 24px;
  }
  
  .success-actions {
    flex-direction: column;
    align-items: center;
  }
  
  .primary-btn,
  .secondary-btn {
    width: 100%;
    justify-content: center;
  }
  
  .additional-info {
    grid-template-columns: 1fr;
  }
  
  .game-item {
    flex-direction: column;
    align-items: flex-start;
    text-align: left;
  }
  
  .game-price {
    align-self: flex-end;
  }
}
</style>