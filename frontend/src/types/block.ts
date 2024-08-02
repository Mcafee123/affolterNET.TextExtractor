import type { Box } from "./boundingBox";
import type { Line } from "./line";
import type { Word } from "./word";

export interface Block {
    id: number,
    boundingBox: Box,
    text: string,
    lines: Line[],
    words: Word[]
}

export type blockType = { [Property in keyof Partial<Block>]: string | Box }
