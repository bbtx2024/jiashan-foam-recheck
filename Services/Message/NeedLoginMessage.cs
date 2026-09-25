//20250716 刷卡
using QA.Business.Manager;

namespace QA.Business.Message
{
    public class NeedLoginMessage
    {
        public EN_UserType TargetUserType { get; set; } = EN_UserType.Operator;
    }
}
