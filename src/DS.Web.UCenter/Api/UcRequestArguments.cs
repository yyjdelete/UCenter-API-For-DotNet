using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace DS.Web.UCenter.Api
{
    /// <summary>
    /// Requser����
    /// Dozer ��Ȩ����
    /// �����ơ��޸ģ����뱣���ҵ���ϵ��ʽ��
    /// http://www.dozer.cc
    /// mailto:dozer.cc@gmail.com
    /// </summary>
    public class UcRequestArguments: IUcRequestArguments
    {
        /// <summary>
        /// Action
        /// </summary>
        public string Action { get;private set; }

        private string Code { get; set; }

        /// <summary>
        /// ʱ��
        /// </summary>
        public long Time { get; private set; }

        /// <summary>
        /// Query����
        /// </summary>
        public NameValueCollection QueryString { get; private set; }

        private string _formData;
        /// <summary>
        /// Form����
        /// </summary>
        public string FormData
        {
            get
            {
                //TODO: �޸������ض�����(��������)ʱ���������
                if (_formData == null)
                    _formData = HttpUtility.UrlDecode(request.Form.ToString(), UcConfig.UcEncoding);
                return _formData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInvalidRequest { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAuthracationExpiried { get; private set; }

        private readonly HttpRequest request;
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="request">Request</param>
        public UcRequestArguments(HttpRequest request)
        {
            this.request = request;
            Code = request.QueryString["code"];
            //FormData = HttpUtility.UrlDecode(request.Form.ToString(), UcConfig.UcEncoding);
            QueryString = HttpUtility.ParseQueryString(UcUtility.AuthCodeDecode(Code));
            Action = QueryString["action"];
            if (Action != null)
                Action = Action.ToLowerInvariant();
            long time;
            if (long.TryParse(QueryString["time"], out time)) Time = time;
            IsInvalidRequest = request.QueryString.Count == 0 || Action == null;
            IsAuthracationExpiried = (UcUtility.PhpTimeNow() - Time) > 0xe10;
        }
    }
}

