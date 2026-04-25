namespace MedicalClinicSystem.API.Authorization
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Receptionist = "Receptionist";
        public const string Doctor = "Doctor";

        public const string AdminOnly = Admin;
        public const string AdminOrReceptionist = $"{Admin},{Receptionist}";
        public const string AllStaff = $"{Admin},{Receptionist},{Doctor}";
        public const string AdminOrDoctor = $"{Admin},{Doctor}";
    }
}
