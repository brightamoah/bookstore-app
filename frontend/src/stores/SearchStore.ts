import api from '@/services/Api'
import { computed, ref, watch } from 'vue'
import { useDebounceFn } from '@vueuse/core'
import type { SearchResult } from '@/Types/types'
import { defineStore, acceptHMRUpdate } from 'pinia'
import type { AxiosError } from 'axios'

export const useSearchStore = defineStore(
  'SearchStore',
  () => {
    const searchQuery = ref<SearchResult['query']>('')
    const searchResults = ref<SearchResult['books']>([])
    const isSearching = ref<boolean>(false)
    const searchError = ref<string | null>(null)
    const searchCache = ref<Record<string, SearchResult>>({})

    const performSearch = async (query: string) => {
      if (!query.trim()) {
        searchResults.value = []
        isSearching.value = false
        return
      }

      const cacheKey = query.toLowerCase().trim()
      if (searchCache.value[cacheKey]) {
        const cached = searchCache.value[cacheKey]
        searchResults.value = cached.books
        isSearching.value = false
        return
      }
      isSearching.value = true
      searchError.value = null

      try {
        const response = await api.get('/api/books/search', {
          params: {
            query: query.trim(),
            page: 1,
            pageSize: 20,
          },
        })

        const result: SearchResult = response.data
        searchResults.value = result.books
        searchCache.value[cacheKey] = result

        const cacheKeys = Object.keys(searchCache.value)
        if (cacheKeys.length > 50) {
          // Remove oldest entries (first 10)
          const keysToRemove = cacheKeys.slice(0, 10)
          keysToRemove.forEach((key) => {
            delete searchCache.value[key]
          })
        }
      } catch (error: any) {
        console.error('Search error:', error)
        if (error.response?.status === 401) {
          searchError.value = 'Session expired. Please log in again.'
          searchResults.value = []
        } else {
          searchError.value = error.response?.data?.message || 'Search failed'
          searchResults.value = []
        }
      } finally {
        isSearching.value = false
      }
    }

    const debouncedSearch = useDebounceFn(performSearch, 300)

    watch(searchQuery, (newQuery) => {
      if (!newQuery.trim()) {
        searchResults.value = []
        isSearching.value = false
        searchError.value = null
        return
      }
      debouncedSearch(newQuery)
    })

    const clearSearch = () => {
      searchQuery.value = ''
      searchResults.value = []
      searchError.value = null
      isSearching.value = false
    }

    const clearCache = () => {
      searchCache.value = {}
    }
    const hasResults = computed(() => searchResults.value.length > 0)
    const hasQuery = computed(() => searchQuery.value.trim().length > 0)
    const isEmpty = computed(() => hasQuery.value && !hasResults.value && !isSearching.value)

    const searchWithPagination = async (query: string, page: number = 1, pageSize: number = 10) => {
      isSearching.value = true
      searchError.value = null

      try {
        const response = await api.get('/api/books/search', {
          params: { query: query.trim(), page, pageSize },
        })
        return response.data as SearchResult
      } catch (error: any) {
        if (error.response?.status === 401) {
          searchError.value = 'Session expired. Please log in again.'
        } else {
          searchError.value = error.response?.data?.message || 'Search failed'
        }
        throw error
      } finally {
        isSearching.value = false
      }
    }

    const clearSearchState = () => {
      searchQuery.value = ''
      searchResults.value = []
      isSearching.value = false
      searchError.value = null
      searchCache.value = {}
    }

    return {
      searchQuery,
      searchResults,
      isSearching,
      searchError,
      searchCache,
      hasResults,
      hasQuery,
      isEmpty,
      cacheSize: computed(() => Object.keys(searchCache.value).length),
      performSearch,
      debouncedSearch,
      clearSearch,
      clearCache,
      searchWithPagination,
      clearSearchState,
    }
  },
  {
    persist: true,
  },
)

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useSearchStore, import.meta.hot))
}
