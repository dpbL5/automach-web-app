<template>
  <Nav />
  <div class="main">
    <!-- Featured Section -->
    <label>Feature & Recommended</label>
    <div class="section feature">
      <button 
        @click="previousFeatured" 
        v-if="featuredGames.length > 1" 
        class="chevron-button"
        :disabled="featuredGames.length <= 1"
      >
        <img class="chevron" src="data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzYiIGhlaWdodD0iMzYiIHZpZXdCb3g9IjAgMCAzNiAzNiIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHBhdGggZD0iTTEzLjUgMTguNUwyMC41IDI1LjVMMTguNzUgMjcuMjVMMTAuNSAxOUwxMC43NSAxOC43NUwxMC41IDE4LjVMMTguNzUgOS43NUwyMC41IDExLjVMMTMuNSAxOC41WiIgZmlsbD0iIzc2ODA4QyIvPgo8L3N2Zz4K" alt="chevron_left">
      </button>
      
      <div v-if="featuredGames.length === 0" class="feature-placeholder">
        <p>Featured games will appear here</p>
      </div>
      
      <div v-else class="featured-content">
        <router-link :to="`/game/${getCurrentFeaturedGame().id}`">
          <img class="feature__banner" :src="getGameImage(getCurrentFeaturedGame())" :alt="getCurrentFeaturedGame().title" />
        </router-link>
        
        <div class="feature__description">
          <h1>{{ getCurrentFeaturedGame().title }}</h1>
          <p>{{ getGameDescription(getCurrentFeaturedGame()) }}</p>
          
          <div class="feature__showcase">
            <img v-for="(game, index) in getOtherFeaturedGames()" :key="index" 
                 :src="getGameImage(game)" :alt="game.title" />
          </div>
          <br><br>
          <div class="price">
            <span v-if="getCurrentFeaturedGame().price === 0">Free-to-play</span>
            <span v-else>${{ getCurrentFeaturedGame().price.toFixed(2) }}</span>
          </div>
          <br><br>

        </div>
      </div>
      
      <button 
        @click="nextFeatured" 
        v-if="featuredGames.length > 1" 
        class="chevron-button"
        :disabled="featuredGames.length <= 1"
      >
        <img class="chevron" src="data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzYiIGhlaWdodD0iMzYiIHZpZXdCb3g9IjAgMCAzNiAzNiIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHBhdGggZD0iTTIyLjUgMTguNUwxNS41IDI1LjVMMTcuMjUgMjcuMjVMMjUuNSAxOUwyNS4yNSAxOC43NUwyNS41IDE4LjVMMTcuMjUgOS43NUwxNS41IDExLjVMMjIuNSAxOC41WiIgZmlsbD0iIzc2ODA4QyIvPgo8L3N2Zz4K" alt="chevron_right">
      </button>
    </div>

    <!-- Free to Play Games -->
    <label>Free to play games</label>
    <div class="section section--horizontal-flow">
      <div v-if="freeGames.length === 0" class="feature-placeholder">
        <p>Loading free games...</p>
      </div>
      
      <div v-for="game in freeGames" :key="game.id" class="card">
        <router-link class="card" :to="`/game/${game.id}`">
          <img class="card__img" :src="getGameImage(game)" :alt="game.title" />
          <div class="card__title">{{ game.title }}</div>
          <div class="card__action">
            <div id="dim-text">Free</div>
            <button class="button button--play">Play now</button>
          </div>
        </router-link>
      </div>
    </div>

    <!-- New & Trending -->
    <label>New & Trending</label>
    <div class="section section--horizontal-flow">
      <div v-if="newGames.length === 0" class="feature-placeholder">
        <p>Loading new games...</p>
      </div>
      
      <div v-for="game in newGames" :key="game.id" class="card">
        <router-link class="card" :to="`/game/${game.id}`">
          <img class="card__img" :src="getGameImage(game)" :alt="game.title" />
          <div class="card__title">{{ game.title }}</div>
          <div class="card__action">
            <div id="dim-text">
              <span v-if="game.price === 0">Free</span>
              <span v-else>${{ game.price.toFixed(2) }}</span>
            </div>
            <button class="button button--play">Play now</button>
          </div>
        </router-link>
      </div>
    </div>

    <!-- Top Sellers -->
    <label>Top Selling</label>
    <div class="section section--vertical-flow">
      <div v-if="topSellers.length === 0" class="feature-placeholder">
        <p>Loading top sellers...</p>
      </div>
      
      <div v-for="game in topSellers" :key="game.id" class="card">
        <router-link class="card--horizontal" :to="`/game/${game.id}`">
          <img class="card--horizontal__img" :src="getGameImage(game)" :alt="game.title" />
          <div class="card__title">{{ game.title }}</div>
          <div class="card__action">
            <div class="card__price">
              <span v-if="game.price === 0">Free</span>
              <span v-else>${{ game.price.toFixed(2) }}</span>
            </div>
            <button class="button button--primary">Add to cart</button>
          </div>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script>
import Nav from "./Nav.vue";

export default {
  components: {
    Nav
  },
  data() {
    return {
      featuredGames: [],
      freeGames: [],
      newGames: [],
      topSellers: [],
      currentFeaturedIndex: 0
    };
  },
  async mounted() {
    await Promise.all([
      this.fetchFeaturedGames(),
      this.fetchFreeGames(),
      this.fetchNewGames(),
      this.fetchTopSellers()
    ]);
  },
  methods: {
    async fetchFeaturedGames() {
      try {
        // Fetch featured games (isFeatured = true)
        const response = await fetch('http://localhost:5174/api/games?isFeatured=true&pageSize=3');
        if (response.ok) {
          const data = await response.json();
          this.featuredGames = await this.enrichGamesWithImages(data.items);
        }
      } catch (error) {
        console.error('Error fetching featured games:', error);
      }
    },

    async fetchFreeGames() {
      try {
        // Fetch free games (price = 0) sorted by newest
        const response = await fetch('http://localhost:5174/api/games?maxPrice=0&pageSize=3');
        if (response.ok) {
          const data = await response.json();
          this.freeGames = await this.enrichGamesWithImages(data.items);
        }
      } catch (error) {
        console.error('Error fetching free games:', error);
      }
    },

    async fetchNewGames() {
      try {
        // Fetch games added in the last 3 months
        const threeMonthsAgo = new Date();
        threeMonthsAgo.setMonth(threeMonthsAgo.getMonth() - 3);
        const dateFilter = threeMonthsAgo.toISOString().split('T')[0];
        
        const response = await fetch(`http://localhost:5174/api/games?createdAfter=${dateFilter}&pageSize=3`);
        if (response.ok) {
          const data = await response.json();
          this.newGames = await this.enrichGamesWithImages(data.items);
        } else {
          // Fallback: fetch newest games if date filter not supported
          const fallbackResponse = await fetch('http://localhost:5174/api/games?pageSize=3');
          if (fallbackResponse.ok) {
            const fallbackData = await fallbackResponse.json();
            this.newGames = await this.enrichGamesWithImages(fallbackData.items);
          }
        }
      } catch (error) {
        console.error('Error fetching new games:', error);
        // Fallback: fetch newest games
        try {
          const response = await fetch('http://localhost:5174/api/games?pageSize=3');
          if (response.ok) {
            const data = await response.json();
            this.newGames = await this.enrichGamesWithImages(data.items);
          }
        } catch (fallbackError) {
          console.error('Error fetching fallback new games:', fallbackError);
        }
      }
    },

    async fetchTopSellers() {
      try {
        // Try to get games with most transactions
        // Since we might not have a direct API, we'll implement a workaround
        
        // First, fetch all games
        const gamesResponse = await fetch('http://localhost:5174/api/games?pageSize=20');
        if (!gamesResponse.ok) {
          throw new Error('Failed to fetch games');
        }
        
        const gamesData = await gamesResponse.json();
        const games = gamesData.items;
        
        // Try to fetch transaction items to count sales for each game
        let gamesWithSalesCount = [];
        
        try {
          const transactionItemsResponse = await fetch('http://localhost:5174/api/transactionitems');
          if (transactionItemsResponse.ok) {
            const transactionItems = await transactionItemsResponse.json();
            
            // Count sales for each game
            const salesCount = {};
            transactionItems.forEach(item => {
              salesCount[item.gameId] = (salesCount[item.gameId] || 0) + 1;
            });
            
            // Add sales count to games and sort
            gamesWithSalesCount = games.map(game => ({
              ...game,
              salesCount: salesCount[game.id] || 0
            })).sort((a, b) => b.salesCount - a.salesCount);
            
          } else {
            // Fallback: use featured games or random games
            gamesWithSalesCount = games;
          }
        } catch (transactionError) {
          console.warn('Could not fetch transaction data, using fallback:', transactionError);
          gamesWithSalesCount = games;
        }
        
        // Take top 3 games
        this.topSellers = await this.enrichGamesWithImages(gamesWithSalesCount.slice(0, 3));
        
      } catch (error) {
        console.error('Error fetching top sellers:', error);
        // Final fallback: fetch any 3 games
        try {
          const response = await fetch('http://localhost:5174/api/games?pageSize=3');
          if (response.ok) {
            const data = await response.json();
            this.topSellers = await this.enrichGamesWithImages(data.items);
          }
        } catch (fallbackError) {
          console.error('Error with fallback top sellers:', fallbackError);
        }
      }
    },

    async enrichGamesWithImages(games) {
      // Enrich games with images like in Browse component
      const imagePromises = games.map(async (game) => {
        if (!game.imageUrls || game.imageUrls.length === 0) {
          try {
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), 3000); // 3s timeout for home page
            
            const imgRes = await fetch(`http://localhost:5174/api/games/${game.id}/images`, {
              signal: controller.signal
            });
            
            clearTimeout(timeoutId);
            
            if (imgRes.ok) {
              const imgs = await imgRes.json();
              game.imageUrl = imgs[0]?.url || 'https://via.placeholder.com/300x200?text=Game';
            } else {
              game.imageUrl = 'https://via.placeholder.com/300x200?text=Game';
            }
          } catch (error) {
            if (error.name === 'AbortError') {
              console.warn(`Image fetch timeout for game ${game.id}`);
            } else {
              console.error(`Error fetching images for game ${game.id}:`, error);
            }
            game.imageUrl = 'https://via.placeholder.com/300x200?text=Game';
          }
        }
        return game;
      });
      
      await Promise.allSettled(imagePromises);
      return games;
    },

    getGameImage(game) {
      // Get game image with fallback like in Browse component
      if (game.imageUrl) {
        return game.imageUrl;
      }
      
      if (game.imageUrls && game.imageUrls.length > 0 && game.imageUrls[0].url) {
        return game.imageUrls[0].url;
      }
      
      return 'https://via.placeholder.com/300x200?text=Game';
    },

    getGameDescription(game) {
      // Generate appropriate description based on game title or use default
      if (game.description) {
        return game.description;
      }
      
      // Generate dynamic description based on game type
      const descriptions = [
        'Experience this amazing game with stunning graphics and immersive gameplay.',
        'Join millions of players from across the globe in this epic adventure.',
        'Discover a world full of excitement, challenges, and endless possibilities.',
        'Unleash your potential in this action-packed gaming experience.',
        'Embark on a journey that will test your skills and determination.'
      ];
      
      // Use game ID to consistently get same description for same game
      const index = game.id % descriptions.length;
      return descriptions[index];
    },

    // Chevron navigation methods
    previousFeatured() {
      if (this.featuredGames.length > 1) {
        this.currentFeaturedIndex = this.currentFeaturedIndex === 0 
          ? this.featuredGames.length - 1 
          : this.currentFeaturedIndex - 1;
      }
    },

    nextFeatured() {
      if (this.featuredGames.length > 1) {
        this.currentFeaturedIndex = (this.currentFeaturedIndex + 1) % this.featuredGames.length;
      }
    },

    getCurrentFeaturedGame() {
      return this.featuredGames[this.currentFeaturedIndex] || this.featuredGames[0];
    },

    getOtherFeaturedGames() {
      if (this.featuredGames.length <= 1) return [];
      
      const otherGames = [...this.featuredGames];
      otherGames.splice(this.currentFeaturedIndex, 1);
      return otherGames.slice(0, 2); // Maximum 2 other games for showcase
    }
  }
};
</script>

<style>
:root {
  /* Text Color */
  --TextMain: #F3F3F3;
  --TextDim: #76808C;
  
  /* Background Color */
  --BgMain: #0E141B;
  --BgMain50: rgba(14, 20, 27, 50%); 
  --BgHighlight: #1E2329;
  --BgHover: #313843;
  --BgSecondary: #14344B;
  --BgTertiary: #212B45;

  /* Color Scheme */
  --ColorPrimary: #66C0F4;
  --ColorSecondary: #4B619B;

  /* Accent Color */
  --AccentGreen: #A1CD44;
  --AccentGreenHi: #5ba32b;
  --AccentRed: #CD5444;
  --AccentYellow: #C1B15F;

  /* Font Variants */
  --HeadingNormal: normal 700 18px/1.4 "Motiva Sans", Arial, sans-serif;
  --BodyNormal: normal 400 12px/1.4 "Motiva Sans", Arial, sans-serif;
  --BodyLarge: normal 400 14px/1.4 "Motiva Sans", Arial, sans-serif;

  --ShadowLight: 0px 10px 50px 10px rgba(0, 0, 0, 20%);
  --ShadowHeavy: 0px 10px 50px 10px rgba(0, 0, 0, 40%);
}

/*  Body  */
.main {
  background-color: var(--BgSecondary);
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  padding-bottom: 50px;
}

label {
  font: var(--HeadingNormal);
  color: var(--TextMain);
  align-self: center;
  width: 1000px;
  max-width: 100%;
  padding-top: 20px;
  padding-bottom: 12px;
}

/* Section */
.section {
  border-radius: 3px;
  width: 1000px;
  max-width: 100%;
  display: flex;
  align-self: center;
  margin: 0px 10px 50px 50px;
  background-color: var(--BgMain50);
  box-shadow: var(--ShadowLight);
}

.section--horizontal-flow {
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  overflow-x: auto;
  padding: 8px;
  gap: 12px;
}

.section--vertical-flow {
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-template-rows: 1fr 1fr 1fr;
  grid-gap: 12px;
  padding: 8px;
}

.feature {
  max-width: 1000px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.featured-content {
  display: flex;
  width: 100%;
}

.feature__banner {
  width: 550px;
  height: 350px;
  object-fit: cover;
}

.feature__description {
  display: block;
  flex-direction: row;
  color: var(--TextMain);
  padding: 20px;
  height: 300px;
  flex: 1;
}

.feature__description h1 {
  font: var(--HeadingNormal);
  color: var(--TextMain);
  margin-bottom: 10px;
  font-size: 22px;
}

.feature__description p {
  font: var(--BodyNormal);
  color: var(--TextMain);
  margin-bottom: 15px;
  line-height: 1.4;
}

.feature__showcase {
  display: flex;
  gap: 5px;
  margin-bottom: 15px;
}

.feature__showcase img {
  height: 89px;
  width: auto;
  border-radius: 3px;
}

.chevron-button {
  background: none;
  border: none;
  padding: 0;
  cursor: pointer;
  opacity: 0.7;
  transition: opacity 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.chevron-button:hover {
  opacity: 1;
}

.chevron-button:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

.chevron {
  width: 36px;
  height: 36px;
  display: block;
}

.price {
  display: inline-block;
  background-color: var(--BgSecondary);
  padding: 5px 8px;
  border-radius: 3px;
  font: var(--BodyNormal);
  color: var(--TextMain);
}



.feature-placeholder {
  height: 300px;
  background: var(--BgHighlight);
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
}

.feature-placeholder p {
  color: var(--TextDim);
  font: var(--BodyNormal);
}

/* Card */
.card {
  background-color: var(--BgMain);
  text-decoration: none;
  display: flex;
  position: relative;
  flex-direction: column;
  padding: 12px;
  border-radius: 6px;
  min-width: 280px;
  flex-shrink: 0;
}

.card__img {
  width: 280px;
  height: 158px;
  object-fit: cover;
  border-radius: 3px;
}

.card__action {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.card__action > * {
  padding: 0px 12px;
}

.card__title {
  color: var(--TextMain);
  font: var(--BodyLarge);
  padding: 8px 12px 8px 0px;
  margin: 0px 0px 12px 0px;
  font-weight: 700;
  font-size: 16px;
  text-decoration: none;
}

#dim-text {
  color: var(--TextDim);
  font: var(--BodyNormal);
}



.card--horizontal {
  display: grid;
  text-decoration: none;
  grid-template-columns: 1fr 2fr 2fr;
  grid-template-areas:
    "img title action";
  column-gap: 8px;
  align-items: center;
}

.card--horizontal__img {
  width: 173px;
  height: 76px;
  object-fit: cover;
  grid-area: img;
  border-radius: 3px;
}

.card--horizontal .card__title {
  padding: 0px;
  grid-area: title;
  margin: 0;
  align-self: center;
}

.card--horizontal .card__action {
  grid-area: action;
  justify-content: flex-end;
  align-self: center;
}

.card__price {
  text-decoration: none;
  color: var(--TextMain);
  font-weight: 700;
  font: var(--BodyNormal);
}



/* Button Styles */
.button {
  color: var(--TextMain);
  background-color: var(--BgHighlight);
  font: var(--BodyNormal);
  border-radius: 3px;
  border: none;
  padding: 12px 24px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.button:hover {
  color: var(--TextMain);
  background-color: var(--BgHover);
}

.button--primary {
  background: var(--ColorSecondary);
}

.button--primary:hover {
  color: var(--BgMain);
  background: var(--ColorPrimary);
}

.button--play {
  background-color: var(--AccentGreen);
  color: var(--BgMain);
  font-weight: 700;
}

.button--play:hover {
  background: var(--AccentGreenHi);
  color: var(--BgMain);
}

/* Responsive Design */
@media (max-width: 1024px) {
  label {
    width: 95%;
    padding-left: 20px;
  }
  
  .section {
    width: 95%;
    margin: 0 auto 30px auto;
  }
  
  .feature__banner {
    width: 400px;
    height: 250px;
  }
  
  .feature__description {
    height: auto;
    padding: 15px;
  }
}

@media (max-width: 768px) {
  .featured-content {
    flex-direction: column;
  }
  
  .feature__banner {
    width: 100%;
    height: 200px;
  }
  
  .section--vertical-flow {
    grid-template-columns: 1fr;
    grid-template-rows: auto;
  }
  
  .section--horizontal-flow {
    flex-direction: column;
    align-items: center;
  }
  
  .card {
    min-width: 100%;
    max-width: 300px;
  }
  
  .card__img {
    width: 100%;
  }
  
     .card--horizontal {
     grid-template-columns: 120px 1fr;
     grid-template-areas:
       "img title"
       "img action";
   }
  
  .card--horizontal__img {
    width: 120px;
    height: 68px;
  }
}

@media (max-width: 480px) {
  .main {
    padding: 0 10px;
  }
  
  label {
    font-size: 16px;
    padding-left: 10px;
  }
  
  .section {
    margin: 0 0 20px 0;
  }
  
  .feature__description h1 {
    font-size: 18px;
  }
  
  .feature__showcase {
    flex-wrap: wrap;
  }
  
  .feature__showcase img {
    height: 60px;
  }
}
</style>
