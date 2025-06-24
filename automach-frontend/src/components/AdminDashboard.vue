<template>
  <div class="admin-dashboard">
    <div class="dashboard-header">
      <h1>📊 Admin Dashboard</h1>
      <p>AutoMach system overview and revenue analysis</p>
      <div class="last-updated">
        <span class="update-icon">🔄</span>
        Last updated: {{ lastUpdated }}
      </div>
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
      <button @click="refreshData" class="refresh-btn" :disabled="isLoading">
        <span v-if="isLoading" class="loading-spinner">⏳</span>
        <span v-else>🔄</span>
        Refresh Data
      </button>
    </div>
    
    <!-- Revenue Overview Cards -->
    <div class="revenue-overview">
      <div class="overview-card total-revenue">
        <div class="card-header">
          <span class="card-icon">💰</span>
          <span class="card-title">Total Revenue</span>
          <div class="card-trend positive">
            <span class="trend-icon">📈</span>
            <span class="trend-value">+{{ revenueGrowth.toFixed(1) }}%</span>
          </div>
        </div>
        <div class="card-amount">${{ totalRevenue.toFixed(2) }}</div>
        <div class="card-subtitle">All time</div>
      </div>
      
      <div class="overview-card monthly-revenue">
        <div class="card-header">
          <span class="card-icon">📅</span>
          <span class="card-title">This Month Revenue</span>
          <div class="card-trend" :class="monthlyGrowth >= 0 ? 'positive' : 'negative'">
            <span class="trend-icon">{{ monthlyGrowth >= 0 ? '📈' : '📉' }}</span>
            <span class="trend-value">{{ monthlyGrowth >= 0 ? '+' : '' }}{{ monthlyGrowth.toFixed(1) }}%</span>
          </div>
        </div>
        <div class="card-amount">${{ monthlyRevenue.toFixed(2) }}</div>
        <div class="card-subtitle">{{ getCurrentMonthName() }} 2024</div>
      </div>
      
      <div class="overview-card quarterly-revenue">
        <div class="card-header">
          <span class="card-icon">📊</span>
          <span class="card-title">This Quarter Revenue</span>
          <div class="card-trend" :class="quarterlyGrowth >= 0 ? 'positive' : 'negative'">
            <span class="trend-icon">{{ quarterlyGrowth >= 0 ? '📈' : '📉' }}</span>
            <span class="trend-value">{{ quarterlyGrowth >= 0 ? '+' : '' }}{{ quarterlyGrowth.toFixed(1) }}%</span>
          </div>
        </div>
        <div class="card-amount">${{ quarterlyRevenue.toFixed(2) }}</div>
        <div class="card-subtitle">Q{{ getCurrentQuarter() }} 2024</div>
      </div>
      
      <div class="overview-card avg-transaction">
        <div class="card-header">
          <span class="card-icon">💳</span>
          <span class="card-title">Average/Transaction</span>
          <div class="card-trend" :class="avgTransactionGrowth >= 0 ? 'positive' : 'negative'">
            <span class="trend-icon">{{ avgTransactionGrowth >= 0 ? '📈' : '📉' }}</span>
            <span class="trend-value">{{ avgTransactionGrowth >= 0 ? '+' : '' }}{{ avgTransactionGrowth.toFixed(1) }}%</span>
          </div>
        </div>
        <div class="card-amount">${{ avgTransactionValue.toFixed(2) }}</div>
        <div class="card-subtitle">{{ totalTransactions }} transactions</div>
      </div>
    </div>
    
    <!-- System Statistics -->
    <div class="system-stats">
      <h2>📋 System Statistics</h2>
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-number">{{ totalUsers }}</div>
          <div class="stat-label">Total Users</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">{{ totalGames }}</div>
          <div class="stat-label">Total Games</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">{{ totalReviews }}</div>
          <div class="stat-label">Total Reviews</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">{{ totalTransactions }}</div>
          <div class="stat-label">Total Transactions</div>
        </div>
      </div>
    </div>
    
    <div class="dashboard-content">
      <!-- Revenue Chart Section -->
      <div class="revenue-section">
        <h2>📈 Quarterly Revenue Chart</h2>
        <div class="chart-container">
          <div v-if="revenueData.length > 0" class="pie-chart">
            <canvas ref="pieChart"></canvas>
          </div>
          <div v-else class="no-data">
            <span class="no-data-icon">📊</span>
            <p>No revenue data available</p>
          </div>
        </div>
      </div>
      
      <!-- Top Games Section -->
      <div class="top-games-section">
        <h2>🎮 Top Games by Revenue</h2>
        <div class="top-games-list">
          <div v-if="topGamesByRevenue.length > 0">
            <div v-for="(game, index) in topGamesByRevenue" :key="game.gameId" class="top-game-item">
              <div class="game-rank">{{ index + 1 }}</div>
              <div class="game-info">
                <div class="game-title">{{ getGameTitle(game.gameId) }}</div>
                <div class="game-stats">{{ game.sales }} sales • ${{ game.revenue.toFixed(2) }} revenue</div>
              </div>
              <div class="game-revenue">${{ game.revenue.toFixed(2) }}</div>
            </div>
          </div>
          <div v-else class="no-data">
            <span class="no-data-icon">🎮</span>
            <p>No game data available</p>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Monthly Revenue Table -->
    <div class="monthly-revenue">
      <h2>📅 Monthly Revenue Details</h2>
      <div class="revenue-table">
        <div class="table-header">
          <div class="header-cell">Month</div>
          <div class="header-cell">Transactions</div>
          <div class="header-cell">Revenue</div>
        </div>
        <div v-if="revenueData.length > 0">
          <div v-for="(item, index) in revenueData" :key="index" class="revenue-row">
            <div class="month-cell">{{ item.monthName }}</div>
            <div class="transactions-cell">{{ item.transactionCount }} transactions</div>
            <div class="amount-cell">${{ item.totalRevenue.toFixed(2) }}</div>
          </div>
        </div>
        <div v-else class="no-data-row">
          <div class="no-data">
            <span class="no-data-icon">📊</span>
            <p>No monthly revenue data available</p>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Recent Activity -->
    <div class="recent-activity">
      <h2>🕒 Recent Activity</h2>
      <div class="activity-filters">
        <button @click="setActivityFilter('all')" :class="['filter-btn', { active: activityFilter === 'all' }]">
          All
        </button>
        <button @click="setActivityFilter('today')" :class="['filter-btn', { active: activityFilter === 'today' }]">
          Today
        </button>
        <button @click="setActivityFilter('week')" :class="['filter-btn', { active: activityFilter === 'week' }]">
          This Week
        </button>
      </div>
      <div class="activity-list">
        <div v-if="filteredRecentTransactions.length > 0">
          <div v-for="tx in filteredRecentTransactions" :key="tx.id" class="activity-item">
            <div class="activity-icon">💰</div>
            <div class="activity-info">
              <div class="activity-text">
                <strong>{{ getUserName(tx.accountId) }}</strong> purchased 
                {{ tx.gameIds ? tx.gameIds.length : 0 }} game(s)
              </div>
              <div class="activity-time">{{ formatDate(tx.createdAt) }}</div>
            </div>
            <div class="activity-amount">${{ tx.totalPrice ? tx.totalPrice.toFixed(2) : '0.00' }}</div>
          </div>
        </div>
        <div v-else class="no-data">
          <span class="no-data-icon">🕒</span>
          <p>No activity {{ activityFilter === 'all' ? 'recently' : activityFilter === 'today' ? 'today' : 'this week' }}</p>
        </div>
      </div>
    </div>
    
    <!-- Quick Stats -->
    <div class="quick-stats">
      <h2>⚡ Quick Stats</h2>
      <div class="quick-stats-grid">
        <div class="quick-stat-card">
          <div class="stat-icon">👥</div>
          <div class="stat-content">
            <div class="stat-title">Active users</div>
            <div class="stat-number">{{ activeUsersCount }}</div>
            <div class="stat-subtitle">last 7 days</div>
          </div>
        </div>
        <div class="quick-stat-card">
          <div class="stat-icon">🎮</div>
          <div class="stat-content">
            <div class="stat-title">Best selling game</div>
            <div class="stat-number">{{ bestSellingGame.title || 'N/A' }}</div>
            <div class="stat-subtitle">{{ bestSellingGame.sales || 0 }} sales</div>
          </div>
        </div>
        <div class="quick-stat-card">
          <div class="stat-icon">⭐</div>
          <div class="stat-content">
            <div class="stat-title">Average rating</div>
            <div class="stat-number">{{ averageRating.toFixed(1) }}/100</div>
            <div class="stat-subtitle">{{ totalReviews }} reviews</div>
          </div>
        </div>
        <div class="quick-stat-card">
          <div class="stat-icon">💎</div>
          <div class="stat-content">
            <div class="stat-title">Highest daily revenue</div>
            <div class="stat-number">${{ highestDailyRevenue.toFixed(0) }}</div>
            <div class="stat-subtitle">in one day</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Chart from 'chart.js/auto';

export default {
  data() {
    return {
      revenueData: [],
      pieChart: null,
      allTransactions: [],
      allUsers: [],
      allGames: [],
      allReviews: [],
      accounts: [],
      isLoading: false,
      lastUpdated: '',
      previousMonthRevenue: 0,
      previousQuarterRevenue: 0,
      previousAvgTransaction: 0,
      activityFilter: 'all'
    };
  },
  computed: {
    totalRevenue() {
      if (!this.allTransactions.length) return 0;
      return this.allTransactions.reduce((total, tx) => total + (tx.totalPrice || 0), 0);
    },
    monthlyRevenue() {
      if (!this.allTransactions.length) return 0;
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
      if (!this.allTransactions.length) return 0;
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
    totalTransactions() {
      return this.allTransactions.length;
    },
    avgTransactionValue() {
      if (!this.allTransactions.length) return 0;
      return this.totalRevenue / this.allTransactions.length;
    },
    totalUsers() {
      return this.allUsers.length;
    },
    totalGames() {
      return this.allGames.length;
    },
    totalReviews() {
      return this.allReviews.length;
    },
    topGamesByRevenue() {
      if (!this.allTransactions.length) return [];
      
      const gameStats = {};
      this.allTransactions.forEach(tx => {
        if (tx.gameIds && tx.gameIds.length > 0) {
          tx.gameIds.forEach(gameId => {
            if (!gameStats[gameId]) {
              gameStats[gameId] = { gameId, sales: 0, revenue: 0 };
            }
            gameStats[gameId].sales++;
            gameStats[gameId].revenue += (tx.totalPrice || 0) / tx.gameIds.length;
          });
        }
      });
      
      return Object.values(gameStats)
        .sort((a, b) => b.revenue - a.revenue)
        .slice(0, 5);
    },
    recentTransactions() {
      if (!this.allTransactions.length) return [];
      return [...this.allTransactions]
        .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
        .slice(0, 5);
    },
    revenueGrowth() {
      // Tính tăng trưởng doanh thu so với năm trước (giả định)
      return Math.random() * 20 + 5; // Giả lập 5-25% tăng trưởng
    },
    monthlyGrowth() {
      if (this.previousMonthRevenue === 0) return 0;
      return ((this.monthlyRevenue - this.previousMonthRevenue) / this.previousMonthRevenue) * 100;
    },
    quarterlyGrowth() {
      if (this.previousQuarterRevenue === 0) return 0;
      return ((this.quarterlyRevenue - this.previousQuarterRevenue) / this.previousQuarterRevenue) * 100;
    },
    avgTransactionGrowth() {
      if (this.previousAvgTransaction === 0) return 0;
      return ((this.avgTransactionValue - this.previousAvgTransaction) / this.previousAvgTransaction) * 100;
    },
    filteredRecentTransactions() {
      if (!this.allTransactions.length) return [];
      
      const now = new Date();
      let startDate = new Date();
      
      switch (this.activityFilter) {
        case 'today':
          startDate.setHours(0, 0, 0, 0);
          break;
        case 'week':
          startDate.setDate(now.getDate() - 7);
          break;
        default:
          startDate = new Date(0); // All time
      }
      
      return [...this.allTransactions]
        .filter(tx => new Date(tx.createdAt) >= startDate)
        .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
        .slice(0, 10);
    },
    activeUsersCount() {
      const oneWeekAgo = new Date();
      oneWeekAgo.setDate(oneWeekAgo.getDate() - 7);
      
      const activeUserIds = this.allTransactions
        .filter(tx => new Date(tx.createdAt) >= oneWeekAgo)
        .map(tx => tx.accountId);
      
      return new Set(activeUserIds).size;
    },
    bestSellingGame() {
      if (!this.topGamesByRevenue.length) return {};
      return {
        title: this.getGameTitle(this.topGamesByRevenue[0].gameId),
        sales: this.topGamesByRevenue[0].sales
      };
    },
    averageRating() {
      if (!this.allReviews.length) return 0;
      const sum = this.allReviews.reduce((total, review) => total + (review.rating || 0), 0);
      return sum / this.allReviews.length;
    },
    highestDailyRevenue() {
      if (!this.allTransactions.length) return 0;
      
      const dailyRevenue = {};
      this.allTransactions.forEach(tx => {
        const date = new Date(tx.createdAt).toDateString();
        dailyRevenue[date] = (dailyRevenue[date] || 0) + (tx.totalPrice || 0);
      });
      
      return Math.max(...Object.values(dailyRevenue));
    }
  },
  async mounted() {
    this.isLoading = true;
    await this.fetchAllData();
    await this.fetchRevenueData();
    this.calculatePreviousData();
    this.renderPieChart();
    this.updateLastUpdated();
    this.isLoading = false;
    
    // Debug logs
    console.log('Dashboard Data Loaded:');
    console.log('Total Games:', this.totalGames);
    console.log('Total Reviews:', this.totalReviews);
    console.log('Total Users:', this.totalUsers);
    console.log('Total Transactions:', this.totalTransactions);
  },
  methods: {
    async fetchAllData() {
      try {
        const token = localStorage.getItem('token');
        
        // Fetch all transactions
        try {
          const txRes = await fetch('http://localhost:5174/api/transactions', {
            headers: { 'Authorization': `Bearer ${token}` }
          });
          if (txRes.ok) {
            const txData = await txRes.json();
            // Handle array response format for transactions
            this.allTransactions = Array.isArray(txData) ? txData : (txData.items || []);
          }
        } catch (error) {
          console.error('Error fetching transactions:', error);
        }
        
        // Fetch all users
        try {
          const usersRes = await fetch('http://localhost:5174/api/accounts', {
            headers: { 'Authorization': `Bearer ${token}` }
          });
          if (usersRes.ok) {
            const usersData = await usersRes.json();
            // Handle array response format for accounts
            this.allUsers = Array.isArray(usersData) ? usersData : (usersData.items || []);
            this.accounts = this.allUsers;
          }
        } catch (error) {
          console.error('Error fetching users:', error);
        }
        
        // Fetch all games
        try {
          const gamesRes = await fetch('http://localhost:5174/api/games?pageSize=1000');
          if (gamesRes.ok) {
            const gamesData = await gamesRes.json();
            // Handle pagination response format
            this.allGames = gamesData.items || gamesData;
          }
        } catch (error) {
          console.error('Error fetching games:', error);
        }
        
        // Fetch all reviews
        try {
          const reviewsRes = await fetch('http://localhost:5174/api/review');
          if (reviewsRes.ok) {
            const reviewsData = await reviewsRes.json();
            // Handle array response format for reviews
            this.allReviews = Array.isArray(reviewsData) ? reviewsData : (reviewsData.items || []);
          }
        } catch (error) {
          console.error('Error fetching reviews:', error);
        }
        
      } catch (error) {
        console.error('Error fetching data:', error);
      }
    },
    async fetchRevenueData() {
      try {
        const token = localStorage.getItem('token');
        const response = await fetch('http://localhost:5174/api/transactions/revenue-data', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (!response.ok) {
          console.log('Revenue data endpoint not available, using transactions data');
          // Fallback to calculating from transactions
          this.calculateRevenueData();
          return;
        }
        
        this.revenueData = await response.json();
        console.log('Revenue data:', this.revenueData);
      } catch (error) {
        console.error('Error fetching revenue data:', error);
        this.calculateRevenueData();
      }
    },
    calculateRevenueData() {
      if (!this.allTransactions.length) return;
      
      const months = [
        'January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December'
      ];
      
      const monthlyStats = {};
      
      this.allTransactions.forEach(tx => {
        const date = new Date(tx.createdAt);
        const month = date.getMonth() + 1;
        const monthName = months[date.getMonth()];
        
        if (!monthlyStats[month]) {
          monthlyStats[month] = {
            month,
            monthName,
            transactionCount: 0,
            totalRevenue: 0
          };
        }
        
        monthlyStats[month].transactionCount++;
        monthlyStats[month].totalRevenue += tx.totalPrice || 0;
      });
      
      this.revenueData = Object.values(monthlyStats).sort((a, b) => a.month - b.month);
    },
    renderPieChart() {
      if (!this.revenueData.length || !this.$refs.pieChart) return;
      
      // Group data by quarter for the pie chart
      const quarters = [
        { name: 'Q1 2024', months: [1, 2, 3], color: '#10b981' },
        { name: 'Q2 2024', months: [4, 5, 6], color: '#3b82f6' },
        { name: 'Q3 2024', months: [7, 8, 9], color: '#8b5cf6' },
        { name: 'Q4 2024', months: [10, 11, 12], color: '#f59e0b' }
      ];
      
      const quarterData = quarters.map(quarter => {
        const amount = this.revenueData
          .filter(item => quarter.months.includes(item.month))
          .reduce((sum, item) => sum + item.totalRevenue, 0);
        return {
          name: quarter.name,
          amount,
          color: quarter.color
        };
      });
      
      // Create the pie chart
      if (this.pieChart) {
        this.pieChart.destroy();
      }
      
      this.pieChart = new Chart(this.$refs.pieChart, {
        type: 'pie',
        data: {
          labels: quarterData.map(q => q.name),
          datasets: [{
            data: quarterData.map(q => q.amount),
            backgroundColor: quarterData.map(q => q.color),
            borderWidth: 2,
            borderColor: '#ffffff'
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: {
              position: 'right',
              labels: {
                color: '#ffffff',
                font: {
                  size: 14
                },
                padding: 20
              }
            },
            tooltip: {
              callbacks: {
                label: function(context) {
                  return context.label + ': $' + context.parsed.toFixed(2);
                }
              }
            }
          }
        }
      });
    },
    getCurrentQuarter() {
      return Math.floor(new Date().getMonth() / 3) + 1;
    },
    getCurrentMonthName() {
      const months = ['January', 'February', 'March', 'April', 'May', 'June',
                      'July', 'August', 'September', 'October', 'November', 'December'];
      return months[new Date().getMonth()];
    },
    getGameTitle(gameId) {
      const game = this.allGames.find(g => g.id === gameId || g.Id === gameId);
      return game ? game.title : `Game #${gameId}`;
    },
    getUserName(accountId) {
      const user = this.accounts.find(u => (u.id || u.Id) === accountId);
      return user ? (user.name || user.username || 'Unknown User') : `User #${accountId}`;
    },
    formatDate(dateString) {
      if (!dateString) return 'Unknown';
      return new Date(dateString).toLocaleDateString('vi-VN', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    },
    async refreshData() {
      this.isLoading = true;
      await this.fetchAllData();
      await this.fetchRevenueData();
      this.calculatePreviousData();
      this.renderPieChart();
      this.updateLastUpdated();
      this.isLoading = false;
    },
    updateLastUpdated() {
      this.lastUpdated = new Date().toLocaleString('vi-VN', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
      });
    },
    calculatePreviousData() {
      // Tính doanh thu tháng trước
      const currentMonth = new Date().getMonth();
      const currentYear = new Date().getFullYear();
      const previousMonth = currentMonth === 0 ? 11 : currentMonth - 1;
      const previousMonthYear = currentMonth === 0 ? currentYear - 1 : currentYear;
      
      this.previousMonthRevenue = this.allTransactions.reduce((total, tx) => {
        const txDate = new Date(tx.createdAt);
        if (txDate.getMonth() === previousMonth && txDate.getFullYear() === previousMonthYear) {
          return total + (tx.totalPrice || 0);
        }
        return total;
      }, 0);
      
      // Tính doanh thu quý trước
      const currentQuarter = this.getCurrentQuarter();
      const previousQuarter = currentQuarter === 1 ? 4 : currentQuarter - 1;
      const previousQuarterYear = currentQuarter === 1 ? currentYear - 1 : currentYear;
      
      this.previousQuarterRevenue = this.allTransactions.reduce((total, tx) => {
        const txDate = new Date(tx.createdAt);
        const txQuarter = Math.floor(txDate.getMonth() / 3) + 1;
        if (txQuarter === previousQuarter && txDate.getFullYear() === previousQuarterYear) {
          return total + (tx.totalPrice || 0);
        }
        return total;
      }, 0);
      
      // Tính trung bình giao dịch trước
      const previousTransactions = this.allTransactions.filter(tx => {
        const txDate = new Date(tx.createdAt);
        return txDate.getMonth() === previousMonth && txDate.getFullYear() === previousMonthYear;
      });
      
             this.previousAvgTransaction = previousTransactions.length > 0 
         ? this.previousMonthRevenue / previousTransactions.length 
         : 0;
     },
     setActivityFilter(filter) {
       this.activityFilter = filter;
     },
     updateLastUpdated() {
       this.lastUpdated = new Date().toLocaleString('vi-VN');
     },
     async refreshData() {
       this.isLoading = true;
       await this.fetchAllData();
       await this.fetchRevenueData();
       this.calculatePreviousData();
       this.renderPieChart();
       this.updateLastUpdated();
       this.isLoading = false;
       
       // Debug logs after refresh
       console.log('Dashboard Data Refreshed:');
       console.log('Total Games:', this.totalGames);
       console.log('Total Reviews:', this.totalReviews);
       console.log('Total Users:', this.totalUsers);
       console.log('Total Transactions:', this.totalTransactions);
     }
  }
};
</script>

<style scoped>
.admin-dashboard {
  padding: 2rem;
  color: var(--Color-Text-Main);
  background: var(--Gradient-Background-System);
  min-height: 100vh;
  max-width: 1400px;
  margin: 0 auto;
}

/* Header */
.dashboard-header {
  margin-bottom: 2rem;
}

.dashboard-header h1 {
  color: var(--Color-Text-Main);
  margin-bottom: 0.5rem;
  font: var(--Text-HeadingLarge);
}

.dashboard-header p {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
  margin: 0;
}

.last-updated {
  margin-top: 0.5rem;
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.update-icon {
  font-size: 0.9rem;
}

h2 {
  color: var(--Color-Text-Main);
  margin-bottom: 1.5rem;
  font: var(--Text-HeadingMedium);
  display: flex;
  align-items: center;
  gap: 0.5rem;
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

.refresh-btn {
  padding: 0.75rem 1.5rem;
  background-color: var(--Color-Primary);
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-left: auto;
}

.refresh-btn:hover:not(:disabled) {
  background-color: var(--Color-Primary-Dark);
  transform: translateY(-2px);
}

.refresh-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.loading-spinner {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Revenue Overview Cards */
.revenue-overview {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.overview-card {
  background: var(--Color-Background-Highlight);
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: var(--ShadowLight);
  border-left: 4px solid var(--Color-Primary);
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.overview-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--ShadowHeavy);
}

.overview-card.total-revenue {
  border-left-color: #10b981;
}

.overview-card.monthly-revenue {
  border-left-color: #3b82f6;
}

.overview-card.quarterly-revenue {
  border-left-color: #8b5cf6;
}

.overview-card.avg-transaction {
  border-left-color: #f59e0b;
}

.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.card-header .card-icon {
  font-size: 1.5rem;
}

.card-title {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  flex: 1;
}

.card-trend {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 600;
}

.card-trend.positive {
  background-color: rgba(16, 185, 129, 0.1);
  color: #10b981;
}

.card-trend.negative {
  background-color: rgba(239, 68, 68, 0.1);
  color: #ef4444;
}

.trend-icon {
  font-size: 0.8rem;
}

.trend-value {
  font-size: 0.75rem;
  font-weight: 700;
}

.card-amount {
  color: var(--Color-Primary);
  font: var(--Text-HeadingLarge);
  font-weight: 700;
  margin-bottom: 0.5rem;
  font-size: 2rem;
}

.card-subtitle {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
}

/* System Statistics */
.system-stats {
  margin-bottom: 2rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
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
  font-size: 2.5rem;
}

.stat-label {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodyMedium);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Dashboard Content Layout */
.dashboard-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  margin-bottom: 2rem;
}

/* Revenue Section */
.revenue-section {
  background-color: var(--Color-Background-Highlight);
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: var(--ShadowLight);
}

.chart-container {
  display: flex;
  justify-content: center;
  height: 300px;
  margin-top: 1rem;
}

.pie-chart {
  width: 100%;
  height: 100%;
  max-width: 400px;
}

/* Top Games Section */
.top-games-section {
  background-color: var(--Color-Background-Highlight);
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: var(--ShadowLight);
}

.top-games-list {
  margin-top: 1rem;
}

.top-game-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 0.5rem;
  transition: background 0.2s ease;
}

.top-game-item:hover {
  background: var(--Color-Background-System);
}

.game-rank {
  width: 32px;
  height: 32px;
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
  margin-bottom: 0.25rem;
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

/* Monthly Revenue Table */
.monthly-revenue {
  background-color: var(--Color-Background-Highlight);
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: var(--ShadowLight);
  margin-bottom: 2rem;
}

.revenue-table {
  width: 100%;
  border-radius: 8px;
  overflow: hidden;
  background: var(--Color-Background-System);
}

.table-header {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  background: var(--Color-Primary);
  color: white;
  font-weight: 600;
}

.header-cell {
  padding: 1rem;
  text-align: center;
  font: var(--Text-BodyMedium);
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.revenue-row {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  padding: 1rem;
  border-bottom: 1px solid var(--Color-Background-Highlight);
  transition: background 0.2s ease;
}

.revenue-row:hover {
  background-color: var(--Color-Background-Highlight);
}

.revenue-row:last-child {
  border-bottom: none;
}

.month-cell, .transactions-cell, .amount-cell {
  display: flex;
  align-items: center;
  justify-content: center;
  font: var(--Text-BodyMedium);
}

.amount-cell {
  font-weight: 700;
  color: var(--Color-Primary);
}

/* Recent Activity */
.recent-activity {
  background-color: var(--Color-Background-Highlight);
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: var(--ShadowLight);
}

.activity-list {
  margin-top: 1rem;
}

.activity-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 0.5rem;
  transition: background 0.2s ease;
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
  font-size: 1.2rem;
  flex-shrink: 0;
}

.activity-info {
  flex: 1;
}

.activity-text {
  color: var(--Color-Text-Main);
  font: var(--Text-BodyMedium);
  margin-bottom: 0.25rem;
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

/* No Data States */
.no-data {
  text-align: center;
  padding: 2rem 1rem;
  color: var(--Color-Text-Dim);
}

.no-data-icon {
  font-size: 3rem;
  display: block;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.no-data p {
  margin: 0;
  font: var(--Text-BodyMedium);
}

.no-data-row {
  grid-column: 1 / -1;
}

/* Responsive Design */
@media (max-width: 1024px) {
  .dashboard-content {
    grid-template-columns: 1fr;
  }
  
  .revenue-overview {
    grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  }
}

@media (max-width: 768px) {
  .admin-dashboard {
    padding: 1rem;
  }
  
  .admin-menu {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .revenue-overview {
    grid-template-columns: 1fr;
  }
  
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .chart-container {
    height: 250px;
  }
  
  .card-amount {
    font-size: 1.5rem;
  }
  
  .stat-number {
    font-size: 2rem;
  }
  
  .table-header,
  .revenue-row {
    grid-template-columns: 1fr;
    text-align: center;
  }
  
  .header-cell,
  .month-cell,
  .transactions-cell,
  .amount-cell {
    padding: 0.5rem;
  }
}

@media (max-width: 480px) {
  .top-game-item,
  .activity-item {
    flex-direction: column;
    text-align: center;
    gap: 0.5rem;
  }
  
  .game-info,
  .activity-info {
    text-align: center;
  }
}

/* Activity Filters */
.activity-filters {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.filter-btn {
  padding: 0.5rem 1rem;
  border: 2px solid var(--Color-Background-System);
  background: var(--Color-Background-Highlight);
  color: var(--Color-Text-Main);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-weight: 500;
}

.filter-btn:hover {
  border-color: var(--Color-Primary);
  background: var(--Color-Background-System);
}

.filter-btn.active {
  background: var(--Color-Primary);
  border-color: var(--Color-Primary);
  color: white;
}

/* Quick Stats */
.quick-stats {
  background-color: var(--Color-Background-Highlight);
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: var(--ShadowLight);
  margin-bottom: 2rem;
}

.quick-stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
}

.quick-stat-card {
  background: var(--Color-Background-System);
  border-radius: 12px;
  padding: 1rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  transition: all 0.3s ease;
  border-left: 4px solid var(--Color-Primary);
}

.quick-stat-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--ShadowLight);
}

.stat-icon {
  font-size: 2rem;
  background: rgba(var(--Color-Primary-RGB), 0.1);
  padding: 0.75rem;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 60px;
  height: 60px;
}

.stat-content {
  flex: 1;
}

.stat-title {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 0.25rem;
}

.stat-number {
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingMedium);
  font-weight: 700;
  margin-bottom: 0.25rem;
}

.stat-subtitle {
  color: var(--Color-Text-Dim);
  font: var(--Text-BodySmall);
}

/* Responsive improvements */
@media (max-width: 1024px) {
  .dashboard-content {
    grid-template-columns: 1fr;
  }
  
  .revenue-overview {
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  }
  
  .quick-stats-grid {
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  }
}

@media (max-width: 768px) {
  .admin-dashboard {
    padding: 1rem;
  }
  
  .admin-menu {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .refresh-btn {
    margin-left: 0;
    align-self: flex-start;
  }
  
  .revenue-overview {
    grid-template-columns: 1fr;
  }
  
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .quick-stats-grid {
    grid-template-columns: 1fr;
  }
  
  .activity-filters {
    flex-wrap: wrap;
  }
}
</style> 