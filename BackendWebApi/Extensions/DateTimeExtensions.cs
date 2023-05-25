namespace BackendWebApi.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly dateofbirth)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var age = today.Year - dateofbirth.Year;

            if(dateofbirth > today.AddYears(-age)) age--;

            return age;
        }
    }
}