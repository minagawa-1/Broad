public static class BooleanExpansion
{
    public static int ToInt(this bool sorce) => System.Convert.ToInt32(sorce);

    public static bool Random(float rate = 0.5f) => UnityEngine.Random.value < rate;
}