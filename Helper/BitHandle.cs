using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class BitHandle
    {
        /// <summary>
        /// Bit0 based index
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitPosition">Bit0 based index</param>
        /// <returns></returns>
        public static bool GetBit(int value, int bitPosition)
        {
            return (value & (1 << bitPosition)) != 0;
        }
        /// <summary>
        /// Bit0 based index
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitPosition">Bit0 based index</param>
        public static void SetBit(ref int value, int bitPosition)
        {
            value |= 1 << bitPosition;
        }
        /// <summary>
        /// Bit0 based index
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitPosition">Bit0 based index</param>
        public static void ResetBit(ref int value, int bitPosition)
        {
            value &= ~(1 << bitPosition);
        }
    }
}
