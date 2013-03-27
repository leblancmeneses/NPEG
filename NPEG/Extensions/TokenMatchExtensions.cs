using System.Text;

namespace NPEG.Extensions
{
	public static class TokenMatchExtensions
	{
		public static string ValueAsString(this TokenMatch match, IInputIterator iterator)
		{
			var matchedBytes = iterator.Text(match.Start, match.End);
			return Encoding.UTF8.GetString(matchedBytes, 0, matchedBytes.Length);
		}

		public static byte[] ValueAsBytes(this TokenMatch match, IInputIterator iterator)
		{
			return iterator.Text(match.Start, match.End);
		}
	}
}
