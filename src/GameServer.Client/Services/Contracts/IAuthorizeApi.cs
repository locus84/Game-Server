using GameServer.SharedKernel;

namespace GameServer.Client.Services.Contracts;

  public interface IAuthorizeApi
  {
      Task Login(LoginParameters loginParameters);
      Task Register(RegisterParameters registerParameters);
      Task Logout();
      Task<UserInfo?> GetUserInfo();
  }
