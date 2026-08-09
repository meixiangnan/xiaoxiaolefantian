namespace Watermelon.Message
{
    public class MD5Helper
    {
        //加盐hash
        //salt为固定字符串"KJL"
        public static string MD5WithSalt(string str)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str + "KJL"));
            string md5Str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                md5Str += bytes[i].ToString("x2");
            }
            return md5Str;
        }
    }
}