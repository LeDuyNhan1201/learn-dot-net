namespace BuildingBlocks.Application.DataSeeders;

public interface IUserSeeder
{
    IEnumerable<string> InitAdministrators();
    IEnumerable<string> InitCustomers();
    IEnumerable<string> InitUserRoles();
}