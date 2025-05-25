using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Trigger.Display;
using Trigger.Enum;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace Trigger
{
    [VideoEffect("エフェクトトリガー", ["描画"], ["Effect Trigger","エフェクト トリガー"], isAviUtlSupported: false)]
    internal class TriggerEffect : VideoEffectBase
    {
        public override string Label => "エフェクトトリガー";

        [Display(GroupName = "条件", Name = "", Description = "")]
        [DisplayControl]
        public string DescriptionText { get => descriptionText; set => Set(ref descriptionText, value); }
        string descriptionText = "X が 0 に等しいとき";

        //--------------------------
        [Display(AutoGenerateField = true)]
        public ImmutableList<IfAnimationModePx> ModesPx { get => modesPx; set => Set(ref modesPx, value); }
        ImmutableList<IfAnimationModePx> modesPx = [];

        [Display(AutoGenerateField = true)]
        public ImmutableList<IfAnimationModeOpacityPercentage> ModesOpacityPercentage { get => modesOpacityPercentage; set => Set(ref modesOpacityPercentage, value); }
        ImmutableList<IfAnimationModeOpacityPercentage> modesOpacityPercentage = [];

        [Display(AutoGenerateField = true)]
        public ImmutableList<IfAnimationModeZoomPercentage> ModesZoomPercentage { get => modesZoomPercentage; set => Set(ref modesZoomPercentage, value); }
        ImmutableList<IfAnimationModeZoomPercentage> modesZoomPercentage = [];

        [Display(AutoGenerateField = true)]
        public ImmutableList<IfAnimationModeAngle> ModesAngle { get => modesAngle; set => Set(ref modesAngle, value); }
        ImmutableList<IfAnimationModeAngle> modesAngle = [];
        //--------------------------

        [Display(GroupName = "条件", Name = "対象", Description = "対象")]
        [EnumComboBox]
        public IfMode Enum_IfMode { get => enum_IfMode; set => Set(ref enum_IfMode, value); }
        IfMode enum_IfMode = IfMode.X;

        [Display(GroupName = "条件", Name = "条件", Description = "条件")]
        [EnumComboBox]
        public SignMode Enum_SignMode { get => enum_SignMode; set => Set(ref enum_SignMode, value); }
        SignMode enum_SignMode = SignMode.Equal;

        [Display(GroupName = "条件", Name = "リセット", Description = "条件を満たすたびにエフェクトの適用")]
        [ToggleSlider]
        public bool ResetFrame { get => resetFrame; set => Set(ref resetFrame, value); }
        bool resetFrame = true;

        [Display(GroupName = "実行エフェクト", Name = "", Description = "")]
        [VideoEffectSelector(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<IVideoEffect> Effects { get => effects; set => Set(ref effects, value); }
        ImmutableList<IVideoEffect> effects = [];

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new TriggerEffectProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [..Effects, ..ModesPx, ..ModesOpacityPercentage, ..ModesZoomPercentage, ..ModesAngle];

        public override async ValueTask EndEditAsync()
        {
            await base.EndEditAsync();

            switch (Enum_IfMode)
            {
                case IfMode.X:
                case IfMode.Y:
                case IfMode.Z:
                    ModesPx = modesPx.IsEmpty ? [new IfAnimationModePx()] : ModesPx;
                    ModesOpacityPercentage = [];
                    ModesZoomPercentage = [];
                    ModesAngle = [];
                    break;
                case IfMode.Opacity:
                    ModesPx = [];
                    ModesOpacityPercentage = ModesOpacityPercentage.IsEmpty ? [new IfAnimationModeOpacityPercentage()] : ModesOpacityPercentage;
                    ModesZoomPercentage = [];
                    ModesAngle = [];
                    break;
                case IfMode.ZoomX:
                case IfMode.ZoomY:
                    ModesPx = [];
                    ModesOpacityPercentage = [];
                    ModesZoomPercentage = ModesZoomPercentage.IsEmpty ? [new IfAnimationModeZoomPercentage()] : ModesZoomPercentage;
                    ModesAngle = [];
                    break;
                case IfMode.RotationX:
                case IfMode.RotationY:
                case IfMode.RotationZ:
                    ModesPx = [];
                    ModesOpacityPercentage = [];
                    ModesZoomPercentage = [];
                    ModesAngle = ModesAngle.IsEmpty ? [new IfAnimationModeAngle()] : ModesAngle;
                    break;
                default:
                    ModesPx = [];
                    ModesOpacityPercentage = [];
                    ModesZoomPercentage = [];
                    ModesAngle = [];
                    break;
            }

            var val = GetSlider1Value(0, 1, 60);
            DescriptionText = Enum_SignMode.ToDisplayString(Enum_IfMode,val);
        }

        public double GetSlider1Value(int frame, int length, int fps)
        {
            return Enum_IfMode switch
            {
                IfMode.X or IfMode.Y or IfMode.Z => ModesPx.FirstOrDefault()?.Value.GetValue(frame, length, fps) ?? 0,
                IfMode.Opacity => ModesOpacityPercentage.FirstOrDefault()?.Value.GetValue(frame, length, fps) ?? 0,
                IfMode.ZoomX or IfMode.ZoomY => ModesZoomPercentage.FirstOrDefault()?.Value.GetValue(frame, length, fps) ?? 0,
                IfMode.RotationX or IfMode.RotationY or IfMode.RotationZ => ModesAngle.FirstOrDefault()?.Value.GetValue(frame, length, fps) ?? 0,
                _ => 0
            };
        }

        //座標px
        public class IfAnimationModePx : Animatable
        {
            [Display(GroupName = "条件", Name = "", Description = "")]
            [AnimationSlider("F1", "px", -500.0, 500.0)]
            public Animation Value { get; } = new(0, -99999.0, 99999.0);
            protected override IEnumerable<IAnimatable> GetAnimatables() => [Value];
        }

        //不透明度%
        public class IfAnimationModeOpacityPercentage : Animatable
        {
            [Display(GroupName = "条件", Name = "", Description = "")]
            [AnimationSlider("F1", "%", 0.0, 100.0)]
            public Animation Value { get; } = new(100, 0, 100);
            protected override IEnumerable<IAnimatable> GetAnimatables() => [Value];
        }

        //拡大率%
        public class IfAnimationModeZoomPercentage : Animatable
        {
            [Display(GroupName = "条件", Name = "", Description = "")]
            [AnimationSlider("F1", "%", 0.0, 400.0)]
            public Animation Value { get; } = new(100, 0, 5000.0);
            protected override IEnumerable<IAnimatable> GetAnimatables() => [Value];
        }

        //回転角°
        public class IfAnimationModeAngle : Animatable
        {
            [Display(GroupName = "条件", Name = "", Description = "")]
            [AnimationSlider("F1", "°", -360.0, 360.0)]
            public Animation Value { get; } = new(0, -36000.0, 36000.0);
            protected override IEnumerable<IAnimatable> GetAnimatables() => [Value];
        }
    }
}
