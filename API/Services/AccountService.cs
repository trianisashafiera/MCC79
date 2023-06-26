﻿using API.Contracts;
using API.DTOs.Accounts;
using API.Models;

namespace API.Services;

    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public IEnumerable<NewAccountDto> GetAccount()
        {
            var accounts = _accountRepository.GetAll();
            if (!accounts.Any())
            {
                return null;
            }
            var toDto = accounts.Select(account =>
                                               new NewAccountDto
                                               {
                                                   Guid = account.Guid,
                                                   IsDeleted = account.IsDeleted,
                                                   IsUsed = account.IsUsed
                                               }).ToList();

            return toDto;

        }
        public NewAccountDto? GetAccount(Guid guid)
        {
            var account = _accountRepository.GetByGuid(guid);
            if (account is null)
            {
                return null; // Booking not found
            }

            var toDto = new NewAccountDto
            {
                Guid = account.Guid,
                IsDeleted = account.IsDeleted,
                IsUsed = account.IsUsed
            };

            return toDto; // Booking found
        }

        public NewAccountDto? CreateAccount(NewAccountDto newAccountDto)
        {
            var account = new Account
            {
                Guid = new Guid(),
                IsDeleted = newAccountDto.IsDeleted,
                IsUsed = newAccountDto.IsUsed,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            var createdAccount = _accountRepository.Create(account);
            if (createdAccount is null)
            {
                return null; // Booking not created
            }

            var toDto = new NewAccountDto
            {
                Guid = createdAccount.Guid,
                IsDeleted = createdAccount.IsDeleted,
                IsUsed = createdAccount.IsUsed
            };

            return toDto; // Booking created
        }

        public int UpdateAccount(NewAccountDto updateAccountDto)
        {
            var isExist = _accountRepository.IsExist(updateAccountDto.Guid);
            if (!isExist)
            {
                // Booking not found
                return -1;
            }

            var getAccount = _accountRepository.GetByGuid(updateAccountDto.Guid);

            var account = new Account
            {
                Guid = updateAccountDto.Guid,
                IsUsed = updateAccountDto.IsUsed,
                IsDeleted = updateAccountDto.IsDeleted,
                ModifiedDate = DateTime.Now,
                CreatedDate = getAccount!.CreatedDate
            };

            var isUpdate = _accountRepository.Update(account);
            if (!isUpdate)
            {
                return 0; // Booking not updated
            }

            return 1;
        }

        public int DeleteAccount(Guid guid)
        {
            var isExist = _accountRepository.IsExist(guid);
            if (!isExist)
            {
                return -1; // Booking not found
            }

            var account = _accountRepository.GetByGuid(guid);
            var isDelete = _accountRepository.Delete(account!);
            if (!isDelete)
            {
                return 0; // Booking not deleted
            }

            return 1;
        }

        
    }

