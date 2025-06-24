<template>
  <div class="admin-game-list">
    <div class="page-header">
      <h1>🎮 Game Management</h1>
      <p>Manage game listings and detailed information</p>
    </div>
    
    <div class="admin-menu">
      <router-link to="/admin/dashboard" class="menu-item" active-class="active">
        📊 Dashboard
      </router-link>
      <router-link to="/admin/games" class="menu-item" active-class="active">
        🎮 Manage Games
      </router-link>
      <router-link to="/admin/tags" class="menu-item" active-class="active">
        🏷️ Manage Tags
      </router-link>
    </div>
    
    <!-- Game Statistics -->
    <div class="game-stats">
      <div class="stat-card">
        <div class="stat-number">{{ totalGames }}</div>
        <div class="stat-label">Total Games</div>
      </div>
      <div class="stat-card featured-stats">
        <div class="stat-number">{{ featuredGamesCount }}</div>
        <div class="stat-label">Featured Games</div>
        <div class="stat-percentage">({{ featuredPercentage }}%)</div>
      </div>
      <div class="stat-card">
        <div class="stat-number">{{ totalGames - featuredGamesCount }}</div>
        <div class="stat-label">Regular Games</div>
      </div>
    </div>
    
    <div class="game-controls">
      <div class="search-container">
        <div class="search-box">
          <input 
            type="text" 
            v-model="searchQuery" 
            placeholder="🔍 Search games by title..." 
            @input="filterGames"
            class="search-input"
          />
          <button v-if="searchQuery" @click="clearSearch" class="clear-button">✕</button>
        </div>
        <div class="filter-container">
          <select v-model="featuredFilter" @change="filterGames" class="filter-select">
            <option value="all">All games</option>
            <option value="featured">Featured only</option>
            <option value="normal">Regular only</option>
          </select>
        </div>
      </div>
      <button class="add-button" @click="goToAddGame">
        <span class="button-icon">+</span>
        Add New Game
      </button>
    </div>
    
    <div class="games-container">
      <div v-if="loading" class="loading-state">
        <div class="loading-spinner"></div>
        <p>Loading game list...</p>
      </div>
      
      <div v-else-if="filteredGames.length === 0" class="empty-state">
        <div class="empty-icon">🎮</div>
        <h3>No games found</h3>
        <p v-if="searchQuery">
          No games match the keyword "{{ searchQuery }}"
          <button @click="clearSearch" class="clear-search">Clear search</button>
        </p>
        <p v-else>No games in the system yet</p>
      </div>
      
      <div v-else class="games-grid">
        <div v-for="game in filteredGames" :key="game.id" class="game-card">
          <div class="game-image">
            <img 
              :src="getGameImage(game)" 
              :alt="`${game.title} banner`" 
              class="game-thumbnail"
              @error="handleImageError"
            >
            <div class="image-overlay">
              <div class="overlay-content">
                <span class="play-icon">▶</span>
              </div>
            </div>
            <div v-if="game.isFeatured" class="featured-badge">⭐ Featured</div>
          </div>
          
          <div class="game-content">
            <div class="game-header">
              <h3 class="game-title">{{ game.title }}</h3>
              <div class="game-price">${{ game.price.toFixed(2) }}</div>
            </div>
            
            <div class="game-info">
              <div class="info-item">
                <span class="info-label">Release:</span>
                <span class="info-value">{{ formatDate(game.releaseDate) }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">Developer:</span>
                <span class="info-value">{{ game.developer || 'N/A' }}</span>
              </div>
            </div>
            
            <div class="game-description">
              {{ truncateText(game.gameInfo, 100) }}
            </div>
          </div>
          
          <div class="game-actions">
            <button 
              class="action-btn feature-btn" 
              :class="{ featured: game.isFeatured }"
              @click="toggleFeatured(game)"
              :title="game.isFeatured ? 'Remove from featured' : 'Set as featured'"
            >
              <span class="btn-icon">{{ game.isFeatured ? '⭐' : '☆' }}</span>
              {{ game.isFeatured ? 'Featured' : 'Regular' }}
            </button>
            <button class="action-btn edit-btn" @click="editGame(game.id)">
              <span class="btn-icon">✏️</span>
              Edit
            </button>
            <button class="action-btn delete-btn" @click="deleteGame(game.id)">
              <span class="btn-icon">🗑️</span>
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Pagination -->
    <div v-if="totalPages > 1" class="pagination">
      <button 
        :disabled="currentPage === 1" 
        @click="changePage(currentPage - 1)"
        class="pagination-btn"
      >
        <span class="btn-icon">⬅️</span>
        Previous
      </button>
      <div class="page-info">
        <span class="page-current">{{ currentPage }}</span>
        <span class="page-separator">/</span>
        <span class="page-total">{{ totalPages }}</span>
      </div>
      <button 
        :disabled="currentPage === totalPages" 
        @click="changePage(currentPage + 1)"
        class="pagination-btn"
      >
        Next
        <span class="btn-icon">➡️</span>
      </button>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      games: [],
      filteredGames: [],
      searchQuery: '',
      featuredFilter: 'all',
      currentPage: 1,
      pageSize: 12,
      totalPages: 1,
      loading: true,
      allGames: [], // Store all games for statistics
      totalFeaturedGames: 0,
      totalGames: 0
    };
  },
  computed: {
    featuredGamesCount() {
      return this.totalFeaturedGames;
    },
    featuredPercentage() {
      if (this.totalGames === 0) return 0;
      return Math.round((this.totalFeaturedGames / this.totalGames) * 100);
    }
  },
  async mounted() {
    // Load stats first, then games
    await this.refreshAllData();
  },
  methods: {
    async fetchGames() {
      this.loading = true;
      try {
        const token = localStorage.getItem('token');
        
        // Build query parameters
        let queryParams = new URLSearchParams({
          pageNumber: this.currentPage,
          pageSize: this.pageSize
        });
        
        // Add search filter
        if (this.searchQuery) {
          queryParams.append('title', this.searchQuery);
        }
        
        // Add featured filter
        if (this.featuredFilter === 'featured') {
          queryParams.append('isFeatured', 'true');
        } else if (this.featuredFilter === 'normal') {
          queryParams.append('isFeatured', 'false');
        }
        
        const response = await fetch(`http://localhost:5174/api/games?${queryParams.toString()}`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (!response.ok) {
          throw new Error('Failed to fetch games');
        }
        
        const data = await response.json();
        
        // Normalize property names for consistency
        data.items.forEach(game => {
          // Handle both camelCase (isFeatured) and PascalCase (IsFeatured) from server
          if (game.IsFeatured !== undefined && game.isFeatured === undefined) {
            game.isFeatured = game.IsFeatured;
          }
          // Ensure boolean type
          game.isFeatured = Boolean(game.isFeatured);
        });
        
        this.games = data.items;
        this.totalPages = data.totalPages;
        
        // Enrich games with images like GameLibrary
        await this.enrichGamesWithImages(this.games);
        
        // Since filtering is now done on backend, we just use all games
        this.filteredGames = [...this.games];
      } catch (error) {
        console.error('Error fetching games:', error);
      } finally {
        this.loading = false;
      }
    },
    
    async fetchAllGamesForStats() {
      try {
        const token = localStorage.getItem('token');
        const response = await fetch('http://localhost:5174/api/games/stats', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (response.ok) {
          const stats = await response.json();
          this.totalFeaturedGames = stats.featuredGames;
          this.totalGames = stats.totalGames;
        }
      } catch (error) {
        console.error('Error fetching game stats:', error);
        // Fallback to old method if stats endpoint fails
        try {
          const response = await fetch('http://localhost:5174/api/games?pageSize=1000', {
            headers: {
              'Authorization': `Bearer ${token}`
            }
          });
          
          if (response.ok) {
            const data = await response.json();
            this.allGames = data.items;
            this.totalFeaturedGames = this.allGames.filter(game => game.isFeatured).length;
            this.totalGames = data.items.length;
          }
        } catch (fallbackError) {
          console.error('Error with fallback stats method:', fallbackError);
        }
      }
    },
    
    async enrichGamesWithImages(games) {
      const imagePromises = games.map(async (game) => {
        if (!game.imageUrls || game.imageUrls.length === 0) {
          try {
            const token = localStorage.getItem('token');
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), 3000);
            
            const imgRes = await fetch(`http://localhost:5174/api/games/${game.id}/images`, {
              headers: {
                'Authorization': `Bearer ${token}`
              },
              signal: controller.signal
            });
            
            clearTimeout(timeoutId);
            
            if (imgRes.ok) {
              const imgs = await imgRes.json();
              game.imageUrl = imgs[0]?.url || 'https://via.placeholder.com/350x200?text=Game+Image';
              game.imageUrls = imgs;
            } else {
              game.imageUrl = 'https://via.placeholder.com/350x200?text=Game+Image';
            }
          } catch (error) {
            if (error.name === 'AbortError') {
              console.warn(`Image fetch timeout for game ${game.id}`);
            } else {
              console.error(`Error fetching images for game ${game.id}:`, error);
            }
            game.imageUrl = 'https://via.placeholder.com/350x200?text=Game+Image';
          }
        } else {
          // Game already has imageUrls, use the first one
          game.imageUrl = game.imageUrls[0]?.url || 'https://via.placeholder.com/350x200?text=Game+Image';
        }
        return game;
      });
      
      await Promise.allSettled(imagePromises);
      return games;
    },
    filterGames() {
      // Reset to page 1 when filter changes
      this.currentPage = 1;
      // Fetch from backend with new filters
      this.fetchGames();
    },
    getGameImage(game) {
      // Use imageUrl if available (from enrichGamesWithImages)
      if (game.imageUrl) {
        return game.imageUrl;
      }
      
      // Fallback to imageUrls array
      if (game.imageUrls && game.imageUrls.length > 0 && game.imageUrls[0].url) {
        return game.imageUrls[0].url;
      }
      
      return 'https://via.placeholder.com/350x200?text=Game+Image';
    },
    formatDate(dateStr) {
      const date = new Date(dateStr);
      return date.toLocaleDateString();
    },
    goToAddGame() {
      this.$router.push('/admin/games/add');
    },
    editGame(gameId) {
      this.$router.push(`/admin/games/edit/${gameId}`);
    },
    async deleteGame(gameId) {
      const game = this.games.find(g => g.id === gameId);
      const gameName = game ? game.title : 'game này';
      const wasFeatured = game ? game.isFeatured : false;
      
      if (!confirm(`Are you sure you want to delete "${gameName}"? This action cannot be undone.`)) return;
      
      try {
        const token = localStorage.getItem('token');
        const response = await fetch(`http://localhost:5174/api/games/${gameId}`, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (!response.ok) {
          throw new Error('Failed to delete game');
        }
        
        // Update featured count if the deleted game was featured
        if (wasFeatured) {
          this.totalFeaturedGames--;
        }
        
        // Refresh the game list and stats
        await this.refreshAllData();
        
        // Show success message
        this.showSuccessMessage(`Game "${gameName}" has been deleted successfully`);
        
      } catch (error) {
        console.error('Error deleting game:', error);
        alert('Unable to delete game. Please try again.');
      }
    },
    changePage(page) {
      this.currentPage = page;
      this.fetchGames();
    },
    clearSearch() {
      this.searchQuery = '';
      this.featuredFilter = 'all';
      this.currentPage = 1;
      this.fetchGames();
    },
    truncateText(text, maxLength) {
      if (!text) return 'No description available';
      if (text.length <= maxLength) return text;
      return text.substring(0, maxLength) + '...';
    },
    handleImageError(event) {
      // Set fallback image when image fails to load
      event.target.src = 'https://via.placeholder.com/350x200?text=Game+Image';
    },
    
    async toggleFeatured(game) {
      console.log('Toggle Featured clicked for:', game.title, 'Current status:', game.isFeatured);
      
      const newFeaturedStatus = !game.isFeatured;
      const actionText = newFeaturedStatus ? 'feature' : 'unfeature';
      
      if (!confirm(`Are you sure you want to ${actionText} the game "${game.title}"?`)) {
        return;
      }
      
      try {
        const token = localStorage.getItem('token');
        
        console.log('Sending PUT request with payload:', {
          title: game.title,
          gameInfo: game.gameInfo,
          price: game.price,
          releaseDate: game.releaseDate,
          developer: game.developer,
          isFeatured: newFeaturedStatus
        });
        
        const response = await fetch(`http://localhost:5174/api/games/${game.id}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          },
          body: JSON.stringify({
            title: game.title,
            gameInfo: game.gameInfo,
            price: game.price,
            releaseDate: game.releaseDate,
            developer: game.developer,
            isFeatured: newFeaturedStatus
          })
        });
        
        if (!response.ok) {
          const errorText = await response.text();
          console.error('Server error:', errorText);
          throw new Error(`Failed to update game: ${response.status}`);
        }
        
        const updatedGame = await response.json();
        console.log('Server response:', updatedGame);
        
        // Normalize property name from server response
        if (updatedGame.IsFeatured !== undefined && updatedGame.isFeatured === undefined) {
          updatedGame.isFeatured = updatedGame.IsFeatured;
        }
        updatedGame.isFeatured = Boolean(updatedGame.isFeatured);
        
        // Update local game object with server response
        game.isFeatured = updatedGame.isFeatured;
        
        // Force reactivity update
        this.$forceUpdate();
        
        // Update local stats
        if (newFeaturedStatus) {
          this.totalFeaturedGames++;
        } else {
          this.totalFeaturedGames--;
        }
        
        // Refresh stats to ensure accuracy
        await this.fetchAllGamesForStats();
        
        // Show success message
        const successMessage = `Game "${game.title}" has been ${actionText}d successfully`;
        this.showSuccessMessage(successMessage);
        
        console.log('Toggle completed successfully. New status:', game.isFeatured);
        
      } catch (error) {
        console.error('Error updating game featured status:', error);
        alert(`Unable to ${actionText} game. Error: ${error.message}`);
        
        // No need to revert since we update from server response
      }
    },
    
    showSuccessMessage(message) {
      // Create a temporary success notification
      const notification = document.createElement('div');
      notification.className = 'success-notification';
      notification.innerHTML = `
        <span class="success-icon">✅</span>
        ${message}
      `;
      
      // Add to page
      document.body.appendChild(notification);
      
      // Auto remove after 3 seconds
      setTimeout(() => {
        if (notification.parentNode) {
          notification.parentNode.removeChild(notification);
        }
      }, 3000);
    },

    async refreshAllData() {
      await Promise.all([
        this.fetchAllGamesForStats(),
        this.fetchGames()
      ]);
    }
  }
};
</script>

<style scoped>
.admin-game-list {
  padding: 2rem;
  color: var(--Color-Text-Main);
  background: var(--Gradient-Background-System);
  min-height: 100vh;
  max-width: 1400px;
  margin: 0 auto;
}

/* Page Header */
.page-header {
  margin-bottom: 2rem;
}

.page-header h1 {
  color: var(--Color-Text-Main);
  margin-bottom: 0.5rem;
  font: var(--Text-HeadingLarge);
}

.page-header p {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
  margin: 0;
}

/* Admin Menu */
.admin-menu {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  background-color: var(--Color-Background-Highlight);
  border-radius: 12px;
  padding: 1rem;
  box-shadow: var(--ShadowLight);
}

.menu-item {
  padding: 0.75rem 1.5rem;
  color: var(--Color-Text-Main);
  text-decoration: none;
  border-radius: 8px;
  transition: all 0.3s ease;
  font-weight: 600;
}

.menu-item:hover {
  background-color: var(--Color-Background-System);
  transform: translateY(-2px);
}

.menu-item.active {
  background-color: var(--Color-Primary);
  color: white;
  box-shadow: var(--ShadowLight);
}

/* Game Statistics */
.game-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: var(--Color-Background-Highlight);
  padding: 1.5rem;
  border-radius: 12px;
  box-shadow: var(--ShadowLight);
  text-align: center;
  transition: all 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--ShadowHeavy);
}

.stat-number {
  display: block;
  color: var(--Color-Primary);
  font: var(--Text-HeadingLarge);
  font-weight: 700;
  margin-bottom: 0.5rem;
  font-size: 2rem;
}

.stat-label {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.stat-percentage {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  font-weight: 500;
  margin-top: 0.25rem;
}

.featured-stats {
  background: linear-gradient(135deg, var(--Color-Background-Highlight), rgba(var(--Color-Primary-RGB), 0.1));
  border: 1px solid rgba(var(--Color-Primary-RGB), 0.2);
}

/* Success Notification */
:global(.success-notification) {
  position: fixed;
  top: 2rem;
  right: 2rem;
  background: #10b981;
  color: white;
  padding: 1rem 1.5rem;
  border-radius: 8px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  z-index: 1000;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  animation: slideInRight 0.3s ease-out;
}

:global(.success-notification .success-icon) {
  font-size: 1.2rem;
}

@keyframes slideInRight {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

/* Game Controls */
.game-controls {
  margin-bottom: 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
}

.search-container {
  flex: 1;
  max-width: 600px;
  display: flex;
  gap: 1rem;
  align-items: center;
}

.search-box {
  position: relative;
  display: flex;
  align-items: center;
  flex: 1;
}

.filter-container {
  min-width: 150px;
}

.filter-select {
  width: 100%;
  padding: 0.75rem 1rem;
  background: var(--Color-Background-Highlight);
  border: 2px solid transparent;
  border-radius: 8px;
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.filter-select:focus {
  outline: none;
  border-color: var(--Color-Primary);
  box-shadow: 0 0 0 3px rgba(var(--Color-Primary-RGB), 0.1);
}

.filter-select:hover {
  background: var(--Color-Background-System);
}

.search-input {
  width: 100%;
  padding: 0.75rem 1rem;
  padding-right: 3rem;
  background-color: var(--Color-Background-Highlight);
  border: 2px solid transparent;
  border-radius: 12px;
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  transition: all 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: var(--Color-Primary);
  box-shadow: 0 0 0 3px rgba(var(--Color-Primary-RGB), 0.1);
}

.clear-button {
  position: absolute;
  right: 0.75rem;
  background: none;
  border: none;
  color: var(--Color-Text-Dim);
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 50%;
  transition: all 0.2s ease;
}

.clear-button:hover {
  background: var(--Color-Background-System);
  color: var(--Color-Text-Main);
}

.add-button {
  background: var(--Gradient-Button-Action);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 12px;
  cursor: pointer;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.3s ease;
  box-shadow: var(--ShadowLight);
}

.add-button:hover {
  background: var(--Color-Primary);
  transform: translateY(-2px);
  box-shadow: var(--ShadowHeavy);
}

.button-icon {
  font-size: 1.2rem;
  font-weight: 700;
}

/* Games Container */
.games-container {
  background: var(--Color-Background-Highlight);
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: var(--ShadowLight);
  margin-bottom: 2rem;
}

/* Loading State */
.loading-state {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--Color-Text-Dim);
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid var(--Color-Background-System);
  border-top: 4px solid var(--Color-Primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--Color-Text-Dim);
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.empty-state h3 {
  color: var(--Color-Text-Main);
  margin-bottom: 1rem;
  font: var(--Text-HeadingMedium);
}

.clear-search {
  background: var(--Color-Primary);
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  cursor: pointer;
  margin-left: 0.5rem;
  transition: all 0.2s ease;
}

.clear-search:hover {
  background: var(--Color-Secondary);
}

/* Games Grid */
.games-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
}

/* Game Card */
.game-card {
  background: var(--Color-Background-System);
  border-radius: 16px;
  overflow: hidden;
  transition: all 0.3s ease;
  box-shadow: var(--ShadowLight);
}

.game-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--ShadowHeavy);
}

.game-image {
  position: relative;
  height: 200px;
  overflow: hidden;
  border-radius: 8px 8px 0 0;
}

.game-thumbnail {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
  display: block;
}

.game-card:hover .game-thumbnail {
  transform: scale(1.05);
}

.image-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.game-card:hover .image-overlay {
  opacity: 1;
}

.overlay-content {
  text-align: center;
  color: white;
}

.play-icon {
  font-size: 2rem;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  width: 60px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

.game-card:hover .play-icon {
  background: rgba(255, 255, 255, 0.3);
  transform: scale(1.1);
}

.featured-badge {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: var(--Color-Primary);
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font: var(--Text-BodySmall);
  font-weight: 600;
  box-shadow: var(--ShadowLight);
}

.game-content {
  padding: 1.5rem;
}

.game-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
  gap: 1rem;
}

.game-title {
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingMedium);
  font-weight: 600;
  margin: 0;
  flex: 1;
  line-height: 1.3;
}

.game-price {
  color: var(--Color-Primary);
  font: var(--Text-BodyLarge);
  font-weight: 700;
  background: var(--Color-Background-Highlight);
  padding: 0.5rem 1rem;
  border-radius: 8px;
  flex-shrink: 0;
}

.game-info {
  margin-bottom: 1rem;
}

.info-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.info-label {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  font-weight: 600;
}

.info-value {
  color: var(--Color-Text-Main);
  font: var(--Text-BodySmall);
  text-align: right;
}

.game-description {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  line-height: 1.5;
  margin-bottom: 1.5rem;
}

/* Game Actions */
.game-actions {
  display: flex;
  gap: 0.5rem;
  padding: 0 1.5rem 1.5rem;
  flex-wrap: wrap;
}

.action-btn {
  flex: 1;
  padding: 0.6rem 0.8rem;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
  transition: all 0.2s ease;
  font-size: 0.85rem;
  min-width: 80px;
}

.edit-btn {
  background: var(--Color-Secondary);
  color: white;
}

.edit-btn:hover {
  background: var(--Color-Primary);
  transform: translateY(-2px);
}

.delete-btn {
  background: var(--Color-Accent-Red);
  color: white;
}

.delete-btn:hover {
  background: #dc2626;
  transform: translateY(-2px);
}

.btn-icon {
  font-size: 0.9rem;
}

.feature-btn {
  background: var(--Color-Background-System);
  color: var(--Color-Text-Main);
  border: 2px solid var(--Color-Background-Highlight);
}

.feature-btn:hover {
  background: var(--Color-Background-Highlight);
  transform: translateY(-2px);
}

.feature-btn.featured {
  background: linear-gradient(135deg, #ffd700, #ffa500);
  color: #000;
  border-color: #ffd700;
  box-shadow: 0 0 10px rgba(255, 215, 0, 0.3);
}

.feature-btn.featured:hover {
  background: linear-gradient(135deg, #ffed4e, #ff8c00);
  transform: translateY(-2px);
  box-shadow: 0 0 15px rgba(255, 215, 0, 0.4);
}

/* Pagination */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 2rem;
}

.pagination-btn {
  background: var(--Color-Background-Highlight);
  color: var(--Color-Text-Main);
  border: none;
  border-radius: 8px;
  padding: 0.75rem 1.5rem;
  cursor: pointer;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s ease;
  box-shadow: var(--ShadowLight);
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-btn:not(:disabled):hover {
  background: var(--Color-Primary);
  color: white;
  transform: translateY(-2px);
}

.page-info {
  background: var(--Color-Background-Highlight);
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  box-shadow: var(--ShadowLight);
}

.page-current {
  color: var(--Color-Primary);
  font-weight: 700;
  font-size: 1.1rem;
}

.page-separator {
  color: var(--Color-Text-Dim);
}

.page-total {
  color: var(--Color-Text-Main);
  font-weight: 600;
}

/* Responsive Design */
@media (max-width: 1024px) {
  .games-grid {
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  }
}

@media (max-width: 768px) {
  .admin-game-list {
    padding: 1rem;
  }
  
  .admin-menu {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .game-stats {
    grid-template-columns: 1fr 1fr;
  }
  
  .game-controls {
    flex-direction: column;
    align-items: stretch;
  }
  
  .search-container {
    max-width: none;
    margin-bottom: 1rem;
    flex-direction: column;
    align-items: stretch;
  }
  
  .filter-container {
    min-width: auto;
  }
  
  .games-grid {
    grid-template-columns: 1fr;
  }
  
  .game-header {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .game-price {
    align-self: flex-start;
  }
  
  .pagination {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .pagination-btn {
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 480px) {
  .game-stats {
    grid-template-columns: 1fr;
  }
  
  .stat-number {
    font-size: 1.5rem;
  }
  
  .game-actions {
    flex-direction: column;
  }
}

/* Success Notification */
:global(.success-notification) {
  position: fixed;
  top: 2rem;
  right: 2rem;
  background: linear-gradient(135deg, #10b981, #059669);
  color: white;
  padding: 1rem 1.5rem;
  border-radius: 12px;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  z-index: 1000;
  animation: slideInFromRight 0.5s ease-out;
}

:global(.success-notification .success-icon) {
  font-size: 1.2rem;
}

@keyframes slideInFromRight {
  from { 
    opacity: 0; 
    transform: translateX(100%); 
  }
  to { 
    opacity: 1; 
    transform: translateX(0); 
  }
}
</style> 