import type { Block } from "./block";
import type { Word } from "./word";

export interface Footnote {
  id: string,
  inlineWords: Word[],
  bottomContents: Block
}