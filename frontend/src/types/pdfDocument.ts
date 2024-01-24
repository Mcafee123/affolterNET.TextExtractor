import type { Page } from "./page";

export interface PdfDocument {
    filename: string,
    pages: Page[],
}