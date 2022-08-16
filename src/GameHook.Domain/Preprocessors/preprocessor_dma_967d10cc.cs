using GameHook.Domain.Interfaces;

namespace GameHook.Domain.Preprocessors
{
    public static partial class Preprocessors
    {
        public static PreprocessorPropertyResult dma_967d10cc(MemoryAddress memoryAddress, int size, int offset, MemoryAddressBlockResult memoryAddressBlockResult)
        {
            var dmaMemoryAddressBytes = memoryAddressBlockResult.GetRelativeAddress(memoryAddress, size);
            var dmaMemoryAddressValue = ValueTransformers.UnsignedIntegerTransformer.ToValue(dmaMemoryAddressBytes);

            var actualMemoryAddress = (MemoryAddress)(dmaMemoryAddressValue + offset);

            return new PreprocessorPropertyResult()
            {
                Address = actualMemoryAddress
            };
        }
    }
}
