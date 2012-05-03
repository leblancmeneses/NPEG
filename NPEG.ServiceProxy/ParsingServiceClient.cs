using System;
using System.ServiceModel;

// namespace of service reference added

namespace Npeg.ServiceProxy.ParsingService
{
	public partial class ParsingServiceClient : IDisposable
	{
		#region IDisposable Members

		void IDisposable.Dispose()
		{
			//If faulted we will call abort. Otherewise, if not already closed, call close;
			if (State == CommunicationState.Faulted)
				Abort();
			else if (State != CommunicationState.Closed)
				Close();
		}

		#endregion

		public static ParsingServiceClient Create()
		{
			return new ParsingServiceClient();
		}
	}
}