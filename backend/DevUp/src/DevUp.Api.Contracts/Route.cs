namespace DevUp.Api.Contracts
{
    public static class Route
    {
        public static class Api
        {
            public const string Url = "/api";

            public static class V1
            {
                public const string Url = Api.Url + "/v1";

                public static class Example
                {
                    public const string Url = V1.Url + "/example";
                }

                public static class Identity
                {
                    private const string Url = V1.Url + "/identity";

                    public const string Register = Url + "/register";
                    public const string Login = Url + "/login";
                    public const string Refresh = Url + "/refresh";
                }

                public static class Teams
                {
                    private const string Url = V1.Url + "/teams";

                    public const string Create = Url;
                    public const string GetAll = Url;
                    public const string GetById = Url + "/{teamId:guid}";
                    public const string Update = Url + "/{teamId:guid}";
                    public const string Delete = Url + "/{teamId:guid}";
                }
            }
        }
    }
}
