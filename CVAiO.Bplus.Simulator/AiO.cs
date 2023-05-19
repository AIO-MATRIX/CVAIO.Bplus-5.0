using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVAiO.Bplus.Simulator
{

	public static class AiO
	{
		private static DateTime dateTime;

		private static string watchName;

		public static byte[] GetByteContatiner(short lowWord, short highWord)
		{
			return new byte[4]
			{
			(byte)((uint)lowWord & 0xFFu),
			(byte)(lowWord >> 8),
			(byte)((uint)highWord & 0xFFu),
			(byte)(highWord >> 8)
			};
		}

		public static byte[] GetByteContatiner(int word)
		{
			return new byte[2]
			{
			(byte)((uint)word & 0xFFu),
			(byte)(word >> 8)
			};
		}

		public static byte[] GetByteContatiner(short word)
		{
			return new byte[2]
			{
			(byte)((uint)word & 0xFFu),
			(byte)(word >> 8)
			};
		}

		public static float TwoWordToFloat(short lowWord, short highWord)
		{
			return BitConverter.ToSingle(GetByteContatiner(lowWord, highWord), 0);
		}

		public static float TwoWordToFloat(short lowWord, short highWord, int sosu)
		{
			int value = BitConverter.ToInt32(GetByteContatiner(lowWord, highWord), 0);
			return (float)value * (float)Math.Pow(10.0, sosu * -1);
		}

		public static float TwoWordToFloat(short[] twoWord, int startIndex)
		{
			return TwoWordToFloat(twoWord[startIndex], twoWord[startIndex + 1]);
		}

		public static float TwoWordToFloat(short[] twoWord, int startIndex, int sosu)
		{
			return TwoWordToFloat(twoWord[startIndex], twoWord[startIndex + 1], sosu);
		}

		public static short[] FloatToTwoWord(float value)
		{
			short[] twoWord = new short[2];
			byte[] by = BitConverter.GetBytes(value);
			twoWord[0] = (short)((by[0] | (by[1] << 8)) & 0xFFFF);
			twoWord[1] = (short)((by[2] | (by[3] << 8)) & 0xFFFF);
			return twoWord;
		}

		public static short[] FloatToTwoWord(float value, int sosu)
		{
			value = (int)Math.Floor(value * (float)(int)Math.Pow(10.0, sosu));
			short[] twoWord = new short[2];
			byte[] by = BitConverter.GetBytes((int)value);
			twoWord[0] = (short)((by[0] | (by[1] << 8)) & 0xFFFF);
			twoWord[1] = (short)((by[2] | (by[3] << 8)) & 0xFFFF);
			return twoWord;
		}

		public static void FloatToTwoWord(float value, short[] twoWord, int startIndex)
		{
			if (twoWord == null || twoWord.Length < 2)
			{
				throw new Exception("배열 크기 초과 또는 배열 Null");
			}
			byte[] by = BitConverter.GetBytes(value);
			twoWord[startIndex] = (short)((by[0] | (by[1] << 8)) & 0xFFFF);
			twoWord[startIndex + 1] = (short)((by[2] | (by[3] << 8)) & 0xFFFF);
		}

		public static void FloatToTwoWord(float value, int sosu, short[] twoWord, int startIndex)
		{
			value = (int)Math.Floor(value * (float)(int)Math.Pow(10.0, sosu));
			byte[] by = BitConverter.GetBytes((int)value);
			twoWord[startIndex] = (short)((by[0] | (by[1] << 8)) & 0xFFFF);
			twoWord[startIndex + 1] = (short)((by[2] | (by[3] << 8)) & 0xFFFF);
		}

		public static short[] IntToWord(int value)
		{
			short[] oneWord = new short[1];
			IntToWord(value, oneWord, 0);
			return oneWord;
		}

		public static void IntToWord(int value, short[] oneWord, int startIndex)
		{
			byte[] by = BitConverter.GetBytes(value);
			oneWord[startIndex] = (short)((by[0] | (by[1] << 8)) & 0xFFFF);
		}

		public static int TwoWordToInt(short lowWord, short highWord)
		{
			return BitConverter.ToInt32(GetByteContatiner(lowWord, highWord), 0);
		}

		public static int TwoWordToInt(short[] twoWord, int startIndex)
		{
			return TwoWordToInt(twoWord[startIndex], twoWord[startIndex + 1]);
		}

		public static short[] IntToTwoWord(int value)
		{
			short[] twoWord = new short[2];
			IntToTwoWord(value, twoWord, 0);
			return twoWord;
		}

		public static string ShortToString(short value)
		{
			byte[] by = BitConverter.GetBytes(value);
			string data = "";
			for (int i = by.Length; i > 0; i--)
			{
				data += System.Convert.ToChar(by[i - 1]);
			}
			return data;
		}

		public static void IntToTwoWord(int value, short[] twoWord, int startIndex)
		{
			byte[] by = BitConverter.GetBytes(value);
			twoWord[startIndex] = (short)((by[0] | (by[1] << 8)) & 0xFFFF);
			twoWord[startIndex + 1] = (short)((by[2] | (by[3] << 8)) & 0xFFFF);
		}

		public static string HexToAscii(int nRrecvData)
		{
			string strRecvData = $"{nRrecvData:X}";
			if (strRecvData.Length < 4)
			{
				return "";
			}
			int subStrIdx = 2;
			int subStrLen = 2;
			string strResult = string.Empty;
			for (int nIdx = 0; nIdx < 2; nIdx++)
			{
				string tmpHexStr = string.Empty;
				tmpHexStr = strRecvData.ToString().Substring(subStrIdx, subStrLen);
				subStrIdx -= 2;
				short nHexNum = short.Parse(tmpHexStr, NumberStyles.HexNumber);
				strResult += System.Convert.ToChar(nHexNum);
			}
			return strResult;
		}

		public static string HexToAscii2(int nRrecvData)
		{
			string strRecvData = $"{nRrecvData:X}";
			string strResult = string.Empty;
			if ((nRrecvData >= 48 && nRrecvData <= 57) || (nRrecvData >= 65 && nRrecvData <= 90) || (nRrecvData >= 97 && nRrecvData <= 122))
			{
				int subStrLen = 2;
				for (int nIdx = 0; nIdx < strRecvData.Length; nIdx += 2)
				{
					string tmpHexStr = string.Empty;
					tmpHexStr = strRecvData.Substring(nIdx, subStrLen);
					uint decval = System.Convert.ToUInt32(tmpHexStr, 16);
					strResult += System.Convert.ToChar(decval);
				}
			}
			return strResult;
		}

		public static string WordToString(int[] word, int length)
		{
			string strValue = "";
			for (int i = 0; i < length; i++)
			{
				strValue += HexToAscii2(word[i]);
			}
			return strValue;
		}

		public static string WordToTwoString(int[] word, int length)
		{
			string strValue = "";
			short[] split = new short[2];
			for (int i = 0; i < length; i++)
			{
				split = IntToTwoWord(word[i]);
				strValue += System.Convert.ToChar(split[0]);
				strValue += System.Convert.ToChar(split[1]);
			}
			return strValue;
		}
	}
}
