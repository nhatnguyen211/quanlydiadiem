﻿using BELibrary.Entity;

namespace BELibrary.Core.Entity.Repositories
{
    //this.Configuration.LazyLoadingEnabled = false;
    public interface IAccountRepository : IRepository<Account>
    {
        Account ValidBEAccount(string username, string password);

        Account ValidFeAccount(string username, string password);

        Account GetAccountByUsername(string email);

        Account GetAccountFeByUsername(string email);
    }
}