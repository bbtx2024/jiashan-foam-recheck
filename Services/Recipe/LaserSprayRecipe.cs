using System;
using System.ComponentModel;
using CSSI9075;

namespace QA.Business.Recipe
{
    [Serializable]
    public class LaserSprayRecipe : INotifyPropertyChanged, IRecipe
    {
        #region Property Notify
        public event PropertyChangedEventHandler PropertyChanged;
        public void ChangeProperty(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private string _RecipeName = "";
        [Category("1.名称"), DisplayName("配方名称"), ReadOnly(false)]
        public string RecipeName
        {
            get { return _RecipeName; }
            set
            {
                _RecipeName = value;
                ChangeProperty("RecipeName");
            }
        }

        [Browsable(false)]
        public float Place1stHeightLimit = 10;
        [Category("贴合参数")]
        [DisplayName("一次高度")]
        public float Place1stHeight
        {
            get { return Place1stHeightLimit; }
            set
            {
                if (value < 3) value = 3;
                if (value > 100) value = 100;
                Place1stHeightLimit = value;
            }
        }

        [Browsable(false)]
        public float[] place1stHeightSetSpeedLimit = new float[3] { 0, 20, 200 };
        [Category("贴合参数")]
        [DisplayName("一次高度后设置速度")]

        public float[] Place1stHeightSetSpeed
        {
            get { return place1stHeightSetSpeedLimit; }
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 50) value[0] = 50;

                if (value[1] < 10) value[1] = 10;
                if (value[1] > 300) value[1] = 300;

                if (value[2] < 100) value[2] = 100;
                if (value[2] > 2000) value[2] = 2000;

                place1stHeightSetSpeedLimit = value;
            }
        }

        [Browsable(false)]
        public int place1stDelayLimit = 200;
        [Category("贴合参数")]
        [DisplayName("保压时间")]
        [Description("ms")]
        public int Place1stDelay
        {
            get { return place1stDelayLimit; }
            set
            {
                if (value < 1) value = 1;
                if (value > 5000) value = 5000;
                place1stDelayLimit = value;
            }
        }

        [Browsable(false)]
        public int breakDelay = 200;
        [Category("贴合参数")]
        [DisplayName("真空破时间")]
        [Description("ms")]
        public int BreakDelay
        {
            get { return breakDelay; }
            set
            {
                if (value < 1) value = 1;
                if (value > 5000) value = 5000;
                breakDelay = value;
            }
        }

        //[Browsable(false)]
        //public float LiftupHeightLimit = 10;
        //[Category("贴合参数")]
        //[DisplayName("上抬距离Z")]
        //[Description("mm")]
        //public float LiftupHeight
        //{
        //    get { return LiftupHeightLimit; }
        //    set
        //    {
        //        if (value < 3) value = 3f;
        //        if (value > 100) value = 100f;
        //        LiftupHeightLimit = value;
        //    }
        //}

        //[Category("动作参数")]
        //[DisplayName("贴合后上抬Z轴速度")]
        //public float[] LiftupZSetSpeed
        //{
        //    get;
        //    set;
        //} = new float[3] { 5, 50, 1000 };

        //[Browsable(false)]
        //public ushort tmpsoldertimeout { get; set; } = 5;
        //[Category("报警设置")]
        //[DisplayName("贴合超时")]
        //[Description("ms")]
        //public ushort SoldTimeout
        //{
        //    get
        //    {
        //        return tmpsoldertimeout;
        //    }
        //    set
        //    {
        //        if (value <= 1) value = 1;
        //        if (value >= 50) value = 50;
        //        tmpsoldertimeout = value;
        //    }
        //}

        [Category("压力参数")]
        [DisplayName("启用找压力模式")]
        public bool PosMode
        {
            get;
            set;
        } = false;

        [Browsable(false)]
        public float MaxPressLimit = 5;
        [Category("压力参数")]
        [DisplayName("允许设置最大目标压力")]
        [Description("N，0.1N-20N")]
        public float MaxPress
        {
            get { return MaxPressLimit; }
            set
            {
                if (value < 0.1) value = 0.1f;
                if (value > 20) value = 20f;
                MaxPressLimit = value;
                if (MaxPressLimit < Press)
                {
                    Press = MaxPressLimit;
                }
            }
        }

        [Browsable(false)]
        public float PressLimit = 2;
        [Category("压力参数")]
        [DisplayName("目标压力")]
        [Description("N，0.1N-最大目标压力")]
        public float Press
        {
            get { return PressLimit; }
            set
            {
                if (value < 0.1) value = 0.1f;
                if (value > MaxPressLimit) value = MaxPressLimit;
                PressLimit = value;
            }
        }

        [Browsable(false)]
        public float beforePress = 0;
        [Category("压力参数")]
        [DisplayName("压力提前量")]
        [Description("N，0N-2N")]
        public float BeforePress
        {
            get { return beforePress; }
            set
            {
                if (value < 0) value = 0f;
                if (value > 2) value = 2f;
                if (value > Press) value = Press;
                beforePress = value;
            }
        }

        [Browsable(false)]
        private ushort findPressTimeout { get; set; } = 5000;
        [Category("压力参数")]
        [DisplayName("找压力超时")]
        [Description("ms，1ms-10000ms")]
        public ushort FindPressTimeout
        {
            get
            {
                return findPressTimeout;
            }
            set
            {
                if (value <= 1) value = 1;
                if (value >= 10000) value = 10000;
                findPressTimeout = value;
            }
        }

        //[Browsable(false)]
        //public float SlowDisLimit = 5;
        //[Category("压力参数")]
        //[DisplayName("减速距离")]
        //[Description("mm")]
        //public float SlowDis
        //{
        //    get { return SlowDisLimit; }
        //    set
        //    {
        //        if (value < 0.1) value = 0.1f;
        //        if (value > 20) value = 20f;
        //        SlowDisLimit = value;
        //    }
        //}

        //[Browsable(false)]
        //public float SlowSpeedLimit = 5;
        //[Category("压力参数")]
        //[DisplayName("减速速度")]
        //[Description("mm/s")]
        //public float SlowSpeed
        //{
        //    get { return SlowSpeedLimit; }
        //    set
        //    {
        //        if (value < 1) value = 1f;
        //        if (value > 200) value = 200f;
        //        SlowSpeedLimit = value;
        //    }
        //}

        //[Browsable(false)]
        //public float DisRangeLimit = 5;
        //[Category("压力参数")]
        //[DisplayName("位置范围")]
        //[Description("mm")]
        //public float DisRange
        //{
        //    get { return DisRangeLimit; }
        //    set
        //    {
        //        if (value < 0.1) value = 0.1f;
        //        if (value > 20) value = 20f;
        //        DisRangeLimit = value;
        //    }
        //}

        //[Category("压力参数")]
        //[DisplayName("找压力最后抬起速度")]
        //[Description("mm")]
        //public float[] findpress_lasetspeed
        //{
        //    get;
        //    set;
        //} = new float[3] { 10, 200, 2000 };


        //确定下是否输入kgN
        public CSSI_FindPressParam GetFPParam(float plusez)
        {
            return new CSSI_FindPressParam()
            {
                //alarm = PosMode,
                //dirdisrange = DisRange * plusez,
                //dir_f = Press * 100,
                //down_maxf = MaxPress * 100,
                //slowdown_dis = SlowDis * plusez,
                //slowdown_speed = SlowSpeed * plusez,
                //fqOld = false,
            };
        }
    }

    /// <summary>
    /// 激光工艺参数
    /// </summary>
    [Serializable]
    public class LaserTechnologyParam
    {
        [Category("激光焊接参数"), DisplayName("激光功率")]
        public int LaserPower { get; set; }

        [Category("激光焊接参数"), DisplayName("出光时间")]
        public int LaserOnTime { get; set; }

        [Category("激光焊接参数"), DisplayName("关光时间")]
        public int LaserOffTime { get; set; }

        public override string ToString()
        {
            return "";
        }

        public string ToStringInfo()
        {
            return $"激光功率:{LaserPower},出光时间:{LaserOnTime},关光时间:{LaserOffTime}";
        }
    }

}
