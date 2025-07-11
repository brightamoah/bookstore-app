import type { FormError } from '@nuxt/ui'

const validateBookName = (value: string): string => {
  if (!value || !value.trim()) {
    return 'Book name is required'
  }
  if (value.trim().length < 2) {
    return 'Book name must be at least 2 characters long'
  }
  if (value.trim().length > 100) {
    return 'Book name must not exceed 100 characters'
  }
  return ''
}

const validateCategory = (value: string): string => {
  if (!value || !value.trim()) {
    return 'Category is required'
  }
  const validCategories = [
    'Fiction',
    'Non-Fiction',
    'Mystery',
    'Romance',
    'Science Fiction',
    'Fantasy',
    'Biography',
    'History',
    'Self-Help',
    'Business',
    'Poetry',
    'Drama',
    'Horror',
    'Thriller',
    'Dystopian',
  ]
  if (!validCategories.includes(value)) {
    return 'Please select a valid category'
  }
  return ''
}

const validateAuthor = (value: string): string => {
  if (!value || !value.trim()) {
    return 'Author name is required'
  }
  if (value.trim().length < 2) {
    return 'Author name must be at least 2 characters long'
  }
  if (value.trim().length > 50) {
    return 'Author name must not exceed 50 characters'
  }
  // Check for valid author name format (letters, spaces, hyphens, apostrophes)
  const authorRegex = /^[a-zA-Z\s\-'\.]+$/
  if (!authorRegex.test(value.trim())) {
    return 'Author name can only contain letters, spaces, hyphens, and apostrophes'
  }
  return ''
}

const validatePrice = (value: number): string => {
  if (value === null || value === undefined) {
    return 'Price is required'
  }
  if (isNaN(value)) {
    return 'Price must be a valid number'
  }
  if (value < 0) {
    return 'Price cannot be negative'
  }
  if (value === 0) {
    return 'Price must be greater than 0'
  }
  if (value > 999999.99) {
    return 'Price cannot exceed $999,999.99'
  }
  // Check for valid decimal places (max 2)
  const decimalPlaces = (value.toString().split('.')[1] || '').length
  if (decimalPlaces > 2) {
    return 'Price can have at most 2 decimal places'
  }
  return ''
}

const validateDescription = (value: string): string => {
  if (!value || !value.trim()) {
    return 'Description is required'
  }
  if (value.trim().length < 10) {
    return 'Description must be at least 10 characters long'
  }
  if (value.trim().length > 1000) {
    return 'Description must not exceed 1000 characters'
  }
  return ''
}

const validateImageUrl = (value: string): string => {
  if (!value || !value.trim()) {
    return 'Book image is required'
  }
  // Check if it's a base64 string (for uploaded files)
  if (value.startsWith('data:image/')) {
    const sizeInBytes = (value.length * 3) / 4
    const maxSizeInMB = 5
    if (sizeInBytes > maxSizeInMB * 1024 * 1024) {
      return `Image size must be less than ${maxSizeInMB}MB`
    }
    return ''
  }
  // Check if it's a valid URL
  try {
    new URL(value)
    return ''
  } catch {
    return 'Please provide a valid image URL or upload an image file'
  }
}

export {
  validateBookName,
  validateCategory,
  validateAuthor,
  validatePrice,
  validateDescription,
  validateImageUrl,
}

export const validateForm = (state: any): FormError[] => {
  const errors: FormError[] = []

  // Book Name validation
  if (!state.bookName || !state.bookName.trim()) {
    errors.push({ name: 'bookName', message: 'Book name is required' })
  } else if (state.bookName.trim().length < 2) {
    errors.push({ name: 'bookName', message: 'Book name must be at least 2 characters long' })
  } else if (state.bookName.trim().length > 100) {
    errors.push({ name: 'bookName', message: 'Book name must not exceed 100 characters' })
  }

  // Category validation
  if (!state.category || !state.category.trim()) {
    errors.push({ name: 'category', message: 'Category is required' })
  }

  // Author validation
  if (!state.author || !state.author.trim()) {
    errors.push({ name: 'author', message: 'Author name is required' })
  } else if (state.author.trim().length < 2) {
    errors.push({ name: 'author', message: 'Author name must be at least 2 characters long' })
  } else if (state.author.trim().length > 50) {
    errors.push({ name: 'author', message: 'Author name must not exceed 50 characters' })
  } else if (!/^[a-zA-Z\s\-'\.]+$/.test(state.author.trim())) {
    errors.push({
      name: 'author',
      message: 'Author name can only contain letters, spaces, hyphens, and apostrophes',
    })
  }

  // Price validation
  if (state.price === null || state.price === undefined) {
    errors.push({ name: 'price', message: 'Price is required' })
  } else if (isNaN(state.price)) {
    errors.push({ name: 'price', message: 'Price must be a valid number' })
  } else if (state.price <= 0) {
    errors.push({ name: 'price', message: 'Price must be greater than 0' })
  } else if (state.price > 999999.99) {
    errors.push({ name: 'price', message: 'Price cannot exceed $999,999.99' })
  }

  // Description validation
  if (!state.description || !state.description.trim()) {
    errors.push({ name: 'description', message: 'Description is required' })
  } else if (state.description.trim().length < 10) {
    errors.push({ name: 'description', message: 'Description must be at least 10 characters long' })
  } else if (state.description.trim().length > 1000) {
    errors.push({ name: 'description', message: 'Description must not exceed 1000 characters' })
  }

  // Image URL validation
  if (!state.imageUrl || !state.imageUrl.trim()) {
    errors.push({ name: 'imageUrl', message: 'Book image is required' })
  }

  return errors
}
