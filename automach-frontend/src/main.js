import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import "./assets/css/main.css";
import { eventBus } from "./eventBus";

// Import FontAwesome
import { library } from '@fortawesome/fontawesome-svg-core';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { 
  faCreditCard, 
  faCheckCircle, 
  faArrowLeft, 
  faGamepad, 
  faHome, 
  faShoppingBag,
  faDownload,
  faEnvelope,
  faHeadset,
  faSpinner,
  faCheck,
  faPlay,
  faShoppingCart
} from '@fortawesome/free-solid-svg-icons';
import { 
  faCcVisa, 
  faCcMastercard, 
  faPaypal 
} from '@fortawesome/free-brands-svg-icons';

// Add icons to the library
library.add(
  faCreditCard,
  faCheckCircle,
  faArrowLeft,
  faGamepad,
  faHome,
  faShoppingBag,
  faDownload,
  faEnvelope,
  faHeadset,
  faSpinner,
  faCheck,
  faPlay,
  faShoppingCart,
  faCcVisa,
  faCcMastercard,
  faPaypal
);

// Tạo ứng dụng Vue
const app = createApp(App);

// Register FontAwesome component globally
app.component('font-awesome-icon', FontAwesomeIcon);

// Thêm eventBus vào app để các component có thể sử dụng
app.config.globalProperties.eventBus = eventBus;

app.use(router);
app.mount("#app");
