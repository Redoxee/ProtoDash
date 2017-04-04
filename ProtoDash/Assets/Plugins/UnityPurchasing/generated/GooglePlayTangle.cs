#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("FaLJGCtSuQfciE7CawD1vURSpByIOrmaiLW+sZI+8D5Ptbm5ub24u4jlZHEmTr9A9oBKKgwAg+GCOS6hi7KW4X73ygs1sELUiI4eBXGfBE8W2UbQoHH1/qr4JGhmzJ7bLqxeQEWXh3w9YHUUUIDpkE5WsVpXlkVMCJh09FhM8js764ScH59vPN8I8jpV1RTGtdcdMIHH2IBfUpJUh/9TRTx5qsxwvIfDh4ea6vOfjXJoSnz5ZlY0yRatK3xNWK0LgryqSe+dBmOA7tk36FIJlpUcGDGoNO/8MGoZl5KNsMwYSLbgrQDRWdBR6kc/MjpsOrm3uIg6ubK6Orm5uCoAfmirHYOThlSy1usvlAtv+qeRIcP8xUBbFTbn84jP3HVKHbq7ubi5");
        private static int[] order = new int[] { 6,6,2,7,12,5,8,7,9,10,11,12,12,13,14 };
        private static int key = 184;

        public static byte[] Data() {
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
