using UnityEngine;
using System.Collections;

public static class ExtendEnum
{
    public static T ToEnum<T>(this string value)
    {
        // + 변환 오류인 경우 디폴트값 리턴. (디폴트값 : 0번째 value)
        if (!System.Enum.IsDefined(typeof(T), value))
            return default(T);

        return (T)System.Enum.Parse(typeof(T), value, true);
    }
}
