# Modbus-with-Unity
Using NModbus

# Modbus with Unity - NModus

## [Modbus with Unity](https://github.com/miroc99/Modbus-with-Unity)

- **安裝NModus**

    首先要安裝 NMobus， 開啟 IDE(以VSCode 為例)，如果沒有安裝過 NuGet Package Manager 就先到 Extensions 搜尋 Nuget 安裝

     

    ![image](https://media.githubusercontent.com/media/miroc99/Modbus-with-Unity/master/README%20Images/NModbus01.png)

    安裝好後 **Ctrl+Shift+P** 開啟命令選擇列，輸入 `>nuget` ，選擇 `NuGet Package Manager: Add Package`

    ![image](https://media.githubusercontent.com/media/miroc99/Modbus-with-Unity/master/README%20Images/NModbus02.png)

    搜尋 NModus 選擇第一個安裝

    ![image](https://media.githubusercontent.com/media/miroc99/Modbus-with-Unity/master/README%20Images/NModbus03.png)

- **Unity 初連接**

    安裝完 NModus 後， clone 上面的專案開啟 ModbusRTU Serial

    確認硬體連接好之後，在 ReadM15s 物件的 ReadM15s component 裡輸入硬體連接的 COM port

    ![image](https://media.githubusercontent.com/media/miroc99/Modbus-with-Unity/master/README%20Images/Unity01.png)

    Play 後對 ReadM15s component 右鍵 ，選擇 `Read` 順利的話就可以看見現在位置的值顯示在Game view 中

    ![image](https://media.githubusercontent.com/media/miroc99/Modbus-with-Unity/master/README%20Images/Unity02.png)
