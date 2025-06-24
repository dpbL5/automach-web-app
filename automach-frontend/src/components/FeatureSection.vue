<template>
  <div class="feature">
    <div v-if="loading" class="loading">Loading featured games...</div>
    <div v-else-if="slides.length > 0" class="container">
      <button class="arrow left" @click="prevSlide">&#8592;</button>
      <div class="game-slide" @click="navigateToGame(slides[current])">
        <img
          :src="getCurrentGameImage()"
          :alt="slides[current].title"
          class="banner"
          @error="handleImageError"
        />
        <div class="gameinfo">
          <h2>{{ slides[current].title }}</h2>
          <p>{{ slides[current].gameInfo || 'Experience this amazing game!' }}</p>
          <div class="price-tag" v-if="slides[current].price !== undefined">
            <span v-if="slides[current].price === 0" class="free">Free</span>
            <span v-else class="price">${{ slides[current].price.toFixed(2) }}</span>
          </div>
        </div>
      </div>
      <button class="arrow right" @click="nextSlide">&#8594;</button>
    </div>
    <div v-else class="no-games">
      <p>No featured games available</p>
    </div>
    
    <div class="dots" v-if="slides.length > 1">
      <span
        v-for="(slide, idx) in slides"
        :key="idx"
        class="dot"
        :class="{ active: idx === current }"
        @click="goToSlide(idx)"
      ></span>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      slides: [],
      current: 0,
      timer: null,
      loading: true
    };
  },
  async mounted() {
    await this.fetchFeaturedGames();
    if (this.slides.length > 1) {
      this.startSlideshow();
    }
  },
  beforeUnmount() {
    if (this.timer) {
      clearInterval(this.timer);
    }
  },
  methods: {
    async fetchFeaturedGames() {
      try {
        this.loading = true;
        // Fetch featured games
        const response = await fetch('http://localhost:5174/api/games?isFeatured=true&pageSize=5');
        
        if (!response.ok) {
          throw new Error('Failed to fetch featured games');
        }
        
        const data = await response.json();
        
        // Fetch images for each game
        const gamesWithImages = await Promise.all(data.items.map(async game => {
          try {
            const imgResponse = await fetch(`http://localhost:5174/api/games/${game.id}/images`);
            if (imgResponse.ok) {
              const images = await imgResponse.json();
              game.images = images;
            }
          } catch (error) {
            console.error(`Error fetching images for game ${game.id}:`, error);
            game.images = [];
          }
          return game;
        }));
        
        this.slides = gamesWithImages;
      } catch (error) {
        console.error('Error fetching featured games:', error);
        // Fallback to static data if API fails
        this.slides = [
          {
            id: null,
            title: "Welcome to Automach",
            gameInfo: "Discover amazing games in our store!",
            price: 0,
            images: []
          }
        ];
      } finally {
        this.loading = false;
      }
    },

    getCurrentGameImage() {
      const currentGame = this.slides[this.current];
      if (currentGame.images && currentGame.images.length > 0) {
        // Use first image as banner
        return currentGame.images[0].url;
      }
      
      // Fallback to placeholder
      return `https://via.placeholder.com/800x300/2c3e50/ffffff?text=${encodeURIComponent(currentGame.title)}`;
    },

    handleImageError(event) {
      const currentGame = this.slides[this.current];
      event.target.src = `https://via.placeholder.com/800x300/2c3e50/ffffff?text=${encodeURIComponent(currentGame.title)}`;
    },

    navigateToGame(game) {
      if (game.id) {
        this.$router.push(`/game/${game.id}`);
      }
    },

    startSlideshow() {
      this.timer = setInterval(this.nextSlide, 5000); // 5 seconds
    },

    nextSlide() {
      this.current = (this.current + 1) % this.slides.length;
    },

    prevSlide() {
      this.current = (this.current - 1 + this.slides.length) % this.slides.length;
    },

    goToSlide(idx) {
      this.current = idx;
      // Restart slideshow timer
      if (this.timer) {
        clearInterval(this.timer);
        this.startSlideshow();
      }
    }
  }
};
</script>

<style scoped>
.feature {
  height: 320px;
  background: var(--Color-Background-Main50);
  box-shadow: var(--ShadowLight);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  position: relative;
  border-radius: 8px;
  overflow: hidden;
}

.loading,
.no-games {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 200px;
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
}

.container {
  display: flex;
  align-items: center;
  position: relative;
  width: 100%;
  max-width: 900px;
  height: 260px;
  padding: 0 20px;
}

.game-slide {
  position: relative;
  width: 100%;
  height: 240px;
  cursor: pointer;
  transition: transform 0.3s ease;
  border-radius: 12px;
  overflow: hidden;
}

.game-slide:hover {
  transform: scale(1.02);
}

.banner {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: opacity 0.3s ease;
}

.arrow {
  background: rgba(0, 0, 0, 0.5);
  border: none;
  color: #fff;
  font-size: 2rem;
  width: 50px;
  height: 50px;
  border-radius: 50%;
  cursor: pointer;
  z-index: 2;
  transition: all 0.3s ease;
  position: absolute;
  display: flex;
  align-items: center;
  justify-content: center;
}

.arrow.left {
  left: 10px;
}

.arrow.right {
  right: 10px;
}

.arrow:hover {
  background: rgba(0, 0, 0, 0.8);
  transform: scale(1.1);
}

.gameinfo {
  position: absolute;
  left: 30px;
  bottom: 30px;
  color: #fff;
  text-shadow: 0 2px 8px rgba(0, 0, 0, 0.8);
  background: linear-gradient(135deg, rgba(0, 0, 0, 0.7), rgba(0, 0, 0, 0.4));
  padding: 20px 24px;
  border-radius: 12px;
  min-width: 250px;
  max-width: 400px;
  backdrop-filter: blur(10px);
}

.gameinfo h2 {
  font: var(--Text-HeadingMedium);
  color: #fff;
  margin: 0 0 8px 0;
  font-weight: 700;
}

.gameinfo p {
  font: var(--Text-BodyMedium);
  color: rgba(255, 255, 255, 0.9);
  margin: 0 0 12px 0;
  line-height: 1.4;
}

.price-tag {
  display: inline-block;
}

.price-tag .free {
  background: var(--Color-Accent-Green);
  color: var(--Color-Background-Main);
  padding: 6px 12px;
  border-radius: 4px;
  font: var(--Text-BodySmall);
  font-weight: 700;
  text-transform: uppercase;
}

.price-tag .price {
  background: var(--Color-Primary);
  color: #fff;
  padding: 6px 12px;
  border-radius: 4px;
  font: var(--Text-BodySmall);
  font-weight: 700;
}

.dots {
  display: flex;
  justify-content: center;
  margin-top: 16px;
  gap: 8px;
}

.dot {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.4);
  cursor: pointer;
  transition: all 0.3s ease;
}

.dot:hover {
  background: rgba(255, 255, 255, 0.6);
  transform: scale(1.2);
}

.dot.active {
  background: var(--Color-Primary);
  transform: scale(1.3);
}

/* Responsive */
@media (max-width: 768px) {
  .feature {
    height: 280px;
  }
  
  .container {
    height: 220px;
    padding: 0 10px;
  }
  
  .game-slide {
    height: 200px;
  }
  
  .gameinfo {
    left: 20px;
    bottom: 20px;
    padding: 16px 20px;
    min-width: 200px;
    max-width: 300px;
  }
  
  .gameinfo h2 {
    font: var(--Text-HeadingSmall);
  }
  
  .gameinfo p {
    font: var(--Text-BodySmall);
  }
  
  .arrow {
    width: 40px;
    height: 40px;
    font-size: 1.5rem;
  }
}
</style>
