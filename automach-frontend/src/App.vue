<template>
  <SupNav />
  <router-view />
</template>

<script>
import SupNav from "./components/SupNav.vue";

export default {
  components: {
    SupNav,
  },
};
</script>

<style>
body {
  margin: 0;
  padding: 0;
  background-color: #0e141b;
  color: #f3f3f3;
}
</style>
