import type { Box } from "./boundingBox";
import type { Word } from "./word";

export interface Line {
    boundingBox: Box,
    text: string | null,
    words: Word[]
}

export type lineProp = keyof Line
export type lineType = { prop: lineProp, val: Line[lineProp] }
