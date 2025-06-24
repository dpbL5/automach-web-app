/**
 * Helper functions for cart management
 */

// Get cart from localStorage
export const getCart = () => {
  try {
    return JSON.parse(localStorage.getItem('cart')) || [];
  } catch (error) {
    console.error('Error parsing cart from localStorage:', error);
    return [];
  }
};

// Save cart to localStorage and emit update event
export const saveCart = (cart) => {
  try {
    localStorage.setItem('cart', JSON.stringify(cart));
    // Emit event to update Nav component
    window.dispatchEvent(new Event('cart-updated'));
  } catch (error) {
    console.error('Error saving cart to localStorage:', error);
  }
};

// Add game to cart (only if not already exists)
export const addToCart = (gameId) => {
  const cart = getCart();
  
  if (!cart.includes(gameId)) {
    cart.push(gameId);
    saveCart(cart);
    return true; // Successfully added
  }
  
  return false; // Already exists
};

// Remove game from cart
export const removeFromCart = (gameId) => {
  const cart = getCart();
  const newCart = cart.filter(id => id !== gameId);
  saveCart(newCart);
  return newCart;
};

// Clear entire cart
export const clearCart = () => {
  localStorage.removeItem('cart');
  window.dispatchEvent(new Event('cart-updated'));
};

// Get cart count
export const getCartCount = () => {
  return getCart().length;
};

// Check if game is in cart
export const isInCart = (gameId) => {
  return getCart().includes(gameId);
}; 