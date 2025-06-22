<template>
  <header>
    <div class="limitbox">
      <img class="logo" src="../assets/images/logo.png" alt="logo" />
      <nav class="container">
        <ul>
          <li>Store</li>
          <li>{{ Name }}</li>
          <li>Library</li>
        </ul>
      </nav>
      <div class="cta">
        <a href="">Log in</a>
        <button>Register</button>
      </div>
    </div>
  </header>
</template>

<script></script>

<style scoped>
header {
  height: 58px;
  background-color: var(--Color-Background-Main);
  display: flex;
  justify-content: center;
}

.limitbox {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 940px;
}

.logo {
  height: 16px;
}

nav,
ul {
  display: flex;
  list-style: none;
  justify-content: space-between;
}

li,
a {
  text-decoration: none;
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingSmall);
  padding: 0px 16px;
}

li {
  text-transform: uppercase;
}

.cta {
  align-items: center;
}

button {
  border: none;
  border-radius: 3px;
  background-color: var(--Color-Background-Highlight);
  color: var(--Color-Text-Main);
  font: var(--Text-HeadingSmall);
  padding: 8px 16px;
}
</style>
