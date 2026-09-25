/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-02-24
 * 说明：（固高运动控制参数）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using QA.Business.Interfaces;
using QA_Infrastructure;
using QA_Infrastructure.Attributes;

namespace QA.Business.Component.Motion.Googol
{
    public enum EN_GoogolExtendOutput
    {
        Out0,
        Out1,
        Out2,
        Out3,
        Out4,
        Out5,
        Out6,
        Out7,
        Out8,
        Out9,
        Out10,
        Out11,
        Out12,
        Out13,
        Out14,
        Out15,
    }
    public enum EN_GoogolExtendInput
    {
        In0,
        In1,
        In2,
        In3,
        In4,
        In5,
        In6,
        In7,
        In8,
        In9,
        In10,
        In11,
        In12,
        In13,
        In14,
        In15,
    }

    [SaveParam(FileType.Binary)]
    [Serializable]
    [Description("固高板卡设置")]
    public class MotionGoogolParam : IParam
    {
        #region 脉冲当量

        public readonly float[] PluseEquivalents = new float[10] {
            1280,1280,
            1280,1280,2560,
            138.89f,138.89f,138.89f,138.89f,
            138.89f
        };
        //private float _PluseEquivalentX1 { get; set; } = 1280;
        [Category("1.脉冲当量"), DisplayName("X1脉冲当量"), Description("每个轴的脉冲当量")]
        public float PluseEquivalentX1
        {
            get
            {
                return PluseEquivalents[(int)En_AxisNum.X1];
            }
            //private set
            //{
            //    if (value < 1) value = 1;
            //    else if (value > 3500) value = 3500;
            //    PluseEquivalents[(int)En_AxisNum.X1] = value;
            //}
        }
        //private float _PluseEquivalentY1 { get; set; } = 1280;
        [Category("1.脉冲当量"), DisplayName("Y1脉冲当量"), Description("每个轴的脉冲当量")]
        public float PluseEquivalentY1
        {
            get
            {
                return PluseEquivalents[(int)En_AxisNum.Y1];
            }
            //private set
            //{
            //    if (value < 1) value = 1;
            //    else if (value > 3500) value = 3500;
            //    PluseEquivalents[(int)En_AxisNum.Y1] = value;
            //}
        }
        //private float _PluseEquivalentX2 { get; set; } = 1280;
        [Category("1.脉冲当量"), DisplayName("X2脉冲当量"), Description("每个轴的脉冲当量")]
        public float PluseEquivalentX2
        {
            get
            {
                return PluseEquivalents[(int)En_AxisNum.X2];
            }
            //private set
            //{
            //    if (value < 1) value = 1;
            //    else if (value > 3500) value = 3500;
            //    PluseEquivalents[(int)En_AxisNum.X2] = value;
            //}
        }
        //private float _PluseEquivalentY2 { get; set; } = 1280;
        [Category("1.脉冲当量"), DisplayName("Y2脉冲当量"), Description("每个轴的脉冲当量")]
        public float PluseEquivalentY2
        {
            get
            {
                return PluseEquivalents[(int)En_AxisNum.Y2];
            }
            //private set
            //{
            //    if (value < 1) value = 1;
            //    else if (value > 3500) value = 3500;
            //    PluseEquivalents[(int)En_AxisNum.Y2] = value;
            //}
        }
        //private float _PluseEquivalentZ2 { get; set; } = 2560;
        [Category("1.脉冲当量"), DisplayName("Z2脉冲当量"), Description("每个轴的脉冲当量")]
        public float PluseEquivalentZ2
        {
            get
            {
                return PluseEquivalents[(int)En_AxisNum.Z2];
            }
            //set
            //{
            //    if (value < 1) value = 1;
            //    else if (value > 600) value = 600;
            //    PluseEquivalents[(int)En_AxisNum.Z2] = value;
            //}
        }
        //private float _PluseEquivalentR1 { get; set; } = 138.89f;
        [Category("1.脉冲当量"), DisplayName("R1脉冲当量"), Description("每个轴的脉冲当量")]
        public float PluseEquivalentR1
        {
            get
            {
                return PluseEquivalents[(int)En_AxisNum.R1];
            }
            //set
            //{
            //    if (value < 1) value = 1;
            //    else if (value > 600) value = 600;
            //    PluseEquivalents[(int)En_AxisNum.R1] = value;
            //}
        }
        //private float _PluseEquivalentR2 { get; set; } = 138.89f;
        [Category("1.脉冲当量"), DisplayName("R2脉冲当量"), Description("每个轴的脉冲当量")]
        public float PluseEquivalentR2
        {
            get
            {
                return PluseEquivalents[(int)En_AxisNum.R2];
            }
            //set
            //{
            //    if (value < 1) value = 1;
            //    else if (value > 600) value = 600;
            //    PluseEquivalents[(int)En_AxisNum.R2] = value;
            //}
        }
        //private float _PluseEquivalentR3 { get; set; } = 138.89f;
        [Category("1.脉冲当量"), DisplayName("R3脉冲当量"), Description("每个轴的脉冲当量")]
        public float PluseEquivalentR3
        {
            get
            {
                return PluseEquivalents[(int)En_AxisNum.R3];
            }
            //set
            //{
            //    if (value < 1) value = 1;
            //    else if (value > 600) value = 600;
            //    PluseEquivalents[(int)En_AxisNum.R3] = value;
            //}
        }
        //private float _PluseEquivalentR4 { get; set; } = 138.89f;
        [Category("1.脉冲当量"), DisplayName("R4脉冲当量"), Description("每个轴的脉冲当量")]
        public float PluseEquivalentR4
        {
            get
            {
                return PluseEquivalents[(int)En_AxisNum.R4];
            }
            //set
            //{
            //    if (value < 1) value = 1;
            //    else if (value > 600) value = 600;
            //    PluseEquivalents[(int)En_AxisNum.R4] = value;
            //}
        }

        #endregion

        #region 速度设置

        #region 工作速度

        private float[] _workSpeedX1 = new float[3] { 10, 200, 2000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("2.工作速度"), DisplayName("X1工作速度"), Description("起始速度、最大速度、加速度")]
        public float[] WorkSpeedX1
        {
            get => _workSpeedX1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _workSpeedX1 = value;
            }
        }

        private float[] _workSpeedY1 = new float[3] { 10, 200, 2000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("2.工作速度"), DisplayName("Y1工作速度"), Description("起始速度、最大速度、加速度")]
        public float[] WorkSpeedY1
        {
            get => _workSpeedY1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _workSpeedY1 = value;
            }
        }

        private float[] _workSpeedX2 = new float[3] { 10, 200, 2000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("2.工作速度"), DisplayName("X2工作速度"), Description("起始速度、最大速度、加速度")]
        public float[] WorkSpeedX2
        {
            get => _workSpeedX2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _workSpeedX2 = value;
            }
        }

        private float[] _workSpeedY2 = new float[3] { 10, 200, 2000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("2.工作速度"), DisplayName("Y2工作速度"), Description("起始速度、最大速度、加速度")]
        public float[] WorkSpeedY2
        {
            get => _workSpeedY2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _workSpeedY2 = value;
            }
        }

        private float[] _workSpeedZ2 = new float[3] { 10, 100, 1500 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("2.工作速度"), DisplayName("Z2工作速度"), Description("起始速度、最大速度、加速度")]
        public float[] WorkSpeedZ2
        {
            get => _workSpeedZ2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _workSpeedZ2 = value;
            }
        }

        private float[] _workSpeedR1 = new float[3] { 10, 200, 2000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("2.工作速度"), DisplayName("R1工作速度"), Description("起始速度、最大速度、加速度")]
        public float[] WorkSpeedR1
        {
            get => _workSpeedR1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _workSpeedR1 = value;
            }
        }

        private float[] _workSpeedR2 = new float[3] { 10, 200, 2000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("2.工作速度"), DisplayName("R2工作速度"), Description("起始速度、最大速度、加速度")]
        public float[] WorkSpeedR2
        {
            get => _workSpeedR2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _workSpeedR2 = value;
            }
        }

        private float[] _workSpeedR3 = new float[3] { 10, 200, 2000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("2.工作速度"), DisplayName("R3工作速度"), Description("起始速度、最大速度、加速度")]
        public float[] WorkSpeedR3
        {
            get => _workSpeedR3;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _workSpeedR3 = value;
            }
        }

        private float[] _workSpeedR4 = new float[3] { 10, 200, 2000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("2.工作速度"), DisplayName("R4工作速度"), Description("起始速度、最大速度、加速度")]
        public float[] WorkSpeedR4
        {
            get => _workSpeedR4;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _workSpeedR4 = value;
            }
        }

        #endregion

        #region 示教低速

        private float[] _lowSpeedX1 = new float[3] { 0, 10, 50 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("3.示教低速"), DisplayName("X1示教低速"), Description("起始速度、最大速度、加速度")]
        public float[] LowSpeedX1
        {
            get => _lowSpeedX1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _lowSpeedX1 = value;
            }
        }

        private float[] _lowSpeedY1 = new float[3] { 0, 10, 50 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("3.示教低速"), DisplayName("Y1示教低速"), Description("起始速度、最大速度、加速度")]
        public float[] LowSpeedY1
        {
            get => _lowSpeedY1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _lowSpeedY1 = value;
            }
        }

        private float[] _lowSpeedX2 = new float[3] { 0, 10, 50 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("3.示教低速"), DisplayName("X2示教低速"), Description("起始速度、最大速度、加速度")]
        public float[] LowSpeedX2
        {
            get => _lowSpeedX2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _lowSpeedX2 = value;
            }
        }

        private float[] _lowSpeedY2 = new float[3] { 0, 10, 50 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("3.示教低速"), DisplayName("Y2示教低速"), Description("起始速度、最大速度、加速度")]
        public float[] LowSpeedY2
        {
            get => _lowSpeedY2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _lowSpeedY2 = value;
            }
        }

        private float[] _lowSpeedZ2 = new float[3] { 0, 10, 50 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("3.示教低速"), DisplayName("Z2示教低速"), Description("起始速度、最大速度、加速度")]
        public float[] LowSpeedZ2
        {
            get => _lowSpeedZ2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _lowSpeedZ2 = value;
            }
        }

        private float[] _lowSpeedR1 = new float[3] { 0, 30, 100 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("3.示教低速"), DisplayName("R1示教低速"), Description("起始速度、最大速度、加速度")]
        public float[] LowSpeedR1
        {
            get => _lowSpeedR1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _lowSpeedR1 = value;
            }
        }

        private float[] _lowSpeedR2 = new float[3] { 0, 30, 100 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("3.示教低速"), DisplayName("R2示教低速"), Description("起始速度、最大速度、加速度")]
        public float[] LowSpeedR2
        {
            get => _lowSpeedR2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _lowSpeedR2 = value;
            }
        }

        private float[] _lowSpeedR3 = new float[3] { 0, 30, 100 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("3.示教低速"), DisplayName("R3示教低速"), Description("起始速度、最大速度、加速度")]
        public float[] LowSpeedR3
        {
            get => _lowSpeedR3;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _lowSpeedR3 = value;
            }
        }

        private float[] _lowSpeedR4 = new float[3] { 0, 30, 100 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("3.示教低速"), DisplayName("R4示教低速"), Description("起始速度、最大速度、加速度")]
        public float[] LowSpeedR4
        {
            get => _lowSpeedR4;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _lowSpeedR4 = value;
            }
        }

        #endregion

        #region 示教中速

        private float[] _midSpeedX1 = new float[3] { 5, 50, 500 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教中速"), DisplayName("X1示教中速"), Description("起始速度、最大速度、加速度")]
        public float[] MidSpeedX1
        {
            get => _midSpeedX1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _midSpeedX1 = value;
            }
        }

        private float[] _midSpeedY1 = new float[3] { 5, 50, 500 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教中速"), DisplayName("Y1示教中速"), Description("起始速度、最大速度、加速度")]
        public float[] MidSpeedY1
        {
            get => _midSpeedY1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _midSpeedY1 = value;
            }
        }

        private float[] _midSpeedX2 = new float[3] { 5, 50, 500 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教中速"), DisplayName("X2示教中速"), Description("起始速度、最大速度、加速度")]
        public float[] MidSpeedX2
        {
            get => _midSpeedX2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _midSpeedX2 = value;
            }
        }

        private float[] _midSpeedY2 = new float[3] { 5, 50, 500 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教中速"), DisplayName("Y2示教中速"), Description("起始速度、最大速度、加速度")]
        public float[] MidSpeedY2
        {
            get => _midSpeedY2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _midSpeedY2 = value;
            }
        }

        private float[] _midSpeedZ2 = new float[3] { 5, 50, 500 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教中速"), DisplayName("Z2示教中速"), Description("起始速度、最大速度、加速度")]
        public float[] MidSpeedZ2
        {
            get => _midSpeedZ2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _midSpeedZ2 = value;
            }
        }

        private float[] _midSpeedR1 = new float[3] { 5, 100, 1000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教中速"), DisplayName("R1示教中速"), Description("起始速度、最大速度、加速度")]
        public float[] MidSpeedR1
        {
            get => _midSpeedR1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _midSpeedR1 = value;
            }
        }

        private float[] _midSpeedR2 = new float[3] { 5, 100, 1000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教中速"), DisplayName("R2示教中速"), Description("起始速度、最大速度、加速度")]
        public float[] MidSpeedR2
        {
            get => _midSpeedR2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _midSpeedR2 = value;
            }
        }

        private float[] _midSpeedR3 = new float[3] { 5, 100, 1000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教中速"), DisplayName("R3示教中速"), Description("起始速度、最大速度、加速度")]
        public float[] MidSpeedR3
        {
            get => _midSpeedR3;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _midSpeedR3 = value;
            }
        }

        private float[] _midSpeedR4 = new float[3] { 5, 100, 1000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教中速"), DisplayName("R4示教中速"), Description("起始速度、最大速度、加速度")]
        public float[] MidSpeedR4
        {
            get => _midSpeedR4;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _midSpeedR4 = value;
            }
        }

        #endregion



        #region 示教高速

        private float[] _highSpeedX1 = new float[3] { 10, 100, 500 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教高速"), DisplayName("X1示教高速"), Description("起始速度、最大速度、加速度")]
        public float[] HighSpeedX1
        {
            get => _highSpeedX1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _highSpeedX1 = value;
            }
        }

        private float[] _highSpeedY1 = new float[3] { 10, 100, 500 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教高速"), DisplayName("Y1示教高速"), Description("起始速度、最大速度、加速度")]
        public float[] HighSpeedY1
        {
            get => _highSpeedY1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _highSpeedY1 = value;
            }
        }

        private float[] _highSpeedX2 = new float[3] { 10, 100, 500 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教高速"), DisplayName("X2示教高速"), Description("起始速度、最大速度、加速度")]
        public float[] HighSpeedX2
        {
            get => _highSpeedX2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _highSpeedX2 = value;
            }
        }

        private float[] _highSpeedY2 = new float[3] { 10, 100, 500 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教高速"), DisplayName("Y2示教高速"), Description("起始速度、最大速度、加速度")]
        public float[] HighSpeedY2
        {
            get => _highSpeedY2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _highSpeedY2 = value;
            }
        }

        private float[] _highSpeedZ2 = new float[3] { 10, 100, 500 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教高速"), DisplayName("Z2示教高速"), Description("起始速度、最大速度、加速度")]
        public float[] HighSpeedZ2
        {
            get => _highSpeedZ2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _highSpeedZ2 = value;
            }
        }

        private float[] _highSpeedR1 = new float[3] { 10, 200, 1000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教高速"), DisplayName("R1示教高速"), Description("起始速度、最大速度、加速度")]
        public float[] HighSpeedR1
        {
            get => _highSpeedR1;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _highSpeedR1 = value;
            }
        }

        private float[] _highSpeedR2 = new float[3] { 10, 200, 1000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教高速"), DisplayName("R2示教高速"), Description("起始速度、最大速度、加速度")]
        public float[] HighSpeedR2
        {
            get => _highSpeedR2;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _highSpeedR2 = value;
            }
        }

        private float[] _highSpeedR3 = new float[3] { 10, 200, 1000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教高速"), DisplayName("R3示教高速"), Description("起始速度、最大速度、加速度")]
        public float[] HighSpeedR3
        {
            get => _highSpeedR3;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _highSpeedR3 = value;
            }
        }

        private float[] _highSpeedR4 = new float[3] { 10, 200, 1000 };
        [Editor(typeof(HidePropertyViewArrayEdit), typeof(UITypeEditor))]
        [Category("4.示教高速"), DisplayName("R4示教高速"), Description("起始速度、最大速度、加速度")]
        public float[] HighSpeedR4
        {
            get => _highSpeedR4;
            set
            {
                if (value[0] < 0) value[0] = 0;
                if (value[0] > 30) value[0] = 30;
                if (value[1] < 0) value[1] = 0;
                if (value[1] > 500) value[1] = 500;
                if (value[2] < 0) value[2] = 0;
                if (value[2] > 4000) value[2] = 4000;
                _highSpeedR4 = value;
            }
        }

        #endregion

        #endregion

        #region 端口IO配置

        [Category("6.端口配置"), DisplayName("上相机扫码光源")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutUpLightScan { get; set; } = EN_GoogolExtendOutput.Out0;

        [Category("6.端口配置"), DisplayName("上相机拍照光源")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutUpLightPhoto { get; set; } = EN_GoogolExtendOutput.Out1;

        [Category("6.端口配置"), DisplayName("下相机光源")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutDownLight { get; set; } = EN_GoogolExtendOutput.Out2;

        [Category("6.端口配置"), DisplayName("1#气缸")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutCylinderDownNo1 { get; set; } = EN_GoogolExtendOutput.Out12;

        [Category("6.端口配置"), DisplayName("2#气缸")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutCylinderDownNo2 { get; set; } = EN_GoogolExtendOutput.Out13;

        [Category("6.端口配置"), DisplayName("3#气缸")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutCylinderDownNo3 { get; set; } = EN_GoogolExtendOutput.Out14;

        [Category("6.端口配置"), DisplayName("4#气缸")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutCylinderDownNo4 { get; set; } = EN_GoogolExtendOutput.Out15;

        [Category("6.端口配置"), DisplayName("1#真空吸")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutNozzleMaterialSucNo1 { get; set; } = EN_GoogolExtendOutput.Out4;

        [Category("6.端口配置"), DisplayName("2#真空吸")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutNozzleMaterialSucNo2 { get; set; } = EN_GoogolExtendOutput.Out6;

        [Category("6.端口配置"), DisplayName("3#真空吸")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutNozzleMaterialSucNo3 { get; set; } = EN_GoogolExtendOutput.Out8;

        [Category("6.端口配置"), DisplayName("4#真空吸")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutNozzleMaterialSucNo4 { get; set; } = EN_GoogolExtendOutput.Out10;

        [Category("6.端口配置"), DisplayName("1#真空破")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutNozzleMaterialVacBreakNo1 { get; set; } = EN_GoogolExtendOutput.Out5;

        [Category("6.端口配置"), DisplayName("2#真空破")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutNozzleMaterialVacBreakNo2 { get; set; } = EN_GoogolExtendOutput.Out7;

        [Category("6.端口配置"), DisplayName("3#真空破")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutNozzleMaterialVacBreakNo3 { get; set; } = EN_GoogolExtendOutput.Out9;

        [Category("6.端口配置"), DisplayName("4#真空破")]
        [ReadOnly(true)]
        public EN_GoogolExtendOutput OutNozzleMaterialVacBreakNo4 { get; set; } = EN_GoogolExtendOutput.Out11;

        [Category("6.端口配置"), DisplayName("1#气缸上到位")]
        [ReadOnly(true)]
        public EN_GoogolExtendInput InNozzleCylinderDownReadyNo1 { get; set; } = EN_GoogolExtendInput.In0;

        [Category("6.端口配置"), DisplayName("2#气缸上到位")]
        [ReadOnly(true)]
        public EN_GoogolExtendInput InNozzleCylinderDownReadyNo2 { get; set; } = EN_GoogolExtendInput.In1;

        [Category("6.端口配置"), DisplayName("3#气缸上到位")]
        [ReadOnly(true)]
        public EN_GoogolExtendInput InNozzleCylinderDownReadyNo3 { get; set; } = EN_GoogolExtendInput.In2;

        [Category("6.端口配置"), DisplayName("4#气缸上到位")]
        [ReadOnly(true)]
        public EN_GoogolExtendInput InNozzleCylinderDownReadyNo4 { get; set; } = EN_GoogolExtendInput.In3;

        [Category("6.端口配置"), DisplayName("1#吸到位")]
        [ReadOnly(true)]
        public EN_GoogolExtendInput InNozzleMaterialSucReadyNo1 { get; set; } = EN_GoogolExtendInput.In4;

        [Category("6.端口配置"), DisplayName("2#吸到位")]
        [ReadOnly(true)]
        public EN_GoogolExtendInput InNozzleMaterialSucReadyNo2 { get; set; } = EN_GoogolExtendInput.In5;

        [Category("6.端口配置"), DisplayName("3#吸到位")]
        [ReadOnly(true)]
        public EN_GoogolExtendInput InNozzleMaterialSucReadyNo3 { get; set; } = EN_GoogolExtendInput.In6;

        [Category("6.端口配置"), DisplayName("4#吸到位")]
        [ReadOnly(true)]
        public EN_GoogolExtendInput InNozzleMaterialSucReadyNo4 { get; set; } = EN_GoogolExtendInput.In7;

        #endregion

        public List<float[]> GetSpeeds(En_SpeedType speed)
        {
            List<float[]> ret = new List<float[]>();
            switch (speed)
            {
                case En_SpeedType.Low:
                    ret.Add(LowSpeedX1);
                    ret.Add(LowSpeedY1);
                    //ret.Add(LowSpeedZ1);
                    ret.Add(LowSpeedX2);
                    ret.Add(LowSpeedY2);
                    ret.Add(LowSpeedZ2);
                    ret.Add(LowSpeedR1);
                    ret.Add(LowSpeedR2);
                    ret.Add(LowSpeedR3);
                    ret.Add(LowSpeedR4);
                    break;
                case En_SpeedType.Mid:
                    ret.Add(MidSpeedX1);
                    ret.Add(MidSpeedY1);

                    //ret.Add(MidSpeedZ1);
                    ret.Add(MidSpeedX2);
                    ret.Add(MidSpeedY2);
                    ret.Add(MidSpeedZ2);
                    ret.Add(MidSpeedR1);
                    ret.Add(MidSpeedR2);
                    ret.Add(MidSpeedR3);
                    ret.Add(MidSpeedR4);
                    break;
                case En_SpeedType.High:
                    ret.Add(HighSpeedX1);
                    ret.Add(HighSpeedY1);

                    //ret.Add(HighSpeedZ1);
                    ret.Add(HighSpeedX2);
                    ret.Add(HighSpeedY2);
                    ret.Add(HighSpeedZ2);
                    ret.Add(HighSpeedR1);
                    ret.Add(HighSpeedR2);
                    ret.Add(HighSpeedR3);
                    ret.Add(HighSpeedR4);
                    break;
                case En_SpeedType.Work:
                    ret.Add(WorkSpeedX1);
                    ret.Add(WorkSpeedY1);
                    //ret.Add(WorkSpeedZ1);
                    ret.Add(WorkSpeedX2);
                    ret.Add(WorkSpeedY2);
                    ret.Add(WorkSpeedZ2);
                    ret.Add(WorkSpeedR1);
                    ret.Add(WorkSpeedR2);
                    ret.Add(WorkSpeedR3);
                    ret.Add(WorkSpeedR4);
                    break;
            }
            return ret;
        }
        public float[] GetSpeed(En_AxisNum axis, En_SpeedType speed)
        {
            var speeds = GetSpeeds(speed);
            float[] ret = new float[3];
            switch (axis)
            {
                case En_AxisNum.X1:
                    ret = speeds[0];
                    break;
                case En_AxisNum.Y1:
                    ret = speeds[1];
                    break;
                case En_AxisNum.X2:
                    ret = speeds[2];
                    break;
                case En_AxisNum.Y2:
                    ret = speeds[3];
                    break;
                case En_AxisNum.Z2:
                    ret = speeds[4];
                    break;
                case En_AxisNum.R1:
                    ret = speeds[5];
                    break;
                case En_AxisNum.R2:
                    ret = speeds[6];
                    break;
                case En_AxisNum.R3:
                    ret = speeds[7];
                    break;
                case En_AxisNum.R4:
                    ret = speeds[8];
                    break;
            }
            return ret;
        }
    }
}
