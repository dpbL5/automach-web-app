const USER_CACHE_KEY = 'automach_user_cache';
const CACHE_EXPIRY_MS = 30 * 60 * 1000; // 30 minutes

export const userCache = {
  // Lưu user vào cache
  setUser(user) {
    const cacheData = {
      user,
      timestamp: Date.now()
    };
    localStorage.setItem(USER_CACHE_KEY, JSON.stringify(cacheData));
  },

  // Lấy user từ cache
  getUser() {
    try {
      const cached = localStorage.getItem(USER_CACHE_KEY);
      if (!cached) return null;

      const cacheData = JSON.parse(cached);
      const now = Date.now();

      // Kiểm tra xem cache đã hết hạn chưa
      if (now - cacheData.timestamp > CACHE_EXPIRY_MS) {
        this.clearUser();
        return null;
      }

      return cacheData.user;
    } catch (error) {
      console.error('Error reading user cache:', error);
      this.clearUser();
      return null;
    }
  },

  // Xóa user khỏi cache
  clearUser() {
    localStorage.removeItem(USER_CACHE_KEY);
  },

  // Kiểm tra xem user có trong cache không
  hasUser() {
    return this.getUser() !== null;
  },

  // Cập nhật một phần thông tin user
  updateUser(updates) {
    const currentUser = this.getUser();
    if (currentUser) {
      const updatedUser = { ...currentUser, ...updates };
      this.setUser(updatedUser);
      return updatedUser;
    }
    return null;
  }
};

// Cache cho accounts list (dành cho admin)
const ACCOUNTS_CACHE_KEY = 'automach_accounts_cache';

export const accountsCache = {
  setAccounts(accounts) {
    const cacheData = {
      accounts,
      timestamp: Date.now()
    };
    localStorage.setItem(ACCOUNTS_CACHE_KEY, JSON.stringify(cacheData));
  },

  getAccounts() {
    try {
      const cached = localStorage.getItem(ACCOUNTS_CACHE_KEY);
      if (!cached) return null;

      const cacheData = JSON.parse(cached);
      const now = Date.now();

      // Cache accounts trong 15 phút
      if (now - cacheData.timestamp > 15 * 60 * 1000) {
        this.clearAccounts();
        return null;
      }

      return cacheData.accounts;
    } catch (error) {
      console.error('Error reading accounts cache:', error);
      this.clearAccounts();
      return null;
    }
  },

  clearAccounts() {
    localStorage.removeItem(ACCOUNTS_CACHE_KEY);
  },

  hasAccounts() {
    return this.getAccounts() !== null;
  }
};

// Cache cho games list
const GAMES_CACHE_KEY = 'automach_games_cache';

export const gamesCache = {
  setGames(games) {
    const cacheData = {
      games,
      timestamp: Date.now()
    };
    localStorage.setItem(GAMES_CACHE_KEY, JSON.stringify(cacheData));
  },

  getGames() {
    try {
      const cached = localStorage.getItem(GAMES_CACHE_KEY);
      if (!cached) return null;

      const cacheData = JSON.parse(cached);
      const now = Date.now();

      // Cache games trong 10 phút
      if (now - cacheData.timestamp > 10 * 60 * 1000) {
        this.clearGames();
        return null;
      }

      return cacheData.games;
    } catch (error) {
      console.error('Error reading games cache:', error);
      this.clearGames();
      return null;
    }
  },

  clearGames() {
    localStorage.removeItem(GAMES_CACHE_KEY);
  },

  hasGames() {
    return this.getGames() !== null;
  }
}; 