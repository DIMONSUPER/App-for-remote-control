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
            public const string RESULT = nameof(RESULT);
            public const string AUTH_TYPE = nameof(AUTH_TYPE);
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
                public const string MOTION_STATUS = "3.1.85";
                public const string LUX_STATUS = "0.3.85";
                public const string ILLUMINATION_STATUS = "0.4.85";
            }

            public static class EventTypes
            {
                //Documentation: https://opendoc.aqara.cn/en/docs/developmanual/messagePush/messagePushFormat.html
                public const string gateway_bind = nameof(gateway_bind);
                public const string subdevice_bind = nameof(subdevice_bind);
                public const string gateway_unbind = nameof(gateway_unbind);
                public const string unbind_sub_gw = nameof(unbind_sub_gw);
                public const string gateway_online = nameof(gateway_online);
                public const string gateway_offline = nameof(gateway_offline);
                public const string subdevice_online = nameof(subdevice_online);
                public const string subdevice_offline = nameof(subdevice_offline);
                public const string dev_name_change = nameof(dev_name_change);
                public const string dev_position_assign = nameof(dev_position_assign);
                public const string resource_alias_changed = nameof(resource_alias_changed);
                public const string linkage_created = nameof(linkage_created);
                public const string scene_created = nameof(scene_created);
                public const string event_created = nameof(event_created);
                public const string linkage_deleted = nameof(linkage_deleted);
                public const string scene_deleted = nameof(scene_deleted);
                public const string event_deleted = nameof(event_deleted);
                public const string resource_report = nameof(resource_report);
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
        }

        public static class Analytics
        {
            public const string AndroidKey = "8a361957-36a1-4bf0-a252-f03a47119eca";
        }
    }
}