namespace DevUp.Infrastructure.Postgres.Identity.Dtos
{
    internal record DeviceDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
