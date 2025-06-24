<template>
  <div class="admin-game-list">
    <h1>Game Management</h1>
    
    <div class="admin-menu">
      <router-link to="/admin/dashboard" class="menu-item" active-class="active">
        Dashboard
      </router-link>
      <router-link to="/admin/games" class="menu-item" active-class="active">
        Manage Games
      </router-link>
      <router-link to="/admin/tags" class="menu-item" active-class="active">
        Manage Tags
      </router-link>
    </div>
    
    <div class="game-controls">
      <div class="search-box">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Search games..." 
          @input="filterGames"
          class="search-input"
        />
      </div>
      <button class="add-button" @click="goToAddGame">Add New Game</button>
    </div>
    
    <div class="game-table">
      <div class="game-header">
        <div class="game-cell">Image</div>
        <div class="game-cell">Title</div>
        <div class="game-cell">Price</div>
        <div class="game-cell">Release Date</div>
        <div class="game-cell">Actions</div>
      </div>
      
      <div v-if="loading" class="loading">
        Loading games...
      </div>
      
      <div v-else-if="filteredGames.length === 0" class="no-games">
        No games found. 
        <span v-if="searchQuery" @click="clearSearch" class="clear-search">Clear search</span>
      </div>
      
      <div v-else v-for="game in filteredGames" :key="game.id" class="game-row">
        <div class="game-cell">
          <img v-if="getGameImage(game)" :src="getGameImage(game)" alt="Game cover" class="game-thumbnail">
          <div v-else class="no-image">No Image</div>
        </div>
        <div class="game-cell">{{ game.title }}</div>
        <div class="game-cell">${{ game.price.toFixed(2) }}</div>
        <div class="game-cell">{{ formatDate(game.releaseDate) }}</div>
        <div class="game-cell actions">
          <button class="edit-button" @click="editGame(game.id)">Edit</button>
          <button class="delete-button" @click="deleteGame(game.id)">Delete</button>
        </div>
      </div>
    </div>
    
    <!-- Pagination -->
    <div v-if="totalPages > 1" class="pagination">
      <button 
        :disabled="currentPage === 1" 
        @click="changePage(currentPage - 1)"
        class="pagination-button"
      >
        Previous
      </button>
      <span class="page-info">Page {{ currentPage }} of {{ totalPages }}</span>
      <button 
        :disabled="currentPage === totalPages" 
        @click="changePage(currentPage + 1)"
        class="pagination-button"
      >
        Next
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
      currentPage: 1,
      pageSize: 10,
      totalPages: 1,
      loading: true
    };
  },
  async mounted() {
    await this.fetchGames();
  },
  methods: {
    async fetchGames() {
      this.loading = true;
      try {
        const token = localStorage.getItem('token');
        const response = await fetch(`http://localhost:5174/api/games?pageNumber=${this.currentPage}&pageSize=${this.pageSize}`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (!response.ok) {
          throw new Error('Failed to fetch games');
        }
        
        const data = await response.json();
        this.games = data.items;
        this.totalPages = data.totalPages;
        this.filterGames();
      } catch (error) {
        console.error('Error fetching games:', error);
      } finally {
        this.loading = false;
      }
    },
    filterGames() {
      if (!this.searchQuery) {
        this.filteredGames = this.games;
      } else {
        const query = this.searchQuery.toLowerCase();
        this.filteredGames = this.games.filter(game => 
          game.title.toLowerCase().includes(query)
        );
      }
    },
    getGameImage(game) {
      if (game.imageUrls && game.imageUrls.length > 0) {
        return game.imageUrls[0].url;
      }
      return null;
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
      if (!confirm('Are you sure you want to delete this game? This action cannot be undone.')) return;
      
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
        
        // Refresh the game list
        await this.fetchGames();
      } catch (error) {
        console.error('Error deleting game:', error);
        alert('Failed to delete game');
      }
    },
    changePage(page) {
      this.currentPage = page;
      this.fetchGames();
    },
    clearSearch() {
      this.searchQuery = '';
      this.filterGames();
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
}

h1 {
  color: var(--Color-Text-Main);
  margin-bottom: 1.5rem;
  font: var(--Text-HeadingLarge);
}

.admin-menu {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  background-color: var(--Color-Background-Highlight);
  border-radius: 8px;
  padding: 1rem;
}

.menu-item {
  padding: 0.75rem 1.5rem;
  color: var(--Color-Text-Main);
  text-decoration: none;
  border-radius: 4px;
  transition: background-color 0.3s;
}

.menu-item:hover {
  background-color: var(--Color-Background-Main);
}

.menu-item.active {
  background-color: var(--Color-Primary);
  color: white;
}

.game-controls {
  margin-bottom: 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.search-box {
  flex: 1;
  max-width: 300px;
}

.search-input {
  width: 100%;
  padding: 0.5rem;
  background-color: var(--Color-Background-System);
  border: none;
  border-radius: 4px;
  color: var(--Color-Text-Main);
}

.add-button {
  background: var(--Gradient-Button-Action);
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
}

.add-button:hover {
  background: var(--Color-Primary);
}

.game-table {
  background-color: var(--Color-Background-Main);
  border-radius: 8px;
  overflow: hidden;
  box-shadow: var(--ShadowLight);
  margin-bottom: 1rem;
}

.game-header {
  display: grid;
  grid-template-columns: 100px 2fr 1fr 1fr 1fr;
  background-color: var(--Color-Background-Highlight);
  padding: 1rem;
  font-weight: bold;
}

.game-row {
  display: grid;
  grid-template-columns: 100px 2fr 1fr 1fr 1fr;
  padding: 1rem;
  border-bottom: 1px solid var(--Color-Background-Highlight);
  align-items: center;
}

.game-row:nth-child(even) {
  background-color: var(--Color-Background-Highlight);
}

.game-cell {
  display: flex;
  align-items: center;
}

.game-thumbnail {
  width: 80px;
  height: 45px;
  object-fit: cover;
  border-radius: 4px;
}

.no-image {
  width: 80px;
  height: 45px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: var(--Color-Background-System);
  border-radius: 4px;
  font-size: 0.8rem;
}

.actions {
  display: flex;
  gap: 0.5rem;
}

.edit-button, .delete-button {
  padding: 0.4rem 0.8rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
}

.edit-button {
  background-color: var(--Color-Secondary);
  color: white;
}

.edit-button:hover {
  background-color: var(--Color-Primary);
}

.delete-button {
  background-color: var(--Color-Accent-Red);
  color: white;
}

.delete-button:hover {
  background-color: var(--Color-Accent-Red-Hover);
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 1rem;
}

.pagination-button {
  background-color: var(--Color-Background-System);
  color: white;
  border: none;
  border-radius: 4px;
  padding: 0.5rem 1rem;
  cursor: pointer;
}

.pagination-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-button:not(:disabled):hover {
  background-color: var(--Color-Secondary);
}

.page-info {
  color: var(--Color-Text-Main);
}

.loading, .no-games {
  padding: 2rem;
  text-align: center;
  color: var(--Color-Text-Dim);
}

.clear-search {
  color: var(--Color-Primary);
  cursor: pointer;
  text-decoration: underline;
}
</style> 