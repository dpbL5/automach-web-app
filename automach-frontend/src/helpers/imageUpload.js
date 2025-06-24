/**
 * Helper function to convert an image file to a base64 string
 * @param {File} file - The image file to convert
 * @returns {Promise<string>} - A promise that resolves to the base64 string
 */
export const convertFileToBase64 = (file) => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result);
    reader.onerror = (error) => reject(error);
    reader.readAsDataURL(file);
  });
};

/**
 * Compress an image file to reduce size and improve upload speed
 * @param {File} file - The image file to compress
 * @param {number} maxWidthOrHeight - Maximum width or height of the compressed image
 * @param {number} quality - Image quality (0 to 1)
 * @returns {Promise<Blob>} - A promise that resolves to the compressed image as Blob
 */
export const compressImage = (file, maxWidthOrHeight = 1200, quality = 0.7) => {
  return new Promise((resolve, reject) => {
    // Create image element to load the file
    const img = new Image();
    img.src = URL.createObjectURL(file);
    
    img.onload = () => {
      // Create canvas for resizing
      const canvas = document.createElement('canvas');
      let width = img.width;
      let height = img.height;
      
      // Calculate new dimensions while maintaining aspect ratio
      if (width > height) {
        if (width > maxWidthOrHeight) {
          height = Math.round(height * maxWidthOrHeight / width);
          width = maxWidthOrHeight;
        }
      } else {
        if (height > maxWidthOrHeight) {
          width = Math.round(width * maxWidthOrHeight / height);
          height = maxWidthOrHeight;
        }
      }
      
      // Set canvas dimensions
      canvas.width = width;
      canvas.height = height;
      
      // Draw resized image on canvas
      const ctx = canvas.getContext('2d');
      ctx.drawImage(img, 0, 0, width, height);
      
      // Convert to blob with specified quality
      canvas.toBlob(
        blob => {
          // Release object URL to prevent memory leaks
          URL.revokeObjectURL(img.src);
          resolve(blob);
        },
        file.type,
        quality
      );
    };
    
    img.onerror = (error) => {
      URL.revokeObjectURL(img.src);
      reject(error);
    };
  });
};

/**
 * Upload an image and get back a URL
 * Note: In a real app, you would upload to your server or a third-party service
 * For this example, we're using a simulated approach that converts to base64
 * @param {File} file - The image file to upload
 * @returns {Promise<string>} - A promise that resolves to the image URL
 */
export const uploadImage = async (file) => {
  try {
    // Compress the image before uploading
    const compressedImage = await compressImage(file);
    
    // Convert compressed image to base64
    const base64Image = await convertFileToBase64(compressedImage);
    
    // Simulating a server response with a shorter delay
    await new Promise(resolve => setTimeout(resolve, 200));
    
    // Return the base64 string as the "URL"
    return base64Image;
  } catch (error) {
    console.error('Error uploading image:', error);
    throw error;
  }
}; 