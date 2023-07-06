namespace DataAccess.Xml.Tests
{
    public interface ITestFixture<TContext>
    {
        TContext CreateContext();
    }
}