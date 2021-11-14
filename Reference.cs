namespace CSVIngestion
{
    public static class Reference
    {
        public static string inputFolderPath = "EnrollmentInput";

        public enum Enrollment_Format {
            User_Id = 0,
            First_Name = 1,
            Last_Name = 2,
            Version = 3,
            Insurance_Company = 4
        }
    }
}