import type { Box } from "./boundingBox"
import type { Letter } from "./letter";

export interface Word {
    id: number,
    boundingBox: Box,
    baseLineY: number,
    text: string,
    pageNr: number,
    fontName: string,
    orientation: string,
    letters: Letter[],
}

export type wordType = { [Property in keyof Partial<Word>]: number | string | Box }
