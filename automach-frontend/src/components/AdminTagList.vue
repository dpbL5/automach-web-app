<template>
  <div class="admin-tag-list">
    <h1>Tag Management</h1>
    
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
    
    <div v-if="successMessage" class="success-message">
      {{ successMessage }}
    </div>
    
    <div class="tag-controls">
      <div class="search-box">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Search tags..." 
          @input="filterTags"
          class="search-input"
        />
      </div>
      <button class="add-button" @click="openTagModal()">Add New Tag</button>
    </div>
    
    <div class="tag-table">
      <div class="tag-header">
        <div class="tag-cell">ID</div>
        <div class="tag-cell">Title</div>
        <div class="tag-cell">Actions</div>
      </div>
      
      <div v-if="loading" class="loading">
        Loading tags...
      </div>
      
      <div v-else-if="filteredTags.length === 0" class="no-tags">
        No tags found. 
        <span v-if="searchQuery" @click="clearSearch" class="clear-search">Clear search</span>
      </div>
      
      <div v-else v-for="tag in filteredTags" :key="tag.id" class="tag-row">
        <div class="tag-cell">{{ tag.id }}</div>
        <div class="tag-cell">{{ tag.title }}</div>
        <div class="tag-cell actions">
          <button class="edit-button" @click="openTagModal(tag)">Edit</button>
          <button class="delete-button" @click="deleteTag(tag.id)">Delete</button>
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
    
    <!-- Tag Modal for Add/Edit -->
    <div v-if="showModal" class="modal-overlay" @click.self="closeTagModal">
      <div class="modal-content">
        <h2>{{ editingTag ? 'Edit Tag' : 'Add New Tag' }}</h2>
        
        <div class="form-group">
          <label for="tag-title">Tag Title</label>
          <input 
            type="text" 
            id="tag-title" 
            v-model="tagForm.title" 
            class="form-control"
            placeholder="Enter tag title"
          />
          <div v-if="formError" class="form-error">{{ formError }}</div>
        </div>
        
        <div class="modal-actions">
          <button 
            @click="saveTag" 
            class="save-button" 
            :disabled="saving"
          >
            {{ saving ? 'Saving...' : 'Save' }}
          </button>
          <button @click="closeTagModal" class="cancel-button" :disabled="saving">Cancel</button>
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
      pageSize: 10,
      loading: true,
      showModal: false,
      editingTag: null,
      tagForm: {
        title: ''
      },
      formError: '',
      saving: false,
      successMessage: ''
    };
  },
  async mounted() {
    await this.fetchTags();
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
          ? `Tag "${this.tagForm.title}" updated successfully` 
          : `Tag "${this.tagForm.title}" created successfully`;
        
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
      if (!confirm('Are you sure you want to delete this tag? This action cannot be undone.')) {
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
        this.successMessage = 'Tag deleted successfully';
        
        // Auto-hide success message after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
        
        // Refresh tags
        await this.fetchTags();
      } catch (error) {
        console.error('Error deleting tag:', error);
        alert('Failed to delete tag. It may be in use by games.');
      }
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

.success-message {
  background-color: var(--Color-Accent-Green);
  color: white;
  padding: 1rem;
  border-radius: 4px;
  margin-bottom: 1.5rem;
  font-weight: 500;
  animation: fadeIn 0.3s ease-in-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(-10px); }
  to { opacity: 1; transform: translateY(0); }
}

.tag-controls {
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

.tag-table {
  background-color: var(--Color-Background-Main);
  border-radius: 8px;
  overflow: hidden;
  box-shadow: var(--ShadowLight);
  margin-bottom: 1rem;
}

.tag-header {
  display: grid;
  grid-template-columns: 80px 1fr 120px;
  background-color: var(--Color-Background-Highlight);
  padding: 1rem;
  font-weight: bold;
}

.tag-row {
  display: grid;
  grid-template-columns: 80px 1fr 120px;
  padding: 1rem;
  border-bottom: 1px solid var(--Color-Background-Highlight);
  align-items: center;
}

.tag-row:nth-child(even) {
  background-color: var(--Color-Background-Highlight);
}

.tag-cell {
  display: flex;
  align-items: center;
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

.loading, .no-tags {
  padding: 2rem;
  text-align: center;
  color: var(--Color-Text-Dim);
}

.clear-search {
  color: var(--Color-Primary);
  cursor: pointer;
  text-decoration: underline;
}

/* Modal styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 100;
}

.modal-content {
  background-color: var(--Color-Background-Main);
  border-radius: 8px;
  padding: 2rem;
  width: 90%;
  max-width: 500px;
  box-shadow: var(--ShadowHeavy);
}

.modal-content h2 {
  margin-bottom: 1.5rem;
  font: var(--Text-HeadingMedium);
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.form-control {
  width: 100%;
  padding: 0.5rem;
  background-color: var(--Color-Background-System);
  border: none;
  border-radius: 4px;
  color: var(--Color-Text-Main);
}

.form-error {
  color: var(--Color-Accent-Red);
  margin-top: 0.5rem;
  font-size: 0.9rem;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}

.save-button {
  background: var(--Gradient-Button-Action);
  color: white;
  border: none;
  border-radius: 4px;
  padding: 0.5rem 1rem;
  cursor: pointer;
  font-weight: bold;
}

.save-button:hover:not(:disabled) {
  background: var(--Color-Primary);
}

.save-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.cancel-button {
  background-color: var(--Color-Background-System);
  color: white;
  border: none;
  border-radius: 4px;
  padding: 0.5rem 1rem;
  cursor: pointer;
}

.cancel-button:hover:not(:disabled) {
  background-color: var(--Color-Accent-Red);
}

.cancel-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}
</style> 