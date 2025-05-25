<script setup>
import Nav from '../components/Nav.vue'
import { ref } from 'vue'

// Feature games data
const featuredGames = [
  {
    title: 'Counter Strike 2',
    desc: 'Counter-Strike 2 is here. New and refined, a whole new experience for FPS fans. Join the fight!',
    img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/730/header.jpg',
    thumbs: [
      'https://cdn.cloudflare.steamstatic.com/steam/apps/730/ss_1.jpg',
      'https://cdn.cloudflare.steamstatic.com/steam/apps/730/ss_2.jpg'
    ],
    btn: 'Free to Play',
    btnClass: 'btn-free'
  },
  {
    title: 'PUBG: BATTLEGROUNDS',
    desc: 'The original Battle Royale. Drop in, loot, survive, and be the last one standing!',
    img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/578080/header.jpg',
    thumbs: [
      'https://cdn.cloudflare.steamstatic.com/steam/apps/578080/ss_1.jpg',
      'https://cdn.cloudflare.steamstatic.com/steam/apps/578080/ss_2.jpg'
    ],
    btn: 'Free to Play',
    btnClass: 'btn-free'
  },
  {
    title: 'Cities Skylines',
    desc: 'Build the city of your dreams! A modern take on the classic city simulation.',
    img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/255710/header.jpg',
    thumbs: [
      'https://cdn.cloudflare.steamstatic.com/steam/apps/255710/ss_1.jpg',
      'https://cdn.cloudflare.steamstatic.com/steam/apps/255710/ss_2.jpg'
    ],
    btn: 'Buy Now',
    btnClass: 'btn-buy'
  }
]
const currentFeature = ref(0)
function nextFeature() {
  currentFeature.value = (currentFeature.value + 1) % featuredGames.length
}
function prevFeature() {
  currentFeature.value = (currentFeature.value - 1 + featuredGames.length) % featuredGames.length
}

// Free to play games (nhiều game hơn)
const freeGames = [
  {
    title: 'PUBG: BATTLEGROUNDS', price: 'Free', btnText: 'Play', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/578080/header.jpg'
  },
  {
    title: 'Apex Legends', price: 'Free', btnText: 'Play', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1172470/header.jpg'
  },
  {
    title: 'Destiny 2', price: 'Free', btnText: 'Play', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1085660/header.jpg'
  },
  {
    title: 'Warframe', price: 'Free', btnText: 'Play', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/230410/header.jpg'
  },
  {
    title: 'Team Fortress 2', price: 'Free', btnText: 'Play', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/440/header.jpg'
  }
]

// New & Trending (nhiều game hơn)
const trendingGames = [
  {
    title: 'Cities Skylines', price: '49,995', btnText: 'Buy', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/255710/header.jpg'
  },
  {
    title: 'Hades II', price: '29,995', btnText: 'Buy', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1145350/header.jpg'
  },
  {
    title: 'Manor Lords', price: '39,995', btnText: 'Buy', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1363080/header.jpg'
  },
  {
    title: 'Sons Of The Forest', price: '29,995', btnText: 'Buy', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1326470/header.jpg'
  },
  {
    title: 'Palworld', price: '29,995', btnText: 'Buy', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1623730/header.jpg'
  }
]

// Top Sellers (nhiều game hơn)
const topSellers = [
  {
    title: 'Balatro', price: '8,995', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/2379780/header.jpg'
  },
  {
    title: 'Animal Well', price: '14,995', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/953490/header.jpg'
  },
  {
    title: 'Helldivers 2', price: '49,995', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/553850/header.jpg'
  },
  {
    title: 'Elden Ring', price: '59,995', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/header.jpg'
  },
  {
    title: 'Palworld', price: '29,995', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/1623730/header.jpg'
  },
  {
    title: 'Stardew Valley', price: '9,995', img: 'https://cdn.cloudflare.steamstatic.com/steam/apps/413150/header.jpg'
  }
]

// GameCard component
const GameCard = {
  props: ['title', 'price', 'btnText', 'img'],
  template: `
    <div class='game-card'>
      <img :src="img" alt="Game" />
      <div class='game-title'>{{ title }}</div>
      <div class='game-price'>{{ price }}</div>
      <button class='btn' :class="{ 'btn-free': price === 'Free', 'btn-buy': price !== 'Free' }">{{ btnText }}</button>
    </div>
  `
}

// TopSellerCard component
const TopSellerCard = {
  props: ['title', 'price', 'img'],
  template: `
    <div class='top-seller-card'>
      <img :src="img" alt="Game" />
      <div class='top-seller-info'>
        <div class='top-seller-title'>{{ title }}</div>
        <div class='top-seller-price'>{{ price }}</div>
      </div>
      <button class='btn btn-cart'>Add to cart</button>
    </div>
  `
}
</script>

<template>
  <Nav />
  <div class="main-content">
    <!-- Feature & Recommended -->
    <section class="section feature-section">
      <h2>Feature & Recommended</h2>
      <div class="feature-content">
        <button class="feature-arrow left" @click="prevFeature">&#8592;</button>
        <div class="feature-image">
          <img :src="featuredGames[currentFeature].img" alt="Feature Game" />
        </div>
        <div class="feature-info">
          <h3>{{ featuredGames[currentFeature].title }}</h3>
          <p>{{ featuredGames[currentFeature].desc }}</p>
          <div class="feature-thumbnails">
            <img v-for="(thumb, idx) in featuredGames[currentFeature].thumbs" :key="idx" :src="thumb" alt="thumb" />
          </div>
          <button class="btn" :class="featuredGames[currentFeature].btnClass">{{ featuredGames[currentFeature].btn }}</button>
        </div>
        <button class="feature-arrow right" @click="nextFeature">&#8594;</button>
      </div>
    </section>

    <!-- Free to play games -->
    <section class="section">
      <h2>Free to play games</h2>
      <div class="card-row">
        <component :is="GameCard" v-for="(game, i) in freeGames" :key="'free'+i" v-bind="game" />
      </div>
    </section>

    <!-- New & Trending -->
    <section class="section">
      <h2>New & Trending</h2>
      <div class="card-row">
        <component :is="GameCard" v-for="(game, i) in trendingGames" :key="'trend'+i" v-bind="game" />
      </div>
    </section>

    <!-- Top Sellers -->
    <section class="section">
      <h2>Top Sellers</h2>
      <div class="top-sellers">
        <component :is="TopSellerCard" v-for="(game, i) in topSellers" :key="'topseller'+i" v-bind="game" />
      </div>
    </section>
  </div>
</template>

<style scoped>
.main-content {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 32px 0;
  background: #1a2233;
}

.section {
  background: #232b3b;
  border-radius: 8px;
  margin-bottom: 32px;
  padding: 24px;
  box-shadow: 0 2px 8px #0002;
}

.section h2 {
  color: #fff;
  margin-bottom: 16px;
}

.feature-section .feature-content {
  display: flex;
  gap: 32px;
  align-items: center;
}

.feature-image img {
  width: 350px;
  border-radius: 8px;
}

.feature-info {
  flex: 1;
  color: #fff;
}

.feature-thumbnails img {
  width: 60px;
  margin-right: 8px;
  border-radius: 4px;
}

.feature-arrow {
  background: #232b3b;
  color: #fff;
  border: none;
  font-size: 2rem;
  cursor: pointer;
  padding: 0 16px;
  border-radius: 50%;
  transition: background 0.2s;
}
.feature-arrow:hover {
  background: #2ecc40;
  color: #fff;
}
.feature-arrow.left {
  margin-right: 8px;
}
.feature-arrow.right {
  margin-left: 8px;
}

.btn {
  padding: 8px 24px;
  border-radius: 4px;
  border: none;
  font-weight: bold;
  cursor: pointer;
  margin-top: 12px;
}

.btn-free {
  background: #2ecc40;
  color: #fff;
}

.btn-buy {
  background: #3498db;
  color: #fff;
}

.btn-cart {
  background: #34495e;
  color: #fff;
}

.card-row {
  display: flex;
  gap: 24px;
  flex-wrap: wrap;
}

.game-card {
  background: #1a2233;
  border-radius: 8px;
  padding: 16px;
  width: 220px;
  color: #fff;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.game-card img {
  width: 100%;
  border-radius: 6px;
  margin-bottom: 12px;
}

.game-title {
  font-weight: bold;
  margin-bottom: 8px;
}

.game-price {
  margin-bottom: 8px;
}

.top-sellers {
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
}

.top-seller-card {
  background: #1a2233;
  border-radius: 8px;
  padding: 12px 16px;
  display: flex;
  align-items: center;
  width: 320px;
  color: #fff;
}

.top-seller-card img {
  width: 60px;
  border-radius: 6px;
  margin-right: 16px;
}

.top-seller-info {
  flex: 1;
}

.top-seller-title {
  font-weight: bold;
}

.top-seller-price {
  color: #2ecc40;
}
</style> 