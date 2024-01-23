import type { Block } from "./block";
import type { Box } from "./boundingBox";

export interface Page {
    nr: number,
    boundingBox: Box,
    blocks: Block[],
}