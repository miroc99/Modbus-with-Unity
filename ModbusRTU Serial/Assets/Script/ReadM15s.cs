using System;
using UnityEngine;
using TMPro;

public class ReadM15s : MonoBehaviour
{

    [SerializeField] private bool logToConsole;
    [SerializeField] private string portName = "COM4";
    [SerializeField] private int baudRate = 9600;
    [SerializeField] [Range(100, 3000)] private int delayTime = 500;
    [SerializeField] private TextMeshProUGUI displayTM;
    [SerializeField] private ushort setOriginValue;
    [SerializeField] private bool enableLimitSwitch;

    private ModbusRTUSerial _modbusRTU;
    private string _valueString;

    public ModbusRTUSerial ModbusRTUSerial => _modbusRTU;

    // public float pp;
    private void Awake()
    {
        _modbusRTU = new ModbusRTUSerial(portName, baudRate, delayTime);
        _modbusRTU.OnValueChange += LogValue;
        _modbusRTU.OnValueChange += Display;
    }
    private void Update()
    {
        displayTM.text = _valueString;
        // pp = _modbusRTU.LowLimit;
        if (enableLimitSwitch)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetToStart();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                SetToEnd();
            }
        }
    }

    [ContextMenu("Read")]
    public void StartRead()
    {
        _modbusRTU.StartRead();
    }
    [ContextMenu("Stop")]
    public void StopRead()
    {
        _modbusRTU.StopRead();
    }
    /// <summary>
    /// 將現在值改為 origin
    /// </summary>
    [ContextMenu("SetToOrigin")]
    public void SetToOrigin()
    {
        _modbusRTU.SetToOrigin();
        Display(0);
    }
    /// <summary>
    /// 設定 origin 的值
    /// </summary>
    [ContextMenu("SetOriginValue")]
    public void SetOriginValue()
    {
        _modbusRTU.SetOriginValue(setOriginValue);
    }
    /// <summary>
    /// 起始點 = 0
    /// </summary>
    private void SetToStart()
    {
        _modbusRTU.SetOriginValue(0);
        _modbusRTU.SetToOrigin();
        Display(0);
    }
    /// <summary>
    /// 末端點 = 600
    /// </summary>
    private void SetToEnd()
    {
        _modbusRTU.SetOriginValue(60000);
        _modbusRTU.SetToOrigin();
        Display(600);
    }
    public void LogValue(float value)
    {
        if (logToConsole)
            Debug.Log(value);
    }

    public void Display(float value)
    {
        _valueString = "Current Position: " + value.ToString() + " mm";
    }


    public float GetReadValue()
    {
        if (null != _modbusRTU)
        {
            return _modbusRTU.Value;
        }
        return -1f;
    }
    private void OnDisable()
    {
        _modbusRTU.OnValueChange -= LogValue;
        _modbusRTU.OnValueChange -= Display;

        _modbusRTU.Close();
    }
}
