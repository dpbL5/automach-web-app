import { userCache, accountsCache, gamesCache } from './userCache.js';

export const cacheManager = {
  // Cache keys và expiry times
  CACHE_EXPIRY: {
    USER: 30 * 60 * 1000,      // 30 minutes
    ACCOUNTS: 15 * 60 * 1000,  // 15 minutes  
    GAMES: 10 * 60 * 1000,     // 10 minutes
    REVIEWS: 5 * 60 * 1000,    // 5 minutes
    TRANSACTIONS: 5 * 60 * 1000 // 5 minutes
  },

  // User cache methods
  user: {
    get: () => userCache.getUser(),
    set: (user) => userCache.setUser(user),
    update: (updates) => userCache.updateUser(updates),
    clear: () => userCache.clearUser(),
    has: () => userCache.hasUser()
  },

  // Accounts cache methods (for admin)
  accounts: {
    get: () => accountsCache.getAccounts(),
    set: (accounts) => accountsCache.setAccounts(accounts),
    clear: () => accountsCache.clearAccounts(),
    has: () => accountsCache.hasAccounts()
  },

  // Games cache methods
  games: {
    get: () => gamesCache.getGames(),
    set: (games) => gamesCache.setGames(games),
    clear: () => gamesCache.clearGames(),
    has: () => gamesCache.hasGames()
  },

  // Clear all caches
  clearAll() {
    this.user.clear();
    this.accounts.clear();
    this.games.clear();
    console.log('All caches cleared');
  },

  // Check cache health (for debugging)
  getStatus() {
    return {
      user: {
        cached: this.user.has(),
        data: this.user.get()
      },
      accounts: {
        cached: this.accounts.has(),
        count: this.accounts.get()?.length || 0
      },
      games: {
        cached: this.games.has(),
        count: this.games.get()?.length || 0
      }
    };
  },

  // Preload essential data
  async preloadUserData(userId) {
    try {
      // Only fetch if not cached
      if (!this.user.has()) {
        const response = await fetch(`http://localhost:5174/api/accounts/${userId}`, {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
        });
        if (response.ok) {
          const user = await response.json();
          this.user.set(user);
          console.log('User data preloaded and cached');
        }
      }
    } catch (error) {
      console.error('Error preloading user data:', error);
    }
  },

  // Smart fetch với cache fallback
  async fetchWithCache(cacheType, fetchFn, forceRefresh = false) {
    const cache = this[cacheType];
    
    if (!forceRefresh && cache.has()) {
      console.log(`${cacheType} loaded from cache`);
      return cache.get();
    }

    try {
      const data = await fetchFn();
      cache.set(data);
      console.log(`${cacheType} fetched and cached`);
      return data;
    } catch (error) {
      console.error(`Error fetching ${cacheType}:`, error);
      // Return cached data if fetch fails
      if (cache.has()) {
        console.log(`Returning cached ${cacheType} due to fetch error`);
        return cache.get();
      }
      throw error;
    }
  }
}; 