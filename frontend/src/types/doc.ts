import type { Footnote } from "./footnote"
import type { Page } from "./page"

export interface Doc {
    filename: string,
    fontNames: string,
    textContent: string,
    fontGroups: string[],
    pages: Page[],
    pageNames: string[],
    footnotes: Footnote[],
}