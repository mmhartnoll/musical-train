namespace DataAccess.Xml.Tests
{
    public abstract class IntegrationTest<TTestContext, TTestContextFixture> where TTestContextFixture : ITestFixture<TTestContext>
    {
        public TTestContextFixture Fixture { get; private init; }

        public IntegrationTest(TTestContextFixture fixture)
            => Fixture = fixture;
    }
}