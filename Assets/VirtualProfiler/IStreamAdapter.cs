using System;
using System.IO;

namespace Assets.VirtualProfiler
{
    public interface IStreamAdapter : IDisposable
    {
        int WriteToStream(MemoryStream buffer);
    }
}
