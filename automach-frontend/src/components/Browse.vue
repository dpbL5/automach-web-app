<template>
  <Nav />
  <div class="view">
    <div class="view-wrapper">
      <h1>BROWSE GAMES</h1>
      <section class="browser-wrapper">
        <div class="grouping">
          <button 
            :class="['button', 'button--grouping', { active: activeFilter === 'all' }]" 
            @click="setFilter('all')">All items</button>
          <button 
            :class="['button', 'button--grouping', { active: activeFilter === 'featured' }]" 
            @click="setFilter('featured')">⭐ Featured</button>
          <button 
            :class="['button', 'button--grouping', { active: activeFilter === 'trending' }]" 
            @click="setFilter('trending')">New & Trending</button>
          <button 
            :class="['button', 'button--grouping', { active: activeFilter === 'free' }]" 
            @click="setFilter('free')">Free to play</button>
          <button 
            :class="['button', 'button--grouping', { active: activeFilter === 'top' }]" 
            @click="setFilter('top')">Top Sellers</button>
        </div>

        <div class="filter">
          <h2>Filter Options</h2>
          <div class="search-box">
            <label for="game-search">Search by Game Name</label>
            <input
              id="game-search"
              v-model="searchTitle"
              type="text"
              placeholder="Enter game title"
              @input="debounceSearch"
            />
          </div>
          
          <div class="tag-search">
            <label for="tag-search">Search for Tags</label>
            <input
              id="tag-search"
              v-model="tagSearchInput"
              type="text"
              placeholder="Filter tags"
              @input="filterTagsDisplay"
            />
          </div>
          
          <div v-if="selectedTags.length > 0" class="selected-tags">
            <h3>Selected Tags</h3>
            <div class="tags">
              <div 
                v-for="tag in selectedTags" 
                :key="tag.id" 
                class="tag selected-tag"
                @click="removeTag(tag)"
              >
                {{ tag.title }} ✕
              </div>
            </div>
          </div>
          
          <label for="genres">Genres</label>
          <div class="tags menu">
            <div 
              v-for="tag in filteredTags" 
              :key="tag.id" 
              :class="['tag', 'menuitem', { 'selected': isTagSelected(tag) }]"
              @click="toggleTag(tag)"
            >
              {{ tag.title }}
            </div>
          </div>
          
          <div>
            <label for="price">Max Price: ${{ maxPrice }}</label>
            <input 
              type="range" 
              min="0" 
              max="100" 
              v-model="maxPrice" 
              @input="debounceSearch"
            />
            <div class="price-range">
              <span>$0</span>
              <span>${{ maxPrice }}</span>
            </div>
          </div>
        </div>

        <div class="results">
          <div v-if="loading" class="loading">
            <div class="loader"></div>
            <p>Loading games...</p>
          </div>
          
          <div v-else-if="games.length === 0" class="no-results">
            <p>No games found matching your criteria.</p>
          </div>
          
          <div v-else class="pagination-info">
            <p>Trang {{ currentPage }} / {{ totalPages }} - Hiển thị {{ ((currentPage - 1) * pageSize) + 1 }}-{{ Math.min(currentPage * pageSize, totalCount) }} trong tổng số {{ totalCount }} game</p>
          </div>
          
          <div v-else v-for="game in games" :key="game.id" class="item-container">
            <router-link :to="`/game/${game.id}`" class="item">
              <div class="item-image-wrapper">
                <img 
                  class="himg" 
                  :src="getGameImage(game)" 
                  :alt="game.title"
                  @error="handleImageError($event)"
                />
                <div v-if="game.isFeatured" class="featured-badge">⭐ Featured</div>
              </div>
              <h3>{{ game.title }}</h3>
              <div class="tags">
                <div v-for="(tag, index) in getGameTags(game)" :key="index" class="tag">
                  {{ tag.title }}
                </div>
              </div>



              <div class="cta">
                <span v-if="game.price === 0">Free</span>
                <span v-else>${{ game.price.toFixed(2) }}</span>
                <div v-if="isGameInLibrary(game.id)" class="owned-status">
                  <button class="button button--owned" disabled>
                    <i class="fas fa-check"></i>
                    In Library
                  </button>
                </div>
                <button 
                  v-else
                  class="button" 
                  :class="isInCart(game.id) ? 'button--secondary' : 'button--primary'" 
                  @click.prevent="handleAddToCart(game)"
                  :disabled="isInCart(game.id)"
                >
                  {{ isInCart(game.id) ? 'In Cart' : 'Add to cart' }}
                </button>
              </div>
            </router-link>
          </div>
        </div>


        
        <div class="pagination-wrapper" v-if="totalPages > 0">
          <div class="pagination-info-mobile">
            <span>Trang {{ currentPage }} / {{ totalPages }}</span>
          </div>
          
                     <div class="pagination">
             <!-- First Page Button -->
             <a 
               href="#" 
               @click.prevent="goToPage(1)" 
               :class="{ disabled: currentPage === 1 || loading }"
               title="Trang đầu"
             >
               <span class="pagination-text">Đầu</span>
               <span class="pagination-icon">⟪</span>
             </a>
             
             <!-- Previous Page Button -->
             <a 
               href="#" 
               @click.prevent="goToPage(currentPage - 1)" 
               :class="{ disabled: currentPage === 1 || loading }"
               title="Trang trước"
             >
               <span class="pagination-text">Trước</span>
               <span class="pagination-icon">‹</span>
             </a>
            
                         <!-- Page Numbers -->
             <template v-for="page in paginationRange" :key="page">
               <span 
                 v-if="page === '...'" 
                 class="pagination-ellipsis"
               >...</span>
                              <a 
                 v-else
                 href="#" 
                 @click.prevent="goToPage(page)" 
                 :class="{ active: currentPage === page, disabled: loading }"
                 :title="`Trang ${page}`"
               >{{ page }}</a>
             </template>
             
             <!-- Next Page Button -->
             <a 
               href="#" 
               @click.prevent="goToPage(currentPage + 1)" 
               :class="{ disabled: currentPage === totalPages || loading }"
               title="Trang sau"
             >
               <span class="pagination-text">Sau</span>
               <span class="pagination-icon">›</span>
             </a>
             
             <!-- Last Page Button -->
             <a 
               href="#" 
               @click.prevent="goToPage(totalPages)" 
               :class="{ disabled: currentPage === totalPages || loading }"
               title="Trang cuối"
             >
               <span class="pagination-text">Cuối</span>
               <span class="pagination-icon">⟫</span>
             </a>
          </div>
          
          <div class="pagination-summary">
            <span>Hiển thị {{ ((currentPage - 1) * pageSize) + 1 }}-{{ Math.min(currentPage * pageSize, totalCount) }} trong tổng số {{ totalCount }} game</span>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script>
import Nav from "./Nav.vue";
import { addToCart, isInCart } from "../helpers/cartHelper.js";
import { gamesCache } from '../helpers/userCache';

export default {
  components: {
    Nav
  },
  data() {
    return {
      games: [],
      tags: [],
      selectedTags: [],
      filteredTags: [],
      tagSearchInput: '',
      searchTitle: '',
      maxPrice: 100,
      currentPage: 1,
      pageSize: 5,
      totalPages: 0,
      totalCount: 0,
      loading: true,
      activeFilter: 'all',
      searchTimeout: null,
      ownedGames: []
    };
  },
  computed: {
    paginationRange() {
      // Create a range of page numbers to display
      const range = [];
      const maxVisiblePages = 7; // Tăng số trang hiển thị
      
      if (this.totalPages <= maxVisiblePages) {
        // Nếu tổng số trang ít hơn hoặc bằng số trang hiển thị tối đa, hiển thị tất cả
        for (let i = 1; i <= this.totalPages; i++) {
          range.push(i);
        }
        return range;
      }
      
      let startPage = Math.max(1, this.currentPage - Math.floor(maxVisiblePages / 2));
      let endPage = Math.min(this.totalPages, startPage + maxVisiblePages - 1);
      
      // Điều chỉnh để luôn hiển thị đủ số trang
      if (endPage - startPage + 1 < maxVisiblePages) {
        startPage = Math.max(1, endPage - maxVisiblePages + 1);
      }
      
      // Thêm ellipsis logic
      if (startPage > 1) {
        range.push(1);
        if (startPage > 2) {
          range.push('...');
        }
      }
      
      for (let i = startPage; i <= endPage; i++) {
        if (i > 1 && i < this.totalPages) {
          range.push(i);
        }
      }
      
      if (endPage < this.totalPages) {
        if (endPage < this.totalPages - 1) {
          range.push('...');
        }
        range.push(this.totalPages);
      }
      
      return range;
    }
  },
  async mounted() {
    // Check for search query and page in URL
    if (this.$route.query.search) {
      this.searchTitle = this.$route.query.search;
    }
    
    if (this.$route.query.page) {
      const page = parseInt(this.$route.query.page);
      if (!isNaN(page) && page > 0) {
        this.currentPage = page;
      }
    }
    
    await Promise.all([
      this.fetchTags(),
      this.fetchGames(),
      this.loadOwnedGames()
    ]);
    
    // Listen for purchase events to refresh owned games
    window.addEventListener('game-purchased', this.loadOwnedGames);
  },
  beforeDestroy() {
    // Cleanup event listener
    window.removeEventListener('game-purchased', this.loadOwnedGames);
  },
  watch: {
    '$route.query.search': {
      handler(newSearch) {
        if (newSearch) {
          this.searchTitle = newSearch;
          this.currentPage = 1; // Reset to first page
          this.debounceSearch();
        }
      },
      immediate: false
    }
  },
  methods: {
    async fetchGames() {
      this.loading = true;
      try {
        // Build query string with filters
        let url = `http://localhost:5174/api/games?pageNumber=${this.currentPage}&pageSize=${this.pageSize}`;
        
        if (this.searchTitle) {
          url += `&title=${encodeURIComponent(this.searchTitle)}`;
        }
        
        // Add max price filter (only for 'all' filter, others have specific logic)
        if (this.activeFilter === 'all') {
          url += `&maxPrice=${this.maxPrice}`;
        }
        
        // If any tags are selected, add them to the query
        if (this.selectedTags.length > 0) {
          // Check if backend supports multiple tags
          if (this.selectedTags.length === 1) {
            // Use single tag parameter for backward compatibility
            url += `&gameTag=${encodeURIComponent(this.selectedTags[0].title)}`;
          } else {
            // Use multiple tags parameter
            this.selectedTags.forEach(tag => {
              url += `&gameTags=${encodeURIComponent(tag.title)}`;
            });
          }
        }
        
        // Apply specific filters based on activeFilter (same logic as Home component)
        if (this.activeFilter === 'featured') {
          // Featured games: isFeatured = true
          url += '&isFeatured=true';
        } else if (this.activeFilter === 'trending') {
          // New & Trending: games added in the last 3 months
          const threeMonthsAgo = new Date();
          threeMonthsAgo.setMonth(threeMonthsAgo.getMonth() - 3);
          const dateFilter = threeMonthsAgo.toISOString().split('T')[0];
          url += `&createdAfter=${dateFilter}`;
        } else if (this.activeFilter === 'free') {
          // Free to play: price = 0
          url += '&maxPrice=0';
        } else if (this.activeFilter === 'top') {
          // Top Sellers: we'll handle this with a different approach
          await this.fetchTopSellers();
          return;
        }
        
        console.log('Fetching games with URL:', url);
        const response = await fetch(url);
        
        if (!response.ok) {
          // If it's trending filter and the createdAfter parameter failed, try fallback
          if (this.activeFilter === 'trending' && url.includes('createdAfter')) {
            console.warn('createdAfter parameter not supported, trying fallback for trending games');
            const fallbackUrl = `http://localhost:5174/api/games?pageNumber=${this.currentPage}&pageSize=${this.pageSize}`;
            const fallbackResponse = await fetch(fallbackUrl);
            if (fallbackResponse.ok) {
              const fallbackData = await fallbackResponse.json();
              this.games = fallbackData.items;
              this.totalPages = fallbackData.totalPages;
              this.totalCount = fallbackData.totalCount;
              
              // Process images for fallback games
              await this.processGameImages(this.games);
              return;
            }
          }
          throw new Error('Failed to fetch games');
        }
        
        const data = await response.json();
        
        // Process images for games
        await this.processGameImages(data.items);
        
        this.games = data.items;
        this.totalPages = data.totalPages;
        this.totalCount = data.totalCount;

      } catch (error) {
        console.error('Error fetching games:', error);
        this.games = [];
      } finally {
        this.loading = false;
      }
    },
    async fetchTags() {
      try {
        const response = await fetch('http://localhost:5174/api/tags');
        
        if (!response.ok) {
          throw new Error('Failed to fetch tags');
        }
        
        this.tags = await response.json();
        this.filteredTags = [...this.tags];
      } catch (error) {
        console.error('Error fetching tags:', error);
        this.tags = [];
        this.filteredTags = [];
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
    getGameImage(game) {
      // Quy ước: Ảnh đầu tiên trong danh sách imageUrls là banner chính của game
      if (game.imageUrl) {
        return game.imageUrl;
      }
      
      if (game.imageUrls && game.imageUrls.length > 0 && game.imageUrls[0].url) {
        return game.imageUrls[0].url;
      }
      
      // Fallback image nếu không có ảnh
      return 'https://via.placeholder.com/300x150?text=Game+Image';
    },
    getGameTags(game) {
      if (game.gameTags && game.gameTags.length > 0) {
        return game.gameTags.map(gt => gt.tag).slice(0, 3); // Show up to 3 tags
      }
      return [];
    },
    toggleTag(tag) {
      const index = this.selectedTags.findIndex(t => t.id === tag.id);
      if (index !== -1) {
        // Remove tag if already selected
        this.selectedTags.splice(index, 1);
      } else {
        // Add tag if not selected
        this.selectedTags.push(tag);
      }
      // Refresh games with the updated tag selection
      this.currentPage = 1; // Reset to first page
      this.debounceSearch();
    },
    removeTag(tag) {
      const index = this.selectedTags.findIndex(t => t.id === tag.id);
      if (index !== -1) {
        this.selectedTags.splice(index, 1);
        this.debounceSearch();
      }
    },
    isTagSelected(tag) {
      return this.selectedTags.some(t => t.id === tag.id);
    },
    filterTagsDisplay() {
      if (!this.tagSearchInput) {
        this.filteredTags = [...this.tags];
      } else {
        const searchLower = this.tagSearchInput.toLowerCase();
        this.filteredTags = this.tags.filter(tag => 
          tag.title.toLowerCase().includes(searchLower)
        );
      }
    },
    goToPage(page) {
      if (page < 1 || page > this.totalPages || page === this.currentPage || this.loading) {
        return;
      }
      
      this.currentPage = page;
      
      // Scroll to top immediately for better UX
      window.scrollTo({ top: 0, behavior: 'smooth' });
      
      // Update URL to reflect current page
      this.updatePageUrl();
      
      // Call appropriate fetch method based on active filter
      if (this.activeFilter === 'top') {
        this.fetchTopSellers();
      } else {
        this.fetchGames();
      }
    },
    updatePageUrl() {
      // Update URL to include current page for better navigation experience
      const query = { ...this.$route.query };
      if (this.currentPage > 1) {
        query.page = this.currentPage;
      } else {
        delete query.page;
      }
      
      // Only update if URL actually changes
      if (JSON.stringify(query) !== JSON.stringify(this.$route.query)) {
        this.$router.replace({
          path: '/browse',
          query
        });
      }
    },
    setFilter(filter) {
      this.activeFilter = filter;
      this.currentPage = 1; // Reset to first page
      this.debounceSearch();
    },
    handleAddToCart(game) {
      // Check if user is logged in
      const token = localStorage.getItem('token');
      if (!token) {
        alert('Please login to add games to cart');
        this.$router.push('/login');
        return;
      }
      
      const success = addToCart(game.id);
      if (success) {
        // Show success message or notification
        console.log(`Added ${game.title} to cart`);
        
        // Optional: Show a toast notification
        // You could implement a toast notification system here
      } else {
        console.log(`${game.title} is already in cart`);
      }
    },
    
    isInCart(gameId) {
      return isInCart(gameId);
    },
    async processGameImages(games) {
      // Fetch ảnh riêng cho từng game song song để tăng hiệu suất
      const imagePromises = games.map(async (game) => {
        if (!game.imageUrls || game.imageUrls.length === 0) {
          try {
            // Thêm timeout để tránh chờ quá lâu
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), 5000); // 5s timeout
            
            const imgRes = await fetch(`http://localhost:5174/api/games/${game.id}/images`, {
              signal: controller.signal
            });
            
            clearTimeout(timeoutId);
            
            if (imgRes.ok) {
              const imgs = await imgRes.json();
              game.imageUrl = imgs[0]?.url || 'https://via.placeholder.com/300x150?text=Game+Image';
            } else {
              game.imageUrl = 'https://via.placeholder.com/300x150?text=Game+Image';
            }
          } catch (error) {
            if (error.name === 'AbortError') {
              console.warn(`Image fetch timeout for game ${game.id}`);
            } else {
              console.error(`Error fetching images for game ${game.id}:`, error);
            }
            game.imageUrl = 'https://via.placeholder.com/300x150?text=Game+Image';
          }
        }
        return game;
      });
      
      // Chờ tất cả image fetch hoàn thành
      await Promise.allSettled(imagePromises);
    },
    handleImageError(event) {
      // Xử lý khi ảnh banner không load được
      event.target.src = 'https://via.placeholder.com/300x150?text=Game+Image';
    },
    async fetchTopSellers() {
      this.loading = true;
      try {
        // Try to get games with most transactions (same logic as Home component)
        
        // First, fetch all games
        const gamesResponse = await fetch('http://localhost:5174/api/games?pageSize=100');
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
            // Fallback: use regular games
            gamesWithSalesCount = games;
          }
        } catch (transactionError) {
          console.warn('Could not fetch transaction data, using fallback:', transactionError);
          gamesWithSalesCount = games;
        }
        
        // Apply pagination to top sellers
        const startIndex = (this.currentPage - 1) * this.pageSize;
        const endIndex = startIndex + this.pageSize;
        const paginatedGames = gamesWithSalesCount.slice(startIndex, endIndex);
        
        // Process images for the paginated games
        await this.processGameImages(paginatedGames);
        
        this.games = paginatedGames;
        this.totalPages = Math.ceil(gamesWithSalesCount.length / this.pageSize);
        this.totalCount = gamesWithSalesCount.length;
        
      } catch (error) {
        console.error('Error fetching top sellers:', error);
        // Final fallback: fetch regular games
        const fallbackUrl = `http://localhost:5174/api/games?pageNumber=${this.currentPage}&pageSize=${this.pageSize}`;
        try {
          const response = await fetch(fallbackUrl);
          if (response.ok) {
            const data = await response.json();
            await this.processGameImages(data.items);
            this.games = data.items;
            this.totalPages = data.totalPages;
            this.totalCount = data.totalCount;
          }
        } catch (fallbackError) {
          console.error('Error with fallback top sellers:', fallbackError);
          this.games = [];
        }
      } finally {
        this.loading = false;
      }
    },
    debounceSearch() {
      // Debounce search to avoid too many API calls
      clearTimeout(this.searchTimeout);
      this.searchTimeout = setTimeout(() => {
        this.currentPage = 1; // Reset to first page when search changes
        
        // Update URL query parameter to reflect current search
        if (this.searchTitle && this.searchTitle.trim()) {
          if (this.$route.query.search !== this.searchTitle.trim()) {
            this.$router.replace({
              path: '/browse',
              query: { ...this.$route.query, search: this.searchTitle.trim() }
            });
          }
        } else {
          // Remove search query if empty
          if (this.$route.query.search) {
            const newQuery = { ...this.$route.query };
            delete newQuery.search;
            this.$router.replace({
              path: '/browse',
              query: newQuery
            });
          }
        }
        
        this.fetchGames();
      }, 300);
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

h1 {
  font: var(--Text-HeadingLarge);
  color: var(--Color-Text-Main);
  margin: 24px 0;
  text-align: center;
}

.browser-wrapper {
  display: grid;
  grid-template-areas: 
    "group group group"
    "filter results results"
    "pagination pagination pagination";
  grid-template-columns: 250px 1fr 1fr;
  gap: 16px;
  padding-bottom: 32px;
}

.browser-wrapper > * {
  background-color: var(--Color-Background-Main50);
  border-radius: 3px;
}

.grouping {
  padding: 3px 8px;
  grid-area: group;
  display: flex;
  height: 45px;
  justify-content: center;
  align-items: center;
  gap: 12px;
}

.filter {
  grid-area: filter;
  padding: 1rem;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.search-box, .tag-search {
  margin-bottom: 12px;
}

input[type="text"] {
  width: 100%;
  padding: 8px;
  border-radius: 4px;
  border: 1px solid #444;
  background-color: var(--Color-Background-Main);
  color: var(--Color-Text-Main);
}

.selected-tags {
  margin-bottom: 12px;
}

.tag.selected-tag {
  background-color: var(--Color-Primary);
  color: white;
  cursor: pointer;
}

.results {
  display: flex;
  flex-direction: column;
  grid-area: results;
  overflow: auto;
  gap: 12px;
  height: calc(135px * 5);
  padding: 12px;
}

.pagination-info {
  padding: 8px 0;
  border-bottom: 1px solid var(--Color-Background-Main20);
  margin-bottom: 8px;
}

.pagination-info p {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  margin: 0;
  text-align: center;
}

.loading, .no-results {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
}

.loader {
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  border-top: 4px solid var(--Color-Primary);
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.item {
  background-color: var(--Color-Background-Main50);
  border-radius: 3px;
  padding: 10px;
  display: grid;
  grid-template-areas: 
  "img title title"
  "img tags cta";
  grid-template-columns: auto 1fr 1fr;
  text-decoration: none;
  gap: 4px;
  transition: background-color 0.2s;
}

.item:hover {
  background-color: var(--Color-Background-Hover);
}

.item-image-wrapper {
  grid-area: img;
  position: relative;
  height: 100px;
  width: 177.8px;
  border-radius: 3px;
  overflow: hidden;
}

.item .himg {
  height: 100px;
  width: 177.8px;
  object-fit: cover;
  border-radius: 3px;
  /* Đảm bảo banner game hiển thị đúng tỷ lệ */
  object-position: center;
  background-color: var(--Color-Background-Main);
}

.featured-badge {
  position: absolute;
  top: 8px;
  right: 8px;
  background: linear-gradient(135deg, #ffd700, #ffa500);
  color: #000;
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  box-shadow: 0 2px 8px rgba(255, 215, 0, 0.4);
  z-index: 2;
}

.item h3 {
  grid-area: title;
  font: var(--Text-BodyLarge);
  color: var(--Color-Text-Main);
  margin: 0;
}

.item .tags {
  grid-area: tags;
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
}

.item .cta {
  grid-area: cta;
  justify-self: end;
  display: flex;
  align-items: center;
  gap: 8px;
  color: var(--Color-Text-Main);
}



.price-range {
  display: flex;
  justify-content: space-between;
  margin-top: 4px;
  color: var(--Color-Text-Dim);
}

.pagination-wrapper {
  grid-area: pagination;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0;
  margin-top: 24px;
}

.pagination-info-mobile {
  display: none;
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0;
  background: linear-gradient(to bottom, #2a3f5f 0%, #1e2329 100%);
  border-radius: 12px;
  padding: 0;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.4), 0 2px 8px rgba(0, 0, 0, 0.2);
  border: 1px solid #2a2a2a;
  overflow: hidden;
  backdrop-filter: blur(10px);
}

.pagination a {
  text-decoration: none;
  background: transparent;
  padding: 14px 18px;
  color: var(--Color-Text-Main);
  border: none;
  border-right: 1px solid rgba(255, 255, 255, 0.08);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  min-width: 52px;
  text-align: center;
  font: var(--Text-BodyMedium);
  font-weight: 500;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 4px;
  position: relative;
  overflow: hidden;
}

.pagination a::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.1), transparent);
  transition: left 0.5s ease;
}

.pagination a:last-child {
  border-right: none;
}

.pagination a:hover:not(.disabled):not(.active) {
  background: rgba(255, 255, 255, 0.12);
  color: var(--Color-Primary);
  cursor: pointer;
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
}

.pagination a:hover:not(.disabled):not(.active)::before {
  left: 100%;
}

.pagination a.active {
  background: linear-gradient(to bottom, var(--Color-Primary) 0%, #4a9dd9 100%);
  color: white;
  font-weight: 600;
  box-shadow: inset 0 2px 6px rgba(0,0,0,0.4), 0 0 0 1px rgba(255,255,255,0.1);
  transform: translateY(-1px);
}

.pagination a.disabled {
  opacity: 0.3;
  cursor: not-allowed;
  color: var(--Color-Text-Dim);
}

.pagination a.disabled:hover {
  background: transparent;
  color: var(--Color-Text-Dim);
  transform: none;
  box-shadow: none;
}

.pagination-text {
  font: var(--Text-BodySmall);
  font-weight: 500;
}

.pagination-icon {
  font-size: 14px;
  font-weight: bold;
}

.pagination-ellipsis {
  padding: 14px 18px;
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border-right: 1px solid rgba(255, 255, 255, 0.08);
  min-width: 52px;
  font-weight: 500;
}

.pagination-summary {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  text-align: center;
  padding: 12px 0;
  margin-top: 12px;
  background: rgba(42, 42, 42, 0.6);
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(5px);
  font-weight: 500;
}

/* Responsive Design */
@media (max-width: 768px) {
  .pagination-info-mobile {
    display: block;
  }
  
  .pagination-summary {
    display: none;
  }
  
  .pagination a .pagination-text {
    display: none;
  }
  
  .pagination a {
    padding: 12px 14px;
    min-width: 48px;
  }
  
  .pagination {
    gap: 0;
    margin: 0 16px;
  }
  
  .pagination-wrapper {
    margin-top: 16px;
  }
}

.tag.menuitem.selected {
  background-color: var(--Color-Primary);
  color: white;
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

.owned-status {
  display: flex;
  align-items: center;
  justify-content: center;
}

@media (max-width: 1200px) {
  .browser-wrapper {
    padding: 0 16px;
    grid-template-areas: 
    "group group group"
    "filter filter filter"
    "results results results"
    "pagination pagination pagination";
    grid-template-columns: 1fr;
  }
  
  .filter {
    margin-bottom: 16px;
  }
}
</style>
