using System;
using System.IO;

namespace Assets.VirtualProfiler
{
    public interface IStreamAdapter : IDisposable
    {
        MemoryStream WriteToStream(MemoryStream buffer);
    }
}
