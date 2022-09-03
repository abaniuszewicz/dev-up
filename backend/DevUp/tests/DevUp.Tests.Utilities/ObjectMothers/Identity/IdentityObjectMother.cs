namespace DevUp.Tests.Utilities.ObjectMothers.Identity
{
    public static class IdentityObjectMother
    {
        public static IIdentityObjectMother JohnCena => new JohnCenaIdentityObjectMother();
        public static IIdentityObjectMother SerenaWilliams => new SerenaWilliamsIdentityObjectMother();
        public static IIdentityObjectMother Unused => new UnusedIdentityObjectMother();
    }
}
