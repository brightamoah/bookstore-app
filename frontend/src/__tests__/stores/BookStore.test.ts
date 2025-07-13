import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useBookStore } from '../../stores/BookStore'
import { mockBooks } from '../mocks/api'
import type { Book } from '../../Types/types'

// Mock the API service
vi.mock('../../services/Api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}))

import api from '../../services/Api'
const mockApi = api as any

describe('BookStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Initial State', () => {
    it('should have correct initial state', () => {
      const bookStore = useBookStore()

      expect(bookStore.books).toEqual([])
      expect(bookStore.book).toBeNull()
      expect(bookStore.isLoading).toBe(false)
      expect(bookStore.errorMessage).toBeNull()
      expect(bookStore.totalBooks).toBe(0)
      expect(bookStore.imagePreview).toBe('')
    })

    it('should have correct initial form data', () => {
      const bookStore = useBookStore()

      expect(bookStore.bookFormData).toEqual({
        bookName: '',
        category: 'Fiction',
        author: '',
        price: 0,
        description: '',
        imageUrl: '',
        createdAt: expect.any(String),
        updatedAt: null,
      })
    })
  })

  describe('Computed Properties', () => {
    it('should compute totalBooks correctly', () => {
      const bookStore = useBookStore()

      expect(bookStore.totalBooks).toBe(0)

      bookStore.books = mockBooks
      expect(bookStore.totalBooks).toBe(mockBooks.length)
    })

    it('should compute booksByCategory correctly', () => {
      const bookStore = useBookStore()
      bookStore.books = mockBooks

      expect(bookStore.booksByCategory).toEqual({
        Fiction: 1,
        'Non-Fiction': 1,
      })
    })

    it('should compute isFormValid correctly', () => {
      const bookStore = useBookStore()

      expect(bookStore.isFormValid).toBe(false)

      // Set valid form data
      bookStore.bookFormData.bookName = 'Test Book'
      bookStore.bookFormData.category = 'Fiction'
      bookStore.bookFormData.author = 'Test Author'
      bookStore.bookFormData.price = 19.99
      bookStore.bookFormData.description = 'This is a test book description that is long enough'
      bookStore.bookFormData.imageUrl = 'https://example.com/image.jpg'

      expect(bookStore.isFormValid).toBe(true)
    })
  })

  describe('Actions', () => {
    describe('getAllBooks', () => {
      it('should fetch all books successfully', async () => {
        const bookStore = useBookStore()

        // Mock successful API response
        mockApi.get.mockResolvedValueOnce({ data: mockBooks })

        const result = await bookStore.getAllBooks()

        expect(mockApi.get).toHaveBeenCalledWith('/api/books')
        expect(bookStore.books).toEqual(mockBooks)
        expect(bookStore.isLoading).toBe(false)
        expect(bookStore.errorMessage).toBeNull()
        expect(result).toEqual(mockBooks)
      })

      it('should handle fetch books error', async () => {
        const bookStore = useBookStore()

        // Mock API error
        mockApi.get.mockRejectedValueOnce(new Error('Server error'))

        await expect(bookStore.getAllBooks()).rejects.toThrow()
        expect(bookStore.errorMessage).toBeTruthy()
        expect(bookStore.isLoading).toBe(false)
      })
    })

    describe('getBookById', () => {
      it('should fetch book by id successfully', async () => {
        const bookStore = useBookStore()

        // Mock successful API response
        mockApi.get.mockResolvedValueOnce({ data: mockBooks[0] })

        const result = await bookStore.getBookById(1)

        expect(mockApi.get).toHaveBeenCalledWith('/api/books/1')
        expect(result).toEqual(mockBooks[0])
        expect(bookStore.book).toEqual(mockBooks[0])
      })

      it('should handle book not found', async () => {
        const bookStore = useBookStore()

        // Mock API error for book not found
        mockApi.get.mockRejectedValueOnce(new Error('Book not found'))

        await expect(bookStore.getBookById(999)).rejects.toThrow()
        expect(bookStore.errorMessage).toBeTruthy()
      })
    })

    describe('createBook', () => {
      it('should create book successfully', async () => {
        const bookStore = useBookStore()

        // Set valid form data
        bookStore.bookFormData.bookName = 'New Test Book'
        bookStore.bookFormData.category = 'Fiction'
        bookStore.bookFormData.author = 'New Author'
        bookStore.bookFormData.price = 29.99
        bookStore.bookFormData.description = 'This is a new test book description'
        bookStore.bookFormData.imageUrl = 'https://example.com/new-image.jpg'

        const newBook: Omit<Book, 'bookId'> = {
          bookName: 'New Test Book',
          category: 'Fiction',
          author: 'New Author',
          price: 29.99,
          description: 'This is a new test book description',
          imageUrl: 'https://example.com/new-image.jpg',
          createdAt: new Date().toISOString(),
          updatedAt: null,
        }

        const mockCreatedBook = { ...newBook, bookId: 3 }

        // Mock successful API response
        mockApi.post.mockResolvedValueOnce({ data: mockCreatedBook })

        const result = await bookStore.createBook(newBook)

        expect(mockApi.post).toHaveBeenCalledWith('/api/books', bookStore.bookFormData)
        expect(result).toMatchObject({
          bookName: 'New Test Book',
          category: 'Fiction',
          author: 'New Author',
          price: 29.99,
        })
        expect(bookStore.books).toHaveLength(1)
      })

      it('should handle validation errors', async () => {
        const bookStore = useBookStore()

        // Invalid form data
        const invalidBook: Omit<Book, 'bookId'> = {
          bookName: '',
          category: '',
          author: '',
          price: 0,
          description: '',
          imageUrl: '',
          createdAt: new Date().toISOString(),
          updatedAt: null,
        }

        await expect(bookStore.createBook(invalidBook)).rejects.toThrow('Form validation failed')
      })
    })

    describe('updateBook', () => {
      it('should update book successfully', async () => {
        const bookStore = useBookStore()
        bookStore.books = [...mockBooks]

        const updateData: Partial<Book> = {
          bookName: 'Updated Book Name',
          price: 39.99,
        }

        const expectedRequest = {
          bookId: 1,
          bookName: 'Updated Book Name',
          category: undefined,
          author: undefined,
          price: 39.99,
          description: undefined,
          imageUrl: '',
          createdAt: undefined,
          updatedAt: expect.any(String),
        }

        const expectedResponse = { ...mockBooks[0], ...updateData }

        // Mock successful API response
        mockApi.put.mockResolvedValueOnce({ data: expectedResponse })

        const result = await bookStore.updateBook(1, updateData)

        expect(mockApi.put).toHaveBeenCalledWith('/api/books/1', expectedRequest)
        expect(result).toMatchObject(updateData)
        expect(bookStore.books[0]).toMatchObject(updateData)
      })

      it('should handle update non-existent book', async () => {
        const bookStore = useBookStore()

        // Mock API error for non-existent book
        mockApi.put.mockRejectedValueOnce(new Error('Book not found'))

        await expect(bookStore.updateBook(999, { bookName: 'Updated' })).rejects.toThrow(
          'Book not found',
        )
      })
    })

    describe('deleteBook', () => {
      it('should delete book successfully', async () => {
        const bookStore = useBookStore()
        bookStore.books = [...mockBooks]

        // Mock successful API response
        mockApi.delete.mockResolvedValueOnce({ data: { message: 'Book deleted successfully' } })

        await bookStore.deleteBook(1)

        expect(mockApi.delete).toHaveBeenCalledWith('/api/books/1')
        expect(bookStore.books).toHaveLength(1)
        expect(bookStore.books[0].bookId).toBe(2)
      })

      it('should handle delete non-existent book', async () => {
        const bookStore = useBookStore()

        // Mock API error for non-existent book
        mockApi.delete.mockRejectedValueOnce(new Error('Book not found'))

        await expect(bookStore.deleteBook(999)).rejects.toThrow('Book not found')
      })
    })

    describe('File Handling', () => {
      it('should handle file change correctly', () => {
        const bookStore = useBookStore()

        // Mock file
        const mockFile = new File(['test'], 'test.jpg', { type: 'image/jpeg' })

        // Mock event
        const mockEvent = {
          target: {
            files: [mockFile],
          },
        } as any

        // Mock FileReader
        const mockFileReader = {
          result: 'data:image/jpeg;base64,mockdata',
          readAsDataURL: vi.fn(function (this: any) {
            setTimeout(() => {
              this.onload({ target: { result: this.result } })
            }, 0)
          }),
          onload: vi.fn(),
        }

        global.FileReader = vi.fn(() => mockFileReader) as any
        global.URL.createObjectURL = vi.fn(() => 'mock-object-url')

        bookStore.handleFileChange(mockEvent)

        expect(global.URL.createObjectURL).toHaveBeenCalledWith(mockFile)
        expect(bookStore.imagePreview).toBe('mock-object-url')
      })

      it('should reject invalid file types', () => {
        const bookStore = useBookStore()

        const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' })
        const mockEvent = {
          target: {
            files: [mockFile],
          },
        } as any

        bookStore.handleFileChange(mockEvent)

        expect(bookStore.imagePreview).toBe('')
      })

      it('should reject files too large', () => {
        const bookStore = useBookStore()

        // Mock large file (> 5MB)
        const mockFile = new File(['x'.repeat(6 * 1024 * 1024)], 'large.jpg', {
          type: 'image/jpeg',
        })
        Object.defineProperty(mockFile, 'size', { value: 6 * 1024 * 1024 })

        const mockEvent = {
          target: {
            files: [mockFile],
          },
        } as any

        bookStore.handleFileChange(mockEvent)

        expect(bookStore.imagePreview).toBe('')
      })
    })

    describe('Helper Methods', () => {
      it('should clear book form data', () => {
        const bookStore = useBookStore()

        bookStore.bookFormData.bookName = 'Test'
        bookStore.bookFormData.author = 'Author'

        bookStore.clearBookFormdata()

        expect(bookStore.bookFormData.bookName).toBe('')
        expect(bookStore.bookFormData.author).toBe('')
      })

      it('should clear error', () => {
        const bookStore = useBookStore()
        bookStore.errorMessage = 'Some error'

        bookStore.clearError()

        expect(bookStore.errorMessage).toBeNull()
      })

      it('should clear image', () => {
        const bookStore = useBookStore()
        bookStore.imagePreview = 'mock-url'
        global.URL.revokeObjectURL = vi.fn()

        bookStore.clearImage()

        expect(global.URL.revokeObjectURL).toHaveBeenCalledWith('mock-url')
        expect(bookStore.imagePreview).toBe('')
        expect(bookStore.bookFormData.imageUrl).toBe('')
      })

      it('should clear book state', () => {
        const bookStore = useBookStore()
        bookStore.books = mockBooks
        bookStore.book = mockBooks[0]
        bookStore.errorMessage = 'Error'

        bookStore.clearBookState()

        expect(bookStore.books).toEqual([])
        expect(bookStore.book).toBeNull()
        expect(bookStore.errorMessage).toBeNull()
      })
    })
  })
})
