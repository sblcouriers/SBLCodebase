namespace Sbl.Helpers
{
    public class WebConstant
    {
        public const double OwnVechicleDeductAmount = 5;
        public const string BackSlash = "/";
        public const string Ad1 = "AD1";
        public const string Ad2 = "AD2";
        public const string Ad3 = "AD3";
        public const string Deduct = "Deduct";
        public const string LitresAndMileage = "Litres & Mileage";

        public class ApiConstant
        {
            public const string API_KEY_HEADER = "X-ApiKey";
            public const string API_KEY_VALUE = "AZXYAWQAMCRT12S2";
            public const string JSON_CONTENT_TYPE = "application/json";
            public const string PASSWORD_VALUE = "flvn@sbl2018";
            public const string USERNAME_VALUE = "sblWebApi";
        }

        public class DateTimeFormat
        {
            public const string FormatDateddMMYY = "dd/MM/yy";
            public const string FormatDateddMMyyyy = "dd/MM/yyyy";
            public const string FullDateFormat = "g";
            public const string DateFormat = "MM/dd/yyyy";
            public const string DateFormatJs = "mm/dd/yy";
            public const string DateFormatForInvoice = "ddMMyy";
        }
        public class EmailTemplate
        {
            public const string MailTemplate = "mailTemplate:";
            public const string TemplateReplaceFormat = "[[{0}]]";
            public class EmailKey
            {
                public const string WeekelyRemittanceDetail = "WeekelyRemittanceDetail";
                public const string WeekDateRange = "WeekDateRange";
                public const string Name = "Name";
                public const string LastName = "LastName";
                public const string Email = "Email";
                public const string WebSiteUrl = "WebSiteUrl";
                public const string WeekNumber = "WeekNumber";
            }
        }
        public class SBLUserRole
        {
            public const string POC = "poc";
            public const string Payroll = "payroll";
            public const string Admin = "admin";
            public const string Master = "master";
            public const string Fleet = "fleet";
            public const string Recruitment = "recruitment";
            public const string Driver = "driver";
        }

        public class SBLUserRoleId
        {
            public const string POCRoleId = "1b616909-af6f-425e-9362-ccbd8460c3a9";
            public const string PayrollRoleId = "b8381aae-4a43-4ab3-bcd6-69dc562599e1";
            public const string AdminRoleId = "e22d87bd-b2ad-4e0e-85d5-ac329374c692";
            public const string MasterRoleId = "4724cf81-3039-4e9d-9e7b-a182d3d4a137";
            public const string FleetRoleId = "ef76d4ec-fcc2-4b6c-ad4c-49be6e760041";
            public const string RecruitmentRoleId = "e71aabd0-7714-4fb1-b8fe-95039b70ba77";
            public const string DriverRoleId = "084AF7CF-6F0C-497E-B75C-53C89F6BEEA9";
        }
    }//end class
}//end namespace