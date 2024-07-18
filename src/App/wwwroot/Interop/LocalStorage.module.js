export const get = key => {
  const value = localStorage.getItem(key);
  if (!value) return null;

  try {
    return JSON.parse(value);
  } catch {
    return null;
  }
};

export const set = (key, value) => localStorage.setItem(key, JSON.stringify(value));