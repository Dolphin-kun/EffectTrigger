using EffectTrigger.Display;
using EffectTrigger.Enum;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.ItemEditor.CustomVisibilityAttributes;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;
using YukkuriMovieMaker.Project;
using YukkuriMovieMaker.Settings;

namespace EffectTrigger
{
    [VideoEffect("エフェクトトリガー", [VideoEffectCategories.Drawing], ["Effect Trigger", "エフェクト トリガー"], isAviUtlSupported: false)]
    internal class TriggerEffect : VideoEffectBase
    {
        public override string Label => "エフェクトトリガー";

        [Display(GroupName = "条件", Name = "", Description = "")]
        [DisplayControl]
        public string DescriptionText { get => descriptionText; set => Set(ref descriptionText, value); }
        string descriptionText = "X が 0 に等しいとき";

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

        [Display(GroupName = "条件", Name = "確率", Description = "確率")]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation Probability { get; } = new(100, 0, 100);

        [Display(GroupName = "画像", Name = "画像", Description = "条件を満たしたときに差し替える画像")]
        [FileSelector(FileGroupType.ImageItem, FileType = FileType.画像)]
        public string ImagePath { get; set => Set(ref field, value); } = "";

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

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Probability, .. Effects, .. ModesPx, .. ModesOpacityPercentage, .. ModesZoomPercentage, .. ModesAngle];

        public override IEnumerable<TimelineResource> GetResources()
        {
            foreach (var resource in base.GetResources())
                yield return resource;

            if (TimelineResource.TryParseFromPath(ImagePath, TimelineResourceType.Image, out var imageResource))
                yield return imageResource;

            foreach (var effect in Effects)
            {
                foreach (var resource in effect.GetResources())
                    yield return resource;
            }
        }

        public override IEnumerable<string> GetFiles()
        {
            foreach (var file in base.GetFiles())
                yield return file;

            if (!string.IsNullOrEmpty(ImagePath))
                yield return ImagePath;

            foreach (var effect in Effects)
            {
                foreach (var file in effect.GetFiles())
                    yield return file;
            }
        }

        public override void ReplaceFile(string from, string to)
        {
            base.ReplaceFile(from, to);

            if (ImagePath == from)
                ImagePath = to;

            foreach (var effect in Effects)
                effect.ReplaceFile(from, to);
        }

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

            var isRange = Enum_SignMode == SignMode.Range;
            IEnumerable<IfAnimationModeBase> modes = [.. ModesPx, .. ModesOpacityPercentage, .. ModesZoomPercentage, .. ModesAngle];
            if (modes.Any(mode => mode.IsRange != isRange))
            {
                foreach (var mode in modes)
                    mode.IsRange = isRange;

                ModesPx = [.. ModesPx];
                ModesOpacityPercentage = [.. ModesOpacityPercentage];
                ModesZoomPercentage = [.. ModesZoomPercentage];
                ModesAngle = [.. ModesAngle];
            }

            var val = GetSlider1Value(0, 1, 60);
            var max = GetSlider2Value(0, 1, 60);
            DescriptionText = Enum_SignMode.ToDisplayString(Enum_IfMode, val, max);
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

        public double GetSlider2Value(int frame, int length, int fps)
        {
            return Enum_IfMode switch
            {
                IfMode.X or IfMode.Y or IfMode.Z => ModesPx.FirstOrDefault()?.MaxValue.GetValue(frame, length, fps) ?? 0,
                IfMode.Opacity => ModesOpacityPercentage.FirstOrDefault()?.MaxValue.GetValue(frame, length, fps) ?? 0,
                IfMode.ZoomX or IfMode.ZoomY => ModesZoomPercentage.FirstOrDefault()?.MaxValue.GetValue(frame, length, fps) ?? 0,
                IfMode.RotationX or IfMode.RotationY or IfMode.RotationZ => ModesAngle.FirstOrDefault()?.MaxValue.GetValue(frame, length, fps) ?? 0,
                _ => 0
            };
        }

        public abstract class IfAnimationModeBase : Animatable
        {
            public bool IsRange { get; set => Set(ref field, value); }
        }

        //座標px
        public class IfAnimationModePx : IfAnimationModeBase
        {
            [RangeValueDisplay]
            [AnimationSlider("F1", "px", -500.0, 500.0)]
            public Animation Value { get; } = new(0, -99999.0, 99999.0);

            [Display(GroupName = "条件", Name = "上限", Description = "範囲の上限")]
            [AnimationSlider("F1", "px", -500.0, 500.0)]
            [ShowPropertyEditorWhen(nameof(IsRange), true)]
            public Animation MaxValue { get; } = new(100, -99999.0, 99999.0);

            protected override IEnumerable<IAnimatable> GetAnimatables() => [Value, MaxValue];
        }

        //不透明度%
        public class IfAnimationModeOpacityPercentage : IfAnimationModeBase
        {
            [RangeValueDisplay]
            [AnimationSlider("F1", "%", 0.0, 100.0)]
            public Animation Value { get; } = new(100, 0, 100);

            [Display(GroupName = "条件", Name = "上限", Description = "範囲の上限")]
            [AnimationSlider("F1", "%", 0.0, 100.0)]
            [ShowPropertyEditorWhen(nameof(IsRange), true)]
            public Animation MaxValue { get; } = new(100, 0, 100);

            protected override IEnumerable<IAnimatable> GetAnimatables() => [Value, MaxValue];
        }

        //拡大率%
        public class IfAnimationModeZoomPercentage : IfAnimationModeBase
        {
            [RangeValueDisplay]
            [AnimationSlider("F1", "%", 0.0, 400.0)]
            public Animation Value { get; } = new(100, 0, 5000.0);

            [Display(GroupName = "条件", Name = "上限", Description = "範囲の上限")]
            [AnimationSlider("F1", "%", 0.0, 400.0)]
            [ShowPropertyEditorWhen(nameof(IsRange), true)]
            public Animation MaxValue { get; } = new(200, 0, 5000.0);

            protected override IEnumerable<IAnimatable> GetAnimatables() => [Value, MaxValue];
        }

        //回転角°
        public class IfAnimationModeAngle : IfAnimationModeBase
        {
            [RangeValueDisplay]
            [AnimationSlider("F1", "°", -360.0, 360.0)]
            public Animation Value { get; } = new(0, -36000.0, 36000.0);

            [Display(GroupName = "条件", Name = "上限", Description = "範囲の上限")]
            [AnimationSlider("F1", "°", -360.0, 360.0)]
            [ShowPropertyEditorWhen(nameof(IsRange), true)]
            public Animation MaxValue { get; } = new(90, -36000.0, 36000.0);

            protected override IEnumerable<IAnimatable> GetAnimatables() => [Value, MaxValue];
        }
    }
}
