<template>
  <div class="limitbox">
    <nav>
      <div>
        <a class="item item--selected" href="">Home</a>
        <a class="item" href="">Browse</a>
      </div>
      <input class="searchbar" type="text" placeholder="Search" />
      <a class="item" href="">Cart {{ gamesInCart }}</a>
    </nav>
  </div>
</template>

<script></script>

<style scoped>
.limitbox {
  display: flex;
  height: 48px;
  background-color: var(--Color-Background-Tertiary);
  align-items: center;
  justify-content: center;
  box-shadow: var(--ShadowLight);
}

nav {
  width: 940px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

a {
  text-decoration: none;
  color: var(--Color-Text-Main);
  font: var(--Text-BodyLarge);
}

.item {
  padding: 6px 24px;
  margin: 0px 6px;
}

.item--selected,
.item:hover {
  border-radius: 3px;
  background-color: var(--Color-Secondary);
}

.searchbar {
  width: 300px;
  background-color: var(--Color-Background-Main20);
  border: none;
  height: 40px;
  color: var(--Color-Text-Main);
  border-radius: 3px;
}

.searchbar:focus {
  outline: none;
}
</style>
