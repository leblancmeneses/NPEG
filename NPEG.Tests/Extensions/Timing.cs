using System;
using System.Diagnostics;

namespace NPEG.Tests.Extensions
{
  public class Timing
  {
    public static Stopwatch TimedFor(Action action, Int32 loops)
    {
      var sw = new Stopwatch();
      sw.Start();
      for (int i = 0; i < loops; ++i)
      {
        action.Invoke();
      }
      sw.Stop();

      return sw;
    }
  }
}
