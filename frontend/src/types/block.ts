import type { Box } from "./boundingBox";
import type { Line } from "./line";

export interface Block {
    boundingBox: Box,
    lines: Line[]
}

export type blockProp = keyof Block
export type blockType = { prop: blockProp, val: Block[blockProp] }