using System;

namespace OneWireAPI
{
	public class owAdapter 
	{
		#region Member variables

		private static owSession		m_oSession;			// The current session
		private static owIdentifier		m_oLastID;			// Last ID selected

		#endregion

		#region Methods

		public static void Initialize(owSession Session)
		{
			// Store the session we are dealing with
			m_oSession = Session;
		}

		public static void Select(owIdentifier ID)
		{
			// Set the ID of the device we want to talk to
			short nResult = TMEX.TMRom(m_oSession.SessionHandle, m_oSession.StateBuffer, ID.RawID);

			// Check the result
			if (nResult != 1)
			{
				// Throw a ROM exception
				throw new owException(owException.owExceptionFunction.Select, ID, nResult);
			}

			// Copy the ID as the last selected ID
			m_oLastID = ID;

			// Access the device
			Access();
		}

		public static void Access()
		{
			// Attempt to access the device
			short nResult = TMEX.TMAccess(m_oSession.SessionHandle, m_oSession.StateBuffer);

			// Check to see if we could access the device
			if (nResult != 1)
			{
				// Throw an access exception
				throw new owException(owException.owExceptionFunction.Access, m_oLastID, nResult);
			}
		}

		public static short SendBlock(byte[] Data, short ByteCount)
		{
			// Send the block and return the result
			short nResult = TMEX.TMBlockStream(m_oSession.SessionHandle, Data, ByteCount);

			// Check to see if the bytes sent matches the value returned
			if (nResult != ByteCount) 
			{
				// Throw an access exception
				throw new owException(owException.owExceptionFunction.SendBlock, m_oLastID, nResult);
			}

			// Return the result
			return nResult;
		}

		public static short SendBlock(byte[] Data, short ByteCount, bool Reset)
		{
			short	nResult;

			// Send the block and return the result
			if (Reset)
				nResult = TMEX.TMBlockStream(m_oSession.SessionHandle, Data, ByteCount);
			else
				nResult = TMEX.TMBlockIO(m_oSession.SessionHandle, Data, ByteCount);

			// Check to see if the bytes sent matches the value returned
			if (nResult != ByteCount) 
			{
				// Throw an access exception
				throw new owException(owException.owExceptionFunction.SendBlock, m_oLastID, nResult);
			}

			// Return the result
			return nResult;
		}

		public static short ReadBit()
		{
			// Send the byte and get back what was sent
			short nResult = TMEX.TMTouchBit(m_oSession.SessionHandle, 0xFF);

			// Return the result
			return nResult;
		}

		public static short SendBit(short Output)
		{
			// Send the byte and get back what was sent
			short nResult = TMEX.TMTouchBit(m_oSession.SessionHandle, Output);

			// Check that the value was sent correctly
			if (nResult != Output)
			{
				// Throw an exception
				throw new owException(owException.owExceptionFunction.SendBit, m_oLastID);
			}

			// Return the result
			return nResult;
		}

		public static short ReadByte()
		{
			// Send the byte and get back what was sent
			short nResult = TMEX.TMTouchByte(m_oSession.SessionHandle, 0xFF);

			// Return the result
			return nResult;
		}

		public static short Reset()
		{
			// Reset all devices
			return TMEX.TMTouchReset(m_oSession.SessionHandle);
		}

		public static short SendByte(short Output)
		{
			// Send the byte and get back what was sent
			short nResult = TMEX.TMTouchByte(m_oSession.SessionHandle, Output);

			// Check that the value was sent correctly
			if (nResult != Output)
			{
				// Throw an exception
				throw new owException(owException.owExceptionFunction.SendByte, m_oLastID);
			}

			// Return the result
			return nResult;
		}	

		public static short SetLevel(TMEX.TMOneWireLevelOperation nOperation, TMEX.TMOneWireLevelMode nLevelMode, TMEX.TMOneWireLevelPrime nPrimed)
		{
			// Set the level
			short nResult = TMEX.TMOneWireLevel(m_oSession.SessionHandle, TMEX.TMOneWireLevelOperation.Write, TMEX.TMOneWireLevelMode.Normal, TMEX.TMOneWireLevelPrime.Immediate);

			// Check the result
			if (nResult < 0)
			{
				// Throw an exception
				throw new owException(owException.owExceptionFunction.SetLevel, nResult);
			}

			// Return the result
			return nResult;
		}

		#endregion
	}
}
