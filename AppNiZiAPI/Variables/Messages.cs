namespace AppNiZiAPI.Variables
{
    public static class Messages
    {
        // Errors
        public const string ErrorServer = "Server Time Out";
        public const string ErrorPostBody = "Incorrect values";
        public const string ErrorMissingValues = "Missing values";

        public const string ErrorIncorrectId = "Incorrect id";

        // Consumption
        public const string ErrorInvalidDateValues = "Invalid date value(s). Date format: yyyy-MM-dd";
        public const string ErrorInvalidConsumptionObject = "Invalid Consumption object. Make sure all fields are present and have valid values";

        public const string ErrorDelete = "Removing Data Failed";
        public const string ErrorPost = "Inserting Data Failed";
        public const string ErrorPut = "Updating Data Failed";

        // Auth
        public const string AuthNoAcces = "Authorization Error: No acces";
        public const string AuthLogIn = "Authification Error: Log in to use the API";

        // OK Messages
        public const string OKResult = "Result: OK";
        public const string OKPost = "Inserting Data Successfully";
        public const string OKUpdate = "Updating Data Successfully";
        public const string OKDelete = "Removing Data Successfully";
    }
}
