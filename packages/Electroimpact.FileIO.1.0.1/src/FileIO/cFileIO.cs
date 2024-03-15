using System;
using System.Collections.Generic;
using System.Text;

namespace Electroimpact
{
	namespace FileIO
	{
		public class cFileWriter : IDisposable
		{
			private System.IO.StreamWriter sw;
			
			public cFileWriter()
			{
			}

      ~cFileWriter()
      {
        Dispose(false);
      }

			/// <summary>
			/// Opens FileName, Writes Line, Closes FileName.  Use when writing small amounts of data
			/// </summary>
			/// <param name="FileName">string file to write to</param>
			/// <param name="Line">string to write</param>
			/// <param name="Append">bool</param>
      public bool WriteLine(string FileName, string Line, bool Append)
      {
        try
        {
					using (System.IO.StreamWriter sw_this = new System.IO.StreamWriter(FileName, Append))
					{
						sw_this.WriteLine(Line);
					}
					return true;
				}
        catch (Exception ex)
        {
          System.Windows.Forms.MessageBox.Show(ex.ToString());
          return false;
        }
      }

			/// <summary>
			/// Writes a single line to a file previously opened.
			/// </summary>
			/// <param name="Line">string Line to write</param>
			public bool WriteLine(string Line)
			{
				if (this.sw == null)
					return false;
				try
				{
					this.sw.WriteLine(Line);
					return true;
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			/// <summary>
			/// Sets stream writer pointer to this file. Does not append.
			/// </summary>
			/// <param name="FileName">string the file</param>
			public void OpenFile(string FileName)
			{
				try
				{
					this.sw = new System.IO.StreamWriter(FileName);
				}
				catch (Exception ex)
				{
					try
					{
						System.IO.File.Delete(FileName);
						this.sw = new System.IO.StreamWriter(FileName);
					}
					catch (Exception ex1)
					{
						throw ex1;
					}
					throw ex;
				}
			}
			/// <summary>
			/// Closes Previously opened file
			/// </summary>
			public void CloseFile()
			{
        DisposeOfStream();
			}
			/// <summary>
			/// Indicates if a file is open.
			/// </summary>
			public bool IsOpen
			{
				get { return this.sw != null; }
			}
      public string CurrentFolder()
      {
        string FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string appname = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        FileName = FileName.Substring(0, FileName.Length - (4 + appname.Length));
        return FileName;
      }


      #region IDisposable Members

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      public void Dispose(bool explicitCall)
      {
        if (explicitCall)
        {
          DisposeOfStream();
        }
      }

      private void DisposeOfStream()
      {
        if (sw != null)
        {
          sw.BaseStream.Flush();
          sw.Close();
          sw.Dispose();
          Console.WriteLine("Disposing cFileWriter objects.");
          sw = null;
        }
      }

      #endregion
    }
		public class cFileReader : IDisposable
		{
			private System.IO.StreamReader sr;
			private int filelength = 0;
			/// <summary>
			/// Reads a single line from the already opened file.
			/// </summary>
			/// <returns>string</returns>
			public string ReadLine()
			{
				try
				{
					return sr.ReadLine();
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			/// <summary>
			/// Returns the number of lines in this file.
			/// </summary>
			/// <returns>int</returns>
			public int Lines()
			{
				try
				{
					return this.filelength;
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			/// <summary>
			/// Used as end of file indicator, returns true if another line exists.
			/// </summary>
			/// <returns>bool.  True if another line exists</returns>
			public bool Peek()
			{
				if (sr != null)
				{
					return sr.Peek() != -1;
				}
				return false;
			}
			/// <summary>
			/// Closes previously opened file
			/// </summary>
			public void CloseFile()
			{
        DisposeOfStream();
			}
			/// <summary>
			/// Opens file of name FileName
			/// </summary>
			/// <param name="FileName">a string indicating the FileName including location information <br />
			/// e.g. d:\todd\bigdogs\dog.txt</param>
			public void OpenFile(string FileName)
			{
				try
				{
          sr = new System.IO.StreamReader(FileName);
          this.filelength = 0;
          while (this.Peek())
          {
            ReadLine();
            this.filelength++;
          }
          sr.Close();
          sr.Dispose();
          sr = new System.IO.StreamReader(FileName);
				}
				catch( Exception ex )
				{
          
					throw ex;
				}
			}
      public string CurrentFolder()
      {
        string FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string appname = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        FileName = FileName.Substring(0, FileName.Length - (4 + appname.Length));
        return FileName;
      }


      #region IDisposable Members

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      public void Dispose(bool explicitCall)
      {
        if (explicitCall)
        {
          DisposeOfStream();
        }
      }

      private void DisposeOfStream()
      {
        if (sr != null)
        {
          sr.BaseStream.Flush();
          sr.Close();
          sr.Dispose();
          Console.WriteLine("Disposing cFileReader objects.");
          sr = null;
        }
      }


      #endregion
    }

		public class cFileOther
		{
			public cFileOther()
			{
			}
      /// <summary>
      /// Gets the Exection Directory
      /// </summary>
      /// <returns></returns>
      static public string CurrentFolderMethod()
      {
        string FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string appname = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        FileName = FileName.Substring(0, FileName.Length - (4 + appname.Length));
        return FileName;
      }
      public string CurrentFolder()
      {
        string FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string appname = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        FileName = FileName.Substring(0, FileName.Length - (4 + appname.Length));
        return FileName;
      }      
      /// <summary>
      /// Gets the Execution Directory
      /// </summary>
      /// <returns></returns>
      static public string GetExecutionDirectoryMethod()
      {
        string FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string appname = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        FileName = FileName.Substring(0, FileName.Length - (4 + appname.Length));
        return FileName;
      }
      public string GetExecutionDirectory()
      {
        string FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string appname = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        FileName = FileName.Substring(0, FileName.Length - (4 + appname.Length));
        return FileName;
      }
      /// <summary>
      /// Deletes the file
      /// </summary>
      /// <param name="FileName"></param>
			static public void DeleteFileMethod(string FileName)
			{
				try
				{
					System.IO.File.Delete(FileName);
				}
				catch
				{
				}
			}
      public void DeleteFile(string FileName)
      {
        try
        {
          System.IO.File.Delete(FileName);
        }
        catch
        {
        }
      }
      /// <summary>
      /// Obvious
      /// </summary>
      /// <param name="FileName"></param>
      /// <returns></returns>
      static public bool FileExistsMethod(string FileName)
      {
        return System.IO.File.Exists(FileName);
      }
      public bool FileExists(string FileName)
      {
        return System.IO.File.Exists(FileName);
      }

      static public string GetDirecotry(string FileName)
      {
        System.IO.FileInfo fi = new System.IO.FileInfo(FileName);
        return fi.DirectoryName + "\\";
      }
    }
	}
}