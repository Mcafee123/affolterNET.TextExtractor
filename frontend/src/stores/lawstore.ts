import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { Doc } from '@/types/doc'
import type { Page } from '@/types/page'

export const useLawStore = defineStore('laws', () => {
  const laws = ref<Doc[]>([])
  const pages = ref<Page[]>([])

  return { laws, pages }
})