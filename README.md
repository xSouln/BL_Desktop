# Описание программы
___
1. [Общее](#общее)
2. [Установка](#установка)
3. [Выбор прошивки](#выбор-прошивки)
4. [Загрузка прошивки](#загрузка-прошивки)

## Общее
- Данная версия программы преднозначена для записи и регистрации одного приложения.

## Установка
- Скачать программу можно по [ссылке](https://gitlab.adani.by:2443/rekuts/Desktop_Bootloader_STM32/-/archive/main/Desktop_Bootloader_STM32-main.zip?path=Bootloader/Application).
- Архив распаковать, приложение находиться в "название скаченного архива"/Desktop_Bootloader_STM32-main-Bootloader-Application/Bootloader/Application/BootloaderDesktop.exe

## Подключение
- Возможные интерфейсы одключения находяться в меню "Connection".
- Подключение по SerialPort устанавливается в меню "Connection" - "SerialPort" установив нужную скорость и Port. При успешно установленом подключении индикатор меню "SerialPort" будет зеленым.

![меню SerialPort](/Images/Screenshot_7.png).

- Подключение по TCP устанавливается в меню "Connection" - "Tcp" установив адрес. При успешно установленом подключении индикатор меню "Tcp" будет зеленым.

![меню Tcp](/Images/Screenshot_6.png).

- При подключенном "SerialPort" и "TCP" предпочтение по обменну данными будет отдоваться SerialPort.
