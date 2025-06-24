<template>
  <Nav />
  <div class="view">
    <div class="view-wrapper">
      <div class="checkout-container">
        <h1 class="checkout-title">Thanh toán</h1>
        
        <div class="checkout-content">
          <!-- Order Summary -->
          <div class="order-summary">
            <h2>Tóm tắt đơn hàng</h2>
            <div class="order-items">
              <div v-for="item in cartItems" :key="item.id" class="order-item">
                <img :src="getGameImage(item)" :alt="item.title" class="item-image" />
                <div class="item-info">
                  <h3>{{ item.title }}</h3>
                  <div class="item-tags">
                    <span v-for="tag in getGameTags(item)" :key="tag.id" class="tag">
                      {{ tag.title }}
                    </span>
                  </div>
                </div>
                <div class="item-price">{{ formatPrice(item.price) }}</div>
              </div>
            </div>
            
            <div class="order-total">
              <div class="total-row">
                <span>Subtotal ({{ cartItems.length }} items)</span>
                <span>{{ formatPrice(subtotal) }}</span>
              </div>
              <div class="total-row">
                <span>Tax</span>
                <span>{{ formatPrice(tax) }}</span>
              </div>
              <div class="total-row total-final">
                <span>Total</span>
                <span>{{ formatPrice(total) }}</span>
              </div>
            </div>
          </div>

          <!-- Payment Form -->
          <div class="payment-form">
            <h2>Phương thức thanh toán</h2>
            
            <!-- Payment Method Selection -->
            <div class="payment-methods">
              <div 
                v-for="method in paymentMethods" 
                :key="method.id"
                class="payment-method"
                :class="{ active: selectedPaymentMethod === method.id }"
                @click="selectedPaymentMethod = method.id"
              >
                <div class="method-icon">
                  <img :src="method.image" :alt="method.name" class="payment-method-img" />
                </div>
                <div class="method-info">
                  <h3>{{ method.name }}</h3>
                  <p>{{ method.description }}</p>
                </div>
                <div class="method-radio">
                  <input 
                    type="radio" 
                    :value="method.id" 
                    v-model="selectedPaymentMethod"
                    :id="`payment-${method.id}`"
                  />
                </div>
              </div>
            </div>

            <!-- Payment Details Form -->
            <div class="payment-details" v-if="selectedPaymentMethod">
              <div v-if="selectedPaymentMethod === 'visa' || selectedPaymentMethod === 'mastercard'" class="card-form">
                <h3>Thông tin thẻ</h3>
                <div class="form-group">
                  <label for="cardNumber">Số thẻ</label>
                  <input 
                    type="text" 
                    id="cardNumber" 
                    v-model="cardDetails.number"
                    placeholder="1234 5678 9012 3456"
                    maxlength="19"
                    @input="formatCardNumber"
                  />
                </div>
                <div class="form-row">
                  <div class="form-group">
                    <label for="expiryDate">Ngày hết hạn</label>
                    <input 
                      type="text" 
                      id="expiryDate" 
                      v-model="cardDetails.expiry"
                      placeholder="MM/YY"
                      maxlength="5"
                      @input="formatExpiry"
                    />
                  </div>
                  <div class="form-group">
                    <label for="cvv">CVV</label>
                    <input 
                      type="text" 
                      id="cvv" 
                      v-model="cardDetails.cvv"
                      placeholder="123"
                      maxlength="4"
                    />
                  </div>
                </div>
                <div class="form-group">
                  <label for="cardName">Tên trên thẻ</label>
                  <input 
                    type="text" 
                    id="cardName" 
                    v-model="cardDetails.name"
                    placeholder="NGUYEN VAN A"
                  />
                </div>
              </div>

              <div v-else-if="selectedPaymentMethod === 'paypal'" class="paypal-form">
                <h3>PayPal</h3>
                <p>Bạn sẽ được chuyển hướng đến PayPal để hoàn tất thanh toán.</p>
                <div class="paypal-info">
                  <img :src="paypalImg" alt="PayPal" class="paypal-logo" />
                  <span>Thanh toán an toàn với PayPal</span>
                </div>
              </div>
            </div>

            <!-- Action Buttons -->
            <div class="checkout-actions">
              <button @click="goBackToCart" class="back-btn">
                <i class="fas fa-arrow-left"></i>
                Quay lại giỏ hàng
              </button>
              <button 
                @click="processPayment" 
                class="pay-btn"
                :disabled="!isFormValid || processing"
              >
                <i class="fas fa-credit-card" v-if="!processing"></i>
                <i class="fas fa-spinner fa-spin" v-else></i>
                {{ processing ? 'Đang xử lý...' : `Thanh toán ${formatPrice(total)}` }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Nav from './Nav.vue';
import { getCart, clearCart } from '../helpers/cartHelper.js';
import visaImg from '../assets/images/visa.png';
import mastercardImg from '../assets/images/mastercard.jpg';
import paypalImg from '../assets/images/Paypal.png';

export default {
  components: {
    Nav
  },
  data() {
    return {
      cartItems: [],
      loading: true,
      processing: false,
      selectedPaymentMethod: '',
      cardDetails: {
        number: '',
        expiry: '',
        cvv: '',
        name: ''
      },
      paypalImg,
      paymentMethods: [
        {
          id: 'visa',
          name: 'Visa',
          description: 'Thanh toán bằng thẻ Visa',
          image: visaImg
        },
        {
          id: 'mastercard',
          name: 'Mastercard',
          description: 'Thanh toán bằng thẻ Mastercard',
          image: mastercardImg
        },
        {
          id: 'paypal',
          name: 'PayPal',
          description: 'Thanh toán qua PayPal',
          image: paypalImg
        }
      ]
    };
  },
  computed: {
    subtotal() {
      return this.cartItems.reduce((total, item) => total + item.price, 0);
    },
    tax() {
      return this.subtotal * 0.1; // 10% thuế
    },
    total() {
      return this.subtotal + this.tax;
    },
    isFormValid() {
      if (!this.selectedPaymentMethod) return false;
      
      if (this.selectedPaymentMethod === 'paypal') return true;
      
      return this.cardDetails.number.length >= 16 &&
             this.cardDetails.expiry.length === 5 &&
             this.cardDetails.cvv.length >= 3 &&
             this.cardDetails.name.length > 0;
    }
  },
  async mounted() {
    await this.loadCartItems();
    if (this.cartItems.length === 0) {
      this.$router.push('/cart');
    }
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

        const gamePromises = cartGameIds.map(async gameId => {
          try {
            const res = await fetch(`http://localhost:5174/api/games/${gameId}`);
            if (res.ok) {
              const game = await res.json();
              
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
        this.cartItems = games.filter(game => game !== null);
      } catch (error) {
        console.error('Error loading cart items:', error);
        this.cartItems = [];
      } finally {
        this.loading = false;
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

    formatCardNumber() {
      let value = this.cardDetails.number.replace(/\s/g, '').replace(/[^0-9]/gi, '');
      let formattedValue = value.match(/.{1,4}/g)?.join(' ') || value;
      this.cardDetails.number = formattedValue;
    },

    formatExpiry() {
      let value = this.cardDetails.expiry.replace(/\D/g, '');
      if (value.length >= 2) {
        value = value.substring(0, 2) + '/' + value.substring(2, 4);
      }
      this.cardDetails.expiry = value;
    },

    goBackToCart() {
      this.$router.push('/cart');
    },

    async processPayment() {
      if (!this.isFormValid) return;
      
      this.processing = true;
      
      try {
        const token = localStorage.getItem('token');
        if (!token) {
          alert('Vui lòng đăng nhập để tiếp tục');
          this.$router.push('/login');
          return;
        }

        let userId = localStorage.getItem('userId');
        if (!userId) {
          const res = await fetch('http://localhost:5174/api/auth/me', {
            headers: { Authorization: `Bearer ${token}` }
          });
          if (res.ok) {
            const data = await res.json();
            userId = data.Id;
            localStorage.setItem('userId', userId);
          } else {
            throw new Error('Không thể xác thực người dùng');
          }
        }

        // Simulate payment processing delay
        await new Promise(resolve => setTimeout(resolve, 2000));

        // Create transaction
        const paymentMethodName = this.paymentMethods.find(m => m.id === this.selectedPaymentMethod)?.name || 'Unknown';
        const transactionData = {
          paymentMethod: paymentMethodName,
          totalPrice: this.total,
          createdAt: new Date().toISOString(),
          accountId: parseInt(userId)
        };

        const transactionResponse = await fetch(`http://localhost:5174/api/transactions/${userId}`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          },
          body: JSON.stringify(transactionData)
        });

        if (!transactionResponse.ok) {
          throw new Error('Không thể tạo giao dịch');
        }

        const transaction = await transactionResponse.json();

        // Create transaction items and add games to owned
        for (const item of this.cartItems) {
          await fetch('http://localhost:5174/api/transactionitems', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
              gameId: item.id,
              transactionId: transaction.id
            })
          });

          await fetch(`http://localhost:5174/api/accounts/${userId}/owned-games/${item.id}`, {
            method: 'POST',
            headers: {
              'Authorization': `Bearer ${token}`
            }
          });
        }

        // Clear cart
        clearCart();

        // Store transaction info for success page
        sessionStorage.setItem('lastTransaction', JSON.stringify({
          id: transaction.id,
          items: this.cartItems,
          total: this.total,
          paymentMethod: paymentMethodName,
          date: new Date().toISOString()
        }));

        // Emit event for other components
        window.dispatchEvent(new Event('game-purchased'));

        // Redirect to success page
        this.$router.push('/checkout/success');
        
      } catch (error) {
        console.error('Error during payment:', error);
        alert('Có lỗi xảy ra khi xử lý thanh toán. Vui lòng thử lại.');
      } finally {
        this.processing = false;
      }
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
  width: 1200px;
  max-width: 100%;
  padding: 0 16px;
  min-width: 0;
  overflow-x: hidden;
}

.checkout-container {
  background: var(--Color-Background-Main50);
  border-radius: 8px;
  margin-top: var(--Gap-Large);
  min-height: 80vh;
  box-shadow: var(--ShadowHeavy);
}

.checkout-title {
  font: var(--Text-HeadingLarge);
  color: var(--Color-Text-Main);
  padding: var(--Gap-Large) 32px 0;
  margin: 0;
  border-bottom: 1px solid var(--Color-Background-Main20);
}

.checkout-content {
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(0, 1fr);
  gap: 40px;
  padding: 32px;
  overflow: hidden;
}

/* Order Summary */
.order-summary {
  background: var(--Color-Background-Main);
  border-radius: 8px;
  padding: 24px;
  height: fit-content;
  position: sticky;
  top: 20px;
  min-width: 0;
  overflow: hidden;
}

.order-summary h2 {
  font: var(--Text-HeadingMedium);
  color: var(--Color-Text-Main);
  margin: 0 0 20px 0;
}

.order-items {
  margin-bottom: 24px;
}

.order-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 0;
  border-bottom: 1px solid var(--Color-Background-Main20);
}

.order-item:last-child {
  border-bottom: none;
}

.item-image {
  width: 80px;
  height: 45px;
  border-radius: 4px;
  object-fit: cover;
}

.item-info {
  flex: 1;
}

.item-info h3 {
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Main);
  margin: 0 0 4px 0;
}

.item-tags {
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

.item-price {
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Main);
  font-weight: 600;
}

.order-total {
  border-top: 1px solid var(--Color-Background-Main20);
  padding-top: 16px;
}

.total-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Dim);
}

.total-final {
  font-weight: 600;
  color: var(--Color-Text-Main);
  font-size: 18px;
  margin-top: 8px;
  padding-top: 8px;
  border-top: 1px solid var(--Color-Background-Main20);
}

/* Payment Form */
.payment-form {
  min-width: 0;
  overflow: hidden;
}

.payment-form h2 {
  font: var(--Text-HeadingMedium);
  color: var(--Color-Text-Main);
  margin: 0 0 24px 0;
}

.payment-methods {
  margin-bottom: 32px;
}

.payment-method {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 16px;
  border: 2px solid var(--Color-Background-Main20);
  border-radius: 8px;
  margin-bottom: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.payment-method:hover {
  border-color: var(--Color-Primary);
  background: var(--Color-Background-Main);
}

.payment-method.active {
  border-color: var(--Color-Primary);
  background: var(--Color-Background-Main);
  box-shadow: 0 0 0 1px var(--Color-Primary);
}

.method-icon {
  width: 70px;
  height: 45px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border-radius: 8px;
  padding: 10px;
  border: 1px solid #e5e7eb;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
}

.payment-method:hover .method-icon {
  border-color: var(--Color-Primary);
  box-shadow: 0 4px 8px rgba(102, 126, 234, 0.2);
}

.payment-method.active .method-icon {
  border-color: var(--Color-Primary);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.payment-method-img {
  max-width: 100%;
  max-height: 100%;
  object-fit: contain;
  filter: brightness(1);
  transition: filter 0.3s ease;
}

.payment-method:hover .payment-method-img,
.payment-method.active .payment-method-img {
  filter: brightness(1.1) saturate(1.1);
}

.method-info {
  flex: 1;
}

.method-info h3 {
  font: var(--Text-BodyLarge);
  color: var(--Color-Text-Main);
  margin: 0 0 4px 0;
}

.method-info p {
  font: var(--Text-BodySmall);
  color: var(--Color-Text-Dim);
  margin: 0;
}

.method-radio input {
  width: 20px;
  height: 20px;
  accent-color: var(--Color-Primary);
}

/* Payment Details */
.payment-details {
  background: var(--Color-Background-Main);
  border-radius: 8px;
  padding: 24px;
  margin-bottom: 32px;
  min-width: 0;
  overflow: hidden;
}

.payment-details h3 {
  font: var(--Text-HeadingSmall);
  color: var(--Color-Text-Main);
  margin: 0 0 20px 0;
}

.form-group {
  margin-bottom: 16px;
  min-width: 0;
}

.card-form {
  max-width: 100%;
  overflow: hidden;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.form-group label {
  display: block;
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Main);
  margin-bottom: 6px;
}

.form-group input {
  width: 100%;
  min-width: 0;
  padding: 12px;
  border: 1px solid var(--Color-Background-Main20);
  border-radius: 4px;
  background: var(--Color-Background-Main50);
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  box-sizing: border-box;
}

.form-group input:focus {
  outline: none;
  border-color: var(--Color-Primary);
  box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.1);
}

.paypal-form p {
  color: var(--Color-Text-Dim);
  margin-bottom: 16px;
}

.paypal-info {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px;
  background: var(--Color-Background-Main50);
  border-radius: 6px;
  color: var(--Color-Text-Main);
}

.paypal-logo {
  height: 24px;
  width: auto;
  object-fit: contain;
}

/* Action Buttons */
.checkout-actions {
  display: flex;
  gap: 16px;
  justify-content: space-between;
}

.back-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 24px;
  background: var(--Color-Background-Main);
  border: 1px solid var(--Color-Background-Main20);
  border-radius: 4px;
  color: var(--Color-Text-Main);
  cursor: pointer;
  font: var(--Text-BodyMedium);
  transition: all 0.3s ease;
}

.back-btn:hover {
  background: var(--Color-Background-Main20);
}

.pay-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 32px;
  background: var(--Color-Primary);
  border: none;
  border-radius: 4px;
  color: white;
  cursor: pointer;
  font: var(--Text-BodyMedium);
  font-weight: 600;
  transition: all 0.3s ease;
  flex: 1;
  justify-content: center;
}

.pay-btn:hover:not(:disabled) {
  background: var(--Color-Secondary);
}

.pay-btn:disabled {
  background: var(--Color-Background-Main20);
  color: var(--Color-Text-Dim);
  cursor: not-allowed;
}

@media (max-width: 1024px) {
  .checkout-content {
    grid-template-columns: 1fr;
    gap: 24px;
    padding: 24px;
  }
  
  .order-summary {
    position: static;
  }
}

@media (max-width: 768px) {
  .checkout-content {
    grid-template-columns: 1fr;
    gap: 20px;
    padding: 16px;
  }
  
  .form-row {
    grid-template-columns: 1fr;
    gap: 12px;
  }
  
  .checkout-actions {
    flex-direction: column;
    gap: 12px;
  }
  
  .order-summary {
    position: static;
    margin-bottom: 20px;
  }
  
  .payment-details {
    padding: 20px;
  }
  
  .method-icon {
    width: 60px;
    height: 38px;
    padding: 8px;
  }
  
  .payment-method {
    padding: 12px;
  }
}
</style>