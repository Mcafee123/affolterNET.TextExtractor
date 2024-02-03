import type { Page } from "./page";

export interface PdfDocument {
    filename: string,
    fontNames: string,
    fontGroups: string[],
    pages: Page[],
}