<template>
  <div class="admin-dashboard">
    <h1>Admin Dashboard</h1>
    
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
    
    <div class="revenue-section">
      <h2>Revenue</h2>
      <div class="chart-container">
        <!-- Pie chart for revenue distribution -->
        <div v-if="revenueData.length > 0" class="pie-chart">
          <canvas ref="pieChart"></canvas>
        </div>
      </div>
    </div>
    
    <div class="monthly-revenue">
      <h2>Monthly Revenue</h2>
      <div class="revenue-table">
        <div v-for="(item, index) in revenueData" :key="index" class="revenue-row">
          <div class="month-cell">{{ item.monthName }}</div>
          <div class="transactions-cell">{{ item.transactionCount }} transactions</div>
          <div class="amount-cell">$ {{ item.totalRevenue.toFixed(2) }}</div>
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
      pieChart: null
    };
  },
  async mounted() {
    await this.fetchRevenueData();
    this.renderPieChart();
  },
  methods: {
    async fetchRevenueData() {
      try {
        const token = localStorage.getItem('token');
        const response = await fetch('http://localhost:5174/api/transactions/revenue-data', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (!response.ok) {
          throw new Error('Failed to fetch revenue data');
        }
        
        this.revenueData = await response.json();
        console.log('Revenue data:', this.revenueData);
      } catch (error) {
        console.error('Error fetching revenue data:', error);
      }
    },
    renderPieChart() {
      if (!this.revenueData.length || !this.$refs.pieChart) return;
      
      // Group data by quarter for the pie chart
      const quarters = [
        { name: 'Q1', months: [1, 2, 3], color: '#FF6B6B' },
        { name: 'Q2', months: [4, 5, 6], color: '#66BB6A' },
        { name: 'Q3', months: [7, 8, 9], color: '#29B6F6' },
        { name: 'Q4', months: [10, 11, 12], color: '#FFA726' }
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
            borderWidth: 0
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: {
              position: 'right',
              labels: {
                color: '#ffffff'
              }
            }
          }
        }
      });
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
}

h1 {
  color: var(--Color-Text-Main);
  margin-bottom: 1.5rem;
  font: var(--Text-HeadingLarge);
}

h2 {
  color: var(--Color-Text-Main);
  margin-bottom: 1.5rem;
  font: var(--Text-HeadingMedium);
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

.revenue-section {
  background-color: var(--Color-Background-Main);
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 2rem;
  box-shadow: var(--ShadowLight);
}

.chart-container {
  display: flex;
  justify-content: center;
  height: 300px;
}

.pie-chart {
  width: 50%;
  height: 100%;
}

.monthly-revenue {
  background-color: var(--Color-Background-Main);
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: var(--ShadowLight);
}

.revenue-table {
  width: 100%;
}

.revenue-row {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  padding: 1rem;
  border-bottom: 1px solid var(--Color-Background-Highlight);
}

.revenue-row:nth-child(even) {
  background-color: var(--Color-Background-Highlight);
}

.month-cell, .transactions-cell, .amount-cell {
  display: flex;
  align-items: center;
}

.amount-cell {
  justify-content: flex-end;
  font-weight: bold;
}
</style> 