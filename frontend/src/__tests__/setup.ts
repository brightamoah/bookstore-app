import { vi } from 'vitest'
import '@testing-library/jest-dom'

// Mock import.meta.env
Object.defineProperty(globalThis, 'import', {
  value: {
    meta: {
      env: {
        VITE_API_BASE_URL: 'http://localhost:8000',
        BASE_URL: '/',
        DEV: true,
        PROD: false,
        SSR: false,
        MODE: 'test',
      },
      hot: undefined,
    },
  },
  writable: true,
})

// Mock console methods to reduce noise in tests
global.console = {
  ...console,
  log: vi.fn(),
  debug: vi.fn(),
  info: vi.fn(),
  warn: vi.fn(),
  error: vi.fn(),
}

// Mock window properties
const mockWindow = {
  scrollTo: vi.fn(),
  location: {
    href: 'http://localhost:3000',
    pathname: '/',
    search: '',
    hash: '',
  },
  URL: {
    createObjectURL: vi.fn(() => 'mock-object-url'),
    revokeObjectURL: vi.fn(),
  },
  matchMedia: vi.fn().mockImplementation((query: string) => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(),
    removeListener: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
}

Object.defineProperty(globalThis, 'window', {
  value: mockWindow,
  writable: true,
})

// Mock ResizeObserver
;(globalThis as any).ResizeObserver = vi.fn().mockImplementation(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn(),
}))

// Mock File API
;(globalThis as any).File = class File {
  name: string
  size: number
  type: string
  lastModified: number

  constructor(chunks: any[], filename: string, options: any = {}) {
    this.name = filename
    this.size = chunks.reduce((acc, chunk) => acc + (chunk.length || 0), 0)
    this.type = options.type || ''
    this.lastModified = Date.now()
  }
}

// Mock FileReader
;(globalThis as any).FileReader = class FileReader {
  onload: ((event: any) => void) | null = null
  onerror: ((event: any) => void) | null = null
  result: string | null = null

  readAsDataURL(_file: any) {
    setTimeout(() => {
      this.result = 'data:image/jpeg;base64,mock-base64-data'
      if (this.onload) {
        this.onload({ target: this })
      }
    }, 0)
  }
}

// Mock DOM Event constructors for Vue Test Utils
Object.assign(globalThis, {
  Event: class Event {
    constructor(type: string, options?: any) {
      this.type = type
      this.bubbles = options?.bubbles ?? false
      this.cancelable = options?.cancelable ?? false
      this.composed = options?.composed ?? false
    }
    type!: string
    bubbles!: boolean
    cancelable!: boolean
    composed!: boolean
    preventDefault() {}
    stopPropagation() {}
  },

  MouseEvent: class MouseEvent extends (globalThis as any).Event {
    constructor(type: string, options?: any) {
      super(type, options)
      this.button = options?.button ?? 0
      this.buttons = options?.buttons ?? 0
      this.clientX = options?.clientX ?? 0
      this.clientY = options?.clientY ?? 0
    }
    button!: number
    buttons!: number
    clientX!: number
    clientY!: number
  },

  KeyboardEvent: class KeyboardEvent extends (globalThis as any).Event {
    constructor(type: string, options?: any) {
      super(type, options)
      this.key = options?.key ?? ''
      this.code = options?.code ?? ''
      this.ctrlKey = options?.ctrlKey ?? false
      this.shiftKey = options?.shiftKey ?? false
      this.altKey = options?.altKey ?? false
      this.metaKey = options?.metaKey ?? false
    }
    key!: string
    code!: string
    ctrlKey!: boolean
    shiftKey!: boolean
    altKey!: boolean
    metaKey!: boolean
  },

  InputEvent: class InputEvent extends (globalThis as any).Event {
    constructor(type: string, options?: any) {
      super(type, options)
      this.data = options?.data ?? null
    }
    data!: string | null
  },

  FocusEvent: class FocusEvent extends (globalThis as any).Event {
    constructor(type: string, options?: any) {
      super(type, options)
      this.relatedTarget = options?.relatedTarget ?? null
    }
    relatedTarget!: EventTarget | null
  },
})

// Mock DOM elements for VueUse compatibility
class MockElement extends EventTarget {
  parentNode: MockElement | null = null
  nextSibling: MockElement | null = null
  childNodes: MockElement[] = []
  textContent = ''
  nodeType = 1
  tagName = 'DIV'

  addEventListener = vi.fn((type: string, listener: any, options?: any) => {
    super.addEventListener(type, listener, options)
  })
  removeEventListener = vi.fn((type: string, listener: any, options?: any) => {
    super.removeEventListener(type, listener, options)
  })
  dispatchEvent = vi.fn((event: Event) => {
    return super.dispatchEvent(event)
  })
  getBoundingClientRect = vi.fn(() => ({
    width: 0,
    height: 0,
    top: 0,
    left: 0,
    bottom: 0,
    right: 0,
    x: 0,
    y: 0,
  }))
  querySelector = vi.fn()
  querySelectorAll = vi.fn(() => [])
  contains = vi.fn(() => false)
  style = {}
  classList = {
    add: vi.fn(),
    remove: vi.fn(),
    contains: vi.fn(() => false),
    toggle: vi.fn(),
  }
  setAttribute = vi.fn()
  getAttribute = vi.fn()
  removeAttribute = vi.fn()
  scrollIntoView = vi.fn()
  focus = vi.fn()
  blur = vi.fn()
  click = vi.fn()

  // Enhanced DOM manipulation methods for Vue compatibility
  insertBefore = vi.fn((newNode: MockElement, referenceNode: MockElement | null) => {
    if (!newNode) return newNode

    // Set parent relationship
    newNode.parentNode = this

    if (!referenceNode) {
      // If no reference node, append to end
      this.childNodes.push(newNode)
    } else {
      // Insert before reference node
      const index = this.childNodes.indexOf(referenceNode)
      if (index >= 0) {
        this.childNodes.splice(index, 0, newNode)
      } else {
        this.childNodes.push(newNode)
      }
    }

    return newNode
  })

  appendChild = vi.fn((node: MockElement) => {
    if (!node) return node

    // Set parent relationship
    node.parentNode = this

    // Add to children
    this.childNodes.push(node)

    return node
  })

  removeChild = vi.fn((node: MockElement) => {
    if (!node) return node

    // Remove parent relationship
    node.parentNode = null

    // Remove from children
    const index = this.childNodes.indexOf(node)
    if (index >= 0) {
      this.childNodes.splice(index, 1)
    }

    return node
  })
}

// Mock VueUse composables to avoid DOM compatibility issues
vi.mock('@vueuse/core', async () => {
  const actual = await vi.importActual('@vueuse/core')
  return {
    ...actual,
    useEventListener: vi.fn(() => {
      // Mock implementation that doesn't require real DOM events
      return {
        stop: vi.fn(),
      }
    }),
    useDateFormat: vi.fn((_date: any, _format: string) => ({
      value: 'Jan 01, 2024', // Mock formatted date
    })),
  }
})

// Mock Element prototype to ensure all elements have addEventListener
if (typeof (globalThis as any).Element !== 'undefined') {
  ;(globalThis as any).Element.prototype.addEventListener =
    (globalThis as any).Element.prototype.addEventListener || vi.fn()
  ;(globalThis as any).Element.prototype.removeEventListener =
    (globalThis as any).Element.prototype.removeEventListener || vi.fn()
}

// Global element creation mock to ensure VueUse compatibility
const originalCreateElement = (globalThis as any).document?.createElement
if (originalCreateElement) {
  ;(globalThis as any).document.createElement = vi.fn((tagName: string) => {
    const element = new MockElement()
    Object.assign(element, {
      tagName: tagName.toUpperCase(),
      // Ensure VueUse compatibility
      addEventListener: vi.fn((type: string, listener: any, _options?: any) => {
        if (typeof listener === 'function') {
          // Mock the registration
        }
      }),
      removeEventListener: vi.fn(),
    })
    return element
  })
}

// Mock document with enhanced DOM tree support
const mockDocumentElement = new MockElement()
mockDocumentElement.tagName = 'HTML'

const mockBody = new MockElement()
mockBody.tagName = 'BODY'
mockDocumentElement.appendChild(mockBody)

const mockHead = new MockElement()
mockHead.tagName = 'HEAD'
mockDocumentElement.appendChild(mockHead)

Object.defineProperty(globalThis, 'document', {
  value: {
    createElement: vi.fn((tagName: string) => {
      const element = new MockElement()
      element.tagName = tagName.toUpperCase()
      return element
    }),
    createTextNode: vi.fn((text: string) => {
      const node = new MockElement()
      Object.assign(node, {
        nodeType: 3,
        textContent: text,
        data: text,
        tagName: '#text',
      })
      return node
    }),
    createComment: vi.fn((text: string) => {
      const node = new MockElement()
      Object.assign(node, {
        nodeType: 8,
        textContent: text,
        data: text,
        tagName: '#comment',
      })
      return node
    }),
    createDocumentFragment: vi.fn(() => {
      const fragment = new MockElement()
      Object.assign(fragment, {
        nodeType: 11,
        tagName: '#document-fragment',
      })
      return fragment
    }),
    getElementById: vi.fn(() => new MockElement()),
    querySelector: vi.fn(() => new MockElement()),
    querySelectorAll: vi.fn(() => []),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    body: mockBody,
    documentElement: mockDocumentElement,
    head: mockHead,
  },
  writable: true,
})
