using System;

namespace Electroimpact
{
	public class csString
	{

		#region MEMBERS
		public delegate void ErrorThrower(string szError);
		public event ErrorThrower StringError;

		private string mString;
		private string[] mszarParsed;
		private int mnCount;
		private int mnCurrentIndex;
		private int mnCurrentPosition;
		#endregion

		System.Collections.Hashtable Constants = new System.Collections.Hashtable();
		System.Collections.ICollection ConstantKeys;
		double ConstantLast;

		#region CONSTRUCTORS
		public csString()
		{
			this.mString = "";
			mnCount = 0;
			mnCurrentIndex = 0;
			mnCurrentPosition = 0;
			SetupConstants();
		}

		public csString(string StringIn)
		{
			this.mString = StringIn;
			mnCount = 0;
			mnCurrentIndex = 0;
			mnCurrentPosition = 0;
			SetupConstants();
		}

		private void SetupConstants()
		{
			this.Constants.Add("pi", System.Math.PI);
			this.Constants.Add("E", System.Math.E);
			this.ConstantKeys = this.Constants.Keys;
		}

		#endregion

		#region Events
		private void ErrorHandler(string szError)
		{
			if (StringError != null)
			{
				this.StringError(szError);
			}
			else
			{
				//      System.Windows.Forms.MessageBox.Show( szError );
			}
		}

		#endregion

		#region Properties
		/// <summary>
		/// Sets or gets internal string.  Setting rewinds the string pointer.
		/// </summary>
		public string String
		{
			get
			{
				return this.mString;
			}
			set
			{
				this.mString = value;
				this.Rewind();
			}
		}
		/// <summary>
		/// Returns length of internal string.
		/// </summary>
		public int Length
		{
			get { return this.mString.Length; }
		}
		/// <summary>
		/// Returns the constants defined as numeric values.  Currently e and pi are it.  e doesn't work in the calculator and I don't care.
		/// </summary>
		public string constants
		{
			get
			{
				string rtn = "";
				foreach (string key in this.ConstantKeys)
				{
					rtn += key + " ";
				}
				rtn = rtn.Substring(0, rtn.Length - 1);
				return rtn;
			}
		}

		#endregion

		#region METHODS
		/// <summary>
		/// Returns array of strings broken up based on the size passed.
		/// </summary>
		/// <param name="str">string to break up</param>
		/// <param name="size">max length of string</param>
		/// <returns>array of System.String[] array</returns>
		public string[] BreakUp(string str, int size)
		{
			int items = str.Length / size + 1;
			string[] ret = new string[items];
			int ii;
			for (ii = 0; ii < items - 1; ii++)
			{
				ret[ii] = str.Substring(ii * size, size);
			}
			ret[ii] = str.Substring(ii * size);
			return ret;
		}
		/// <summary>
		/// Finds integer portion of the member string in string class beginning at start.
		/// </summary>
		/// <param name="start">integer position to start looking for integer in string</param>
		/// <param name="Value">out int the integer this function finds</param>
		/// <returns>the end of the integer portion of the string.</returns>
		public int FindInteger(int start, out int Value)
		{
			Value = -1; //Initial Value.
			if (start >= this.mString.Length)
				return -1;

			bool breakout = false;
			string integer = "";
			int ii;
			for (ii = start; ii < this.mString.Length; ii++)
			{
				string ch = this.mString.Substring(ii, 1);
				switch (ch)
				{
					case "0":
					case "1":
					case "2":
					case "3":
					case "4":
					case "5":
					case "6":
					case "7":
					case "8":
					case "9":
						integer += ch;
						break;
					case "-":
						if (ii == start)
							integer += ch;
						else
							breakout = true;
						break;
					default:
						if (ii == start)
							return -1;

						breakout = true;
						break;
				}
				if (breakout)
					break;
			}
			Value = this.ToInt(integer);
			return ii - 1;
		}
		/// <summary>
		/// Wrapper for system.string.IndexOf(string in).  Returns boolean if CompareString contains c.
		/// </summary>
		/// <param name="c">string:  Substring you are looking for</param>
		/// <param name="CompareString">string: String that may have c in it.</param>
		/// <returns>bool</returns>
		/// <seealso cref="InString(string c, string CompareString)"/>
		public bool InString(string c, string CompareString)
		{
			if (CompareString.IndexOf(c) > -1)
				return true;
			else
				return false;
		}
		/// <summary>
		/// compares member string to Substring
		/// </summary>
		/// <param name="SubString">System.string</param>
		/// <returns>bool</returns>
		/// <seealso cref="public bool InString(string c, string CompareString)"/>
		public bool InThis(string SubString)
		{
			if ((this.mString.IndexOf(SubString) > -1) && (SubString != ""))
				return true;
			else
				return false;
		}
		public bool IsInt(string szLine)
		{
			return CheckInt(szLine);
		}
		public bool IsInt()
		{
			return CheckInt(this.mString);
		}

		public bool IsNumeric()
		{
			return this.CheckNumeric(this.mString);
		}

		public bool IsNumeric(string szIn)
		{
			return this.CheckNumeric(szIn);
		}

		public int ToInt()
		{
			if (this.IsNumeric())
			{
				double d = double.Parse(this.mString);
				if ((d < System.Math.Pow(2, 32)) && (d > -System.Math.Pow(2, 32)))
				{
					int ii = (int)(double.Parse(this.mString));
					return ii;
				}
				else
				{
					this.ErrorHandler("Int32 Overflow!");
					return 0;
				}
			}
			else
			{
				this.ErrorHandler("String is not Numeric!");
				return 0;
			}
		}
		public int ToInt(string inString)
		{
			//this.mString = inString;
			if (this.IsNumeric(inString))
			{
				double d = double.Parse(inString);
				if ((d < System.Math.Pow(2, 32)) && (d > -System.Math.Pow(2, 32)))
				{
					int ii = (int)(double.Parse(inString));
					return ii;
				}
				else
				{
					this.ErrorHandler("Int32 Overflow!");
					return 0;
				}
			}
			else
			{
				this.ErrorHandler("String is not Numeric!");
				return 0;
			}
		}
		public double ToDouble()
		{
			if (this.IsNumeric())
			{
				if (!double.IsNaN(this.ConstantLast))
					return this.ConstantLast;
				double d = double.Parse(this.mString);
				return d;
			}
			else
			{
				this.ErrorHandler("String is not Numeric!");
				return 0;
			}
		}
		public void KillChar(ref string szIn, char cKill)
		{
			string[] strings = szIn.Split(cKill);
			szIn = "";
			foreach (string ass in strings)
				szIn += ass;
		}
		public double ToDouble(char KillChar)
		{
			string s = "";
			char[] arc = this.mString.ToCharArray();
			foreach (char c in arc)
			{
				if (c != KillChar)
					s += c.ToString();
			}
			this.mString = s;

			if (this.IsNumeric())
			{
				if (!double.IsNaN(this.ConstantLast))
					return this.ConstantLast;
				double d = double.Parse(this.mString);
				return d;
			}
			else
			{
				this.ErrorHandler("String is not Numeric!");
				return 0;
			}
		}
		public double ToDouble(string StringIn)
		{
			double ret;
			if (double.TryParse(StringIn, out ret))
				return ret;
			else

			//if (this.IsNumeric(StringIn))
			//{
			//  if (!double.IsNaN(this.ConstantLast))
			//    return this.ConstantLast;
			//  double d = double.Parse(StringIn);
			//  return d;
			//}
			//else
			{
				this.ErrorHandler("String is not Numeric!");
				return 0;
			}
		}
		public int Parse(char cDelim)
		{
			this.mszarParsed = this.mString.Split(cDelim);
			this.mnCount = this.mszarParsed.Length;
			this.mnCurrentIndex = 0;
			return this.mnCount;
		}
		/// <summary>
		/// Returns a string of one character.  This is the next character to the string pointer.  This function advances the string pointer
		/// </summary>
		/// <returns>string of on character</returns>
		public string GetItem()
		{
			if (this.mnCurrentIndex < this.mnCount)
			{
				this.mnCurrentIndex++;
				return this.mszarParsed[this.mnCurrentIndex - 1]; //Have to increment first due to the Return Statement.
			}
			else
				return "";
		}
		public void Rewind()
		{
			this.mnCurrentIndex = 0;
			this.mnCurrentPosition = 0;
		}
		public void Rewind(int amount)
		{
			if (this.mnCurrentPosition >= amount)
				this.mnCurrentPosition -= amount;
			else
				this.mnCurrentPosition = 0;
		}
		/// <summary>
		/// Returns a substring from the current position and advancess the string pointer by ii.
		/// </summary>
		/// <param name="ii">number of characters to return and advance the string pointer by.</param>
		/// <returns>string</returns>
		/// <seealso cref="GetLeft(int ii)"/>
		public string GetLeft(int ii)
		{
			if (this.mnCurrentPosition + ii <= this.mString.Length)
			{
				string szReturn = this.mString.Substring(this.mnCurrentPosition, ii);
				this.mnCurrentPosition += ii;
				return szReturn;
			}
			else
				return "";
		}
		/// <summary>
		/// Returns a substring from the current position without affecting the string pointer.
		/// </summary>
		/// <param name="ii">integer number of characters to return</param>
		/// <returns>string</returns>
		/// <seealso cref="GetLeft(int ii)"/>
		public string GetLeftNoAdv(int ii)
		{
			if (this.mnCurrentPosition + ii <= this.mString.Length)
			{
				string szReturn = this.mString.Substring(this.mnCurrentPosition, ii);
				return szReturn;
			}
			else
				return "";
		}
		public string GetRemaining
		{
			get { return this.mString.Substring(this.mnCurrentPosition, this.mString.Length - this.mnCurrentPosition); }
		}
		public bool Peek(ref string nextChar)
		{
			int ii = 0;
			nextChar = " ";
			while (nextChar == " ")
			{
				if (this.mnCurrentPosition + ii < this.mString.Length)
					nextChar = this.mString.Substring(this.mnCurrentPosition + ii, 1);
				else
					return false;
			}
			return true;
		}
		public void NukeWhiteSpace()
		{
      //string newstring = "";
      //string ch = "";
      //for (int ii = 0; ii < this.mString.Length; ii++)
      //{
      //  ch = this.mString.Substring(ii, 1);
      //  if (!char.IsWhiteSpace(ch, 0))
      //    newstring += ch;
      //}
      //this.mString = newstring;
      NukeWhiteSpace(ref this.mString);
		}
		public void NukeWhiteSpace(ref string sz)
		{
			string newstring = "";
			string ch = "";
			for (int ii = 0; ii < sz.Length; ii++)
			{
				ch = sz.Substring(ii, 1);
				if (!char.IsWhiteSpace(ch, 0))
					newstring += ch;
			}
			sz = newstring;
		}
/// <summary>
/// Replaces groups of whitespace with a space " ".
/// In this case, the internal string is modified.
/// </summary>
    public void CollapseWhiteSpace()
    {
      CollapseWhiteSpace(out this.mString);
    }
    /// <summary>
    /// Replaces groups of whitespace with a space " ".
    /// In this case, only the string parameter "sz" is modified.
    /// </summary>
    /// <param name="sz"></param>
    public void CollapseWhiteSpace(out string sz)
    {
      string newstring = "";
      string ch = "";
      bool inwhitespace = false;
      for (int ii = 0; ii < this.mString.Length; ii++)
      {
        ch = this.mString.Substring(ii, 1);
        if (!char.IsWhiteSpace(ch, 0))
        {
          if (inwhitespace)
            newstring += " ";
          newstring += ch;
          inwhitespace = false;
        }
        else
          inwhitespace = true;
      }
      sz = newstring;
    }

		#endregion

		#region PRIVATE METHODS
		private bool CheckInt(string szLine)
		{
			bool bInt = true;
			if (szLine.Length == 0)
				bInt = false;
			int nCount = 0;
			for (int ii = 0; ii < szLine.Length; ii++)
			{
				switch (szLine.Substring(ii, 1))
				{
					case "-":
						if (nCount > 0) //sign only valid in first character.
							bInt = false;
						break;
					case "+":
						if (nCount > 0) //sign only valid in first character.
							bInt = false;
						break;
					case "0":
						break;
					case "1":
						break;
					case "2":
						break;
					case "3":
						break;
					case "4":
						break;
					case "5":
						break;
					case "6":
						break;
					case "7":
						break;
					case "8":
						break;
					case "9":
						break;
					default:
						bInt = false;
						break;
				}
				if (!bInt)
					break;
				nCount++;
			}
			return bInt;
		}

		private bool InList(string szIn)
		{
			string ch = szIn.Substring(0, 1);
			bool minus = false;
			if (ch == "-")
			{
				ch = szIn.Substring(1, szIn.Length - 1);
				minus = true;
			}
			else
				ch = szIn;

			foreach (string key in this.ConstantKeys)
			{
				if (ch == key)
				{
					this.ConstantLast = minus ? -(double)this.Constants[ch] : (double)this.Constants[ch];
					return true;
				}
			}
			this.ConstantLast = double.NaN;
			return false;
		}

		private bool CheckNumeric(string szLine)
		{
			if (szLine == "")
				return false;

			if (this.InList(szLine))
				return true;

			try
			{
				double dog;
				return double.TryParse(szLine, out dog);
			}
			catch
			{
				return false;
			}
		}
		#endregion

	}
}