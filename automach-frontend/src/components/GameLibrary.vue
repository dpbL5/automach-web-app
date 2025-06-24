<template>
  <div class="library-container">
    <div class="sidebar">
      <h3 class="sidebar-title">Thư viện game của bạn</h3>
      
      <!-- Search Box -->
      <div class="search-container">
        <div class="search-input-wrapper">
          <svg class="search-icon" width="16" height="16" viewBox="0 0 24 24" fill="none">
            <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          <input 
            v-model="searchQuery" 
            class="search-input" 
            placeholder="Tìm kiếm game..."
            @input="filterGames"
          />
          <button v-if="searchQuery" @click="clearSearch" class="clear-search">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none">
              <path d="M18 6L6 18M6 6l12 12" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
            </svg>
          </button>
        </div>
      </div>

      <!-- Filter Options -->
      <div class="filter-section">
        <h4 class="filter-title">Sắp xếp</h4>
        
        <!-- Sort Options -->
        <div class="filter-group">
          <select v-model="sortBy" @change="filterGames" class="filter-select">
            <option value="title-asc">Tên A-Z</option>
            <option value="title-desc">Tên Z-A</option>
            <option value="recent">Gần đây nhất</option>
            <option value="oldest">Cũ nhất</option>
          </select>
        </div>
      </div>

      <!-- Game List in Sidebar -->
      <div class="sidebar-games">
        <div 
          v-for="game in filteredGames" 
          :key="game.id" 
          class="sidebar-item"
          :class="{ active: selectedGameId === game.id }"
          @click="navigateToGame(game.id)"
        >
          <img :src="game.imageUrl" class="sidebar-img" />
          <span class="sidebar-text">{{ game.title }}</span>
        </div>
        
        <div v-if="filteredGames.length === 0" class="no-results">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" class="no-results-icon">
            <path d="M9.5 16a6.5 6.5 0 100-13 6.5 6.5 0 000 13zM21 21l-4.35-4.35" stroke="currentColor" stroke-width="2"/>
          </svg>
          <p>Không tìm thấy game nào</p>
        </div>
      </div>
    </div>
    
    <div class="main-content">
      <div class="content-header">
        <div class="header-left">
          <h2 class="library-title">Thư viện game</h2>
          <p class="game-count">{{ filteredGames.length }} / {{ games.length }} game</p>
        </div>
        
        <div class="header-controls">
          <!-- View Mode Toggle -->
          <div class="view-toggle">
            <button 
              :class="['view-btn', { active: viewMode === 'grid' }]"
              @click="viewMode = 'grid'"
            >
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
                <rect x="3" y="3" width="7" height="7" stroke="currentColor" stroke-width="2"/>
                <rect x="14" y="3" width="7" height="7" stroke="currentColor" stroke-width="2"/>
                <rect x="3" y="14" width="7" height="7" stroke="currentColor" stroke-width="2"/>
                <rect x="14" y="14" width="7" height="7" stroke="currentColor" stroke-width="2"/>
              </svg>
            </button>
            <button 
              :class="['view-btn', { active: viewMode === 'list' }]"
              @click="viewMode = 'list'"
            >
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
                <line x1="8" y1="6" x2="21" y2="6" stroke="currentColor" stroke-width="2"/>
                <line x1="8" y1="12" x2="21" y2="12" stroke="currentColor" stroke-width="2"/>
                <line x1="8" y1="18" x2="21" y2="18" stroke="currentColor" stroke-width="2"/>
                <line x1="3" y1="6" x2="3.01" y2="6" stroke="currentColor" stroke-width="2"/>
                <line x1="3" y1="12" x2="3.01" y2="12" stroke="currentColor" stroke-width="2"/>
                <line x1="3" y1="18" x2="3.01" y2="18" stroke="currentColor" stroke-width="2"/>
              </svg>
            </button>
          </div>
        </div>
      </div>

      <!-- Grid View -->
      <div v-if="viewMode === 'grid'" class="game-grid">
        <div 
          v-for="game in filteredGames" 
          :key="game.id" 
          class="game-card"
          @click="navigateToGame(game.id)"
        >
          <div class="game-img-container">
            <img :src="game.imageUrl" class="game-img" />
            <div class="play-overlay">
              <div class="play-button">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
                  <path d="M8 5v14l11-7z" fill="currentColor"/>
                </svg>
              </div>
            </div>
          </div>
          <div class="game-info">
            <div class="game-title">{{ game.title }}</div>
            <div class="game-subtitle">Đã sở hữu</div>
            <div v-if="game.tags && game.tags.length" class="game-tags">
              <span v-for="tag in game.tags.slice(0, 2)" :key="tag" class="game-tag">
                {{ tag }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- List View -->
      <div v-else class="game-list">
        <div 
          v-for="game in filteredGames" 
          :key="game.id" 
          class="game-list-item"
          @click="navigateToGame(game.id)"
        >
          <img :src="game.imageUrl" class="list-game-img" />
          <div class="list-game-info">
            <div class="list-game-title">{{ game.title }}</div>
            <div class="list-game-subtitle">Đã sở hữu</div>
            <div v-if="game.tags && game.tags.length" class="list-game-tags">
              <span v-for="tag in game.tags.slice(0, 3)" :key="tag" class="game-tag">
                {{ tag }}
              </span>
            </div>
          </div>
          <div class="list-game-actions">
            <button class="action-btn">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
                <path d="M8 5v14l11-7z" fill="currentColor"/>
              </svg>
              Chơi
            </button>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="filteredGames.length === 0" class="empty-state">
        <svg width="80" height="80" viewBox="0 0 24 24" fill="none" class="empty-icon">
          <path d="M9.5 16a6.5 6.5 0 100-13 6.5 6.5 0 000 13zM21 21l-4.35-4.35" stroke="currentColor" stroke-width="1.5"/>
        </svg>
        <h3>Không tìm thấy game nào</h3>
        <p>Thử thay đổi từ khóa tìm kiếm</p>
        <button @click="clearAllFilters" class="clear-filters-btn">
          Xóa tìm kiếm
        </button>
      </div>
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
      sortBy: 'title-asc',
      viewMode: 'grid', // 'grid' or 'list'
      selectedGameId: null
    };
  },
  methods: {
    navigateToGame(gameId) {
      this.selectedGameId = gameId;
      this.$router.push(`/game/${gameId}`);
    },
    
    filterGames() {
      let filtered = [...this.games];
      
      // Search filter
      if (this.searchQuery) {
        const query = this.searchQuery.toLowerCase();
        filtered = filtered.filter(game => 
          game.title.toLowerCase().includes(query) ||
          (game.tags && game.tags.some(tag => tag.toLowerCase().includes(query)))
        );
      }
      
      // Sort
      filtered.sort((a, b) => {
        switch (this.sortBy) {
          case 'title-asc':
            return a.title.localeCompare(b.title);
          case 'title-desc':
            return b.title.localeCompare(a.title);
          case 'recent':
            return new Date(b.dateAdded || 0) - new Date(a.dateAdded || 0);
          case 'oldest':
            return new Date(a.dateAdded || 0) - new Date(b.dateAdded || 0);
          default:
            return 0;
        }
      });
      
      this.filteredGames = filtered;
    },
    
    clearSearch() {
      this.searchQuery = '';
      this.filterGames();
    },
    
    clearAllFilters() {
      this.searchQuery = '';
      this.sortBy = 'title-asc';
      this.filterGames();
    }
  },
  async mounted() {
    const token = localStorage.getItem('token');
    const userId = localStorage.getItem('userId');
    if (!token || !userId) {
      this.$router.push('/login');
      return;
    }
    
    // Lấy danh sách game đã sở hữu
    const res = await fetch(`http://localhost:5174/api/accounts/${userId}/owned-games`, {
      headers: { Authorization: `Bearer ${token}` }
    });
    let games = [];
    if (res.ok) {
      games = await res.json();
    }
    
    // Lấy ảnh và tags cho từng game
    for (let g of games) {
      // Get images
      const imgRes = await fetch(`http://localhost:5174/api/games/${g.id}/images`);
      if (imgRes.ok) {
        const imgs = await imgRes.json();
        g.imageUrl = imgs[0]?.url || 'https://via.placeholder.com/150x200?text=Game';
      } else {
        g.imageUrl = 'https://via.placeholder.com/150x200?text=Game';
      }
      
      // Get tags/genres (giả sử API có endpoint này)
      try {
        const tagsRes = await fetch(`http://localhost:5174/api/games/${g.id}/tags`);
        if (tagsRes.ok) {
          const tagsData = await tagsRes.json();
          g.tags = tagsData.map(tag => tag.name || tag.tagName);
        } else {
          g.tags = [];
        }
      } catch (error) {
        g.tags = [];
      }
      
      // Add some mock data for sorting
      g.dateAdded = new Date();
    }
    
    this.games = games;
    this.filterGames();
  }
};
</script>

<style scoped>
.library-container {
  display: flex;
  min-height: 100vh;
  background: var(--Gradient-Background-Main);
}

.sidebar {
  width: 320px;
  background: linear-gradient(135deg, #1a1d29 0%, #2a2d3a 100%);
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 20px;
  border-right: 1px solid rgba(255, 255, 255, 0.1);
  box-shadow: 2px 0 20px rgba(0, 0, 0, 0.3);
  overflow-y: auto;
}

.sidebar-title {
  color: #fff;
  font-size: 18px;
  font-weight: 600;
  margin: 0;
  padding-bottom: 16px;
  border-bottom: 2px solid rgba(255, 255, 255, 0.1);
}

/* Search */
.search-container {
  margin-bottom: 8px;
}

.search-input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: 12px;
  color: #94a3b8;
  z-index: 1;
}

.search-input {
  width: 100%;
  padding: 12px 12px 12px 40px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 8px;
  color: #fff;
  font-size: 14px;
  transition: all 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: #4f46e5;
  background: rgba(255, 255, 255, 0.15);
}

.search-input::placeholder {
  color: #94a3b8;
}

.clear-search {
  position: absolute;
  right: 8px;
  background: none;
  border: none;
  color: #94a3b8;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: color 0.3s ease;
}

.clear-search:hover {
  color: #fff;
}

/* Filter Section */
.filter-section {
  background: rgba(255, 255, 255, 0.05);
  padding: 16px;
  border-radius: 12px;
}

.filter-title {
  color: #fff;
  font-size: 14px;
  font-weight: 600;
  margin: 0 0 12px 0;
}

.filter-group {
  margin-bottom: 0;
}

.filter-select {
  width: 100%;
  padding: 8px 12px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 6px;
  color: #fff;
  font-size: 13px;
}

.filter-select:focus {
  outline: none;
  border-color: #4f46e5;
}

.filter-select option {
  background: #2a2d3a;
  color: #fff;
}

/* Sidebar Games */
.sidebar-games {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 8px;
  max-height: 400px;
  overflow-y: auto;
}

.sidebar-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  color: #b8bcc8;
  font-weight: 500;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.sidebar-item.active {
  background: rgba(79, 70, 229, 0.2);
  color: #fff;
  border: 1px solid rgba(79, 70, 229, 0.4);
}

.sidebar-item::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  width: 0;
  height: 100%;
  background: linear-gradient(90deg, #4f46e5, #7c3aed);
  transition: width 0.3s ease;
  z-index: 0;
}

.sidebar-item:hover {
  color: #fff;
  background: rgba(255, 255, 255, 0.1);
  transform: translateX(4px);
}

.sidebar-item:hover::before {
  width: 4px;
}

.sidebar-img {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  object-fit: cover;
  position: relative;
  z-index: 1;
  transition: transform 0.3s ease;
}

.sidebar-item:hover .sidebar-img {
  transform: scale(1.1);
}

.sidebar-text {
  position: relative;
  z-index: 1;
  font-size: 14px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.no-results {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 32px 16px;
  color: #64748b;
  text-align: center;
}

.no-results-icon {
  margin-bottom: 12px;
  opacity: 0.6;
}

.no-results p {
  margin: 0;
  font-size: 14px;
}

/* Main Content */
.main-content {
  flex: 1;
  padding: 32px 48px;
}

.content-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 32px;
  gap: 20px;
}

.header-left {
  flex: 1;
}

.library-title {
  color: #fff;
  font-size: 32px;
  font-weight: 700;
  margin-bottom: 8px;
  background: linear-gradient(135deg, #fff 0%, #e2e8f0 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.game-count {
  color: #94a3b8;
  font-size: 16px;
  font-weight: 500;
  margin: 0;
}

.header-controls {
  display: flex;
  align-items: center;
}

/* View Toggle */
.view-toggle {
  display: flex;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  padding: 4px;
}

.view-btn {
  padding: 8px 12px;
  background: none;
  border: none;
  color: #94a3b8;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.view-btn.active {
  background: #4f46e5;
  color: #fff;
}

.view-btn:hover:not(.active) {
  color: #fff;
  background: rgba(255, 255, 255, 0.1);
}

/* Game Grid */
.game-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 24px;
}

.game-card {
  background: linear-gradient(135deg, #2d3748 0%, #374151 100%);
  border-radius: 16px;
  padding: 20px;
  display: flex;
  flex-direction: column;
  cursor: pointer;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid rgba(255, 255, 255, 0.1);
  position: relative;
  overflow: hidden;
}

.game-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, rgba(79, 70, 229, 0.1) 0%, rgba(124, 58, 237, 0.1) 100%);
  opacity: 0;
  transition: opacity 0.3s ease;
  z-index: 0;
}

.game-card:hover {
  transform: translateY(-8px) scale(1.02);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.4), 0 0 0 1px rgba(255, 255, 255, 0.2);
}

.game-card:hover::before {
  opacity: 1;
}

.game-img-container {
  position: relative;
  margin-bottom: 16px;
  z-index: 1;
}

.game-img {
  width: 100%;
  height: 240px;
  border-radius: 12px;
  object-fit: cover;
  transition: all 0.3s ease;
}

.game-card:hover .game-img {
  transform: scale(1.05);
  filter: brightness(1.1);
}

.play-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 12px;
  opacity: 0;
  transition: all 0.3s ease;
}

.game-card:hover .play-overlay {
  opacity: 1;
}

.play-button {
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  transition: all 0.3s ease;
  transform: scale(0.8);
}

.game-card:hover .play-button {
  transform: scale(1);
}

.game-info {
  z-index: 1;
}

.game-title {
  color: #fff;
  font-weight: 700;
  font-size: 16px;
  text-align: center;
  margin-bottom: 8px;
  line-height: 1.4;
  transition: color 0.3s ease;
}

.game-card:hover .game-title {
  color: #e2e8f0;
}

.game-subtitle {
  color: #10b981;
  font-size: 12px;
  font-weight: 600;
  text-align: center;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  opacity: 0.8;
  margin-bottom: 8px;
}

.game-tags {
  display: flex;
  justify-content: center;
  gap: 6px;
  flex-wrap: wrap;
}

.game-tag {
  background: rgba(79, 70, 229, 0.2);
  color: #a5b4fc;
  font-size: 10px;
  font-weight: 500;
  padding: 4px 8px;
  border-radius: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* List View */
.game-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.game-list-item {
  display: flex;
  align-items: center;
  gap: 16px;
  background: linear-gradient(135deg, #2d3748 0%, #374151 100%);
  border-radius: 12px;
  padding: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.game-list-item:hover {
  transform: translateX(4px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.3);
  background: linear-gradient(135deg, #374151 0%, #4b5563 100%);
}

.list-game-img {
  width: 80px;
  height: 80px;
  border-radius: 8px;
  object-fit: cover;
  flex-shrink: 0;
}

.list-game-info {
  flex: 1;
}

.list-game-title {
  color: #fff;
  font-weight: 700;
  font-size: 18px;
  margin-bottom: 4px;
}

.list-game-subtitle {
  color: #10b981;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  margin-bottom: 8px;
}

.list-game-tags {
  display: flex;
  gap: 6px;
  flex-wrap: wrap;
}

.list-game-actions {
  flex-shrink: 0;
}

.action-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%);
  border: none;
  border-radius: 8px;
  color: #fff;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.action-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(79, 70, 229, 0.4);
}

/* Empty State */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 20px;
  text-align: center;
  color: #64748b;
}

.empty-icon {
  margin-bottom: 24px;
  opacity: 0.6;
}

.empty-state h3 {
  color: #94a3b8;
  font-size: 24px;
  font-weight: 600;
  margin-bottom: 8px;
}

.empty-state p {
  font-size: 16px;
  margin-bottom: 24px;
}

.clear-filters-btn {
  padding: 12px 24px;
  background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%);
  border: none;
  border-radius: 8px;
  color: #fff;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.clear-filters-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(79, 70, 229, 0.4);
}

/* Responsive */
@media (max-width: 1024px) {
  .sidebar {
    width: 280px;
  }
  
  .game-grid {
    grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
    gap: 20px;
  }
  
  .main-content {
    padding: 24px 32px;
  }
  
  .content-header {
    flex-direction: column;
    align-items: stretch;
    gap: 16px;
  }
}

@media (max-width: 768px) {
  .library-container {
    flex-direction: column;
  }
  
  .sidebar {
    width: 100%;
    padding: 16px;
    max-height: 300px;
  }
  
  .sidebar-games {
    max-height: 200px;
  }
  
  .game-grid {
    grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
    gap: 16px;
  }
  
  .main-content {
    padding: 20px;
  }
  
  .game-img {
    height: 200px;
  }
  
  .header-controls {
    justify-content: flex-end;
  }
}
</style> 