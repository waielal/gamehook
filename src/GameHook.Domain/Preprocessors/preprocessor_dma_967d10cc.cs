using GameHook.Domain.Interfaces;

namespace GameHook.Domain.Preprocessors
{
    public static partial class Preprocessors
    {
        public static PreprocessorPropertyResult dma_967d10cc(MemoryAddress memoryAddress, int size, int offset, MemoryAddressBlockResult memoryAddressBlockResult)
        {
            var dmaMemoryAddressBytes = memoryAddressBlockResult.GetRelativeAddress(memoryAddress, size);
            var dmaMemoryAddressValue = ValueTransformers.UnsignedIntegerTransformer.ToValue(dmaMemoryAddressBytes);

            // Check to see if the DMA pointer references 0x00.
            // This can happen if the game is in the middle of a reset.
            if (dmaMemoryAddressValue == 0)
            {
                return new PreprocessorPropertyResult() { Address = null };
            }

            var actualMemoryAddress = (MemoryAddress)(dmaMemoryAddressValue + offset);

            return new PreprocessorPropertyResult()
            {
                Address = actualMemoryAddress
            };
        }
    }
}
