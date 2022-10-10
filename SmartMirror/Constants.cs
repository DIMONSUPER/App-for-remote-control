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

        public static class Formats
        {
            public const string DATE_FORMAT = "MM.dd.yyyy";
            public const string TIME_FORMAT = "hh:mm";
            public const string TIME_FORMAT_2 = "{}{0:HH:mm}";
        }
    }
}