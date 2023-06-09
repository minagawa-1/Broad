namespace woskni
{
    public class Base64Encode
    {
        public static readonly System.Text.Encoding encoding = System.Text.Encoding.UTF8;

        /// <summary>Base64でエンコード</summary>
        public static string Encode(string source) => System.Convert.ToBase64String(encoding.GetBytes(source));

        /// <summary>Base64でデコード</summary>
        public static string Decode(string source) => encoding.GetString(System.Convert.FromBase64String(source));
    }
}