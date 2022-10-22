namespace DevUp.Infrastructure.Postgres.Identity.Dtos
{
    internal record DeviceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
