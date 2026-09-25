using System;
using System.ComponentModel;
using QA.Business.Interfaces;
using QA_Infrastructure;

namespace QA.Business.Component.DashBoard
{
    [SaveParam(FileType.Binary)]
    [Serializable]
    [Description("DashBoard设置")]
    public class DashBoardParam : IParam
    {
        public bool BUse { get; set; }

        public void SetAllReadOnly(bool value)
        {
            throw new NotImplementedException();
        }

        public bool SetReadOnly(string propertyName, bool value)
        {
            throw new NotImplementedException();
        }

    }
}
