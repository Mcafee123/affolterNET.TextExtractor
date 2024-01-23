import type { Box } from "./boundingBox";
import type { Line } from "./line";

export interface Block {
    boundingBox: Box,
    lines: Line[]
}