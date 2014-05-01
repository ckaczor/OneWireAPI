using System;
using System.Runtime.InteropServices;

namespace OneWireAPI
{
	public class TMEX
	{
		#region TMEX function enumerations

		// Size of the global state buffer
		public enum TMStateBufferSize
		{
			NoEPROMWriting	=	5120,
			EPROMWriting	=	15360
		}

		// Type of operation for the TMOneWireLevel function
		public enum TMOneWireLevelOperation : short
		{
			Write	=	0,
			Read	=	1
		}	

		// TMOneWireLevel operation mode
		public enum TMOneWireLevelMode : short
		{
			Normal			=	0x0,
			StrongPullup	=	0x1,
			Break			=	0x2,
			ProgramVoltage	=	0x3
		}

		// When the mode from the TMOneWireLevel is to activate
		public enum TMOneWireLevelPrime : short
		{
			Immediate		=	0,
			AfterNextBit	=	1,
			AfterNextByte	=	2
		}

		// Type of CRC to be calculated
		public enum TMCRCType : short
		{
			EightBit	=	0,
			SixteenBit	=	1
		}

		#endregion

		#region TMEX function structures

		[StructLayoutAttribute(LayoutKind.Sequential, Pack=1)]
		public struct FileEntry
		{
			[MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]	
			public byte[]		Name;								/* four-character file name */
			public byte			Extension;							/* extension number, range 0 - 99, 127 */
			public byte			StartPage;							/* page number where file starts */
			public byte			PageCount;							/* number of pages occupied by file */ 
			public byte			Attributes;							/* file/directory attribute */
			[MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
			public byte[]		Bitmap;								/* current bitmap of the device */
		}

		#endregion

		#region TMEX DLL function imports

		[DllImport("IBFS64.DLL")]
		public static extern short Get_Version(byte[] baVersion);

        [DllImport("IBFS64.DLL")]
		public static extern int TMExtendedStartSession(short nPortNumber, short nPortType, IntPtr nEnhancedOptions);

        [DllImport("IBFS64.DLL")]
		public static extern short TMEndSession(int iSession);

        [DllImport("IBFS64.DLL")]
		public static extern short TMReadDefaultPort(out short nPortNumber, out short nPortType);

        [DllImport("IBFS64.DLL")]
		public static extern short TMSetup(int iSession);

        [DllImport("IBFS64.DLL")]
		public static extern short TMClose(int iSession);

        [DllImport("IBFS64.DLL")]
		public static extern short TMRom(int iSession, byte[] bStateBuffer, short[] nROM);

        [DllImport("IBFS64.DLL")]
		public static extern short TMAccess(int iSession, byte[] bStateBuffer);

        [DllImport("IBFS64.DLL")]
		public static extern short TMOneWireLevel(int iSession, TMOneWireLevelOperation nOperation, TMOneWireLevelMode nLevelMode, TMOneWireLevelPrime nPrimed);

        [DllImport("IBFS64.DLL")]
		public static extern short TMTouchBit(int iSession, short nOutput);

        [DllImport("IBFS64.DLL")]
		public static extern short TMTouchByte(int iSession, short nOutput);

        [DllImport("IBFS64.DLL")]
		public static extern short TMTouchReset(int iSession);

        [DllImport("IBFS64.DLL")]
		public static extern short TMBlockStream(int iSession, byte[] aData, short nByteCount);

        [DllImport("IBFS64.DLL")]
		public static extern short TMBlockIO(int iSession, byte[] aData, short nByteCount);

        [DllImport("IBFS64.DLL")]
		public static extern short TMCRC(short nLength, byte[] aData, ushort nSeed, TMCRCType nType);

        [DllImport("IBFS64.DLL")]
		public static extern short TMFirst(int iSession, byte[] bStateBuffer);

        [DllImport("IBFS64.DLL")]
		public static extern short TMNext(int iSession, byte[] bStateBuffer);

        [DllImport("IBFS64.DLL")]
		public static extern short TMFirstFile(int iSession, byte[] bStateBuffer, ref FileEntry uFileEntry);

        [DllImport("IBFS64.DLL")]
		public static extern short TMOpenFile(int iSession, byte[] bStateBuffer, ref FileEntry uFileEntry);

        [DllImport("IBFS64.DLL")]
		public static extern short TMReadFile(int iSession, byte[] bStateBuffer, short nFileHandle, byte[] baReadBuffer, short nBufferSize);

        [DllImport("IBFS64.DLL")]
		public static extern short TMCloseFile(int iSession, byte[] bStateBuffer, short nFileHandle);

		#endregion
	}
}
