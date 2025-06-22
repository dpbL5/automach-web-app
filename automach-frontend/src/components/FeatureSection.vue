<template>
  <div class="feature">
    <div class="container">
      <button class="arrow left" @click="prevSlide">&#8592;</button>
      <img :src="slides[current].image" :alt="slides[current].title" class="banner" />
      <button class="arrow right" @click="nextSlide">&#8594;</button>
      <div class="gameinfo">
        <h2>{{ slides[current].title }}</h2>
        <p>{{ slides[current].desc }}</p>
      </div>
    </div>
    <div class="dots">
      <span v-for="(slide, idx) in slides" :key="idx" class="dot" :class="{ active: idx === current }" @click="goToSlide(idx)"></span>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      slides: [
        {
          image: '/frontend/src/assets/images/sc1.jpg',
          title: 'StarCraft I',
          desc: 'Classic real-time strategy game.'
        },
        {
          image: '/frontend/src/assets/images/sc2.jpg',
          title: 'StarCraft II',
          desc: 'Epic sequel with new campaigns.'
        },
        {
          image: '/frontend/src/assets/images/pubg-header.jpg',
          title: 'PUBG',
          desc: 'Battle royale phenomenon.'
        },
        {
          image: '/frontend/src/assets/images/cs2.jpg',
          title: 'Counter-Strike 2',
          desc: 'Tactical shooter action.'
        }
      ],
      current: 0,
      timer: null
    };
  },
  mounted() {
    this.startSlideshow();
  },
  beforeUnmount() {
    clearInterval(this.timer);
  },
  methods: {
    startSlideshow() {
      this.timer = setInterval(this.nextSlide, 4000);
    },
    nextSlide() {
      this.current = (this.current + 1) % this.slides.length;
    },
    prevSlide() {
      this.current = (this.current - 1 + this.slides.length) % this.slides.length;
    },
    goToSlide(idx) {
      this.current = idx;
    }
  }
};
</script>

<style scoped>
.feature {
  height: 320px;
  background: var(--Color-Background-Main50);
  box-shadow: var(--ShadowLight);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  position: relative;
}
.container {
  display: flex;
  align-items: center;
  position: relative;
  width: 800px;
  height: 260px;
}
.banner {
  width: 700px;
  height: 240px;
  object-fit: cover;
  border-radius: 12px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.15);
}
.arrow {
  background: rgba(0,0,0,0.3);
  border: none;
  color: #fff;
  font-size: 2rem;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  cursor: pointer;
  z-index: 2;
  transition: background 0.2s;
}
.arrow.left {
  margin-right: 16px;
}
.arrow.right {
  margin-left: 16px;
}
.arrow:hover {
  background: rgba(0,0,0,0.6);
}
.gameinfo {
  position: absolute;
  left: 40px;
  bottom: 24px;
  color: #fff;
  text-shadow: 0 2px 8px rgba(0,0,0,0.7);
  background: rgba(0,0,0,0.3);
  padding: 12px 24px;
  border-radius: 8px;
  min-width: 200px;
}
.dots {
  display: flex;
  justify-content: center;
  margin-top: 12px;
}
.dot {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background: #bbb;
  margin: 0 6px;
  cursor: pointer;
  transition: background 0.2s;
}
.dot.active {
  background: var(--Color-Secondary, #0078ff);
}
</style>
