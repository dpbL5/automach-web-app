<script setup>
import SuperNav from "./components/SuperNav.vue";
import Footer from "./components/Footer.vue";
import Nav from "./components/Nav.vue";
</script>

<template>
  <div class="app">
    <SuperNav />
    <main class="main-content">
      <router-view></router-view>
    </main>
    <Footer />
  </div>
</template>

<style>
.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

</style>
