using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

using NModbus;
using NModbus.Serial;

using System.Threading;
using System.Threading.Tasks;

public class ModbusRTUSerial
{
    //在 MonoBehaviour 裡註冊 OnValueChange 時要注意，因為 RTUSerialReadRegsiters 不是在主執行緒上運行，依照 Unity 的架構非主執行續不可對 GameObject 進行操作
    //建議把 value cache 起來，在 MonoBehaviour 的 cycle 裡(ex:Awake(),Start(),Update)做更新

    private SerialPort _port;
    private string _portName;
    private int _baudRate = 9600;

    private IModbusSerialMaster master;
    private byte _slaveID = 2;//裝置位址
    private ushort _startAddress = 0;//current position寄存器起始位址
    private ushort _commandAddress = 4097;//命令寄存器位址
    private ushort _numRegisters = 2;//寄存器數量

    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();//Task 取消 token
    private static CancellationToken _token = _cancellationTokenSource.Token;//Task 取消 token
    // private static CancellationTokenSource _lowLimitCancellationTokenSource = new CancellationTokenSource();//Task 取消 token
    // private static CancellationToken _lowLimitToken = _lowLimitCancellationTokenSource.Token;//Task 取消 token

    private float _previousValue = 0f;
    public float Value { get; private set; }
    // public float LowLimit { get; private set; }
    public Action<float> OnValueChange;



    private int _delayTime;

    public ModbusRTUSerial(string portName, int baudRate, int delayTime)
    {
        this._delayTime = delayTime;

        _portName = portName;
        _baudRate = baudRate;

        _port = new SerialPort(_portName);
        _port.BaudRate = _baudRate;
        _port.DataBits = 8;
        _port.Parity = Parity.None;
        _port.StopBits = StopBits.Two;



    }
    #region  Helper
    /// <summary>
    /// 打開指定的 Serial Port，並實例一個 RTU Master
    /// </summary>
    public void OpenSerialPort()
    {
        try
        {
            _port.Open();
            Debug.Log("<color=green>Connect to </color>" + _portName);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }


        var factory = new ModbusFactory();
        master = factory.CreateRtuMaster(_port);

    }
    /// <summary>
    /// 讀取現在位置值
    /// </summary>
    /// <returns></returns>
    public async void StartRead()
    {
        OpenSerialPort();
        Debug.Log(">> Start reading");
        await Task.Run(() => ReadCurrentPosition(_startAddress, _token), _token);
        // await Task.Run(() => ReadLowLimit(_commandAddress, _lowLimitToken), _lowLimitToken);
    }
    
    public void StopRead()
    {
        Debug.Log(">> Stop reading");
        _cancellationTokenSource.Cancel();
        // _lowLimitCancellationTokenSource.Cancel();
    }

    public void SetToOrigin()
    {
        Debug.Log(">> Set to origin");
        ushort[] value = { 32768 };//原始 RTU 指令 02 10 10 00 00 01 02 80 00
        Value = 0;
        _previousValue = 0;
        RTUSerialWriteMultipleRegsiter(_commandAddress, value);
    }

    /// <summary>
    /// 輸入值須為期望值*100 (因為帶有兩位小數點)
    /// </summary>
    /// <param name="value"></param>
    public void SetOriginValue(ushort value)
    {
        Debug.Log(">> Set origin value " + value);
        //原始 RTU 指令 02 10 00 08 00 02 04 00 00 00 00 (後四組為寫入值)
        ushort[] valueSet = { value, 0 };
        RTUSerialWriteMultipleRegsiter(9, valueSet);
    }


    #endregion
    // private void ReadLowLimit(ushort address, CancellationToken token)
    // {
    //     while (true)
    //     {
    //         ushort[] regsiters = master.ReadHoldingRegisters(_slaveID, address, _numRegisters);
    //         LowLimit = regsiters[0];

    //         Thread.Sleep(_delayTime);

    //         if (token.IsCancellationRequested)
    //         {
    //             break;
    //         }
    //     }
    // }

    private void ReadCurrentPosition(ushort address, CancellationToken token)
    {
        while (true)
        {
            ushort[] regsiters = master.ReadHoldingRegisters(_slaveID, address, _numRegisters);
            Value = Convert.ToSingle(regsiters[0]) / 100f;
            if (Value != _previousValue)
            {
                OnValueChange.Invoke(Value);
                _previousValue = Value;
            }

            Thread.Sleep(_delayTime);

            if (token.IsCancellationRequested)
            {
                break;
            }
        }
    }


    private void RTUSerialWriteMultipleRegsiter(ushort adress, ushort[] value)
    {
        master.WriteMultipleRegisters(_slaveID, adress, value);
    }
    public void Close()
    {
        StopRead();
        _port.Close();
        Debug.Log("<color=red>Disconnect </color>" + _portName);
    }

}
