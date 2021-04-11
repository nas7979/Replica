#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("nLDXlHcnHzfYqgNkdrHQwPC0D1fXNIi9gyPx5LsvSU9ueY+DuSk5PbAV584tO3IWRMg41RjJ/jQR6YXzH5d249YO31a8N+Of8x4OKXfRsFv9HIc7j+g2nqX2qsQwrSmsJAzjO26ehlXkNEYPCGfOkKFBSlkaU/6Hvv13jZZVFoRluACFxeCXySBkVLG8Do2uvIGKhaYKxAp7gY2NjYmMjw6Ng4y8Do2Gjg6NjYwargX8uUpMLLLOBPaHjaJBVCMH6HgK8Rh70RWgw4e4w7x2zYB4M3ZQr8vF0DYaD3kVuvIg2Y/Hn5Tx1otOBD+9EMHXDlY1CAVGbWbvPVrOujGyErDAE2RzAk2Jy49L39SZ7hpztHVPHBC3QGUH97d392Umn46PjYyN");
        private static int[] order = new int[] { 11,6,2,13,10,9,10,11,10,9,10,11,13,13,14 };
        private static int key = 140;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
