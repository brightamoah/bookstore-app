<template>
  <div class="min-h-screen">
    <BookHeader
      :is-loading
      @refresh-books="retryLoadBooks"
      @submit="handleBookSubmitted"
      @success="handleSuccess"
      @error="handleError"
    />

    <div class="mx-auto px-4 sm:px-6 lg:px-8 py-8 max-w-7xl">
      <div v-if="!isLoading && !errorMessage" class="mb-6">
        <Pagination
          :current-page-first="currentPageFirst"
          :current-page-last="currentPageLast"
          :total-books="totalBooks"
          :page-size="pageSize"
          :is-loading="isLoading"
          :page-size-options="pageSizeOptions"
          @update:page-size="onPageSizeChange"
        />
      </div>

      <LoadingState v-if="isLoading" />

      <ErrorState
        v-else-if="errorMessage"
        :error-message="errorMessage"
        @retry="retryLoadBooks"
        @clear-error="clearError"
      />

      <div v-else-if="paginatedBooks.length > 0">
        <BookGrid
          :edit-book="editBook"
          :show-book-detail="showBookDetail"
          :is-loading
          :books="paginatedBooks"
          :delete-book="handleDeleteBook"
          @delete-book="handleDeleteBook"
        />

        <PaginationControls
          :current-page="currentPage"
          :page-count="pageCount"
          :total-books="totalBooks"
          :can-prev="canPrev"
          :can-next="canNext"
          :visible-pages="visiblePages"
          @first="first"
          @prev="prev"
          @next="next"
          @last="last"
          @go-to-page="goToPage"
        />
      </div>

      <EmptyState @fetch-books="loadBooks" v-else />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useBookStore } from '@/stores/BookStore'
import { useAuthStore } from '@/stores/AuthStore'
import type { Book } from '@/Types/types'
import { storeToRefs } from 'pinia'
import { ref, onMounted, computed, watch, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { useOffsetPagination } from '@vueuse/core'

const router = useRouter()

const bookstore = useBookStore()
const authStore = useAuthStore()
const { user, isLoggedIn } = storeToRefs(authStore)
const { books, isLoading, errorMessage, totalBooks, bookFormData, booksLoaded } =
  storeToRefs(bookstore)
const { getAllBooks, clearBookFormdata, deleteBook, clearError, refreshBooks } = bookstore

const toast = useToast()

const pageSize = ref(6)
const pageSizeOptions = [
  { label: '6', value: 6 },
  { label: '12', value: 12 },
  { label: '24', value: 24 },
  { label: '48', value: 48 },
]

const { currentPage, currentPageSize, pageCount, isFirstPage, isLastPage, prev, next } =
  useOffsetPagination({
    total: computed(() => books.value.length),
    page: 1,
    pageSize: pageSize,
  })

const paginatedBooks = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return books.value.slice(start, end)
})

const currentPageFirst = computed(() => {
  if (books.value.length === 0) return 0
  return (currentPage.value - 1) * pageSize.value + 1
})

const currentPageLast = computed(() => {
  if (books.value.length === 0) return 0
  return Math.min(currentPage.value * pageSize.value, books.value.length)
})

const canPrev = computed(() => !isFirstPage.value)
const canNext = computed(() => !isLastPage.value)

const visiblePages = computed(() => {
  const pages: (number | string)[] = []
  const totalPages = pageCount.value
  const current = currentPage.value

  if (current > 3) pages.push(1)
  if (current > 4) pages.push('...')

  for (let i = Math.max(1, current - 2); i <= Math.min(totalPages, current + 2); i++) {
    if (!pages.includes(i)) pages.push(i)
  }
  if (current < totalPages - 3) pages.push('...')
  if (current < totalPages - 2 && totalPages > 1) pages.push(totalPages)

  return pages.filter((page) => page !== '...' || pages.indexOf(page) === pages.lastIndexOf(page))
})

const goToPage = (page: number) => {
  if (page >= 1 && page <= pageCount.value) {
    currentPage.value = page
  }
}

const first = () => {
  currentPage.value = 1
}

const last = () => {
  currentPage.value = pageCount.value
}

const onPageSizeChange = (newPageSize: number) => {
  pageSize.value = newPageSize
  currentPage.value = 1
}

// Watch for page changes to scroll to top
watch(currentPage, () => {
  window.scrollTo({ top: 0, behavior: 'smooth' })
})

const loadBooks = async () => {
  if (!isLoggedIn.value && !user.value) {
    router.push({
      name: 'login',
      query: {
        redirect: '/books',
        message: 'Please log in to view books',
      },
    })
    return
  }
  if (books.value.length > 0 || isLoading.value) {
    console.log('Books already loaded or loading, skipping duplicate call')
    booksLoaded.value = true
    return
  }

  try {
    clearError()
    await getAllBooks()
    booksLoaded.value = true
  } catch (error: any) {
    console.error('Failed to load books:', error)
    booksLoaded.value = false
    if (error.response?.status === 401) {
      toast.add({
        title: 'Authentication Required',
        description: 'Please log in to view books',
        color: 'error',
        icon: 'i-heroicons-exclamation-triangle-solid',
      })

      router.push({
        name: 'login',
        query: {
          redirect: '/books',
          message: 'Please log in to view books',
        },
      })
    }
  }
}

const retryLoadBooks = async () => {
  if (!isLoggedIn.value && !user.value) {
    router.push({
      name: 'login',
      query: {
        redirect: '/books',
        message: 'Please log in to view books',
      },
    })
    return
  }
  try {
    booksLoaded.value = false
    clearError()
    await refreshBooks()
    booksLoaded.value = true
  } catch (error: any) {
    console.error('Failed to retry loading books:', error)
    booksLoaded.value = false

    if (error.response?.status === 401) {
      toast.add({
        title: 'Authentication Required',
        description: 'Please log in to view books',
        color: 'error',
      })

      router.push({
        name: 'login',
        query: {
          redirect: '/books',
          message: 'Session expired. Please log in again.',
        },
      })
    }
  }
}

onMounted(async () => {
  await nextTick()

  if (books.value.length === 0 && !booksLoaded.value) {
    await loadBooks()
  } else {
    booksLoaded.value = true
    console.log('Books already loaded, skipping fetch')
  }
})

const handleBookSubmitted = (book: Omit<Book, 'id'>) => {
  // Book submission handled
}

const handleSuccess = (message: string) => {
  toast.add({
    title: 'Success',
    description: message,
    color: 'success',
  })
  booksLoaded.value = false
  currentPage.value = 1
  // showBookForm.value = false
  // retryLoadBooks()
}

const handleError = (message: string) => {
  toast.add({
    title: 'Error',
    description: message,
    color: 'error',
  })
}

const editBook = (book: Book) => {
  isEditing.value = true
  showBookDetails.value = false
  Object.assign(bookFormData, book)
  showBookForm.value = true
}

// const deleteBook = (book: Book) => {
//   bookToDelete.value = book
//   showDeleteModal.value = true
// }

// Form and modal states
const showBookForm = ref(false)
const showBookDetails = ref(false)
const showDeleteModal = ref(false)
const isEditing = ref(false)
const saving = ref(false)
const deleting = ref(false)
const selectedBook = ref<Book | null>(null)
const bookToDelete = ref<Book | null>(null)

const handleDeleteBook = async (book: Book) => {
  try {
    await bookstore.deleteBook(book.bookId)
    toast.add({
      title: 'Success',
      description: `"${book.bookName}" has been deleted successfully`,
      color: 'success',
    })
  } catch (error) {
    toast.add({
      title: 'Error',
      description: 'Failed to delete book',
      color: 'error',
    })
  }
}

// Methods
const openAddModal = () => {
  isEditing.value = false
  clearBookFormdata()
  showBookForm.value = true
}

const closeBookForm = () => {
  showBookForm.value = false
  clearBookFormdata()
}

const showBookDetail = (book: Book) => {
  const bookIdentifier = book.bookId || book.id
  if (!book || !bookIdentifier) {
    console.error('Book or book.id is missing:', book)
    toast.add({
      title: 'Error',
      description: 'Cannot view book details: Invalid book data',
      color: 'error',
    })
    return
  }
  try {
    selectedBook.value = book
    // Use bookId instead of id if that's what your API expects
    router.push({
      name: 'book-details',
      params: {
        id: bookIdentifier,
      },
    })
  } catch (error) {
    console.error('Navigation error:', error)
    toast.add({
      title: 'Error',
      description: 'Failed to navigate to book details',
      color: 'error',
    })
  }
}
</script>
