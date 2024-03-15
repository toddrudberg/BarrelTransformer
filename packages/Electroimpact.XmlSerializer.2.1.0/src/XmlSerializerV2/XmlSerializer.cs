using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
namespace Electroimpact.XmlSerialization
{
  //TODO: 
  //  Proper capitalization
  //  Create a namespace for the serializer
  //  Move custom serializer classes (Dictionary, TimeSpan) to their own files, and out of the serializer class
  #region XML Settings File From Object (Static Version)
  /// <summary>
  /// Serialize object into an XML file.  Most tedious taskes are handled for you.
  /// </summary>


  //EXAMPLE USE:

  //cSettings settings = new cSettings(); //Where cSettings is a custom class that you define
  //Make any changes to "settings"
  //Electroimpact.MikesXmlSerializer.Save(settings);  //This saves a copy of your object in an xml file located at  C:\ProgramData\Electroimpact\[ProgramName]\settings.xml
  //
  //To Load:
  //settings = Electroimpact.MikesXmlSerializer.Load<cSettings>();

  public class Serializer
  {
    // TO DO:
    // -Some sort of version checking on the settings file

    /// <summary>
    /// Save an object as an XML file located at C:\ProgramData\Electroimpact\[ProgramName]\settings.xml
    /// </summary>
    /// <param name="ObjectToSerialize">object to be serialized</param>
    public static void Save(object ObjectToSerialize)
    {
      Save(ObjectToSerialize, GenerateDefaultFilename());
    }
    /// <summary>
    /// Save an object as an XML file
    /// </summary>
    /// <param name="ObjectToSerialize">object to be serialized</param>
    /// <param name="SettingFileLocation">location (including filename) to save the XML</param>
    public static void Save(object ObjectToSerialize, string SettingFileLocation)
    {
      Save(ObjectToSerialize, SettingFileLocation, null);
    }
    /// <summary>
    /// Save an object as an XML file
    /// </summary>
    /// <param name="ObjectToSerialize">object to be serialized</param>
    /// <param name="SettingFileLocation">location (including filename) to save the XML</param>
    /// <param name="extraTypes">Derived types to be serialized</param>
    public static void Save(object ObjectToSerialize, string SettingFileLocation, Type[] extraTypes)
    {
      if (!DesignMode)
      {
        try
        {

          Type ObjectToSerializeType = ObjectToSerialize.GetType();
          XmlSerializer s;

          if (extraTypes == null)
          {
            s = new XmlSerializer(ObjectToSerializeType);
          }
          else
          {
            s = new XmlSerializer(ObjectToSerializeType, extraTypes);
          }

          if (!Directory.Exists(Path.GetDirectoryName(SettingFileLocation)))
            Directory.CreateDirectory(Path.GetDirectoryName(SettingFileLocation));
          TextWriter tw = new StreamWriter(SettingFileLocation);
          s.Serialize(tw, ObjectToSerialize);
          tw.Dispose();
        }
        catch (Exception ex)
        {
          //ExceptionHandler.ExceptionHandler.ShowException(ex, "Error saving file \"" + SettingFileLocation + "\"");
          throw new Exception("Error saving file \"" + SettingFileLocation + "\"", ex);
          //System.Windows.Forms.MessageBox.Show("Error: " + ex.Message + "\r\n\r\nStack Trace: " + ex.StackTrace);
        }
      }
    }

    /// <summary>
    /// Load an object from an XML file located at C:\ProgramData\Electroimpact\[ProgramName]\settings.xml.
    /// </summary>
    /// <typeparam name="T">Type of the object that the XML file will be loaded into.  This handles the casting.</typeparam>
    /// <returns></returns>
    public static T Load<T>()
    {
      return Load<T>(GenerateDefaultFilename(), null);
    }
    /// <summary>
    /// Load an object from an XML file.
    /// </summary>
    /// <typeparam name="T">Type of the object that the XML file will be loaded into.  This handles the casting.</typeparam>
    /// <param name="SettingFileLocation">Location of the XML file to load</param>
    /// <returns></returns>
    public static T Load<T>(string SettingFileLocation)
    {
      return Load<T>(SettingFileLocation, null);
    }

    /// <summary>
    /// Load an object from an XML file.
    /// </summary>
    /// <typeparam name="T">Type of the object that the XML file will be loaded into.  This handles the casting.</typeparam>
    /// <param name="SettingFileLocation">Location of the XML file to load</param>
    /// <param name="extraTypes">Derived types to be deserialized</param>
    /// <returns></returns>
    public static T Load<T>(string SettingFileLocation, Type[] extraTypes)
    {
      T retval = default(T);
      try
      {
        if (File.Exists(SettingFileLocation))
        {
          XmlSerializer s;
          if (extraTypes == null)
          {
            s = new XmlSerializer(typeof(T));
          }
          else
          {
            s = new XmlSerializer(typeof(T), extraTypes);
          }
          using (TextReader r = new StreamReader(SettingFileLocation))
          {
            object o = s.Deserialize(r);
            if (o is T)
              retval = (T)o;
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception("Error loading file \"" + SettingFileLocation + "\"", ex);
        //ExceptionHandler.ExceptionHandler.ShowException(ex, "Error loading file \"" + SettingFileLocation + "\"");
      }

      return retval;
    }

    /// <summary>
    /// Load an object from an XML formated string. The type of the object must match the object that was stored in the XML string
    /// </summary>
    /// <typeparam name="T">Type of the object that the XML file will be loaded into.  This handles the casting.</typeparam>
    /// <param name="xml">String from which the object will be loaded</param>
    /// <param name="extraTypes">Derived types to be serialized</param>
    /// <returns></returns>
    public static T LoadFromXmlString<T>(string xml, Encoding encoding = null, Type[] extraTypes = null)
    {
      if (encoding == null) encoding = ASCIIEncoding.Default;
      T retval = default(T);
      try
      {
        XmlSerializer s;
        if (extraTypes != null)
        {
          s = new XmlSerializer(typeof(T), extraTypes);
        }
        else
        {
          s = new XmlSerializer(typeof(T));
        }

        MemoryStream ms = new MemoryStream(encoding.GetBytes(xml));
        retval = (T)s.Deserialize(ms);
        ms.Close();
      }
      catch (Exception ex)
      {
        ex.Data.Add("Xml String", xml);
        throw new Exception("Error loading from XML string.  See data.", ex);
        //ExceptionHandler.ExceptionHandler.ShowException(ex, "Error loading from XML string.  See data.");

      }
      return retval;

    }


    /// <summary>
    /// Convert an object to an XML string.  Use this if you want to manually manage saving, or only use the object in memory.
    /// </summary>
    /// <param name="ObjectToSerialize">object to be serialized</param>
    /// <returns>XML string that contains the object information</returns>
    public static string ObjectToString(object ObjectToSerialize)
    {
      return ObjectToString(ObjectToSerialize, null);
    }
    /// <summary>
    /// Convert an object to an XML string.  Use this if you want to manually manage saving, or only use the object in memory.
    /// </summary>
    /// <param name="ObjectToSerialize">object to be serialized</param>
    /// <param name="extraTypes">Derived types to be serialized</param>
    /// <returns>XML string that contains the object information</returns>
    public static string ObjectToString(object ObjectToSerialize, Type[] extraTypes)
    {
      try
      {
        Type ObjectToSerializeType = ObjectToSerialize.GetType();

        XmlSerializer s;
        if (extraTypes != null)
        {
          s = new XmlSerializer(ObjectToSerializeType, extraTypes);
        }
        else
        {
          s = new XmlSerializer(ObjectToSerializeType);
        }

        MemoryStream ms = new MemoryStream();
        s.Serialize(ms, ObjectToSerialize);
        ms.Seek(0, SeekOrigin.Begin);
        StreamReader sr = new StreamReader(ms);
        string rv = sr.ReadToEnd();
        ms.Dispose();
        return rv;
      }
      catch (Exception ex)
      {
        throw ex;
        //ExceptionHandler.ExceptionHandler.ShowException(ex);
        //System.Windows.Forms.MessageBox.Show("Error: " + ex.Message + "\r\n\r\nStack Trace: " + ex.StackTrace);
        return null;
      }
    }

    /// <summary>
    /// Clone an object by serializing to XML.  Only clones public members.  Object must be XML serializable.  This is slow, but convienent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ObjectToClone"></param>
    /// <returns></returns>
    public static T Clone<T>(T ObjectToClone)
    {
      string xml = ObjectToString(ObjectToClone);
      return LoadFromXmlString<T>(xml, System.Text.UTF8Encoding.Default);
    }

    /// <summary>
    /// Returns a string to a default file path for saving XML files. Form: C:\ProgramData\Electroimpact\[ProgramName]\settings.xml
    /// </summary>
    /// <returns>C:\ProgramData\Electroimpact\[ProgramName]\settings.xml</returns>
    public static string GenerateDefaultFilename()
    {
      string programFolder = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
      return GenerateDefaultFilename("Electroimpact", programFolder);
    }

    /// <summary>
    /// Returns a string to a default file path for saving XML files. Form: C:\ProgramData\[companyFolderName]\[programFolderName]\settings.xml
    /// </summary>
    /// <param name="companyFolderName"></param>
    /// <param name="programFolderName"></param>
    /// <returns>C:\ProgramData\[companyFolderName]\[programFolderName]\settings.xml</returns>
    public static string GenerateDefaultFilename(string companyFolderName, string programFolderName)
    {
      return GenerateDefaultFilename(companyFolderName, programFolderName, "settings.xml");
    }

    /// <summary>
    /// Returns a string to a default file path for saving XML files. Form: C:\ProgramData\[companyFolderName]\[programFolderName]\[FileName]
    /// </summary>
    /// <param name="companyFolderName"></param>
    /// <param name="programFolderName"></param>
    /// <param name="FileName"></param>
    /// <returns>C:\ProgramData\[companyFolderName]\[programFolderName]\[FileName]</returns>
    public static string GenerateDefaultFilename(string companyFolderName, string programFolderName, string FileName)
    {
      return GenerateDefaultFilename(companyFolderName, programFolderName, FileName, true);
    }

    /// <summary>
    /// Returns a string to a default file path for saving XML files. Form: C:\[ProgramData or ...\AppData]\[companyFolderName]\[programFolderName]\[FileName]
    /// </summary>
    /// <param name="companyFolderName"></param>
    /// <param name="programFolderName"></param>
    /// <param name="FileName"></param>
    /// <param name="computerWide">If true the file will be in ProgramData, if false it will be in AppData (user specific)</param>
    /// <returns>C:\[ProgramData or ...\AppData]\[companyFolderName]\[programFolderName]\[FileName]</returns>
    public static string GenerateDefaultFilename(string companyFolderName, string programFolderName, string FileName, bool computerWide)
    {
      string filename;

      if (computerWide)
      {
        filename = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        //set permissions
        string tf = Path.Combine(filename, companyFolderName);
        //setDirectorySecurity(tf);
        tf = Path.Combine(filename, programFolderName);
        //setDirectorySecurity(tf);
      }
      else
        filename = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

      filename = Path.Combine(filename, companyFolderName);
      filename = Path.Combine(filename, programFolderName);
      filename = Path.Combine(filename, FileName);
      return filename;

    }



    public static bool DesignMode
    {

      get
      {

        return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");

      }

    }

    static void SetDirectorySecurity(string directory)
    {
      if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

      System.Security.Principal.SecurityIdentifier securityIdentifier =
        new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null);

      DirectoryInfo directoryInfo = new DirectoryInfo(directory);
      System.Security.AccessControl.DirectorySecurity directorySecurity;
      directorySecurity = directoryInfo.GetAccessControl();
      directorySecurity.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(
        securityIdentifier, System.Security.AccessControl.FileSystemRights.FullControl,
        System.Security.AccessControl.InheritanceFlags.ContainerInherit |
        System.Security.AccessControl.InheritanceFlags.ObjectInherit,
        System.Security.AccessControl.PropagationFlags.None,
        System.Security.AccessControl.AccessControlType.Allow));
      directoryInfo.SetAccessControl(directorySecurity);
    }
  }

  #endregion
}
