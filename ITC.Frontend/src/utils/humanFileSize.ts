/**
 * Функция, создающая читабельный вид размер файла
 * @param size длина файла
 * @returns читабельный вид размер файла
 */
export const humanFileSize = (size: number) => {
  const i = size === 0 ? 0 : Math.floor(Math.log(size) / Math.log(1024));
  return `${Number(size / Math.pow(1024, i)).toFixed(2)} ${
    ["Б", "Кб", "Мб", "Кб", "Тб"][i]
  }`;
};
