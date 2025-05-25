using Trigger.Enum;

namespace Trigger
{
    public static class SignModeExtensions
    {
        public static bool Compare(this SignMode mode, float a, float b)
        {
            return mode switch
            {
                SignMode.Equal => a == b,
                SignMode.NotEqual => a != b,
                SignMode.GreaterThan => a > b,
                SignMode.LessThan => a < b,
                SignMode.GreaterThanOrEqual => a >= b,
                SignMode.LessThanOrEqual => a <= b,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }


        public static string ToDisplayString(this SignMode signMode, IfMode ifMode, double value)
        {
            string propName = ifMode switch
            {
                IfMode.X => "X",
                IfMode.Y => "Y",
                IfMode.Z => "Z",
                IfMode.Opacity => "不透明度",
                IfMode.ZoomX => "拡大率X",
                IfMode.ZoomY => "拡大率Y",
                IfMode.RotationX => "回転角X",
                IfMode.RotationY => "回転角Y",
                IfMode.RotationZ => "回転角Z",
                _ => "値"
            };

            string unit = ifMode switch
            {
                IfMode.X or IfMode.Y or IfMode.Z => "px",
                IfMode.Opacity => "%",
                IfMode.RotationX or IfMode.RotationY or IfMode.RotationZ => "°",
                IfMode.ZoomX or IfMode.ZoomY => "%",
                _ => ""
            };

            string valueWithUnit = $"{value}{unit}";

            string condition = signMode switch
            {
                SignMode.Equal => $"が {valueWithUnit} に等しいとき",
                SignMode.NotEqual => $"が {valueWithUnit} と等しくないとき",
                SignMode.GreaterThan => $"が {valueWithUnit} より大きいとき",
                SignMode.LessThan => $"が {valueWithUnit} より小さいとき",
                SignMode.GreaterThanOrEqual => $"が {valueWithUnit} 以上のとき",
                SignMode.LessThanOrEqual => $"が {valueWithUnit} 以下のとき",
                _ => ""
            };

            return $"{propName} {string.Format(condition, value)}";
        }

    }
}
