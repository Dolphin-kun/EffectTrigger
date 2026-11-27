using YukkuriMovieMaker.ItemEditor;

namespace EffectTrigger.Display
{
    internal class RangeValueDisplayAttribute : CustomDisplayAttributeBase
    {
        public override string? GetGroupName(object instance) => "条件";

        public override string? GetName(object instance) => IsRange(instance) ? "下限" : "値";

        public override string? GetDescription(object instance) => IsRange(instance) ? "範囲の下限" : "比べる値";

        public override int? GetOrder(object instance) => null;

        public override bool? GetAutoGenerateField(object instance) => null;

        public override bool? GetAutoGenerateFilter(object instance) => null;

        static bool IsRange(object instance) => instance is TriggerEffect.IfAnimationModeBase { IsRange: true };
    }
}
