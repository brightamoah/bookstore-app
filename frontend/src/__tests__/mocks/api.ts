import { http, HttpResponse } from 'msw'
import type { Book, User, SearchResult } from '../../Types/types'

// Mock data
export const mockUser: User = {
  userId: '1',
  firstName: 'John',
  lastName: 'Doe',
  email: 'john.doe@example.com',
  isVerified: true,
}

// Mock API response data (different structure from User interface)
export const mockUserApiResponse = {
  id: '1', // Note: API returns 'id', not 'userId'
  firstName: 'John',
  lastName: 'Doe',
  email: 'john.doe@example.com',
  isVerified: true,
}

export const mockBooks: Book[] = [
  {
    bookId: 1,
    bookName: 'Test Book 1',
    category: 'Fiction',
    author: 'Test Author 1',
    price: 19.99,
    description: 'A great test book for fiction lovers',
    imageUrl: '/test-image-1.jpg',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  },
  {
    bookId: 2,
    bookName: 'Test Book 2',
    category: 'Non-Fiction',
    author: 'Test Author 2',
    price: 24.99,
    description: 'An informative non-fiction book',
    imageUrl: '/test-image-2.jpg',
    createdAt: '2024-01-02T00:00:00Z',
    updatedAt: '2024-01-02T00:00:00Z',
  },
]

export const mockSearchResult: SearchResult = {
  books: mockBooks,
  page: 1,
  pageSize: 10,
  query: 'test',
  totalResults: mockBooks.length,
}

// MSW handlers
export const handlers = [
  // Auth endpoints
  http.post('http://localhost:8000/api/login', () => {
    return HttpResponse.json({ message: 'Login successful' }, { status: 200 })
  }),

  http.post('http://localhost:8000/api/logout', () => {
    return HttpResponse.json({ message: 'Logout successful' }, { status: 200 })
  }),

  http.post('http://localhost:8000/api/signup', () => {
    return HttpResponse.json({ message: 'Signup successful' }, { status: 201 })
  }),

  http.get('http://localhost:8000/api/user', () => {
    return HttpResponse.json(mockUserApiResponse, { status: 200 })
  }),

  // Book endpoints
  http.get('http://localhost:8000/api/books', () => {
    return HttpResponse.json(mockBooks, { status: 200 })
  }),

  http.get('http://localhost:8000/api/books/:id', ({ params }) => {
    const id = Number(params.id)
    const book = mockBooks.find((b) => b.bookId === id)
    if (book) {
      return HttpResponse.json(book, { status: 200 })
    }
    return HttpResponse.json({ message: 'Book not found' }, { status: 404 })
  }),

  http.post('http://localhost:8000/api/books', async ({ request }) => {
    const newBook = (await request.json()) as Omit<Book, 'bookId'>
    const book: Book = {
      ...newBook,
      bookId: mockBooks.length + 1,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }
    return HttpResponse.json(book, { status: 201 })
  }),

  http.put('http://localhost:8000/api/books/:id', async ({ params, request }) => {
    const id = Number(params.id)
    const updateData = (await request.json()) as Partial<Book>
    const book = mockBooks.find((b) => b.bookId === id)
    if (book) {
      const updatedBook = { ...book, ...updateData, updatedAt: new Date().toISOString() }
      return HttpResponse.json(updatedBook, { status: 200 })
    }
    return HttpResponse.json({ message: 'Book not found' }, { status: 404 })
  }),

  http.delete('http://localhost:8000/api/books/:id', ({ params }) => {
    const id = Number(params.id)
    const bookIndex = mockBooks.findIndex((b) => b.bookId === id)
    if (bookIndex !== -1) {
      return HttpResponse.json({ message: 'Book deleted successfully' }, { status: 200 })
    }
    return HttpResponse.json({ message: 'Book not found' }, { status: 404 })
  }),

  // Search endpoint
  http.get('http://localhost:8000/api/books/search', ({ request }) => {
    const url = new URL(request.url)
    const query = url.searchParams.get('query') || ''
    const filteredBooks = mockBooks.filter(
      (book) =>
        book.bookName.toLowerCase().includes(query.toLowerCase()) ||
        book.author.toLowerCase().includes(query.toLowerCase()),
    )
    return HttpResponse.json(
      {
        books: filteredBooks,
        totalResults: filteredBooks.length,
        page: 1,
        pageSize: 10,
        query,
      } as SearchResult,
      { status: 200 },
    )
  }),
]

// Error handlers for testing error scenarios
export const errorHandlers = [
  http.post('http://localhost:8000/api/login', () => {
    return HttpResponse.json({ message: 'Invalid credentials' }, { status: 401 })
  }),

  http.post('http://localhost:8000/api/signup', () => {
    return HttpResponse.json({ message: 'User already exists' }, { status: 400 })
  }),

  http.get('http://localhost:8000/api/books', () => {
    return HttpResponse.json({ message: 'Internal server error' }, { status: 500 })
  }),

  http.get('http://localhost:8000/api/user', () => {
    return HttpResponse.json({ message: 'Unauthorized' }, { status: 401 })
  }),
]
