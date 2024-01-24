import type { Box } from "./boundingBox"
import type { Letter } from "./letter";

export interface Word {
    boundingBox: Box,
    text: string,
    pageNr: number,
    fontName: string,
    letters: Letter[],
}

export type wordProp = keyof Word
export type wordType = { prop: wordProp, val: Word[wordProp] }
