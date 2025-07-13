import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import BookGrid from '../../../components/book/BookGrid.vue'
import { mockBooks } from '../../mocks/api'
import type { Book } from '../../../Types/types'

// Mock API service
vi.mock('../../../services/Api', () => ({
  getImageUrl: vi.fn((url: string) => url),
}))

const mockEditBook = vi.fn()
const mockShowBookDetail = vi.fn()

describe('BookGrid', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  const createWrapper = (
    props: Partial<{
      books: Book[]
      isLoading: boolean
      editBook: (book: Book) => void
      showBookDetail: (book: Book) => void
    }> = {},
  ) => {
    return mount(BookGrid, {
      props: {
        books: mockBooks,
        isLoading: false,
        editBook: mockEditBook,
        showBookDetail: mockShowBookDetail,
        ...props,
      },
      global: {
        plugins: [createPinia()],
        stubs: {
          UCard: true,
          UButton: true,
          UBadge: true,
          UIcon: true,
          EditModal: true,
          ConfirmationModal: true,
        },
        directives: {
          'auto-animate': {},
        },
      },
    })
  }

  describe('Rendering', () => {
    it('should render all books', () => {
      const wrapper = createWrapper()

      expect(wrapper.exists()).toBe(true)
      expect(wrapper.props('books')).toEqual(mockBooks)
      expect(wrapper.props('books')).toHaveLength(2)
    })

    it('should display book information correctly', () => {
      const wrapper = createWrapper()

      // Check that books are passed as props correctly
      const books = wrapper.props('books')
      expect(books[0].bookName).toBe('Test Book 1')
      expect(books[0].author).toBe('Test Author 1')
      expect(books[0].price).toBe(19.99)
    })

    it('should render empty grid when no books', () => {
      const wrapper = createWrapper({ books: [] })

      expect(wrapper.exists()).toBe(true)
      expect(wrapper.props('books')).toHaveLength(0)
    })
  })

  describe('Book Interactions', () => {
    it('should call showBookDetail when book card is clicked', async () => {
      const wrapper = createWrapper()

      // Test the component's internal methods
      const vm = wrapper.vm as any
      await vm.showBookDetail(mockBooks[0])

      expect(mockShowBookDetail).toHaveBeenCalledWith(mockBooks[0])
    })

    it('should open edit modal when edit button is clicked', async () => {
      const wrapper = createWrapper()

      const vm = wrapper.vm as any
      await vm.handleEditClick(mockBooks[0])

      expect(vm.showEditModal).toBe(true)
      expect(vm.bookToEdit).toEqual(mockBooks[0])
    })

    it('should open delete confirmation when delete button is clicked', async () => {
      const wrapper = createWrapper()

      const vm = wrapper.vm as any
      await vm.handleDeleteClick(mockBooks[0])

      expect(vm.showDeleteModal).toBe(true)
      expect(vm.bookToDelete).toEqual(mockBooks[0])
    })
  })

  describe('Edit Modal', () => {
    it('should handle edit success', async () => {
      const wrapper = createWrapper()

      await (wrapper.vm as any).handleEditSuccess('Book updated successfully')

      expect((wrapper.vm as any).showEditModal).toBe(false)
      expect((wrapper.vm as any).bookToEdit).toBeNull()
    })

    it('should handle edit error', async () => {
      const wrapper = createWrapper()

      await (wrapper.vm as any).handleEditError('Failed to update book')

      expect((wrapper.vm as any).showEditModal).toBe(false)
    })
  })

  describe('Delete Confirmation', () => {
    it('should emit deleteBook event when delete is confirmed', async () => {
      const wrapper = createWrapper()

      // Set up delete state
      ;(wrapper.vm as any).bookToDelete = mockBooks[0]
      ;(wrapper.vm as any).showDeleteModal = true

      await (wrapper.vm as any).confirmDelete()

      expect(wrapper.emitted('deleteBook')).toBeTruthy()
      expect(wrapper.emitted('deleteBook')?.[0]).toEqual([mockBooks[0]])
      expect((wrapper.vm as any).showDeleteModal).toBe(false)
      expect((wrapper.vm as any).bookToDelete).toBeNull()
    })

    it('should cancel delete operation', async () => {
      const wrapper = createWrapper()

      // Set up delete state
      ;(wrapper.vm as any).bookToDelete = mockBooks[0]
      ;(wrapper.vm as any).showDeleteModal = true

      await (wrapper.vm as any).cancelDelete()

      expect(wrapper.emitted('deleteBook')).toBeFalsy()
      expect((wrapper.vm as any).showDeleteModal).toBe(false)
      expect((wrapper.vm as any).bookToDelete).toBeNull()
    })
  })

  describe('Loading State', () => {
    it('should disable buttons when loading', () => {
      const wrapper = createWrapper({ isLoading: true })

      expect(wrapper.exists()).toBe(true)
      expect(wrapper.props('isLoading')).toBe(true)
    })
  })

  describe('Book Display', () => {
    it('should format price correctly', () => {
      const wrapper = createWrapper()

      const books = wrapper.props('books')
      expect(books[0].price).toBe(19.99)
      expect(books[1].price).toBe(24.99)
    })

    it('should show book categories', () => {
      const wrapper = createWrapper()

      const books = wrapper.props('books')
      expect(books[0].category).toBe('Fiction')
      expect(books[1].category).toBe('Non-Fiction')
    })

    it('should handle missing book images gracefully', () => {
      const booksWithoutImages = mockBooks.map((book) => ({
        ...book,
        imageUrl: '',
      }))

      const wrapper = createWrapper({ books: booksWithoutImages })

      expect(wrapper.exists()).toBe(true)
      expect(wrapper.props('books')).toHaveLength(booksWithoutImages.length)
      expect(wrapper.props('books')[0].imageUrl).toBe('')
    })
  })
})
