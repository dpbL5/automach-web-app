<template>
  <div class="add-game-container">
    <h1>{{ pageTitle }}</h1>

    <div class="form-container">
      <div class="form-group">
        <label for="game-title">Name</label>
        <input 
          type="text" 
          id="game-title" 
          v-model="gameData.title" 
          class="form-control"
          placeholder="Enter game name"
        />
      </div>

      <div class="form-group">
        <label>Banner (1 banner only)</label>
        <div class="file-upload">
          <input 
            type="text" 
            v-model="bannerFileName" 
            class="form-control"
            placeholder="banner.png" 
            disabled
          />
          <button @click="selectBanner" class="upload-btn" :disabled="processingImages">
            <span v-if="processingImages" class="processing-indicator"></span>
            Upload
          </button>
          <input 
            type="file" 
            ref="bannerInput" 
            accept="image/*" 
            @change="handleBannerUpload" 
            style="display: none;" 
          />
        </div>
        <div v-if="bannerPreview" class="banner-preview">
          <img :src="bannerPreview" alt="Banner preview" />
          <button @click="removeBanner" class="remove-image-btn">×</button>
        </div>
      </div>

      <div class="form-group">
        <label>Showcase</label>
        <div class="showcase-images">
          <div 
            v-for="(image, index) in showcaseImages" 
            :key="index" 
            class="showcase-image"
          >
            <img :src="image.preview" alt="Showcase preview" />
            <button @click="removeShowcaseImage(index)" class="remove-image-btn">×</button>
          </div>
          <div 
            v-for="n in (4 - showcaseImages.length)" 
            :key="`empty-${n}`" 
            class="showcase-image empty"
            v-if="!processingImages"
          ></div>
          <div v-if="processingImages && showcaseImages.length < 4" class="showcase-image processing">
            <div class="processing-indicator"></div>
          </div>
        </div>
        <div class="file-upload showcase-upload">
          <button @click="selectShowcaseImages" class="upload-btn" :disabled="processingImages">
            <span v-if="processingImages" class="processing-indicator"></span>
            Upload
          </button>
          <input 
            type="file" 
            ref="showcaseInput" 
            accept="image/*" 
            multiple 
            @change="handleShowcaseUpload" 
            style="display: none;" 
          />
        </div>
      </div>

      <div class="form-group">
        <label for="tags">Add tag</label>
        <div class="tags-container">
          <input 
            type="text" 
            id="tags" 
            v-model="tagInput" 
            class="form-control" 
            placeholder="Add tag"
            @keyup.enter="addTag"
            @focus="showTagsDropdown = true"
            @blur="setTimeout(() => { showTagsDropdown = false }, 200)"
          />
          <div class="tags-dropdown" v-if="showTagsDropdown && filteredTags.length">
            <div 
              v-for="tag in filteredTags" 
              :key="tag.id" 
              class="tag-option"
              @click="selectTag(tag)"
            >
              {{ tag.title }}
            </div>
          </div>
        </div>
        <div class="selected-tags">
          <div v-for="tag in selectedTags" :key="tag.id" class="tag">
            {{ tag.title }} <span @click="removeTag(tag)" class="remove-tag">×</span>
          </div>
        </div>
      </div>

      <div class="form-row">
        <div class="form-group half">
          <label for="release-date">Release Date</label>
          <input 
            type="date" 
            id="release-date" 
            v-model="gameData.releaseDate" 
            class="form-control"
          />
        </div>
        <div class="form-group half">
          <label for="price">Price:</label>
          <input 
            type="number" 
            id="price" 
            v-model="gameData.price" 
            class="form-control"
            min="0"
            step="0.01"
          />
        </div>
      </div>

      <div class="form-group">
        <label>About This Game (Markdown supported)</label>
        <div class="markdown-editor">
          <div class="editor-tabs">
            <button 
              @click="activeTab = 'write'" 
              :class="{ active: activeTab === 'write' }"
              class="tab-btn"
              type="button"
            >
              Write
            </button>
            <button 
              @click="activeTab = 'preview'" 
              :class="{ active: activeTab === 'preview' }"
              class="tab-btn"
              type="button"
            >
              Preview
            </button>
          </div>
          <div class="editor-toolbar" v-if="activeTab === 'write'">
            <button @click.prevent="insertMarkdown('**', '**', 'bold text')" class="toolbar-btn" title="Bold">
              <strong>B</strong>
            </button>
            <button @click.prevent="insertMarkdown('*', '*', 'italic text')" class="toolbar-btn" title="Italic">
              <em>I</em>
            </button>
            <button @click.prevent="insertMarkdown('`', '`', 'code')" class="toolbar-btn" title="Code">
              &lt;/&gt;
            </button>
            <button @click.prevent="insertMarkdown('## ', '', 'Heading')" class="toolbar-btn" title="Heading">
              H2
            </button>
            <button @click.prevent="insertMarkdown('- ', '', 'List item')" class="toolbar-btn" title="List">
              ≡
            </button>
            <button @click.prevent="insertMarkdown('[', '](url)', 'link text')" class="toolbar-btn" title="Link">
              🔗
            </button>
          </div>
          <textarea 
            v-if="activeTab === 'write'"
            v-model="gameData.gameInfo" 
            class="editor-content"
            rows="12"
            placeholder="Enter game description using Markdown syntax...

Examples:
**Bold text**
*Italic text*
## Heading
- List item
[Link text](https://example.com)
`Code`
            "
            ref="markdownTextarea"
          ></textarea>
          <div 
            v-if="activeTab === 'preview'" 
            class="markdown-preview"
            v-html="markdownPreview"
          ></div>
        </div>
      </div>

      <div class="form-actions">
        <button @click="publishGame" class="publish-btn" :disabled="publishingGame || processingImages">
          <span v-if="processingImages">Processing Images...</span>
          <span v-else-if="publishingGame">{{ isEditMode ? 'Updating...' : 'Publishing...' }}</span>
          <span v-else>{{ isEditMode ? 'Update' : 'Publish' }}</span>
        </button>
        <button @click="cancel" class="cancel-btn" :disabled="publishingGame || processingImages">Cancel</button>
      </div>

      <!-- Validation errors -->
      <div v-if="validationErrors.length" class="validation-errors">
        <div class="error-title">Please fix the following errors:</div>
        <ul>
          <li v-for="(error, index) in validationErrors" :key="index">{{ error }}</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script>
import { uploadImage, compressImage } from '../helpers/imageUpload';
import { marked } from 'marked';

export default {
  data() {
    return {
      isEditMode: false,
      gameId: null,
      gameData: {
        title: '',
        price: 0,
        gameInfo: '',
        releaseDate: new Date().toISOString().split('T')[0],
        developer: 'Automach Games',
        isFeatured: false
      },
      bannerFile: null,
      bannerFileName: '',
      bannerPreview: null,
      bannerUrl: null,
      originalBannerUrl: null,
      showcaseImages: [],
      existingImageIds: [],
      availableTags: [],
      selectedTags: [],
      tagInput: '',
      showTagsDropdown: false,
      publishingGame: false,
      validationErrors: [],
      processingImages: false,
      activeTab: 'write'
    };
  },
  computed: {
    filteredTags() {
      if (!this.tagInput) return this.availableTags;
      const searchTerm = this.tagInput.toLowerCase();
      return this.availableTags.filter(tag => 
        tag.title.toLowerCase().includes(searchTerm) && 
        !this.selectedTags.some(selectedTag => selectedTag.id === tag.id)
      );
    },
    pageTitle() {
      return this.isEditMode ? 'Edit Game' : 'Publish a game';
    },
    markdownPreview() {
      if (!this.gameData.gameInfo) {
        return '<p class="preview-placeholder">Preview will appear here...</p>';
      }
      return marked(this.gameData.gameInfo);
    }
  },
  async mounted() {
    await this.fetchTags();
    
    // Kiểm tra xem đang ở chế độ thêm hay sửa game
    if (this.$route.params.id) {
      this.isEditMode = true;
      this.gameId = this.$route.params.id;
      await this.fetchGameDetails();
    }
  },
  methods: {
    async fetchTags() {
      try {
        const response = await fetch('http://localhost:5174/api/tags');
        if (response.ok) {
          this.availableTags = await response.json();
        } else {
          console.error('Failed to fetch tags');
        }
      } catch (error) {
        console.error('Error fetching tags:', error);
      }
    },
    selectBanner() {
      this.$refs.bannerInput.click();
    },
    async handleBannerUpload(event) {
      const file = event.target.files[0];
      if (!file) return;
      
      this.processingImages = true;
      this.bannerFile = file;
      this.bannerFileName = file.name;
      
      try {
        // Create a compressed version for preview
        const compressedImage = await compressImage(file, 800, 0.6);
        const reader = new FileReader();
        reader.onload = e => {
          this.bannerPreview = e.target.result;
          this.processingImages = false;
        };
        reader.readAsDataURL(compressedImage);
      } catch (error) {
        console.error('Error processing banner image:', error);
        this.processingImages = false;
        
        // Fallback to original if compression fails
        const reader = new FileReader();
        reader.onload = e => {
          this.bannerPreview = e.target.result;
        };
        reader.readAsDataURL(file);
      }
    },
    selectShowcaseImages() {
      this.$refs.showcaseInput.click();
    },
    async handleShowcaseUpload(event) {
      const files = event.target.files;
      if (!files.length) return;
      
      // Limit to 4 images maximum
      const maxImages = 4 - this.showcaseImages.length;
      const selectedFiles = Array.from(files).slice(0, maxImages);
      
      this.processingImages = true;
      
      try {
        // Process all images in parallel for better performance
        const processedImages = await Promise.all(
          selectedFiles.map(async (file) => {
            // Create compressed version for preview
            const compressedImage = await compressImage(file, 400, 0.6);
            const preview = await new Promise((resolve) => {
              const reader = new FileReader();
              reader.onload = e => resolve(e.target.result);
              reader.readAsDataURL(compressedImage);
            });
            
            return {
              file,
              preview
            };
          })
        );
        
        // Add all processed images to the showcase
        this.showcaseImages.push(...processedImages);
      } catch (error) {
        console.error('Error processing showcase images:', error);
        
        // Fallback to original processing if compression fails
        selectedFiles.forEach(file => {
          const reader = new FileReader();
          reader.onload = e => {
            this.showcaseImages.push({
              file,
              preview: e.target.result
            });
          };
          reader.readAsDataURL(file);
        });
      } finally {
        this.processingImages = false;
      }
    },
    addTag() {
      if (!this.tagInput.trim()) return;
      
      // Check if tag already exists in the available tags
      const existingTag = this.availableTags.find(
        tag => tag.title.toLowerCase() === this.tagInput.trim().toLowerCase()
      );
      
      if (existingTag && !this.selectedTags.some(t => t.id === existingTag.id)) {
        this.selectedTags.push(existingTag);
      } else if (!existingTag) {
        // Create a new tag if it doesn't exist
        this.createNewTag(this.tagInput.trim());
      }
      
      this.tagInput = '';
      this.showTagsDropdown = false;
    },
    async createNewTag(title) {
      try {
        const token = localStorage.getItem('token');
        const response = await fetch('http://localhost:5174/api/tags', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          },
          body: JSON.stringify({ title })
        });
        
        if (response.ok) {
          const newTag = await response.json();
          this.availableTags.push(newTag);
          this.selectedTags.push(newTag);
        } else {
          console.error('Failed to create new tag');
        }
      } catch (error) {
        console.error('Error creating tag:', error);
      }
    },
    selectTag(tag) {
      if (!this.selectedTags.some(t => t.id === tag.id)) {
        this.selectedTags.push(tag);
      }
      this.tagInput = '';
      this.showTagsDropdown = false;
    },
    removeTag(tag) {
      this.selectedTags = this.selectedTags.filter(t => t.id !== tag.id);
    },
    insertMarkdown(before, after, placeholder) {
      const textarea = this.$refs.markdownTextarea;
      if (!textarea) return;
      
      const start = textarea.selectionStart;
      const end = textarea.selectionEnd;
      const selectedText = textarea.value.substring(start, end);
      
      let insertText;
      if (selectedText) {
        insertText = before + selectedText + after;
      } else {
        insertText = before + placeholder + after;
      }
      
      const newValue = 
        textarea.value.substring(0, start) + 
        insertText + 
        textarea.value.substring(end);
      
      this.gameData.gameInfo = newValue;
      
      // Set cursor position
      this.$nextTick(() => {
        const newPos = start + before.length + (selectedText || placeholder).length;
        textarea.setSelectionRange(newPos, newPos);
        textarea.focus();
      });
    },
    async uploadBanner() {
      if (!this.bannerFile) return null;
      
      try {
        // The uploadImage function already includes compression
        return await uploadImage(this.bannerFile);
      } catch (error) {
        console.error('Error uploading banner:', error);
        return null;
      }
    },
    async uploadShowcaseImages() {
      if (!this.showcaseImages.length) return [];
      
      try {
        // Upload images in parallel for better performance
        const urls = await Promise.all(
          this.showcaseImages
            .filter(image => image.file && !image.isExisting)
            .map(image => uploadImage(image.file))
        );
        return urls;
      } catch (error) {
        console.error('Error uploading showcase images:', error);
        return [];
      }
    },
    async publishGame() {
      if (this.publishingGame || this.processingImages) return;
      
      // Validate form
      this.validationErrors = [];
      
      if (!this.gameData.title.trim()) {
        this.validationErrors.push('Game name is required');
      }
      
      if (this.gameData.price < 0) {
        this.validationErrors.push('Price must be a positive number');
      }
      
      if (!this.gameData.gameInfo.trim()) {
        this.validationErrors.push('Game description is required');
      }
      
      // Chỉ kiểm tra banner và showcase khi thêm mới, không cần khi sửa
      if (!this.isEditMode) {
        if (!this.bannerFile && !this.bannerUrl) {
          this.validationErrors.push('Banner image is required');
        }
        
        if (this.showcaseImages.length === 0) {
          this.validationErrors.push('At least one showcase image is required');
        }
      }
      
      if (this.selectedTags.length === 0) {
        this.validationErrors.push('At least one tag is required');
      }
      
      // If there are validation errors, stop here
      if (this.validationErrors.length > 0) {
        window.scrollTo(0, 0);
        return;
      }
      
      this.publishingGame = true;
      
      try {
        const token = localStorage.getItem('token');
        let gameId;
        
        // 1. Create or update the game
        if (this.isEditMode) {
          // Update existing game
          const response = await fetch(`http://localhost:5174/api/games/${this.gameId}`, {
            method: 'PUT',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(this.gameData)
          });
          
          if (!response.ok) {
            throw new Error('Failed to update game');
          }
          
          gameId = this.gameId;
          
          // Delete marked images
          for (const image of this.existingImageIds) {
            if (image.toDelete) {
              await fetch(`http://localhost:5174/api/games/${gameId}/images/${image.id}`, {
                method: 'DELETE',
                headers: { 'Authorization': `Bearer ${token}` }
              });
            }
          }
        } else {
          // Create new game
          const response = await fetch('http://localhost:5174/api/games', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(this.gameData)
          });
          
          if (!response.ok) {
            throw new Error('Failed to create game');
          }
          
          const newGame = await response.json();
          gameId = newGame.id;
        }
        
        // 2. Upload banner if a new one is selected
        if (this.bannerFile) {
          const bannerUrl = await this.uploadBanner();
          
          if (bannerUrl) {
            await fetch(`http://localhost:5174/api/games/${gameId}/images`, {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
              },
              body: JSON.stringify({
                url: bannerUrl
              })
            });
          }
        }
        
        // 3. Upload new showcase images
        const showcaseToUpload = this.showcaseImages.filter(img => !img.isExisting);
        for (const image of showcaseToUpload) {
          if (image.file) {
            const url = await uploadImage(image.file);
            await fetch(`http://localhost:5174/api/games/${gameId}/images`, {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
              },
              body: JSON.stringify({
                url
              })
            });
          }
        }
        
        // 4. Update tags - first remove all existing tags, then add the selected ones
        if (this.isEditMode) {
          // Fetch current tags to remove them
          const tagsResponse = await fetch(`http://localhost:5174/api/games/${gameId}/tags`);
          if (tagsResponse.ok) {
            const currentTags = await tagsResponse.json();
            
            // Remove all current tags
            for (const tag of currentTags) {
              await fetch(`http://localhost:5174/api/games/${gameId}/tags/${tag.id}`, {
                method: 'DELETE',
                headers: { 'Authorization': `Bearer ${token}` }
              });
            }
          }
        }
        
        // Add selected tags
        for (const tag of this.selectedTags) {
          await fetch(`http://localhost:5174/api/games/${gameId}/tags`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
              tagId: tag.id
            })
          });
        }
        
        // Success! Redirect to game list
        alert(this.isEditMode ? 'Game updated successfully!' : 'Game published successfully!');
        this.$router.push('/admin/games');
      } catch (error) {
        console.error('Error publishing/updating game:', error);
        alert('Failed to publish/update game. Please try again.');
      } finally {
        this.publishingGame = false;
      }
    },
    cancel() {
      this.$router.push('/admin/games');
    },
    removeBanner() {
      this.bannerFile = null;
      this.bannerFileName = '';
      this.bannerPreview = null;
      this.bannerUrl = null;
      
      // Nếu đang ở chế độ sửa và có ảnh gốc, đánh dấu để xóa
      if (this.isEditMode && this.originalBannerUrl) {
        // Tìm ID của ảnh banner từ existingImageIds
        const bannerImage = this.existingImageIds.find(img => img.url === this.originalBannerUrl);
        if (bannerImage) {
          bannerImage.toDelete = true;
        }
      }
    },
    removeShowcaseImage(index) {
      // Nếu đang ở chế độ sửa và ảnh này là ảnh đã tồn tại
      if (this.isEditMode && this.showcaseImages[index].id) {
        // Đánh dấu ảnh để xóa
        const imageId = this.showcaseImages[index].id;
        const existingImage = this.existingImageIds.find(img => img.id === imageId);
        if (existingImage) {
          existingImage.toDelete = true;
        }
      }
      
      // Xóa ảnh khỏi danh sách hiển thị
      this.showcaseImages.splice(index, 1);
    },
    async fetchGameDetails() {
      try {
        const token = localStorage.getItem('token');
        const response = await fetch(`http://localhost:5174/api/games/${this.gameId}`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        
        if (!response.ok) {
          throw new Error('Failed to fetch game details');
        }
        
        const game = await response.json();
        
        // Cập nhật thông tin game
        this.gameData.title = game.title;
        this.gameData.price = game.price;
        this.gameData.gameInfo = game.gameInfo;
        this.gameData.developer = game.developer;
        this.gameData.isFeatured = game.isFeatured;
        
        // Định dạng ngày
        if (game.releaseDate) {
          const date = new Date(game.releaseDate);
          this.gameData.releaseDate = date.toISOString().split('T')[0];
        }
        
        // Fetch images
        await this.fetchGameImages();
        
        // Fetch tags
        await this.fetchGameTags();
      } catch (error) {
        console.error('Error fetching game details:', error);
        alert('Failed to load game details. Please try again.');
      }
    },
    
    async fetchGameImages() {
      try {
        const response = await fetch(`http://localhost:5174/api/games/${this.gameId}/images`);
        
        if (!response.ok) {
          throw new Error('Failed to fetch game images');
        }
        
        const images = await response.json();
        this.existingImageIds = images.map(img => ({ 
          id: img.id, 
          url: img.url,
          toDelete: false
        }));
        
        // Hiển thị ảnh đầu tiên là banner (nếu có)
        if (images.length > 0) {
          this.bannerUrl = images[0].url;
          this.bannerPreview = images[0].url;
          this.originalBannerUrl = images[0].url;
          this.bannerFileName = 'existing-banner.jpg';
          
          // Các ảnh còn lại là showcase
          if (images.length > 1) {
            for (let i = 1; i < images.length && i < 5; i++) {
              this.showcaseImages.push({
                id: images[i].id,
                preview: images[i].url,
                isExisting: true
              });
            }
          }
        }
      } catch (error) {
        console.error('Error fetching game images:', error);
      }
    },
    
    async fetchGameTags() {
      try {
        const response = await fetch(`http://localhost:5174/api/games/${this.gameId}/tags`);
        
        if (!response.ok) {
          throw new Error('Failed to fetch game tags');
        }
        
        const tags = await response.json();
        this.selectedTags = tags;
      } catch (error) {
        console.error('Error fetching game tags:', error);
      }
    }
  }
};
</script>

<style scoped>
.add-game-container {
  padding: 2rem;
  color: var(--Color-Text-Main, #ffffff);
  background: var(--Gradient-Background-System);
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
}

h1 {
  font-size: 1.8rem;
  margin-bottom: 2rem;
  text-align: center;
  font: var(--Text-HeadingLarge);
}

.form-container {
  max-width: 800px;
  width: 100%;
  background-color: var(--Color-Background-Main, #171a21);
  padding: 2rem;
  border-radius: 8px;
  box-shadow: var(--ShadowLight);
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-row {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.half {
  flex: 1;
  min-width: 0;
  box-sizing: border-box;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: var(--Color-Text-Main);
}

.form-control {
  width: 100%;
  max-width: 100%;
  padding: 0.5rem;
  background-color: var(--Color-Background-System);
  border: none;
  border-radius: 4px;
  color: #fff;
  box-sizing: border-box;
}

.file-upload {
  display: flex;
  gap: 0.5rem;
}

.upload-btn {
  background-color: var(--Color-Secondary);
  color: white;
  border: none;
  border-radius: 4px;
  padding: 0.5rem 1rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.upload-btn:hover {
  background-color: var(--Color-Primary);
}

.showcase-images {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.showcase-image {
  aspect-ratio: 16/9;
  background-color: var(--Color-Background-System);
  border-radius: 4px;
  overflow: hidden;
  position: relative;
}

.showcase-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.showcase-image.empty {
  background-color: var(--Color-Background-Highlight);
}

.showcase-image.processing {
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: var(--Color-Background-System);
  border-radius: 4px;
}

.showcase-image.processing .processing-indicator {
  width: 30px;
  height: 30px;
  border-width: 3px;
}

.showcase-upload {
  justify-content: center;
  margin-top: 1rem;
}

.tags-container {
  position: relative;
}

.tags-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background-color: var(--Color-Background-System);
  border-radius: 0 0 4px 4px;
  max-height: 200px;
  overflow-y: auto;
  z-index: 10;
}

.tag-option {
  padding: 0.5rem;
  cursor: pointer;
}

.tag-option:hover {
  background-color: var(--Color-Secondary);
}

.selected-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.tag {
  background-color: var(--Color-Secondary);
  color: white;
  padding: 0.3rem 0.6rem;
  border-radius: 4px;
  display: flex;
  align-items: center;
  gap: 0.3rem;
  transition: background-color 0.2s;
}

.tag:hover {
  background-color: var(--Color-Primary);
}

.remove-tag {
  cursor: pointer;
  font-size: 1.2rem;
  line-height: 1;
}

.markdown-editor {
  border: 1px solid var(--Color-Background-System);
  border-radius: 4px;
  overflow: hidden;
}

.editor-tabs {
  background-color: var(--Color-Background-Highlight);
  display: flex;
  border-bottom: 1px solid var(--Color-Background-System);
}

.tab-btn {
  background: none;
  border: none;
  color: var(--Color-Text-Dim);
  padding: 0.75rem 1rem;
  cursor: pointer;
  transition: all 0.2s;
  border-bottom: 2px solid transparent;
}

.tab-btn:hover {
  color: var(--Color-Text-Main);
  background-color: var(--Color-Background-System);
}

.tab-btn.active {
  color: var(--Color-Primary);
  border-bottom-color: var(--Color-Primary);
  background-color: var(--Color-Background-Main);
}

.editor-toolbar {
  background-color: var(--Color-Background-Highlight);
  padding: 0.5rem;
  display: flex;
  gap: 0.5rem;
  border-bottom: 1px solid var(--Color-Background-System);
}

.toolbar-btn {
  background-color: var(--Color-Background-System);
  color: white;
  border: none;
  border-radius: 4px;
  min-width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: background-color 0.2s;
  font-size: 0.9rem;
  padding: 0 0.5rem;
}

.toolbar-btn:hover {
  background-color: var(--Color-Secondary);
}

.editor-content {
  width: 100%;
  padding: 1rem;
  background-color: var(--Color-Background-Main);
  border: none;
  color: #fff;
  resize: vertical;
  font-family: 'Courier New', monospace;
  font-size: 14px;
  line-height: 1.5;
}

.markdown-preview {
  padding: 1rem;
  background-color: var(--Color-Background-Main);
  color: var(--Color-Text-Main);
  min-height: 300px;
  line-height: 1.6;
}

.markdown-preview h1,
.markdown-preview h2,
.markdown-preview h3,
.markdown-preview h4,
.markdown-preview h5,
.markdown-preview h6 {
  color: var(--Color-Primary);
  margin: 1em 0 0.5em 0;
}

.markdown-preview h1 { font-size: 2em; }
.markdown-preview h2 { font-size: 1.5em; }
.markdown-preview h3 { font-size: 1.25em; }

.markdown-preview p {
  margin: 1em 0;
}

.markdown-preview ul,
.markdown-preview ol {
  margin: 1em 0;
  padding-left: 2em;
}

.markdown-preview li {
  margin: 0.5em 0;
}

.markdown-preview code {
  background-color: var(--Color-Background-System);
  color: var(--Color-Accent-Green);
  padding: 0.2em 0.4em;
  border-radius: 3px;
  font-family: 'Courier New', monospace;
}

.markdown-preview pre {
  background-color: var(--Color-Background-System);
  color: var(--Color-Text-Main);
  padding: 1em;
  border-radius: 4px;
  overflow-x: auto;
}

.markdown-preview pre code {
  background: none;
  padding: 0;
}

.markdown-preview a {
  color: var(--Color-Primary);
  text-decoration: none;
}

.markdown-preview a:hover {
  text-decoration: underline;
}

.markdown-preview blockquote {
  border-left: 4px solid var(--Color-Primary);
  margin: 1em 0;
  padding: 0.5em 1em;
  background-color: var(--Color-Background-Highlight);
  color: var(--Color-Text-Dim);
}

.preview-placeholder {
  color: var(--Color-Text-Dim);
  font-style: italic;
}

.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
  justify-content: center;
}

.publish-btn {
  background: var(--Gradient-Button-Action);
  color: white;
  border: none;
  border-radius: 4px;
  padding: 0.7rem 2rem;
  cursor: pointer;
  font-weight: bold;
  transition: background 0.2s;
}

.publish-btn:hover:not(:disabled) {
  background: var(--Color-Primary);
}

.publish-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.processing-indicator {
  display: inline-block;
  margin-right: 8px;
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  border-top-color: #fff;
  animation: spin 1s infinite linear;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.cancel-btn {
  background-color: var(--Color-Background-System);
  color: white;
  border: none;
  border-radius: 4px;
  padding: 0.7rem 2rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.cancel-btn:hover:not(:disabled) {
  background-color: var(--Color-Accent-Red);
}

.cancel-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.validation-errors {
  margin-top: 1rem;
  padding: 1rem;
  background-color: var(--Color-Accent-Red);
  border-radius: 4px;
  color: white;
}

.error-title {
  font-size: 1.2rem;
  font-weight: bold;
  margin-bottom: 0.5rem;
}

.error-list {
  list-style-type: disc;
  padding-left: 20px;
}

.banner-preview {
  margin-top: 0.5rem;
  position: relative;
  max-width: 400px;
  border-radius: 4px;
  overflow: hidden;
}

.banner-preview img {
  width: 100%;
  height: auto;
  display: block;
}

.remove-image-btn {
  position: absolute;
  top: 5px;
  right: 5px;
  background-color: rgba(0, 0, 0, 0.6);
  color: white;
  border: none;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 16px;
  transition: background-color 0.2s;
}

.remove-image-btn:hover {
  background-color: var(--Color-Accent-Red);
}

/* Specific styles for date and number inputs */
input[type="date"],
input[type="number"] {
  min-width: 120px;
  font-size: 14px;
}

input[type="date"] {
  color-scheme: dark;
}

/* Responsive design */
@media (max-width: 768px) {
  .form-row {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .half {
    flex: none;
    width: 100%;
  }
  
  input[type="date"],
  input[type="number"] {
    min-width: 100px;
    font-size: 16px; /* Prevents zoom on iOS */
  }
}

@media (max-width: 480px) {
  .form-container {
    padding: 1rem;
  }
  
  .add-game-container {
    padding: 1rem;
  }
  
  input[type="date"],
  input[type="number"] {
    font-size: 16px;
  }
}
</style> 