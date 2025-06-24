<template>
  <div class="admin-tag-list">
    <div class="page-header">
      <h1>🏷️ Tag Management</h1>
      <p>Manage tags and game categorization</p>
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
    
    <div v-if="successMessage" class="success-message">
      <span class="success-icon">✅</span>
      {{ successMessage }}
    </div>
    
    <!-- Tag Statistics -->
    <div class="tag-stats">
      <div class="stat-card">
        <div class="stat-number">{{ tags.length }}</div>
        <div class="stat-label">Total Tags</div>
      </div>
    </div>
    
    <div class="tag-controls">
      <div class="search-container">
        <div class="search-box">
          <input 
            type="text" 
            v-model="searchQuery" 
            placeholder="🔍 Search tags by name..." 
            @input="filterTags"
            class="search-input"
          />
          <button v-if="searchQuery" @click="clearSearch" class="clear-button">✕</button>
        </div>
      </div>
      <button class="add-button" @click="openTagModal()">
        <span class="button-icon">+</span>
        Add New Tag
      </button>
    </div>
    
    <div class="tags-container">
      <div v-if="loading" class="loading-state">
        <div class="loading-spinner"></div>
        <p>Loading tag list...</p>
      </div>
      
      <div v-else-if="filteredTags.length === 0" class="empty-state">
        <div class="empty-icon">🏷️</div>
        <h3>No tags found</h3>
        <p v-if="searchQuery">
          No tags match the keyword "{{ searchQuery }}"
          <button @click="clearSearch" class="clear-search">Clear search</button>
        </p>
        <p v-else>No tags in the system yet</p>
      </div>
      
      <div v-else class="tags-grid">
        <div v-for="tag in filteredTags" :key="tag.id" class="tag-card">
          <div class="tag-content">
            <div class="tag-header">
              <div class="tag-icon">🏷️</div>
              <div class="tag-info">
                <h3 class="tag-title">{{ tag.title }}</h3>
                <div class="tag-meta">
                  <span class="tag-id">ID: {{ tag.id }}</span>
                  <span class="tag-usage" v-if="getTagUsage(tag.id) > 0">
                    {{ getTagUsage(tag.id) }} games
                  </span>
                  <span class="tag-unused" v-else>Not used</span>
                </div>
              </div>
            </div>
          </div>
          
          <div class="tag-actions">
            <button class="action-btn edit-btn" @click="openTagModal(tag)">
              <span class="btn-icon">✏️</span>
              Edit
            </button>
            <button class="action-btn delete-btn" @click="deleteTag(tag.id)">
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
    
    <!-- Tag Modal for Add/Edit -->
    <div v-if="showModal" class="modal-overlay" @click.self="closeTagModal">
      <div class="modal-content">
        <div class="modal-header">
          <h2>{{ editingTag ? '✏️ Edit Tag' : '➕ Add New Tag' }}</h2>
          <button @click="closeTagModal" class="close-btn" :disabled="saving">✕</button>
        </div>
        
        <div class="form-group">
          <label for="tag-title">
            <span class="label-icon">🏷️</span>
            Tag Name
          </label>
          <input 
            type="text" 
            id="tag-title" 
            v-model="tagForm.title" 
            class="form-control"
            placeholder="Enter tag name (e.g: Action, Adventure, RPG...)"
            maxlength="50"
          />
          <div class="char-count">{{ tagForm.title.length }}/50</div>
          <div v-if="formError" class="form-error">{{ formError }}</div>
        </div>
        
        <div class="modal-actions">
          <button 
            @click="saveTag" 
            class="save-button" 
            :disabled="saving || !tagForm.title.trim()"
          >
            <span class="btn-icon">{{ saving ? '⏳' : '💾' }}</span>
            {{ saving ? 'Saving...' : (editingTag ? 'Update' : 'Create') }}
          </button>
          <button @click="closeTagModal" class="cancel-button" :disabled="saving">
            <span class="btn-icon">❌</span>
            Cancel
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      tags: [],
      filteredTags: [],
      searchQuery: '',
      currentPage: 1,
      totalTags: 0,
      totalPages: 1,
      pageSize: 12,
      loading: true,
      showModal: false,
      editingTag: null,
      tagForm: {
        title: ''
      },
      formError: '',
      saving: false,
      successMessage: '',
      gameTagsData: [] // Để tính toán usage
    };
  },
  computed: {
    // Computed properties có thể thêm sau nếu cần
  },
  async mounted() {
    await this.fetchTags();
    await this.fetchGameTagsData();
  },
  methods: {
    async fetchTags() {
      this.loading = true;
      try {
        const token = localStorage.getItem('token');
        const response = await fetch('http://localhost:5174/api/tags', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (!response.ok) {
          throw new Error('Failed to fetch tags');
        }
        
        const data = await response.json();
        this.tags = data;
        
        this.totalTags = this.tags.length;
        this.totalPages = Math.ceil(this.totalTags / this.pageSize);
        this.filterTags();
      } catch (error) {
        console.error('Error fetching tags:', error);
      } finally {
        this.loading = false;
      }
    },
    
    filterTags() {
      if (!this.searchQuery) {
        // No search, just paginate
        const startIndex = (this.currentPage - 1) * this.pageSize;
        this.filteredTags = this.tags.slice(startIndex, startIndex + this.pageSize);
      } else {
        // Search and paginate results
        const query = this.searchQuery.toLowerCase();
        const filtered = this.tags.filter(tag => 
          tag.title.toLowerCase().includes(query)
        );
        
        this.totalTags = filtered.length;
        this.totalPages = Math.ceil(this.totalTags / this.pageSize);
        
        // Adjust current page if it's now out of bounds
        if (this.currentPage > this.totalPages && this.totalPages > 0) {
          this.currentPage = this.totalPages;
        }
        
        const startIndex = (this.currentPage - 1) * this.pageSize;
        this.filteredTags = filtered.slice(startIndex, startIndex + this.pageSize);
      }
    },
    
    changePage(page) {
      this.currentPage = page;
      this.filterTags();
    },
    
    clearSearch() {
      this.searchQuery = '';
      this.currentPage = 1;
      this.filterTags();
    },
    
    openTagModal(tag = null) {
      this.editingTag = tag;
      if (tag) {
        this.tagForm.title = tag.title;
      } else {
        this.tagForm.title = '';
      }
      this.formError = '';
      this.showModal = true;
    },
    
    closeTagModal() {
      if (this.saving) return;
      this.showModal = false;
      this.editingTag = null;
      this.tagForm.title = '';
      this.formError = '';
    },
    
    async saveTag() {
      // Validate form
      if (!this.tagForm.title.trim()) {
        this.formError = 'Tag title is required';
        return;
      }
      
      this.saving = true;
      const token = localStorage.getItem('token');
      
      try {
        let response;
        
        if (this.editingTag) {
          // Update existing tag
          response = await fetch(`http://localhost:5174/api/tags/${this.editingTag.id}`, {
            method: 'PUT',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({ title: this.tagForm.title })
          });
        } else {
          // Create new tag
          response = await fetch('http://localhost:5174/api/tags', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({ title: this.tagForm.title })
          });
        }
        
        if (!response.ok) {
          throw new Error('Failed to save tag');
        }
        
        // Show success message
        this.successMessage = this.editingTag 
          ? `Tag "${this.tagForm.title}" has been updated successfully` 
          : `Tag "${this.tagForm.title}" has been created successfully`;
        
        // Auto-hide success message after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
        
        // Refresh tags
        await this.fetchTags();
        this.closeTagModal();
      } catch (error) {
        console.error('Error saving tag:', error);
        this.formError = 'Failed to save tag. Please try again.';
      } finally {
        this.saving = false;
      }
    },
    
    async deleteTag(id) {
      const tag = this.tags.find(t => t.id === id);
      const tagName = tag ? tag.title : 'tag này';
      const tagUsage = this.getTagUsage(id);
      
      let confirmMessage = `Are you sure you want to delete tag "${tagName}"?`;
      if (tagUsage > 0) {
        confirmMessage += ` This tag is currently being used by ${tagUsage} game(s).`;
      }
      confirmMessage += ' This action cannot be undone.';
      
      if (!confirm(confirmMessage)) {
        return;
      }
      
      try {
        const token = localStorage.getItem('token');
        const response = await fetch(`http://localhost:5174/api/tags/${id}`, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (!response.ok) {
          throw new Error('Failed to delete tag');
        }
        
        // Show success message
        this.successMessage = `Tag "${tagName}" has been deleted successfully`;
        
        // Auto-hide success message after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
        
        // Refresh tags
        await this.fetchTags();
        await this.fetchGameTagsData();
      } catch (error) {
        console.error('Error deleting tag:', error);
        alert('Unable to delete tag. This tag may be in use by games.');
      }
    },
    
    async fetchGameTagsData() {
      try {
        const token = localStorage.getItem('token');
        const response = await fetch('http://localhost:5174/api/gametags', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (response.ok) {
          this.gameTagsData = await response.json();
        }
      } catch (error) {
        console.error('Error fetching game tags data:', error);
        // Không hiển thị lỗi vì đây không phải là chức năng quan trọng
      }
    },
    
    getTagUsage(tagId) {
      return this.gameTagsData.filter(gt => gt.tagId === tagId).length;
    }
  }
};
</script>

<style scoped>
.admin-tag-list {
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

/* Success Message */
.success-message {
  background: linear-gradient(135deg, #10b981, #059669);
  color: white;
  padding: 1rem 1.5rem;
  border-radius: 12px;
  margin-bottom: 2rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  animation: slideInFromTop 0.5s ease-out;
  box-shadow: var(--ShadowLight);
}

.success-icon {
  font-size: 1.2rem;
}

@keyframes slideInFromTop {
  from { 
    opacity: 0; 
    transform: translateY(-20px); 
  }
  to { 
    opacity: 1; 
    transform: translateY(0); 
  }
}

/* Tag Statistics */
.tag-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
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

/* Tag Controls */
.tag-controls {
  margin-bottom: 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
}

.search-container {
  flex: 1;
  max-width: 400px;
}

.search-box {
  position: relative;
  display: flex;
  align-items: center;
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

/* Tags Container */
.tags-container {
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

/* Tags Grid */
.tags-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1rem;
}

/* Tag Card */
.tag-card {
  background: var(--Color-Background-System);
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
  box-shadow: var(--ShadowLight);
}

.tag-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--ShadowHeavy);
}

.tag-content {
  padding: 1.5rem;
}

.tag-header {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.tag-icon {
  font-size: 2rem;
  opacity: 0.8;
}

.tag-info {
  flex: 1;
}

.tag-title {
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingMedium);
  font-weight: 600;
  margin: 0 0 0.5rem 0;
}

.tag-meta {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.tag-id {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  background: var(--Color-Background-Highlight);
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
}

.tag-usage {
  color: var(--Color-Primary);
  font: var(--Text-BodySmall);
  font-weight: 600;
  background: rgba(var(--Color-Primary-RGB), 0.1);
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
}

.tag-unused {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  opacity: 0.7;
}

/* Tag Actions */
.tag-actions {
  display: flex;
  gap: 0.5rem;
  padding: 0 1.5rem 1.5rem;
}

.action-btn {
  flex: 1;
  padding: 0.75rem 1rem;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  transition: all 0.2s ease;
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

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(4px);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  animation: fadeInOverlay 0.3s ease;
}

@keyframes fadeInOverlay {
  from { opacity: 0; }
  to { opacity: 1; }
}

.modal-content {
  background: var(--Color-Background-Main);
  border-radius: 16px;
  padding: 0;
  width: 90%;
  max-width: 500px;
  box-shadow: var(--ShadowHeavy);
  animation: slideInModal 0.3s ease;
  overflow: hidden;
}

@keyframes slideInModal {
  from { 
    opacity: 0; 
    transform: scale(0.9) translateY(-20px); 
  }
  to { 
    opacity: 1; 
    transform: scale(1) translateY(0); 
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 2rem;
  background: var(--Color-Background-Highlight);
  border-bottom: 1px solid var(--Color-Background-System);
}

.modal-header h2 {
  margin: 0;
  font: var(--Text-HeadingMedium);
  color: var(--Color-Text-Main);
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: var(--Color-Text-Dim);
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 50%;
  transition: all 0.2s ease;
  line-height: 1;
}

.close-btn:hover:not(:disabled) {
  background: var(--Color-Background-System);
  color: var(--Color-Text-Main);
}

.close-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.form-group {
  padding: 2rem;
  padding-bottom: 0;
}

.form-group label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
  font-weight: 600;
  color: var(--Color-Text-Main);
}

.label-icon {
  font-size: 1.1rem;
}

.form-control {
  width: 100%;
  padding: 0.75rem 1rem;
  background: var(--Color-Background-Highlight);
  border: 2px solid transparent;
  border-radius: 8px;
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  transition: all 0.3s ease;
}

.form-control:focus {
  outline: none;
  border-color: var(--Color-Primary);
  box-shadow: 0 0 0 3px rgba(var(--Color-Primary-RGB), 0.1);
}

.char-count {
  text-align: right;
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  margin-top: 0.5rem;
}

.form-error {
  color: var(--Color-Accent-Red);
  margin-top: 0.5rem;
  font: var(--Text-BodySmall);
  background: rgba(239, 68, 68, 0.1);
  padding: 0.5rem;
  border-radius: 6px;
  border-left: 3px solid var(--Color-Accent-Red);
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 2rem;
  background: var(--Color-Background-Highlight);
}

.save-button {
  background: var(--Gradient-Button-Action);
  color: white;
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

.save-button:hover:not(:disabled) {
  background: var(--Color-Primary);
  transform: translateY(-1px);
}

.save-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

.cancel-button {
  background: var(--Color-Background-System);
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
}

.cancel-button:hover:not(:disabled) {
  background: var(--Color-Accent-Red);
  color: white;
  transform: translateY(-1px);
}

.cancel-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

/* Responsive Design */
@media (max-width: 768px) {
  .admin-tag-list {
    padding: 1rem;
  }
  
  .admin-menu {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .tag-stats {
    grid-template-columns: 1fr;
  }
  
  .tag-controls {
    flex-direction: column;
    align-items: stretch;
  }
  
  .search-container {
    max-width: none;
    margin-bottom: 1rem;
  }
  
  .tags-grid {
    grid-template-columns: 1fr;
  }
  
  .pagination {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .pagination-btn {
    width: 100%;
    justify-content: center;
  }
  
  .modal-content {
    width: 95%;
    margin: 1rem;
  }
  
  .modal-header {
    padding: 1rem 1.5rem;
  }
  
  .form-group {
    padding: 1.5rem;
    padding-bottom: 0;
  }
  
  .modal-actions {
    padding: 1.5rem;
    flex-direction: column;
  }
  
  .save-button,
  .cancel-button {
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 480px) {
  .stat-number {
    font-size: 1.5rem;
  }
  
  .tag-actions {
    flex-direction: column;
  }
}
</style> 