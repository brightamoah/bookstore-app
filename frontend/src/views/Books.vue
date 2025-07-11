<!-- pages/dashboard.vue -->
<template>
  <div class="min-h-screen">
    <!-- Header -->
    <div class="shadow-sm border-b">
      <div class="mx-auto px-4 sm:px-6 lg:px-8 max-w-7xl">
        <div class="flex justify-between items-center py-6">
          <div>
            <h1 class="font-bold text-2xl">
              <UIcon name="tabler-books" class="size-10 text-primary" />
              <span>Book Store Dashboard</span>
            </h1>
            <p class="mt-1">Manage your book inventory</p>
          </div>
          <AddModal @submit="handleBookSubmitted" @success="handleSuccess" @error="handleError" />
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="mx-auto px-4 sm:px-6 lg:px-8 py-8 max-w-7xl">
      <!-- Book Details Modal -->

      <!-- Books Grid -->
      <BookGrid :edit-book :delete-book :show-book-detail :books />

      <!-- Empty State -->
      <div v-if="books.length === 0" class="py-12 text-center">
        <div class="mx-auto w-12 h-12 text-gray-400">📚</div>
        <h3 class="mt-4 font-medium text-gray-900 dark:text-white text-sm">No books</h3>
        <p class="mt-1 text-gray-500 dark:text-gray-400 text-sm">
          Get started by adding your first book to the inventory.
        </p>
        <div class="mt-6">
          <UButton @click="openAddModal" color="primary" variant="solid" icon="i-heroicons-plus">
            Add New Book
          </UButton>
        </div>
      </div>
    </div>
    <!-- <UpdateModal v-model="showBookForm" /> -->
  </div>
</template>

<script setup lang="ts">
import { useBookStore } from '@/stores/BookStore'
import type { Book } from '@/Types/types'
import { get } from 'http'
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const bookstore = useBookStore()
const { getAllBooks } = bookstore

const toast = useToast()

const handleBookSubmitted = (book: Omit<Book, 'id'>) => {
  console.log('Book submitted:', book)
}

const handleSuccess = (message: string) => {
  toast.add({
    title: 'Success',
    description: message,
    color: 'success',
  })
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
  Object.assign(bookForm, book)
  showBookForm.value = true
}

const deleteBook = (book: Book) => {
  bookToDelete.value = book
  showDeleteModal.value = true
}

const book = ref<Book>({
  id: 0,
  bookName: '',
  author: '',
  category: '',
  price: 0,
  imageUrl: '',
  description: '',
})
// const books = ref([])
// Reactive data
const books = ref([
  {
    id: 1,
    bookName: 'The Great Gatsby',
    author: 'F. Scott Fitzgerald',
    category: 'Fiction',
    price: 12.99,
    stock: 25,
    publishedYear: 1925,
    imageUrl: 'https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=300&h=400&fit=crop',
    description:
      'A classic American novel set in the Jazz Age, exploring themes of wealth, love, and the American Dream through the eyes of narrator Nick Carraway.',
  },
  {
    id: 2,
    bookName: 'To Kill a Mockingbird',
    author: 'Harper Lee',
    category: 'Fiction',
    price: 14.99,
    stock: 18,
    publishedYear: 1960,
    imageUrl: 'https://images.unsplash.com/photo-1481627834876-b7833e8f5570?w=300&h=400&fit=crop',
    description:
      'A gripping tale of racial injustice and childhood innocence in 1930s Alabama, told through the perspective of young Scout Finch.',
  },
  {
    id: 3,
    bookName: '1984',
    author: 'George Orwell',
    category: 'Dystopian',
    price: 13.99,
    stock: 0,
    publishedYear: 1949,
    imageUrl: 'https://images.unsplash.com/photo-1495640388908-05fa85288e61?w=300&h=400&fit=crop',
    description:
      'A dystopian social science fiction novel that explores the dangers of totalitarianism and extreme political ideology.',
  },
  {
    id: 4,
    bookName: 'Pride and Prejudice',
    author: 'Jane Austen',
    category: 'Romance',
    price: 11.99,
    stock: 32,
    publishedYear: 1813,
    imageUrl: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300&h=400&fit=crop',
    description:
      'A romantic novel that critiques the British landed gentry at the end of the 18th century, focusing on Elizabeth Bennet and Mr. Darcy.',
  },
  {
    id: 5,
    bookName: 'The Catcher in the Rye',
    author: 'J.D. Salinger',
    category: 'Fiction',
    price: 10.99,
    stock: 15,
    publishedYear: 1951,
    imageUrl: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300&h=400&fit=crop',
    description:
      'A novel about teenage angst and alienation, narrated by the iconic character Holden Caulfield, who recounts his experiences in New York City after being expelled from prep school.',
  },
])

// Form and modal states
const showBookForm = ref(false)
const showBookDetails = ref(false)
const showDeleteModal = ref(false)
const isEditing = ref(false)
const saving = ref(false)
const deleting = ref(false)
const selectedBook = ref<Book | null>(null)
const bookToDelete = ref<Book | null>(null)

// Form data
const bookForm = reactive({
  title: '',
  author: '',
  genre: '',
  price: '',
  stock: '',
  publishedYear: '',
  imageUrl: '',
  description: '',
})

// Genre options
const genres = [
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

// Methods
const openAddModal = () => {
  isEditing.value = false
  resetForm()
  showBookForm.value = true
}

const closeBookForm = () => {
  showBookForm.value = false
  resetForm()
}

const resetForm = () => {
  Object.assign(bookForm, {
    title: '',
    author: '',
    genre: '',
    price: '',
    stock: '',
    publishedYear: '',
    cover: '',
    description: '',
  })
}

const showBookDetail = (book: Book) => {
  selectedBook.value = book
  // showBookDetails.value = true
  router.push({ name: 'book-details', params: { id: book.id } })
}
</script>
