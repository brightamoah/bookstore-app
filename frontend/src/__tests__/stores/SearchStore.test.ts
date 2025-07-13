import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useSearchStore } from '../../stores/SearchStore'
import { mockBooks } from '../mocks/api'

// Mock the API service
vi.mock('../../services/Api', () => ({
  default: {
    get: vi.fn(),
  },
}))

import Api from '../../services/Api'
const mockApi = Api as any

describe('SearchStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Initial State', () => {
    it('should have correct initial state', () => {
      const searchStore = useSearchStore()

      expect(searchStore.searchQuery).toBe('')
      expect(searchStore.searchResults).toEqual([])
      expect(searchStore.isSearching).toBe(false)
      expect(searchStore.searchError).toBeNull()
    })
  })

  describe('Computed Properties', () => {
    it('should compute isEmpty correctly', () => {
      const searchStore = useSearchStore()

      // Initially no query, so isEmpty should be false
      expect(searchStore.isEmpty).toBe(false)

      // With query but no results, isEmpty should be true
      searchStore.searchQuery = 'test'
      expect(searchStore.isEmpty).toBe(true)

      searchStore.searchResults = mockBooks
      // With query and results, isEmpty should be false
      expect(searchStore.isEmpty).toBe(false)
    })

    it('should compute hasQuery correctly', () => {
      const searchStore = useSearchStore()

      expect(searchStore.hasQuery).toBe(false)

      searchStore.searchQuery = 'test'
      expect(searchStore.hasQuery).toBe(true)
    })

    it('should compute hasResults correctly', () => {
      const searchStore = useSearchStore()

      expect(searchStore.hasResults).toBe(false)

      searchStore.searchResults = mockBooks
      expect(searchStore.hasResults).toBe(true)
    })

    it('should compute cacheSize correctly', () => {
      const searchStore = useSearchStore()

      expect(searchStore.cacheSize).toBe(0)

      // Simulate cached search results
      searchStore.searchCache = {
        test: { books: mockBooks, page: 1, pageSize: 10, query: 'test', totalResults: 2 },
        book: { books: [mockBooks[0]], page: 1, pageSize: 10, query: 'book', totalResults: 1 },
      }
      expect(searchStore.cacheSize).toBe(2)
    })
  })

  describe('Actions', () => {
    describe('performSearch', () => {
      it('should search books successfully', async () => {
        const searchStore = useSearchStore()

        // Mock successful API response with SearchResult structure
        const mockSearchResult = {
          books: mockBooks,
          page: 1,
          pageSize: 20,
          query: 'test',
          totalResults: 2,
        }
        mockApi.get.mockResolvedValueOnce({ data: mockSearchResult })

        await searchStore.performSearch('test')

        expect(mockApi.get).toHaveBeenCalledWith('/api/books/search', {
          params: {
            query: 'test',
            page: 1,
            pageSize: 20,
          },
        })
        expect(searchStore.searchResults).toHaveLength(2)
        expect(searchStore.searchResults[0]).toEqual(mockBooks[0])
        expect(searchStore.isSearching).toBe(false)
        expect(searchStore.searchError).toBeNull()
      })

      it('should handle empty search query', async () => {
        const searchStore = useSearchStore()

        await searchStore.performSearch('')

        expect(searchStore.searchResults).toEqual([])
        expect(searchStore.isSearching).toBe(false)
      })

      it('should handle search failure', async () => {
        const searchStore = useSearchStore()

        // Mock API error
        mockApi.get.mockRejectedValueOnce(new Error('Search failed'))

        await searchStore.performSearch('error')

        expect(searchStore.searchError).toBeTruthy()
        expect(searchStore.searchResults).toEqual([])
        expect(searchStore.isSearching).toBe(false)
      })

      it('should use cached results', async () => {
        const searchStore = useSearchStore()

        // Mock successful API response for first search
        const mockSearchResult = {
          books: mockBooks,
          page: 1,
          pageSize: 20,
          query: 'test',
          totalResults: 2,
        }
        mockApi.get.mockResolvedValueOnce({ data: mockSearchResult })

        // First search to populate cache
        await searchStore.performSearch('test')
        expect(searchStore.searchResults).toHaveLength(2)

        // Reset the mock calls counter
        mockApi.get.mockClear()

        // Search again with same query - should use cache
        await searchStore.performSearch('test')

        // Should get cached results without making another API call
        expect(searchStore.searchResults).toHaveLength(2)
        expect(mockApi.get).not.toHaveBeenCalled() // No additional API calls due to caching
      })
    })

    describe('debouncedSearch', () => {
      it('should have debounced search function', () => {
        const searchStore = useSearchStore()

        // Check that the debounced search function exists
        expect(typeof searchStore.debouncedSearch).toBe('function')

        // Test that calling it doesn't throw an error
        expect(() => searchStore.debouncedSearch('test')).not.toThrow()
      })
    })

    describe('clearSearch', () => {
      it('should clear search state', () => {
        const searchStore = useSearchStore()

        // Set some search state
        searchStore.searchQuery = 'test'
        searchStore.searchResults = mockBooks
        searchStore.searchError = 'Some error'

        searchStore.clearSearch()

        expect(searchStore.searchQuery).toBe('')
        expect(searchStore.searchResults).toEqual([])
        expect(searchStore.searchError).toBeNull()
      })
    })

    describe('clearCache', () => {
      it('should clear search cache', () => {
        const searchStore = useSearchStore()

        // Set some cache
        searchStore.searchCache = {
          test: { books: mockBooks, page: 1, pageSize: 10, query: 'test', totalResults: 2 },
        }

        searchStore.clearCache()

        expect(searchStore.searchCache).toEqual({})
        expect(searchStore.cacheSize).toBe(0)
      })
    })
  })
})
