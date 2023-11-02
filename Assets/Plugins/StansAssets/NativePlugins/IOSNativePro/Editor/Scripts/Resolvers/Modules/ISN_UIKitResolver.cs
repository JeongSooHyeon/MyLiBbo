using SA.iOS.XCode;

namespace SA.iOS 
{
    public class ISN_UIKitResolver : ISN_LSApplicationQueriesSchemesResolver
    {
        protected override ISN_XcodeRequirements GenerateRequirements() 
        {
            var requirements = new ISN_XcodeRequirements();
            var property = new ISD_BuildProperty("GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
            requirements.AddBuildProperty(property);
            
            if (ISN_Settings.Instance.ApplicationQueriesSchemes.Count > 0) 
            {
                var LSApplicationQueriesSchemes = new ISD_PlistKey();
                LSApplicationQueriesSchemes.Name = "LSApplicationQueriesSchemes";
                LSApplicationQueriesSchemes.Type = ISD_PlistKeyType.Array;

                requirements.AddInfoPlistKey(LSApplicationQueriesSchemes);

                foreach (var scheme in ISN_Settings.Instance.ApplicationQueriesSchemes) 
                {
                    var schemeName = new ISD_PlistKey();
                    schemeName.StringValue = scheme.Identifier;
                    schemeName.Type = ISD_PlistKeyType.String;
                    LSApplicationQueriesSchemes.AddChild(schemeName);
                }
            }

            var settings = ISN_Settings.Instance;
            ResolvePlistKey(settings.CameraUsageDescriptionEnabled, 
                "NSCameraUsageDescription",
                settings.CameraUsageDescription, requirements);
            
            ResolvePlistKey(settings.PhotoLibraryUsageDescriptionEnabled, 
                "NSPhotoLibraryUsageDescription",
                settings.PhotoLibraryUsageDescription, requirements);
            
            ResolvePlistKey(settings.PhotoLibraryAddUsageDescriptionEnabled, 
                "NSPhotoLibraryAddUsageDescription",
                settings.PhotoLibraryAddUsageDescription, requirements);
            
            ResolvePlistKey(settings.MicrophoneUsageDescriptionEnabled, 
                "NSMicrophoneUsageDescription",
                settings.MicrophoneUsageDescription, requirements);

            return requirements;
        }

        private void ResolvePlistKey(bool isEnabled, string name, string value, ISN_XcodeRequirements requirements)
        {
            if (isEnabled)
            {
                var plistKey = new ISD_PlistKey();
                plistKey.Name = name;
                plistKey.StringValue = value;
                plistKey.Type = ISD_PlistKeyType.String;
                requirements.AddInfoPlistKey(plistKey);
            }
            else
            {
                ISD_API.RemoveInfoPlistKey(name);
            }
        }
        
        public override bool IsSettingsEnabled { get { return true; } set { } }
        protected override string LibFolder { get { return string.Empty; } }
        public override string DefineName { get { return string.Empty; } }
    }
}

