namespace MedicalClinicSystem.Infrastructure.Persistence
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}