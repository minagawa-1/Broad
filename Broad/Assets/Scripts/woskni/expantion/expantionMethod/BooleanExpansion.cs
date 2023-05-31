public static class BooleanExpansion
{
    public static int ToInt(this bool sorce) => System.Convert.ToInt32(sorce);

    public static bool Random() => UnityEngine.Random.value > 0.5f;
}