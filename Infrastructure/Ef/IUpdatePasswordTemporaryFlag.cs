namespace Infrastructure.Ef;

public interface IUpdatePasswordTemporaryFlag
{
    Task Process(string userId);
}