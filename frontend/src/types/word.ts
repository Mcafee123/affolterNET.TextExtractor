import type { Box } from "./boundingBox"

export interface Word {
    boundingBox: Box,
    text: string,
    pageNr: number,
}