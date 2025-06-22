<template>
  <Nav />
  <div class="view">
    <div class="view-wrapper">
      <br />
      <h3>Feature & Recommended</h3>
      <FeatureSection />
      <br />
      <h3>Free to play games</h3>
      <br />
      <h3>New & Trendings</h3>
      <br />
      <h3>Top sellers</h3>
    </div>
  </div>
</template>
<script>
import Nav from "./Nav.vue";
import FeatureSection from "./FeatureSection.vue";

export default {
  components: {
    Nav,
    FeatureSection,
  },
};
</script>
<style>
.view {
  height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.view-wrapper {
  width: 940px;
}

h3 {
  font: var(--Text-BodyMedium);
  color: var(--Color-Text-Main);
}
</style>
