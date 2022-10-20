namespace SmartMirror
{
    public static class Constants
    {
        public static class Amazon
        {
            public const string API_URL = "https://api.amazon.com";
            public const string PRODUCT_ID = "smart_mirror";
            public const string PRODUCT_DSN = "123456";
        }

        public static class DialogsParameterKeys
        {
            public const string TITLE = nameof(TITLE);
            public const string DESCRIPTION = nameof(DESCRIPTION);
        }
        
        public static class Aqara
        {
            public const string API_URL = "https://open-usa.aqara.com/v3.0/open/api";
            public const string APP_ID = "1019974506077405184fac3b";
            public const string APP_KEY = "gvnepmdyciqbb4ob7rocp6nnxt9elxmf";
            public const string KEY_ID = "K.1019974506438115328";

            public static class AttibutesId
            {
                public const string SWITCH_CHANNEL_0_STATUS = "4.1.85";
                public const string SWITCH_CHANNEL_1_STATUS = "4.2.85";
                public const string SWITCH_CHANNEL_2_STATUS = "4.3.85";
                public const string HUMIDITY_STATUS = "0.2.85";
                public const string TEMPERATURE_STATUS = "0.1.85";
                public const string AIR_PRESSURE_STATUS = "0.3.85";
            }
        }

        public static class Formats
        {
            public const string DATE_FORMAT = "MM.dd.yyyy";
            public const string TIME_FORMAT = "hh:mm";
            public const string TIME_FORMAT_2 = "{}{0:HH:mm}";
        }

        public static class Limits
        {
            public const int TIME_TO_ATTEMPT_UPDATE_IN_SECONDS = 15;
            public const int REFRESH_RATE_IN_MILISECONDS = 100;
        }
    }
}