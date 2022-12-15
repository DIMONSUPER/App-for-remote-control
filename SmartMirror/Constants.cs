namespace SmartMirror
{
    public static class Constants
    {
        public const string DATABASE_NAME = "SmartMirror.db3";

        public static class Amazon
        {
            public const string API_URL = "https://api.amazon.com";
            public const string PRODUCT_ID = "smart_mirror";
            public const string PRODUCT_DSN = "123456";
        }

        public static class DialogsParameterKeys
        {
            public const string RESULT = nameof(RESULT);
            public const string ACCESSORY = nameof(ACCESSORY);
            public const string SCENARIO = nameof(SCENARIO);
            public const string CAMERA = nameof(CAMERA);
            public const string AUTOMATION = nameof(AUTOMATION);
            public const string NOTIFICATION = nameof(NOTIFICATION);
            public const string AUTH_TYPE = nameof(AUTH_TYPE);
            public const string TITLE = nameof(TITLE);
            public const string DESCRIPTION = nameof(DESCRIPTION);
            public const string IP_ADDRESS = nameof(IP_ADDRESS);
            public const string PASSWORD = nameof(PASSWORD);
            public const string LOGIN = nameof(LOGIN);
            public const string NAME = nameof(NAME);
            public const string CONFIRM_ACTION = nameof(CONFIRM_ACTION);
            public const string CONFIRM_TEXT = nameof(CONFIRM_TEXT);
            public const string CANCEL_TEXT = nameof(CANCEL_TEXT);
        }

        public static class Google
        {
            public const string NEST_SERVICES_URL = $"https://nestservices.google.com/partnerconnections/";
            public const string CLIENT_ID = "584828440898-arffsu8ltmg71vhvctmcd6h5bdg8gofe.apps.googleusercontent.com";
            public const string PROJECT_ID = "1f4c1129-0b4d-4b64-bc65-ddd682d036b8";
            public const string WEB_CLIENT_ID = "584828440898-ud6okjt7qb6qrlspbmr55i90s6as8o90.apps.googleusercontent.com";
            public const string WEB_CLIENT_SECRET = "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";
            public const string API_KEY = "AIzaSyAYOtf5x02-DWhKyrEJwWstxg0ZZy7shZ4";
            public const string SDM_SERVICE_SCOPE = "https://www.googleapis.com/auth/sdm.service";
            public const string REDIRECT_URI = "https://smartmirror.page.link/open";
        }

        public static class Aqara
        {
            public const string API_URL = "https://open-usa.aqara.com/v3.0/open/api";
            public const string TEST_EMAIL = "botheadworks@gmail.com";
            //public const string TEST_EMAIL = "dmytro.fedchenko@headworks.com.ua";

            //botheadworks@gmail.com
            //public const string APP_ID = "1019974506077405184fac3b";
            //public const string APP_KEY = "gvnepmdyciqbb4ob7rocp6nnxt9elxmf";
            //public const string KEY_ID = "K.1019974506438115328";

            //dmytro.fedchenko@headworks.com.ua
            public const string APP_ID = "10424569422108057604ba21";
            public const string APP_KEY = "rf9bq7v5f118s8v2balwfsatqgwhz8wz";
            public const string KEY_ID = "K.1042456943007723522";

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

            public static class Models
            {
                public const string APP_TIMER_V1 = "app.timer.v1";
                public const string APP_IFTTT_V1 = "app.ifttt.v1";
                public const string APP_IFTTT_POSITION_USER_ALERT = "app.ifttt.position_user_alert";
                public const string APP_GEOFENCE_TRIGGER = "app.geofence.trigger";
                public const string APP_WEATHER_HUMIDITY = "app.weather.humidity";
                public const string APP_WEATHER_SUN = "app.weather.sun";
                public const string APP_WEATHER_TEMPERATURE = "app.weather.temperature";
                public const string APP_WEATHER_PHENOMENON = "app.weather.phenomenon";
                public const string APP_GEOFENCE_FORECAST = "app.geofence.forecast";
                public const string APP_SCENE_V1 = "app.scene.v1";
                public const string APP_MOBILEPUSH_V1 = "app.mobilepush.v1";
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
            public const string TIME_FORMAT = "HH:mm";
            public const string TIME_FORMAT_2 = "{}{0:HH:mm}";
        }

        public static class Limits
        {
            public const int TIME_TO_ATTEMPT_UPDATE_IN_SECONDS = 15;
            public const int DELAY_MILLISEC_NAVIGATION_COMMAND = 500;
            public const int CAMERA_TIME_CHECK_SECONDS = 5;
            public const int CAMERA_NAME_MAX_LENGTH = 32;
        }

        public static class Analytics
        {
            public const string AndroidKey = "8a361957-36a1-4bf0-a252-f03a47119eca";
        }

        public static class Rings
        {
            public const string DOORBELL = "doorbell.mp3";
        }
    }
}