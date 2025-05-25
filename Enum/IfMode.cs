using System.ComponentModel.DataAnnotations;

namespace Trigger.Enum
{
    public enum IfMode
    {
        [Display(Name = "X", Description = "描画位置（横方向）")]
        X = 1,
        [Display(Name = "Y", Description = "描画位置（縦方向）")]
        Y = 2,
        [Display(Name = "Z", Description = "描画位置（奥行き）")]
        Z = 3,
        [Display(Name = "不透明度", Description = "不透明度")]
        Opacity = 4,
        [Display(Name = "拡大率X", Description = "拡大率（横方向）")]
        ZoomX = 5,
        [Display(Name = "拡大率Y", Description = "拡大率（縦方向）")]
        ZoomY =6 ,
        [Display(Name = "回転角X", Description = "縦方向、X軸に対する回転角")]
        RotationX = 7,
        [Display(Name = "回転角Y", Description = "横方向、Y軸に対する回転角")]
        RotationY = 8,
        [Display(Name = "回転角Z", Description = "平面方向、Z軸に対する回転角")]
        RotationZ = 9,
    }
}
