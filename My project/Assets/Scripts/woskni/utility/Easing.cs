using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class Easing
    {
        public static Color Linear(float time, float limit, Color min, Color max)
        {
            min.r = Linear(time, limit, min.r, max.r);
            min.g = Linear(time, limit, min.g, max.g);
            min.b = Linear(time, limit, min.b, max.b);
            min.a = Linear(time, limit, min.a, max.a);

            return min;
        }

        public static Vector3 Linear(float time, float limit, Vector3 min, Vector3 max)
        {
            min.x = Linear(time, limit, min.x, max.x);
            min.y = Linear(time, limit, min.y, max.y);
            min.z = Linear(time, limit, min.z, max.z);

            return min;
        }

        public static float Linear(float time, float limit, float min, float max)
        {
            max -= min;
            time /= limit;

            return max * time + min;
        }

        public static float InQuad(float time, float limit, float min, float max)
        {
            max -= min;
            time /= limit;

            return max * time * time + min;
        }

        public static float OutQuad(float time, float limit, float min, float max)
        {
            max -= min;
            time /= limit;

            return -max * time * (time - 2f) + min;
        }

        public static float InOutQuad(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit / 2f;

            if (time < 1f)
                return max / 2f * time * time + min;

            time -= 1f;

            return -max / 2f * (time * (time - 2f) - 1f) + min;
        }

        public static float InCubic(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit;

            return max * time * time * time + min;
        }

        public static Color InCubic(float time, float limit, Color min, Color max)
        {
            min.r = InCubic(time, limit, min.r, max.r);
            min.g = InCubic(time, limit, min.g, max.g);
            min.b = InCubic(time, limit, min.b, max.b);
            min.a = InCubic(time, limit, min.a, max.a);

            return min;
        }

        public static Vector3 InCubic(float time, float limit, Vector3 min, Vector3 max)
        {
            min.x = InCubic(time, limit, min.x, max.x);
            min.y = InCubic(time, limit, min.y, max.y);
            min.z = InCubic(time, limit, min.z, max.z);

            return min;
        }

        public static float OutCubic(float time, float limit, float min, float max)
        {
            max -= min;

            time = time / limit - 1f;

            return max * (time * time * time + 1f) + min;
        }
        public static float InOutCubic(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit / 2f;

            if (time < 1f)

                return max / 2f * time * time * time + min;

            time -= 2f;

            return max / 2f * (time * time * time + 2f) + min;
        }
        public static float InQuart(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit;

            return max * time * time * time * time + min;
        }
        public static float OutQuart(float time, float limit, float min, float max)
        {
            max -= min;

            time = time / limit - 1f;

            return -max * (time * time * time * time - 1f) + min;
        }
        public static float InOutQuart(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit / 2f;

            if (time < 1f)
                return max / 2f * time * time * time * time + min;

            time -= 2f;

            return -max / 2f * (time * time * time * time - 2f) + min;
        }
        public static float InQuintic(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit;

            return max * time * time * time * time * time + min;
        }

        public static Vector3 InQuintic(float time, float limit, Vector3 min, Vector3 max)
        {
            min.x = InQuintic(time, limit, min.x, max.x);
            min.y = InQuintic(time, limit, min.y, max.y);
            min.z = InQuintic(time, limit, min.z, max.z);

            return min;
        }

        public static float OutQuintic(float time, float limit, float min, float max)
        {
            max -= min;

            time = time / limit - 1f;

            return max * (time * time * time * time * time + 1f) + min;
        }
        public static float InOutQuintic(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit / 2f;

            if (time < 1f)
                return max / 2f * time * time * time * time * time + min;

            time -= 2f;

            return max / 2f * (time * time * time * time * time + 2f) + min;
        }
        public static float InSine(float time, float limit, float min, float max)
        {
            max -= min;

            return -max * Mathf.Cos(time * Mathf.Deg2Rad * 90f / limit) + max + min;
        }

        public static Vector3 InSine(float time, float limit, Vector3 min, Vector3 max)
        {
            min.x = InSine(time, limit, min.x, max.x);
            min.y = InSine(time, limit, min.y, max.y);
            min.z = InSine(time, limit, min.z, max.z);

            return min;
        }

        public static float OutSine(float time, float limit, float min, float max)
        {
            max -= min;

            return max * Mathf.Sin(time * Mathf.Deg2Rad * 90f / limit) + min;
        }
        public static float InOutSine(float time, float limit, float min, float max)
        {
            max -= min;

            return -max / 2f * (Mathf.Sin(time * Mathf.PI / limit) - 1f) + min;
        }
        public static float InExp(float time, float limit, float min, float max)
        {
            max -= min;

            return time == 0f ? min : max * Mathf.Pow(2f, 10f * (time / limit - 1f)) + min;
        }
        public static float OutExp(float time, float limit, float min, float max)
        {
            max -= min;

            return time == limit ? max + min : max * (-Mathf.Pow(2f, -10f * time / limit) + 1) + min;
        }
        public static float InOutExp(float time, float limit, float min, float max)
        {
            if (time == 0f) return min;

            if (time == limit) return max;

            max -= min;

            time /= limit / 2f;

            if (time < 1f)
                return max / 2f * Mathf.Pow(2f, 10f * (time - 1f)) + min;

            time -= 1f;

            return max / 2f * (-Mathf.Pow(2f, -10f * time) + 2f) + min;
        }
        public static float InCirc(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit;

            return -max * (Mathf.Sqrt(1f - time * time) - 1f) + min;
        }
        public static float OutCirc(float time, float limit, float min, float max)
        {
            max -= min;

            time = time / limit - 1f;

            return max * Mathf.Sqrt(1f - time * time) + min;
        }
        public static Vector3 OutCirc(float time, float limit, Vector3 min, Vector3 max)
        {
            min.x = OutCirc(time, limit, min.x, max.x);
            min.y = OutCirc(time, limit, min.y, max.y);
            min.z = OutCirc(time, limit, min.z, max.z);

            return min;
        }
        public static float InOutCirc(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit / 2f;

            if (time < 1f)
                return -max / 2f * (Mathf.Sqrt(1f - time * time) - 1f) + min;

            time -= 2f;

            return max / 2f * (Mathf.Sqrt(1f - time * time) + 1f) + min;
        }
        public static float OutBounce(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit;

            if (time < 1f / 2.75f)
            {
                return max * (7.5625f * time * time) + min;
            }
            else if (time < 2f / 2.75f)
            {
                time -= 1.5f / 2.75f;

                return max * (7.5625f * time * time + 0.75f) + min;
            }
            else if (time < 2.5f / 2.75f)
            {
                time -= 2.25f / 2.75f;

                return max * (7.5625f * time * time + 0.9375f) + min;
            }
            else
            {
                time -= 2.625f / 2.75f;

                return max * (7.5625f * time * time + 0.984375f) + min;
            }
        }
        public static float InBounce(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit;

            return max * Mathf.Pow(2f, 6f * (time - 1f)) * Mathf.Abs(Mathf.Sin(time * Mathf.PI * 3.5f)) + min;

            //return max - OutBounce(limit - time, limit, 0f, max - min) + min;
        }
        public static float InOutBounce(float time, float limit, float min, float max)
        {
            max -= min;

            time /= limit;

            if (time < 0.5f)
                return max * 8f * Mathf.Pow(2f, 8f * (time - 1f)) * Mathf.Abs(Mathf.Sin(time * Mathf.PI * 7f)) + min;
            else
                return max * (1f - 8f * Mathf.Pow(2f, -8f * time) * Mathf.Abs(Mathf.Sin(time * Mathf.PI * 7f))) + min;

            //if (time < limit / 2f)
            //    return InBounce(time * 2f, limit, min, max) * 0.5f + min;
            //else
            //    return OutBounce(time * 2f - limit, limit, 0f, max ) * 0.5f + (max - min) * 0.5f;
        }
        public static float OutBack(float time, float limit, float min, float max, float s)
        {
            max -= min;

            time = time / limit - 1f;

            return max * (time * time * ((s + 1f) * time + s) + 1f) + min;
        }
        public static float InBack(float time, float limit, float min, float max, float s = 2f)
        {
            max -= min;

            time /= limit;

            return max * time * time * ((s + 1f) * time - s) + min;
        }

        public static Vector3 InBack(float time, float limit, Vector3 min, Vector3 max, float s = 2f)
        {
            Vector3 ans = Vector3.zero;

            ans.x = InBack(time, limit, min.x, max.y, s);
            ans.y = InBack(time, limit, min.y, max.y, s);
            ans.z = InBack(time, limit, min.z, max.z, s);

            return ans;
        }
        public static float InOutBack(float time, float limit, float min, float max, float s)
        {
            max -= min;

            s *= 1.525f;

            time /= limit / 2f;

            if (time < 1f)
                return max / 2f * (time * time * ((s + 1f) * time - s)) + min;

            time -= 2;

            return max / 2f * (time * time * ((s + 1f) * time + s) + 2f) + min;
        }
    }
}
