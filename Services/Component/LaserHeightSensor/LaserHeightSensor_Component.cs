/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-03-12
 * 说明：（激光测高仪逻辑）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ComCtrl;
using QA.Business.Interfaces;
using QA.Business.Model.Alarm;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;

namespace QA.Business.Component.LaserHeightSensor
{
    public class LaserHeightSensor_Component : SerialPortCtrl, ILaserHeightSensor
    {
        private LaserHeightSensorParam _laserHeightSensorParam;
        private SerialPortParam _serialPortParam;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        public IParam Param { get; set; }
        public string ComponentName { get; set; }

        public bool IsConnected { get; set; }

        public LaserHeightSensor_Component()
        {
            Param = new LaserHeightSensorParam();
            _serialPortParam = new SerialPortParam();
        }

        public bool Initial(IParam param)
        {
            try
            {
                serialport_retrycount = 3;
                Param = _laserHeightSensorParam = param as LaserHeightSensorParam;
                _serialPortParam.Com = _laserHeightSensorParam.COMPort;
                _serialPortParam.BaudRate = _laserHeightSensorParam.Baudrate;
                _serialPortParam.DataBit = 8;
                _serialPortParam.StopBits = System.IO.Ports.StopBits.One;
                _serialPortParam.Parity = System.IO.Ports.Parity.None;
                _serialPortParam.OrderTimeOut = _laserHeightSensorParam.ReceiveTimeout;
                _serialPortParam.ReadTimeOut = _laserHeightSensorParam.ReceiveTimeout;
                _serialPortParam.WriteTimeOut = _laserHeightSensorParam.SendTimeout;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.CriticalError, e + e.StackTrace);
                return false;
            }
            return true;
        }

        public void GetCurAlarms(ref ObservableCollection<AlarmInfoModel> alarmInfos)
        {
            if (_laserHeightSensorParam.BUse)
            {
                if (!IsConnected)
                {
                    var count = alarmInfos.Count;
                    alarmInfos.Add(new AlarmInfoModel()
                    {
                        AlarmLevel = EN_WARN_LEVEL.Error,
                        AlarmModule = EN_WarnModules.System,//添加异常报警模块
                        AlarmMsg = "无连接",
                        Datetime = DateTime.Now,
                        ErrorCode = 0,
                        Index = count
                    });
                }
            }
        }

        public bool GetLaserHightStatus()
        {
            float temp;
            return MeasureOnce(out temp);
        }

        public bool MeasureOnce(out float value)
        {
            value = 0;
            if (!IsOpen())
            {
                return false;
            }
            lock (obj)
            {
                int retrycount = 0;
                while (retrycount < serialport_retrycount)
                {
                    string retStr1 = "";
                    string retStr2 = "";
                    try
                    {
                        //send: \u0002MEASURE\u0003
                        //ret:  \u000234.7999\u0003
                        ClearReadBuffer();
                        byte[] send = Encoding.Default.GetBytes("\u0002MEASURE\u0003");
                        Send(send);
                        WhileReadExisting(ref retStr1, 8, OrderTimeOut);
                        if (retStr1.Contains("\u0002"))
                        {
                            retStr2 = retStr1.Substring(retStr1.IndexOf('\u0002') + 1);
                            if (retStr2.Contains("\u0003"))
                            {
                                retStr2 = retStr2.Substring(0, retStr2.IndexOf('\u0003'));
                                //value 一般是 25-35
                                value = float.Parse(retStr2);
                                return true;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"MeasureOnce {ex.ToString()},{ex.StackTrace}");
                    }
                    finally
                    {
                        byte[] str1bytes = Encoding.Default.GetBytes(retStr1);
                        string s1 = "";
                        foreach (var b in str1bytes)
                        {
                            s1 += b + ",";
                        }
                        byte[] str2bytes = Encoding.Default.GetBytes(retStr2);
                        string s2 = "";
                        foreach (var b in str1bytes)
                        {
                            s2 += b + ",";
                        }
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"[{retStr1}] [{s1}] [{retStr2}] [{s2}]", En_Logout_Type.Pdca);
                    }
                    Close();
                    IsConnected = OpenPort(_serialPortParam);
                    retrycount++;
                }
                return false;
            }
        }

        public bool Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            Task.Factory.StartNew(async () =>
            {
                var delayTime = _laserHeightSensorParam.BUse ? 100 : 1000;
                try
                {
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (_laserHeightSensorParam.BUse)
                    {
                        if (!IsConnected)
                        {
                            if (IsOpen())
                                Close();
                            IsConnected = OpenPort(_serialPortParam);
                        }
                    }
                }
                catch (Exception e)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
                }

                await Task.Delay(delayTime, _cancellationToken);
            }, _cancellationToken);

            return true;
        }

        public bool Stop()
        {
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
            }
            return true;
        }
    }
}
