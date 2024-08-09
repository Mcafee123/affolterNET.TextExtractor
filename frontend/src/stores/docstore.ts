import type { Doc } from '@/types/doc'

let instance: DocStore | null = null

class DocStore {

  constructor() {
    if (!instance) {
      // eslint-disable-next-line @typescript-eslint/no-this-alias
      instance = this
    }
    return this
  }
  
  public docs = new Map<string, Doc>()

  public addDoc(folder: string, doc: Doc) {
    this.docs.set(folder, doc)
  }

  public getDoc = (folder: string): Doc | null => {
    const doc = docstore.docs.get(folder) as Doc | null
    return doc || null
  }

  public removeDoc = (folder: string) => {
    this.docs.delete(folder)
  }
}

export const docstore = new DocStore()