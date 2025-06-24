  <template>
    <Nav />
    <div class="game-page-container">
    <div v-if="loading" class="loading-container">
      <div class="loading-text">Loading game...</div>
    </div>
    
    <div v-else-if="error" class="error-container">
      <div class="error-text">{{ error }}</div>
      <button class="button button--primary" @click="$router.go(-1)">Go Back</button>
    </div>

    <div v-else-if="gameData" class="game-content">
      <!-- Game Title --> 
      <label class="section-label">{{ gameData.title }}</label>
      
      <!-- Game Showcase -->
      <div class="main-content-grid">
        <!-- Left Section: Main Showcase -->
        <div class="showcase-section">
          <div class="main-showcase">
            <img 
              :src="currentImage" 
              :alt="gameData.title + ' screenshot'"
              class="main-showcase-image"
              @error="handleImageError"
            />
          </div>
          
          <!-- Thumbnail Navigation -->
          <div class="thumbnail-grid" v-if="gameImages.length > 1">
            <img 
              v-for="(image, index) in gameImages" 
              :key="index"
              :src="image"
              :alt="`Screenshot ${index + 1}`"
              class="thumbnail"
              :class="{ active: index === currentImageIndex }"
              @click="setCurrentImage(index)"
              @error="handleImageError"
            />
          </div>
        </div>

        <!-- Right Section: Game Info Banner -->
        <div class="game-info-section">
          <div class="game-banner">
            <img 
              :src="bannerImage" 
              :alt="gameData.title + ' banner'"
              class="banner-image"
              @error="handleImageError"
            />
            
            <div class="game-tags" v-if="gameTags.length > 0">
              <span 
                v-for="tag in gameTags" 
                :key="tag.id"
                class="tag"
              >
                {{ tag.title }}
              </span>
            </div>

            <div class="game-meta">
              <div class="release-info">
                <span class="label">Release Date:</span>
                <span class="value">{{ formatDate(gameData.releaseDate) }}</span>
              </div>
              <div class="developer-info">
                <span class="label">Developer:</span>
                <span class="value">{{ gameData.developer }}</span>
              </div>
              <div class="publisher-info">
                <span class="label">Publisher:</span>
                <span class="value">Automach, Inc.</span>
              </div>
              <div class="price-info">
                <span class="label">Price:</span>
                <span class="value price">${{ gameData.price.toFixed(2) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Get the Game -->
      <div class="action-section">
        <h2 class="action-title">Download and Play {{ gameData.title }}</h2>
        <div class="action-buttons">
          <button v-if="userOwnsGame" class="button button--play" @click="playGame">
            <i class="fas fa-play"></i>
            Play Game
          </button>
          <div v-if="userOwnsGame" class="owned-status">
            <button class="button button--owned" disabled>
              <i class="fas fa-check"></i>
              In Library
            </button>
          </div>
          <template v-else>
            <button 
              class="button" 
              :class="isInCart ? 'button--secondary' : 'button--primary'" 
              @click="handleAddToCart"
              :disabled="isInCart"
            >
              <i class="fas fa-shopping-cart" v-if="!isInCart"></i>
              <i class="fas fa-check" v-else></i>
              {{ isInCart ? 'In Cart' : `Add to cart - $${gameData.price.toFixed(2)}` }}
            </button>
          </template>
        </div>
      </div>

      <!-- About This Game -->
      <label class="section-label">About This Game</label>
      <div class="description-section">
        <div class="game-description" v-html="formattedGameDescription"></div>
      </div>

      <!-- User Reviews -->
      <div class="user-reviews-section">
        <div class="reviews-header">
          <h2 class="reviews-title">User reviews</h2>
          <div v-if="gameReviews.length > 0" class="reviews-summary">
            <div class="average-rating-display">
              <span class="average-score">{{ averageRating }}</span>
              <span class="average-label">Average rating</span>
            </div>
            <div class="total-reviews">
              {{ gameReviews.length }} review{{ gameReviews.length !== 1 ? 's' : '' }}
            </div>
          </div>
        </div>

        <!-- Add Review Form (only for users who own the game) -->
        <div v-if="isLoggedIn && userOwnsGame && !userHasReviewed" class="add-review-section">
          <h4>Write a Review</h4>
          <div class="review-form">
            <div class="rating-input">
              <label>Rating (1-100):</label>
              <input 
                type="range" 
                v-model="newReview.rating" 
                min="1" 
                max="100" 
                class="rating-slider"
              />
              <span class="rating-value">{{ newReview.rating }}/100</span>
            </div>
            <div class="review-content">
              <label>Your Review:</label>
              <textarea 
                v-model="newReview.content" 
                placeholder="Share your thoughts about this game..."
                rows="4"
                class="review-textarea"
              ></textarea>
            </div>
            <div class="review-actions">
              <button @click="submitReview" :disabled="submittingReview" class="submit-review-btn">
                {{ submittingReview ? 'Submitting...' : 'Submit Review' }}
              </button>
            </div>
          </div>
        </div>

        <!-- Edit Review Form (for users who own the game and already reviewed) -->
        <div v-else-if="isLoggedIn && userOwnsGame && userHasReviewed && editingReview" class="add-review-section">
          <h4>Edit Your Review</h4>
          <div class="review-form">
            <div class="rating-input">
              <label>Rating (1-100):</label>
              <input 
                type="range" 
                v-model="editReview.rating" 
                min="1" 
                max="100" 
                class="rating-slider"
              />
              <span class="rating-value">{{ editReview.rating }}/100</span>
            </div>
            <div class="review-content">
              <label>Your Review:</label>
              <textarea 
                v-model="editReview.content" 
                placeholder="Share your thoughts about this game..."
                rows="4"
                class="review-textarea"
              ></textarea>
            </div>
            <div class="review-actions">
              <button @click="updateReview" :disabled="submittingReview" class="submit-review-btn">
                {{ submittingReview ? 'Updating...' : 'Update Review' }}
              </button>
              <button @click="cancelEditReview" class="cancel-review-btn">Cancel</button>
            </div>
          </div>
        </div>

        <!-- User hasn't bought the game notice -->
        <div v-else-if="isLoggedIn && !userOwnsGame" class="purchase-required-notice">
          <p>You need to purchase this game before you can write a review.</p>
        </div>

        <!-- User already reviewed and can edit -->
        <div v-else-if="isLoggedIn && userOwnsGame && userHasReviewed && !editingReview" class="user-review-notice">
          <p>You have already reviewed this game.</p>
          <button @click="startEditReview" class="edit-review-btn">Edit Your Review</button>
        </div>

        <!-- Login prompt -->
        <div v-else-if="!isLoggedIn" class="login-prompt">
          <p>Please <router-link to="/login">login</router-link> to write a review.</p>
        </div>

        <!-- Reviews List -->
        <div class="reviews-list">
          <div v-if="gameReviews.length === 0" class="no-reviews">
            <p>No reviews yet. Be the first to review this game!</p>
          </div>
          
          <div v-else>
            <div v-for="review in displayedReviews" :key="review.id" class="review-card">
              <div class="review-card-header">
                <div class="reviewer-avatar">
                  <span class="avatar-text">{{ getReviewerInitials(review.accountId) }}</span>
                </div>
                <div class="reviewer-details">
                  <div class="reviewer-name">{{ getReviewerName(review.accountId) }}</div>
                  <div class="review-date">{{ formatReviewDate(review.createdAt) }}</div>
                </div>
                <div class="review-score">
                  <span class="score-number">{{ review.rating }}</span>
                  <span class="score-label">scores</span>
                </div>
              </div>
              <div class="review-card-content">
                <p>{{ review.content }}</p>
              </div>
              <div v-if="isLoggedIn && review.accountId === currentUserId" class="review-actions-bottom">
                <button @click="startEditReviewInline(review)" class="action-btn edit-btn">Edit</button>
                <button @click="deleteReview(review.id)" class="action-btn delete-btn">Delete</button>
              </div>
            </div>

            <!-- Show More Reviews Button -->
            <div v-if="gameReviews.length > reviewsToShow" class="show-more-reviews">
              <button @click="showMoreReviews" class="show-more-btn">
                Show More Reviews ({{ gameReviews.length - reviewsToShow }} remaining)
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { eventBus } from '../eventBus';
import { gamesCache } from '../helpers/userCache';
import { marked } from 'marked';
import { addToCart, isInCart } from '../helpers/cartHelper.js';
import Nav from './Nav.vue';

export default {
  name: 'GamePage',
  components: {
    Nav
  },
  data() {
    return {
      loading: true,
      error: null,
      gameData: null,
      gameImages: [],
      gameTags: [],
      gameReviews: [],
      reviewers: {},
      currentImageIndex: 0,
      slideInterval: null,
      defaultImage: '/api/placeholder/800/450',
      defaultBanner: '/api/placeholder/350/164',
      newReview: {
        content: '',
        rating: 75
      },
      editReview: {
        content: '',
        rating: 75
      },
      submittingReview: false,
      reviewsToShow: 10,
      userOwnsGame: false,
      editingReview: false
    };
  },
  computed: {
    currentImage() {
      return this.gameImages.length > 0 
        ? this.gameImages[this.currentImageIndex] 
        : this.defaultImage;
    },
    bannerImage() {
      // Lấy hình đầu tiên làm banner hoặc dùng default
      return this.gameImages.length > 0 
        ? this.gameImages[0] 
        : this.defaultBanner;
    },
    formattedGameDescription() {
      if (!this.gameData || !this.gameData.gameInfo) {
        return '<p class="no-description">No description available for this game.</p>';
      }
      return marked(this.gameData.gameInfo);
    },
    isLoggedIn() {
      return !!localStorage.getItem('token');
    },
    currentUserId() {
      return parseInt(localStorage.getItem('userId') || '0');
    },
    userHasReviewed() {
      return this.gameReviews.some(review => review.accountId === this.currentUserId);
    },
    userReview() {
      return this.gameReviews.find(review => review.accountId === this.currentUserId);
    },
    averageRating() {
      if (this.gameReviews.length === 0) return 0;
      const sum = this.gameReviews.reduce((acc, review) => acc + review.rating, 0);
      return Math.round(sum / this.gameReviews.length);
    },
    displayedReviews() {
      return this.gameReviews.slice(0, this.reviewsToShow);
    },
    isInCart() {
      return this.gameData ? isInCart(this.gameData.id) : false;
    }
  },
  async mounted() {
    await this.fetchGameData();
    if (this.gameImages.length > 1) {
      this.startSlideshow();
    }
    
    // Listen for purchase events to refresh game ownership
    window.addEventListener('game-purchased', this.handleGamePurchased);
  },
  beforeUnmount() {
    this.stopSlideshow();
    window.removeEventListener('game-purchased', this.handleGamePurchased);
  },
  methods: {
    async fetchGameData() {
      try {
        this.loading = true;
        const gameId = this.$route.params.id;
        
        console.log('Fetching game with ID:', gameId);
        
        // Fetch game data
        const gameResponse = await fetch(`http://localhost:5174/api/games/${gameId}`);
        console.log('Game response status:', gameResponse.status);
        
        if (!gameResponse.ok) {
          throw new Error('Game not found');
        }
        
        const gameData = await gameResponse.json();
        console.log('Game data received:', gameData);
        
        // Ensure we have valid game data
        if (!gameData || !gameData.id) {
          throw new Error('Invalid game data received');
        }
        
        this.gameData = gameData;

        // Fetch game images, tags, and reviews in parallel
        await Promise.all([
          this.fetchGameImages(gameId),
          this.fetchGameTags(gameId),
          this.fetchGameReviews(gameId),
          this.checkGameOwnership(gameId)
        ]);

      } catch (error) {
        console.error('Error fetching game data:', error);
        this.error = 'Failed to load game data. Please try again.';
      } finally {
        this.loading = false;
      }
    },

    async fetchGameImages(gameId) {
      try {
        const response = await fetch(`http://localhost:5174/api/games/${gameId}/images`);
        console.log('Images response status:', response.status);
        
        if (response.ok) {
          const images = await response.json();
          console.log('Raw images data:', images);
          this.gameImages = images.map(img => img.url);
        }
      } catch (error) {
        console.warn('Could not fetch game images:', error);
        this.gameImages = [];
      }
    },

    async fetchGameTags(gameId) {
      try {
        const response = await fetch(`http://localhost:5174/api/games/${gameId}/tags`);
        console.log('Tags response status:', response.status);
        
        if (response.ok) {
          const tags = await response.json();
          console.log('Raw tags data:', tags);
          this.gameTags = tags;
        }
      } catch (error) {
        console.warn('Could not fetch game tags:', error);
        this.gameTags = [];
      }
    },

    async checkGameOwnership(gameId) {
      if (!this.isLoggedIn) {
        this.userOwnsGame = false;
        return;
      }

      try {
        const userId = this.currentUserId;
        const response = await fetch(`http://localhost:5174/api/accounts/${userId}/owned-games/check/${gameId}`, {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (response.ok) {
          const result = await response.json();
          this.userOwnsGame = result.isOwned;
          console.log('User owns game:', this.userOwnsGame);
        } else {
          this.userOwnsGame = false;
        }
      } catch (error) {
        console.warn('Could not check game ownership:', error);
        this.userOwnsGame = false;
      }
    },

    formatDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', { 
        year: 'numeric', 
        month: 'short', 
        day: 'numeric' 
      });
    },

    handleImageError(event) {
      // Replace broken images with placeholder
      event.target.src = event.target.classList.contains('banner-image') 
        ? this.defaultBanner 
        : this.defaultImage;
    },

    startSlideshow() {
      if (this.gameImages.length <= 1) return;
      
      this.slideInterval = setInterval(() => {
        this.nextImage();
      }, 5000); // 5 seconds
    },

    stopSlideshow() {
      if (this.slideInterval) {
        clearInterval(this.slideInterval);
        this.slideInterval = null;
      }
    },

    nextImage() {
      if (this.gameImages.length <= 1) return;
      this.currentImageIndex = (this.currentImageIndex + 1) % this.gameImages.length;
    },

    setCurrentImage(index) {
      this.currentImageIndex = index;
      // Restart slideshow timer when user manually selects image
      this.stopSlideshow();
      this.startSlideshow();
    },

    playGame() {
      // Placeholder for play functionality
      alert(`Starting ${this.gameData.title}...`);
    },

    handleAddToCart() {
      // Check if user is logged in
      const token = localStorage.getItem('token');
      if (!token) {
        alert('Please login to add games to cart');
        this.$router.push('/login');
        return;
      }
      
      // Check if game is already in cart
      if (this.isInCart) {
        console.log(`${this.gameData.title} is already in cart`);
        return;
      }
      
      const success = addToCart(this.gameData.id);
      if (success) {
        console.log(`Added ${this.gameData.title} to cart`);
        // Force reactivity update
        this.$forceUpdate();
      } else {
        console.log(`${this.gameData.title} is already in cart`);
      }
    },

    async fetchGameReviews(gameId) {
      try {
        const response = await fetch(`http://localhost:5174/api/review/game/${gameId}`);
        console.log('Reviews response status:', response.status);
        
        if (response.ok) {
          const reviews = await response.json();
          console.log('Reviews data received:', reviews);
          this.gameReviews = reviews;
          
          // Fetch reviewer names
          await this.fetchReviewerNames();
        }
      } catch (error) {
        console.warn('Could not fetch game reviews:', error);
        this.gameReviews = [];
      }
    },

    async fetchReviewerNames() {
      try {
        const uniqueAccountIds = [...new Set(this.gameReviews.map(r => r.accountId))];
        
        for (const accountId of uniqueAccountIds) {
          if (!this.reviewers[accountId]) {
            try {
              const response = await fetch(`http://localhost:5174/api/accounts/${accountId}/public`);
              
              if (response.ok) {
                const account = await response.json();
                this.reviewers[accountId] = account.name || account.username || 'Anonymous';
              } else {
                this.reviewers[accountId] = 'Anonymous';
              }
            } catch {
              this.reviewers[accountId] = 'Anonymous';
            }
          }
        }
      } catch (error) {
        console.warn('Could not fetch reviewer names:', error);
      }
    },

    async submitReview() {
      if (!this.newReview.content.trim()) {
        alert('Please write a review before submitting.');
        return;
      }

      if (!this.isLoggedIn) {
        alert('Please login to submit a review.');
        return;
      }

      if (!this.userOwnsGame) {
        alert('You must own this game to write a review.');
        return;
      }

      this.submittingReview = true;

      try {
        const token = localStorage.getItem('token');
        const userId = this.currentUserId;
        const gameId = this.gameData.id;

        const response = await fetch(`http://localhost:5174/api/review/${userId}/${gameId}`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          },
          body: JSON.stringify({
            content: this.newReview.content,
            rating: parseInt(this.newReview.rating)
          })
        });

        if (response.ok) {
          // Refresh reviews
          await this.fetchGameReviews(gameId);
          
          // Reset form
          this.newReview.content = '';
          this.newReview.rating = 75;
          
          alert('Review submitted successfully!');
        } else {
          const errorText = await response.text();
          console.error('Review submission failed:', errorText);
          alert('Failed to submit review. Please try again.');
        }
      } catch (error) {
        console.error('Error submitting review:', error);
        alert('An error occurred while submitting your review.');
      } finally {
        this.submittingReview = false;
      }
    },

    startEditReview() {
      const userReview = this.userReview;
      if (userReview) {
        this.editReview.content = userReview.content;
        this.editReview.rating = userReview.rating;
        this.editingReview = true;
      }
    },

    cancelEditReview() {
      this.editingReview = false;
      this.editReview.content = '';
      this.editReview.rating = 75;
    },

    async updateReview() {
      if (!this.editReview.content.trim()) {
        alert('Please write a review before submitting.');
        return;
      }

      this.submittingReview = true;

      try {
        const token = localStorage.getItem('token');
        const userReview = this.userReview;

        const response = await fetch(`http://localhost:5174/api/review/${userReview.id}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          },
          body: JSON.stringify({
            content: this.editReview.content,
            rating: parseInt(this.editReview.rating)
          })
        });

        if (response.ok) {
          // Refresh reviews
          await this.fetchGameReviews(this.gameData.id);
          
          // Reset edit form
          this.cancelEditReview();
          
          alert('Review updated successfully!');
        } else {
          const errorText = await response.text();
          console.error('Review update failed:', errorText);
          alert('Failed to update review. Please try again.');
        }
      } catch (error) {
        console.error('Error updating review:', error);
        alert('An error occurred while updating your review.');
      } finally {
        this.submittingReview = false;
      }
    },

    getReviewerName(accountId) {
      return this.reviewers[accountId] || 'Anonymous';
    },

    getReviewerInitials(accountId) {
      const name = this.reviewers[accountId] || 'Anonymous';
      return name.split(' ').map(word => word.charAt(0)).join('').substring(0, 2).toUpperCase();
    },

    formatReviewDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', { 
        year: 'numeric', 
        month: 'short', 
        day: 'numeric' 
      });
    },

    startEditReviewInline(review) {
      if (review.accountId === this.currentUserId) {
        this.editReview.content = review.content;
        this.editReview.rating = review.rating;
        this.editingReview = true;
      }
    },

    async deleteReview(reviewId) {
      if (!confirm('Are you sure you want to delete this review?')) {
        return;
      }

      try {
        const token = localStorage.getItem('token');
        const response = await fetch(`http://localhost:5174/api/review/${reviewId}`, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (response.ok) {
          await this.fetchGameReviews(this.gameData.id);
          alert('Review deleted successfully!');
        } else {
          alert('Failed to delete review. Please try again.');
        }
      } catch (error) {
        console.error('Error deleting review:', error);
        alert('An error occurred while deleting your review.');
      }
    },

    showMoreReviews() {
      this.reviewsToShow += 10;
    },

    async handleGamePurchased() {
      // Refresh game ownership status when a purchase is made
      if (this.gameData) {
        await this.checkGameOwnership(this.gameData.id);
        this.$forceUpdate();
      }
    }
  },

  watch: {
    '$route'() {
      // Reload data when route changes
      this.fetchGameData();
    }
  }
};
</script>

<style scoped>
.game-page-container {
  min-height: 100vh;
  background: var(--Gradient-Background-Main);
  color: var(--Color-Text-Main);
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: var(--Gap-Large) var(--Gap-Medium);
}

.loading-container,
.error-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 50vh;
  gap: var(--Gap-Medium);
}

.loading-text,
.error-text {
  font: var(--Text-HeadingMedium);
  color: var(--Color-Text-Main);
}

.game-content {
  width: 1000px;
  max-width: 100%;
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Medium);
}

/* Section Labels */
.section-label {
  font: var(--Text-HeadingMedium);
  color: var(--Color-Text-Main);
  align-self: flex-start;
  width: 100%;
  padding: var(--Gap-Medium) 0 var(--Gap-Small) 0;
  margin: 0;
  font-weight: 600;
}

.game-title {
  font: var(--Text-HeadingLarge);
  color: var(--Color-Text-Main);
  font-weight: 700;
  margin: 0 0 var(--Gap-Medium) 0;
  text-align: center;
}

.main-content-grid {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: var(--Gap-Large);
  background: var(--Color-Background-Highlight);
  padding: var(--Gap-Large);
  border-radius: 8px;
  box-shadow: var(--ShadowLight);
}

/* Left Section - Showcase */
.showcase-section {
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Medium);
}

.main-showcase {
  width: 100%;
  aspect-ratio: 16/9;
  border-radius: 4px;
  overflow: hidden;
  background: var(--Color-Background-Highlight);
}

.main-showcase-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: opacity 0.3s ease;
}

.thumbnail-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  gap: var(--Gap-Small);
}

.thumbnail {
  aspect-ratio: 16/9;
  width: 100%;
  object-fit: cover;
  border-radius: 4px;
  cursor: pointer;
  border: 2px solid transparent;
  transition: all 0.3s ease;
}

.thumbnail:hover {
  border-color: var(--Color-Primary);
  transform: scale(1.02);
}

.thumbnail.active {
  border-color: var(--Color-Primary);
  box-shadow: 0 0 0 1px var(--Color-Primary);
}

/* Right Section - Game Info */
.game-info-section {
  display: flex;
  flex-direction: column;
}

.game-banner {
  background: var(--Color-Background-Main);
  border-radius: 8px;
  padding: var(--Gap-Large);
  border: 1px solid var(--Color-Background-System);
  box-shadow: var(--ShadowLight);
}

.banner-image {
  width: 100%;
  height: 164px;
  object-fit: cover;
  border-radius: 4px;
  margin-bottom: var(--Gap-Medium);
}

.game-tags {
  display: flex;
  flex-wrap: wrap;
  gap: var(--Gap-Small);
  margin-bottom: var(--Gap-Medium);
}

.game-meta {
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Small);
}

.game-meta > div {
  display: flex;
  justify-content: space-between;
}

.label {
  font: var(--Text-BodySmall);
  color: var(--Color-Text-Dim);
  font-weight: 600;
}

.value {
  font: var(--Text-BodySmall);
  color: var(--Color-Text-Main);
}

.value.price {
  color: var(--Color-Accent-Green);
  font-weight: 700;
}

/* Action Section */
.action-section {
  background: var(--Color-Background-Highlight);
  padding: var(--Gap-Large);
  border-radius: 8px;
  box-shadow: var(--ShadowLight);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.action-title {
  font: var(--Text-HeadingMedium);
  color: var(--Color-Text-Main);
  margin: 0;
  flex: 1;
}

.action-buttons {
  display: flex;
  gap: var(--Gap-Medium);
}

.action-buttons .button {
  padding: 12px 24px;
  font: var(--Text-BodyMedium);
  font-weight: 700;
  display: flex;
  align-items: center;
  gap: 8px;
}

.button--owned {
  background-color: #6b7280;
  color: #f9fafb;
  cursor: default;
  display: flex;
  align-items: center;
  gap: 6px;
  font-weight: 500;
  border: 1px solid #4b5563;
  opacity: 0.8;
}

.button--owned:hover {
  background-color: #6b7280;
  color: #f9fafb;
  transform: none;
  cursor: default;
}

.button--owned i {
  font-size: 12px;
}

.button--secondary {
  background-color: var(--Color-Background-System);
  color: var(--Color-Text-Dim);
  cursor: not-allowed;
}

.button--secondary:hover {
  background-color: var(--Color-Background-System);
  transform: none;
}

.owned-status {
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Description Section */
.description-section {
  background: var(--Color-Background-Highlight);
  padding: var(--Gap-Large);
  border-radius: 8px;
  box-shadow: var(--ShadowLight);
}

.game-description {
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Main);
  line-height: 1.6;
}

.game-description p {
  margin: 1em 0;
}

.game-description h1,
.game-description h2,
.game-description h3,
.game-description h4,
.game-description h5,
.game-description h6 {
  color: var(--Color-Primary);
  margin: 1.5em 0 0.5em 0;
  font-weight: 700;
}

.game-description h1 { font-size: 2em; }
.game-description h2 { font-size: 1.5em; }
.game-description h3 { font-size: 1.25em; }
.game-description h4 { font-size: 1.1em; }

.game-description ul,
.game-description ol {
  margin: 1em 0;
  padding-left: 2em;
}

.game-description li {
  margin: 0.5em 0;
}

.game-description code {
  background-color: var(--Color-Background-System);
  color: var(--Color-Accent-Green);
  padding: 0.2em 0.4em;
  border-radius: 3px;
  font-family: 'Courier New', monospace;
  font-size: 0.9em;
}

.game-description pre {
  background-color: var(--Color-Background-System);
  color: var(--Color-Text-Main);
  padding: 1em;
  border-radius: 4px;
  overflow-x: auto;
  margin: 1em 0;
}

.game-description pre code {
  background: none;
  padding: 0;
}

.game-description a {
  color: var(--Color-Primary);
  text-decoration: none;
}

.game-description a:hover {
  text-decoration: underline;
}

.game-description blockquote {
  border-left: 4px solid var(--Color-Primary);
  margin: 1em 0;
  padding: 0.5em 1em;
  background-color: var(--Color-Background-Highlight);
  color: var(--Color-Text-Dim);
  border-radius: 0 4px 4px 0;
}

.game-description strong {
  color: var(--Color-Text-Main);
  font-weight: 700;
}

.game-description em {
  color: var(--Color-Text-Dim);
  font-style: italic;
}

.game-description img {
  max-width: 100%;
  height: auto;
  border-radius: 4px;
  margin: 1em 0;
}

.no-description {
  color: var(--Color-Text-Dim);
  font-style: italic;
  margin: 0;
}

/* User Reviews Section */
.user-reviews-section {
  background: #1a1a1a;
  border-radius: 8px;
  overflow: hidden;
  margin-top: var(--Gap-Large);
}

.reviews-header {
  background: #2a2a2a;
  padding: var(--Gap-Large);
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #3a3a3a;
}

.reviews-title {
  color: #ffffff;
  font-size: 18px;
  font-weight: 600;
  margin: 0;
}

.reviews-summary {
  display: flex;
  align-items: center;
  gap: var(--Gap-Large);
}

.average-rating-display {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.average-score {
  color: #ffffff;
  font-size: 24px;
  font-weight: 700;
  line-height: 1;
}

.average-label {
  color: #999999;
  font-size: 12px;
  margin-top: 4px;
}

.total-reviews {
  color: #cccccc;
  font-size: 14px;
}

/* Add Review Form */
.add-review-section {
  background: #2a2a2a;
  padding: var(--Gap-Large);
  border-radius: 4px;
  margin: var(--Gap-Large);
  border: 1px solid #3a3a3a;
}

.add-review-section h4 {
  color: #ffffff;
  font-size: 16px;
  font-weight: 600;
  margin-bottom: var(--Gap-Medium);
}

.review-form {
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Medium);
}

.rating-input {
  display: flex;
  align-items: center;
  gap: var(--Gap-Medium);
  flex-wrap: wrap;
}

.rating-input label {
  color: #ffffff;
  font-size: 14px;
  font-weight: 600;
  min-width: 120px;
}

.rating-slider {
  flex: 1;
  min-width: 200px;
  height: 6px;
  background: #555555;
  outline: none;
  border-radius: 3px;
  cursor: pointer;
}

.rating-slider::-webkit-slider-thumb {
  appearance: none;
  width: 20px;
  height: 20px;
  background: #4a9eff;
  border-radius: 50%;
  cursor: pointer;
}

.rating-slider::-moz-range-thumb {
  width: 20px;
  height: 20px;
  background: #4a9eff;
  border-radius: 50%;
  cursor: pointer;
  border: none;
}

.rating-value {
  color: #4a9eff;
  font-size: 14px;
  font-weight: 700;
  min-width: 60px;
}

.review-content {
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Small);
}

.review-content label {
  color: #ffffff;
  font-size: 14px;
  font-weight: 600;
}

.review-textarea {
  padding: var(--Gap-Medium);
  background: #3a3a3a;
  border: 1px solid #555555;
  border-radius: 4px;
  color: #ffffff;
  font-size: 14px;
  resize: vertical;
  min-height: 100px;
}

.review-textarea:focus {
  outline: none;
  border-color: #4a9eff;
}

.review-actions {
  display: flex;
  justify-content: flex-end;
}

.submit-review-btn {
  background: var(--Gradient-Button-Action);
  color: white;
  border: none;
  border-radius: 4px;
  padding: 12px 24px;
  font: var(--Text-BodyMedium);
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s;
}

.submit-review-btn:hover:not(:disabled) {
  background: var(--Color-Primary);
  transform: translateY(-1px);
}

.submit-review-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

/* User Review Notice */
.user-review-notice,
.login-prompt,
.purchase-required-notice {
  background: #2a2a2a;
  padding: var(--Gap-Medium);
  border-radius: 4px;
  margin: var(--Gap-Large);
  text-align: center;
  border: 1px solid #3a3a3a;
}

.user-review-notice p,
.login-prompt p,
.purchase-required-notice p {
  margin: 0 0 var(--Gap-Small) 0;
  color: #cccccc;
  font-size: 14px;
}

.login-prompt a {
  color: #4a9eff;
  text-decoration: none;
}

.login-prompt a:hover {
  text-decoration: underline;
}

.edit-review-btn,
.cancel-review-btn {
  background: #4a9eff;
  color: white;
  border: none;
  border-radius: 4px;
  padding: 8px 16px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  margin: 0 4px;
}

.edit-review-btn:hover,
.cancel-review-btn:hover {
  background: #357abd;
  transform: translateY(-1px);
}

.cancel-review-btn {
  background: #555555;
  color: #ffffff;
  border: 1px solid #666666;
}

.cancel-review-btn:hover {
  background: #666666;
  border-color: #4a9eff;
}

.purchase-required-notice {
  border: 1px solid #ff9500;
  background: rgba(255, 149, 0, 0.1);
}

.purchase-required-notice p {
  color: #ff9500;
}

/* Reviews List */
.reviews-list {
  padding: var(--Gap-Large);
}

.no-reviews {
  text-align: center;
  padding: var(--Gap-Large);
  color: #999999;
  font: var(--Text-BodyMedium);
}

.review-card {
  background: #2a2a2a;
  margin-bottom: var(--Gap-Medium);
  border-radius: 4px;
  overflow: hidden;
}

.review-card-header {
  display: flex;
  align-items: center;
  padding: var(--Gap-Medium);
  gap: var(--Gap-Medium);
}

.reviewer-avatar {
  width: 40px;
  height: 40px;
  background: #555555;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.avatar-text {
  color: #ffffff;
  font-size: 14px;
  font-weight: 600;
}

.reviewer-details {
  flex: 1;
}

.reviewer-name {
  color: #ffffff;
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 2px;
}

.review-date {
  color: #999999;
  font-size: 12px;
}

.review-score {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  text-align: right;
}

.score-number {
  color: #ffffff;
  font-size: 18px;
  font-weight: 700;
  line-height: 1;
}

.score-label {
  color: #999999;
  font-size: 11px;
  margin-top: 2px;
}

.review-card-content {
  padding: 0 var(--Gap-Medium) var(--Gap-Medium) var(--Gap-Medium);
}

.review-card-content p {
  margin: 0;
  color: #cccccc;
  font-size: 14px;
  line-height: 1.5;
}

.review-actions-bottom {
  padding: var(--Gap-Small) var(--Gap-Medium);
  border-top: 1px solid #3a3a3a;
  display: flex;
  gap: var(--Gap-Small);
}

.action-btn {
  background: none;
  border: none;
  color: #999999;
  font-size: 12px;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 3px;
  transition: all 0.2s;
}

.action-btn:hover {
  background: #3a3a3a;
}

.edit-btn:hover {
  color: #4a9eff;
}

.delete-btn:hover {
  color: #ff6b6b;
}

.show-more-reviews {
  text-align: center;
  margin-top: var(--Gap-Medium);
  padding: 0 var(--Gap-Large) var(--Gap-Large) var(--Gap-Large);
}

.show-more-btn {
  background: #3a3a3a;
  color: #ffffff;
  border: 1px solid #555555;
  border-radius: 4px;
  padding: 12px 24px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s;
}

.show-more-btn:hover {
  background: #555555;
  border-color: #4a9eff;
}

/* Responsive */
@media (max-width: 1024px) {
  .game-content {
    width: 95%;
  }
  
  .section-label {
    padding-left: var(--Gap-Medium);
  }
  
  .main-content-grid {
    grid-template-columns: 1fr;
  }
  
  .game-info-section {
    order: -1;
  }
  
  .action-section {
    flex-direction: column;
    text-align: center;
    gap: var(--Gap-Medium);
  }
  
  .reviews-header {
    flex-direction: column;
    align-items: center;
    text-align: center;
  }
  
  .average-rating {
    justify-content: center;
  }
}

@media (max-width: 768px) {
  .game-page-container {
    padding: var(--Gap-Medium) var(--Gap-Small);
  }
  
  .game-content {
    width: 100%;
  }
  
  .section-label {
    padding-left: var(--Gap-Small);
    font-size: 1.1rem;
  }
  
  .thumbnail-grid {
    grid-template-columns: repeat(auto-fit, minmax(80px, 1fr));
  }
  
  .action-section {
    flex-direction: column;
    text-align: center;
    gap: var(--Gap-Medium);
  }
  
  .action-buttons {
    flex-direction: column;
    gap: var(--Gap-Small);
  }
  
  .action-buttons .button {
    width: 100%;
    justify-content: center;
  }
  
  .owned-status {
    width: 100%;
  }
  
  .owned-status .button {
    width: 100%;
    justify-content: center;
  }
  
  .rating-input {
    flex-direction: column;
    align-items: stretch;
  }
  
  .rating-input label {
    min-width: auto;
    text-align: center;
  }
  
  .rating-slider {
    min-width: auto;
  }
  
  .review-header {
    flex-direction: column;
    align-items: center;
    gap: var(--Gap-Small);
  }
  
  .reviewer-info {
    text-align: center;
  }
  
  .review-rating {
    justify-content: center;
  }
}

@media (max-width: 480px) {
  .game-page-container {
    padding: var(--Gap-Small);
  }
  
  .section-label {
    font-size: 1rem;
    padding-left: 0;
  }
  
  .main-content-grid,
  .action-section,
  .description-section,
  .reviews-section {
    padding: var(--Gap-Medium);
  }
  
  .add-review-section {
    padding: var(--Gap-Medium);
  }
  
  .review-item {
    padding: var(--Gap-Medium);
  }
  
  .rating-display {
    flex-direction: column;
    gap: 8px;
  }
  
  .rating-bar {
    width: 100%;
    max-width: 200px;
  }
}
</style>
