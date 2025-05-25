<script setup>

</script>

<template>
    <footer class="footer">
        <div class="footer-container">
            <div class="footer-section">
                <img src="../assets/images/AUTOMACH.png" alt="Logo">
                <p>© 2025 Automach Corporation. All rights reserved. All trademarks are property of their respective owners in the US and other countries.
VAT included in all prices where applicable.</p>
            </div>
            <div class="footer-section">
                <h3>Links</h3>
                <ul>
                    <li><a href="#">Home</a></li>   
                    <li><a href="#">About</a></li>
                    <li><a href="#">Contact</a></li>
                </ul>
            </div>
            <div class="footer-section">
                <h3>Contact</h3>
                <p>Email: info@automach.com</p>
                <p>Phone: +123 456 7890</p>
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
  max-width: 1200px;
  margin: 0 auto;
  gap: 32px;
}

.footer-section {
    width: 300px;
}

a, p {
    color: var(--Color-Text-Dim);
}
</style>