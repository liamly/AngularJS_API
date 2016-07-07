using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSocketApp
{
    class ClassMsgSendRecv
    {
        private string msClientIPAddress;
        private string msMsgData;
        private string msRemark;

        public ClassMsgSendRecv()
        {
            msClientIPAddress = "";
            msMsgData = "";
            msRemark = "";
        }

        public ClassMsgSendRecv(string sMsgID, string sMsgData)
        {
            msClientIPAddress= sMsgID;
            msMsgData = sMsgData;
        }

        public string IPAddress
        {
            get
            {
                return msClientIPAddress;
            }
            set
            {
                msClientIPAddress = value;
            }
        }

        public string MsgData
        {
            get
            {
                return msMsgData;
            }
            set
            {
                msMsgData = value;
            }
        }

        public string Remark
        {
            get
            {
                return msRemark;
            }
            set
            {
                msRemark = value;
            }
        }
    }
}
