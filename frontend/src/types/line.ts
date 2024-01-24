import type { Box } from "./boundingBox";
import type { Word } from "./word";

export interface Line {
    boundingBox: Box,
    text: string | null,
    words: Word[]
}

export type lineType = { [Property in keyof Partial<Line>]: number | string | Box }
