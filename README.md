# DataWebApi

Данное приложение имеет 3 метода.

ПЕРВЫЙ МЕТОД
Парсит CSV-файл, в котором на каждой строке находятся значения вида: 
{Дата в формате ГГГГ-ММ-ДД_чч-мм-сс};{Время в секундах};{Показатель в виде числа с плавающей запятой} 
Пример: 2023-02-05_09-18-17;1744;163,472
Полученные значения записываются в базу данных в таблицу Values. 
При чтении файла происходит валидация:
- дата не может быть позже текущей и раньше 01.01.2000;
- время и показатель не могут быть меньше 0;
- файл не может быть пустым;
- количество строк в файле не может превышать 10000.
Из полученных значений подсчитываются результаты (все время, момент запуска операции, среднее время выполнения,
средний показатель, медиана по показателям, максимальный показатель, минимальный показатель, количество строк), 
и записываются в таблицу Results.
Если файл с некоторым именем уже существует, то значения в базе данных перезаписываются.

ВТОРОЙ МЕТОД
Возвращает Результаты в формате JSON. Могут применяться фильтры:
- по имени файла;
- по времени запуска первой операции;
- по среднему показателю (диапазон);
- по среднему времени (диапазон).

ТРЕТИЙ МЕТОД
Получает значения из таблицы Values по имени файла.
