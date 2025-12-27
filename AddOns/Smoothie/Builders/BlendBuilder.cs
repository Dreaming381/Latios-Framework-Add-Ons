using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

// Note: I kinda like the API this produces, but I do not like the architecture. This is a temporary solution that should be redesigned.
// It would be especially awesome if there was some way to describe a template, and then swap out parts of that template at runtime so
// that we could use fast instantiation. Or maybe we need an InstantiateArchetypeCommandBuffer?

namespace Latios.Smoothie
{
    public static partial class Blend
    {
        public static BlendBuilderTarget<float> Target<TComponent>(Entity entity, ref TComponent component, ref float floatField) where TComponent : unmanaged, IComponentData
        {
            return new BlendBuilderTarget<float>
            {
                description = new BlendBuilderDescription<float>
                {
                    targetBinding = new EntityComponentBinding
                    {
                        entity  = entity,
                        binding = ComponentBinding.Create(ref component, ref floatField)
                    }
                }
            };
        }
    }

    public struct BlendBuilderTarget<TOutput> where TOutput : unmanaged
    {
        internal BlendBuilderDescription<TOutput> description;

        public BlendBuilderFrom<TOutput> From(TOutput fromConstant)
        {
            description.startConstant                          = fromConstant;
            description.useStartBinding                        = false;
            return new BlendBuilderFrom<TOutput> { description = description };
        }

        public BlendBuilderFrom<TOutput> From<TComponent>(Entity sourceEntity, ref TComponent component, ref TOutput fromField) where TComponent : unmanaged, IComponentData
        {
            description.startBinding = new EntityComponentBinding
            {
                entity  = sourceEntity,
                binding = ComponentBinding.Create(ref component, ref fromField)
            };
            description.useStartBinding                        = true;
            return new BlendBuilderFrom<TOutput> { description = description };
        }
    }

    public struct BlendBuilderFrom<TOutput> where TOutput : unmanaged
    {
        internal BlendBuilderDescription<TOutput> description;

        public BlendBuilderTo<TOutput> To(TOutput toConstant)
        {
            description.endConstant                          = toConstant;
            description.useEndBinding                        = false;
            return new BlendBuilderTo<TOutput> { description = description };
        }

        public BlendBuilderTo<TOutput> To<TComponent>(Entity sourceEntity, ref TComponent component, ref TOutput toField) where TComponent : unmanaged, IComponentData
        {
            description.endBinding = new EntityComponentBinding
            {
                entity  = sourceEntity,
                binding = ComponentBinding.Create(ref component, ref toField)
            };
            description.useEndBinding                        = true;
            return new BlendBuilderTo<TOutput> { description = description };
        }
    }

    public struct BlendBuilderTo<TOutput> where TOutput : unmanaged
    {
        internal BlendBuilderDescription<TOutput> description;

        public BlendBuilderDuration<TOutput> Linearly()
        {
            description.curveFunction                              = InstructionSet.CurveFunction.Passthrough;
            return new BlendBuilderDuration<TOutput> { description = description };
        }

        public BlendBuilderDuration<TOutput> Smoothstepped()
        {
            description.curveFunction                              = InstructionSet.CurveFunction.Smoothstep;
            return new BlendBuilderDuration<TOutput> { description = description };
        }
    }

    public struct BlendBuilderDuration<TOutput> where TOutput : unmanaged
    {
        internal BlendBuilderDescription<TOutput> description;

        public BlendBuilderBuild<TOutput> OverDuration(float seconds)
        {
            description.duration                                = seconds;
            return new BlendBuilderBuild<TOutput> { description = description };
        }
    }

    public struct BlendBuilderBuild<TOutput> where TOutput : unmanaged
    {
        internal BlendBuilderDescription<TOutput> description;

        public Entity Build(ref EntityCommandBuffer.ParallelWriter ecb, int sortKey)
        {
            BlendInstructions instructions = default;
            InstructionSet.SetProcedure(ref instructions, InstructionSet.Procedure.ProgressCurveInterpolate);
            InstructionSet.SetUsesIncompleteFlag(ref instructions, true);
            InstructionSet.SetInterpolationStartIsBinding(ref instructions, description.useStartBinding);
            InstructionSet.SetInterpolationEndIsBinding(ref instructions, description.useEndBinding);
            InstructionSet.SetCurveFunction(ref instructions, description.curveFunction);
            var                              entity     = ecb.CreateEntity(sortKey);
            FixedList128Bytes<ComponentType> typesToAdd = new TypePack<Progression, Duration, EnabledFlag, IncompleteFlag, OutputBinding>();
            typesToAdd.Add(ComponentType.ReadWrite<OutputEntity>());
            switch (description.targetBinding.binding.primitiveType)
            {
                case BindingPrimitiveType.Float:
                    typesToAdd.Add(ComponentType.ReadWrite<OutputFloat>());
                    if (!description.useStartBinding)
                        typesToAdd.Add(ComponentType.ReadWrite<ConstantStartFloat>());
                    if (!description.useEndBinding)
                        typesToAdd.Add(ComponentType.ReadWrite<ConstantEndFloat>());
                    InstructionSet.SetInterpolatedOutputType(ref instructions, InstructionSet.InterpolatedOutputType.Float);
                    break;
            }
            if (description.useStartBinding)
                typesToAdd.Add(ComponentType.ReadWrite<ComponentBindingStart>());
            if (description.useEndBinding)
                typesToAdd.Add(ComponentType.ReadWrite<ComponentBindingEnd>());

            switch (description.curveFunction)
            {
                case InstructionSet.CurveFunction.Passthrough:
                case InstructionSet.CurveFunction.Smoothstep:
                    break;
            }

            // Add this first because if we set it last, Unity might do a change-chunk-in-place and fragment our chunks really bad.
            ecb.AddSharedComponent(sortKey, entity, instructions);

            ecb.AddComponent(sortKey, entity, new ComponentTypeSet(in typesToAdd));
            ecb.SetComponent(sortKey, entity, new Duration { duration     = description.duration });
            ecb.SetComponent(sortKey, entity, new OutputEntity { entity   = description.targetBinding.entity });
            ecb.SetComponent(sortKey, entity, new OutputBinding { binding = description.targetBinding.binding });
            switch (description.targetBinding.binding.primitiveType)
            {
                case BindingPrimitiveType.Float:
                    if (!description.useStartBinding)
                        ecb.SetComponent(sortKey, entity, UnsafeUtility.As<TOutput, ConstantStartFloat>(ref description.startConstant));
                    if (!description.useEndBinding)
                        ecb.SetComponent(sortKey, entity, UnsafeUtility.As<TOutput, ConstantEndFloat>(ref description.endConstant));
                    break;
            }

            if (description.useStartBinding)
                ecb.SetComponent(sortKey, entity, new ComponentBindingStart { binding = description.startBinding });
            if (description.useEndBinding)
                ecb.SetComponent(sortKey, entity, new ComponentBindingEnd { binding = description.endBinding });

            return entity;
        }
    }

    internal struct BlendBuilderDescription<TOutput> where TOutput : unmanaged
    {
        public EntityComponentBinding       targetBinding;
        public EntityComponentBinding       startBinding;
        public TOutput                      startConstant;
        public bool                         useStartBinding;
        public EntityComponentBinding       endBinding;
        public TOutput                      endConstant;
        public bool                         useEndBinding;
        public InstructionSet.CurveFunction curveFunction;
        public float                        duration;
    }
}

