import api from '@/services/Api'
import type { Book, ValidationErrors } from '@/Types/types'
import {
  validateAuthor,
  validateBookName,
  validateCategory,
  validateDescription,
  validateForm,
  validateImageUrl,
  validatePrice,
} from '@/utils/validations'
import type { FormError } from '@nuxt/ui'
import { defineStore, acceptHMRUpdate } from 'pinia'
import { computed, reactive, ref } from 'vue'

export const useBookStore = defineStore(
  'BookStore',
  () => {
    const books = ref<Book[]>([])
    const book = ref<Book | null>(null)
    const isLoading = ref<boolean>(false)
    const errorMessage = ref<string | null>(null)
    const validationErrors = ref<ValidationErrors>({})
    const imagePreview = ref<string>('')
    const bookFormData = reactive<Omit<Book, 'bookId'> & { bookId?: number }>({
      bookName: '',
      category: 'Fiction',
      author: '',
      price: 0,
      description: '',
      imageUrl: '',
      createdAt: new Date().toISOString(),
      updatedAt: null,
    })

    const totalBooks = computed(() => books.value.length)

    const booksByCategory = computed(() => {
      return books.value.reduce(
        (acc, book) => {
          acc[book.category] = (acc[book.category] || 0) + 1
          return acc
        },
        {} as Record<string, number>,
      )
    })

    const validate = (state: typeof bookFormData): FormError[] => {
      return validateForm(state)
    }

    const isFormValid = computed(() => {
      const errors = validate(bookFormData)
      return errors.length === 0
    })

    const handleFileChange = (event: Event) => {
      const input = event.target as HTMLInputElement
      const file = input.files?.[0]

      if (file) {
        const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp']
        if (!allowedTypes.includes(file.type)) {
          console.error('Invalid file type')
          return
        }

        if (file.size > 5 * 1024 * 1024) {
          console.error('File too large')
          return
        }

        imagePreview.value = URL.createObjectURL(file)

        const reader = new FileReader()
        reader.onload = (e) => {
          bookFormData.imageUrl = e.target?.result as string
        }
        reader.readAsDataURL(file)
      }
    }

    const getAllBooks = async () => {
      isLoading.value = true
      errorMessage.value = null

      try {
        const response = await api.get('/api/books')
        books.value = response.data
        console.log('Books fetched successfully')
        return response.data
      } catch (err: any) {
        errorMessage.value = err.response?.data?.message || 'Failed to fetch books'
        throw err
      } finally {
        isLoading.value = false
      }
    }

    const getBookById = async (id: number) => {
      isLoading.value = true
      errorMessage.value = null

      try {
        const response = await api.get(`/api/books/${id}`)
        book.value = response.data
        console.log(`Book with id ${id} fetched successfully`)
        return response.data
      } catch (err: any) {
        errorMessage.value = err.response?.data?.message || 'Failed to fetch book'
        throw err
      } finally {
        isLoading.value = false
      }
    }

    const createBook = async (newBook: Omit<Book, 'id'>) => {
      const errors = validate(bookFormData)
      if (errors.length > 0) {
        throw new Error('Form validation failed' + errors.map((e) => e.message).join(', '))
      }

      errorMessage.value = null

      try {
        const response = await api.post('/api/books', bookFormData)
        const newBook = response.data
        books.value.push(newBook)
        console.log('Book created successfully', newBook)
        return newBook
      } catch (err: any) {
        errorMessage.value = err.response?.data?.message || 'Failed to create book'
        throw err
      }
    }

    const updateBook = async (id: Book['bookId'], bookData: Partial<Book>) => {
      isLoading.value = true
      errorMessage.value = null

      try {
        const updateData = {
          bookId: id,
          bookName: bookData.bookName,
          category: bookData.category,
          author: bookData.author,
          price: Number(bookData.price),
          description: bookData.description,
          imageUrl: bookData.imageUrl || '',
          createdAt: bookData.createdAt,
          updatedAt: new Date().toISOString(),
        }
        console.log('Sending update data:', updateData)

        const response = await api.put(`/api/books/${id}`, updateData)
        const updatedBook = response.data

        const index = books.value.findIndex((book) => book.bookId === id)
        if (index !== -1) {
          books.value[index] = updatedBook
        }
        console.log(`Book with id ${id} updated successfully`)

        return updatedBook
      } catch (err: any) {
        errorMessage.value = err.response?.data?.message || 'Failed to update book'
        throw err
      } finally {
        isLoading.value = false
      }
    }

    const deleteBook = async (id: Book['bookId']) => {
      isLoading.value = true
      errorMessage.value = null

      try {
        await api.delete(`/api/books/${id}`)
        books.value = books.value.filter((book) => book.bookId !== id)
        console.log(`Book with id ${id} deleted successfully`)
      } catch (err: any) {
        errorMessage.value = err.response?.data?.message || 'Failed to delete book'
        throw err
      } finally {
        isLoading.value = false
      }
    }

    const refreshBooks = async () => {
      books.value = []
      await getAllBooks()
    }

    const clearBookFormdata = () => {
      bookFormData.bookName = ''
      bookFormData.category = 'Fiction'
      bookFormData.author = ''
      bookFormData.price = 0
      bookFormData.description = ''
      bookFormData.imageUrl = ''
      bookFormData.createdAt = new Date().toISOString()
      bookFormData.updatedAt = null
      if ('bookId' in bookFormData) {
        delete (bookFormData as any).bookId
      }
    }

    const clearError = () => {
      errorMessage.value = null
    }

    const clearImage = () => {
      if (imagePreview.value) {
        URL.revokeObjectURL(imagePreview.value)
      }
      imagePreview.value = ''
      bookFormData.imageUrl = ''
    }

    const clearBookState = () => {
      books.value = []
      book.value = null
      isLoading.value = false
      errorMessage.value = null
      validationErrors.value = {}
      imagePreview.value = ''
      clearBookFormdata()
      clearError()
      clearImage()
    }

    return {
      books,
      book,
      isLoading,
      errorMessage,
      totalBooks,
      booksByCategory,
      bookFormData,
      imagePreview,
      isFormValid,
      validationErrors,
      handleFileChange,
      getAllBooks,
      getBookById,
      createBook,
      updateBook,
      deleteBook,
      clearError,
      clearBookFormdata,
      clearImage,
      validate,
      clearBookState,
      refreshBooks,
    }
  },
  {
    persist: true,
  },
)

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useBookStore, import.meta.hot))
}
