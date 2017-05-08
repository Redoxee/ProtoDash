#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("b9izYlEow32m8jS4EXqPxz4o3mZy4g6OIjaIQUGR/uZl5RVGpXKIQD/t/QZHGg9uKvqT6jQsyyAt7D82+pSjTZIoc+zvZmJL0k6VhkoQY+1Aw83C8kDDyMBAw8PCUHoEEtFn+en8LsiskVXucRWA3etbuYa/OiFvHCxOs2zXUQY3Itdx+MbQM5XnfBlsozyq2guPhNCCXhIctuShVNYkOvHI7JsEjbBxT8o4rvL0ZH8L5X418kDD4PLPxMvoRIpENc/Dw8PHwsFGA9C2Csb9uf394JCJ5fcIEjAGg/KfHgtcNMU6jPowUHZ6+Zv4Q1Tb6PfKtmIyzJrXeqsjqiuQPUVIQBYvr268z61nSvu9ovolKOgu/YUpP0ydifK1pg8wZ8DBw8LD");
        private static int[] order = new int[] { 6,8,5,11,8,13,10,12,12,10,10,13,12,13,14 };
        private static int key = 194;

        public static byte[] Data() {
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
