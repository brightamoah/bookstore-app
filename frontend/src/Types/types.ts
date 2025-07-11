export interface User {
  userId: string
  firstName: string
  lastName: string
  email: string
  isVerified: boolean
}

export interface AuthData extends User {
  password: string
  confirmPassword: string
  isLoggedIn: boolean
  loginFormData: {
    email: string
    password: string
    rememberMe?: boolean
  }
  registerFormData: {
    firstName: string
    lastName: string
    email: string
    password: string
    confirmPassword: string
  }
}

export interface Book {
  bookId: number
  id?: number
  bookName: string
  category: string
  price: number
  description: string
  imageUrl: string
  author: string
  createdAt?: string
  updatedAt?: string | null
}

export interface ValidationErrors {
  bookName?: string
  category?: string
  author?: string
  price?: string
  description?: string
  imageUrl?: string
}

export interface SearchResult {
  books: Book[]
  query: string
  page: number
  pageSize: number
  totalResults: number
}
