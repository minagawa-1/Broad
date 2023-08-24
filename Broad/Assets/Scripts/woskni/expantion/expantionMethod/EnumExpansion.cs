using System.Linq;

namespace woskni
{
    /// <summary>
    /// 列挙型に関する汎用クラス
    /// </summary>
    public static class Enum
    {
        private static readonly System.Random m_Rand = new System.Random();  // 乱数

        /// <summary>指定された列挙型のランダムな列挙子を返す</summary>
        public static T AtRandom<T>() where T : System.Enum
        {
            return System.Enum.GetValues(typeof(T))
                .Cast<T>()
                .OrderBy(c => m_Rand.Next())
                .FirstOrDefault();
        }

        /// <summary>列挙型の列挙子の数を返す</summary>
        public static int GetLength<T>() where T : System.Enum
        {
            return System.Enum.GetValues(typeof(T)).Length;
        }
    }
}