import { ref, watch } from 'vue'
import { useStorage } from './useStorage'
import type { blockType } from '@/types/block'
import type { lineType } from '@/types/line'
import type { wordType } from '@/types/word'
import type { letterType } from '@/types/letter'
import type { Box } from '@/types/boundingBox'

const { getFromStorage, setToStorage } = useStorage()
const showLetterBorders = ref(getFromStorage('showLetterBorders', 'false') === 'true')
const showWordBorders = ref(getFromStorage('showWordBorders', 'false') === 'true')
const showLineBorders = ref(getFromStorage('showLineBorders', 'false') === 'true')
const showBlockBorders = ref(getFromStorage('showBlockBorders', 'false') === 'true')
const showFootnotes = ref(getFromStorage('showFootnotes', 'false') === 'true')
const showPageNumbers = ref(getFromStorage('showPageNumbers', 'false') === 'true')

const blockJson = ref<blockType | null>(null)
const lineJson = ref<lineType | null>(null)
const wordJson = ref<wordType | null>(null)
const letterJson = ref<letterType | null>(null)

const defaultCustomBox = {
  bottomLeftX: 0,
  bottomLeftY: 0,
  topRightX: 0,
  topRightY: 0,
  color: 'violet',
  shadow: true
}

const customBox = ref<customBoxType>(defaultCustomBox)

export type customBoxType = Box & { color: string, shadow: boolean }


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

  // showFootnotes
  watch(showFootnotes, (value: boolean) => {
    setToStorage('showFootnotes', value ? 'true' : 'false')
  })

  // showPageNumbers
  watch(showPageNumbers, (value: boolean) => {
    setToStorage('showPageNumbers', value ? 'true' : 'false')
  })

  const getCssVar = (cssvar: string, element: HTMLElement = document.body) => {
    const val = element.style.getPropertyValue(cssvar)
    return val
  }

  // rgb from hex color
  const hexToRgb = (cssvar: string) => {
    const hex = getCssVar(cssvar)
    const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex)
    return result ? {
      r: parseInt(result[1], 16),
      g: parseInt(result[2], 16),
      b: parseInt(result[3], 16)
    } : null;
  }

  const clearCustomBox = (): void => {
    customBox.value = defaultCustomBox
  }

  const customBoxIsSet = (): boolean => {
    const bx = customBox.value
    if (!bx.bottomLeftX || !bx.bottomLeftY || !bx.topRightX || !bx.topRightY) {
      return false
    }
    return bx.bottomLeftX > 0 && bx.bottomLeftY > 0 && bx.topRightX > 0 && bx.topRightY > 0
  }

  const round = (inp: number): number => Math.round(inp * 100) / 100

  return { 
    showLetterBorders, 
    showWordBorders, 
    showLineBorders, 
    showBlockBorders,
    showFootnotes,
    showPageNumbers,
    blockJson, 
    lineJson, 
    wordJson, 
    letterJson, 
    hexToRgb, 
    getCssVar,
    customBox,
    clearCustomBox,
    customBoxIsSet,
    round
  }
}
