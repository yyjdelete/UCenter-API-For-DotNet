namespace DS.Web.UCenter
{
    /// <summary>
    /// 用户注册Model
    /// </summary>
    public class UcUserRegister
    {
        /// <summary>
        /// Uid, 若成功(大于0), 为用户uid, 若注册不成功(小于等于0), 则为错误码
        /// </summary>
        public int Uid { get;private set; }
        /// <summary>
        /// 注册结果
        /// </summary>
        public RegisterResult Result
        {
            get { return Uid > 0 ? RegisterResult.Success : (RegisterResult)Uid; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xml">数据</param>
        public  UcUserRegister(string xml)
        {
            var result = 0;
            int.TryParse(xml, out result);
            Uid = result;
        }
    }

}
