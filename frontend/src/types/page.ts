import type { Block } from "./block"
import type { Box } from "./boundingBox"
import type { ImageBlock } from "./image"

export interface Page {
    nr: number,
    pageNumberBlockId: number | null,
    headerBlockIds: number[],
    boundingBox: Box,
    blocks: Block[],
    imageBlocks: ImageBlock[],
}