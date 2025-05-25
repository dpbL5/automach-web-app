<script setup>
import { ref } from "vue";
const msg = ref("Welcome to Automach!");
</script>

<template>
  <header class="header">
    <p class="nav__item">{{ msg }}</p>
  </header>
</template>

<style src="./assets/main.css"></style>
