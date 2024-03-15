using System;
using System.Collections;
using System.Windows.Forms;

namespace Electroimpact
{
	
	namespace StringCalc
	{
		/// <summary>
		/// 
		/// To utilize string calc, simply call SimpleCalc(string myString)
		/// where myString is an equation.  
		/// 
		/// To get more fancy assign variables and use them
		/// 
		/// To break the steps down, you can call SetOrderOfOperations2(string myString)
		/// to see how the string will be sent to the parser.
		/// 
		/// Then call parse(string returnfromSetOrderOfOperations2)
		/// 
		/// To add operators, first you will need to modify code in:
		/// 
		/// SetOrderOfOperation2
		/// parse
		/// ReturnThisLevel
		/// WhoIsBigger
		/// 
		/// Written for a veritety of purposes for Electroimpact by Todd W. Rudberg.
		/// </summary>
		/// 
    public interface IVariables
    {
      void _AssignVariable(string Tag, double Value);
      void _RemoveVariable(string Tag);
      double _GetVariable(string Tag);
      void KillScope();
      IList _GetAllVariables();
      IList _GetAllVariableNames();
      bool _ContainsVariable(string Tag);
    }
		public class cStringCalc : IVariables
		{
			#region MEMBERS
			private csString Operators = new csString("+ * / ^ = > < & |");
			private csString fns = new csString("sin asin cos acos tan atan abs exp ln log rnd sqrt int ! fact sign");
			private cVariables _vars = new cVariables();
			private cMacroVars _MacroVars;

			private enum eWhoIsBigger
			{
				Tied = 0,
				Op1,
				Op2
			}

			private bool _Degrees = true;
			#endregion

			#region CONSTRUCTOR
			public cStringCalc()
			{
				//this._MacroVars = new cMacroVars();
			}
			public cStringCalc(FANUC.OpenCNC CNC)
			{
				this._MacroVars = new cMacroVars(CNC);
			}
			#endregion

			#region PROPERTIES
			public bool Degrees
			{
				get { return this._Degrees; }
				set { this._Degrees = value; }
			}
			public double Pi
			{
				get { return System.Math.PI; }
			}
			public string Functions
			{
				get { return this.fns.String; }
			}

			#endregion

			#region PUBLIC METHODS
			public double SimpleCalc(string Expr)
			{
				string dog = SetOrderOfOperation2(Expr);
				return parse(dog);
			}
			public string SetOrderOfOperation2(csString Expr)
			{
				return this.SetOrderOfOperation2(Expr, 0, false);
			}
			public string SetOrderOfOperation2(string Expr)
			{
				csString cs = new csString(Expr);
				return this.SetOrderOfOperation2(cs);
			}
			private string SetOrderOfOperation2(csString Expr, int RecursionLevel, bool InaBracket)
			{
				string ch;
				string Op1 = "";
				string Op2 = "";
				string Arg1 = "";
				string Arg2 = "";
				string hold = "";
				string sz = "";
				bool first = true;

				Expr.NukeWhiteSpace(); //Whitespace is evil.

				//Catch Evil Leading minus signs.
				ch = Expr.GetLeftNoAdv(1);
				if (ch == "-")
				{
					ch = Expr.GetLeft(1);
					string fixit = Expr.GetRemaining;
					fixit = "-1*" + fixit;
					Expr = new csString(fixit);
				}

				while ((ch = Expr.GetLeft(1)) != "")
				{
					switch (ch)
					{
						case " ":
							break;
						case "{":
						case "(":
						case "[":
							{
								int LP = 1;
								csString substr = new csString(Expr.GetRemaining);
								string newstring = "";
								string ss;
								string lp = ch;
								int ii = 0;
								while (LP > 0)
								{
									ss = substr.GetLeft(1);
									if (ss == "")
										break;
									switch (ss)
									{
										case "{":
										case "(":
										case "[":
											{
												LP++;
												break;
											}
										case "}":
										case ")":
										case "]":
											LP--;
											break;
									}
									newstring += ss;
									ii++;
								}
								string rp = newstring.Substring(newstring.Length - 1, 1);
								newstring = newstring.Substring(0, newstring.Length - 1);
								csString newexpr = new csString(newstring);
								newstring = lp + this.SetOrderOfOperation2(newexpr, 0, false) + rp;
								hold += newstring;
								Expr.Rewind(-ii);
								break;
							}
						case "}":
						case ")":
						case "]":
							break;

						case "+":
						case "*":
						case "/":
						case "^":
						case "=":
						case ">":
						case "<":
						case "&":
						case "|":
						case "-":
							if (ch == "-")
							{
								if (((Op1 != "") || first) && (Arg1 == "") && (hold == ""))
								{
									hold += ch;
									//Just put this here...it gets wiped out later, but indicates we have part of Arg1 for future serches.
									break;
								}
								else if ((Op1 != "") && (hold == ""))
								{
									hold += ch;
									break;
								}
							}
							if (Op1 == "")
							{
								Arg1 = hold;
								Op1 = ch;
								hold = "";
							}
							else
							{
								if (this.IsE(hold))
								{
									hold += ch;
									break;
								}
								Arg2 = hold;
								hold = "";
								Op2 = ch;
								eWhoIsBigger ii = this.WhoIsBigger(Op1, Op2);
								if (ii == eWhoIsBigger.Tied)
								{
									sz += Arg1 + Op1 + Arg2;
									int rwd = Op2.Length;
									Expr.Rewind(rwd);
									Arg1 = Arg2 = Op1 = Op2 = "";
								}
								else if (ii == eWhoIsBigger.Op1)
								{
									int rwd = Op2.Length;
									Expr.Rewind(rwd);
									sz += Arg1 + Op1 + Arg2;
									Arg1 = Arg2 = Op1 = Op2 = "";
								}
								else //Op2 is bigger.
								{
									int amt = Arg2.Length + Op2.Length;
									Expr.Rewind(amt);
									csString expr = this.ReturnThisLevel(Expr, Op1);
									string szRtn = SetOrderOfOperation2(expr, RecursionLevel + 1, false);
									hold = "(" + szRtn;
									Op2 = Arg2 = "";
								}
							}
							break;
						default:
							hold += ch;
							break;
					}
					first = false;
				}

				if (RecursionLevel > 0)
					return sz + Arg1 + Op1 + hold + ")";
				else
					return sz + Arg1 + Op1 + hold;
			}
			public double parse(string expr)			
			{
				csString cs = new csString(expr);
				return this.parse(cs);
			}
			public double parse(csString expr)
			{
				csString aString = new csString();
				string ch;
				string Operand = "";
				string Operator = "";
				double Arg1 = double.NaN;
				bool first = true;
				while ((ch = expr.GetLeft(1)) != "")
				{
					switch (ch)
					{
						case " ":
							break;
						case "{":
						case "(":
						case "[":
							{
								if (Operator != "")
								{
									double Arg2;
									string minus = "";

									if (Operand != "")
									{
										minus = Operand.Substring(0, 1);
										if (minus == "-")
											Operand = Operand.Substring(1, Operand.Length - 1);
									}
									if (fns.InThis(Operand)) //Mono Variable Functions
									{
										double Arg = this.parse(expr);
										Arg2 = this.CalculateMonoVar(Operand, Arg);
									}
									else //Regular Functions
									{
										Arg2 = parse(expr);
									}
									if (minus == "-")
										Arg2 = -Arg2;
									Arg1 = this.Calculate(Operator, Arg1, Arg2);
									Operand = "";
									Operator = "";
								}
								else
								{
									string minus = "";
									if (Operand != "")
									{
										minus = Operand.Substring(0, 1);
										if (minus == "-")
											Operand = Operand.Substring(1, Operand.Length - 1);
									}
									if (fns.InThis(Operand)) //Mono Variable Functions
									{
										Arg1 = this.parse(expr);
										Arg1 = this.CalculateMonoVar(Operand, Arg1);
										Operand = "";
									}
									else //Regular Functions
										Arg1 = parse(expr);
									if (minus == "-")
										Arg1 = -1;
								}
								break;
							}
						case "}":
						case ")":
						case "]":
							{
								if (Operator != "")
								{
									if (this.IsANumber(Operand))
										return this.Calculate(Operator, Arg1, this.ToDouble(Operand));
									else
										return Arg1; //Should have already calculated at this point.  (1+(1)).
								}
								else if (this.IsANumber(Operand))
									return this.ToDouble(Operand);  //1+(1)
								else
									return Arg1;  //Give Back what we recieved. ((1+1))
							}
						case "+":
						case "*":
						case "/":
						case "^":
						case "=":
						case ">":
						case "<":
						case "&":
						case "|":
							{
								if (Operator != "")
								{
									if (this.IsANumber(Operand))
										Arg1 = this.Calculate(Operator, Arg1, this.ToDouble(Operand));
								}
								else
								{
									if (this.IsANumber(Operand))
										Arg1 = this.ToDouble(Operand);
								}
								Operand = "";
								Operator = ch;
								break;
							}
						case "-":
							{
								if (Operator != "")
								{
									if (this.IsANumber(Operand))
									{
										if (this.IsE(Operand))
										{
											Operand += ch;
											break;
										}
										Arg1 = this.Calculate(Operator, Arg1, this.ToDouble(Operand));
										Operand = "";
										Operator = ch;
									}
									else
									{
										Operand += ch;
										break;
									}
								}
								else
								{
									if (this.IsANumber(Operand))
									{
										if (this.IsE(Operand))
										{
											Operand += ch;
											break;
										}
										Arg1 = this.ToDouble(Operand);
										Operator = ch;
										Operand = "";
									}
									else if (first)
										Operand += ch;
									else
									{
										Operator = ch;
										Operand = "";
									}
								}
								break;
							}
						default:
							{
								Operand += ch;
								break;
							}
					}
					first = false;
				}
				if (this.IsANumber(Operand))
				{
					if ((Operator != "") && (Operand != ""))
						return this.Calculate(Operator, Arg1, this.ToDouble(Operand));
					else if (this.IsANumber(Operand))
						return this.ToDouble(Operand);
					return double.NaN;
				}
				else if (Operand == "")
					return Arg1;
				else
					return double.NaN;
			}

			public void KillAllVars()
			{
				this._vars.KillAllVars();
				//			this._MacroVars.ClearMacroVars();
			}
			#endregion

			#region PRIVATE METHODS
			private bool IsANumber(string operand)
			{
				csString css = new csString(operand);
				return css.IsNumeric() || this._vars._ContainsVar(operand) || (this._MacroVars != null && this._MacroVars.IsMacroVar(operand));
			}
			private double ToDouble(string operand)
			{
				csString css = new csString(operand);
				if (this._vars._ContainsVar(operand))
					return this._vars._GetVariable(operand);
				else if( this._MacroVars != null && this._MacroVars.IsMacroVar(operand) )
						return this._MacroVars.ToDouble(operand);
				else
					return css.ToDouble();
			}
			/// <summary>
			/// Checks last character in a string for e.  However, if the 
			/// characters prior to e are not numeric, this returns false.
			/// The purpose of this function is to determin if the "e"
			/// stands for exp or a power of ten function.
			/// </summary>
			/// <param name="Operand">string to be evaluated</param>
			/// <returns>bool</returns>
			private bool IsE(string Operand)
			{
				csString cs = new csString(Operand);
				if( !cs.IsNumeric(cs.GetLeft(cs.Length-1)) )
					return false;
				string LastChar = Operand.Substring(Operand.Length - 1, 1);
				return LastChar == "e";
			}
			private csString ReturnThisLevel(csString Expr, string LastOp)
			{
				string ch = "";
				string hold = "";
				string Op1 = "";
				string Op2 = "";
				string arg = "";
				int LP = 0;

				csString rtn;
				while ((ch = Expr.GetLeft(1)) != "")
				{
					switch (ch)
					{
						case " ":
							break;
						case "{":
						case "(":
						case "[":
							{
								hold += ch;
								LP++;
								break;
							}
						case "}":
						case ")":
						case "]":
							{
								hold += ch;
								LP--;
								break;
							}
						case "+":
						case "*":
						case "/":
						case "^":
						case "=":
						case ">":
						case "<":
						case "&":
						case "|":
						case "-":
							if (ch == "-")
							{
								string Test = "";
								Expr.Peek(ref Test);

								if (arg == "")
								{
									hold += ch;
									break;
								}
							}
							if (Op1 == "")
							{
								Op1 = ch;
								hold += Op1;
								arg = "";
							}
							else
							{
								Op2 = ch;
								arg = "";
								if (LP == 0)
								{
									if (this.WhoIsBigger(Op1, Op2) == eWhoIsBigger.Op1)
									{
										eWhoIsBigger who = this.WhoIsBigger(Op2, LastOp);
										if (who == eWhoIsBigger.Tied || who == eWhoIsBigger.Op2)
										{
											rtn = new csString(hold);
											Expr.Rewind(Op2.Length);
											return rtn;
										}
									}
								}
								hold += Op2;
								Op1 = Op2;
								Op2 = "";
							}
							break;
						default:
							arg += ch;
							hold += ch;
							break;
					}
				}
				rtn = new csString(hold);
				return rtn;
			}
			private eWhoIsBigger WhoIsBigger(string Op1, string Op2)
			{
				System.Collections.Hashtable myOpHash = new System.Collections.Hashtable();
				myOpHash.Add("<", 1);
				myOpHash.Add(">", 1);
				myOpHash.Add("=", 1);
				myOpHash.Add("&", 1);
				myOpHash.Add("|", 1);
				myOpHash.Add("-", 2);
				myOpHash.Add("+", 2);
				myOpHash.Add("/", 3);
				myOpHash.Add("*", 3);
				//			myOpHash.Add( "*", 4 ); //Changed 10 Nov 2004
				myOpHash.Add("^", 5);

				if ((int)myOpHash[Op1] == (int)myOpHash[Op2])
					return eWhoIsBigger.Tied;
				if ((int)myOpHash[Op1] > (int)myOpHash[Op2])
					return eWhoIsBigger.Op1;
				else
					return eWhoIsBigger.Op2;
			}

			private double Calculate(string Fn, double Arg1, double Arg2)
			{
				switch (Fn)
				{
					case "+":
						return Arg1 + Arg2;
					case "-":
						return Arg1 - Arg2;
					case "*":
						return Arg1 * Arg2;
					case "/":
						{
							if (Arg2 != 0)
							{
								return Arg1 / Arg2;
							}
							else
							{
								System.Windows.Forms.MessageBox.Show("Divide by Zero", "Calculate Error");
								return -1;
							}
						}
					case "^":
						return Math.Pow(Arg1, Arg2);
					case "=":
						return Arg1 == Arg2 ? 1 : 0;
					case ">":
						return Arg1 > Arg2 ? 1 : 0;
					case "<":
						return Arg1 < Arg2 ? 1 : 0;
					case "&":
						{
							bool bArg1 = Arg1 == 1;
							bool bArg2 = Arg2 == 1;
							return bArg1 && bArg2 ? 1 : 0;
						}
					case "|":
						{
							bool bArg1 = Arg1 == 1;
							bool bArg2 = Arg2 == 1;
							return bArg1 || bArg2 ? 1 : 0;
						}
					default:
						break;
				}
				return -1;
			}
			private double CalculateMonoVar(string Fn, double Arg1)
			{
				double ret;
				switch (Fn)
        {
          case "sign":
            try
            {
              return Math.Sign(Arg1);
            }
            catch (Exception ex)
            {
              System.Windows.Forms.MessageBox.Show(ex.Message, "Error in sign function");
              break;
            }
          case "sin":
            try
            {
              Arg1 = this.Degrees ? Arg1 * Pi / 180.0 : Arg1;
              return Math.Sin(Arg1);
            }
            catch (Exception ex)
            {
              System.Windows.Forms.MessageBox.Show(ex.Message, "Error in sin function");
              break;
            }

					case "asin":
						try
						{
							ret = Math.Asin(Arg1);
							return this.Degrees ? ret * 180 / Pi : ret;
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in asin function");
							break;
						}

					case "cos":
						try
						{
							Arg1 = this.Degrees ? Arg1 * Pi / 180.0 : Arg1;
							return Math.Cos(Arg1);
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}

					case "acos":
						try
						{
							ret = Math.Acos(Arg1);
							return this.Degrees ? ret * 180 / Pi : ret;
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "tan":
						try
						{
							Arg1 = this.Degrees ? Arg1 * Pi / 180.0 : Arg1;
							return Math.Tan(Arg1);
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "atan":
						try
						{
							ret = Math.Atan(Arg1);
							return this.Degrees ? ret * 180 / Pi : ret;
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "abs":
						return Math.Abs(Arg1);
					case "exp":
						try
						{
							return Math.Exp(Arg1);
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "ln":
						try
						{
							Arg1 = Math.Log(Arg1, Math.E);
							return Arg1;
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "log":
						try
						{
							return Math.Log10(Arg1);
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "rnd":
						return Math.Round(Arg1, 0);
					case "sqrt":
						try
						{
							return Math.Sqrt(Arg1);
						}
						catch (Exception ex)
						{
							System.Windows.Forms.MessageBox.Show(ex.Message, "Error in " + Fn + " function");
							break;
						}
					case "int":
						return (long)Arg1;
					case "!":
						Arg1 = Arg1 == 1 ? 0 : 1;
						return Arg1;
					case "fact":
						int Number = (int)Arg1;
						ret = Number;
						if (Number > 0)
						{
							while (Number > 1)
							{
								ret *= --Number;
							}
							return ret;
						}
						else
						{
							System.Windows.Forms.MessageBox.Show("Cannot have negative number for Factorials");
							break;
						}
				}
				return 0;
			}
			#endregion

			#region Variable classes

			public class cVariable
			{
				string _tag;
				double _value;
        string _note;

				public cVariable()
				{
					this._tag = "";
					this._value = 0;
				}

				public cVariable(string Tag, double Value)
				{
					this._tag = Tag;
					this._value = Value;
				}


        public cVariable(string Tag, double Value, string note)
        {
          this._tag = Tag;
          this._value = Value;
          this._note = note;
        }

				public string Tag
				{
					get { return this._tag; }
					set { this._tag = value; }
				}

				public double Value
				{
					get { return this._value; }
					set { this._value = value; }
				}

        public string Note
        {
          get { return this._note; }
          set { this._note = value; }
        }

				public override string ToString()
				{
					return this._tag + " " + this._value.ToString();
				}
			}

			private class cVariables : System.Collections.SortedList
			{
				private uint _cntscope;
				private uint _cntsubscope;

				private class cScope
				{
					public uint scope;
					public uint subscope;

					public cScope(uint scope, uint subscope)
					{
						this.scope = scope;
						this.subscope = subscope;
					}
				}
				System.Collections.ArrayList scopes = new ArrayList();


				//HashTable Format will be "scope.subscope.tag", this must be unique.
				public cVariables()
				{
					this._cntscope = 0;
					this._cntsubscope = 0;
				}

				public void _AssignVariable(cVariable var)
				{
					string key = this._createkey(var.Tag);
					if (this.ContainsKey(key))
					{
						cVariable avar = (cVariable)this[key];
						avar.Value = var.Value;
					}
					else
						this.Add(key, var);
				}

				public void _AssignVariable(string Tag, double Value)
				{
					cVariable newvar = new cVariable(Tag, Value);
					this._AssignVariable(newvar);
				}

        public void _AssignVariableNote(string Tag, string note)
        {
          string key = this._createkey(Tag);
          if (this.ContainsKey(key))
          {
            ((cVariable)this[key]).Note = note;
          }
        }

				public void _RemoveVariable(string Tag)
				{
					string key = this._createkey(Tag);
					try
					{
						this.Remove(key);
					}
					catch
					{
						System.Windows.Forms.MessageBox.Show(Tag + " Doesn't exist...dumbass!.");
					}
				}

        /// <summary>
        /// Returns the double variable value
        /// </summary>
        /// <param name="Tag"></param>
        /// <returns></returns>
				public double _GetVariable(string Tag)
				{
					string Key = this._createkey(Tag);
					if (this.ContainsKey(Key))
					{
						return ((cVariable)this[Key]).Value;
					}
					return 0.0;
				}

        /// <summary>
        /// Returns the double variable value
        /// </summary>
        /// <param name="Tag"></param>
        /// <returns></returns>
        public string _GetVariableNote(string Tag)
        {
          string Key = this._createkey(Tag);
          if (this.ContainsKey(Key))
          {
            return ((cVariable)this[Key]).Note;
          }
          return null;
        }



				public bool _VariableExists(string Tag)
				{
					string Key = this._createkey(Tag);
					if (this.ContainsKey(Key))
					{
						return true;
					}
					else
					{
						return false;
					}
				}


				public uint _Scope
				{
					get { return this._cntscope; }
					set
					{
						if (value > this._cntscope)
						{
							cScope pastscope = new cScope(this._cntscope, this._cntsubscope);
							this.scopes.Add(pastscope);
							this._cntsubscope = 0;
							this._cntscope = value;
						}
						else
						{
							if (value >= 0)
							{
								System.Collections.IList keys = this.GetKeyList();
								for (int ii = (keys.Count - 1); ii >= 0; ii--)
								{
									string[] pcs = ((string)keys[ii]).Split('.');
									int scope = int.Parse(pcs[0]);
									if (scope == this._cntscope)
										this.Remove(keys[ii]);
								}
								cScope thisscope = (cScope)this.scopes[this.scopes.Count - 1];
								if ((this._cntscope - 1) != thisscope.scope)
									System.Windows.Forms.MessageBox.Show("ScopeError");
								this._cntsubscope = thisscope.subscope;
								this.scopes.RemoveAt(this.scopes.Count - 1);
								this._cntscope = value;
							}
						}
					}
				}
				public uint _SubScope
				{
					get { return this._cntsubscope; }
					set
					{
						if (this._cntsubscope > value)
						{
							System.Collections.IList list = this.GetKeyList();
							for (int ii = (list.Count - 1); ii >= 0; ii--)
							{
								string[] pcs = ((string)list[ii]).Split('.');
								int scope = int.Parse(pcs[0]);
								int subscope = int.Parse(pcs[1]);
								if ((scope == this._Scope && subscope >= this._SubScope) || scope > this._Scope)
									this.Remove(list[ii]);
							}
						}
						this._cntsubscope = value;
					}
				}

				public bool _ContainsVar(string Tag)
				{
					string key = this._createkey(Tag);
					return this.ContainsKey(key);
				}

				private string _createkey(string Tag)
				{
					return this._cntscope.ToString() + "." + this._cntsubscope.ToString() + "." + Tag;
				}
				public void KillAllVars()
				{
					System.Collections.IList keys = this.GetKeyList();
					for (int ii = keys.Count - 1; ii >= 0; ii--)
					{
						this.Remove(keys[ii]);
					}
				}
				public void KillScope()
				{
					System.Collections.IList keys = this.GetKeyList();
					for (int ii = keys.Count - 1; ii >= 0; ii--)
					{
						string scopePrefix = keys[ii].ToString();
						int index = scopePrefix.IndexOf(".");
						index = scopePrefix.IndexOf(".", index + 1);
						scopePrefix = scopePrefix.Substring(0, index);
						string compareTo = this._cntscope.ToString() + "." + this._cntsubscope.ToString();
						if (scopePrefix == compareTo)
						{
							this.Remove(keys[ii]);
						}
					}
				}

			}

			private class cMacroVars
			{
				Electroimpact.FANUC.OpenCNC _CNC;

				public cMacroVars()
				{
					FANUC.Err_Code myErr;
					this._CNC = new FANUC.OpenCNC(0, out myErr);
				}

				public cMacroVars(FANUC.OpenCNC CNC)
				{
					this._CNC = CNC;
				}
				public bool IsMacroVar(string operand)
				{
					csString Operand = new csString(operand);
					if (Operand.GetLeft(1) == "#")
					{
						int num;
						Operand.FindInteger(1, out num);
						if (num > 0 && num < 9999)
							return true;
					}
					return false;
				}
				public double ToDouble(string operand)
				{
					csString cs = new csString(operand);
					int macnum;
					cs.FindInteger(1, out macnum);
					if (macnum > 0 && macnum < 9999)
					{
						return this._CNC.ReadMacroVariable((short)macnum);
					}
					return double.NaN;
				}
				/// <summary>
				/// Clears macro variables #100-#199. This will
				/// be used when a new program is loaded or when
				/// M2, M30, or M31 is run. Closer to the way
				/// the A380 control program runs.
				/// </summary>
				public void ClearMacroVars()
				{
					for (short ii = 100; ii < 200; ii++)
					{
						this._CNC.WriteMacroVariable(ii, 0);
					}
				}
			}

			#endregion

			#region IVariables Members

			public void _AssignVariable(string Tag, double Value)
			{
				this._vars._AssignVariable(Tag, Value);
			}

      public void _AssignVariableNote(string Tag, string note)
      {
        this._vars._AssignVariableNote(Tag, note);
      }

			public void _RemoveVariable(string Tag)
			{
				this._vars._RemoveVariable(Tag);
			}

			public double _GetVariable(string Tag)
			{
				return this._vars._GetVariable(Tag);
			}

      public string _GetVariableNote(string Tag)
      {
        return this._vars._GetVariableNote(Tag);
      }

			public IList _GetAllVariables()
			{
				return this._vars.GetValueList();
			}

			public IList _GetAllVariableNames()
			{
				return this._vars.GetKeyList();
			}

			public void KillScope()
			{
				this._vars.KillScope();
			}

      public bool _ContainsVariable(string Tag)
      {
        return this._vars._VariableExists(Tag);
      }

			#endregion

    }
	}
}