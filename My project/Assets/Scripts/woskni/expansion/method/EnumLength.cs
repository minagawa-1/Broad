using System.Linq;

namespace woskni
{
    /// <summary>
    /// �񋓌^�Ɋւ���ėp�N���X
    /// </summary>
    public static class Enum
    {
        private static readonly System.Random m_Rand = new System.Random();  // ����

        /// <summary>�w�肳�ꂽ�񋓌^�̃����_���ȗ񋓎q��Ԃ�</summary>
        public static T Random<T>()
        {
            return System.Enum.GetValues(typeof(T))
                .Cast<T>()
                .OrderBy(c => m_Rand.Next())
                .FirstOrDefault();
        }

        /// <summary>�񋓌^�̗񋓎q�̐���Ԃ�</summary>
        public static int GetLength<T>()
        {
            return System.Enum.GetValues(typeof(T)).Length;
        }
    }
}