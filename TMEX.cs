using System;
using System.Runtime.InteropServices;

namespace OneWireAPI
{
    // ReSharper disable once InconsistentNaming
    public class TMEX
    {
        // Size of the global state buffer
        public enum StateBufferSize
        {
            NoEpromWriting = 5120,
            EpromWriting = 15360
        }

        // Type of operation for the TMOneWireLevel function
        public enum LevelOperation : short
        {
            Write = 0,
            Read = 1
        }

        // TMOneWireLevel operation mode
        public enum LevelMode : short
        {
            Normal = 0x0,
            StrongPullup = 0x1,
            Break = 0x2,
            ProgramVoltage = 0x3
        }

        // When the mode from the TMOneWireLevel is to activate
        public enum LevelPrime : short
        {
            Immediate = 0,
            AfterNextBit = 1,
            AfterNextByte = 2
        }

        // Type of CRC to be calculated
        public enum CrcType : short
        {
            EightBit = 0,
            SixteenBit = 1
        }

        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
        public struct FileEntry
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Name;								        /* four-character file name */
            public byte Extension;							        /* extension number, range 0 - 99, 127 */
            public byte StartPage;							        /* page number where file starts */
            public byte PageCount;							        /* number of pages occupied by file */
            public byte Attributes;							        /* file/directory attribute */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Bitmap;								    /* current bitmap of the device */
        }

        [DllImport("IBFS64.DLL")]
        public static extern short Get_Version(byte[] version);

        [DllImport("IBFS64.DLL")]
        public static extern int TMExtendedStartSession(short portNumber, short portType, IntPtr enhancedOptions);

        [DllImport("IBFS64.DLL")]
        public static extern short TMEndSession(int session);

        [DllImport("IBFS64.DLL")]
        public static extern short TMReadDefaultPort(out short portNumber, out short portType);

        [DllImport("IBFS64.DLL")]
        public static extern short TMSetup(int session);

        [DllImport("IBFS64.DLL")]
        public static extern short TMClose(int session);

        [DllImport("IBFS64.DLL")]
        public static extern short TMRom(int session, byte[] stateBuffer, short[] rom);

        [DllImport("IBFS64.DLL")]
        public static extern short TMAccess(int session, byte[] stateBuffer);

        [DllImport("IBFS64.DLL")]
        public static extern short TMOneWireLevel(int session, LevelOperation operation, LevelMode levelMode, LevelPrime primed);

        [DllImport("IBFS64.DLL")]
        public static extern short TMTouchBit(int session, short output);

        [DllImport("IBFS64.DLL")]
        public static extern short TMTouchByte(int session, short output);

        [DllImport("IBFS64.DLL")]
        public static extern short TMTouchReset(int session);

        [DllImport("IBFS64.DLL")]
        public static extern short TMBlockStream(int session, byte[] data, short byteCount);

        [DllImport("IBFS64.DLL")]
        public static extern short TMBlockIO(int session, byte[] data, short byteCount);

        [DllImport("IBFS64.DLL")]
        public static extern short TMCRC(short length, byte[] data, ushort seed, CrcType type);

        [DllImport("IBFS64.DLL")]
        public static extern short TMFirst(int session, byte[] stateBuffer);

        [DllImport("IBFS64.DLL")]
        public static extern short TMNext(int session, byte[] stateBuffer);

        [DllImport("IBFS64.DLL")]
        public static extern short TMFirstFile(int session, byte[] stateBuffer, ref FileEntry fileEntry);

        [DllImport("IBFS64.DLL")]
        public static extern short TMOpenFile(int session, byte[] stateBuffer, ref FileEntry fileEntry);

        [DllImport("IBFS64.DLL")]
        public static extern short TMReadFile(int session, byte[] stateBuffer, short fileHandle, byte[] readBuffer, short bufferSize);

        [DllImport("IBFS64.DLL")]
        public static extern short TMCloseFile(int session, byte[] stateBuffer, short fileHandle);
    }
}
