# IMU-Data-Processing

Проект IMU-Data-Processing представляет комплексное решение для обработки пространственных данных с акселерометра WIT Motion. 

## Запуск

Для запуска обработки замеров с акселерометра необходимо:
1. Скачать репозиторий.
2. Собрать проект средствами сборки среды разработки.
3. (опционально) В папке со сборкой (по относительному пути из папки проекта **bin\Debug\net8.0**) создать папку **samples** 
3
4. Положить в папку **samples**  замеры с акселерометра **Wit Motion** в формате **\*.txt** без дополнительных директорой
5. Запустить проект
6. В папке со сборкой зайти в папку **images** 

## Основные возможности

1. Считывание файла замеров с акселерометра WIT Motion.
2. Интегирование скоростей на основе ускорений акселерометра.
3. Интегирование положений на основе скоростей акселерометра.
4. Отрисовка графиков функций.
5. Сохранение графиков в директории приложения.

## Пример обработки
### Сырые данные

![alt text](doc/images/XYZ_raw.png)

### Интегрирование положений

![alt text](doc/images/XY_int.png)

### Интегрирование положений методом трапеций

![alt text](doc/images/XY_tr.png)

### Интегрирование положений методом Симпсона

![alt text](doc/images/XY_smp.png)
