import type { Page } from "./page";

export interface Document {
    filename: string,
    pages: Page[],
}