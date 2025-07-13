import { describe, it, expect } from 'vitest'
import {
  validateBookName,
  validateCategory,
  validateAuthor,
  validatePrice,
  validateDescription,
  validateImageUrl,
  validateForm,
} from '../../utils/validations.ts'

describe('Validation Utils', () => {
  describe('validateBookName', () => {
    it('should return empty string for valid book name', () => {
      expect(validateBookName('Valid Book Name')).toBe('')
      expect(validateBookName('AB')).toBe('')
      expect(validateBookName('A'.repeat(100))).toBe('')
    })

    it('should return error for empty book name', () => {
      expect(validateBookName('')).toBe('Book name is required')
      expect(validateBookName('   ')).toBe('Book name is required')
    })

    it('should return error for book name too short', () => {
      expect(validateBookName('A')).toBe('Book name must be at least 2 characters long')
    })

    it('should return error for book name too long', () => {
      expect(validateBookName('A'.repeat(101))).toBe('Book name must not exceed 100 characters')
    })
  })

  describe('validateCategory', () => {
    it('should return empty string for valid category', () => {
      expect(validateCategory('Fiction')).toBe('')
      expect(validateCategory('Non-Fiction')).toBe('')
      expect(validateCategory('Science Fiction')).toBe('')
    })

    it('should return error for empty category', () => {
      expect(validateCategory('')).toBe('Category is required')
      expect(validateCategory('   ')).toBe('Category is required')
    })

    it('should return error for invalid category', () => {
      expect(validateCategory('Invalid Category')).toBe('Please select a valid category')
    })
  })

  describe('validateAuthor', () => {
    it('should return empty string for valid author name', () => {
      expect(validateAuthor('John Doe')).toBe('')
      expect(validateAuthor("O'Connor")).toBe('')
      expect(validateAuthor('Jean-Pierre')).toBe('')
      expect(validateAuthor('Dr. Smith')).toBe('')
    })

    it('should return error for empty author name', () => {
      expect(validateAuthor('')).toBe('Author name is required')
      expect(validateAuthor('   ')).toBe('Author name is required')
    })

    it('should return error for author name too short', () => {
      expect(validateAuthor('A')).toBe('Author name must be at least 2 characters long')
    })

    it('should return error for author name too long', () => {
      expect(validateAuthor('A'.repeat(51))).toBe('Author name must not exceed 50 characters')
    })

    it('should return error for invalid characters', () => {
      expect(validateAuthor('Author123')).toBe(
        'Author name can only contain letters, spaces, hyphens, and apostrophes',
      )
      expect(validateAuthor('Author@Name')).toBe(
        'Author name can only contain letters, spaces, hyphens, and apostrophes',
      )
    })
  })

  describe('validatePrice', () => {
    it('should return empty string for valid price', () => {
      expect(validatePrice(10)).toBe('')
      expect(validatePrice(99.99)).toBe('')
      expect(validatePrice(0.01)).toBe('')
    })

    it('should return error for null/undefined price', () => {
      expect(validatePrice(null as any)).toBe('Price is required')
      expect(validatePrice(undefined as any)).toBe('Price is required')
    })

    it('should return error for NaN price', () => {
      expect(validatePrice(NaN)).toBe('Price must be a valid number')
    })

    it('should return error for negative price', () => {
      expect(validatePrice(-1)).toBe('Price cannot be negative')
    })

    it('should return error for zero price', () => {
      expect(validatePrice(0)).toBe('Price must be greater than 0')
    })

    it('should return error for price too high', () => {
      expect(validatePrice(1000000)).toBe('Price cannot exceed $999,999.99')
    })

    it('should return error for too many decimal places', () => {
      expect(validatePrice(10.123)).toBe('Price can have at most 2 decimal places')
    })
  })

  describe('validateDescription', () => {
    it('should return empty string for valid description', () => {
      expect(validateDescription('This is a valid description that is long enough')).toBe('')
      expect(validateDescription('A'.repeat(1000))).toBe('')
    })

    it('should return error for empty description', () => {
      expect(validateDescription('')).toBe('Description is required')
      expect(validateDescription('   ')).toBe('Description is required')
    })

    it('should return error for description too short', () => {
      expect(validateDescription('Too short')).toBe(
        'Description must be at least 10 characters long',
      )
    })

    it('should return error for description too long', () => {
      expect(validateDescription('A'.repeat(1001))).toBe(
        'Description must not exceed 1000 characters',
      )
    })
  })

  describe('validateImageUrl', () => {
    it('should return empty string for valid URL', () => {
      expect(validateImageUrl('https://example.com/image.jpg')).toBe('')
      expect(validateImageUrl('http://localhost:3000/image.png')).toBe('')
    })

    it('should return empty string for valid base64 data', () => {
      expect(validateImageUrl('data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD')).toBe('')
    })

    it('should return error for empty image URL', () => {
      expect(validateImageUrl('')).toBe('Book image is required')
      expect(validateImageUrl('   ')).toBe('Book image is required')
    })

    it('should return error for invalid URL', () => {
      expect(validateImageUrl('invalid-url')).toBe(
        'Please provide a valid image URL or upload an image file',
      )
    })

    it('should return error for base64 image too large', () => {
      const largeBase64 = 'data:image/jpeg;base64,' + 'A'.repeat(7000000) // > 5MB
      expect(validateImageUrl(largeBase64)).toBe('Image size must be less than 5MB')
    })
  })

  describe('validateForm', () => {
    const validFormData = {
      bookName: 'Valid Book',
      category: 'Fiction',
      author: 'Valid Author',
      price: 19.99,
      description: 'This is a valid description that meets the requirements',
      imageUrl: 'https://example.com/image.jpg',
    }

    it('should return empty array for valid form data', () => {
      expect(validateForm(validFormData)).toEqual([])
    })

    it('should return errors for invalid form data', () => {
      const invalidFormData = {
        bookName: '',
        category: '',
        author: '',
        price: 0,
        description: '',
        imageUrl: '',
      }

      const errors = validateForm(invalidFormData)
      expect(errors).toHaveLength(6)
      expect(errors.map((e) => e.name)).toEqual([
        'bookName',
        'category',
        'author',
        'price',
        'description',
        'imageUrl',
      ])
    })

    it('should return specific validation errors', () => {
      const partiallyInvalidData = {
        ...validFormData,
        bookName: 'A', // too short
        price: -10, // negative
        description: 'short', // too short
      }

      const errors = validateForm(partiallyInvalidData)
      expect(errors).toHaveLength(3)
      expect(errors.find((e) => e.name === 'bookName')?.message).toBe(
        'Book name must be at least 2 characters long',
      )
      expect(errors.find((e) => e.name === 'price')?.message).toBe('Price must be greater than 0')
      expect(errors.find((e) => e.name === 'description')?.message).toBe(
        'Description must be at least 10 characters long',
      )
    })
  })
})
