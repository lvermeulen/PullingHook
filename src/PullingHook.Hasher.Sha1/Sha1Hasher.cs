using System.Security.Cryptography;

namespace PullingHook.Hasher.Sha1
{
    public class Sha1Hasher : HasherBase
    {
        protected override HashAlgorithm HashAlgorithm => SHA1.Create();
    }
}
