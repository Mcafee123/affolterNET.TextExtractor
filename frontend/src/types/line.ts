import type { Box } from "./boundingBox";
import type { Word } from "./word";

export interface Line {
    boundingBox: Box,
    words: Word[]
}