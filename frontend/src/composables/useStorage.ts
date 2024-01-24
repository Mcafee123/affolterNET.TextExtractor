export function useStorage() {

  const prefix = 'text_ext'

  const getFromStorage = (name: string, def: string = ''): string => {
    return localStorage.getItem(`${prefix}_${name}`) ?? def
  }
  const setToStorage = (name: string, val: string) => {
    localStorage.setItem(`${prefix}_${name}`, val)
  }

  return { getFromStorage, setToStorage }
}