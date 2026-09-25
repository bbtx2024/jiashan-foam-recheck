using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QA.Business.Component.OtherSetting;
using QA.Business.Component.PLC;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Model.Alarm;
using QA.Business.Steps;
using QA.Business.WebReference;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;

namespace QA.Business.Component.MES
{
    public class MES_Component : IMES
    {
        #region Field
        private MESParam _mesParam;
        #endregion

        #region Property
        public IParam Param { get; set; } = null;
        public string ComponentName { get; set; } = "MES";
        public bool IsConnected { get; set; } = false;
        #endregion

        public MES_Component()
        {
            _mesParam = IoC.Get<MESParam>();
        }

        public bool Initial(IParam param)
        {
            Param = _mesParam = param as MESParam;
            return Param != null && _mesParam != null;
        }

        public bool Start()
        {
            _ = Task.Run(() =>
            {
                string[] line =  new string[12];
                GetSipSNs(line,"", out string[] sipSn);//用于初始化IsConnected
            });
            return true;
        }

        public bool Stop()
        {
            return true;
        }

        public void GetCurAlarms(ref ObservableCollection<AlarmInfoModel> alarmInfos)
        {
        }



        /// <summary>
        /// 获取Hive管控软件版本号和当前软件版本号做比对查询
        /// </summary>
        /// <param name="otherSettingParam"></param>
        /// <param name="softVersion"></param>
        /// <param name="isRetry"></param>
        /// <returns></returns>
        public bool GetVersion(OtherSettingParam otherSettingParam, out string softVersion, bool isRetry = false)
        {
            /*
            发送：
            [
              {
                "sn": "carrierSN",
                "line": "BU22-J6***",  // ***根据具体情况填写，如 BU22-J6101
                "station": "STEP ANT",
                "app": "LINKSIP",
                "p": "getlinksip",
                "fixid": "",
                "barcode": "",
                "carrier": ""
              }
            ]
            返回：
            {
              "Result": "OK",
              "Info": [
                {
                  "msg": "checkparametric=xxx,xxx,xxx,..."   //xxx表示sip码，空的穴位不会出现，所以长度不一定为12
                }
              ]
            }
            */
            softVersion = "";
            JArray arr = new JArray();
            JObject obj = new JObject();
            arr.Add(obj);
            obj.Add("sn", "");
            obj.Add("line", _mesParam.Line);
            obj.Add("station", _mesParam.Station);
            obj.Add("barcode", "");
            obj.Add("app", "PQC");
            obj.Add("fixid", _mesParam.Fixid);
            obj.Add("carrier", "");
            obj.Add("p", "checkparametric");

            StringBuilder sb = new StringBuilder();
            sb.Append("Software version");
            sb.Append("|");
            sb.Append(otherSettingParam.SoftwareVersion);
            obj.Add("parametric", sb.ToString());
            string sendStr = arr.ToString(Formatting.None);
            try
            {
                LinkSF link = new LinkSF();
                string recceivedStr = link.Query(sendStr);
                JObject receiveObj = JObject.Parse(recceivedStr);
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, "\n发送：" + sendStr + "\n接收：" + receiveObj.ToString(Formatting.None), En_Logout_Type.Mes, true);
                IsConnected = true;
                if ((string)receiveObj["Result"] != "OK")
                {
                    return false;
                }
                string msg = (string)receiveObj["Info"][0]["msg"];
                if (!msg.StartsWith("checkparametric="))
                {
                    return false;
                }
                softVersion = msg.Substring("checkparametric=".Length);
                if (softVersion != "OK")
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, ex.ToString(), En_Logout_Type.Mes, true);
                if (!isRetry)
                {
                    return GetVersion(otherSettingParam, out softVersion, true);
                }
                else
                {
                    IsConnected = false;
                    return false;
                }
            }
        }


        /// <summary>
        /// 判断TapeSN是否合规
        /// </summary>
        public bool CheckTapeSNOk(string sn, string ipns, int len, out string info)
        {
            var plc = IoC.Get<IPLC>() as PLC_Component;
            if (plc.IsSimulateRun())
            {
                info = "空跑，不检测卷料SN";
                return true;
            }
            if (sn == null || sn.Length != len)
            {
                info = "卷料SN长度不是" + len + "位";
                return false;
            }
            string[] ipnArr = ipns.Split(',');
            foreach (string s in ipnArr)
            {
                if (!string.IsNullOrEmpty(s) && sn.Contains(s))
                {
                    info = "卷料SN检测通过";
                    return true;
                }
            }
            info = "卷料SN不包含特征码";
            return false;
        }


        /// <summary>
        /// 通过产品上的排线码和载具码获取Sip码的码
        /// </summary>
        /// <param name="lineSN">产品上的排线码</param>
        /// <param name="sipSN">SipSN</param>
        /// <returns>通信成功且解析成功返回true，否则返回false</returns>
        public bool GetSipSN(string lineSN, string carrierSN, out string sipSN, bool isRetry = false)
        {
            /*
            发送：
            [
              {
                "sn": "产品上的任意码",
                "line": "BU22-J6***",  // ***根据具体情况填写，如 BU22-J6101
                "station": "STEP ANT",
                "app": "PQC",
                "fixid": "",
                "barcode": "",
                "p": "sn",
                "carrier": ""
              }
            ]
            返回：
            {
              "Result": "OK",
              "Info": [
                {
                  "msg": "sn=xxx"   //xxx表示sip码
                }
              ]
            }
            */
            sipSN = "";
            JArray arr = new JArray();
            JObject obj = new JObject();
            arr.Add(obj);
            obj.Add("sn", lineSN);
            obj.Add("line", _mesParam.Line);
            obj.Add("station", _mesParam.Station);
            obj.Add("app", "PQC");
            obj.Add("fixid", _mesParam.Fixid);
            obj.Add("barcode", "");
            obj.Add("p", "sn");
            obj.Add("carrier", carrierSN);
            string sendStr = arr.ToString(Formatting.None);
            try
            {
                LinkSF link = new LinkSF();
                string recceivedStr = link.Query(sendStr);
                JObject receiveObj = JObject.Parse(recceivedStr);
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, "\n发送：" + sendStr + "\n接收：" + receiveObj.ToString(Formatting.None), En_Logout_Type.Mes, true);
                IsConnected = true;
                if ((string)receiveObj["Result"] != "OK")
                {
                    return false;
                }
                string msg = (string)receiveObj["Info"][0]["msg"];
                if (!msg.StartsWith("sn="))
                {
                    return false;
                }
                sipSN = msg.Substring(3);
                return true;
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, ex.ToString(), En_Logout_Type.Mes, true);
                if (!isRetry)
                {
                    return GetSipSN(lineSN,carrierSN, out sipSN, true);
                }
                else
                {
                    IsConnected = false;
                    return false;
                }
            }
        }

        /// <summary>
        /// 通过载具码获取所有Sip板的码
        /// </summary>
        /// <param name="carrierSN">载具码</param>
        /// <param name="sipSNs">长度不定的数组，包含载具内所有sipSN</param>
        /// <returns>通信成功且解析成功返回true，否则返回false</returns>
        public bool GetSipSNs(string[]lineSN, string carrierSN, out string[] sipSNs, bool isRetry = false)
        {
            /*
            发送：
            [
              {
                "sn": "carrierSN",
                "line": "BU22-J6***",  // ***根据具体情况填写，如 BU22-J6101
                "station": "STEP ANT",
                "app": "LINKSIP",
                "p": "getlinksip",
                "fixid": "",
                "barcode": "",
                "carrier": ""
              }
            ]
            返回：
            {
              "Result": "OK",
              "Info": [
                {
                  "msg": "getlinksip=xxx,xxx,xxx,..."   //xxx表示sip码，空的穴位不会出现，所以长度不一定为12
                }
              ]
            }
            */
            sipSNs = new string[12];
            for (int i = 0; i < 12; i++)
            {
                if (string.IsNullOrEmpty(lineSN[i]))
                {
                    continue;
                }
                if (!GetSipSN(lineSN[i], carrierSN, out sipSNs[i], true) == true)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Camera->{i+1}穴MES查询sip码失败", En_Logout_Type.Run, true);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查sip板过站状态
        /// </summary>
        /// <param name="sipSN"></param>
        /// <param name="routingResult">路由检查结果</param>
        /// <returns>通信成功且解析完成返回true，否则返回false</returns>
        public bool GetRoutingAndCav(string sipSN, out EN_Routing_Result routingResult, out string sipType, out int cavity, bool isRetry = false)
        {
            /*
            发送：
            [
              {
                "sn": "SIP码",
                "line": "BU22-J6***",  // ***根据具体情况填写，如 BU22-J6101
                "station": "STEP ANT",
                "app": "PQC",
                "fixid": "",
                "barcode": "",
                "p": "checkrouting,checksipcavity,MODEL",
                "carrier": ""
              }
            ]           
            返回：
            {
              "Result": "OK",
              "Info": [
                {
                  "msg": "checksipcavity=01;checkrouting=OK;MODEL=LA4D"
                }
              ]
            }
            */
            routingResult = 0;
            cavity = 0;
            sipType = "";
            JArray arr = new JArray();
            JObject obj = new JObject();
            arr.Add(obj);
            obj.Add("sn", sipSN);
            obj.Add("line", _mesParam.Line);
            obj.Add("station", _mesParam.Station);
            obj.Add("app", "PQC");
            obj.Add("fixid", "");
            obj.Add("barcode", "");
            obj.Add("p", "checkrouting,checksipcavity,MODEL");
            obj.Add("carrier", "");
            string sendStr = arr.ToString(Formatting.None);
            try
            {
                LinkSF link = new LinkSF();
                string recceivedStr = link.Query(sendStr);
                JObject receiveObj = JObject.Parse(recceivedStr);
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, "\n发送：" + sendStr + "\n接收：" + receiveObj.ToString(Formatting.None), En_Logout_Type.Mes, true);
                IsConnected = true;
                if ((string)receiveObj["Result"] != "OK")
                {
                    return false;
                }
                string msg = (string)receiveObj["Info"][0]["msg"];
                string checkrouting = "";
                string checksipcavity = "";
                foreach (var s in msg.Split(';'))
                {
                    if (s.StartsWith("checkrouting="))
                    {
                        checkrouting = s.Substring("checkrouting=".Length);
                    }
                    else if (s.StartsWith("checksipcavity="))
                    {
                        checksipcavity = s.Substring("checksipcavity=".Length);
                    }
                    else if (s.StartsWith("MODEL="))
                    {
                        string MODEL = s.Substring("MODEL=".Length);
                        if (MODEL.Length >= 4)
                        {
                            sipType = MODEL.Substring(3);
                        }
                    }
                }
                if (checkrouting == "OK")
                {
                    routingResult = EN_Routing_Result.OK;
                }
                else if (new Regex("From:.*---To:.*").IsMatch(checkrouting))
                {
                    //FA06-005都属于STEP ANT站
                    if (checkrouting.StartsWith($"From:{_mesParam.Station}---To:"))
                    {
                        //From:STEP ANT---To:COS S3 or SIP Link BOX or STEP ANT-R
                        //上面的意思是这个料已经在STEP ANT上传，接下来应该在COS S3或SIP Link BOX或STEP ANT-R上传
                        routingResult = EN_Routing_Result.HasUploaded;
                    }
                    else
                    {
                        routingResult = EN_Routing_Result.NotThisStation;
                    }
                }
                else
                {
                    routingResult = EN_Routing_Result.Other;
                }
                if (!int.TryParse(checksipcavity, out cavity) || cavity <= 0 || cavity > 12)
                {
                    routingResult = EN_Routing_Result.Other;
                }
                return true;
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, ex.ToString(), En_Logout_Type.Mes, true);
                if (!isRetry)
                {
                    return GetRoutingAndCav(sipSN, out routingResult, out sipType, out cavity, true);
                }
                else
                {
                    IsConnected = false;
                    return false;
                }
            }
        }

        /// <summary>
        /// 检查sip板过站状态
        /// </summary>
        /// <param name="sipSN"></param>
        /// <param name="carrierSN">载具SN</param>
        /// <param name="routingResult">路由检查结果</param>
        /// <returns>通信成功且解析完成返回true，否则返回false</returns>
        public bool GetRouting(string sipSN,string carrierSN, out EN_Routing_Result routingResult, bool isRetry = false)
        {
            /*
            发送：
            [
              {
                "sn": "SIP码",
                "line": "BU22-J6***",  // ***根据具体情况填写，如 BU22-J6101
                "station": "STEP ANT",
                "app": "PQC",
                "fixid": "",
                "barcode": "",
                "p": "checkrouting,checksipcavity,MODEL",
                "carrier": ""
              }
            ]           
            返回：
            {
              "Result": "OK",
              "Info": [
                {
                  "msg": "checksipcavity=01;checkrouting=OK;MODEL=LA4D"
                }
              ]
            }
            */
            routingResult = 0;
            JArray arr = new JArray();
            JObject obj = new JObject();
            arr.Add(obj);
            obj.Add("sn", sipSN);
            obj.Add("line", _mesParam.Line);
            obj.Add("station", _mesParam.Station);
            obj.Add("app", "PQC");
            obj.Add("fixid", "");
            obj.Add("barcode", "");
            obj.Add("p", "checkrouting,checksaqt0");
            obj.Add("carrier", carrierSN);
            string sendStr = arr.ToString(Formatting.None);
            try
            {
                LinkSF link = new LinkSF();
                string recceivedStr = link.Query(sendStr);
                JObject receiveObj = JObject.Parse(recceivedStr);
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, "\n发送：" + sendStr + "\n接收：" + receiveObj.ToString(Formatting.None), En_Logout_Type.Mes, true);
                IsConnected = true;
                if ((string)receiveObj["Result"] != "OK")
                {
                    return false;
                }
                string msg = (string)receiveObj["Info"][0]["msg"];
                string checkrouting = "";
                foreach (var s in msg.Split(';'))
                {
                    if (s.StartsWith("checkrouting="))
                    {
                        checkrouting = s.Substring("checkrouting=".Length);
                    }
                    else if (s.StartsWith("MODEL="))
                    {
                        string MODEL = s.Substring("MODEL=".Length);
                    }
                }
                if (checkrouting == "OK")
                {
                    routingResult = EN_Routing_Result.OK;
                }
                else if (new Regex("From:.*---To:.*").IsMatch(checkrouting))
                {
                    //FA06-005都属于STEP ANT站
                    if (checkrouting.StartsWith($"From:{_mesParam.Station}---To:"))
                    {
                        //From:STEP ANT---To:COS S3 or SIP Link BOX or STEP ANT-R
                        //上面的意思是这个料已经在STEP ANT上传，接下来应该在COS S3或SIP Link BOX或STEP ANT-R上传
                        routingResult = EN_Routing_Result.HasUploaded;
                    }
                    else
                    {
                        routingResult = EN_Routing_Result.NotThisStation;
                    }
                }
                else
                {
                    routingResult = EN_Routing_Result.Other;
                }
                return true;
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, ex.ToString(), En_Logout_Type.Mes, true);
                if (!isRetry)
                {
                    return GetRouting(sipSN, carrierSN,out routingResult, true);
                }
                else
                {
                    IsConnected = false;
                    return false;
                }
            }
        }

        /// <summary>
        /// 通过载具码获取所有Sip板的码
        /// </summary>
        /// <param name="carrierSN">载具码</param>
        /// <param name="sipSNs">长度为12的数组，包含排列好的所有sipSN</param>
        /// <returns>通信成功且解析成功返回true，否则返回false</returns>
        //public bool GetSipSNsAndRoutings(string carrierSN, out string[] sipSNs, out EN_Routing_Result[] routingResults, out string[] sipTypes, bool isRetry = false)
        //{
        //    //sipSNs = new string[12];
        //    //routingResults = new EN_Routing_Result[12];
        //    //sipTypes = new string[12];
        //    //for (int i = 0; i < 12; i++)
        //    //{
        //    //    sipSNs[i] = "";
        //    //    routingResults[i] = 0;
        //    //    sipTypes[i] = "";
        //    //}
        //    //if (!GetSipSNs(carrierSN, out string[] getlinksipArr))
        //    //{
        //    //    return false;
        //    //}
        //    //if (getlinksipArr.Length == 0)
        //    //{
        //    //    return true;
        //    //}
        //    //foreach (string sn in getlinksipArr)
        //    //{
        //    //    if (!GetRoutingAndCav(sn, out EN_Routing_Result routing, out string sipType, out int cavity))
        //    //    {
        //    //        return false;
        //    //    }
        //    //    if (routing == EN_Routing_Result.Other)
        //    //    {
        //    //        return false;
        //    //    }
        //    //    sipSNs[cavity - 1] = sn;
        //    //    routingResults[cavity - 1] = routing;
        //    //    sipTypes[cavity - 1] = sipType;
        //    //}
        //    return true;
        //}

        /// <summary>
        /// 将多项信息与sip板绑定。
        /// 一旦绑定成功，则不允许再次绑定。所以应确保所有信息都存在后，再一次性绑定所有内容。
        /// </summary>
        private bool Bind(string carrierSN, string sipSN, Dictionary<string, string> dic)
        {
            /*
            发送：
            [
              {
                "sn": "SIP SN",
                "line": "BU22-J6***",
                "station": "STEP ANT",
                "app": "PQC",
                "fixid": "Machine No",
                "barcode": "",
                "errcode": "PASS",
                "carrier": "carrierSN",
                "Others": "dic[0].key|dic[0].value,dic[1].key|dic[1].value,..."
              }
            ]
            返回：
            {
              "Result": "OK",
              "Info": [
                {
                  "msg": "OK"
                }
              ]
            }
            */
            JArray arr = new JArray();
            JObject obj = new JObject();
            arr.Add(obj);
            obj.Add("sn", sipSN);
            obj.Add("line", _mesParam.Line);
            obj.Add("station", _mesParam.Station);
            obj.Add("app", "PQC");
            obj.Add("fixid", _mesParam.Fixid);
            obj.Add("barcode", "");
            obj.Add("errcode", "PASS");
            obj.Add("carrier", carrierSN);
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (var data in dic)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(",");
                }
                sb.Append(data.Key).Append("|").Append(data.Value);
            }
            obj.Add("Others", sb.ToString());
            string sendStr = arr.ToString(Formatting.None);
            try
            {
                LinkSF link = new LinkSF();
                string recceivedStr = link.ADD(sendStr);
                JObject receiveObj = JObject.Parse(recceivedStr);
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, "\n发送：" + sendStr + "\n接收：" + receiveObj.ToString(Formatting.None), En_Logout_Type.Mes, true);
                IsConnected = true;
                if ((string)receiveObj["Result"] != "OK")
                {
                    return false;
                }
                string msg = (string)receiveObj["Info"][0]["msg"];
                return msg == "OK";
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, ex.ToString(), En_Logout_Type.Mes, true);
                IsConnected = false;
                return false;
            }
        }

        /// <summary>
        /// 手动流程传MES
        /// </summary>
        public bool BindOne(string carrierSN, int cavity, string sipSN, string Battery_Foam_Tape_SN)
        {
            CarrierStatus carrierStatus = new CarrierStatus();
            carrierStatus.carrierSN = carrierSN;
            int idx = cavity - 1;
            carrierStatus.sipSN[idx] = sipSN;
            carrierStatus.Tape_SN[idx] = Battery_Foam_Tape_SN;
            return BindOne(carrierStatus, cavity);
        }

        /// <summary>
        /// 自动流程传MES
        /// </summary>
        public bool BindOne(CarrierStatus carrierStatus, int cavity)
        {
            //NPI
            int idx = cavity - 1;
            string carrierSN = carrierStatus.carrierSN;
            string sipSN = carrierStatus.sipSN[idx];
            return Bind(carrierSN, sipSN, new Dictionary<string, string>(){
                    { $"{_mesParam.TapeName} PnP nozzle", carrierStatus.Tape_Nozzle[idx].ToString("D2") },//贴装吸嘴
                    { $"{_mesParam.TapeName} press head", carrierStatus.Tape_Indenter[idx].ToString("D2") },//保压头
                    { "Carrier SN",carrierSN },
                    { "Carrier cavity", cavity.ToString("D2") },
                    { $"{_mesParam.TapeName} SN", carrierStatus.Tape_SN[idx] },
                    { $"{_mesParam.TapeName} PnP force", carrierStatus.Tape_PastePress[idx].ToString("f2") },//贴装压力
                    { $"{_mesParam.TapeName} press force",  _mesParam.BumperPressure},//保压压力
                    { $"{_mesParam.TapeName} press time",_mesParam.BumperPressureTime },//保压时间
                    { $"{_mesParam.TapeName} inspection X",carrierStatus.cameraResults[idx].x1.ToString("f2")  },
                    { $"{_mesParam.TapeName} inspection Y",carrierStatus.cameraResults[idx].y1.ToString("f2")  },
                    { $"{_mesParam.TapeName} inspection A",carrierStatus.cameraResults[idx].r1.ToString("f2")  },
                });
        }


        /// <summary>
        /// 发送机台部分信息到MES
        /// </summary>
        private bool SendAlarm(string parameter, string lower, string upper, string raw_val, string new_val, string unit, bool isRetry = false)
        {
            /*
            发送：
            [
              {
                "line": "BU22-J6***",  // ***根据具体情况填写，如 BU22-J6101
                "station": "STEP ANT",
                "fixtureid": "fixid",
                "parameter": "项目名称/版本名称",
                "lower": "",
                "upper": "",
                "raw_val": "",
                "new_val": "软件版本号",
                "user": "12345",
                "unit": "mg",
                "time": "2022-05-09 09:37:06"
              }
            ]
            返回：
            {
              "Result": "OK",
              "Info": [
                {
                  "msg": "OK"
                }
              ]
            }
            */
            JArray arr = new JArray();
            JObject obj = new JObject();
            arr.Add(obj);
            obj.Add("line", _mesParam.Line);
            obj.Add("stationid", _mesParam.Station);
            obj.Add("fixtureid", _mesParam.Fixid);
            obj.Add("parameter", parameter);
            obj.Add("lower", lower);
            obj.Add("upper", upper);
            obj.Add("raw_val", raw_val);
            obj.Add("new_val", new_val);
            //obj.Add("user", "");
            obj.Add("user", _mesParam.Opuserid);
            obj.Add("unit", unit);
            obj.Add("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            string sendStr = arr.ToString(Formatting.None);
            try
            {
                LinkSF link = new LinkSF();
                string recceivedStr = link.ALARM(sendStr);
                JObject receiveObj = JObject.Parse(recceivedStr);
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, "\n发送：" + sendStr + "\n接收：" + receiveObj.ToString(Formatting.None), En_Logout_Type.EditProgram, true);
                IsConnected = true;
                if ((string)receiveObj["Result"] != "OK")
                {
                    return false;
                }
                string msg = (string)receiveObj["Info"][0]["msg"];
                return msg == "OK";
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, ex.ToString(), En_Logout_Type.EditProgram, true);
                if (!isRetry)
                {
                    return SendAlarm(parameter, lower, upper, raw_val, new_val, unit, true);
                }
                else
                {
                    IsConnected = false;
                    return false;
                }
            }
        }

        /// <summary>
        /// 发送部分信息到MES
        /// </summary>
        public bool UploadMachineInfo()
        {
            /*
            发送：
            [
              {
                "sn": "",
                "line": "BU22-J6***",
                "station": "STEP ANT",
                "app": "PQC",
                "fixid": "",
                "barcode": "",
                "p": "checkparametric",
                "carrier": "",
                "parametric": "Software version|XXX"
              }
            ]
            返回：
            {
              "Result": "OK",
              "Info": [
                {
                  "msg": "checkparametric=OK"
                }
              ]
            }
            */
            JArray arr = new JArray();
            JObject obj = new JObject();
            arr.Add(obj);
            obj.Add("sn", "");
            obj.Add("line", _mesParam.Line);
            obj.Add("station", _mesParam.Station);
            obj.Add("app", "PQC");
            obj.Add("fixid", _mesParam.Fixid);
            obj.Add("barcode", "");
            obj.Add("p", "checkparametric");
            obj.Add("carrier", "");
            string swVersion = IoC.Get<ParamManager>().OtherSettingParam.SoftwareVersion;
            obj.Add("parametric", $"Software version|{swVersion}");
            string sendStr = arr.ToString(Formatting.None);
            try
            {
                LinkSF link = new LinkSF();
                string recceivedStr = link.Query(sendStr);
                JObject receiveObj = JObject.Parse(recceivedStr);
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, "\n发送：" + sendStr + "\n接收：" + receiveObj.ToString(Formatting.None), En_Logout_Type.EditProgram, true);
                IsConnected = true;
                if ((string)receiveObj["Result"] != "OK")
                {
                    return false;
                }
                string msg = (string)receiveObj["Info"][0]["msg"];
                return msg == "checkparametric=OK";
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, ex.ToString(), En_Logout_Type.Mes, true);
                IsConnected = false;
                return false;
            }
        }
    }
}
