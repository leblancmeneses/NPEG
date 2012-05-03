using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

// namespace of service reference added
namespace Npeg.ServiceProxy.ParsingService
{
    public partial class ParsingServiceClient : IDisposable
    {
        public static ParsingServiceClient Create()
        {
            return new ParsingServiceClient();
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            //If faulted we will call abort. Otherewise, if not already closed, call close;
            if (this.State == CommunicationState.Faulted)
                this.Abort();
            else if (this.State != CommunicationState.Closed)
                this.Close();

        }

        #endregion
    }
}
