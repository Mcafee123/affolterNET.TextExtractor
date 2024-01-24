import type { Box } from "./boundingBox";
import type { Line } from "./line";

export interface Block {
    boundingBox: Box,
    text: string,
    lines: Line[]
}

export type blockType = { [Property in keyof Partial<Block>]: string | Box }
