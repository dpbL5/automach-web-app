<template>
  <div class="account-detail-container">
    <div class="sidebar">
      <div class="sidebar-header">
        <h3>Account Settings</h3>
      </div>
      <div class="sidebar-menu">
        <div :class="['sidebar-item', { active: tab === 'account' }]" @click="changeTab('account')">
          <span class="sidebar-icon">👤</span>
          Profile Settings
        </div>
        <div :class="['sidebar-item', { active: tab === 'transactions' }]" @click="changeTab('transactions')">
          <span class="sidebar-icon">💰</span>
          {{ isAdmin ? 'System Transactions' : 'Purchase History' }}
        </div>
        <div :class="['sidebar-item', { active: tab === 'reviews' }]" @click="changeTab('reviews')">
          <span class="sidebar-icon">⭐</span>
          {{ isAdmin ? 'All User Reviews' : 'My Reviews' }}
        </div>
        <div v-if="isAdmin" :class="['sidebar-item', { active: tab === 'users' }]" @click="changeTab('users')">
          <span class="sidebar-icon">👥</span>
          All Users
        </div>
      </div>
    </div>
    
    <div class="main-content">
      <template v-if="tab === 'account'">
        <div class="section-header">
          <h2>Profile Settings</h2>
          <p>Manage your account information and security settings</p>
        </div>
        
        <div class="profile-section">
          <div class="profile-card">
            <div class="profile-avatar">
              <span class="avatar-text">{{ getInitials() }}</span>
            </div>
            <div class="profile-info">
              <h3>{{ account?.name || account?.username || 'User' }}</h3>
              <p class="profile-role">{{ isAdmin ? 'Administrator' : 'User' }}</p>
            </div>
          </div>
          
          <div class="info-grid">
            <div class="info-card">
              <div class="info-label">Profile Name</div>
              <div class="info-value">{{ account?.name || account?.username }}</div>
            </div>
            
            <div class="info-card">
              <div class="info-label">Email Address</div>
              <div class="info-value">{{ maskEmail(account?.email) }}</div>
              <button v-if="!showChangeEmail" @click="showChangeEmail = true" class="action-btn">
                Change Email
              </button>
            </div>
            
            <div class="info-card">
              <div class="info-label">Password</div>
              <div class="info-value">••••••••</div>
              <button v-if="!showChangePassword" @click="showChangePassword = true" class="action-btn">
                Change Password
              </button>
            </div>
          </div>

          <!-- Change Email Form -->
          <div v-if="showChangeEmail" class="form-overlay">
            <div class="form-card">
              <h4>Change Email Address</h4>
              <div class="form-group">
                <label>New Email</label>
                <input v-model="newEmail" type="email" placeholder="Enter new email address" />
              </div>
              <div class="form-actions">
                <button @click="submitChangeEmail" class="btn-primary">Save Changes</button>
                <button @click="showChangeEmail = false" class="btn-secondary">Cancel</button>
              </div>
              <div v-if="emailMsg" :class="['form-message', emailMsgColor === 'green' ? 'success' : 'error']">
                {{ emailMsg }}
              </div>
            </div>
          </div>
          
          <!-- Change Password Form -->
          <div v-if="showChangePassword" class="form-overlay">
            <div class="form-card">
              <h4>Change Password</h4>
              <div class="form-group">
                <label>New Password</label>
                <input v-model="newPassword" type="password" placeholder="Enter new password" />
              </div>
              <div class="form-group">
                <label>Confirm Password</label>
                <input v-model="confirmPassword" type="password" placeholder="Confirm new password" />
              </div>
              <div class="form-actions">
                <button @click="submitChangePassword" class="btn-primary">Save Changes</button>
                <button @click="showChangePassword = false" class="btn-secondary">Cancel</button>
              </div>
              <div v-if="passwordMsg" :class="['form-message', passwordMsgColor === 'green' ? 'success' : 'error']">
                {{ passwordMsg }}
              </div>
            </div>
          </div>
        </div>
      </template>
      <template v-else-if="tab === 'transactions'">
        <div class="section-header">
          <h2>{{ isAdmin ? 'All System Transactions' : 'Purchase History' }}</h2>
          <p>{{ isAdmin ? 'View all transactions in the system' : 'View your game purchase history' }}</p>
        </div>
        
        <div v-if="isAdmin" class="filter-section">
          <div class="search-box">
            <input 
              v-model="transactionSearch" 
              placeholder="Search by user or payment method..." 
              class="search-input"
            />
            <span class="search-icon">🔍</span>
          </div>
        </div>
        
        <div class="transactions-container">
          <div v-if="filteredTransactions.length === 0" class="empty-state">
            <div class="empty-icon">📦</div>
            <h3>No transactions found</h3>
            <p>{{ isAdmin ? 'No transactions in the system yet.' : 'You haven\'t made any purchases yet.' }}</p>
          </div>
          
          <div v-else class="transactions-list">
            <div v-for="tx in filteredTransactions" :key="tx.id" class="transaction-card">
              <div class="transaction-header">
                <div class="transaction-info">
                  <div class="transaction-date">{{ formatDate(tx.createdAt) }}</div>
                  <div v-if="isAdmin" class="transaction-user">{{ getUserName(tx.accountId) }}</div>
                  <div class="transaction-method">{{ tx.paymentMethod }}</div>
                </div>
                <div class="transaction-total">
                  <span class="total-amount">${{ tx.totalPrice.toFixed(2) }}</span>
                </div>
              </div>
              
              <div class="transaction-games">
                <h4>Games Purchased:</h4>
                <div class="games-list">
                  <div v-for="gameId in tx.gameIds" :key="gameId" class="game-item">
                    <span class="game-name">{{ getGameTitle(gameId) }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </template>
      <template v-else-if="tab === 'reviews'">
        <div class="section-header">
          <h2>{{ isAdmin ? 'All User Reviews' : 'My Reviews' }}</h2>
          <p>{{ isAdmin ? 'Manage all user reviews in the system' : 'View and manage your game reviews' }}</p>
        </div>
        
        <div v-if="isAdmin" class="filter-section">
          <div class="search-box">
            <input 
              v-model="reviewSearch" 
              placeholder="Search by user or game..." 
              class="search-input"
            />
            <span class="search-icon">🔍</span>
          </div>
          <div class="admin-notice">
            <span class="notice-icon">ℹ️</span>
            Admin can view all reviews but can only edit or delete their own reviews
          </div>
        </div>
        
        <div class="reviews-container">
          <div v-if="filteredReviews.length === 0" class="empty-state">
            <div class="empty-icon">⭐</div>
            <h3>No reviews found</h3>
            <p>{{ isAdmin ? 'No reviews in the system yet.' : 'You haven\'t written any reviews yet.' }}</p>
          </div>
          
          <div v-else class="reviews-list">
            <div v-for="review in filteredReviews" :key="review.id" class="review-card">
              <div class="review-header">
                <div class="review-info">
                  <div class="review-game">{{ getGameTitle(review.gameId) }}</div>
                  <div class="review-meta">
                    <span v-if="isAdmin" class="reviewer-name">{{ getUserName(review.accountId) }}</span>
                    <span class="review-date">{{ formatDate(review.createdAt) }}</span>
                  </div>
                </div>
                <div class="review-rating">
                  <span class="rating-score">{{ review.rating }}</span>
                  <span class="rating-label">/100</span>
                </div>
              </div>
              
              <div class="review-content">
                <div v-if="editReviewId !== review.id" class="review-text">
                  {{ review.content }}
                </div>
                <div v-else class="edit-form">
                  <textarea v-model="editReviewContent" class="edit-textarea"></textarea>
                  <div class="rating-input">
                    <label>Rating:</label>
                    <input 
                      v-model.number="editReviewRating" 
                      type="number" 
                      min="1" 
                      max="100" 
                      class="rating-input-field"
                    />
                  </div>
                </div>
              </div>
              
              <div class="review-actions">
                <template v-if="!isAdmin || (isAdmin && review.accountId === account?.id)">
                  <button 
                    v-if="editReviewId !== review.id" 
                    @click="startEditReview(review)" 
                    class="action-btn edit-btn"
                  >
                    Edit
                  </button>
                  <button 
                    v-else 
                    @click="submitEditReview(review)" 
                    class="action-btn save-btn"
                  >
                    Save
                  </button>
                  <button 
                    v-if="editReviewId === review.id" 
                    @click="cancelEditReview" 
                    class="action-btn cancel-btn"
                  >
                    Cancel
                  </button>
                  <button 
                    @click="deleteReview(review.id)" 
                    class="action-btn delete-btn"
                  >
                    Delete
                  </button>
                </template>
                <span v-else class="admin-view-only">View only</span>
              </div>
            </div>
          </div>
        </div>
        
        <div v-if="editReviewMsg" :class="['form-message', editReviewMsgColor === 'green' ? 'success' : 'error']">
          {{ editReviewMsg }}
        </div>
      </template>

      <template v-else-if="tab === 'users'">
        <div class="section-header">
          <h2>System Users</h2>
          <p>Manage all user accounts in the system</p>
        </div>
        
        <div class="filter-section">
          <div class="search-box">
            <input 
              v-model="userSearch" 
              placeholder="Search by name, email, or username..." 
              class="search-input"
            />
            <span class="search-icon">🔍</span>
          </div>
          <div class="stats-row">
            <div class="stat-card">
              <span class="stat-number">{{ allUsers.length }}</span>
              <span class="stat-label">Total Users</span>
            </div>
            <div class="stat-card">
              <span class="stat-number">{{ adminCount }}</span>
              <span class="stat-label">Admins</span>
            </div>
            <div class="stat-card">
              <span class="stat-number">{{ userCount }}</span>
              <span class="stat-label">Regular Users</span>
            </div>
            <div class="stat-card revenue-card">
              <span class="stat-number">${{ totalRevenue.toFixed(2) }}</span>
              <span class="stat-label">Total Revenue</span>
            </div>
          </div>
        </div>
        
        <div class="users-container">
          <div v-if="filteredUsers.length === 0" class="empty-state">
            <div class="empty-icon">👤</div>
            <h3>No users found</h3>
            <p>No users match your search criteria.</p>
          </div>
          
          <div v-else class="users-list">
            <div v-for="user in filteredUsers" :key="user.id || user.Id" class="user-card">
              <div class="user-header">
                <div class="user-avatar">
                  <span class="avatar-text">{{ getUserInitials(user) }}</span>
                </div>
                <div class="user-info">
                  <div class="user-name">{{ user.name || user.username || 'Unknown User' }}</div>
                  <div class="user-email">{{ user.email }}</div>
                  <div class="user-meta">
                    <span class="user-role" :class="user.role === 'admin' ? 'admin-role' : 'user-role'">
                      {{ user.role === 'admin' ? 'Administrator' : 'User' }}
                    </span>
                    <span class="user-date">Joined: {{ formatDate(user.createdAt) }}</span>
                  </div>
                </div>
                                 <div class="user-actions">
                   <button 
                     @click="viewUserDetails(user)" 
                     class="action-btn view-btn"
                   >
                     View Details
                   </button>
                 </div>
              </div>
              
                             <div v-if="user.role !== 'admin'" class="user-stats">
                 <div class="stat-item">
                   <span class="stat-value">{{ getUserReviewCount(user.id || user.Id) }}</span>
                   <span class="stat-name">Reviews</span>
                 </div>
                 <div class="stat-item">
                   <span class="stat-value">{{ getUserTransactionCount(user.id || user.Id) }}</span>
                   <span class="stat-name">Purchases</span>
                 </div>
                 <div class="stat-item">
                   <span class="stat-value">${{ getUserTotalSpent(user.id || user.Id) }}</span>
                   <span class="stat-name">Total Spent</span>
                 </div>
               </div>
            </div>
          </div>
        </div>
        
        <!-- User Details Modal -->
        <div v-if="selectedUser" class="form-overlay">
          <div class="user-details-modal">
            <div class="modal-header">
              <h3>User Details</h3>
              <button @click="selectedUser = null" class="close-btn">✕</button>
            </div>
            <div class="modal-content">
              <div class="user-detail-grid">
                <div class="detail-item">
                  <label>Name:</label>
                  <span>{{ selectedUser.name || 'Not provided' }}</span>
                </div>
                <div class="detail-item">
                  <label>Username:</label>
                  <span>{{ selectedUser.username }}</span>
                </div>
                <div class="detail-item">
                  <label>Email:</label>
                  <span>{{ selectedUser.email }}</span>
                </div>
                <div class="detail-item">
                  <label>Phone:</label>
                  <span>{{ selectedUser.phoneNumber || 'Not provided' }}</span>
                </div>
                <div class="detail-item">
                  <label>Role:</label>
                  <span class="role-badge" :class="selectedUser.role === 'admin' ? 'admin-role' : 'user-role'">
                    {{ selectedUser.role === 'admin' ? 'Administrator' : 'User' }}
                  </span>
                </div>
                <div class="detail-item">
                  <label>Member Since:</label>
                  <span>{{ formatDate(selectedUser.createdAt) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
        
      </template>
    </div>
  </div>
</template>

<script>
import { cacheManager } from '../helpers/cacheManager.js';

export default {
  data() {
    return {
      tab: 'account',
      account: null,
      transactions: [],
      allTransactions: [],
      reviews: [],
      allReviews: [],
      accounts: [],
      games: [],
      showChangeEmail: false,
      showChangePassword: false,
      newEmail: '',
      emailMsg: '',
      emailMsgColor: 'red',
      newPassword: '',
      confirmPassword: '',
      passwordMsg: '',
      passwordMsgColor: 'red',
      editReviewId: null,
      editReviewContent: '',
      editReviewRating: 1,
      editReviewMsg: '',
      editReviewMsgColor: 'red',
      transactionSearch: '',
      reviewSearch: '',
      userSearch: '',
      allUsers: [],
      selectedUser: null,
    };
  },
  computed: {
    isAdmin() {
      return localStorage.getItem('role') === 'admin';
    },
    filteredReviews() {
      if (this.isAdmin) {
        if (!this.reviewSearch) return this.allReviews;
        const searchLower = this.reviewSearch.toLowerCase();
        return this.allReviews.filter(review => {
          const userName = this.getUserName(review.accountId).toLowerCase();
          const gameTitle = this.getGameTitle(review.gameId).toLowerCase();
          return userName.includes(searchLower) || gameTitle.includes(searchLower) || 
                 review.content.toLowerCase().includes(searchLower);
        });
      } else {
        if (!this.account) return [];
        return this.reviews.filter(r => {
          const reviewAccountId = r.accountId || r.AccountId;
          const accountId = this.account.id || this.account.Id;
          return String(reviewAccountId) === String(accountId);
        });
      }
    },
    filteredTransactions() {
      if (this.isAdmin) {
        if (!this.transactionSearch) return this.allTransactions;
        const searchLower = this.transactionSearch.toLowerCase();
        return this.allTransactions.filter(tx => {
          const userName = this.getUserName(tx.accountId).toLowerCase();
          const paymentMethod = tx.paymentMethod.toLowerCase();
          return userName.includes(searchLower) || paymentMethod.includes(searchLower);
        });
      } else {
        return this.transactions;
      }
    },
    filteredUsers() {
      if (!this.userSearch) return this.allUsers;
      const searchLower = this.userSearch.toLowerCase();
      return this.allUsers.filter(user => {
        const name = (user.name || '').toLowerCase();
        const email = (user.email || '').toLowerCase();
        const username = (user.username || '').toLowerCase();
        return name.includes(searchLower) || email.includes(searchLower) || username.includes(searchLower);
      });
    },
    adminCount() {
      return this.allUsers.filter(user => user.role === 'admin').length;
    },
    userCount() {
      return this.allUsers.filter(user => user.role === 'user').length;
    },
    totalRevenue() {
      if (!this.allTransactions) return 0;
      return this.allTransactions.reduce((total, tx) => {
        return total + (tx.totalPrice || 0);
      }, 0);
    },
    monthlyRevenue() {
      if (!this.allTransactions) return 0;
      const currentMonth = new Date().getMonth();
      const currentYear = new Date().getFullYear();
      return this.allTransactions.reduce((total, tx) => {
        const txDate = new Date(tx.createdAt);
        if (txDate.getMonth() === currentMonth && txDate.getFullYear() === currentYear) {
          return total + (tx.totalPrice || 0);
        }
        return total;
      }, 0);
    },
    quarterlyRevenue() {
      if (!this.allTransactions) return 0;
      const currentQuarter = this.getCurrentQuarter();
      const currentYear = new Date().getFullYear();
      return this.allTransactions.reduce((total, tx) => {
        const txDate = new Date(tx.createdAt);
        const txQuarter = Math.floor(txDate.getMonth() / 3) + 1;
        if (txQuarter === currentQuarter && txDate.getFullYear() === currentYear) {
          return total + (tx.totalPrice || 0);
        }
        return total;
      }, 0);
    },
    totalTransactionCount() {
      return this.allTransactions ? this.allTransactions.length : 0;
    },
    avgTransactionValue() {
      if (!this.allTransactions || this.allTransactions.length === 0) return 0;
      return this.totalRevenue / this.allTransactions.length;
    },
    totalReviews() {
      return this.allReviews ? this.allReviews.length : 0;
    },
    topGamesByRevenue() {
      if (!this.allTransactions) return [];
      
      const gameStats = {};
      this.allTransactions.forEach(tx => {
        tx.gameIds.forEach(gameId => {
          if (!gameStats[gameId]) {
            gameStats[gameId] = { gameId, sales: 0, revenue: 0 };
          }
          gameStats[gameId].sales++;
          gameStats[gameId].revenue += tx.totalPrice / tx.gameIds.length; // Approximate revenue per game
        });
      });
      
      return Object.values(gameStats)
        .sort((a, b) => b.revenue - a.revenue)
        .slice(0, 5);
    },
    recentTransactions() {
      if (!this.allTransactions) return [];
      return [...this.allTransactions]
        .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
        .slice(0, 5);
    }
  },
  methods: {
    async fetchAccount() {
      let userId = localStorage.getItem('userId');
      if (!userId) {
        const token = localStorage.getItem('token');
        if (token) {
          const res = await fetch('http://localhost:5174/api/auth/me', {
            headers: { Authorization: `Bearer ${token}` }
          });
          if (res.ok) {
            const data = await res.json();
            userId = data.Id;
            localStorage.setItem('userId', userId);
          }
        }
      }
      if (!userId) return;

      // Sử dụng cacheManager để fetch user data
      try {
        this.account = await cacheManager.fetchWithCache('user', async () => {
          const res = await fetch(`http://localhost:5174/api/accounts/${userId}`, {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
          });
          if (!res.ok) throw new Error('Failed to fetch account');
          return await res.json();
        });
      } catch (error) {
        console.error('Error fetching account:', error);
      }
      
      if (this.isAdmin) {
        // Sử dụng cacheManager để fetch accounts
        try {
          this.accounts = await cacheManager.fetchWithCache('accounts', async () => {
            const res = await fetch('http://localhost:5174/api/accounts', {
              headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
            });
            if (!res.ok) throw new Error('Failed to fetch accounts');
            return await res.json();
          });
          this.allUsers = this.accounts; // Copy to allUsers for user management
        } catch (error) {
          console.error('Error fetching accounts:', error);
        }
        
        // Fetch all transactions for admin
        try {
          const allTxRes = await fetch('http://localhost:5174/api/transactions', {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
          });
          if (allTxRes.ok) {
            this.allTransactions = await allTxRes.json();
            console.log('All transactions fetched:', this.allTransactions);
          }
        } catch (error) {
          console.error('Error fetching all transactions:', error);
          this.allTransactions = [];
        }
      }
      
      // Fetch user transactions (both admin and regular users see their own transactions)
      try {
        const txRes = await fetch(`http://localhost:5174/api/accounts/${userId}/transactions`, {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
        });
        if (txRes.ok) {
          let txs = await txRes.json();
          console.log('User transactions fetched:', txs);
          this.transactions = txs;
        }
      } catch (error) {
        console.error('Error fetching user transactions:', error);
        this.transactions = [];
      }
    },
    async fetchReviewsAndGames() {
      try {
        console.log("Fetching reviews and games...");
        const res = await fetch('http://localhost:5174/api/review');
        let allReviews = [];
        if (res.ok) {
          allReviews = await res.json();
          console.log("Reviews fetched:", allReviews);
          this.allReviews = allReviews;
        } else {
          console.error("Failed to fetch reviews:", await res.text());
        }
        
        let userId = this.account?.id || localStorage.getItem('userId');
        console.log("Current userId:", userId);
        this.reviews = allReviews.filter(r => r.accountId == userId);
        console.log("Filtered reviews:", this.reviews);
        
        // Get all game IDs from reviews to make sure we fetch all needed games
        const gameIdsFromReviews = new Set();
        allReviews.forEach(review => {
          if (review.gameId) {
            gameIdsFromReviews.add(review.gameId);
          }
        });
        
        // Sử dụng cacheManager để fetch games
        try {
          this.games = await cacheManager.fetchWithCache('games', async () => {
            const gameRes = await fetch('http://localhost:5174/api/games?pageSize=100');
            if (!gameRes.ok) throw new Error('Failed to fetch games');
            
            const gamesData = await gameRes.json();
            console.log("Games data fetched:", gamesData);
            
            let allGames = [];
            
            // Handle the paginated response
            if (gamesData.items) {
              allGames = gamesData.items;
              
              // If there are more games than the page size, fetch more
              if (gamesData.totalCount > gamesData.items.length) {
                const remainingPages = Math.ceil(gamesData.totalCount / 100) - 1;
                for (let page = 2; page <= remainingPages + 1; page++) {
                  const additionalGamesRes = await fetch(`http://localhost:5174/api/games?pageNumber=${page}&pageSize=100`);
                  if (additionalGamesRes.ok) {
                    const additionalGamesData = await additionalGamesRes.json();
                    allGames = [...allGames, ...additionalGamesData.items];
                  }
                }
              }
            } else {
              allGames = gamesData;
            }
            
            console.log("Processed games:", allGames);
            return allGames;
          });
        } catch (error) {
          console.error("Error fetching games:", error);
        }
        
        // Make sure we have all games from reviews
        const missingGameIds = [...gameIdsFromReviews].filter(
          gameId => !this.games.some(g => g.id === gameId || g.Id === gameId)
        );
        
        // Fetch any missing games individually
        for (const gameId of missingGameIds) {
          try {
            const singleGameRes = await fetch(`http://localhost:5174/api/games/${gameId}`);
            if (singleGameRes.ok) {
              const game = await singleGameRes.json();
              this.games.push(game);
              // Cập nhật cache với game mới
              cacheManager.games.set(this.games);
            }
          } catch (error) {
            console.error(`Error fetching game ${gameId}:`, error);
          }
        }
      } catch (error) {
        console.error("Error in fetchReviewsAndGames:", error);
      }
    },
    getGameTitle(gameId) {
      if (!gameId) return 'Unknown';
      
      const game = this.games.find(g => 
        String(g.id) === String(gameId) || 
        String(g.Id) === String(gameId)
      );
      
      if (!game) {
        console.warn(`Game with ID ${gameId} not found!`);
        return 'Unknown Game';
      }
      
      // Try all possible property case variants for title
      return game.title || game.Title || game.TITLE || 'Unknown Game';
    },
    getUserName(accountId) {
      const account = this.accounts.find(a => a.id === accountId || a.Id === accountId);
      return account ? (account.name || account.username || 'Unknown User') : 'Unknown User';
    },
    getInitials() {
      const name = this.account?.name || this.account?.username || 'User';
      return name.split(' ').map(word => word.charAt(0)).join('').substring(0, 2).toUpperCase();
    },
    formatDate(dateStr) {
      const d = new Date(dateStr);
      return d.toLocaleDateString();
    },
    maskEmail(email) {
      if (!email) return '';
      const [name, domain] = email.split('@');
      return name.slice(0, 1) + '***@' + domain;
    },
    editReview(review) {
      alert('Edit review: ' + review.id);
    },
    async deleteReview(id) {
      if (!confirm('Delete this review?')) return;
      
      // Kiểm tra nếu người dùng là admin và không phải là chủ sở hữu review
      const review = this.allReviews.find(r => r.id === id);
      if (this.isAdmin && review && review.accountId !== this.account?.id) {
        alert('Admin cannot delete reviews owned by other users');
        return;
      }
      
      const res = await fetch(`http://localhost:5174/api/review/${id}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
      });
      if (res.ok) {
        this.reviews = this.reviews.filter(r => r.id !== id);
        this.allReviews = this.allReviews.filter(r => r.id !== id);
      }
    },
    async submitChangeEmail() {
      this.emailMsg = '';
      if (!this.newEmail || !this.newEmail.includes('@')) {
        this.emailMsg = 'Invalid email!';
        this.emailMsgColor = 'red';
        return;
      }
      const userId = this.account?.id;
      if (!userId) return;
      const body = {
        name: this.account.name || '',
        username: this.account.username || '',
        phoneNumber: this.account.phoneNumber || '',
        email: this.newEmail,
        password: this.account.password || ''
      };
      const res = await fetch(`http://localhost:5174/api/accounts/${userId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(body)
      });
      if (res.ok) {
        this.emailMsg = 'Email changed successfully!';
        this.emailMsgColor = 'green';
        this.account.email = this.newEmail;
        this.showChangeEmail = false;
        this.newEmail = '';
        // Cập nhật cache với thông tin mới
        cacheManager.user.update({ email: this.newEmail });
      } else {
        this.emailMsg = 'Failed to change email!';
        this.emailMsgColor = 'red';
      }
    },
    async submitChangePassword() {
      this.passwordMsg = '';
      if (!this.newPassword || this.newPassword.length < 6) {
        this.passwordMsg = 'Password must be at least 6 characters!';
        this.passwordMsgColor = 'red';
        return;
      }
      if (this.newPassword !== this.confirmPassword) {
        this.passwordMsg = 'Passwords do not match!';
        this.passwordMsgColor = 'red';
        return;
      }
      const userId = this.account?.id;
      if (!userId) return;
      const body = {
        name: this.account.name || '',
        username: this.account.username || '',
        phoneNumber: this.account.phoneNumber || '',
        email: this.account.email || '',
        password: this.newPassword
      };
      const res = await fetch(`http://localhost:5174/api/accounts/${userId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(body)
      });
      if (res.ok) {
        this.passwordMsg = 'Password changed successfully!';
        this.passwordMsgColor = 'green';
        this.showChangePassword = false;
        this.newPassword = '';
        this.confirmPassword = '';
        // Cache sẽ tự động expire, không cần update password trong cache
      } else {
        this.passwordMsg = 'Failed to change password!';
        this.passwordMsgColor = 'red';
      }
    },
    startEditReview(review) {
      // Kiểm tra nếu người dùng là admin và không phải là chủ sở hữu review
      if (this.isAdmin && review.accountId !== this.account?.id) {
        alert('Admin cannot edit reviews owned by other users');
        return;
      }
      
      this.editReviewId = review.id;
      this.editReviewContent = review.content;
      this.editReviewRating = review.rating;
      this.editReviewMsg = '';
    },
    cancelEditReview() {
      this.editReviewId = null;
      this.editReviewContent = '';
      this.editReviewRating = 1;
      this.editReviewMsg = '';
    },
    async submitEditReview(review) {
      if (!this.editReviewContent || this.editReviewRating < 1 || this.editReviewRating > 100) {
        this.editReviewMsg = 'Invalid content or rating!';
        this.editReviewMsgColor = 'red';
        return;
      }
      const res = await fetch(`http://localhost:5174/api/review/${review.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({
          content: this.editReviewContent,
          rating: this.editReviewRating
        })
      });
      if (res.ok) {
        // Update in both review arrays
        const updateInArray = (arr) => {
          const idx = arr.findIndex(r => r.id === review.id);
          if (idx !== -1) {
            arr[idx].content = this.editReviewContent;
            arr[idx].rating = this.editReviewRating;
          }
        };
        
        updateInArray(this.reviews);
        updateInArray(this.allReviews);
        
        this.editReviewMsg = 'Review updated!';
        this.editReviewMsgColor = 'green';
        this.cancelEditReview();
      } else {
        this.editReviewMsg = 'Failed to update review!';
        this.editReviewMsgColor = 'red';
      }
    },
    async changeTab(newTab) {
      console.log("Changing tab to:", newTab);
      this.tab = newTab;
      
      // Debug: Log cache status
      console.log('Cache status:', cacheManager.getStatus());
      
      // If switching to reviews tab, refresh the reviews data
      if (newTab === 'reviews') {
        await this.fetchReviewsAndGames();
      }
      
      // If switching to users tab and admin, ensure we have all users
      if (newTab === 'users' && this.isAdmin && this.allUsers.length === 0) {
        await this.fetchAccount();
      }
    },
    getUserInitials(user) {
      const name = user.name || user.username || 'User';
      return name.split(' ').map(word => word.charAt(0)).join('').substring(0, 2).toUpperCase();
    },
    getUserReviewCount(userId) {
      return this.allReviews.filter(review => 
        String(review.accountId) === String(userId)
      ).length;
    },
    getUserTransactionCount(userId) {
      return this.allTransactions.filter(tx => 
        String(tx.accountId) === String(userId)
      ).length;
    },
    getUserTotalSpent(userId) {
      const userTransactions = this.allTransactions.filter(tx => 
        String(tx.accountId) === String(userId)
      );
      const total = userTransactions.reduce((sum, tx) => sum + (tx.totalPrice || 0), 0);
      return total.toFixed(2);
    },

    viewUserDetails(user) {
      this.selectedUser = user;
    },
    getCurrentQuarter() {
      return Math.floor(new Date().getMonth() / 3) + 1;
    },
    getCurrentMonthName() {
      const months = ['January', 'February', 'March', 'April', 'May', 'June',
                      'July', 'August', 'September', 'October', 'November', 'December'];
      return months[new Date().getMonth()];
    },
  },
  async mounted() {
    await this.fetchAccount();
    await this.fetchReviewsAndGames();
  }
};
</script>

<style scoped>
.account-detail-container {
  display: flex;
  min-height: 100vh;
  background: var(--Gradient-Background-Main);
  color: var(--Color-Text-Main);
}

/* Sidebar Styles */
.sidebar {
  width: 280px;
  background: var(--Color-Background-Highlight);
  border-radius: 0 8px 8px 0;
  box-shadow: var(--ShadowLight);
}

.sidebar-header {
  padding: var(--Gap-Large);
  border-bottom: 1px solid var(--Color-Background-System);
}

.sidebar-header h3 {
  margin: 0;
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingMedium);
}

.sidebar-menu {
  padding: var(--Gap-Medium) 0;
}

.sidebar-item {
  display: flex;
  align-items: center;
  gap: var(--Gap-Medium);
  padding: var(--Gap-Medium) var(--Gap-Large);
  cursor: pointer;
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
  font-weight: 500;
  transition: all 0.2s ease;
  border-left: 3px solid transparent;
}

.sidebar-item:hover {
  background: var(--Color-Background-System);
  color: var(--Color-Text-Main);
}

.sidebar-item.active {
  background: var(--Color-Background-System);
  color: var(--Color-Primary);
  border-left-color: var(--Color-Primary);
}

.sidebar-icon {
  font-size: 18px;
}

/* Main Content */
.main-content {
  flex: 1;
  padding: var(--Gap-Large);
  max-width: calc(100% - 280px);
}

.section-header {
  margin-bottom: var(--Gap-Large);
}

.section-header h2 {
  margin: 0 0 var(--Gap-Small) 0;
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingLarge);
}

.section-header p {
  margin: 0;
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
}

/* Profile Section */
.profile-section {
  max-width: 800px;
}

.profile-card {
  display: flex;
  align-items: center;
  gap: var(--Gap-Large);
  background: var(--Color-Background-Highlight);
  padding: var(--Gap-Large);
  border-radius: 8px;
  margin-bottom: var(--Gap-Large);
  box-shadow: var(--ShadowLight);
}

.profile-avatar {
  width: 80px;
  height: 80px;
  background: var(--Color-Primary);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.avatar-text {
  color: white;
  font-size: 28px;
  font-weight: 700;
}

.profile-info h3 {
  margin: 0 0 var(--Gap-Small) 0;
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingMedium);
}

.profile-role {
  margin: 0;
  color: var(--Color-Primary);
  font: var(--Text-BodyMedium);
  font-weight: 600;
}

.info-grid {
  display: grid;
  gap: var(--Gap-Medium);
}

.info-card {
  background: var(--Color-Background-Highlight);
  padding: var(--Gap-Large);
  border-radius: 8px;
  box-shadow: var(--ShadowLight);
}

.info-label {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: var(--Gap-Small);
}

.info-value {
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  margin-bottom: var(--Gap-Medium);
}

.action-btn {
  background: var(--Color-Primary);
  color: white;
  border: none;
  border-radius: 4px;
  padding: 8px 16px;
  font: var(--Text-BodySmall);
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.action-btn:hover {
  background: var(--Color-Primary-Dark);
  transform: translateY(-1px);
}

/* Form Overlay */
.form-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.form-card {
  background: var(--Color-Background-Highlight);
  padding: var(--Gap-Large);
  border-radius: 8px;
  box-shadow: var(--ShadowHeavy);
  max-width: 400px;
  width: 90%;
}

.form-card h4 {
  margin: 0 0 var(--Gap-Medium) 0;
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingSmall);
}

.form-group {
  margin-bottom: var(--Gap-Medium);
}

.form-group label {
  display: block;
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  font-weight: 600;
  margin-bottom: var(--Gap-Small);
}

.form-group input {
  width: 100%;
  padding: 12px;
  border: 1px solid var(--Color-Background-System);
  border-radius: 4px;
  background: var(--Color-Background-Main);
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
}

.form-group input:focus {
  outline: none;
  border-color: var(--Color-Primary);
}

.form-actions {
  display: flex;
  gap: var(--Gap-Medium);
  justify-content: flex-end;
}

.btn-primary {
  background: var(--Color-Primary);
  color: white;
  border: none;
  border-radius: 4px;
  padding: 12px 24px;
  font: var(--Text-BodyMedium);
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary:hover {
  background: var(--Color-Primary-Dark);
}

.btn-secondary {
  background: var(--Color-Background-System);
  color: var(--Color-Text-Main);
  border: 1px solid var(--Color-Background-Hover);
  border-radius: 4px;
  padding: 12px 24px;
  font: var(--Text-BodyMedium);
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-secondary:hover {
  background: var(--Color-Background-Hover);
}

.form-message {
  margin-top: var(--Gap-Medium);
  padding: 12px;
  border-radius: 4px;
  font: var(--Text-BodyMedium);
}

.form-message.success {
  background: rgba(76, 175, 80, 0.1);
  color: #4caf50;
  border: 1px solid #4caf50;
}

.form-message.error {
  background: rgba(244, 67, 54, 0.1);
  color: #f44336;
  border: 1px solid #f44336;
}

/* Filter Section */
.filter-section {
  margin-bottom: var(--Gap-Large);
}

.search-box {
  position: relative;
  max-width: 400px;
}

.search-input {
  width: 100%;
  padding: 12px 40px 12px 16px;
  border: 1px solid var(--Color-Background-System);
  border-radius: 8px;
  background: var(--Color-Background-Highlight);
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
}

.search-input:focus {
  outline: none;
  border-color: var(--Color-Primary);
}

.search-icon {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--Color-Text-Dim);
}

.admin-notice {
  display: flex;
  align-items: center;
  gap: var(--Gap-Small);
  margin-top: var(--Gap-Medium);
  padding: 12px;
  background: rgba(255, 193, 7, 0.1);
  border: 1px solid var(--Color-Accent-Yellow);
  border-radius: 4px;
  color: var(--Color-Accent-Yellow);
  font: var(--Text-BodySmall);
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: var(--Gap-XLarge);
  color: var(--Color-Text-Dim);
}

.empty-icon {
  font-size: 48px;
  margin-bottom: var(--Gap-Medium);
}

.empty-state h3 {
  margin: 0 0 var(--Gap-Small) 0;
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingMedium);
}

.empty-state p {
  margin: 0;
  font: var(--Text-BodyMedium);
}

/* Transactions */
.transactions-container {
  max-width: 800px;
}

.transactions-list {
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Medium);
}

.transaction-card {
  background: var(--Color-Background-Highlight);
  border-radius: 8px;
  padding: var(--Gap-Large);
  box-shadow: var(--ShadowLight);
}

.transaction-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--Gap-Medium);
}

.transaction-info {
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Small);
}

.transaction-date {
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  font-weight: 600;
}

.transaction-user,
.transaction-method {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
}

.transaction-total {
  text-align: right;
}

.total-amount {
  color: var(--Color-Accent-Green);
  font: var(--Text-HeadingSmall);
  font-weight: 700;
}

.transaction-games h4 {
  margin: 0 0 var(--Gap-Small) 0;
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  font-weight: 600;
}

.games-list {
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Small);
}

.game-item {
  padding: 8px 12px;
  background: var(--Color-Background-System);
  border-radius: 4px;
}

.game-name {
  color: var(--Color-Text-Main);
  font: var(--Text-BodySmall);
}

/* Reviews */
.reviews-container {
  max-width: 900px;
}

.reviews-list {
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Medium);
}

.review-card {
  background: var(--Color-Background-Highlight);
  border-radius: 8px;
  padding: var(--Gap-Large);
  box-shadow: var(--ShadowLight);
}

.review-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--Gap-Medium);
}

.review-info {
  flex: 1;
}

.review-game {
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  font-weight: 600;
  margin-bottom: var(--Gap-Small);
}

.review-meta {
  display: flex;
  gap: var(--Gap-Medium);
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
}

.review-rating {
  display: flex;
  align-items: baseline;
  gap: 2px;
}

.rating-score {
  color: var(--Color-Primary);
  font: var(--Text-HeadingSmall);
  font-weight: 700;
}

.rating-label {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
}

.review-content {
  margin-bottom: var(--Gap-Medium);
}

.review-text {
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  line-height: 1.6;
}

.edit-form {
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Medium);
}

.edit-textarea {
  width: 100%;
  min-height: 100px;
  padding: 12px;
  border: 1px solid var(--Color-Background-System);
  border-radius: 4px;
  background: var(--Color-Background-Main);
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  resize: vertical;
}

.edit-textarea:focus {
  outline: none;
  border-color: var(--Color-Primary);
}

.rating-input {
  display: flex;
  align-items: center;
  gap: var(--Gap-Medium);
}

.rating-input label {
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  font-weight: 600;
}

.rating-input-field {
  width: 80px;
  padding: 8px;
  border: 1px solid var(--Color-Background-System);
  border-radius: 4px;
  background: var(--Color-Background-Main);
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
}

.review-actions {
  display: flex;
  gap: var(--Gap-Small);
}

.edit-btn {
  background: var(--Color-Primary);
}

.save-btn {
  background: var(--Color-Accent-Green);
}

.cancel-btn {
  background: var(--Color-Background-System);
  color: var(--Color-Text-Main);
  border: 1px solid var(--Color-Background-Hover);
}

.delete-btn {
  background: var(--Color-Accent-Red);
}

.admin-view-only {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  font-style: italic;
}

/* Users Management */
.stats-row {
  display: flex;
  gap: var(--Gap-Medium);
  margin-top: var(--Gap-Medium);
  margin-bottom: var(--Gap-Large);
}

.stat-card {
  background: var(--Color-Background-Highlight);
  padding: var(--Gap-Medium);
  border-radius: 8px;
  box-shadow: var(--ShadowLight);
  text-align: center;
  flex: 1;
}

.stat-number {
  display: block;
  color: var(--Color-Primary);
  font: var(--Text-HeadingMedium);
  font-weight: 700;
  margin-bottom: var(--Gap-Small);
}

.stat-label {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.users-container {
  max-width: 1000px;
}

.users-list {
  display: flex;
  flex-direction: column;
  gap: var(--Gap-Medium);
}

.user-card {
  background: var(--Color-Background-Highlight);
  border-radius: 8px;
  padding: var(--Gap-Large);
  box-shadow: var(--ShadowLight);
  transition: transform 0.2s ease;
}

.user-card:hover {
  transform: translateY(-2px);
}

.user-header {
  display: flex;
  align-items: flex-start;
  gap: var(--Gap-Medium);
  margin-bottom: var(--Gap-Medium);
}

.user-avatar {
  width: 60px;
  height: 60px;
  background: var(--Color-Primary);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.user-avatar .avatar-text {
  color: white;
  font-size: 20px;
  font-weight: 700;
}

.user-info {
  flex: 1;
  min-width: 0;
}

.user-name {
  color: var(--Color-Text-Main);
  font: var(--Text-BodyLarge);
  font-weight: 600;
  margin-bottom: var(--Gap-Small);
}

.user-email {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
  margin-bottom: var(--Gap-Small);
}

.user-meta {
  display: flex;
  gap: var(--Gap-Medium);
  align-items: center;
}

.user-role {
  padding: 4px 8px;
  border-radius: 4px;
  font: var(--Text-BodySmall);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.admin-role {
  background: rgba(244, 67, 54, 0.1);
  color: #f44336;
  border: 1px solid #f44336;
}

.user-role.user-role {
  background: rgba(76, 175, 80, 0.1);
  color: #4caf50;
  border: 1px solid #4caf50;
}

.user-date {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
}

.user-actions {
  display: flex;
  gap: var(--Gap-Small);
  flex-shrink: 0;
}

.view-btn {
  background: var(--Color-Primary);
}

.user-stats {
  display: flex;
  gap: var(--Gap-Large);
  padding-top: var(--Gap-Medium);
  border-top: 1px solid var(--Color-Background-System);
}

.stat-item {
  text-align: center;
  flex: 1;
}

.stat-value {
  display: block;
  color: var(--Color-Primary);
  font: var(--Text-BodyLarge);
  font-weight: 700;
  margin-bottom: var(--Gap-XSmall);
}

.stat-name {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* User Details Modal */
.user-details-modal {
  background: var(--Color-Background-Highlight);
  border-radius: 8px;
  box-shadow: var(--ShadowHeavy);
  max-width: 400px;
  width: 85%;
  max-height: 60vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--Gap-Medium);
  border-bottom: 1px solid var(--Color-Background-System);
}

.modal-header h3 {
  margin: 0;
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingMedium);
  font-weight: 600;
}

.close-btn {
  background: none;
  border: none;
  color: var(--Color-Text-Dim);
  font-size: 20px;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.2s;
}

.close-btn:hover {
  background: var(--Color-Background-System);
  color: var(--Color-Text-Main);
}

.modal-content {
  padding: var(--Gap-Small);
}

.user-detail-grid {
  display: grid;
  gap: var(--Gap-XSmall);
}

.detail-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--Gap-XSmall) var(--Gap-Small);
  background: var(--Color-Background-System);
  border-radius: 4px;
  min-height: 32px;
}

.detail-item label {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  font-weight: 600;
  min-width: 60px;
}

.detail-item span {
  color: var(--Color-Text-Main);
  font: var(--Text-BodySmall);
  text-align: right;
  flex: 1;
}

.role-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font: var(--Text-BodySmall);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Responsive Design */
@media (max-width: 768px) {
  .account-detail-container {
    flex-direction: column;
  }
  
  .sidebar {
    width: 100%;
    border-radius: 0;
  }
  
  .sidebar-menu {
    display: flex;
    overflow-x: auto;
    padding: var(--Gap-Small) 0;
  }
  
  .sidebar-item {
    white-space: nowrap;
    border-left: none;
    border-bottom: 3px solid transparent;
  }
  
  .sidebar-item.active {
    border-left: none;
    border-bottom-color: var(--Color-Primary);
  }
  
  .main-content {
    max-width: 100%;
  }
  
  .profile-card {
    flex-direction: column;
    text-align: center;
  }
  
  .transaction-header,
  .review-header {
    flex-direction: column;
    gap: var(--Gap-Medium);
  }
  
  .form-overlay .form-card {
    margin: var(--Gap-Medium);
  }
  
  .stats-row {
    flex-direction: column;
  }
  
  .user-header {
    flex-direction: column;
    text-align: center;
  }
  
  .user-actions {
    justify-content: center;
    margin-top: var(--Gap-Medium);
  }
  
  .user-stats {
    justify-content: space-around;
  }
  
  .user-details-modal {
    margin: var(--Gap-Small);
    max-height: calc(100vh - 2 * var(--Gap-Small));
    max-width: none;
    width: calc(100% - 2 * var(--Gap-Small));
  }
  
  .modal-header {
    padding: var(--Gap-Small);
  }
  
  .modal-content {
    padding: var(--Gap-Small);
  }
  
  .detail-item {
    flex-direction: column;
    align-items: flex-start;
    text-align: left;
    min-height: auto;
    padding: var(--Gap-XSmall);
  }
  
  .detail-item label {
    margin-bottom: var(--Gap-XSmall);
    min-width: auto;
  }
  
  .detail-item span {
    text-align: left;
  }
}

/* Dashboard Styles */
.dashboard-container {
  max-width: 1200px;
}

.dashboard-section {
  margin-bottom: var(--Gap-XLarge);
}

.section-title {
  margin: 0 0 var(--Gap-Large) 0;
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingMedium);
  display: flex;
  align-items: center;
  gap: var(--Gap-Small);
}

/* Revenue Stats */
.revenue-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: var(--Gap-Large);
  margin-bottom: var(--Gap-Large);
}

.revenue-card {
  background: var(--Color-Background-Highlight);
  padding: var(--Gap-Large);
  border-radius: 12px;
  box-shadow: var(--ShadowLight);
  border-left: 4px solid var(--Color-Primary);
  transition: transform 0.2s ease;
}

.revenue-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--ShadowHeavy);
}

.revenue-header {
  display: flex;
  align-items: center;
  gap: var(--Gap-Small);
  margin-bottom: var(--Gap-Medium);
}

.revenue-icon {
  font-size: 24px;
}

.revenue-label {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.revenue-amount {
  color: var(--Color-Primary);
  font: var(--Text-HeadingLarge);
  font-weight: 700;
  margin-bottom: var(--Gap-Small);
}

.revenue-subtitle {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
}

.total-revenue {
  border-left-color: #10b981;
}

.monthly-revenue {
  border-left-color: #3b82f6;
}

.quarterly-revenue {
  border-left-color: #8b5cf6;
}

.avg-transaction {
  border-left-color: #f59e0b;
}

/* System Stats */
.system-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: var(--Gap-Medium);
}

.system-stats .stat-card {
  background: var(--Color-Background-Highlight);
  padding: var(--Gap-Large);
  border-radius: 8px;
  box-shadow: var(--ShadowLight);
  text-align: center;
  transition: transform 0.2s ease;
}

.system-stats .stat-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--ShadowHeavy);
}

/* Top Games */
.top-games-list {
  background: var(--Color-Background-Highlight);
  border-radius: 8px;
  overflow: hidden;
  box-shadow: var(--ShadowLight);
}

.top-game-item {
  display: flex;
  align-items: center;
  gap: var(--Gap-Medium);
  padding: var(--Gap-Medium) var(--Gap-Large);
  border-bottom: 1px solid var(--Color-Background-System);
  transition: background 0.2s ease;
}

.top-game-item:last-child {
  border-bottom: none;
}

.top-game-item:hover {
  background: var(--Color-Background-System);
}

.game-rank {
  width: 30px;
  height: 30px;
  background: var(--Color-Primary);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  flex-shrink: 0;
}

.game-info {
  flex: 1;
}

.game-title {
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  font-weight: 600;
  margin-bottom: var(--Gap-XSmall);
}

.game-stats {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
}

.game-revenue {
  color: var(--Color-Primary);
  font: var(--Text-BodyLarge);
  font-weight: 700;
}

/* Recent Activity */
.recent-activity {
  background: var(--Color-Background-Highlight);
  border-radius: 8px;
  overflow: hidden;
  box-shadow: var(--ShadowLight);
}

.activity-item {
  display: flex;
  align-items: center;
  gap: var(--Gap-Medium);
  padding: var(--Gap-Medium) var(--Gap-Large);
  border-bottom: 1px solid var(--Color-Background-System);
  transition: background 0.2s ease;
}

.activity-item:last-child {
  border-bottom: none;
}

.activity-item:hover {
  background: var(--Color-Background-System);
}

.activity-icon {
  width: 40px;
  height: 40px;
  background: var(--Color-Primary);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  flex-shrink: 0;
}

.activity-info {
  flex: 1;
}

.activity-text {
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  margin-bottom: var(--Gap-XSmall);
}

.activity-time {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
}

.activity-amount {
  color: var(--Color-Primary);
  font: var(--Text-BodyMedium);
  font-weight: 700;
}

/* Dashboard responsive */
@media (max-width: 768px) {
  .revenue-stats {
    grid-template-columns: 1fr;
    gap: var(--Gap-Medium);
  }
  
  .system-stats {
    grid-template-columns: repeat(2, 1fr);
    gap: var(--Gap-Small);
  }
  
  .top-game-item,
  .activity-item {
    padding: var(--Gap-Small) var(--Gap-Medium);
    gap: var(--Gap-Small);
  }
  
  .game-rank,
  .activity-icon {
    width: 35px;
    height: 35px;
    font-size: 16px;
  }
  
  .revenue-amount {
    font-size: 24px;
  }
}
</style> 