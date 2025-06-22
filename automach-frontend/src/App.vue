<template>
  <SupNav />
  <Register />
  <Footer />
</template>

<script>
import SupNav from "./components/SupNav.vue";
import Home from "./components/Home.vue";
import Footer from "./components/Footer.vue";
import Login from "./components/Login.vue";
import Register from "./components/Register.vue";

export default {
  data() {
    return {};
  },

  components: {
    SupNav,
    Home, 
    Login,
    Register,
    Footer,
  },
};
</script>

<style></style>
