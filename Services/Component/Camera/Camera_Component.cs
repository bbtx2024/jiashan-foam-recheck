/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-02-24
 * 说明：（视觉功能逻辑）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Model.Alarm;
using QA.Business.Steps;
using QA_Infrastructure;
using QA_Infrastructure.BaseCtrls.ParamEnum;
using QA_Infrastructure.NLogOut;
using ICamera = QA.Business.Interfaces.ICamera;

namespace QA.Business.Component.Camera
{
    public enum EnCameraFuncCodeType
    {
        MulMark,
        MulRecheck,
    }

    public enum EN_CameraTask
    {
        TLN,
        TLT,
        TFC1,
        TFC2,
        TFC3,
        //注意，如果添加类型，方法GroupPicture要增加对应图像处理
    }

    public class Camera_Component : TcpCtrls, ICamera
    {
        private CameraParam _cameraParam = null;
        private SocketParam _socketParam = new SocketParam();
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public IParam Param { get; set; }
        public string ComponentName { get; set; } = "Camera";
        public new bool IsConnected => base.IsConnected;

        public enum TemplateName
        {
            PointCheck,
            RedPoint,
            Nozzle,
            Point1,
            Point4
        }

        public Camera_Component()
        {
            _cameraParam = IoC.Get<CameraParam>();
        }

        public bool Initial(IParam param)
        {
            try
            {
                Param = _cameraParam = param as CameraParam;
                if (_cameraParam != null)
                {
                    _socketParam.Ip = _cameraParam.IP;
                    _socketParam.Port = _cameraParam.Port;
                    _socketParam.SendTimeout = _cameraParam.SendTimeout;
                    _socketParam.ReceiveTimeout = _cameraParam.ReceiveTimeout;
                    _socketParam.SendBuffSize = 8192;
                    _socketParam.ReceiveBuffSize = 8192;
                    base.SetParam(_socketParam);
                }
                else
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.CriticalError, "_cameraParam == null", En_Logout_Type.Exception);
                    return false;
                }
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.CriticalError, e + e.StackTrace, En_Logout_Type.Exception);
                return false;
            }
            return true;
        }

        public bool Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var delayTime = _cameraParam.BUse ? 100 : 1000;
                    try
                    {
                        if (_cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        if (_cameraParam.BUse)
                        {
                            if (!IsConnected)
                            {
                                //base.Close();
                                base.Connect(_socketParam);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace, En_Logout_Type.Exception);
                    }
                    await Task.Delay(delayTime, _cancellationToken);
                }
            }, _cancellationToken);
            return true;
        }

        public bool Stop()
        {
            try
            {
                //base.Close();
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace, En_Logout_Type.Exception);
            }
            return true;
        }

        public void GetCurAlarms(ref ObservableCollection<AlarmInfoModel> alarmInfos)
        {
            if (_cameraParam.BUse)
            {
                if (!IsConnected)
                {
                    var count = alarmInfos.Count;
                    alarmInfos.Add(new AlarmInfoModel()
                    {
                        AlarmLevel = EN_WARN_LEVEL.Error,
                        AlarmModule = EN_WarnModules.Camera,
                        AlarmMsg = "无连接",
                        Datetime = DateTime.Now,
                        ErrorCode = 0,
                        Index = count
                    });
                }
            }
        }

        public bool GetCameraResultS1(CarrierStatus carrierStatus, int idx)
        {
            try
            {
                string lineSN_OK;
                if (string.IsNullOrEmpty(carrierStatus.lineSN[idx]))
                {
                    lineSN_OK = "1";
                }
                else
                {
                    lineSN_OK = "0";
                }
                //T1,载具码,穴位号,扫码时间,主板码,主板类型
                string command = $"T1,{carrierStatus.carrierSN},{idx + 1},{carrierStatus.scanTime.Ticks},{carrierStatus.sipSN[idx]},{lineSN_OK}";
                string receiveStr = string.Empty;
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"开始复检，指令:{command}");
                if (!SendData(command, ref receiveStr))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"复检通信失败，指令:{command}");
                    return false;
                }
                var trimStr = receiveStr.TrimEnd(new char[] { '\r', '\n', '\t' });
                if (!trimStr.StartsWith("T1,OK"))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"复检返回异常，指令:{command} 返回:{trimStr}");
                    return false;
                }
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"复检完成，指令:{command} 返回:{trimStr}");
                return true;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e.ToString());
                return false;
            }
        }

        public bool SaveAllPhoto(CarrierStatus carrierStatus, int idx,int times)
        {
            try
            {
                //T1,载具码,穴位号,扫码时间,主板码,主板类型
                string command = $"S{times},{carrierStatus.carrierSN},{idx + 1}";
                string receiveStr = string.Empty;
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"开始拍全图，指令:{command}");
                if (!SendData(command, ref receiveStr))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"拍全图通信失败，指令:{command}");
                    return false;
                }
                var trimStr = receiveStr.TrimEnd(new char[] { '\r', '\n', '\t' });
                if (!trimStr.StartsWith($"S{times},OK"))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"拍全图返回异常，指令:{command} 返回:{trimStr}");
                    return false;
                }
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"拍全图完成，指令:{command} 返回:{trimStr}");
                return true;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e.ToString());
                return false;
            }

        }
        public bool GetCameraResultR1(CarrierStatus carrierStatus, int idx)
        {
            try
            {
                string command = $"R,1,{carrierStatus.carrierSN},{idx + 1},{carrierStatus.scanTime.Ticks},{carrierStatus.sipSN[idx]}";
                string receiveStr = string.Empty;
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"开始复拍，指令:{command}");
                if (!SendData(command, ref receiveStr))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"复拍通信失败，指令:{command}");
                    return false;
                }
                var trimStr = receiveStr.TrimEnd(new char[] { '\r', '\n', '\t' });
                //返回R,1
                if (!trimStr.StartsWith("R,1"))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"复拍返回异常，指令:{command} 返回:{trimStr}");
                    return false;
                }
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"复拍完成，指令:{command} 返回:{trimStr}");
                return true;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e.ToString());
                return false;
            }
        }

        public bool GetCameraResultS1A(CarrierStatus carrierStatus, out bool finished, out string[] lineSN)
        {
            lineSN = new string[12];
            finished = false;
            try
            {
                //T2,载具码
                string command = $"T2,{carrierStatus.carrierSN}";
                string receiveStr = string.Empty;
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"开始获取复检全部结果，指令:{command}");
                if (!SendData(command, ref receiveStr))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"获取复检全部结果通信失败，指令:{command}");
                    return false;
                }
                var trimStr = receiveStr.TrimEnd(new char[] { '\r', '\n', '\t' });
                //返回T2
                if (!trimStr.StartsWith("T2"))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"获取复检全部结果返回异常，指令:{command} 返回:{trimStr}");
                    return false;
                }
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"获取复检全部结果完成，指令:{command} 返回:{trimStr}");
                //T2,是否处理完毕,1穴错误代码_1穴x1_1穴y1_1穴r1_1穴原图_1穴处理图,
                //..._12穴处理图,minx1_maxx1,miny1_maxy1,minr1_maxr1
                var data = trimStr.Split(',');
                finished = data[1] == "OK";
                if (!finished)
                {
                    return false;
                }
                var x1 = data[14].Split('_');
                var x1_min = double.Parse(x1[0]);
                var x1_max = double.Parse(x1[1]);
                var y1 = data[15].Split('_');
                var y1_min = double.Parse(y1[0]);
                var y1_max = double.Parse(y1[1]);
                var r1 = data[16].Split('_');
                var r1_min = double.Parse(r1[0]);
                var r1_max = double.Parse(r1[1]);
                for (int cav = 1; cav <= 12; cav++)
                {
                    //1穴信息在data[2]
                    //data[2]: 1穴错误代码_1穴x1_1穴y1_1穴r1_1穴原图_1穴处理图
                    var data1 = data[cav + 1].Split('_');
                    //data1: {1穴错误代码, 1穴x1, ...}
                    CameraResult ret = carrierStatus.cameraResults[cav - 1];
                    ret.camResult = (EN_CamResult)int.Parse(data1[0]);
                    ret.x1 = double.Parse(data1[1]);
                    ret.x1_min = x1_min;
                    ret.x1_max = x1_max;
                    ret.y1 = double.Parse(data1[2]);
                    ret.y1_min = y1_min;
                    ret.y1_max = y1_max;
                    ret.r1 = double.Parse(data1[3]);
                    ret.r1_min = r1_min;
                    ret.r1_max = r1_max;
                    //注意：原图、处理图路径可能包含"_"
                    //循环4次，去除前面的内容
                    var data2 = data[cav + 1];
                    for (int i = 0; i < 4; i++)
                    {
                        data2 = data2.Substring(data2.IndexOf('_') + 1);
                    }
                    //data2: 1穴原图_1穴处理图_排线SN
                    int idx1 = data2.IndexOf(".jpg_") + 4;
                    ret.imgOrigin = data2.Substring(0, idx1);
                    //data3: 1穴处理图_排线SN
                    var data3 = data2.Substring(idx1 + 1);
                    int idx2 = data3.IndexOf(".jpg_") + 4;
                    ret.imgProcess = data3.Substring(0, idx2);
                    if (string.IsNullOrEmpty(carrierStatus.lineSN[cav-1]))
                    {
                        lineSN[cav - 1] = data3.Substring(idx2 + 1);
                        carrierStatus.lineSN[cav - 1] = data3.Substring(idx2 + 1);
                    }
                    if (ret.camResult == EN_CamResult.OK && carrierStatus.lineSN[cav-1] == "7")
                    {
                        ret.camResult = (EN_CamResult)7;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e.ToString());
                return false;
            }
        }

        public bool Cpk(out bool isOk, out string x, out string y)
        {
            isOk = false;
            x = "";
            y = "";
            try
            {
                string command = "D1";
                string receiveStr = string.Empty;
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"开始CPK，指令：{command}", En_Logout_Type.SpotCheck, true);
                if (!SendData(command, ref receiveStr))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CPK失败，指令：{command}", En_Logout_Type.SpotCheck, true);
                    Close();
                    Connect(_socketParam);
                    if (!SendData(command, ref receiveStr))
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CPK失败，指令：{command}", En_Logout_Type.SpotCheck, true);
                        return false;
                    }
                }
                var trimStr = receiveStr.TrimEnd(new char[] { '\r', '\n', '\t' });
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CPK成功，返回：{trimStr}", En_Logout_Type.SpotCheck, true);
                isOk = trimStr.ToLower().StartsWith("d1,true");
                if (isOk)
                {
                    x = trimStr.Split(',')[2];
                    y = trimStr.Split(',')[3];
                }
                return true;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e.ToString(), En_Logout_Type.SpotCheck, true);
                return false;
            }
        }
        //是否拍全图
        public bool IsGetAllPhoto()
        {
            return _cameraParam.GetAllPhoto;
        }
    }

    public class CameraResult
    {
        public DateTime time = DateTime.Now;
        public EN_CamResult camResult = EN_CamResult.空穴;
        public double x1 = 9999;
        public double x1_min;
        public double x1_max;
        public double y1 = 9999;
        public double y1_min;
        public double y1_max;
        public double r1 = 9999;
        public double r1_min;
        public double r1_max;
        public string imgOrigin;
        public string imgProcess;


        public CameraResult()
        {
        }
    }

    public abstract class TcpCtrls
    {
        private SocketParam _param = new SocketParam()
        {
            Ip = "127.0.0.1",
            Port = 10086,
            SendBuffSize = 8192,
            ReceiveBuffSize = 8192,
            SendTimeout = 1000,
            ReceiveTimeout = 1000,
        };
        private readonly object _socketLock = new object();
        private Socket _socket = null;
        private int _connectTimeout = 500;
        private bool _isEnabledAsyncConnected = false;
        public bool IsConnected
        {
            get
            {
                if (_socket == null)
                    return false;
                return _socket.Connected;
            }
        }

        public void SetParam(SocketParam param)
        {
            _param = param;
        }
        public void SetConnectTimeout(int millisecond = 500)
        {
            if (millisecond <= 0)
                _connectTimeout = 500;
            if (millisecond > 3000)
                _connectTimeout = 3000;
            else
                _connectTimeout = millisecond;
        }
        public bool Connect(SocketParam param, bool async = false)
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ipe = new IPEndPoint(IPAddress.Parse(param.Ip), param.Port);
                _socket.SendBufferSize = param.SendBuffSize;
                _socket.ReceiveBufferSize = param.ReceiveBuffSize;
                _socket.SendTimeout = param.SendTimeout;
                _socket.ReceiveTimeout = param.ReceiveTimeout;
                _socket.NoDelay = true;
                _param = param;
                _isEnabledAsyncConnected = async;
                if (async)
                {
                    IAsyncResult result = _socket.BeginConnect(ipe, null, null);
                    result.AsyncWaitHandle.WaitOne(_connectTimeout);
                }
                else
                    _socket.Connect(ipe);

            }
            catch (Exception ex)
            {
                TraceDebugInfo.TraceOuPut(EN_WARN_LEVEL.CriticalError, $"TcpCtrl Init() ex,{ex.ToString() + ex.StackTrace}");
                return false;
            }
            return _socket == null ? false : _socket.Connected;
        }
        public bool Close()
        {
            lock (_socketLock)
            {
                if (_socket == null)
                    return false;
                try
                {
                    if (_socket.Connected)
                        _socket.Disconnect(true);
                    _socket.Close();
                    _socket = null;
                }
                catch (Exception ex)
                {
                    TraceDebugInfo.TraceOuPut(EN_WARN_LEVEL.CriticalError, $"TcpCtrl Close() ex,{ex.ToString() + ex.StackTrace}");
                    return false;
                }
                return true;
            }
        }
        public bool ReConnect(int retry = 0)
        {
            try
            {
                lock (_socketLock)
                {
                    Close();
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    int count = 0;
                Retry:
                    if (Connect(_param, _isEnabledAsyncConnected))
                        return true;
                    else if (count < retry)
                    {
                        count++;
                        goto Retry;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                TraceDebugInfo.TraceOuPut(EN_WARN_LEVEL.Error, $"TcpCtrl ReConnect() ex,{ex.ToString() + ex.StackTrace}");
                return false;
            }
        }
        public bool SendData(string data, ref string receive, bool needreceive = true)
        {
            try
            {
                int count = 0;
            connect:
                lock (_socketLock)
                {
                    if (_socket == null || !_socket.Connected)
                    {
                        if (!ReConnect(1))
                            return false;
                    }
                    var senddata = Encoding.ASCII.GetBytes(data);
                    if (_socket.Available > 0)
                    {
                        var revdep = new byte[_param.ReceiveBuffSize];
                        _socket.Receive(revdep);
                    }
                    _socket.Send(senddata);
                    //if (needreceive == false) return true;
                    //var receivedata = new byte[_param.ReceiveBuffSize];
                    //if (_socket.Receive(receivedata) > 0)
                    //{
                    //    receive = Encoding.ASCII.GetString(receivedata);
                    //}
                    if (needreceive == false) return true;
                    var receivedata = new byte[_param.ReceiveBuffSize];
                    int datalength = 0;
                    int index = 0;
                    int timeout = _param.ReceiveTimeout;
                    DateTime dt = DateTime.Now;
                    datalength = _socket.Receive(receivedata);
                    Thread.Sleep(10);
                    while (_socket.Available > 0 && DateTime.Now.Subtract(dt).TotalMilliseconds < timeout)
                    {
                        if (_socket.ReceiveBufferSize - datalength <= 0)
                        {
                            TraceDebugInfo.TraceOuPut(EN_WARN_LEVEL.Error, $"TcpCtrl SendData() _socket.ReceiveBufferSize - datalength <= 0 Actually value = {(_socket.ReceiveBufferSize - datalength).ToString()}");
                            return false;
                        }
                        //参数 数据缓存区  起始位置  数据长度  值的按位组合
                        index = _socket.Receive(receivedata, datalength, _socket.ReceiveBufferSize - datalength, SocketFlags.None);
                        datalength += index;
                        Thread.Sleep(10);
                    }
                    if (DateTime.Now.Subtract(dt).TotalMilliseconds >= timeout)
                    {
                        TraceDebugInfo.TraceOuPut(EN_WARN_LEVEL.Error, $"TcpCtrl SendData() send {data} receive {receive} ,Timeout {timeout}");
                        return false;
                    }
                    receive = Encoding.ASCII.GetString(receivedata).Trim('\0');
                    if (!_socket.Connected)
                    {
                        if (count < 3)
                        {
                            count++;
                            goto connect;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceDebugInfo.TraceOuPut(EN_WARN_LEVEL.Error, $"TcpCtrl SendData() send {data} receive {receive} ex,{ex.ToString() + ex.StackTrace}");
                return false;
            }
            return true;
        }
        public bool SendData(string data, ref string receive, int minlen, bool needreceive = true)
        {
            try
            {
                int count = 0;
                if (minlen <= 0) return false;
                connect:
                lock (_socketLock)
                {
                    if (_socket == null || !_socket.Connected)
                    {
                        if (!ReConnect(1))
                            return false;
                    }
                    var senddata = Encoding.ASCII.GetBytes(data);
                    if (_socket.Available > 0)
                    {
                        var revdep = new byte[_param.ReceiveBuffSize];
                        _socket.Receive(revdep);
                    }
                    _socket.Send(senddata);
                    //if (needreceive == false) return true;
                    //var receivedata = new byte[_param.ReceiveBuffSize];
                    //if (_socket.Receive(receivedata) > 0)
                    //{
                    //    receive = Encoding.ASCII.GetString(receivedata);
                    //}
                    if (needreceive == false) return true;
                    var receivedata = new byte[_param.ReceiveBuffSize];
                    int datalength = 0;
                    int index = 0;
                    int timeout = _param.ReceiveTimeout;
                    DateTime dt = DateTime.Now;
                    datalength = _socket.Receive(receivedata);
                    while ((datalength < minlen) && DateTime.Now.Subtract(dt).TotalMilliseconds < timeout)
                    {
                        if (_socket.ReceiveBufferSize - datalength <= 0)
                        {
                            TraceDebugInfo.TraceOuPut(EN_WARN_LEVEL.Error, $"TcpCtrl SendData() _socket.ReceiveBufferSize - datalength <= 0 Actually value = {(_socket.ReceiveBufferSize - datalength).ToString()}");
                            return false;
                        }
                        //参数 数据缓存区  起始位置  数据长度  值的按位组合
                        index = _socket.Receive(receivedata, datalength, _socket.ReceiveBufferSize - datalength, SocketFlags.None);
                        datalength += index;
                        Thread.Sleep(1);
                    }
                    if (DateTime.Now.Subtract(dt).TotalMilliseconds >= timeout)
                    {
                        TraceDebugInfo.TraceOuPut(EN_WARN_LEVEL.Error, $"TcpCtrl SendData() send {data} receive {receive} ,Timeout {timeout}");
                        return false;
                    }

                    receive = Encoding.ASCII.GetString(receivedata).Trim('\0');
                    if (!_socket.Connected)
                    {
                        if (count < 3)
                        {
                            count++;
                            goto connect;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceDebugInfo.TraceOuPut(EN_WARN_LEVEL.Error, $"TcpCtrl SendData() send {data} receive {receive} ex,{ex.ToString() + ex.StackTrace}");
                return false;
            }
            return true;
        }
        public bool SendData(string data, ref string receive, string contains, int retry = 1, bool needreceive = true)
        {
            try
            {
                int count = 0;
                lock (_socketLock)
                {
                connect:
                    if (_socket == null || !_socket.Connected)
                    {
                        if (!ReConnect(retry))
                            return false;
                    }
                    var senddata = Encoding.ASCII.GetBytes(data);
                    if (_socket.Available > 0)
                    {
                        var revdep = new byte[_param.ReceiveBuffSize];
                        _socket.Receive(revdep);
                    }
                    _socket.Send(senddata);
                    if (needreceive == false) return true;
                    var receivedata = new byte[_param.ReceiveBuffSize];
                    string revtmp = "";
                    int timeout = _param.ReceiveTimeout;
                    DateTime dt = DateTime.Now;
                    while (DateTime.Now.Subtract(dt).TotalMilliseconds < timeout)
                    {
                        if (_socket.Receive(receivedata) > 0)
                        {
                            revtmp = Encoding.ASCII.GetString(receivedata);
                            receive += revtmp.Trim('\0');

                            if (receive.Contains(contains))
                                return true;
                        }
                        Thread.Sleep(10);
                    }
                    if (!_socket.Connected)
                    {
                        if (count < 3)
                        {
                            count++;
                            goto connect;
                        }
                    }
                    if (!receive.Contains(contains))
                        return false;
                }
            }
            catch (Exception ex)
            {
                TraceDebugInfo.TraceOuPut(EN_WARN_LEVEL.Error, $"TcpCtrl SendData() contains send {data} receive {receive} ex,{ex.ToString() + ex.StackTrace}");
                return false;
            }
            return true;
        }
        public bool SendData(byte[] data, byte[] receive, bool needreceive = true)
        {
            try
            {
                int count = 0;
                lock (_socketLock)
                {
                connect:
                    if (_socket == null || !_socket.Connected)
                    {
                        if (!ReConnect(1))
                            return false;
                    }

                    if (_socket.Available > 0)
                    {
                        var revdep = new byte[_param.ReceiveBuffSize];
                        _socket.Receive(revdep);
                    }
                    _socket.Send(data);
                    if (needreceive == false)
                        return true;
                    var receivedata = new byte[_param.ReceiveBuffSize];
                    if (_socket.Receive(receivedata) > 0)
                    {
                        Buffer.BlockCopy(receivedata, 0, receive, 0, receive.Length);
                        return true;
                    }
                    if (!_socket.Connected)
                    {
                        if (count < 3)
                        {
                            count++;
                            goto connect;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceDebugInfo.TraceOuPut(EN_WARN_LEVEL.Error, $"TcpCtrl SendData() send {TraceDebugInfo.GetByteArrayString(data)} receive {TraceDebugInfo.GetByteArrayString(receive)} ex,{ex.ToString() + ex.StackTrace}");
                return false;
            }
            return false;
        }
    }
}
