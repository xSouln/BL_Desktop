# Описание программы
___
1. [Общее](#общее)
2. [Установка](#установка)
3. [Подключение](#подключение)
4. [Выбор прошивки](#выбор-прошивки)
5. [Загрузка прошивки](#загрузка-прошивки)

## Общее
- Данная версия программы преднозначена для записи и регистрации одного приложения.

## Установка
- Скачать программу можно по [ссылке](https://gitlab.adani.by:2443/rekuts/Desktop_Bootloader_STM32/-/archive/main/Desktop_Bootloader_STM32-main.zip?path=Bootloader/Application).
- Архив распаковать, приложение находиться в "название скаченного архива"/Desktop_Bootloader_STM32-main-Bootloader-Application/Bootloader/Application/BootloaderDesktop.exe

## Подключение
- Возможные интерфейсы одключения находяться в меню "Connection".
- Подключение по SerialPort устанавливается в меню "Connection" - "SerialPort". Установив нужную скорость и Port нажать "Connect". При успешно установленом подключении индикатор меню "SerialPort" и индикатор в окне "SerialPort" будут зелеными.

![меню Connection](/Images/Screenshot_7.png).

![меню Connection-SerialPort](/Images/Screenshot_3.png).

- Подключение по TCP устанавливается в меню "Connection" - "Tcp". Установив адрес устройства нажать "Connect". При успешно установленом подключении индикатор меню "Tcp" и индикатор в окне "Tcp" будут зелеными.

![меню Connection](/Images/Screenshot_6.png).

![меню Connection-Tcp](/Images/Screenshot_5.png).

- При подключенном "SerialPort" и "TCP" предпочтение по обменну данными будет отдоваться SerialPort.

## Выбор прошивки
- Программа работает с файлами .hex.
- Для выбора прошивки выбрать "File" - "Open" и в диологовом окне указать файл. При успешном окрытии будет показано содержимое.

![меню File](/Images/Screenshot_4.png).

![меню File-Open](/Images/Screenshot_2.png).

## Загрузка прошивки


