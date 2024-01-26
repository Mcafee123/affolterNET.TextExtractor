import { ref, watch } from 'vue'
import { useStorage } from './useStorage'
import type { blockType } from '@/types/block'
import type { lineType } from '@/types/line'
import type { wordType } from '@/types/word'
import type { letterType } from '@/types/letter'

const { getFromStorage, setToStorage } = useStorage()
const showLetterBorders = ref(getFromStorage('showLetterBorders', 'false') === 'true')
const showWordBorders = ref(getFromStorage('showWordBorders', 'false') === 'true')
const showLineBorders = ref(getFromStorage('showLineBorders', 'false') === 'true')
const showBlockBorders = ref(getFromStorage('showBlockBorders', 'false') === 'true')

const blockJson = ref<blockType | null>(null)
const lineJson = ref<lineType | null>(null)
const wordJson = ref<wordType | null>(null)
const letterJson = ref<letterType | null>(null)

export function useViewSettings() {

  // showLetterBorders
  watch(showLetterBorders, (value: boolean) => {
    setToStorage('showLetterBorders', value ? 'true' : 'false')
  })

  // showWordBorders
  watch(showWordBorders, (value: boolean) => {
    setToStorage('showWordBorders', value ? 'true' : 'false')
  })

  // showLineBorders
  watch(showLineBorders, (value: boolean) => {
    setToStorage('showLineBorders', value ? 'true' : 'false')
  })

  // showBlockBorders
  watch(showBlockBorders, (value: boolean) => {
    setToStorage('showBlockBorders', value ? 'true' : 'false')
  })

  // rgb from hex color
  const hexToRgb = (cssvar: string) => {
    const hex = document.body.style.getPropertyValue(cssvar)
    const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex)
    return result ? {
      r: parseInt(result[1], 16),
      g: parseInt(result[2], 16),
      b: parseInt(result[3], 16)
    } : null;
  }

  return { showLetterBorders, showWordBorders, showLineBorders, showBlockBorders, blockJson, lineJson, wordJson, letterJson, hexToRgb }
}