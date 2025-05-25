using System.ComponentModel.DataAnnotations;

namespace Trigger.Enum
{
    public enum SignMode
    {
        [Display(Name = "=", Description = "AがBに等しい")]
        Equal = 1,

        [Display(Name = "≠", Description = "AがBと等しくない")]
        NotEqual = 2,

        [Display(Name = ">", Description = "AがBより大きい")]
        GreaterThan = 3,

        [Display(Name = "<", Description = "AがBより小さい")]
        LessThan = 4,

        [Display(Name = "≥", Description = "AがB以上")]
        GreaterThanOrEqual = 5,

        [Display(Name = "≤", Description = "AがB以下")]
        LessThanOrEqual = 6,
    }
}
