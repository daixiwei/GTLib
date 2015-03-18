using System;
using System.Collections.Generic;
using System.Reflection;

namespace com.gt.units
{
    /// <summary>
    /// 
    /// </summary>
    public static class GTUnit
    {
        /// <summary>
        /// 
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// 
        /// </summary>
        public const float Rad2Deg = 57.2958f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="probability"></param>
        /// <returns></returns>
        public static bool GetProbability(int probability)
        {
            int rand = GetRand(1, 100);
            if (rand <= probability)
                return true;
            return false;
        }

        /// <summary>
        /// 产生随机种子
        /// </summary>
        /// <returns></returns>
        public static int SwapSeed()
        {
            int key = Guid.NewGuid().GetHashCode();
            random = new Random(key);
            return key;
        }

        /// <summary>
        /// 设置随机种子
        /// </summary>
        /// <param name="key"></param>
        public static void SetSeed(int key)
        {
            random = new Random(key);
        }

        /// <summary>
        /// 获得(0-range)随机数
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int GetRand(int range)
        {
            return GetRand(0, range);
        }

        /// <summary>
        /// 获得(min-max)随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRand(int min, int max)
        {
            int ranInt = random.Next(min, max + 1);
            return ranInt;
        }

        /// <summary>
        /// 随机List数组下标
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T GetRand<T>(List<T> array)
        {
            int index = GetRand(0, array.Count - 1);
            return array[index];
        }

        /// <summary>
        /// 检测字符串是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullString(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 切割字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="splitStr"></param>
        /// <returns></returns>
        public static string[] SplitStr(string source, string splitStr)
        {
            if (IsNullString(source) || IsNullString(splitStr))
            {
                return null;
            }
            string[] returnStrArr = source.Split(new string[] { splitStr }, StringSplitOptions.None);
            return returnStrArr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(object f) where T : Attribute
        {
            FieldInfo EnumInfo = f.GetType().GetField(f.ToString());
            object[] tem = EnumInfo.GetCustomAttributes(typeof(T), false);
            if (tem != null && tem.Length > 0)
            {
                return (T)tem[0];
            }
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static string SecondToHMS(int second)
        {
            int h = second / 3600;

            second -= h * 3600;

            int m = second / 60;

            second -= m * 60;

            int s = second;

            return AmendTimeString(h) + ":" + AmendTimeString(m) + ":" + AmendTimeString(s);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeType"></param>
        /// <param name="spaceMark"></param>
        /// <returns></returns>
        public static string TimeToString(float time,TimeType timeType, string spaceMark)
        {
            System.Text.StringBuilder app = new System.Text.StringBuilder();

            bool first = false;

            if ((timeType & TimeType.Hour) != 0)
            {
                first = true;
                int h = (int)(time / 3600);
                time -= h * 3600;
                app.Append(AmendTimeString(h));

            }
            if ((timeType & TimeType.Minute) != 0)
            {
                if (first)
                    app.Append(spaceMark);
                first = true;
                int m = (int)(time / 60);
                time -= m * 60;
                app.Append(AmendTimeString(m));

            }
            if ((timeType & TimeType.Second) != 0)
            {
                if (first)
                    app.Append(spaceMark);
                app.Append(AmendTimeString((int)(time)));
            }
            return app.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string AmendTimeString(int number)
        {
            if (number < 10)
            {
                return "0" + number;
            }
            return number + "";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TimeType
    {
        Hour = 2,
        Minute = 4,
        Second = 8
    }
}
