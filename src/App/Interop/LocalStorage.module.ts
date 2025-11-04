export const get = (key: string) => {
  const value = localStorage.getItem(key);
  if (!value) return null;

  try {
    return JSON.parse(value);
  } catch {
    return null;
  }
};

export const set = (key: string, value: any) => localStorage.setItem(key, JSON.stringify(value));