using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using Caliburn.Micro;
using DCCK.PlatformSDK;
using HandyControl.Controls;
using Microsoft.Win32;
using QA.Business.Converts;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Message;
using QA.Business.Station;
using QA.Business.Steps;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;

namespace QA.Pages.OtherViews.ViewModels
{
    [Export("VisionViewModel")]
    public class VisionViewModel : Screen, IHandle<List<IComponent>>, IHandle<CarrierInfoPanelMessage>
    {
        #region Field
        private IEventAggregator _eventAggregator;
        private IBaseBiz _baseBiz;
        private StepStatus _stepStatus;
        public VpsSolution DcckVpsSolution = null;
        private static readonly TrayStatusToStringConvert trayStatusToStringConvert = new TrayStatusToStringConvert();
        private static readonly TrayStatusToColorConvert trayStatusToColorConvert = new TrayStatusToColorConvert();
        #endregion

        #region Property

        public string XStr { get; set; } = "X: ";
        public string YStr { get; set; } = "Y: ";
        public string RStr { get; set; } = "R: ";
        public string VisionOkNgStr1 { get; set; } = "";
        public string VisionOkNgStr2 { get; set; } = "";
        public Brush VisionOkNgColor { get; set; } = Brushes.White;

        private bool _enableButtons = false;
        public bool EnableButtons
        {
            get => _enableButtons;
            set
            {
                _enableButtons = value;
                NotifyOfPropertyChange(() => EnableButtons);
            }
        }

        /// <summary>
        /// 德创软件运行界面
        /// </summary>
        public UserControl _visionBindingApp;
        public UserControl VisionBindingApp
        {
            get { return _visionBindingApp; }
            set { _visionBindingApp = value; NotifyOfPropertyChange(() => VisionBindingApp); }
        }

        /// <summary>
        /// 德创某个指定窗口
        /// </summary>
        public UserControl _visionBindingWindow1;
        public UserControl VisionBindingWindow1
        {
            get { return _visionBindingWindow1; }
            set { _visionBindingWindow1 = value; NotifyOfPropertyChange(() => VisionBindingWindow1); }
        }

        /// <summary>
        /// 德创某个指定窗口
        /// </summary>
        public UserControl _visionBindingWindow2;
        public UserControl VisionBindingWindow2
        {
            get { return _visionBindingWindow2; }
            set { _visionBindingWindow2 = value; NotifyOfPropertyChange(() => VisionBindingWindow2); }
        }

        #endregion

        #region Constructor

        public VisionViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _baseBiz = IoC.Get<IBaseBiz>();
            _stepStatus = IoC.Get<StepStatus>();
            try
            {
                DcckVpsSolution = new VpsSolution();
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "视觉初始化失败，" + ex.Message, En_Logout_Type.Exception, true);
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, ex.ToString(), En_Logout_Type.Exception, false);
            }
        }

        #endregion

        #region Caliburn.Micro
        public void Handle(List<IComponent> message)
        {
            EnableButtons = !(_baseBiz as BaseBiz).AutoRun;
        }

        public void Handle(CarrierInfoPanelMessage message)
        {
            XStr = "X: " + message.XStr;
            YStr = "Y: " + message.YStr;
            RStr = "R: " + message.RStr;
            if (message.Cavity == 0)
            {
                //穴位为零，说明要重置xyr显示区
                VisionOkNgStr1 = "";
                VisionOkNgStr2 = "";
                VisionOkNgColor = Brushes.White;
            }
            else
            {
                if (message.Status < EN_TrayStatus.禁用)
                {
                    //显示处理后的状态
                    VisionOkNgStr1 = message.Cavity + "穴";
                    VisionOkNgStr2 = (string)trayStatusToStringConvert.Convert(message.Status, null, null, null);
                    VisionOkNgColor = (Brush)trayStatusToColorConvert.Convert(message.Status, null, null, null);
                }
                else
                {
                    //只是刷新显示（例如切换一个穴位为待处理状态），此时不需要变换xyr区域显示
                    return;
                }
            }
            NotifyOfPropertyChange(() => XStr);
            NotifyOfPropertyChange(() => YStr);
            NotifyOfPropertyChange(() => RStr);
            NotifyOfPropertyChange(() => VisionOkNgStr1);
            NotifyOfPropertyChange(() => VisionOkNgStr2);
            NotifyOfPropertyChange(() => VisionOkNgColor);
        }
        #endregion

        /// <summary>
        /// 加载德创视觉
        /// </summary>
        public void VisionLoad()
        {
            string vpsPath = _stepStatus.ParamManager.CameraParam.VisionPath;
            bool isPathOk = vpsPath.EndsWith(".vps") && File.Exists(vpsPath);
            if (!isPathOk)
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "(*.vps)|*.vps";
                if (openFileDialog.ShowDialog() == true)
                {
                    vpsPath = openFileDialog.FileName;
                    _stepStatus.ParamManager.CameraParam.VisionPath = vpsPath;
                    _stepStatus.ParamManager.SaveParam(_stepStatus.ParamManager.CameraParam);
                    isPathOk = true;
                }
            }
            if (!isPathOk)
            {
                return;
            }
            try
            {
                DcckVpsSolution.Dispose();
                //加载德创应用界面
                VisionBindingApp = DcckVpsSolution.HMIControl;
                //加载的是某个指定窗口
                VisionBindingWindow1 = DcckVpsSolution.GetWindowControl("窗体1");
                //VisionBindingWindow2 = DcckVpsSolution.GetWindowControl("视觉结果");
                DcckVpsSolution.Load(vpsPath, false);
            }
            catch (Exception ex)
            {
                _stepStatus.ParamManager.CameraParam.VisionPath = "";
                _stepStatus.ParamManager.SaveParam(_stepStatus.ParamManager.CameraParam);
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 德创视觉关闭
        /// </summary>
        public void VisionStop()
        {
            try
            {
                DcckVpsSolution.Dispose();
                VisionBindingApp = null;
                VisionBindingWindow1 = null;
                VisionBindingWindow2 = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
