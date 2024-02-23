import type { Footnote } from "./footnote";
import type { Page } from "./page";

export interface PdfDocument {
    filename: string,
    fontNames: string,
    textContent: string,
    fontGroups: string[],
    pages: Page[],
    footnotes: Footnote[],
}