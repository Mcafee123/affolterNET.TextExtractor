import type { Block } from "./block";
import type { Line } from "./line";
import type { Word } from "./word";

export interface Footnote {
  id: string,
  inlineWords: Word[],
  bottomContentsCaption: Line,
  bottomContents: Block
}