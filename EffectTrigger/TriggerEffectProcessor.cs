using EffectTrigger.Enum;
using System.Collections.Immutable;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace EffectTrigger
{
    internal class TriggerEffectProcessor : IVideoEffectProcessor
    {
        readonly IGraphicsDevicesAndContext devices;
        readonly TriggerEffect item;

        ID2D1Image? input;
        ID2D1Image? output;

        int? triggerFrame = null;
        private bool triggerHit;
        readonly List<(IVideoEffect effect, IVideoEffectProcessor processor)> chain = [];

        public ID2D1Image Output => output ?? input ?? throw new NullReferenceException("No valid output image");

        public TriggerEffectProcessor(IGraphicsDevicesAndContext devices, TriggerEffect item)
        {
            this.devices = devices;
            this.item = item;
            UpdateChain(item.Effects);
        }

        private void UpdateChain(ImmutableList<IVideoEffect> effects)
        {
            var disposedIndex = chain
                .Where(t => !effects.Contains(t.effect))
                .Select(t => chain.IndexOf(t))
                .OrderByDescending(i => i);

            foreach (int index in disposedIndex)
            {
                chain[index].processor.Dispose();
                chain.RemoveAt(index);
            }

            var kept = chain.Select(t => t.effect).ToList();
            var newChain = new List<(IVideoEffect, IVideoEffectProcessor)>(effects.Count);

            foreach (var effect in effects)
            {
                int index = kept.IndexOf(effect);
                if (index >= 0)
                {
                    newChain.Add(chain[index]);
                }
                else
                {
                    newChain.Add((effect, effect.CreateVideoEffect(devices)));
                }
            }

            chain.Clear();
            chain.AddRange(newChain);
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            DrawDescription desc = effectDescription.DrawDescription;

            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var resetFrame = item.ResetFrame;
            var slider1 = item.GetSlider1Value(frame, length, fps);
            var probability = item.Probability.GetValue(frame, length, fps);

            float target = item.Enum_IfMode switch
            {
                IfMode.X => desc.Draw.X,
                IfMode.Y => desc.Draw.Y,
                IfMode.Z => desc.Draw.Z,
                IfMode.Opacity => (float)desc.Opacity * 100f,
                IfMode.ZoomX => desc.Zoom.X * 100f,
                IfMode.ZoomY => desc.Zoom.Y * 100f,
                IfMode.RotationX => desc.Rotation.X,
                IfMode.RotationY => desc.Rotation.Y,
                IfMode.RotationZ => desc.Rotation.Z,
                _ => throw new ArgumentOutOfRangeException(nameof(effectDescription), effectDescription, null)
            };
            
            bool condition = item.Enum_SignMode.Compare(target, (float)slider1);
            if (condition)
            {
                if (triggerFrame == null)
                {
                    triggerFrame = frame;

                    float r = PseudoRandom01(frame);

                    triggerHit =
                        probability >= 100 ||
                        (probability > 0 && r * 100f < probability);
                }

                if (!triggerHit)
                {
                    output = input;
                    return desc;
                }

                int relFrame = Math.Max(0, frame - (resetFrame ? triggerFrame.Value : 0));

                EffectDescription chainEffectDescription = new(
                     new TimelineItemSourceDescription(
                         effectDescription,
                         relFrame,
                         length,
                         effectDescription.Layer),
                     desc,
                     effectDescription.InputIndex,
                     effectDescription.InputCount,
                     effectDescription.GroupIndex,
                     effectDescription.GroupCount
                 );


                UpdateChain(item.Effects);

                ID2D1Image? current = input;


                foreach (var (effect, processor) in chain)
                {
                    if (!effect.IsEnabled) continue;

                    processor.SetInput(current);
                    desc = processor.Update(chainEffectDescription);
                    current = processor.Output;
                }

                output = current;
                return desc;
            }
            else
            {
                triggerFrame = null;
                triggerHit = false;
                output = input;
                return desc;
            }
        }

        private static float PseudoRandom01(int seed)
        {
            unchecked
            {
                uint x = (uint)seed;
                x ^= x << 13;
                x ^= x >> 17;
                x ^= x << 5;
                return (x & 0x00FFFFFF) / 16777216f; // 0.0 ～ 1.0
            }
        }


        public void SetInput(ID2D1Image? input)
        {
            this.input = input;
            output = input;
        }

        public void ClearInput()
        {
            input = null;
            output = null;

            foreach (var (_, processor) in chain)
            {
                processor.ClearInput();
            }
        }

        public void Dispose()
        {
            foreach (var (_, processor) in chain)
            {
                processor.Dispose();
            }
            chain.Clear();
        }
    }
}
