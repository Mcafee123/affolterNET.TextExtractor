import type { Box } from "./boundingBox"
import type { Point } from "./point"

export interface Letter {
    glyphRectangle: Box,
    startBaseLine: Point,
    endBaseLine: Point,
    text: string,
    fontSize: number,
    isItalic: boolean,
    isBold: boolean,
}

export type letterType = { [Property in keyof Partial<Letter>]: number | string | Box | Point }
