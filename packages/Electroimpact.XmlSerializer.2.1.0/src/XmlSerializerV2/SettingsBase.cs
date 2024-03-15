using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Electroimpact.XmlSerialization
{
  public abstract class SettingsBase<TSettingType> where TSettingType : SettingsBase<TSettingType>
  {
    [XmlIgnore]
    public string SettingsFullFilePath = null;

    public void SaveSettings()
    {
      if (SettingsFullFilePath == null) throw new Exception("A save location has not been specified");
      Serializer.Save(this, SettingsFullFilePath);
    }
    public TSettingType LoadSettings(bool GenerateIfNotExists = true, bool SaveOnGenerate = true)
    {
      return LoadSettings(null, GenerateIfNotExists, SaveOnGenerate);
    }
    public TSettingType LoadSettings(object[] ctorParameters, bool GenerateIfNotExists = true, bool SaveOnGenerate = true)
    {
      TSettingType rv = default(TSettingType);
      if (SettingsFullFilePath == null) throw new Exception("A save location has not been specified");

      if (!System.IO.File.Exists(SettingsFullFilePath) )
      {
        if (GenerateIfNotExists)
        {
          if (ctorParameters == null)
          {
            //Serialization requires a parameterless constructor.  So this should always work.
            rv = (TSettingType)Activator.CreateInstance(typeof(TSettingType));
          }
          else
          {
            rv = (TSettingType)Activator.CreateInstance(typeof(TSettingType), ctorParameters);
          }

          if (SaveOnGenerate)
          {
            SaveSettings();
          }
        }
      }
      else
      {
        rv = Serializer.Load<TSettingType>(SettingsFullFilePath);
      }
      rv.SettingsFullFilePath = this.SettingsFullFilePath;

      return rv;
    }


  }
}
