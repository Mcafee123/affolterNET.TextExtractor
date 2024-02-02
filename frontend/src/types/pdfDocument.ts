import type { Page } from "./page";

export interface PdfDocument {
    filename: string,
    fontNames: string,
    pages: Page[],
}