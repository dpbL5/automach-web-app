<script setup></script>

<template>
  <footer class="footer">
    <div class="footer-container">
      <div class="footer-section">
        <img src="../assets/images/logo.png" alt="Logo" />
        <p>
          © 2025 Automach Corporation. All rights reserved. All trademarks are
          property of their respective owners in the US and other countries. VAT
          included in all prices where applicable.
        </p>
      </div>
      <div class="footer-section">
        <ul>
          <li><a href="#">About</a></li>
          <li><a href="#">Privacy Policy</a></li>
          <li><a href="#">Legal</a></li>
          <li><a href="#">Cookies</a></li>
        </ul>
      </div>
      <div class="footer-section">
        <ul>
          <li><a href="">Facebook</a></li>
          <li><a href="">X (Twitter)</a></li>
        </ul>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.footer {
  width: 100%;
  height: 200px;
  background: var(--Color-Background-Highlight);
  color: var(--Color-Text-Main, #f3f3f3);
  padding: 24px 0;
  font: var(--Text-BodyMedium);
}
.footer-container {
  display: flex;
  justify-content: space-around;
  align-items: flex-start;
  max-width: 940px;
  margin: 0 auto;
  gap: 32px;
}

.footer-section {
  width: 350px;
}

ul {
  list-style: none;
}

li {
  margin: 12px 0;
}

li a {
  text-decoration: none;
  color: var(--Color-Text-Main);
  margin: 10px 0;
}
p {
  text-decoration: none;
  color: var(--Color-Text-Dim);
}
</style>
