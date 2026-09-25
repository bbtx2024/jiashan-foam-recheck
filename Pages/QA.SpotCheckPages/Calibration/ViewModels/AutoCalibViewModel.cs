using System.ComponentModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using QA.Business.Component.Camera;
using QA.Business.Component.Motion.Googol;
using QA.Business.Interfaces;
using QA.Business.Manager;

namespace QA.SpotCheckPages.Calibration.ViewModels
{
    [Export(typeof(ICalibrationViewModel))]
    public class AutoCalibViewModel : Screen, INotifyPropertyChanged, ICalibrationViewModel
    {
        #region Field
        private Camera_Component _camera_Component;
        private ParamManager _paramManager;
        private MotionGoogol_Component _mGoogol_Component;
        private CacheParamManager _cacheParamManager;
        #endregion

        #region Property
        public override string DisplayName { get; set; } = "自动一键标定";

        public ushort OrderID { get; set; } = 3;

        #endregion

        #region Constructor
        public AutoCalibViewModel()
        {
            _camera_Component = (Camera_Component)IoC.Get<ICamera>();
            _paramManager = IoC.Get<ParamManager>();
            _mGoogol_Component = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            _cacheParamManager = IoC.Get<CacheParamManager>();

        }
        #endregion

        #region Override

        #endregion

        #region Method


        #endregion
    }
}
